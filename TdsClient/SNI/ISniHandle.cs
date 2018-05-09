namespace Medella.TdsClient.SNI
{
    public interface ISniHandle
    {
        string  InstanceName { get; }
        void FlushBuffer(byte[] writeBuffer, int count);
        int Receive(byte[] readBuffer, int offset, int count);

        byte[] GetClientToken(byte[] serverToken);
    }
}