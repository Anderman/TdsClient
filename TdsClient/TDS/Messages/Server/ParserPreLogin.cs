using System;
using Medella.TdsClient.Constants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserPreLogin
    {
        public static EncryptionOptions ParseConnect(this TdsPackageReader reader, EncryptionOptions encryptionRequested)
        {
            var result = ParsePreLoginHandshake(reader.ReadBuffer, TdsEnums.HEADER_LEN, encryptionRequested);
            reader.PackageDone();
            return result;
        }

        public static EncryptionOptions ParsePreLoginHandshake(byte[] payload, int payloadOffset, EncryptionOptions encryptionRequested)
        {
            if (payload[payloadOffset] == 0xaa) throw SQL.InvalidSqlServerVersionUnknown();
            var result = new TdsConnectionResult();
            var offset = payloadOffset;
            int option = payload[offset++];

            while (option != (byte)PreLoginOptions.LASTOPT)
            {
                var dataOffset = ((payload[offset] << 8) | payload[offset + 1]) + payloadOffset;
                switch (option)
                {
                    case (int)PreLoginOptions.VERSION:
                        result.Version = payload[dataOffset];
                        break;

                    case (int)PreLoginOptions.ENCRYPT:
                        var serverOption = (EncryptionOptions)payload[dataOffset];

                        /* internal enum encryptionOptions {
                            OFF,
                            ON,
                            NOT_SUP,
                            REQ,
                            LOGIN
                        } */
                        result.EncryptionOption = encryptionRequested;
                        switch (encryptionRequested)
                        {
                            case EncryptionOptions.ON:
                                if (serverOption == EncryptionOptions.NOT_SUP)
                                    throw new Exception(SQLMessage.EncryptionNotSupportedByServer());
                                break;

                            case EncryptionOptions.OFF:
                                switch (serverOption)
                                {
                                    case EncryptionOptions.OFF:
                                        // Only encrypt login.
                                        result.EncryptionOption = EncryptionOptions.LOGIN;
                                        break;
                                    case EncryptionOptions.REQ:
                                        // Encrypt all.
                                        result.EncryptionOption = EncryptionOptions.ON;
                                        break;
                                }

                                break;

                            case EncryptionOptions.NOT_SUP:
                                if (serverOption == EncryptionOptions.REQ)
                                    throw new Exception(SQLMessage.EncryptionNotSupportedByClient());
                                break;
                        }

                        break;

                    case (int)PreLoginOptions.INSTANCE:

                        if (payload[dataOffset] == 0x1) throw new Exception("Instancename not correct");

                        break;

                    case (int)PreLoginOptions.THREADID:
                        // DO NOTHING FOR THREADID
                        break;

                    case (int)PreLoginOptions.MARS:
                        result.IsMarsCapable = payload[dataOffset] != 0;
                        break;

                    case (int)PreLoginOptions.TRACEID:
                        // DO NOTHING FOR TRACEID
                        break;
                }

                offset += 4;
                if (offset >= payload.Length)
                    break;
                option = payload[offset++];
            }

            return result.EncryptionOption;
        }
    }
}