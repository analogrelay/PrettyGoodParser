using System;
using System.Buffers.Binary;

namespace pgparser
{
    internal class OpenPgpParser
    {
        internal static bool TryParse(ReadOnlySpan<byte> bytes, out PgpPacket packet)
        {
            if (bytes.Length == 0)
            {
                // Insufficient data
                packet = null;
                return false;
            }

            if (!TryReadHeader(ref bytes, out var tag, out var length))
            {
                packet = null;
                return false;
            }

            // Slice the input
            bytes = bytes.Slice(0, length);

            switch (tag)
            {
                case PgpPacketTag.Signature:
                    return TryParseSignature(ref bytes, out packet);
                default:
                    throw new FormatException($"Unsupported packet type: ");
            }
        }

        private static bool TryReadHeader(ref ReadOnlySpan<byte> bytes, out PgpPacketTag tag, out int length)
        {
            var prefix = bytes[0] & 0xC0;
            if (prefix == 0x80)
            {
                // Old format
                tag = (PgpPacketTag)((bytes[0] & 0x3C) >> 2);
                var lengthType = bytes[0] & 0x03;

                if (lengthType == 0)
                {
                    // 1 byte length
                    if (bytes.Length < 2)
                    {
                        length = 0;
                        return false;
                    }
                    length = bytes[1];
                    bytes = bytes.Slice(2);
                    return true;
                }
                else if (lengthType == 1)
                {
                    // 2 byte length
                    if (bytes.Length < 3)
                    {
                        length = 0;
                        return false;
                    }
                    length = BinaryPrimitives.ReadInt16BigEndian(bytes.Slice(1, 2));
                    bytes = bytes.Slice(3);
                    return true;
                }
                else if (lengthType == 2)
                {
                    // 4 byte length
                    if (bytes.Length < 5)
                    {
                        length = 0;
                        return false;
                    }
                    length = BinaryPrimitives.ReadInt32BigEndian(bytes.Slice(1, 4));
                    bytes = bytes.Slice(5);
                    return true;
                }
                else if (lengthType == 3)
                {
                    throw new NotSupportedException("Indeterminate-length packets not yet supported.");
                }
                else
                {
                    throw new FormatException("Invalid length type.");
                }
            }
            else if (prefix == 0xC0)
            {
                // New format
                throw new NotSupportedException("New format not yet supported.");
            }
            else
            {
                throw new FormatException("Invalid PGP Tag.");
            }
        }

        private static bool TryParseSignature(ref ReadOnlySpan<byte> bytes, out PgpPacket packet)
        {
            if (bytes.Length == 0)
            {
                packet = null;
                return false;
            }

            var version = (int)bytes[0];
            bytes = bytes.Slice(1);

            if (version == 3)
            {
                return TryParseV3Signature(ref bytes, out packet);
            }
            else if (version == 4)
            {
                return TryParseV4Signature(ref bytes, out packet);
            }
            else
            {
                throw new NotSupportedException($"Unsupported signature version: {version}");
            }
        }

        private static bool TryParseV4Signature(ref ReadOnlySpan<byte> bytes, out PgpPacket packet)
        {
            if(bytes.Length < 5)
            {
                // Insufficient data for the initial headers
                packet = null;
                return false;
            }

            var type = (SignatureType)bytes[0];
            var pkAlgorithm = bytes[1];
            var hashAlgorithm = bytes[2];
            var hashedDataLength = BinaryPrimitives.ReadInt16BigEndian(bytes.Slice(3, 2));

            var readSoFar = 5;

            if(bytes.Length < readSoFar + hashedDataLength)
            {
                // Insufficient data for the hashed data
                packet = null;
                return false;
            }
            var hashedData = bytes.Slice(readSoFar, hashedDataLength);
            readSoFar += hashedDataLength;

            if(bytes.Length < readSoFar + 2)
            {
                // Insufficient data for the unhashed data length
                packet = null;
                return false;
            }
            var unhashedDataLength = BinaryPrimitives.ReadInt16BigEndian(bytes.Slice(readSoFar, 2));
            readSoFar += 2;

            if(bytes.Length < readSoFar + unhashedDataLength)
            {
                // Insufficient data for the unhashed data
                packet = null;
                return false;
            }
            var unhashedData = bytes.Slice(readSoFar, unhashedDataLength);
            readSoFar += unhashedDataLength;

            packet = new SignaturePacket(4, type, pkAlgorithm, hashAlgorithm, hashedData.ToArray(), unhashedData.ToArray(), bytes.Length);
            return true;
        }

        private static bool TryParseV3Signature(ref ReadOnlySpan<byte> bytes, out PgpPacket packet)
        {
            throw new NotSupportedException("Version 3 signatures are not yet supported.");
        }
    }
}
