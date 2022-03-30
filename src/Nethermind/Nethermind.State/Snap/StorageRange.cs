using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethermind.Core.Crypto;

namespace Nethermind.State.Snap
{
    public class StorageRange
    {
        //public StorageRange(Keccak rootHash, PathWithAccount[] accounts, Keccak startingHash = null, Keccak limitHash = null, long? blockNumber = null)
        //{
        //    RootHash = rootHash;
        //    Accounts = accounts;
        //    StartingHash = startingHash;
        //    BlockNumber = blockNumber;
        //    LimitHash = limitHash;
        //}

        public long? BlockNumber { get; set; }

        /// <summary>
        /// Root hash of the account trie to serve
        /// </summary>
        public Keccak RootHash { get; set; }

        /// <summary>
        /// Accounts of the storage tries to serve
        /// </summary>
        public PathWithAccount[] Accounts { get; set; }

        /// <summary>
        /// Account hash of the first to retrieve
        /// </summary>
        public Keccak? StartingHash { get; set; }

        /// <summary>
        /// Account hash after which to stop serving data
        /// </summary>
        public Keccak? LimitHash { get; set; }
    }
}