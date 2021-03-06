// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Pipes;
using Medella.TdsClient.Constants;

namespace Medella.TdsClient.TdsStream
{
    /// <summary>
    ///     SSL encapsulated over TDS transport. During SSL handshake, SSL packets are
    ///     transported in TDS packet type 0x12. Once SSL handshake has completed, SSL
    ///     packets are sent transparently.
    /// </summary>
    internal sealed class SslOverTdsStream : Stream
    {
        private const int PACKET_SIZE_WITHOUT_HEADER = TdsEnums.DEFAULT_LOGIN_PACKET_SIZE - TdsEnums.HEADER_LEN;
        private const int PRELOGIN_PACKET_TYPE = 0x12;
        private readonly Stream _stream;
        private bool _encapsulate;

        private int _packetBytes;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="stream">Underlying stream</param>
        public SslOverTdsStream(Stream stream)
        {
            _stream = stream;
            _encapsulate = true;
        }

        /// <summary>
        ///     Get/set stream position
        /// </summary>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///     Check if stream can be read from
        /// </summary>
        public override bool CanRead => _stream.CanRead;

        /// <summary>
        ///     Check if stream can be written to
        /// </summary>
        public override bool CanWrite => _stream.CanWrite;

        /// <summary>
        ///     Check if stream can be seeked
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        ///     Get stream length
        /// </summary>
        public override long Length => throw new NotSupportedException();

        /// <summary>
        ///     Finish SSL handshake. Stop encapsulating in TDS.
        /// </summary>
        public void FinishHandshake()
        {
            _encapsulate = false;
        }

        /// <summary>
        ///     Read buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Byte count</param>
        /// <returns>Bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var readBytes = 0;
            var packetData = new byte[count < TdsEnums.HEADER_LEN ? TdsEnums.HEADER_LEN : count];

            if (_encapsulate)
            {
                if (_packetBytes == 0)
                {
                    // Account for split packets
                    while (readBytes < TdsEnums.HEADER_LEN) readBytes += _stream.Read(packetData, readBytes, TdsEnums.HEADER_LEN - readBytes);

                    _packetBytes = (packetData[TdsEnums.HEADER_LEN_FIELD_OFFSET] << 8) | packetData[TdsEnums.HEADER_LEN_FIELD_OFFSET + 1];
                    _packetBytes -= TdsEnums.HEADER_LEN;
                }

                if (count > _packetBytes) count = _packetBytes;
            }

            readBytes = _stream.Read(packetData, 0, count);

            if (_encapsulate) _packetBytes -= readBytes;

            Buffer.BlockCopy(packetData, 0, buffer, offset, readBytes);
            return readBytes;
        }

        /// <summary>
        ///     Write buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Byte count</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            var currentCount = 0;
            var currentOffset = offset;

            while (count > 0)
            {
                // During the SSL negotiation phase, SSL is tunnelled over TDS packet type 0x12. After
                // negotiation, the underlying socket only sees SSL frames.
                //
                if (_encapsulate)
                {
                    if (count > PACKET_SIZE_WITHOUT_HEADER)
                        currentCount = PACKET_SIZE_WITHOUT_HEADER;
                    else
                        currentCount = count;

                    count -= currentCount;

                    // Prepend buffer data with TDS prelogin header
                    var combinedBuffer = new byte[TdsEnums.HEADER_LEN + currentCount];

                    // We can only send 4088 bytes in one packet. Header[1] is set to 1 if this is a 
                    // partial packet (whether or not count != 0).
                    // 
                    combinedBuffer[0] = PRELOGIN_PACKET_TYPE;
                    combinedBuffer[1] = (byte)(count > 0 ? 0 : 1);
                    combinedBuffer[2] = (byte)((currentCount + TdsEnums.HEADER_LEN) / 0x100);
                    combinedBuffer[3] = (byte)((currentCount + TdsEnums.HEADER_LEN) % 0x100);
                    combinedBuffer[4] = 0;
                    combinedBuffer[5] = 0;
                    combinedBuffer[6] = 0;
                    combinedBuffer[7] = 0;

                    for (var i = TdsEnums.HEADER_LEN; i < combinedBuffer.Length; i++) combinedBuffer[i] = buffer[currentOffset + (i - TdsEnums.HEADER_LEN)];

                    _stream.Write(combinedBuffer, 0, combinedBuffer.Length);
                }
                else
                {
                    currentCount = count;
                    count = 0;

                    _stream.Write(buffer, currentOffset, currentCount);
                }

                _stream.Flush();
                currentOffset += currentCount;
            }
        }

        /// <summary>
        ///     Set stream length.
        /// </summary>
        /// <param name="value">Length</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Flush stream
        /// </summary>
        public override void Flush()
        {
            // Can sometimes get Pipe broken errors from flushing a PipeStream.
            // PipeStream.Flush() also doesn't do anything, anyway.
            if (!(_stream is PipeStream)) _stream.Flush();
        }

        /// <summary>
        ///     Seek in stream
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="origin">Origin</param>
        /// <returns>Position</returns>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    }
}