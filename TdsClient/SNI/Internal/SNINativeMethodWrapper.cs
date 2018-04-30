using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.SNI.Internal
{
    internal static partial class SniNativeMethodWrapper
    {
        private const string SNI = "sni.dll";

        private const int SniOpenTimeOut = -1; // infinite

        private static int s_sniMaxComposedSpnLength = -1;

        internal static int SniMaxComposedSpnLength
        {
            get
            {
                if (s_sniMaxComposedSpnLength == -1) s_sniMaxComposedSpnLength = checked((int) GetSniMaxComposedSpnLength());
                return s_sniMaxComposedSpnLength;
            }
        }

        internal static uint SniGetConnectionId(SniHandle pConn, ref Guid connId)
        {
            return SNIGetInfoWrapper(pConn, QTypes.SNI_QUERY_CONN_CONNID, out connId);
        }

        internal static uint SNIInitialize()
        {
            return SNIInitialize(IntPtr.Zero);
        }

        internal static uint SNIOpenMarsSession(ConsumerInfo consumerInfo, SniHandle parent, ref IntPtr pConn, bool fSync)
        {
            // initialize consumer info for MARS
            var native_consumerInfo = new Sni_Consumer_Info();
            MarshalConsumerInfo(consumerInfo, ref native_consumerInfo);

            return SNIOpenWrapper(ref native_consumerInfo, "session:", parent, out pConn, fSync);
        }

        internal static unsafe uint SNIOpenSyncEx(ConsumerInfo consumerInfo, string constring, ref IntPtr pConn, byte[] spnBuffer, byte[] instanceName, bool fOverrideCache, bool fSync, int timeout, bool fParallel)
        {
            fixed (byte* pin_instanceName = &instanceName[0])
            {
                var clientConsumerInfo = new SNI_CLIENT_CONSUMER_INFO();

                // initialize client ConsumerInfo part first
                MarshalConsumerInfo(consumerInfo, ref clientConsumerInfo.ConsumerInfo);

                clientConsumerInfo.wszConnectionString = constring;
                clientConsumerInfo.networkLibrary = PrefixEnum.UNKNOWN_PREFIX;

                clientConsumerInfo.szInstanceName = pin_instanceName;
                clientConsumerInfo.cchInstanceName = (uint) instanceName.Length;
                clientConsumerInfo.fOverrideLastConnectCache = fOverrideCache;
                clientConsumerInfo.fSynchronousConnection = fSync;
                clientConsumerInfo.timeout = timeout;
                clientConsumerInfo.fParallel = fParallel;

                clientConsumerInfo.transparentNetworkResolution = TransparentNetworkResolutionMode.DisabledMode;
                clientConsumerInfo.totalTimeout = SniOpenTimeOut;
                clientConsumerInfo.isAzureSqlServerEndpoint = ADP.IsAzureSqlServerEndpoint(constring);

                if (spnBuffer != null)
                    fixed (byte* pin_spnBuffer = &spnBuffer[0])
                    {
                        clientConsumerInfo.szSPN = pin_spnBuffer;
                        clientConsumerInfo.cchSPN = (uint) spnBuffer.Length;
                        return SNIOpenSyncExWrapper(ref clientConsumerInfo, out pConn);
                    }

                return SNIOpenSyncExWrapper(ref clientConsumerInfo, out pConn);
            }
        }

        internal static void SNIPacketAllocate(SafeHandle pConn, IOType IOType, ref IntPtr pPacket)
        {
            pPacket = SNIPacketAllocateWrapper(pConn, IOType);
        }

        internal static uint SNIPacketGetData(IntPtr packet, byte[] readBuffer, ref uint dataSize)
        {
            return SNIPacketGetDataWrapper(packet, readBuffer, (uint) readBuffer.Length, out dataSize);
        }

        internal static unsafe void SNIPacketSetData(SniPacket packet, byte[] data, int length)
        {
            fixed (byte* pin_data = &data[0])
            {
                SNIPacketSetData(packet, pin_data, (uint) length);
            }
        }

        internal static unsafe uint SNISecGenClientContext(SniHandle pConnectionObject, byte[] inBuff, uint receivedLength, byte[] OutBuff, ref uint sendLength, byte[] serverUserName)
        {
            fixed (byte* pin_serverUserName = &serverUserName[0])
            {
                bool local_fDone;
                return SNISecGenClientContextWrapper(
                    pConnectionObject,
                    inBuff,
                    receivedLength,
                    OutBuff,
                    ref sendLength,
                    out local_fDone,
                    pin_serverUserName,
                    (uint) serverUserName.Length,
                    null,
                    null);
            }
        }

        internal static uint SNIWritePacket(SniHandle pConn, SniPacket packet, bool sync)
        {
            if (sync)
                return SNIWriteSyncOverAsync(pConn, packet);
            return SNIWriteAsyncWrapper(pConn, packet);
        }

        private static void MarshalConsumerInfo(ConsumerInfo consumerInfo, ref Sni_Consumer_Info native_consumerInfo)
        {
            native_consumerInfo.DefaultUserDataLength = consumerInfo.defaultBufferSize;
            native_consumerInfo.fnReadComp = null != consumerInfo.readDelegate
                ? Marshal.GetFunctionPointerForDelegate(consumerInfo.readDelegate)
                : IntPtr.Zero;
            native_consumerInfo.fnWriteComp = null != consumerInfo.writeDelegate
                ? Marshal.GetFunctionPointerForDelegate(consumerInfo.writeDelegate)
                : IntPtr.Zero;
            native_consumerInfo.ConsumerKey = consumerInfo.key;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void SqlAsyncCallbackDelegate(IntPtr m_ConsKey, IntPtr pPacket, uint dwError);

        #region Structs\Enums

        [StructLayout(LayoutKind.Sequential)]
        internal struct ConsumerInfo
        {
            internal int defaultBufferSize;
            internal SqlAsyncCallbackDelegate readDelegate;
            internal SqlAsyncCallbackDelegate writeDelegate;
            internal IntPtr key;
        }

        internal enum ConsumerNumber
        {
            SNI_Consumer_SNI,
            SNI_Consumer_SSB,
            SNI_Consumer_PacketIsReleased,
            SNI_Consumer_Invalid
        }

        internal enum IOType
        {
            READ,
            WRITE
        }

        internal enum PrefixEnum
        {
            UNKNOWN_PREFIX,
            SM_PREFIX,
            TCP_PREFIX,
            NP_PREFIX,
            VIA_PREFIX,
            INVALID_PREFIX
        }

        internal enum ProviderEnum
        {
            HTTP_PROV,
            NP_PROV,
            SESSION_PROV,
            SIGN_PROV,
            SM_PROV,
            SMUX_PROV,
            SSL_PROV,
            TCP_PROV,
            VIA_PROV,
            MAX_PROVS,
            INVALID_PROV
        }

        internal enum QTypes
        {
            SNI_QUERY_CONN_INFO,
            SNI_QUERY_CONN_BUFSIZE,
            SNI_QUERY_CONN_KEY,
            SNI_QUERY_CLIENT_ENCRYPT_POSSIBLE,
            SNI_QUERY_SERVER_ENCRYPT_POSSIBLE,
            SNI_QUERY_CERTIFICATE,
            SNI_QUERY_LOCALDB_HMODULE,
            SNI_QUERY_CONN_ENCRYPT,
            SNI_QUERY_CONN_PROVIDERNUM,
            SNI_QUERY_CONN_CONNID,
            SNI_QUERY_CONN_PARENTCONNID,
            SNI_QUERY_CONN_SECPKG,
            SNI_QUERY_CONN_NETPACKETSIZE,
            SNI_QUERY_CONN_NODENUM,
            SNI_QUERY_CONN_PACKETSRECD,
            SNI_QUERY_CONN_PACKETSSENT,
            SNI_QUERY_CONN_PEERADDR,
            SNI_QUERY_CONN_PEERPORT,
            SNI_QUERY_CONN_LASTREADTIME,
            SNI_QUERY_CONN_LASTWRITETIME,
            SNI_QUERY_CONN_CONSUMER_ID,
            SNI_QUERY_CONN_CONNECTTIME,
            SNI_QUERY_CONN_HTTPENDPOINT,
            SNI_QUERY_CONN_LOCALADDR,
            SNI_QUERY_CONN_LOCALPORT,
            SNI_QUERY_CONN_SSLHANDSHAKESTATE,
            SNI_QUERY_CONN_SOBUFAUTOTUNING,
            SNI_QUERY_CONN_SECPKGNAME,
            SNI_QUERY_CONN_SECPKGMUTUALAUTH,
            SNI_QUERY_CONN_CONSUMERCONNID,
            SNI_QUERY_CONN_SNIUCI,
            SNI_QUERY_CONN_SUPPORTS_EXTENDED_PROTECTION,
            SNI_QUERY_CONN_CHANNEL_PROVIDES_AUTHENTICATION_CONTEXT,
            SNI_QUERY_CONN_PEERID,
            SNI_QUERY_CONN_SUPPORTS_SYNC_OVER_ASYNC
        }

        internal enum TransparentNetworkResolutionMode : byte
        {
            DisabledMode = 0,
            SequentialMode,
            ParallelMode
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Sni_Consumer_Info
        {
            public int DefaultUserDataLength;
            public IntPtr ConsumerKey;
            public IntPtr fnReadComp;
            public IntPtr fnWriteComp;
            public readonly IntPtr fnTrace;
            public readonly IntPtr fnAcceptComp;
            public readonly uint dwNumProts;
            public readonly IntPtr rgListenInfo;
            public readonly IntPtr NodeAffinity;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct SNI_CLIENT_CONSUMER_INFO
        {
            public Sni_Consumer_Info ConsumerInfo;
            [MarshalAs(UnmanagedType.LPWStr)] public string wszConnectionString;
            public PrefixEnum networkLibrary;
            public byte* szSPN;
            public uint cchSPN;
            public byte* szInstanceName;
            public uint cchInstanceName;
            [MarshalAs(UnmanagedType.Bool)] public bool fOverrideLastConnectCache;
            [MarshalAs(UnmanagedType.Bool)] public bool fSynchronousConnection;
            public int timeout;
            [MarshalAs(UnmanagedType.Bool)] public bool fParallel;
            public TransparentNetworkResolutionMode transparentNetworkResolution;
            public int totalTimeout;
            public bool isAzureSqlServerEndpoint;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SNI_Error
        {
            internal ProviderEnum provider;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
            internal string errorMessage;

            internal uint nativeError;
            internal uint sniError;
            [MarshalAs(UnmanagedType.LPWStr)] internal string fileName;
            [MarshalAs(UnmanagedType.LPWStr)] internal string function;
            internal uint lineNumber;
        }

        #endregion

        #region DLL Imports

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIAddProviderWrapper")]
        internal static extern uint SNIAddProvider(SniHandle pConn, ProviderEnum ProvNum, [In] ref uint pInfo);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNICheckConnectionWrapper")]
        internal static extern uint SNICheckConnection([In] SniHandle pConn);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNICloseWrapper")]
        internal static extern uint SNIClose(IntPtr pConn);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SNIGetLastError(out SNI_Error pErrorStruct);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SNIPacketRelease(IntPtr pPacket);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIPacketResetWrapper")]
        internal static extern void SNIPacketReset([In] SniHandle pConn, IOType IOType, SniPacket pPacket, ConsumerNumber ConsNum);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SNIQueryInfo(QTypes QType, ref uint pbQInfo);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SNIQueryInfo(QTypes QType, ref IntPtr pbQInfo);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIReadAsyncWrapper")]
        internal static extern uint SNIReadAsync(SniHandle pConn, ref IntPtr ppNewPacket);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SNIReadSyncOverAsync(SniHandle pConn, ref IntPtr ppNewPacket, int timeout);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIRemoveProviderWrapper")]
        internal static extern uint SNIRemoveProvider(SniHandle pConn, ProviderEnum ProvNum);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SNISecInitPackage(ref uint pcbMaxToken);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNISetInfoWrapper")]
        internal static extern uint SNISetInfo(SniHandle pConn, QTypes QType, [In] ref uint pbQInfo);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SNITerminate();

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SNIWaitForSSLHandshakeToCompleteWrapper")]
        internal static extern uint SNIWaitForSSLHandshakeToComplete([In] SniHandle pConn, int dwMilliseconds);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint UnmanagedIsTokenRestricted([In] IntPtr token, [MarshalAs(UnmanagedType.Bool)] out bool isRestricted);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint GetSniMaxComposedSpnLength();

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIGetInfoWrapper([In] SniHandle pConn, QTypes QType, out Guid pbQInfo);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIInitialize([In] IntPtr pmo);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIOpenSyncExWrapper(ref SNI_CLIENT_CONSUMER_INFO pClientConsumerInfo, out IntPtr ppConn);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIOpenWrapper(
            [In] ref Sni_Consumer_Info pConsumerInfo,
            [MarshalAs(UnmanagedType.LPStr)] string szConnect,
            [In] SniHandle pConn,
            out IntPtr ppConn,
            [MarshalAs(UnmanagedType.Bool)] bool fSync);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SNIPacketAllocateWrapper([In] SafeHandle pConn, IOType IOType);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIPacketGetDataWrapper([In] IntPtr packet, [In] [Out] byte[] readBuffer, uint readBufferLength, out uint dataSize);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void SNIPacketSetData(SniPacket pPacket, [In] byte* pbBuf, uint cbBuf);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe uint SNISecGenClientContextWrapper(
            [In] SniHandle pConn,
            [In] [Out] byte[] pIn,
            uint cbIn,
            [In] [Out] byte[] pOut,
            [In] ref uint pcbOut,
            [MarshalAs(UnmanagedType.Bool)] out bool pfDone,
            byte* szServerInfo,
            uint cbServerInfo,
            [MarshalAs(UnmanagedType.LPWStr)] string pwszUserName,
            [MarshalAs(UnmanagedType.LPWStr)] string pwszPassword);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIWriteAsyncWrapper(SniHandle pConn, [In] SniPacket pPacket);

        [DllImport(SNI, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SNIWriteSyncOverAsync(SniHandle pConn, [In] SniPacket pPacket);

        #endregion
    }

    internal static partial class SniNativeMethodWrapper
    {
        internal enum SniSpecialErrors : uint
        {
            LocalDBErrorCode = SniCommon.LocalDBErrorCode,

            // multi-subnet-failover specific error codes
            MultiSubnetFailoverWithMoreThan64IPs = SniCommon.MultiSubnetFailoverWithMoreThan64IPs,
            MultiSubnetFailoverWithInstanceSpecified = SniCommon.MultiSubnetFailoverWithInstanceSpecified,
            MultiSubnetFailoverWithNonTcpProtocol = SniCommon.MultiSubnetFailoverWithNonTcpProtocol,

            // max error code value
            MaxErrorValue = SniCommon.MaxErrorValue
        }
    }
}