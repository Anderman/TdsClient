using System;
using System.Diagnostics;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;
using TdsPackageWriter = Medella.TdsClient.TDS.Package.Writer.TdsPackageWriter;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterLogin
    {
        public static void SendTdsLogin(this TdsPackageWriter writer, LoginOptions rec, SessionData recoverySessionData)
        {
            const string clientInterfaceName = TdsEnums.SQL_PROVIDER_NAME;

            var requestedFeatures = rec.RequestedFeatures;
            if (recoverySessionData != null && (requestedFeatures & TdsEnums.FeatureExtension.SessionRecovery) == 0) throw new Exception("Recovery session data without session recovery feature request");
            if (rec.HostName.Length > TdsEnums.MAXLEN_HOSTNAME) throw new Exception("_workstationId.Length exceeds the max length for this value");
            if (!rec.UseSspi && rec.UserName.Length > TdsEnums.MAXLEN_USERNAME) throw new Exception("_userID.Length exceeds the max length for this value");
            if (!rec.UseSspi && rec.Password.Length > TdsEnums.MAXLEN_PASSWORD) throw new Exception("_password.Length exceeds the max length for this value");
            if (rec.ApplicationName.Length > TdsEnums.MAXLEN_APPNAME) throw new Exception("_applicationName.Length exceeds the max length for this value");
            if (rec.ServerName.Length > TdsEnums.MAXLEN_SERVERNAME) throw new Exception("_dataSource.Length exceeds the max length for this value");
            if (rec.Language.Length > TdsEnums.MAXLEN_LANGUAGE) throw new Exception("_currentLanguage .Length exceeds the max length for this value");
            if (rec.Database.Length > TdsEnums.MAXLEN_DATABASE) throw new Exception("_initialCatalog.Length exceeds the max length for this value");
            if (rec.AttachDbFilename.Length > TdsEnums.MAXLEN_ATTACHDBFILE) throw new Exception("_attachDBFileName.Length exceeds the max length for this value");

            if (clientInterfaceName.Length > TdsEnums.MAXLEN_CLIENTINTERFACE) throw new Exception("cchCltIntName can specify at most 128 unicode characters. See Tds spec");


            // get the password up front to use in sspi logic below
            var useFeatureExt = requestedFeatures != TdsEnums.FeatureExtension.None;


            // set the message type

            // length in bytes
            var length = TdsEnums.YUKON_LOG_REC_FIXED_LEN;


            // add up variable-len portions (multiply by 2 for byte len of char strings)
            //
            length += (rec.HostName.Length + rec.ApplicationName.Length +
                       rec.ServerName.Length + clientInterfaceName.Length +
                       rec.Language.Length + rec.Database.Length +
                       rec.AttachDbFilename.Length) * 2;
            if (useFeatureExt) length += 4;

            // allocate memory for SSPI variables

            // only add lengths of password and username if not using SSPI

            var userName = "";
            var encryptedPassword = new byte[0];
            if (rec.UseSspi)
            {
                length += rec.ClientToken.Length;
            }
            else
            {
                userName = rec.UserName;
                encryptedPassword = ObfuscatePassword(rec.Password);
                length += userName.Length * 2 + encryptedPassword.Length;
            }

            var feOffset = length;

            if (useFeatureExt)
            {
                if ((requestedFeatures & TdsEnums.FeatureExtension.SessionRecovery) != 0) length += SessionRecoveryFeatureRequestLengths(recoverySessionData).totalLength;
                if ((requestedFeatures & TdsEnums.FeatureExtension.GlobalTransactions) != 0) length += 5;
                length++; // for terminator
            }

            writer.NewPackage(TdsEnums.MT_LOGIN7);
            writer.WriteInt32(length);
            if (recoverySessionData == null)
                writer.WriteInt32((TdsEnums.DENALI_MAJOR << 24) | (TdsEnums.DENALI_INCREMENT << 16) | TdsEnums.DENALI_MINOR);
            else
                writer.WriteUInt32(recoverySessionData.TdsVersion);
            writer.WriteInt32(rec.PacketSize);
            writer.WriteInt32(TdsEnums.CLIENT_PROG_VER);
            writer.WriteInt32(GetCurrentProcessIdForTdsLoginOnly());
            writer.WriteInt32(0); // connectionID is unused

            // Log7Flags (DWORD)
            var log7Flags = 0;

            /*
             Current snapshot from TDS spec with the offsets added:
                0) fByteOrder:1,                // byte order of numeric data types on client
                1) fCharSet:1,                  // character set on client
                2) fFloat:2,                    // Type of floating point on client
                4) fDumpLoad:1,                 // Dump/Load and BCP enable
                5) fUseDb:1,                    // USE notification
                6) fDatabase:1,                 // Initial database fatal flag
                7) fSetLang:1,                  // SET LANGUAGE notification
                8) fLanguage:1,                 // Initial language fatal flag
                9) fODBC:1,                     // Set if client is ODBC driver
               10) fTranBoundary:1,             // Transaction boundary notification
               11) fDelegatedSec:1,             // Security with delegation is available
               12) fUserType:3,                 // Type of user
               15) fIntegratedSecurity:1,       // Set if client is using integrated security
               16) fSQLType:4,                  // Type of SQL sent from client
               20) fOLEDB:1,                    // Set if client is OLEDB driver
               21) fSpare1:3,                   // first bit used for read-only intent, rest unused
               24) fResetPassword:1,            // set if client wants to reset password
               25) fNoNBCAndSparse:1,           // set if client does not support NBC and Sparse column
               26) fUserInstance:1,             // This connection wants to connect to a SQL "user instance"
               27) fUnknownCollationHandling:1, // This connection can handle unknown collation correctly.
               28) fExtension:1                 // Extensions are used                 
               32 - total
            */

            // first byte
            log7Flags |= TdsEnums.USE_DB_ON << 5;
            log7Flags |= TdsEnums.INIT_DB_FATAL << 6;
            log7Flags |= TdsEnums.SET_LANG_ON << 7;

            // second byte
            log7Flags |= TdsEnums.INIT_LANG_FATAL << 8;
            log7Flags |= TdsEnums.ODBC_ON << 9;
            if (rec.UseReplication) log7Flags |= TdsEnums.REPL_ON << 12;
            if (rec.UseSspi) log7Flags |= TdsEnums.SSPI_ON << 15;

            // third byte
            if (rec.ReadOnlyIntent) log7Flags |= TdsEnums.READONLY_INTENT_ON << 21; // read-only intent flag is a first bit of fSpare1

            // 4th one
            if (rec.UserInstance) log7Flags |= 1 << 26;
            if (useFeatureExt) log7Flags |= 1 << 28;

            writer.WriteInt32(log7Flags);

            writer.WriteInt32(0); // ClientTimeZone is not used
            writer.WriteInt32(0); // LCID is unused by server

            // Start writing offset and length of variable length portions
            var offset = TdsEnums.YUKON_LOG_REC_FIXED_LEN;

            // write offset/length pairs

            // note that you must always set ibHostName since it indicates the beginning of the variable length section of the login record
            writer.WriteInt16(offset); // host name offset
            writer.WriteInt16(rec.HostName.Length);
            offset += rec.HostName.Length * 2;

            // Only send user/password over if not fSSPI...  If both user/password and SSPI are in login
            // rec, only SSPI is used.  Confirmed same behavior as in luxor.
            if (rec.UseSspi)
            {
                // case where user/password data is not used, send over zeros
                writer.WriteInt16(0); // userName offset
                writer.WriteInt16(0);
                writer.WriteInt16(0); // password offset
                writer.WriteInt16(0);
            }
            else
            {
                writer.WriteInt16(offset); // userName offset
                writer.WriteInt16(userName.Length);
                offset += userName.Length * 2;

                // the encrypted password is a byte array - so length computations different than strings
                writer.WriteInt16(offset); // password offset
                writer.WriteInt16(encryptedPassword.Length / 2);
                offset += encryptedPassword.Length;
            }

            writer.WriteInt16(offset); // app name offset
            writer.WriteInt16(rec.ApplicationName.Length);
            offset += rec.ApplicationName.Length * 2;

            writer.WriteInt16(offset); // server name offset
            writer.WriteInt16(rec.ServerName.Length);
            offset += rec.ServerName.Length * 2;

            writer.WriteInt16(offset);
            if (useFeatureExt)
            {
                writer.WriteInt16(4); // length of ibFeatgureExtLong (which is a DWORD)
                offset += 4;
            }
            else
            {
                writer.WriteInt16(0); // unused (was remote password ?)
            }

            writer.WriteInt16(offset); // client interface name offset
            writer.WriteInt16(clientInterfaceName.Length);
            offset += clientInterfaceName.Length * 2;

            writer.WriteInt16(offset); // language name offset
            writer.WriteInt16(rec.Language.Length);
            offset += rec.Language.Length * 2;

            writer.WriteInt16(offset); // database name offset
            writer.WriteInt16(rec.Database.Length);
            offset += rec.Database.Length * 2;

            var nicAddress = GetNetworkPhysicalAddressForTdsLoginOnly();

            writer.WriteByteArray(nicAddress);

            writer.WriteInt16(offset); // ibSSPI offset
            if (rec.UseSspi)
            {
                writer.WriteInt16(rec.ClientToken.Length);
                offset += rec.ClientToken.Length;
            }
            else
            {
                writer.WriteInt16(0);
            }

            writer.WriteInt16(offset); // DB filename offset
            writer.WriteInt16(rec.AttachDbFilename.Length);
            offset += rec.AttachDbFilename.Length * 2;

            writer.WriteInt16(offset); // reset password offset
            writer.WriteInt16(0);

            writer.WriteInt32(0); // reserved for chSSPI

            // write variable length portion
            writer.WriteUnicodeString(rec.HostName);

            // if we are using SSPI, do not send over username/password, since we will use SSPI instead
            // same behavior as Luxor
            if (!rec.UseSspi)
            {
                writer.WriteUnicodeString(userName);
                writer.WriteByteArray(encryptedPassword);
            }

            writer.WriteUnicodeString(rec.ApplicationName);
            writer.WriteUnicodeString(rec.ServerName);

            // write ibFeatureExtLong
            if (useFeatureExt) writer.WriteInt32(feOffset);

            writer.WriteUnicodeString(clientInterfaceName);
            writer.WriteUnicodeString(rec.Language);
            writer.WriteUnicodeString(rec.Database);

            // send over SSPI data if we are using SSPI
            if (rec.UseSspi)
                writer.WriteByteArray(rec.ClientToken);

            writer.WriteUnicodeString(rec.AttachDbFilename);
            if (useFeatureExt)
            {
                if ((requestedFeatures & TdsEnums.FeatureExtension.SessionRecovery) != 0) WriteSessionRecoveryFeatureRequest(writer, recoverySessionData);
                if ((requestedFeatures & TdsEnums.FeatureExtension.GlobalTransactions) != 0) WriteGlobalTransactionsFeatureRequest(writer);
                writer.WriteByte(0xFF); // terminator
            }
        }

        internal static void WriteGlobalTransactionsFeatureRequest(TdsPackageWriter writer)
        {
            // Write Feature ID
            writer.WriteByte(TdsEnums.FEATUREEXT_GLOBALTRANSACTIONS);
            writer.WriteInt32(0); // we don't send any data
        }

        private static (int totalLength, int initialLength, int currentLength, bool[] writeState) SessionRecoveryFeatureRequestLengths(SessionData reconnectData)
        {
            var totalLength = 1;
            var initialLength = 0; // sizeof(DWORD) - length itself
            var currentLength = 0; // sizeof(DWORD) - length itself                
            var writeState = new bool[SessionData.MaxNumberOfSessionStates];
            if (reconnectData == null)
            {
                totalLength += 4;
            }
            else
            {
                Debug.Assert(reconnectData.UnrecoverableStatesCount == 0, "Unrecoverable state count should be 0");
                initialLength += 1 + 2 * NullAwareStringLength(reconnectData.InitialDatabase);
                initialLength += 1 + 2 * NullAwareStringLength(reconnectData.InitialLanguage);
                initialLength += reconnectData.InitialCollation == null ? 1 : 6;
                for (var i = 0; i < SessionData.MaxNumberOfSessionStates; i++)
                    if (reconnectData.InitialState[i] != null)
                        initialLength += 1 /* StateId*/ + StateValueLength(reconnectData.InitialState[i].Length);
                currentLength += 1 + 2 * (reconnectData.InitialDatabase == reconnectData.Database ? 0 : NullAwareStringLength(reconnectData.Database));
                currentLength += 1 + 2 * (reconnectData.InitialLanguage == reconnectData.Language ? 0 : NullAwareStringLength(reconnectData.Language));
                currentLength += reconnectData.Collation != null && !SqlCollations.AreSame(reconnectData.Collation, reconnectData.InitialCollation) ? 6 : 1;
                for (var i = 0; i < SessionData.MaxNumberOfSessionStates; i++)
                    if (reconnectData.Delta[i] != null)
                    {
                        Debug.Assert(reconnectData.Delta[i].Recoverable, "State should be recoverable");
                        writeState[i] = true;
                        if (reconnectData.InitialState[i] != null && reconnectData.InitialState[i].Length == reconnectData.Delta[i].DataLength)
                        {
                            writeState[i] = false;
                            for (var j = 0; j < reconnectData.Delta[i].DataLength; j++)
                                if (reconnectData.InitialState[i][j] != reconnectData.Delta[i].Data[j])
                                {
                                    writeState[i] = true;
                                    break;
                                }
                        }

                        if (writeState[i]) currentLength += 1 /* StateId*/ + StateValueLength(reconnectData.Delta[i].DataLength);
                    }

                totalLength += initialLength + currentLength + 12 /* length fields (initial, current, total) */;
            }

            return (totalLength, initialLength, currentLength, writeState);
        }

        private static int StateValueLength(int dataLen)
        {
            return dataLen < 0xFF ? dataLen + 1 : dataLen + 5;
        }

        private static void WriteSessionRecoveryFeatureRequest(TdsPackageWriter writer, SessionData reconnectData)
        {
            var (_, initialLength, currentLength, writeState) = SessionRecoveryFeatureRequestLengths(reconnectData);
            writer.WriteByte(TdsEnums.FEATUREEXT_SRECOVERY);
            if (reconnectData == null)
            {
                writer.WriteInt32(0);
            }
            else
            {
                writer.WriteInt32(8 + initialLength + currentLength); // length of data w/o total length (initial + current + 2 * sizeof(DWORD))
                writer.WriteInt32(initialLength);
                writer.WriteByteLenString(reconnectData.InitialDatabase);
                writer.WriteCollation(reconnectData.InitialCollation);
                writer.WriteByteLenString(reconnectData.InitialLanguage);
                for (var i = 0; i < SessionData.MaxNumberOfSessionStates; i++)
                    if (reconnectData.InitialState[i] != null)
                    {
                        writer.WriteByte((byte) i);
                        if (reconnectData.InitialState[i].Length < 0xFF)
                        {
                            writer.WriteByte((byte) reconnectData.InitialState[i].Length);
                        }
                        else
                        {
                            writer.WriteByte(0xFF);
                            writer.WriteInt32(reconnectData.InitialState[i].Length);
                        }

                        writer.WriteByteArray(reconnectData.InitialState[i]);
                    }

                writer.WriteInt32(currentLength);
                writer.WriteByteLenString(reconnectData.Database != reconnectData.InitialDatabase ? reconnectData.Database : null);
                writer.WriteCollation(SqlCollations.AreSame(reconnectData.InitialCollation, reconnectData.Collation) ? null : reconnectData.Collation);
                writer.WriteByteLenString(reconnectData.Language != reconnectData.InitialLanguage ? reconnectData.Language : null);
                for (var i = 0; i < SessionData.MaxNumberOfSessionStates; i++)
                    if (writeState[i])
                    {
                        writer.WriteByte((byte) i);
                        if (reconnectData.Delta[i].DataLength < 0xFF)
                        {
                            writer.WriteByte((byte) reconnectData.Delta[i].DataLength);
                        }
                        else
                        {
                            writer.WriteByte(0xFF);
                            writer.WriteInt32(reconnectData.Delta[i].DataLength);
                        }

                        writer.WriteByteArray(reconnectData.Delta[i].Data);
                    }
            }
        }

        internal static int NullAwareStringLength(string str)
        {
            return str?.Length ?? 0;
        }

        internal static byte[] ObfuscatePassword(string password)
        {
            var bObfuscated = new byte[password.Length << 1];

            for (var i = 0; i < password.Length; i++)
            {
                int s = password[i];
                var bLo = (byte) (s & 0xff);
                var bHi = (byte) ((s >> 8) & 0xff);
                bObfuscated[i << 1] = (byte) ((((bLo & 0x0f) << 4) | (bLo >> 4)) ^ 0xa5);
                bObfuscated[(i << 1) + 1] = (byte) ((((bHi & 0x0f) << 4) | (bHi >> 4)) ^ 0xa5);
            }

            return bObfuscated;
        }

        internal static int GetCurrentProcessIdForTdsLoginOnly()
        {
            return 0x2a2a2a2a;
        }

        internal static byte[] GetNetworkPhysicalAddressForTdsLoginOnly()
        {
            // For ProjectK\CoreCLR we don't want to take a dependency on the registry to try to read a value
            // that isn't usually set, so we'll just use a random value each time instead

            return new byte[] {0x2a, 0x2a, 0x2a, 0x2a, 0x2a, 0x2a};
        }
    }
}