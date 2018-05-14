using System;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package
{
    public partial class TdsPackageWriter
    {
        public void WriteFloat(float v)
        {
            WriteSqlFloatUnChecked(v);
            CheckBuffer();
        }

        public void WriteDouble(double v)
        {
            WriteSqlDoubleUnChecked(v);
            CheckBuffer();
        }

        private void WriteSqlFloatUnChecked(float v)
        {
            var b = BitConverter.GetBytes(v);
            WriteBuffer[WritePosition] = b[0];
            WriteBuffer[WritePosition + 1] = b[1];
            WriteBuffer[WritePosition + 2] = b[2];
            WriteBuffer[WritePosition + 3] = b[3];
            WritePosition += sizeof(float);
        }

        private void WriteSqlDoubleUnChecked(double v)
        {
            var b = BitConverter.GetBytes(v);
            WriteBuffer[WritePosition] = b[0];
            WriteBuffer[WritePosition + 1] = b[1];
            WriteBuffer[WritePosition + 2] = b[2];
            WriteBuffer[WritePosition + 3] = b[3];
            WriteBuffer[WritePosition + 4] = b[4];
            WriteBuffer[WritePosition + 5] = b[5];
            WriteBuffer[WritePosition + 6] = b[6];
            WriteBuffer[WritePosition + 7] = b[7];
            WritePosition += sizeof(double);
        }
    }
}
