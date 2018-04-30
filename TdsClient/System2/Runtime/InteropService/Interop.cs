using System;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using Medella.TdsClient.System2.Net.Security;
using Microsoft.Win32.SafeHandles;

namespace System.Runtime.InteropServices
{
    public static partial class Interop
    {
        internal static partial class Crypt32
        {
            internal static partial class AuthType
            {
                internal const uint AUTHTYPE_CLIENT = 1;
                internal const uint AUTHTYPE_SERVER = 2;
            }

            internal static partial class CertChainPolicyIgnoreFlags
            {
                internal const uint CERT_CHAIN_POLICY_IGNORE_NOT_TIME_VALID_FLAG = 0x00000001;
                internal const uint CERT_CHAIN_POLICY_IGNORE_CTL_NOT_TIME_VALID_FLAG = 0x00000002;
                internal const uint CERT_CHAIN_POLICY_IGNORE_NOT_TIME_NESTED_FLAG = 0x00000004;
                internal const uint CERT_CHAIN_POLICY_IGNORE_INVALID_BASIC_CONSTRAINTS_FLAG = 0x00000008;
                internal const uint CERT_CHAIN_POLICY_ALLOW_UNKNOWN_CA_FLAG = 0x00000010;
                internal const uint CERT_CHAIN_POLICY_IGNORE_WRONG_USAGE_FLAG = 0x00000020;
                internal const uint CERT_CHAIN_POLICY_IGNORE_INVALID_NAME_FLAG = 0x00000040;
                internal const uint CERT_CHAIN_POLICY_IGNORE_INVALID_POLICY_FLAG = 0x00000080;
                internal const uint CERT_CHAIN_POLICY_IGNORE_END_REV_UNKNOWN_FLAG = 0x00000100;
                internal const uint CERT_CHAIN_POLICY_IGNORE_CTL_SIGNER_REV_UNKNOWN_FLAG = 0x00000200;
                internal const uint CERT_CHAIN_POLICY_IGNORE_CA_REV_UNKNOWN_FLAG = 0x00000400;
                internal const uint CERT_CHAIN_POLICY_IGNORE_ROOT_REV_UNKNOWN_FLAG = 0x00000800;

                internal const uint CERT_CHAIN_POLICY_IGNORE_ALL =
                    CERT_CHAIN_POLICY_IGNORE_NOT_TIME_VALID_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_CTL_NOT_TIME_VALID_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_NOT_TIME_NESTED_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_INVALID_BASIC_CONSTRAINTS_FLAG |
                    CERT_CHAIN_POLICY_ALLOW_UNKNOWN_CA_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_WRONG_USAGE_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_INVALID_NAME_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_INVALID_POLICY_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_END_REV_UNKNOWN_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_CTL_SIGNER_REV_UNKNOWN_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_CA_REV_UNKNOWN_FLAG |
                    CERT_CHAIN_POLICY_IGNORE_ROOT_REV_UNKNOWN_FLAG;
            }

            internal static partial class CertChainPolicy
            {
                internal const int CERT_CHAIN_POLICY_BASE = 1;
                internal const int CERT_CHAIN_POLICY_AUTHENTICODE = 2;
                internal const int CERT_CHAIN_POLICY_AUTHENTICODE_TS = 3;
                internal const int CERT_CHAIN_POLICY_SSL = 4;
                internal const int CERT_CHAIN_POLICY_BASIC_CONSTRAINTS = 5;
                internal const int CERT_CHAIN_POLICY_NT_AUTH = 6;
                internal const int CERT_CHAIN_POLICY_MICROSOFT_ROOT = 7;
                internal const int CERT_CHAIN_POLICY_EV = 8;
            }

            internal static partial class CertChainPolicyErrors
            {
                // Base Policy errors (CERT_CHAIN_POLICY_BASE).
                internal const uint TRUST_E_CERT_SIGNATURE = 0x80096004;
                internal const uint CRYPT_E_REVOKED = 0x80092010;
                internal const uint CERT_E_UNTRUSTEDROOT = 0x800B0109;
                internal const uint CERT_E_UNTRUSTEDTESTROOT = 0x800B010D;
                internal const uint CERT_E_CHAINING = 0x800B010A;
                internal const uint CERT_E_WRONG_USAGE = 0x800B0110;
                internal const uint CERT_E_EXPIRE = 0x800B0101;
                internal const uint CERT_E_INVALID_NAME = 0x800B0114;
                internal const uint CERT_E_INVALID_POLICY = 0x800B0113;

                // Basic Constraints Policy errors (CERT_CHAIN_POLICY_BASIC_CONSTRAINTS).
                internal const uint TRUST_E_BASIC_CONSTRAINTS = 0x80096019;

                // Authenticode Policy errors (CERT_CHAIN_POLICY_AUTHENTICODE and CERT_CHAIN_POLICY_AUTHENTICODE_TS).
                internal const uint CERT_E_CRITICAL = 0x800B0105;
                internal const uint CERT_E_VALIDITYPERIODNESTING = 0x800B0102;
                internal const uint CRYPT_E_NO_REVOCATION_CHECK = 0x80092012;
                internal const uint CRYPT_E_REVOCATION_OFFLINE = 0x80092013;
                internal const uint CERT_E_PURPOSE = 0x800B0106;
                internal const uint CERT_E_REVOKED = 0x800B010C;
                internal const uint CERT_E_REVOCATION_FAILURE = 0x800B010E;

                // SSL Policy errors (CERT_CHAIN_POLICY_SSL).
                internal const uint CERT_E_CN_NO_MATCH = 0x800B010F;
                internal const uint CERT_E_ROLE = 0x800B0103;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct CERT_CONTEXT
            {
                internal uint dwCertEncodingType;
                internal IntPtr pbCertEncoded;
                internal uint cbCertEncoded;
                internal IntPtr pCertInfo;
                internal IntPtr hCertStore;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal unsafe struct SSL_EXTRA_CERT_CHAIN_POLICY_PARA
            {
                internal uint cbSize;
                internal uint dwAuthType;
                internal uint fdwChecks;
                internal char* pwszServerName;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal unsafe struct CERT_CHAIN_POLICY_PARA
            {
                public uint cbSize;
                public uint dwFlags;
                public SSL_EXTRA_CERT_CHAIN_POLICY_PARA* pvExtraPolicyPara;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal unsafe struct CERT_CHAIN_POLICY_STATUS
            {
                public uint cbSize;
                public uint dwError;
                public int lChainIndex;
                public int lElementIndex;
                public void* pvExtraPolicyStatus;
            }
        }
    }

    public static partial class Interop
    {
        internal static partial class Crypt32
        {
            [DllImport(Interop.Libraries.Crypt32, CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool CertFreeCertificateContext(IntPtr pCertContext);

            [DllImport(Interop.Libraries.Crypt32, CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool CertVerifyCertificateChainPolicy(
                IntPtr pszPolicyOID,
                SafeX509ChainHandle pChainContext,
                [In] ref CERT_CHAIN_POLICY_PARA pPolicyPara,
                [In, Out] ref CERT_CHAIN_POLICY_STATUS pPolicyStatus);
        }
    }

    public static partial class Interop
    {
        internal enum SECURITY_STATUS
        {
            // Success / Informational
            OK = 0x00000000,
            ContinueNeeded = unchecked((int)0x00090312),
            CompleteNeeded = unchecked((int)0x00090313),
            CompAndContinue = unchecked((int)0x00090314),
            ContextExpired = unchecked((int)0x00090317),
            CredentialsNeeded = unchecked((int)0x00090320),
            Renegotiate = unchecked((int)0x00090321),

            // Errors
            OutOfMemory = unchecked((int)0x80090300),
            InvalidHandle = unchecked((int)0x80090301),
            Unsupported = unchecked((int)0x80090302),
            TargetUnknown = unchecked((int)0x80090303),
            InternalError = unchecked((int)0x80090304),
            PackageNotFound = unchecked((int)0x80090305),
            NotOwner = unchecked((int)0x80090306),
            CannotInstall = unchecked((int)0x80090307),
            InvalidToken = unchecked((int)0x80090308),
            CannotPack = unchecked((int)0x80090309),
            QopNotSupported = unchecked((int)0x8009030A),
            NoImpersonation = unchecked((int)0x8009030B),
            LogonDenied = unchecked((int)0x8009030C),
            UnknownCredentials = unchecked((int)0x8009030D),
            NoCredentials = unchecked((int)0x8009030E),
            MessageAltered = unchecked((int)0x8009030F),
            OutOfSequence = unchecked((int)0x80090310),
            NoAuthenticatingAuthority = unchecked((int)0x80090311),
            IncompleteMessage = unchecked((int)0x80090318),
            IncompleteCredentials = unchecked((int)0x80090320),
            BufferNotEnough = unchecked((int)0x80090321),
            WrongPrincipal = unchecked((int)0x80090322),
            TimeSkew = unchecked((int)0x80090324),
            UntrustedRoot = unchecked((int)0x80090325),
            IllegalMessage = unchecked((int)0x80090326),
            CertUnknown = unchecked((int)0x80090327),
            CertExpired = unchecked((int)0x80090328),
            AlgorithmMismatch = unchecked((int)0x80090331),
            SecurityQosFailed = unchecked((int)0x80090332),
            SmartcardLogonRequired = unchecked((int)0x8009033E),
            UnsupportedPreauth = unchecked((int)0x80090343),
            BadBinding = unchecked((int)0x80090346),
            DowngradeDetected = unchecked((int)0x80090350),
            ApplicationProtocolMismatch = unchecked((int)0x80090367),
        }

#if TRACE_VERBOSE
        internal static string MapSecurityStatus(uint statusCode)
        {
            switch (statusCode)
            {
                case 0: return "0";
                case 0x80090001: return "NTE_BAD_UID";
                case 0x80090002: return "NTE_BAD_HASH";
                case 0x80090003: return "NTE_BAD_KEY";
                case 0x80090004: return "NTE_BAD_LEN";
                case 0x80090005: return "NTE_BAD_DATA";
                case 0x80090006: return "NTE_BAD_SIGNATURE";
                case 0x80090007: return "NTE_BAD_VER";
                case 0x80090008: return "NTE_BAD_ALGID";
                case 0x80090009: return "NTE_BAD_FLAGS";
                case 0x8009000A: return "NTE_BAD_TYPE";
                case 0x8009000B: return "NTE_BAD_KEY_STATE";
                case 0x8009000C: return "NTE_BAD_HASH_STATE";
                case 0x8009000D: return "NTE_NO_KEY";
                case 0x8009000E: return "NTE_NO_MEMORY";
                case 0x8009000F: return "NTE_EXISTS";
                case 0x80090010: return "NTE_PERM";
                case 0x80090011: return "NTE_NOT_FOUND";
                case 0x80090012: return "NTE_DOUBLE_ENCRYPT";
                case 0x80090013: return "NTE_BAD_PROVIDER";
                case 0x80090014: return "NTE_BAD_PROV_TYPE";
                case 0x80090015: return "NTE_BAD_PUBLIC_KEY";
                case 0x80090016: return "NTE_BAD_KEYSET";
                case 0x80090017: return "NTE_PROV_TYPE_NOT_DEF";
                case 0x80090018: return "NTE_PROV_TYPE_ENTRY_BAD";
                case 0x80090019: return "NTE_KEYSET_NOT_DEF";
                case 0x8009001A: return "NTE_KEYSET_ENTRY_BAD";
                case 0x8009001B: return "NTE_PROV_TYPE_NO_MATCH";
                case 0x8009001C: return "NTE_SIGNATURE_FILE_BAD";
                case 0x8009001D: return "NTE_PROVIDER_DLL_FAIL";
                case 0x8009001E: return "NTE_PROV_DLL_NOT_FOUND";
                case 0x8009001F: return "NTE_BAD_KEYSET_PARAM";
                case 0x80090020: return "NTE_FAIL";
                case 0x80090021: return "NTE_SYS_ERR";
                case 0x80090022: return "NTE_SILENT_CONTEXT";
                case 0x80090023: return "NTE_TOKEN_KEYSET_STORAGE_FULL";
                case 0x80090024: return "NTE_TEMPORARY_PROFILE";
                case 0x80090025: return "NTE_FIXEDPARAMETER";
                case 0x80090300: return "SEC_E_INSUFFICIENT_MEMORY";
                case 0x80090301: return "SEC_E_INVALID_HANDLE";
                case 0x80090302: return "SEC_E_UNSUPPORTED_FUNCTION";
                case 0x80090303: return "SEC_E_TARGET_UNKNOWN";
                case 0x80090304: return "SEC_E_INTERNAL_ERROR";
                case 0x80090305: return "SEC_E_SECPKG_NOT_FOUND";
                case 0x80090306: return "SEC_E_NOT_OWNER";
                case 0x80090307: return "SEC_E_CANNOT_INSTALL";
                case 0x80090308: return "SEC_E_INVALID_TOKEN";
                case 0x80090309: return "SEC_E_CANNOT_PACK";
                case 0x8009030A: return "SEC_E_QOP_NOT_SUPPORTED";
                case 0x8009030B: return "SEC_E_NO_IMPERSONATION";
                case 0x8009030C: return "SEC_E_LOGON_DENIED";
                case 0x8009030D: return "SEC_E_UNKNOWN_CREDENTIALS";
                case 0x8009030E: return "SEC_E_NO_CREDENTIALS";
                case 0x8009030F: return "SEC_E_MESSAGE_ALTERED";
                case 0x80090310: return "SEC_E_OUT_OF_SEQUENCE";
                case 0x80090311: return "SEC_E_NO_AUTHENTICATING_AUTHORITY";
                case 0x00090312: return "SEC_I_CONTINUE_NEEDED";
                case 0x00090313: return "SEC_I_COMPLETE_NEEDED";
                case 0x00090314: return "SEC_I_COMPLETE_AND_CONTINUE";
                case 0x00090315: return "SEC_I_LOCAL_LOGON";
                case 0x80090316: return "SEC_E_BAD_PKGID";
                case 0x80090317: return "SEC_E_CONTEXT_EXPIRED";
                case 0x00090317: return "SEC_I_CONTEXT_EXPIRED";
                case 0x80090318: return "SEC_E_INCOMPLETE_MESSAGE";
                case 0x80090320: return "SEC_E_INCOMPLETE_CREDENTIALS";
                case 0x80090321: return "SEC_E_BUFFER_TOO_SMALL";
                case 0x00090320: return "SEC_I_INCOMPLETE_CREDENTIALS";
                case 0x00090321: return "SEC_I_RENEGOTIATE";
                case 0x80090322: return "SEC_E_WRONG_PRINCIPAL";
                case 0x00090323: return "SEC_I_NO_LSA_CONTEXT";
                case 0x80090324: return "SEC_E_TIME_SKEW";
                case 0x80090325: return "SEC_E_UNTRUSTED_ROOT";
                case 0x80090326: return "SEC_E_ILLEGAL_MESSAGE";
                case 0x80090327: return "SEC_E_CERT_UNKNOWN";
                case 0x80090328: return "SEC_E_CERT_EXPIRED";
                case 0x80090329: return "SEC_E_ENCRYPT_FAILURE";
                case 0x80090330: return "SEC_E_DECRYPT_FAILURE";
                case 0x80090331: return "SEC_E_ALGORITHM_MISMATCH";
                case 0x80090332: return "SEC_E_SECURITY_QOS_FAILED";
                case 0x80090333: return "SEC_E_UNFINISHED_CONTEXT_DELETED";
                case 0x80090334: return "SEC_E_NO_TGT_REPLY";
                case 0x80090335: return "SEC_E_NO_IP_ADDRESSES";
                case 0x80090336: return "SEC_E_WRONG_CREDENTIAL_HANDLE";
                case 0x80090337: return "SEC_E_CRYPTO_SYSTEM_INVALID";
                case 0x80090338: return "SEC_E_MAX_REFERRALS_EXCEEDED";
                case 0x80090339: return "SEC_E_MUST_BE_KDC";
                case 0x8009033A: return "SEC_E_STRONG_CRYPTO_NOT_SUPPORTED";
                case 0x8009033B: return "SEC_E_TOO_MANY_PRINCIPALS";
                case 0x8009033C: return "SEC_E_NO_PA_DATA";
                case 0x8009033D: return "SEC_E_PKINIT_NAME_MISMATCH";
                case 0x8009033E: return "SEC_E_SMARTCARD_LOGON_REQUIRED";
                case 0x8009033F: return "SEC_E_SHUTDOWN_IN_PROGRESS";
                case 0x80090340: return "SEC_E_KDC_INVALID_REQUEST";
                case 0x80090341: return "SEC_E_KDC_UNABLE_TO_REFER";
                case 0x80090342: return "SEC_E_KDC_UNKNOWN_ETYPE";
                case 0x80090343: return "SEC_E_UNSUPPORTED_PREAUTH";
                case 0x80090345: return "SEC_E_DELEGATION_REQUIRED";
                case 0x80090346: return "SEC_E_BAD_BINDINGS";
                case 0x80090347: return "SEC_E_MULTIPLE_ACCOUNTS";
                case 0x80090348: return "SEC_E_NO_KERB_KEY";
                case 0x80090350: return "SEC_E_DOWNGRADE_DETECTED";
                case 0x80091001: return "CRYPT_E_MSG_ERROR";
                case 0x80091002: return "CRYPT_E_UNKNOWN_ALGO";
                case 0x80091003: return "CRYPT_E_OID_FORMAT";
                case 0x80091004: return "CRYPT_E_INVALID_MSG_TYPE";
                case 0x80091005: return "CRYPT_E_UNEXPECTED_ENCODING";
                case 0x80091006: return "CRYPT_E_AUTH_ATTR_MISSING";
                case 0x80091007: return "CRYPT_E_HASH_VALUE";
                case 0x80091008: return "CRYPT_E_INVALID_INDEX";
                case 0x80091009: return "CRYPT_E_ALREADY_DECRYPTED";
                case 0x8009100A: return "CRYPT_E_NOT_DECRYPTED";
                case 0x8009100B: return "CRYPT_E_RECIPIENT_NOT_FOUND";
                case 0x8009100C: return "CRYPT_E_CONTROL_TYPE";
                case 0x8009100D: return "CRYPT_E_ISSUER_SERIALNUMBER";
                case 0x8009100E: return "CRYPT_E_SIGNER_NOT_FOUND";
                case 0x8009100F: return "CRYPT_E_ATTRIBUTES_MISSING";
                case 0x80091010: return "CRYPT_E_STREAM_MSG_NOT_READY";
                case 0x80091011: return "CRYPT_E_STREAM_INSUFFICIENT_DATA";
                case 0x00091012: return "CRYPT_I_NEW_PROTECTION_REQUIRED";
                case 0x80092001: return "CRYPT_E_BAD_LEN";
                case 0x80092002: return "CRYPT_E_BAD_ENCODE";
                case 0x80092003: return "CRYPT_E_FILE_ERROR";
                case 0x80092004: return "CRYPT_E_NOT_FOUND";
                case 0x80092005: return "CRYPT_E_EXISTS";
                case 0x80092006: return "CRYPT_E_NO_PROVIDER";
                case 0x80092007: return "CRYPT_E_SELF_SIGNED";
                case 0x80092008: return "CRYPT_E_DELETED_PREV";
                case 0x80092009: return "CRYPT_E_NO_MATCH";
                case 0x8009200A: return "CRYPT_E_UNEXPECTED_MSG_TYPE";
                case 0x8009200B: return "CRYPT_E_NO_KEY_PROPERTY";
                case 0x8009200C: return "CRYPT_E_NO_DECRYPT_CERT";
                case 0x8009200D: return "CRYPT_E_BAD_MSG";
                case 0x8009200E: return "CRYPT_E_NO_SIGNER";
                case 0x8009200F: return "CRYPT_E_PENDING_CLOSE";
                case 0x80092010: return "CRYPT_E_REVOKED";
                case 0x80092011: return "CRYPT_E_NO_REVOCATION_DLL";
                case 0x80092012: return "CRYPT_E_NO_REVOCATION_CHECK";
                case 0x80092013: return "CRYPT_E_REVOCATION_OFFLINE";
                case 0x80092014: return "CRYPT_E_NOT_IN_REVOCATION_DATABASE";
                case 0x80092020: return "CRYPT_E_INVALID_NUMERIC_STRING";
                case 0x80092021: return "CRYPT_E_INVALID_PRINTABLE_STRING";
                case 0x80092022: return "CRYPT_E_INVALID_IA5_STRING";
                case 0x80092023: return "CRYPT_E_INVALID_X500_STRING";
                case 0x80092024: return "CRYPT_E_NOT_CHAR_STRING";
                case 0x80092025: return "CRYPT_E_FILERESIZED";
                case 0x80092026: return "CRYPT_E_SECURITY_SETTINGS";
                case 0x80092027: return "CRYPT_E_NO_VERIFY_USAGE_DLL";
                case 0x80092028: return "CRYPT_E_NO_VERIFY_USAGE_CHECK";
                case 0x80092029: return "CRYPT_E_VERIFY_USAGE_OFFLINE";
                case 0x8009202A: return "CRYPT_E_NOT_IN_CTL";
                case 0x8009202B: return "CRYPT_E_NO_TRUSTED_SIGNER";
                case 0x8009202C: return "CRYPT_E_MISSING_PUBKEY_PARA";
                case 0x80093000: return "CRYPT_E_OSS_ERROR";
                case 0x80093001: return "OSS_MORE_BUF";
                case 0x80093002: return "OSS_NEGATIVE_UINTEGER";
                case 0x80093003: return "OSS_PDU_RANGE";
                case 0x80093004: return "OSS_MORE_INPUT";
                case 0x80093005: return "OSS_DATA_ERROR";
                case 0x80093006: return "OSS_BAD_ARG";
                case 0x80093007: return "OSS_BAD_VERSION";
                case 0x80093008: return "OSS_OUT_MEMORY";
                case 0x80093009: return "OSS_PDU_MISMATCH";
                case 0x8009300A: return "OSS_LIMITED";
                case 0x8009300B: return "OSS_BAD_PTR";
                case 0x8009300C: return "OSS_BAD_TIME";
                case 0x8009300D: return "OSS_INDEFINITE_NOT_SUPPORTED";
                case 0x8009300E: return "OSS_MEM_ERROR";
                case 0x8009300F: return "OSS_BAD_TABLE";
                case 0x80093010: return "OSS_TOO_LONG";
                case 0x80093011: return "OSS_CONSTRAINT_VIOLATED";
                case 0x80093012: return "OSS_FATAL_ERROR";
                case 0x80093013: return "OSS_ACCESS_SERIALIZATION_ERROR";
                case 0x80093014: return "OSS_NULL_TBL";
                case 0x80093015: return "OSS_NULL_FCN";
                case 0x80093016: return "OSS_BAD_ENCRULES";
                case 0x80093017: return "OSS_UNAVAIL_ENCRULES";
                case 0x80093018: return "OSS_CANT_OPEN_TRACE_WINDOW";
                case 0x80093019: return "OSS_UNIMPLEMENTED";
                case 0x8009301A: return "OSS_OID_DLL_NOT_LINKED";
                case 0x8009301B: return "OSS_CANT_OPEN_TRACE_FILE";
                case 0x8009301C: return "OSS_TRACE_FILE_ALREADY_OPEN";
                case 0x8009301D: return "OSS_TABLE_MISMATCH";
                case 0x8009301E: return "OSS_TYPE_NOT_SUPPORTED";
                case 0x8009301F: return "OSS_REAL_DLL_NOT_LINKED";
                case 0x80093020: return "OSS_REAL_CODE_NOT_LINKED";
                case 0x80093021: return "OSS_OUT_OF_RANGE";
                case 0x80093022: return "OSS_COPIER_DLL_NOT_LINKED";
                case 0x80093023: return "OSS_CONSTRAINT_DLL_NOT_LINKED";
                case 0x80093024: return "OSS_COMPARATOR_DLL_NOT_LINKED";
                case 0x80093025: return "OSS_COMPARATOR_CODE_NOT_LINKED";
                case 0x80093026: return "OSS_MEM_MGR_DLL_NOT_LINKED";
                case 0x80093027: return "OSS_PDV_DLL_NOT_LINKED";
                case 0x80093028: return "OSS_PDV_CODE_NOT_LINKED";
                case 0x80093029: return "OSS_API_DLL_NOT_LINKED";
                case 0x8009302A: return "OSS_BERDER_DLL_NOT_LINKED";
                case 0x8009302B: return "OSS_PER_DLL_NOT_LINKED";
                case 0x8009302C: return "OSS_OPEN_TYPE_ERROR";
                case 0x8009302D: return "OSS_MUTEX_NOT_CREATED";
                case 0x8009302E: return "OSS_CANT_CLOSE_TRACE_FILE";
                case 0x80093100: return "CRYPT_E_ASN1_ERROR";
                case 0x80093101: return "CRYPT_E_ASN1_INTERNAL";
                case 0x80093102: return "CRYPT_E_ASN1_EOD";
                case 0x80093103: return "CRYPT_E_ASN1_CORRUPT";
                case 0x80093104: return "CRYPT_E_ASN1_LARGE";
                case 0x80093105: return "CRYPT_E_ASN1_CONSTRAINT";
                case 0x80093106: return "CRYPT_E_ASN1_MEMORY";
                case 0x80093107: return "CRYPT_E_ASN1_OVERFLOW";
                case 0x80093108: return "CRYPT_E_ASN1_BADPDU";
                case 0x80093109: return "CRYPT_E_ASN1_BADARGS";
                case 0x8009310A: return "CRYPT_E_ASN1_BADREAL";
                case 0x8009310B: return "CRYPT_E_ASN1_BADTAG";
                case 0x8009310C: return "CRYPT_E_ASN1_CHOICE";
                case 0x8009310D: return "CRYPT_E_ASN1_RULE";
                case 0x8009310E: return "CRYPT_E_ASN1_UTF8";
                case 0x80093133: return "CRYPT_E_ASN1_PDU_TYPE";
                case 0x80093134: return "CRYPT_E_ASN1_NYI";
                case 0x80093201: return "CRYPT_E_ASN1_EXTENDED";
                case 0x80093202: return "CRYPT_E_ASN1_NOEOD";
                case 0x80094001: return "CERTSRV_E_BAD_REQUESTSUBJECT";
                case 0x80094002: return "CERTSRV_E_NO_REQUEST";
                case 0x80094003: return "CERTSRV_E_BAD_REQUESTSTATUS";
                case 0x80094004: return "CERTSRV_E_PROPERTY_EMPTY";
                case 0x80094005: return "CERTSRV_E_INVALID_CA_CERTIFICATE";
                case 0x80094006: return "CERTSRV_E_SERVER_SUSPENDED";
                case 0x80094007: return "CERTSRV_E_ENCODING_LENGTH";
                case 0x80094008: return "CERTSRV_E_ROLECONFLICT";
                case 0x80094009: return "CERTSRV_E_RESTRICTEDOFFICER";
                case 0x8009400A: return "CERTSRV_E_KEY_ARCHIVAL_NOT_CONFIGURED";
                case 0x8009400B: return "CERTSRV_E_NO_VALID_KRA";
                case 0x8009400C: return "CERTSRV_E_BAD_REQUEST_KEY_ARCHIVAL";
                case 0x80094800: return "CERTSRV_E_UNSUPPORTED_CERT_TYPE";
                case 0x80094801: return "CERTSRV_E_NO_CERT_TYPE";
                case 0x80094802: return "CERTSRV_E_TEMPLATE_CONFLICT";
                case 0x80096001: return "TRUST_E_SYSTEM_ERROR";
                case 0x80096002: return "TRUST_E_NO_SIGNER_CERT";
                case 0x80096003: return "TRUST_E_COUNTER_SIGNER";
                case 0x80096004: return "TRUST_E_CERT_SIGNATURE";
                case 0x80096005: return "TRUST_E_TIME_STAMP";
                case 0x80096010: return "TRUST_E_BAD_DIGEST";
                case 0x80096019: return "TRUST_E_BASIC_CONSTRAINTS";
                case 0x8009601E: return "TRUST_E_FINANCIAL_CRITERIA";
                case 0x80097001: return "MSSIPOTF_E_OUTOFMEMRANGE";
                case 0x80097002: return "MSSIPOTF_E_CANTGETOBJECT";
                case 0x80097003: return "MSSIPOTF_E_NOHEADTABLE";
                case 0x80097004: return "MSSIPOTF_E_BAD_MAGICNUMBER";
                case 0x80097005: return "MSSIPOTF_E_BAD_OFFSET_TABLE";
                case 0x80097006: return "MSSIPOTF_E_TABLE_TAGORDER";
                case 0x80097007: return "MSSIPOTF_E_TABLE_LONGWORD";
                case 0x80097008: return "MSSIPOTF_E_BAD_FIRST_TABLE_PLACEMENT";
                case 0x80097009: return "MSSIPOTF_E_TABLES_OVERLAP";
                case 0x8009700A: return "MSSIPOTF_E_TABLE_PADBYTES";
                case 0x8009700B: return "MSSIPOTF_E_FILETOOSMALL";
                case 0x8009700C: return "MSSIPOTF_E_TABLE_CHECKSUM";
                case 0x8009700D: return "MSSIPOTF_E_FILE_CHECKSUM";
                case 0x80097010: return "MSSIPOTF_E_FAILED_POLICY";
                case 0x80097011: return "MSSIPOTF_E_FAILED_HINTS_CHECK";
                case 0x80097012: return "MSSIPOTF_E_NOT_OPENTYPE";
                case 0x80097013: return "MSSIPOTF_E_FILE";
                case 0x80097014: return "MSSIPOTF_E_CRYPT";
                case 0x80097015: return "MSSIPOTF_E_BADVERSION";
                case 0x80097016: return "MSSIPOTF_E_DSIG_STRUCTURE";
                case 0x80097017: return "MSSIPOTF_E_PCONST_CHECK";
                case 0x80097018: return "MSSIPOTF_E_STRUCTURE";
                case 0x800B0001: return "TRUST_E_PROVIDER_UNKNOWN";
                case 0x800B0002: return "TRUST_E_ACTION_UNKNOWN";
                case 0x800B0003: return "TRUST_E_SUBJECT_FORM_UNKNOWN";
                case 0x800B0004: return "TRUST_E_SUBJECT_NOT_TRUSTED";
                case 0x800B0005: return "DIGSIG_E_ENCODE";
                case 0x800B0006: return "DIGSIG_E_DECODE";
                case 0x800B0007: return "DIGSIG_E_EXTENSIBILITY";
                case 0x800B0008: return "DIGSIG_E_CRYPTO";
                case 0x800B0009: return "PERSIST_E_SIZEDEFINITE";
                case 0x800B000A: return "PERSIST_E_SIZEINDEFINITE";
                case 0x800B000B: return "PERSIST_E_NOTSELFSIZING";
                case 0x800B0100: return "TRUST_E_NOSIGNATURE";
                case 0x800B0101: return "CERT_E_EXPIRED";
                case 0x800B0102: return "CERT_E_VALIDITYPERIODNESTING";
                case 0x800B0103: return "CERT_E_ROLE";
                case 0x800B0104: return "CERT_E_PATHLENCONST";
                case 0x800B0105: return "CERT_E_CRITICAL";
                case 0x800B0106: return "CERT_E_PURPOSE";
                case 0x800B0107: return "CERT_E_ISSUERCHAINING";
                case 0x800B0108: return "CERT_E_MALFORMED";
                case 0x800B0109: return "CERT_E_UNTRUSTEDROOT";
                case 0x800B010A: return "CERT_E_CHAINING";
                case 0x800B010B: return "TRUST_E_FAIL";
                case 0x800B010C: return "CERT_E_REVOKED";
                case 0x800B010D: return "CERT_E_UNTRUSTEDTESTROOT";
                case 0x800B010E: return "CERT_E_REVOCATION_FAILURE";
                case 0x800B010F: return "CERT_E_CN_NO_MATCH";
                case 0x800B0110: return "CERT_E_WRONG_USAGE";
                case 0x800B0111: return "TRUST_E_EXPLICIT_DISTRUST";
                case 0x800B0112: return "CERT_E_UNTRUSTEDCA";
                case 0x800B0113: return "CERT_E_INVALID_POLICY";
                case 0x800B0114: return "CERT_E_INVALID_NAME";
            }

            return string.Format("0x{0:x} [{1}]", statusCode, statusCode);
        }
#endif // TRACE_VERBOSE
    }

    public partial class Interop
    {
        internal partial class Kernel32
        {
            [DllImport(Libraries.Kernel32, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool CloseHandle(IntPtr handle);
        }
    }

    public static partial class Interop
    {
        public static partial class SspiCli
        {
            internal const uint SECQOP_WRAP_NO_ENCRYPT = 0x80000001;

            internal const int SEC_I_RENEGOTIATE = 0x90321;

            internal const int SECPKG_NEGOTIATION_COMPLETE = 0;
            internal const int SECPKG_NEGOTIATION_OPTIMISTIC = 1;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct CredHandle
            {
                private IntPtr dwLower;
                private IntPtr dwUpper;

                public bool IsZero
                {
                    get { return dwLower == IntPtr.Zero && dwUpper == IntPtr.Zero; }
                }

                internal void SetToInvalid()
                {
                    dwLower = IntPtr.Zero;
                    dwUpper = IntPtr.Zero;
                }

                public override string ToString()
                {
                    { return dwLower.ToString("x") + ":" + dwUpper.ToString("x"); }
                }
            }

            internal enum ContextAttribute
            {
                // sspi.h
                SECPKG_ATTR_SIZES = 0,
                SECPKG_ATTR_NAMES = 1,
                SECPKG_ATTR_LIFESPAN = 2,
                SECPKG_ATTR_DCE_INFO = 3,
                SECPKG_ATTR_STREAM_SIZES = 4,
                SECPKG_ATTR_AUTHORITY = 6,
                SECPKG_ATTR_PACKAGE_INFO = 10,
                SECPKG_ATTR_NEGOTIATION_INFO = 12,
                SECPKG_ATTR_UNIQUE_BINDINGS = 25,
                SECPKG_ATTR_ENDPOINT_BINDINGS = 26,
                SECPKG_ATTR_CLIENT_SPECIFIED_TARGET = 27,
                SECPKG_ATTR_APPLICATION_PROTOCOL = 35,

                // minschannel.h
                SECPKG_ATTR_REMOTE_CERT_CONTEXT = 0x53,    // returns PCCERT_CONTEXT
                SECPKG_ATTR_LOCAL_CERT_CONTEXT = 0x54,     // returns PCCERT_CONTEXT
                SECPKG_ATTR_ROOT_STORE = 0x55,             // returns HCERTCONTEXT to the root store
                SECPKG_ATTR_ISSUER_LIST_EX = 0x59,         // returns SecPkgContext_IssuerListInfoEx
                SECPKG_ATTR_CONNECTION_INFO = 0x5A,        // returns SecPkgContext_ConnectionInfo
                SECPKG_ATTR_UI_INFO = 0x68, // sets SEcPkgContext_UiInfo  
            }

            // These values are defined within sspi.h as ISC_REQ_*, ISC_RET_*, ASC_REQ_* and ASC_RET_*.
            [Flags]
            internal enum ContextFlags
            {
                Zero = 0,
                // The server in the transport application can
                // build new security contexts impersonating the
                // client that will be accepted by other servers
                // as the client's contexts.
                Delegate = 0x00000001,
                // The communicating parties must authenticate
                // their identities to each other. Without MutualAuth,
                // the client authenticates its identity to the server.
                // With MutualAuth, the server also must authenticate
                // its identity to the client.
                MutualAuth = 0x00000002,
                // The security package detects replayed packets and
                // notifies the caller if a packet has been replayed.
                // The use of this flag implies all of the conditions
                // specified by the Integrity flag.
                ReplayDetect = 0x00000004,
                // The context must be allowed to detect out-of-order
                // delivery of packets later through the message support
                // functions. Use of this flag implies all of the
                // conditions specified by the Integrity flag.
                SequenceDetect = 0x00000008,
                // The context must protect data while in transit.
                // Confidentiality is supported for NTLM with Microsoft
                // Windows NT version 4.0, SP4 and later and with the
                // Kerberos protocol in Microsoft Windows 2000 and later.
                Confidentiality = 0x00000010,
                UseSessionKey = 0x00000020,
                AllocateMemory = 0x00000100,

                // Connection semantics must be used.
                Connection = 0x00000800,

                // Client applications requiring extended error messages specify the
                // ISC_REQ_EXTENDED_ERROR flag when calling the InitializeSecurityContext
                // Server applications requiring extended error messages set
                // the ASC_REQ_EXTENDED_ERROR flag when calling AcceptSecurityContext.
                InitExtendedError = 0x00004000,
                AcceptExtendedError = 0x00008000,
                // A transport application requests stream semantics
                // by setting the ISC_REQ_STREAM and ASC_REQ_STREAM
                // flags in the calls to the InitializeSecurityContext
                // and AcceptSecurityContext functions
                InitStream = 0x00008000,
                AcceptStream = 0x00010000,
                // Buffer integrity can be verified; however, replayed
                // and out-of-sequence messages will not be detected
                InitIntegrity = 0x00010000,       // ISC_REQ_INTEGRITY
                AcceptIntegrity = 0x00020000,       // ASC_REQ_INTEGRITY

                InitManualCredValidation = 0x00080000,   // ISC_REQ_MANUAL_CRED_VALIDATION
                InitUseSuppliedCreds = 0x00000080,   // ISC_REQ_USE_SUPPLIED_CREDS
                InitIdentify = 0x00020000,   // ISC_REQ_IDENTIFY
                AcceptIdentify = 0x00080000,   // ASC_REQ_IDENTIFY

                ProxyBindings = 0x04000000,   // ASC_REQ_PROXY_BINDINGS
                AllowMissingBindings = 0x10000000,   // ASC_REQ_ALLOW_MISSING_BINDINGS

                UnverifiedTargetName = 0x20000000,   // ISC_REQ_UNVERIFIED_TARGET_NAME
            }

            internal enum Endianness
            {
                SECURITY_NETWORK_DREP = 0x00,
                SECURITY_NATIVE_DREP = 0x10,
            }

            public enum CredentialUse
            {
                SECPKG_CRED_INBOUND = 0x1,
                SECPKG_CRED_OUTBOUND = 0x2,
                SECPKG_CRED_BOTH = 0x3,
            }

            // wincrypt.h
            [StructLayout(LayoutKind.Sequential)]
            internal struct CERT_CHAIN_ELEMENT
            {
                public uint cbSize;
                public IntPtr pCertContext;
                // Since this structure is allocated by unmanaged code, we can
                // omit the fields below since we don't need to access them
                // CERT_TRUST_STATUS   TrustStatus;
                // IntPtr                pRevocationInfo;
                // IntPtr                pIssuanceUsage;
                // IntPtr                pApplicationUsage;
            }

            // schannel.h
            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct SecPkgContext_IssuerListInfoEx
            {
                public SafeHandle aIssuers;
                public uint cIssuers;

                public unsafe SecPkgContext_IssuerListInfoEx(SafeHandle handle, byte[] nativeBuffer)
                {
                    aIssuers = handle;
                    fixed (byte* voidPtr = nativeBuffer)
                    {
                        // TODO (Issue #3114): Properly marshal the struct instead of assuming no padding.
                        cIssuers = *((uint*)(voidPtr + IntPtr.Size));
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SCHANNEL_CRED
            {
                public const int CurrentVersion = 0x4;

                public int dwVersion;
                public int cCreds;

                // ptr to an array of pointers
                // There is a hack done with this field.  AcquireCredentialsHandle requires an array of
                // certificate handles; we only ever use one.  In order to avoid pinning a one element array,
                // we copy this value onto the stack, create a pointer on the stack to the copied value,
                // and replace this field with the pointer, during the call to AcquireCredentialsHandle.
                // Then we fix it up afterwards.  Fine as long as all the SSPI credentials are not
                // supposed to be threadsafe.
                public IntPtr paCred;

                public IntPtr hRootStore;               // == always null, OTHERWISE NOT RELIABLE
                public int cMappers;
                public IntPtr aphMappers;               // == always null, OTHERWISE NOT RELIABLE
                public int cSupportedAlgs;
                public IntPtr palgSupportedAlgs;       // == always null, OTHERWISE NOT RELIABLE
                public int grbitEnabledProtocols;
                public int dwMinimumCipherStrength;
                public int dwMaximumCipherStrength;
                public int dwSessionLifespan;
                public SCHANNEL_CRED.Flags dwFlags;
                public int reserved;

                [Flags]
                public enum Flags
                {
                    Zero = 0,
                    SCH_CRED_NO_SYSTEM_MAPPER = 0x02,
                    SCH_CRED_NO_SERVERNAME_CHECK = 0x04,
                    SCH_CRED_MANUAL_CRED_VALIDATION = 0x08,
                    SCH_CRED_NO_DEFAULT_CREDS = 0x10,
                    SCH_CRED_AUTO_CRED_VALIDATION = 0x20,
                    SCH_SEND_AUX_RECORD = 0x00200000,
                    SCH_USE_STRONG_CRYPTO = 0x00400000,
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct SecBuffer
            {
                public int cbBuffer;
                public SecurityBufferType BufferType;
                public IntPtr pvBuffer;

                public static readonly int Size = sizeof(SecBuffer);
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct SecBufferDesc
            {
                public readonly int ulVersion;
                public readonly int cBuffers;
                public void* pBuffers;

                public SecBufferDesc(int count)
                {
                    ulVersion = 0;
                    cBuffers = count;
                    pBuffers = null;
                }
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct SEC_WINNT_AUTH_IDENTITY_W
            {
                internal string User;
                internal int UserLength;
                internal string Domain;
                internal int DomainLength;
                internal string Password;
                internal int PasswordLength;
                internal int Flags;
            }

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern int EncryptMessage(
                ref CredHandle contextHandle,
                [In] uint qualityOfProtection,
                [In, Out] ref SecBufferDesc inputOutput,
                [In] uint sequenceNumber
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int DecryptMessage(
                [In] ref CredHandle contextHandle,
                [In, Out] ref SecBufferDesc inputOutput,
                [In] uint sequenceNumber,
                uint* qualityOfProtection
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern int QuerySecurityContextToken(
                ref CredHandle phContext,
                [Out] out SecurityContextTokenHandle handle);

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern int FreeContextBuffer(
                [In] IntPtr contextBuffer);

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern int FreeCredentialsHandle(
                ref CredHandle handlePtr
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern int DeleteSecurityContext(
                ref CredHandle handlePtr
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int AcceptSecurityContext(
                ref CredHandle credentialHandle,
                [In] void* inContextPtr,
                [In] SecBufferDesc* inputBuffer,
                [In] ContextFlags inFlags,
                [In] Endianness endianness,
                ref CredHandle outContextPtr,
                [In, Out] ref SecBufferDesc outputBuffer,
                [In, Out] ref ContextFlags attributes,
                out long timeStamp
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int QueryContextAttributesW(
                ref CredHandle contextHandle,
                [In] ContextAttribute attribute,
                [In] void* buffer);

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int SetContextAttributesW(
                ref CredHandle contextHandle,
                [In] ContextAttribute attribute,
                [In] byte[] buffer,
                [In] int bufferSize);

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern int EnumerateSecurityPackagesW(
                [Out] out int pkgnum,
                [Out] out SafeFreeContextBuffer_SECURITY handle);

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern unsafe int AcquireCredentialsHandleW(
                [In] string principal,
                [In] string moduleName,
                [In] int usage,
                [In] void* logonID,
                [In] ref SEC_WINNT_AUTH_IDENTITY_W authdata,
                [In] void* keyCallback,
                [In] void* keyArgument,
                ref CredHandle handlePtr,
                [Out] out long timeStamp
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern unsafe int AcquireCredentialsHandleW(
                [In] string principal,
                [In] string moduleName,
                [In] int usage,
                [In] void* logonID,
                [In] IntPtr zero,
                [In] void* keyCallback,
                [In] void* keyArgument,
                ref CredHandle handlePtr,
                [Out] out long timeStamp
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern unsafe int AcquireCredentialsHandleW(
                [In] string principal,
                [In] string moduleName,
                [In] int usage,
                [In] void* logonID,
                [In] SafeSspiAuthDataHandle authdata,
                [In] void* keyCallback,
                [In] void* keyArgument,
                ref CredHandle handlePtr,
                [Out] out long timeStamp
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern unsafe int AcquireCredentialsHandleW(
                [In] string principal,
                [In] string moduleName,
                [In] int usage,
                [In] void* logonID,
                [In] ref SCHANNEL_CRED authData,
                [In] void* keyCallback,
                [In] void* keyArgument,
                ref CredHandle handlePtr,
                [Out] out long timeStamp
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int InitializeSecurityContextW(
                ref CredHandle credentialHandle,
                [In] void* inContextPtr,
                [In] byte* targetName,
                [In] ContextFlags inFlags,
                [In] int reservedI,
                [In] Endianness endianness,
                [In] SecBufferDesc* inputBuffer,
                [In] int reservedII,
                ref CredHandle outContextPtr,
                [In, Out] ref SecBufferDesc outputBuffer,
                [In, Out] ref ContextFlags attributes,
                out long timeStamp
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int CompleteAuthToken(
                [In] void* inContextPtr,
                [In, Out] ref SecBufferDesc inputBuffers
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe int ApplyControlToken(
                [In] void* inContextPtr,
                [In, Out] ref SecBufferDesc inputBuffers
            );

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, SetLastError = true)]
            internal static extern unsafe SECURITY_STATUS SspiFreeAuthIdentity(
                [In] IntPtr authData);

            [DllImport(Interop.Libraries.SspiCli, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern unsafe SECURITY_STATUS SspiEncodeStringsAsAuthIdentity(
                [In] string userName,
                [In] string domainName,
                [In] string password,
                [Out] out SafeSspiAuthDataHandle authData);
        }
    }

    public partial class Interop
    {
        internal partial class Kernel32
        {
            public const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
            public const int LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

            [DllImport(Libraries.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern SafeLibraryHandle LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);
        }
    }

    public partial class Interop
    {
        internal partial class Kernel32
        {
            [DllImport(Libraries.Kernel32, ExactSpelling = true, SetLastError = true)]
            public static extern unsafe bool FreeLibrary([In] IntPtr hModule);
        }
    }

    public partial class Interop
    {
        internal partial class Kernel32
        {
            [DllImport(Libraries.Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
            public static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, string lpProcName);

            [DllImport(Libraries.Kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        }
    }

    public static partial class Interop
    {
        internal static partial class Libraries
        {
            internal const string Advapi32 = "advapi32.dll";
            internal const string BCrypt = "BCrypt.dll";
            internal const string CoreComm_L1_1_1 = "api-ms-win-core-comm-l1-1-1.dll";
            internal const string Crypt32 = "crypt32.dll";
            internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";
            internal const string HttpApi = "httpapi.dll";
            internal const string IpHlpApi = "iphlpapi.dll";
            internal const string Kernel32 = "kernel32.dll";
            internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";
            internal const string Mswsock = "mswsock.dll";
            internal const string NCrypt = "ncrypt.dll";
            internal const string NtDll = "ntdll.dll";
            internal const string Odbc32 = "odbc32.dll";
            internal const string OleAut32 = "oleaut32.dll";
            internal const string PerfCounter = "perfcounter.dll";
            internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";
            internal const string Secur32 = "secur32.dll";
            internal const string Shell32 = "shell32.dll";
            internal const string SspiCli = "sspicli.dll";
            internal const string User32 = "user32.dll";
            internal const string Version = "version.dll";
            internal const string WebSocket = "websocket.dll";
            internal const string WinHttp = "winhttp.dll";
            internal const string Ws2_32 = "ws2_32.dll";
            internal const string CompressionNative = "clrcompression.dll";
        }
    }

    public static partial class Interop
    {
        internal enum ApplicationProtocolNegotiationStatus
        {
            None = 0,
            Success,
            SelectedClientOnly
        }

        internal enum ApplicationProtocolNegotiationExt
        {
            None = 0,
            NPN,
            ALPN
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class SecPkgContext_ApplicationProtocol
        {
            private const int MaxProtocolIdSize = 0xFF;

            public ApplicationProtocolNegotiationStatus ProtoNegoStatus;
            public ApplicationProtocolNegotiationExt ProtoNegoExt;
            public byte ProtocolIdSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxProtocolIdSize)]
            public byte[] ProtocolId;
            public byte[] Protocol
            {
                get
                {
                    return ProtocolId.Take(ProtocolIdSize).ToArray();
                }
            }
        }
    }

}