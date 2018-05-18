using System;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {

        public void WriteUnicodeString(string v)
        {
            if (v.Length == 0) return;
            if (v.Length * 2 <= WriteBuffer.Length - WritePosition)
            {
                WritePosition += Encoding.Unicode.GetBytes(v, 0, v.Length, WriteBuffer, WritePosition);
            }
            else
            {
                var buffer = new byte[v.Length * 2];
                var bytes = Encoding.Unicode.GetBytes(v, 0, v.Length, buffer, 0);
                WriteByteArray(buffer);
            }
        }
    }
}
