using System;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace Medella.TdsClient.System2.Net.Security
{
    public class SecurityBuffer
    {
        public int offset;
        public int size;
        public byte[] token;
        public SecurityBufferType type;
        public SafeHandle unmanagedToken;

        public SecurityBuffer(byte[] data, int offset, int size, SecurityBufferType tokentype)
        {
            if (offset < 0 || offset > (data == null ? 0 : data.Length)) NetEventSource.Fail(this, $"'offset' out of range.  [{offset}]");

            if (size < 0 || size > (data == null ? 0 : data.Length - offset)) NetEventSource.Fail(this, $"'size' out of range.  [{size}]");

            this.offset = data == null || offset < 0 ? 0 : Math.Min(offset, data.Length);
            this.size = data == null || size < 0 ? 0 : Math.Min(size, data.Length - this.offset);
            type = tokentype;
            token = size == 0 ? null : data;
        }

        public SecurityBuffer(byte[] data, SecurityBufferType tokentype)
        {
            size = data?.Length ?? 0;
            type = tokentype;
            token = size == 0 ? null : data;
        }

        public SecurityBuffer(int size, SecurityBufferType tokentype)
        {
            if (size < 0) NetEventSource.Fail(this, $"'size' out of range.  [{size}]");

            this.size = size;
            type = tokentype;
            token = size == 0 ? null : new byte[size];
        }

        public SecurityBuffer(ChannelBinding binding)
        {
            size = binding == null ? 0 : binding.Size;
            type = SecurityBufferType.SECBUFFER_CHANNEL_BINDINGS;
            unmanagedToken = binding;
        }
    }
}