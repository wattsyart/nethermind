//  Copyright (c) 2021 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethermind.Blockchain.Synchronization;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Specs;
using Nethermind.Logging;
using Nethermind.Network.P2P.EventArg;
using Nethermind.Network.P2P.Messages;
using Nethermind.Network.P2P.ProtocolHandlers;
using Nethermind.Network.P2P.Subprotocols.Eth.V62;
using Nethermind.Network.P2P.Subprotocols.Eth.V62.Messages;
using Nethermind.Network.P2P.Subprotocols.Eth.V65;
using Nethermind.Network.P2P.Subprotocols.Eth.V66;
using Nethermind.Network.P2P.Subprotocols.Snap.Messages;
using Nethermind.Network.Rlpx;
using Nethermind.Serialization.Rlp;
using Nethermind.State.Snap;
using Nethermind.Stats;
using Nethermind.Stats.Model;
using Nethermind.Synchronization;
using Nethermind.Synchronization.SnapSync;
using Nethermind.TxPool;

namespace Nethermind.Network.P2P.Subprotocols.Snap
{
    public class SnapProtocolHandler : ZeroProtocolHandlerBase, ISnapSyncPeer
    {
        private const int BYTES_LIMIT = 1000_000;

        public override string Name => "snap1";
        protected override TimeSpan InitTimeout => Timeouts.Eth;

        public override byte ProtocolVersion => 1;
        public override string ProtocolCode => Protocol.Snap;
        public override int MessageIdSpaceSize => 8;

        private readonly ISyncServer _syncServer;
        private readonly ILogger _logger;
        private readonly MessageQueue<GetAccountRangeMessage, AccountRangeMessage> _getAccountRangeRequests;
        private readonly MessageQueue<GetStorageRangeMessage, StorageRangeMessage> _getStorageRangeRequests;
        private readonly MessageQueue<GetByteCodesMessage, ByteCodesMessage> _getByteCodesRequests;

        public SnapProtocolHandler(ISession session,
            INodeStatsManager nodeStats,
            IMessageSerializationService serializer,
            ISyncServer syncServer,
            ILogManager logManager)
            : base(session, nodeStats, serializer, logManager)
        {
            _syncServer = syncServer  ?? throw new ArgumentNullException(nameof(syncServer));
            _logger = logManager.GetClassLogger();
            
            _getAccountRangeRequests = new(Send);
            _getStorageRangeRequests = new(Send);
            _getByteCodesRequests = new(Send);
        }

        public override event EventHandler<ProtocolInitializedEventArgs> ProtocolInitialized;
        public override event EventHandler<ProtocolEventArgs>? SubprotocolRequested
        {
            add { }
            remove { }
        }

        public override void Init()
        {
            ProtocolInitialized?.Invoke(this, new ProtocolInitializedEventArgs(this));
        }

        public override void Dispose()
        {
        }

        public override void HandleMessage(ZeroPacket message)
        {
            int size = message.Content.ReadableBytes;

            switch (message.PacketType)
            {
                case SnapMessageCode.GetAccountRange:
                    GetAccountRangeMessage getAccountRangeMessage = Deserialize<GetAccountRangeMessage>(message.Content);
                    ReportIn(getAccountRangeMessage);
                    Handle(getAccountRangeMessage);
                    break;
                case SnapMessageCode.AccountRange:
                    AccountRangeMessage accountRangeMessage = Deserialize<AccountRangeMessage>(message.Content);
                    ReportIn(accountRangeMessage);
                    Handle(accountRangeMessage, size);
                    break;
                case SnapMessageCode.GetStorageRanges:
                    GetStorageRangeMessage getStorageRangesMessage = Deserialize<GetStorageRangeMessage>(message.Content);
                    ReportIn(getStorageRangesMessage);
                    Handle(getStorageRangesMessage);
                    break;
                case SnapMessageCode.StorageRanges:
                    StorageRangeMessage storageRangesMessage = Deserialize<StorageRangeMessage>(message.Content);
                    ReportIn(storageRangesMessage);
                    Handle(storageRangesMessage, size);
                    break;
                case SnapMessageCode.GetByteCodes:
                    GetByteCodesMessage getByteCodesMessage = Deserialize<GetByteCodesMessage>(message.Content);
                    ReportIn(getByteCodesMessage);
                    Handle(getByteCodesMessage);
                    break;
                case SnapMessageCode.ByteCodes:
                    ByteCodesMessage byteCodesMessage = Deserialize<ByteCodesMessage>(message.Content);
                    ReportIn(byteCodesMessage);
                    Handle(byteCodesMessage, size);
                    break;
                case SnapMessageCode.GetTrieNodes:
                    GetTrieNodesMessage getTrieNodesMessage = Deserialize<GetTrieNodesMessage>(message.Content);
                    ReportIn(getTrieNodesMessage);
                    //Handle(msg);
                    break;
                case SnapMessageCode.TrieNodes:
                    TrieNodesMessage trieNodesMessage = Deserialize<TrieNodesMessage>(message.Content);
                    ReportIn(trieNodesMessage);
                    //Handle(msg);
                    break;
            }
        }

        private void Handle(AccountRangeMessage msg, long size)
        {
            Metrics.SnapAccountRangeReceived++;
            _getAccountRangeRequests.Handle(msg, size);
        }

        private void Handle(StorageRangeMessage msg, long size)
        {
            Metrics.SnapStorageRangesReceived++;
            _getStorageRangeRequests.Handle(msg, size);
        }

        private void Handle(ByteCodesMessage msg, long size)
        {
            // TODO: increment metrics
            _getByteCodesRequests.Handle(msg, size);
        }

        private void Handle(GetAccountRangeMessage msg)
        {
            _logger.Info($"received GetAccountRangeMessage.");
            // _logger.Info($"received GetAccountRangeMessage. id: {msg.RequestId}, responseLimit: {msg.ResponseBytes}, range start: {msg.AccountRange.StartingHash}, range end: {msg.AccountRange.LimitHash}");
            IEnumerable<KeyValuePair<byte[], byte[]>>? accountsRange = _syncServer.GetAccountsRange(msg.AccountRange.StartingHash, msg.AccountRange.LimitHash ?? Keccak.MaxValue, msg.ResponseBytes);
            _logger.Info($"collected accountsRange");

            List<PathWithAccount> pathWithAccounts = new();
            foreach ((byte[]? path, byte[]? account) in accountsRange.ToArray())
            {
                pathWithAccounts.Add(new PathWithAccount(new Keccak(path), Rlp.Decode<Account>(account)));
            }
            _logger.Info($"collected pathWithAccounts. number: {pathWithAccounts.Count}");

            try
            {
                // AccountRangeMessage accountRangeMessage = new();
                // accountRangeMessage.RequestId = msg.RequestId;
                // accountRangeMessage.PathsWithAccounts = pathWithAccounts.Count == 0
                //     ? Array.Empty<PathWithAccount>()
                //     : pathWithAccounts.ToArray();
                // accountRangeMessage.Proofs = Array.Empty<byte[]>();

                // _logger.Info($"sending AccountRangeMessage. id: {accountRangeMessage.RequestId}, acc number: {accountRangeMessage.PathsWithAccounts.Length}, first path:{accountRangeMessage.PathsWithAccounts.First().AddressHash}, last path: {accountRangeMessage.PathsWithAccounts.Last().AddressHash}");
                _logger.Info("sending msg");
                PingMessage accountRangeMessage = PingMessage.Instance;
                Send(accountRangeMessage);
            }
            catch(Exception e)
            {
                _logger.Warn($"caught exception when creating acc msg: {e}");
                _logger.Info((new StackTrace()).ToString());
            }
        }

        private void Handle(GetStorageRangeMessage getStorageRangesMessage)
        {
            throw new NotImplementedException();
        }

        private void Handle(GetByteCodesMessage getByteCodesMessage)
        {
            throw new NotImplementedException();
        }

        public override void DisconnectProtocol(DisconnectReason disconnectReason, string details)
        {
            Dispose();
        }

        public async Task<AccountsAndProofs> GetAccountRange(AccountRange range, CancellationToken token)
        {
            var request = new GetAccountRangeMessage()
            {
                AccountRange = range,
                ResponseBytes = BYTES_LIMIT
            };

            AccountRangeMessage response = await SendRequest(request, _getAccountRangeRequests, token);

            return new AccountsAndProofs() { PathAndAccounts = response.PathsWithAccounts, Proofs = response.Proofs };
        }

        public async Task<SlotsAndProofs> GetStoragetRange(StorageRange range, CancellationToken token)
        {
            var request = new GetStorageRangeMessage()
            {
                StoragetRange = range,
                ResponseBytes = BYTES_LIMIT
            };

            StorageRangeMessage response = await SendRequest(request, _getStorageRangeRequests, token);

            return new SlotsAndProofs() { PathsAndSlots = response.Slots, Proofs = response.Proofs };
        }

        public async Task<byte[][]> GetByteCodes(Keccak[] codeHashes, CancellationToken token)
        {
            var request = new GetByteCodesMessage()
            {
                Hashes = codeHashes,
                Bytes = BYTES_LIMIT
            };

            ByteCodesMessage response = await SendRequest(request, _getByteCodesRequests, token);

            return response.Codes;
        }

        private async Task<Tout> SendRequest<Tin, Tout>(Tin msg, MessageQueue<Tin, Tout> _requestQueue, CancellationToken token)
            where Tin : SnapMessageBase
            where Tout : SnapMessageBase
        {
            Request<Tin, Tout> batch = new(msg);

            _requestQueue.Send(batch);

            Task<Tout> task = batch.CompletionSource.Task;

            using CancellationTokenSource delayCancellation = new();
            using CancellationTokenSource compositeCancellation
                = CancellationTokenSource.CreateLinkedTokenSource(token, delayCancellation.Token);
            Task firstTask = await Task.WhenAny(task, Task.Delay(Timeouts.Eth, compositeCancellation.Token));
            if (firstTask.IsCanceled)
            {
                token.ThrowIfCancellationRequested();
            }

            if (firstTask == task)
            {
                delayCancellation.Cancel();
                long elapsed = batch.FinishMeasuringTime();
                long bytesPerMillisecond = (long)((decimal)batch.ResponseSize / Math.Max(1, elapsed));
                if (Logger.IsTrace)
                    Logger.Trace($"{this} speed is {batch.ResponseSize}/{elapsed} = {bytesPerMillisecond}");
                StatsManager.ReportTransferSpeedEvent(Session.Node, TransferSpeedType.NodeData, bytesPerMillisecond);

                return task.Result;
            }

            StatsManager.ReportTransferSpeedEvent(Session.Node, TransferSpeedType.NodeData, 0L);
            throw new TimeoutException($"{Session} Request timeout in {nameof(Tin)}");
        }
    }
}
