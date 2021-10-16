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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nethermind.AccountAbstraction.Data;
using Nethermind.Core;
using Nethermind.Core.Specs;
using Nethermind.Int256;
using Nethermind.JsonRpc;

namespace Nethermind.AccountAbstraction.Executor
{
    public interface IUserOperationSimulator
    {
        Task<ResultWrapper<bool>> Simulate(
            UserOperation userOperation, 
            BlockHeader parent,
            CancellationToken cancellationToken = default, 
            UInt256? timestamp = null);

        public Transaction BuildTransactionFromUserOperations(
            IEnumerable<UserOperation> userOperations,
            BlockHeader parent,
            IReleaseSpec spec);
    }
}