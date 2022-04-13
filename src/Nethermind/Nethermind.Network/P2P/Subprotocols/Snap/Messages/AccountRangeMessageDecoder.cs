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

using Nethermind.Serialization.Rlp;
using Nethermind.State.Snap;

namespace Nethermind.Network.P2P.Subprotocols.Snap.Messages;

public class AccountRangeMessageDecoder : IRlpValueDecoder<AccountRangeMessage>, IRlpStreamDecoder<AccountRangeMessage>
{
    private readonly AccountDecoder _decoder = new (true);

    public Rlp Encode(AccountRangeMessage? item, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
    {
        if (item is null)
        {
            return Rlp.OfEmptySequence;
        }
        
        RlpStream rlpStream = new(GetLength(item, rlpBehaviors));
        Encode(rlpStream, item, rlpBehaviors);
        return new Rlp(rlpStream.Data!);
    }
    
    public void Encode(RlpStream stream, AccountRangeMessage? message, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
    {
        if (message is null)
        {
            stream.EncodeNullObject();
            return;
        }

        (int contentLength, int pwasLength, int proofsLength) = GetLength(message);

        stream.StartSequence(contentLength);

        stream.Encode(message.RequestId);
        if (message.PathsWithAccounts == null || message.PathsWithAccounts.Length == 0)
        {
            stream.EncodeNullObject();
        }
        else
        {
            stream.StartSequence(pwasLength);
            for (int i = 0; i < message.PathsWithAccounts.Length; i++)
            {
                PathWithAccount pwa = message.PathsWithAccounts[i];

                int accountContentLength = _decoder.GetContentLength(pwa.Account);
                int pwaLength = Rlp.LengthOf(pwa.AddressHash) + Rlp.LengthOfSequence(accountContentLength);

                stream.StartSequence(pwaLength);
                stream.Encode(pwa.AddressHash);
                _decoder.Encode(pwa.Account, stream, accountContentLength);
            }
        }

        if (message.Proofs == null || message.Proofs.Length == 0)
        {
            stream.EncodeNullObject();
        }
        else
        {
            stream.StartSequence(proofsLength);
            for (int i = 0; i < message.Proofs.Length; i++)
            {
                stream.Encode(message.Proofs[i]);
            }
        }
    }

    public AccountRangeMessage Decode(ref Rlp.ValueDecoderContext decoderContext, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
    {
        throw new System.NotImplementedException();
    }

    public AccountRangeMessage Decode(RlpStream rlpStream, RlpBehaviors rlpBehaviors = RlpBehaviors.None)
    {
        throw new System.NotImplementedException();
    }

    public int GetLength(AccountRangeMessage accountRangeMessage, RlpBehaviors rlpBehaviors)
    {
        (int contentLength, int pwasLength, int proofsLength) = GetLength(accountRangeMessage);
        return Rlp.LengthOfSequence(contentLength);
    }
    
    private (int contentLength, int pwasLength, int proofsLength) GetLength(AccountRangeMessage message)
    {
        int contentLength = Rlp.LengthOf(message.RequestId);

        int pwasLength = 0;
        if (message.PathsWithAccounts == null || message.PathsWithAccounts.Length == 0)
        {
            pwasLength = 1;
        }
        else
        {
            for (int i = 0; i < message.PathsWithAccounts.Length; i++)
            {
                PathWithAccount pwa = message.PathsWithAccounts[i];
                int itemLength = Rlp.LengthOf(pwa.AddressHash);
                itemLength += _decoder.GetLength(pwa.Account);

                pwasLength += Rlp.LengthOfSequence(itemLength);
            }
        }

        contentLength += Rlp.LengthOfSequence(pwasLength);

        int proofsLength = 0;
        if (message.Proofs == null || message.Proofs.Length == 0)
        {
            proofsLength = 1;
        }
        else
        {
            for (int i = 0; i < message.Proofs.Length; i++)
            {
                proofsLength += Rlp.LengthOf(message.Proofs[i]);
            }
        }

        contentLength += Rlp.LengthOfSequence(proofsLength);

        return (contentLength, pwasLength, proofsLength);
    }
}
