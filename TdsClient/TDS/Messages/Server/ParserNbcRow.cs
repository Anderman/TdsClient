using System;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserNbcRow
    {
        public static void SaveNbcBitmap(this TdsPackageReader reader)
        {
            var bitmap = reader.CurrentRow.NbcBitmap.AsSpan();
            reader.GetBytes(bitmap.Length).CopyTo(bitmap);
            reader.CurrentRow.IsNbcRow = true;
        }

        public static void InitNbcBitmap(this TdsPackageReader reader, int columnsCount)
        {
            // 1-8 columns need 1 byte
            // 9-16: 2 bytes, and so on
            var bitmapArrayLength = (columnsCount + 7) / 8;
            var row = reader.CurrentRow;

            // allow reuse of previously allocated bitmap
            if (row.NbcBitmap?.Length != bitmapArrayLength)
                row.NbcBitmap = new byte[bitmapArrayLength];
        }
    }
}