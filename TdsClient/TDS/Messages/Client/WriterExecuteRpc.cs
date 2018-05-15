using System;
using System.Text;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;
using TdsPackageWriter = Medella.TdsClient.TDS.Package.Writer.TdsPackageWriter;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public static class WriterExecuteRpc
    {
        public static async Task SendRpcASync(this TdsPackageWriter writer, SqlCollations defaultCollation, FormattableString sql, long sqlConnectionId)
        {
            await Task.Run(() => SendRpc(writer, defaultCollation, sql, sqlConnectionId));
        }

        public static void SendRpc(this TdsPackageWriter writer, SqlCollations defaultCollation, FormattableString sql, long sqlConnectionId)
        {
            writer.NewPackage(TdsEnums.MT_RPC);
            writer.WriteMarsHeader(sqlConnectionId);

            writer.WriteInt16(0xffff);
            writer.WriteInt16(TdsEnums.RPC_PROCID_EXECUTESQL);

            // Options
            writer.WriteInt16(TdsEnums.RPC_PARAM_DEFAULT);

            // Stream out parameters
            var parameters = CreateParameters(sql);
            foreach (var parameter in parameters)
            {
                // parameters can be unnamed
                var param = parameter;
                var value = parameter.Value;
                var isNull = value == null;

                writer.WriteByteLenString(param.Name);

                // Write parameter status
                writer.WriteByte(parameter.Status);
                var p = parameter;
                var mt = p.MetaData;
                WriteTdsTypeInfo(writer, mt, p.Size, p.IsNull, defaultCollation, p.Scale);
                WriteValue(writer, value, mt, isNull);
            }

            writer.SendLastMessage();
        }

        public static void WriteValue(TdsPackageWriter writer, object value, TdsMetaType.MetaDataWrite metaData, bool isNull)
        {
            // write the value now
            if (isNull)
                return;
            switch (value)
            {
                case string v:
                    writer.WriteUnicodeString(v);
                    break;
                case decimal v:
                    writer.WriteSqlDecimal(v, 17);
                    break;
                case bool v:
                    writer.WriteByte(v ? 1 : 0);
                    break;
                case DateTime v:
                    writer.WriteDateTime(v);
                    break;
                case byte v:
                    writer.WriteByte(v);
                    break;
                case short v:
                    writer.WriteInt16(v);
                    break;
                case int v:
                    writer.WriteInt32(v);
                    break;
                case long v:
                    writer.WriteInt64(v);
                    break;
                case float v:
                    writer.WriteFloat(v);
                    break;
                case double v:
                    writer.WriteDouble(v);
                    break;
            }
        }

        public static void WriteTdsTypeInfo(this TdsPackageWriter writer, TdsMetaType.MetaDataWrite metaData, int size, bool isNull)
        {
            WriteTdsTypeInfo(writer, metaData, size, isNull, null, 0);
        }

        public static void WriteTdsTypeInfo(this TdsPackageWriter writer, TdsMetaType.MetaDataWrite metaData, int size, bool isNull, SqlCollations defaultCollation, byte scale)
        {
            var mt = metaData;
            writer.WriteByte(mt.NullableType);

            writer.WriteTypeInfoLen(mt, size, isNull); //typeinfo varlen
            if (mt.HasCollation)
                writer.WriteCollation2(defaultCollation);
            if (mt.HasPrecision)
                writer.WriteByte(28); //Max clr precision
            if (mt.HasScale)
                writer.WriteByte(scale); //
            writer.WriteParameterLen(metaData, size, isNull); //len parameter
        }

        public static TdsParameter[] CreateParameters(FormattableString fstring)
        {
            var count = fstring.ArgumentCount;
            var ps = new string[count];
            for (var p = 0; p < count; p++) ps[p] = "@p" + p;
            var pars = new TdsParameter[fstring.ArgumentCount + 2];
            pars[0] = new TdsParameter("", string.Format(fstring.Format, ps));
            var i = 0;
            var sb = new StringBuilder();
            foreach (var arg in fstring.GetArguments())
            {
                TdsParameter p = null;
                switch (arg)
                {
                    case string s:
                        p = new TdsParameter($"@p{i}", s);
                        break;
                    case decimal d:
                        p = new TdsParameter($"@p{i}", d);
                        break;
                    case bool b:
                        p = new TdsParameter($"@p{i}", b);
                        break;
                    case byte v:
                        p = new TdsParameter($"@p{i}", v);
                        break;
                    case short v:
                        p = new TdsParameter($"@p{i}", v);
                        break;
                    case int v:
                        p = new TdsParameter($"@p{i}", v);
                        break;
                    case long v:
                        p = new TdsParameter($"@p{i}", v);
                        break;
                    case float v:
                        p = new TdsParameter($"@p{i}", v);
                        break;
                    case double v:
                        p = new TdsParameter($"@p{i}", v);
                        break;
                    case DateTime d:
                        p = new TdsParameter($"@p{i}", d);
                        break;
                    default: throw new Exception("Unknow type");
                }

                sb.Append($"{(i > 0 ? "," : "")}@p{i} {p.SqlName}");
                pars[i + 2] = p;
                i++;
            }

            pars[1] = new TdsParameter("", sb.ToString());

            return pars;
        }
    }
}