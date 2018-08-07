using System;
using System.Collections.Generic;
using System.Text;

namespace pgparser
{
    public class SignaturePacket : PgpPacket
    {
        public int Version { get; }
        public SignatureType SignatureType { get; }
        public int PublicKeyAlgorithm { get; }
        public int HashAlgorithm { get; }
        public ReadOnlyMemory<byte> HashedData { get; }
        public ReadOnlyMemory<byte> UnhashedData { get; }

        public SignaturePacket(int version, SignatureType signatureType, int publicKeyAlgorithm, int hashAlgorithm, ReadOnlyMemory<byte> hashedData, ReadOnlyMemory<byte> unhashedData, int length) : base(PgpPacketTag.Signature, length)
        {
            Version = version;
            SignatureType = signatureType;
            PublicKeyAlgorithm = publicKeyAlgorithm;
            HashAlgorithm = hashAlgorithm;
            HashedData = hashedData;
            UnhashedData = unhashedData;
        }
    }
}
