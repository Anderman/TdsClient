using System;
using System.Collections.Generic;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        public void WriteNullableSqlDouble(double? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 8);
            if (value != null)
                WriteSqlDoubleUnChecked((double)value);
            CheckBuffer();
        }

        public void WriteNullableSqlFloat(float? value)
        {
            WriteBuffer[WritePosition++] = (byte)(value == null ? 0 : 4);
            if (value != null)
                WriteSqlFloatUnChecked((float)value);
            CheckBuffer();
        }
    }
}
