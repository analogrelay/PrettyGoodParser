namespace pgparser
{
    public enum PgpPacketTag : byte
    {
        PublicKeyEncryptedSessionKey = 1,
        Signature = 2,
        SymmetricKeyEncryptedSessionKey = 3,
        OnePassSignature = 4,
        SecretKey = 5,
        PublicKey = 6,
        SecretSubKey = 7,
        CompressedData = 8,
        SymmetricallyEncryptedData = 9,
        Marker = 10,
        LiteralData = 11,
        Trust = 12,
        UserId = 13,
        PublicSubKey = 14,
        UserAttribute = 17,
        SymmetricallyEncryptedIntegrityProtectedData = 18,
        ModificationDetectionCode = 19,
    }
}
