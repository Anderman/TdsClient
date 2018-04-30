using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    public abstract partial class SafeDeleteContext : SafeHandle
        {
            private const string dummyStr = " ";
            private static readonly byte[] s_dummyBytes = new byte[] { 0 };

            protected SafeFreeCredentials _EffectiveCredential;

            //-------------------------------------------------------------------
            internal static unsafe int InitializeSecurityContext(
                ref SafeFreeCredentials inCredentials,
                ref SafeDeleteContext refContext,
                string targetName,
                Interop.SspiCli.ContextFlags inFlags,
                Interop.SspiCli.Endianness endianness,
                SecurityBuffer inSecBuffer,
                SecurityBuffer[] inSecBuffers,
                SecurityBuffer outSecBuffer,
                ref Interop.SspiCli.ContextFlags outFlags)
            {
#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(null, $"credential:{inCredentials}, crefContext:{refContext}, targetName:{targetName}, inFlags:{inFlags}, endianness:{endianness}");
                if (inSecBuffers == null)
                {
                    NetEventSource.Info(null, $"inSecBuffers = (null)");
                }
                else
                {
                    NetEventSource.Info(null, $"inSecBuffers = {inSecBuffers}");
                }
            }
#endif

                if (outSecBuffer == null)
                {
                    NetEventSource.Fail(null, "outSecBuffer != null");
                }
                if (inSecBuffer != null && inSecBuffers != null)
                {
                    NetEventSource.Fail(null, "inSecBuffer == null || inSecBuffers == null");
                }

                if (inCredentials == null)
                {
                    throw new ArgumentNullException(nameof(inCredentials));
                }

                Interop.SspiCli.SecBufferDesc inSecurityBufferDescriptor = default(Interop.SspiCli.SecBufferDesc);
                bool haveInSecurityBufferDescriptor = false;
                if (inSecBuffer != null)
                {
                    inSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(1);
                    haveInSecurityBufferDescriptor = true;
                }
                else if (inSecBuffers != null)
                {
                    inSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);
                    haveInSecurityBufferDescriptor = true;
                }

                Interop.SspiCli.SecBufferDesc outSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(1);

                // Actually, this is returned in outFlags.
                bool isSspiAllocated = (inFlags & Interop.SspiCli.ContextFlags.AllocateMemory) != 0 ? true : false;

                int errorCode = -1;

                Interop.SspiCli.CredHandle contextHandle = new Interop.SspiCli.CredHandle();
                if (refContext != null)
                {
                    contextHandle = refContext._handle;
                }

                // These are pinned user byte arrays passed along with SecurityBuffers.
                GCHandle[] pinnedInBytes = null;
                GCHandle pinnedOutBytes = new GCHandle();

                // Optional output buffer that may need to be freed.
                SafeFreeContextBuffer outFreeContextBuffer = null;
                try
                {
                    pinnedOutBytes = GCHandle.Alloc(outSecBuffer.token, GCHandleType.Pinned);
                    Interop.SspiCli.SecBuffer[] inUnmanagedBuffer = new Interop.SspiCli.SecBuffer[haveInSecurityBufferDescriptor ? inSecurityBufferDescriptor.cBuffers : 1];
                    fixed (void* inUnmanagedBufferPtr = inUnmanagedBuffer)
                    {
                        if (haveInSecurityBufferDescriptor)
                        {
                            // Fix Descriptor pointer that points to unmanaged SecurityBuffers.
                            inSecurityBufferDescriptor.pBuffers = inUnmanagedBufferPtr;
                            pinnedInBytes = new GCHandle[inSecurityBufferDescriptor.cBuffers];
                            SecurityBuffer securityBuffer;
                            for (int index = 0; index < inSecurityBufferDescriptor.cBuffers; ++index)
                            {
                                securityBuffer = inSecBuffer != null ? inSecBuffer : inSecBuffers[index];
                                if (securityBuffer != null)
                                {
                                    // Copy the SecurityBuffer content into unmanaged place holder.
                                    inUnmanagedBuffer[index].cbBuffer = securityBuffer.size;
                                    inUnmanagedBuffer[index].BufferType = securityBuffer.type;

                                    // Use the unmanaged token if it's not null; otherwise use the managed buffer.
                                    if (securityBuffer.unmanagedToken != null)
                                    {
                                        inUnmanagedBuffer[index].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
                                    }
                                    else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
                                    {
                                        inUnmanagedBuffer[index].pvBuffer = IntPtr.Zero;
                                    }
                                    else
                                    {
                                        pinnedInBytes[index] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
                                        inUnmanagedBuffer[index].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(securityBuffer.token, securityBuffer.offset);
                                    }
#if TRACE_VERBOSE
                                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"SecBuffer: cbBuffer:{securityBuffer.size} BufferType:{securityBuffer.type}");
#endif
                                }
                            }
                        }

                        Interop.SspiCli.SecBuffer[] outUnmanagedBuffer = new Interop.SspiCli.SecBuffer[1];
                        fixed (void* outUnmanagedBufferPtr = &outUnmanagedBuffer[0])
                        {
                            // Fix Descriptor pointer that points to unmanaged SecurityBuffers.
                            outSecurityBufferDescriptor.pBuffers = outUnmanagedBufferPtr;
                            outUnmanagedBuffer[0].cbBuffer = outSecBuffer.size;
                            outUnmanagedBuffer[0].BufferType = outSecBuffer.type;
                            if (outSecBuffer.token == null || outSecBuffer.token.Length == 0)
                            {
                                outUnmanagedBuffer[0].pvBuffer = IntPtr.Zero;
                            }
                            else
                            {
                                outUnmanagedBuffer[0].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(outSecBuffer.token, outSecBuffer.offset);
                            }

                            if (isSspiAllocated)
                            {
                                outFreeContextBuffer = SafeFreeContextBuffer.CreateEmptyHandle();
                            }

                            if (refContext == null || refContext.IsInvalid)
                            {
                                refContext = new SafeDeleteContext_SECURITY();
                            }

                            if (targetName == null || targetName.Length == 0)
                            {
                                targetName = dummyStr;
                            }

                            fixed (char* namePtr = targetName)
                            {
                                errorCode = MustRunInitializeSecurityContext_SECURITY(
                                                ref inCredentials,
                                                contextHandle.IsZero ? null : &contextHandle,
                                                (byte*)(((object)targetName == (object)dummyStr) ? null : namePtr),
                                                inFlags,
                                                endianness,
                                                haveInSecurityBufferDescriptor ? &inSecurityBufferDescriptor : null,
                                                refContext,
                                                ref outSecurityBufferDescriptor,
                                                ref outFlags,
                                                outFreeContextBuffer);
                            }

                            if (NetEventSource.IsEnabled) NetEventSource.Info(null, "Marshalling OUT buffer");

                            // Get unmanaged buffer with index 0 as the only one passed into PInvoke.
                            outSecBuffer.size = outUnmanagedBuffer[0].cbBuffer;
                            outSecBuffer.type = outUnmanagedBuffer[0].BufferType;
                            if (outSecBuffer.size > 0)
                            {
                                outSecBuffer.token = new byte[outSecBuffer.size];
                                Marshal.Copy(outUnmanagedBuffer[0].pvBuffer, outSecBuffer.token, 0, outSecBuffer.size);
                            }
                            else
                            {
                                outSecBuffer.token = null;
                            }
                        }
                    }
                }
                finally
                {
                    if (pinnedInBytes != null)
                    {
                        for (int index = 0; index < pinnedInBytes.Length; index++)
                        {
                            if (pinnedInBytes[index].IsAllocated)
                            {
                                pinnedInBytes[index].Free();
                            }
                        }
                    }
                    if (pinnedOutBytes.IsAllocated)
                    {
                        pinnedOutBytes.Free();
                    }

                    if (outFreeContextBuffer != null)
                    {
                        outFreeContextBuffer.Dispose();
                    }
                }

                if (NetEventSource.IsEnabled) NetEventSource.Exit(null, $"errorCode:0x{errorCode:x8}, refContext:{refContext}");
                return errorCode;
            }

            //
            // After PInvoke call the method will fix the handleTemplate.handle with the returned value.
            // The caller is responsible for creating a correct SafeFreeContextBuffer_XXX flavor or null can be passed if no handle is returned.
            //
            private static unsafe int MustRunInitializeSecurityContext_SECURITY(
                ref SafeFreeCredentials inCredentials,
                void* inContextPtr,
                byte* targetName,
                Interop.SspiCli.ContextFlags inFlags,
                Interop.SspiCli.Endianness endianness,
                Interop.SspiCli.SecBufferDesc* inputBuffer,
                SafeDeleteContext outContext,
                ref Interop.SspiCli.SecBufferDesc outputBuffer,
                ref Interop.SspiCli.ContextFlags attributes,
                SafeFreeContextBuffer handleTemplate)
            {
                int errorCode = (int)Interop.SECURITY_STATUS.InvalidHandle;

                try
                {
                    bool ignore = false;
                    inCredentials.DangerousAddRef(ref ignore);
                    outContext.DangerousAddRef(ref ignore);

                    Interop.SspiCli.CredHandle credentialHandle = inCredentials._handle;

                    long timeStamp;

                    errorCode = Interop.SspiCli.InitializeSecurityContextW(
                                    ref credentialHandle,
                                    inContextPtr,
                                    targetName,
                                    inFlags,
                                    0,
                                    endianness,
                                    inputBuffer,
                                    0,
                                    ref outContext._handle,
                                    ref outputBuffer,
                                    ref attributes,
                                    out timeStamp);
                }
                finally
                {
                    //
                    // When a credential handle is first associated with the context we keep credential
                    // ref count bumped up to ensure ordered finalization.
                    // If the credential handle has been changed we de-ref the old one and associate the
                    //  context with the new cred handle but only if the call was successful.
                    if (outContext._EffectiveCredential != inCredentials && (errorCode & 0x80000000) == 0)
                    {
                        // Disassociate the previous credential handle
                        if (outContext._EffectiveCredential != null)
                        {
                            outContext._EffectiveCredential.DangerousRelease();
                        }

                        outContext._EffectiveCredential = inCredentials;
                    }
                    else
                    {
                        inCredentials.DangerousRelease();
                    }

                    outContext.DangerousRelease();
                }

                // The idea is that SSPI has allocated a block and filled up outUnmanagedBuffer+8 slot with the pointer.
                if (handleTemplate != null)
                {
                    //ATTN: on 64 BIT that is still +8 cause of 2* c++ unsigned long == 8 bytes
                    handleTemplate.Set(((Interop.SspiCli.SecBuffer*)outputBuffer.pBuffers)->pvBuffer);
                    if (handleTemplate.IsInvalid)
                    {
                        handleTemplate.SetHandleAsInvalid();
                    }
                }

                if (inContextPtr == null && (errorCode & 0x80000000) != 0)
                {
                    // an error on the first call, need to set the out handle to invalid value
                    outContext._handle.SetToInvalid();
                }

                return errorCode;
            }

            //-------------------------------------------------------------------
            internal static unsafe int AcceptSecurityContext(
                ref SafeFreeCredentials inCredentials,
                ref SafeDeleteContext refContext,
                Interop.SspiCli.ContextFlags inFlags,
                Interop.SspiCli.Endianness endianness,
                SecurityBuffer inSecBuffer,
                SecurityBuffer[] inSecBuffers,
                SecurityBuffer outSecBuffer,
                ref Interop.SspiCli.ContextFlags outFlags)
            {
#if TRACE_VERBOSE
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(null, $"credential={inCredentials}, refContext={refContext}, inFlags={inFlags}");
                if (inSecBuffers == null)
                {
                    NetEventSource.Info(null, "inSecBuffers = (null)");
                }
                else
                {
                    NetEventSource.Info(null, $"inSecBuffers[] = (inSecBuffers)");
                }
            }
#endif

                if (outSecBuffer == null)
                {
                    NetEventSource.Fail(null, "outSecBuffer != null");
                }
                if (inSecBuffer != null && inSecBuffers != null)
                {
                    NetEventSource.Fail(null, "inSecBuffer == null || inSecBuffers == null");
                }

                if (inCredentials == null)
                {
                    throw new ArgumentNullException(nameof(inCredentials));
                }

                Interop.SspiCli.SecBufferDesc inSecurityBufferDescriptor = default(Interop.SspiCli.SecBufferDesc);
                bool haveInSecurityBufferDescriptor = false;
                if (inSecBuffer != null)
                {
                    inSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(1);
                    haveInSecurityBufferDescriptor = true;
                }
                else if (inSecBuffers != null)
                {
                    inSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);
                    haveInSecurityBufferDescriptor = true;
                }

                Interop.SspiCli.SecBufferDesc outSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(1);

                // Actually, this is returned in outFlags.
                bool isSspiAllocated = (inFlags & Interop.SspiCli.ContextFlags.AllocateMemory) != 0 ? true : false;

                int errorCode = -1;

                Interop.SspiCli.CredHandle contextHandle = new Interop.SspiCli.CredHandle();
                if (refContext != null)
                {
                    contextHandle = refContext._handle;
                }

                // These are pinned user byte arrays passed along with SecurityBuffers.
                GCHandle[] pinnedInBytes = null;
                GCHandle pinnedOutBytes = new GCHandle();

                // Optional output buffer that may need to be freed.
                SafeFreeContextBuffer outFreeContextBuffer = null;
                try
                {
                    pinnedOutBytes = GCHandle.Alloc(outSecBuffer.token, GCHandleType.Pinned);
                    var inUnmanagedBuffer = new Interop.SspiCli.SecBuffer[haveInSecurityBufferDescriptor ? inSecurityBufferDescriptor.cBuffers : 1];
                    fixed (void* inUnmanagedBufferPtr = inUnmanagedBuffer)
                    {
                        if (haveInSecurityBufferDescriptor)
                        {
                            // Fix Descriptor pointer that points to unmanaged SecurityBuffers.
                            inSecurityBufferDescriptor.pBuffers = inUnmanagedBufferPtr;
                            pinnedInBytes = new GCHandle[inSecurityBufferDescriptor.cBuffers];
                            SecurityBuffer securityBuffer;
                            for (int index = 0; index < inSecurityBufferDescriptor.cBuffers; ++index)
                            {
                                securityBuffer = inSecBuffer != null ? inSecBuffer : inSecBuffers[index];
                                if (securityBuffer != null)
                                {
                                    // Copy the SecurityBuffer content into unmanaged place holder.
                                    inUnmanagedBuffer[index].cbBuffer = securityBuffer.size;
                                    inUnmanagedBuffer[index].BufferType = securityBuffer.type;

                                    // Use the unmanaged token if it's not null; otherwise use the managed buffer.
                                    if (securityBuffer.unmanagedToken != null)
                                    {
                                        inUnmanagedBuffer[index].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
                                    }
                                    else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
                                    {
                                        inUnmanagedBuffer[index].pvBuffer = IntPtr.Zero;
                                    }
                                    else
                                    {
                                        pinnedInBytes[index] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
                                        inUnmanagedBuffer[index].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(securityBuffer.token, securityBuffer.offset);
                                    }
#if TRACE_VERBOSE
                                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"SecBuffer: cbBuffer:{securityBuffer.size} BufferType:{securityBuffer.type}");
#endif
                                }
                            }
                        }

                        var outUnmanagedBuffer = new Interop.SspiCli.SecBuffer[1];
                        fixed (void* outUnmanagedBufferPtr = &outUnmanagedBuffer[0])
                        {
                            // Fix Descriptor pointer that points to unmanaged SecurityBuffers.
                            outSecurityBufferDescriptor.pBuffers = outUnmanagedBufferPtr;
                            // Copy the SecurityBuffer content into unmanaged place holder.
                            outUnmanagedBuffer[0].cbBuffer = outSecBuffer.size;
                            outUnmanagedBuffer[0].BufferType = outSecBuffer.type;

                            if (outSecBuffer.token == null || outSecBuffer.token.Length == 0)
                            {
                                outUnmanagedBuffer[0].pvBuffer = IntPtr.Zero;
                            }
                            else
                            {
                                outUnmanagedBuffer[0].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(outSecBuffer.token, outSecBuffer.offset);
                            }

                            if (isSspiAllocated)
                            {
                                outFreeContextBuffer = SafeFreeContextBuffer.CreateEmptyHandle();
                            }

                            if (refContext == null || refContext.IsInvalid)
                            {
                                refContext = new SafeDeleteContext_SECURITY();
                            }

                            errorCode = MustRunAcceptSecurityContext_SECURITY(
                                            ref inCredentials,
                                            contextHandle.IsZero ? null : &contextHandle,
                                            haveInSecurityBufferDescriptor ? &inSecurityBufferDescriptor : null,
                                            inFlags,
                                            endianness,
                                            refContext,
                                            ref outSecurityBufferDescriptor,
                                            ref outFlags,
                                            outFreeContextBuffer);

                            if (NetEventSource.IsEnabled) NetEventSource.Info(null, "Marshaling OUT buffer");

                            // Get unmanaged buffer with index 0 as the only one passed into PInvoke.
                            outSecBuffer.size = outUnmanagedBuffer[0].cbBuffer;
                            outSecBuffer.type = outUnmanagedBuffer[0].BufferType;
                            if (outSecBuffer.size > 0)
                            {
                                outSecBuffer.token = new byte[outSecBuffer.size];
                                Marshal.Copy(outUnmanagedBuffer[0].pvBuffer, outSecBuffer.token, 0, outSecBuffer.size);
                            }
                            else
                            {
                                outSecBuffer.token = null;
                            }
                        }
                    }
                }
                finally
                {
                    if (pinnedInBytes != null)
                    {
                        for (int index = 0; index < pinnedInBytes.Length; index++)
                        {
                            if (pinnedInBytes[index].IsAllocated)
                            {
                                pinnedInBytes[index].Free();
                            }
                        }
                    }

                    if (pinnedOutBytes.IsAllocated)
                    {
                        pinnedOutBytes.Free();
                    }

                    if (outFreeContextBuffer != null)
                    {
                        outFreeContextBuffer.Dispose();
                    }
                }

                if (NetEventSource.IsEnabled) NetEventSource.Exit(null, $"errorCode:0x{errorCode:x8}, refContext:{refContext}");
                return errorCode;
            }

            //
            // After PInvoke call the method will fix the handleTemplate.handle with the returned value.
            // The caller is responsible for creating a correct SafeFreeContextBuffer_XXX flavor or null can be passed if no handle is returned.
            //
            private static unsafe int MustRunAcceptSecurityContext_SECURITY(
                ref SafeFreeCredentials inCredentials,
                void* inContextPtr,
                Interop.SspiCli.SecBufferDesc* inputBuffer,
                Interop.SspiCli.ContextFlags inFlags,
                Interop.SspiCli.Endianness endianness,
                SafeDeleteContext outContext,
                ref Interop.SspiCli.SecBufferDesc outputBuffer,
                ref Interop.SspiCli.ContextFlags outFlags,
                SafeFreeContextBuffer handleTemplate)
            {
                int errorCode = (int)Interop.SECURITY_STATUS.InvalidHandle;

                // Run the body of this method as a non-interruptible block.
                try
                {
                    bool ignore = false;

                    inCredentials.DangerousAddRef(ref ignore);
                    outContext.DangerousAddRef(ref ignore);

                    Interop.SspiCli.CredHandle credentialHandle = inCredentials._handle;
                    long timeStamp;

                    errorCode = Interop.SspiCli.AcceptSecurityContext(
                                    ref credentialHandle,
                                    inContextPtr,
                                    inputBuffer,
                                    inFlags,
                                    endianness,
                                    ref outContext._handle,
                                    ref outputBuffer,
                                    ref outFlags,
                                    out timeStamp);
                }
                finally
                {
                    //
                    // When a credential handle is first associated with the context we keep credential
                    // ref count bumped up to ensure ordered finalization.
                    // If the credential handle has been changed we de-ref the old one and associate the
                    //  context with the new cred handle but only if the call was successful.
                    if (outContext._EffectiveCredential != inCredentials && (errorCode & 0x80000000) == 0)
                    {
                        // Disassociate the previous credential handle.
                        if (outContext._EffectiveCredential != null)
                        {
                            outContext._EffectiveCredential.DangerousRelease();
                        }

                        outContext._EffectiveCredential = inCredentials;
                    }
                    else
                    {
                        inCredentials.DangerousRelease();
                    }

                    outContext.DangerousRelease();
                }

                // The idea is that SSPI has allocated a block and filled up outUnmanagedBuffer+8 slot with the pointer.
                if (handleTemplate != null)
                {
                    //ATTN: on 64 BIT that is still +8 cause of 2* c++ unsigned long == 8 bytes.
                    handleTemplate.Set(((Interop.SspiCli.SecBuffer*)outputBuffer.pBuffers)->pvBuffer);
                    if (handleTemplate.IsInvalid)
                    {
                        handleTemplate.SetHandleAsInvalid();
                    }
                }

                if (inContextPtr == null && (errorCode & 0x80000000) != 0)
                {
                    // An error on the first call, need to set the out handle to invalid value.
                    outContext._handle.SetToInvalid();
                }

                return errorCode;
            }

            internal static unsafe int CompleteAuthToken(
                ref SafeDeleteContext refContext,
                SecurityBuffer[] inSecBuffers)
            {
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Enter(null, "SafeDeleteContext::CompleteAuthToken");
                    NetEventSource.Info(null, $"    refContext       = {refContext}");
                    NetEventSource.Info(null, $"    inSecBuffers[]   = {inSecBuffers}");
                }
                if (inSecBuffers == null)
                {
                    NetEventSource.Fail(null, "inSecBuffers == null");
                }

                var inSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);

                int errorCode = (int)Interop.SECURITY_STATUS.InvalidHandle;

                // These are pinned user byte arrays passed along with SecurityBuffers.
                GCHandle[] pinnedInBytes = null;

                var inUnmanagedBuffer = new Interop.SspiCli.SecBuffer[inSecurityBufferDescriptor.cBuffers];
                fixed (void* inUnmanagedBufferPtr = inUnmanagedBuffer)
                {
                    // Fix Descriptor pointer that points to unmanaged SecurityBuffers.
                    inSecurityBufferDescriptor.pBuffers = inUnmanagedBufferPtr;
                    pinnedInBytes = new GCHandle[inSecurityBufferDescriptor.cBuffers];
                    SecurityBuffer securityBuffer;
                    for (int index = 0; index < inSecurityBufferDescriptor.cBuffers; ++index)
                    {
                        securityBuffer = inSecBuffers[index];
                        if (securityBuffer != null)
                        {
                            inUnmanagedBuffer[index].cbBuffer = securityBuffer.size;
                            inUnmanagedBuffer[index].BufferType = securityBuffer.type;

                            // Use the unmanaged token if it's not null; otherwise use the managed buffer.
                            if (securityBuffer.unmanagedToken != null)
                            {
                                inUnmanagedBuffer[index].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
                            }
                            else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
                            {
                                inUnmanagedBuffer[index].pvBuffer = IntPtr.Zero;
                            }
                            else
                            {
                                pinnedInBytes[index] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
                                inUnmanagedBuffer[index].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(securityBuffer.token, securityBuffer.offset);
                            }
#if TRACE_VERBOSE
                        if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"SecBuffer: cbBuffer:{securityBuffer.size} BufferType: {securityBuffer.type}");
#endif
                        }
                    }

                    Interop.SspiCli.CredHandle contextHandle = new Interop.SspiCli.CredHandle();
                    if (refContext != null)
                    {
                        contextHandle = refContext._handle;
                    }
                    try
                    {
                        if (refContext == null || refContext.IsInvalid)
                        {
                            refContext = new SafeDeleteContext_SECURITY();
                        }

                        try
                        {
                            bool ignore = false;
                            refContext.DangerousAddRef(ref ignore);
                            errorCode = Interop.SspiCli.CompleteAuthToken(contextHandle.IsZero ? null : &contextHandle, ref inSecurityBufferDescriptor);
                        }
                        finally
                        {
                            refContext.DangerousRelease();
                        }
                    }
                    finally
                    {
                        if (pinnedInBytes != null)
                        {
                            for (int index = 0; index < pinnedInBytes.Length; index++)
                            {
                                if (pinnedInBytes[index].IsAllocated)
                                {
                                    pinnedInBytes[index].Free();
                                }
                            }
                        }
                    }
                }

                if (NetEventSource.IsEnabled) NetEventSource.Exit(null, $"unmanaged CompleteAuthToken() errorCode:0x{errorCode:x8} refContext:{refContext}");
                return errorCode;
            }

            internal static unsafe int ApplyControlToken(
                ref SafeDeleteContext refContext,
                SecurityBuffer[] inSecBuffers)
            {
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Enter(null);
                    NetEventSource.Info(null, $"    refContext       = {refContext}");
                    NetEventSource.Info(null, $"    inSecBuffers[]   = length:{inSecBuffers.Length}");
                }

                if (inSecBuffers == null)
                {
                    NetEventSource.Fail(null, "inSecBuffers == null");
                }

                var inSecurityBufferDescriptor = new Interop.SspiCli.SecBufferDesc(inSecBuffers.Length);

                int errorCode = (int)Interop.SECURITY_STATUS.InvalidHandle;

                // These are pinned user byte arrays passed along with SecurityBuffers.
                GCHandle[] pinnedInBytes = null;

                var inUnmanagedBuffer = new Interop.SspiCli.SecBuffer[inSecurityBufferDescriptor.cBuffers];
                fixed (void* inUnmanagedBufferPtr = inUnmanagedBuffer)
                {
                    // Fix Descriptor pointer that points to unmanaged SecurityBuffers.
                    inSecurityBufferDescriptor.pBuffers = inUnmanagedBufferPtr;
                    pinnedInBytes = new GCHandle[inSecurityBufferDescriptor.cBuffers];
                    SecurityBuffer securityBuffer;
                    for (int index = 0; index < inSecurityBufferDescriptor.cBuffers; ++index)
                    {
                        securityBuffer = inSecBuffers[index];
                        if (securityBuffer != null)
                        {
                            inUnmanagedBuffer[index].cbBuffer = securityBuffer.size;
                            inUnmanagedBuffer[index].BufferType = securityBuffer.type;

                            // Use the unmanaged token if it's not null; otherwise use the managed buffer.
                            if (securityBuffer.unmanagedToken != null)
                            {
                                inUnmanagedBuffer[index].pvBuffer = securityBuffer.unmanagedToken.DangerousGetHandle();
                            }
                            else if (securityBuffer.token == null || securityBuffer.token.Length == 0)
                            {
                                inUnmanagedBuffer[index].pvBuffer = IntPtr.Zero;
                            }
                            else
                            {
                                pinnedInBytes[index] = GCHandle.Alloc(securityBuffer.token, GCHandleType.Pinned);
                                inUnmanagedBuffer[index].pvBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(securityBuffer.token, securityBuffer.offset);
                            }
#if TRACE_VERBOSE
                        if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"SecBuffer: cbBuffer:{securityBuffer.size} BufferType:{securityBuffer.type}");
#endif
                        }
                    }

                    // TODO: (#3114): Optimizations to remove the unnecesary allocation of a CredHandle, remove the AddRef
                    // if refContext was previously null, refactor the code to unify CompleteAuthToken and ApplyControlToken.
                    Interop.SspiCli.CredHandle contextHandle = new Interop.SspiCli.CredHandle();
                    if (refContext != null)
                    {
                        contextHandle = refContext._handle;
                    }

                    try
                    {
                        if (refContext == null || refContext.IsInvalid)
                        {
                            refContext = new SafeDeleteContext_SECURITY();
                        }

                        try
                        {
                            bool ignore = false;
                            refContext.DangerousAddRef(ref ignore);
                            errorCode = Interop.SspiCli.ApplyControlToken(contextHandle.IsZero ? null : &contextHandle, ref inSecurityBufferDescriptor);
                        }
                        finally
                        {
                            refContext.DangerousRelease();
                        }
                    }
                    finally
                    {
                        if (pinnedInBytes != null)
                        {
                            for (int index = 0; index < pinnedInBytes.Length; index++)
                            {
                                if (pinnedInBytes[index].IsAllocated)
                                {
                                    pinnedInBytes[index].Free();
                                }
                            }
                        }
                    }
                }

                if (NetEventSource.IsEnabled) NetEventSource.Exit(null, $"unmanaged ApplyControlToken() errorCode:0x{errorCode:x8} refContext: {refContext}");
                return errorCode;
            }
        }

    public abstract partial class SafeDeleteContext : SafeHandle
    {
        //
        // ATN: _handle is internal since it is used on PInvokes by other wrapper methods.
        //      However all such wrappers MUST manually and reliably adjust refCounter of SafeDeleteContext handle.
        //
        internal Interop.SspiCli.CredHandle _handle;

        protected SafeDeleteContext() : base(IntPtr.Zero, true)
        {
            _handle = new Interop.SspiCli.CredHandle();
        }

        public override bool IsInvalid
        {
            get
            {
                return IsClosed || _handle.IsZero;
            }
        }

        public override string ToString()
        {
            return _handle.ToString();
        }

#if DEBUG
        //This method should never be called for this type
        public new IntPtr DangerousGetHandle()
        {
            throw new InvalidOperationException();
        }
#endif
    }
}