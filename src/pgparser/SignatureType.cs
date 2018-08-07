namespace pgparser
{
    public enum SignatureType: byte
    {
        SignatureOfBinaryDocument = 0x00,
        SignatureOfCanonicalTextDocument = 0x01,
        StandaloneSignature = 0x02,
        GenericCertification = 0x10,
        PersonaCertification = 0x11,
        CasualCertification = 0x12,
        PositiveCertification = 0x13,
        SubkeyBinding = 0x18,
        PrimaryKeyBinding = 0x19,
        KeySignature = 0x1F,
        KeyRevocation = 0x20,
        SubkeyRevocation = 0x28,
        CertificationRevocation = 0x30,
        Timestamp = 0x40,
        ThirdPartyConfirmation = 0x50,
    }
}
