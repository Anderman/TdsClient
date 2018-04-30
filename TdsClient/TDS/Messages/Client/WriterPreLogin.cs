using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterPreLogin
    {
        private const int GuidSize = 16;

        public static void SendPreLoginHandshake(this TdsPackageWriter writer, byte[] instanceName, bool marsOn)
        {
            // PreLoginHandshake buffer consists of:
            // 1) Standard header, with type = MT_PRELOGIN
            // 2) Consecutive 5 bytes for each option, (1 byte length, 2 byte offset, 2 byte buffer length)
            // 3) Consecutive data blocks for each option

            // NOTE: packet data needs to be big endian - not the standard little endian used by
            // the rest of the parser.

            writer.NewPackage();
            WriteHeader(writer, instanceName);
            WriteOptions(writer, instanceName, marsOn);

            // Write out last option - to let server know the second part of packet completed
            writer.SetHeader(TdsEnums.ST_EOM, TdsEnums.MT_PRELOGIN);
            writer.FlushBuffer();
        }

        private static void WriteOptions(TdsPackageWriter writer, byte[] instanceName, bool marsOn)
        {
            for (var option = (int) PreLoginOptions.VERSION; option < (int) PreLoginOptions.NUMOPT; option++)
                switch (option)
                {
                    case (int) PreLoginOptions.VERSION:
                        // Major and minor
                        writer.WriteByte(0);
                        writer.WriteByte(0);
                        // Build (Big Endian)
                        writer.WriteByte(0);
                        writer.WriteByte(0);
                        // Sub-build (Little Endian)
                        writer.WriteByte(0);
                        writer.WriteByte(0);
                        break;

                    case (int) PreLoginOptions.ENCRYPT:
                        writer.WriteByte((byte) EncryptionOptions.NOT_SUP);
                        break;

                    case (int) PreLoginOptions.INSTANCE:
                        var i = 0;
                        while (instanceName[i] != 0)
                            writer.WriteByte(instanceName[i++]);
                        writer.WriteByte(0); // null terminate
                        break;

                    case (int) PreLoginOptions.THREADID:
                        writer.WriteUInt32(0);
                        break;

                    case (int) PreLoginOptions.MARS:
                        writer.WriteByte((byte) (marsOn ? 1 : 0)); // null terminate
                        break;

                    case (int) PreLoginOptions.TRACEID:
                        writer.WriteByteArray(new byte[GuidSize], GuidSize);
                        writer.WriteByteArray(new byte[GuidSize], GuidSize);
                        writer.WriteUInt32(0);
                        break;
                }
        }

        private static void WriteHeader(TdsPackageWriter writer, byte[] instanceName)
        {
            // Initialize option offset into buffer buffer
            // 5 bytes for each option (1 byte length, 2 byte offset, 2 byte buffer length)
            var offset = (int) PreLoginOptions.NUMOPT * 5 + 1;

            for (var option = (int) PreLoginOptions.VERSION; option < (int) PreLoginOptions.NUMOPT; option++)
            {
                // Fill in the option
                writer.WriteByte((byte) option);

                // Fill in the offset of the option data
                writer.WriteByte((byte) ((offset & 0xff00) >> 8)); // send upper order byte
                writer.WriteByte((byte) (offset & 0x00ff)); // send lower order byte

                var optionDataSize = GetOptionSize(option, instanceName);

                writer.WriteByte((byte) ((optionDataSize & 0xff00) >> 8));
                writer.WriteByte((byte) (optionDataSize & 0x00ff));
                offset += optionDataSize;
            }

            writer.WriteByte((byte) PreLoginOptions.LASTOPT);
        }

        private static byte GetOptionSize(int option, byte[] instanceName)
        {
            switch (option)
            {
                case (int) PreLoginOptions.VERSION:
                    return 6;
                case (int) PreLoginOptions.ENCRYPT:
                    return 1;
                case (int) PreLoginOptions.INSTANCE:
                    return (byte) instanceName.Length;
                case (int) PreLoginOptions.THREADID:
                    return 4;
                case (int) PreLoginOptions.MARS:
                    return 1;
                case (int) PreLoginOptions.TRACEID:
                    return GuidSize * 2 + sizeof(uint);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}