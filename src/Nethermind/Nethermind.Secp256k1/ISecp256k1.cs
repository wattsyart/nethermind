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

namespace Nethermind.Secp256k1;

public interface ISecp256k1
{
    bool VerifyPrivateKey(byte[] privateKey);
    byte[] GetPublicKey(byte[] privateKey, bool compressed);
    byte[] SignCompact(byte[] messageHash, byte[] privateKey, out int recoveryId);
    bool RecoverKeyFromCompact(Span<byte> output, byte[] messageHash, Span<byte> compactSignature, int recoveryId, bool compressed);
    bool Ecdh(byte[] agreement, byte[] publicKey, byte[] privateKey);
    byte[] EcdhSerialized(byte[] publicKey, byte[] privateKey);
    byte[] Decompress(Span<byte> compressed);
}
