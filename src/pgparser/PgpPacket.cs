namespace pgparser
{
    public class PgpPacket
    {
        public PgpPacketTag Tag { get; }
        public int Length { get; }

        public PgpPacket(PgpPacketTag tag, int length)
        {
            Tag = tag;
            Length = length;
        }
    }
}
