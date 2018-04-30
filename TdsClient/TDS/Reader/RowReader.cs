using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Reader
{
    public class RowReader
    {
        public static Dictionary<int, MethodInfo> SqlTypes = new Dictionary<int, MethodInfo>
        {
            {TdsEnums.SQLDECIMALN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadDecimal))},
            {TdsEnums.SQLNUMERICN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadDecimal))},
            {TdsEnums.SQLUDT, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadBinary))},
            {TdsEnums.SQLBINARY, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadBinary))},
            {TdsEnums.SQLBIGBINARY, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadBinary))},
            {TdsEnums.SQLBIGVARBINARY, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadBinary))},
            {TdsEnums.SQLVARBINARY, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadBinary))},
            {TdsEnums.SQLIMAGE, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadBinary))},
            {TdsEnums.SQLCHAR, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadString))},
            {TdsEnums.SQLBIGCHAR, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadString))},
            {TdsEnums.SQLVARCHAR, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadString))},
            {TdsEnums.SQLBIGVARCHAR, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadString))},
            {TdsEnums.SQLTEXT, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadString))},

            {TdsEnums.SQLNCHAR, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadUnicodeString))},
            {TdsEnums.SQLNVARCHAR, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadUnicodeString))},
            {TdsEnums.SQLNTEXT, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadUnicodeString))},

            {TdsEnums.SQLXMLTYPE, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadUnicodeString))},

            {TdsEnums.SQLDATE, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDate))},
            {TdsEnums.SQLTIME, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlTime))},
            {TdsEnums.SQLDATETIME2, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDateTime2))},
            {TdsEnums.SQLDATETIMEOFFSET, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDateTimeOffset))},

            {TdsEnums.SQLBIT, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlBit))},
            {TdsEnums.SQLBITN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlBit))},

            {TdsEnums.SQLINTN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlIntN))},
            {TdsEnums.SQLINT1, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlByte))},
            {TdsEnums.SQLINT2, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlInt16))},
            {TdsEnums.SQLINT4, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlInt32))},
            {TdsEnums.SQLINT8, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlInt64))},

            {TdsEnums.SQLFLTN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlFloatN))},
            {TdsEnums.SQLFLT4, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlFloat))},
            {TdsEnums.SQLFLT8, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDouble))},
            {TdsEnums.SQLMONEY, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlMoney))},
            {TdsEnums.SQLMONEY4, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlMoney))},
            {TdsEnums.SQLMONEYN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlMoney))},

            {TdsEnums.SQLDATETIMN, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDateTime))},
            {TdsEnums.SQLDATETIM4, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDateTime))},
            {TdsEnums.SQLDATETIME, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlDateTime))},

            {TdsEnums.SQLUNIQUEID, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlGuid))},
            {TdsEnums.SQLVARIANT, typeof(TdsColumnReader).GetMethod(nameof(TdsColumnReader.ReadSqlVariant))},
        };

        public static Func<TdsColumnReader, T> GetComplexReader<T>(TdsPackageReader reader)
        {
            var readerColumns = GetDefaultMapping(reader.CurrentResultset.ColumnsMetadata);
            var typeInfo = typeof(T).GetProperties().Where(p => p.GetSetMethod(false) != null).ToDictionary(x => x.Name, x => x.PropertyType);
            if (readerColumns.Select(x => x.SqlName).Except(typeInfo.Keys).Any())
                throw new ArgumentException($"Not all columns are mapped to class properties. The follow columns could not mapped: {string.Join(",", readerColumns.Select(x => x.SqlName).Except(typeInfo.Keys))}");

            var newMapping = readerColumns.Select(x => new Mapping { SqlIndex = x.SqlIndex, ClrType = typeInfo[x.SqlName], TdsType = x.TdsType, PropertyName = x.SqlName }).ToArray();

            return _GetReader<T>(newMapping);
        }

        private static Func<TdsColumnReader, T> _GetReader<T>(Mapping[] mapping)
        {
            //create a method with lambda expressions
            /*
			 GetBinWriter(IDataReader reader)
			 {
				T obj;
				obj = new T();
			    obj.prop1 = reader.GetTypedValue(i)
			    obj.prop2 = reader.GetTypedValue(i)
				return obj;
			}
			 */
            //var sqlDic = specification.Where(x => map.ContainsKey(x.SqlName)).Select((x, i) => new {x.Type, x.IsNullable, x.SqlName, Name = map[x.SqlName], SqlIndex = i}).ToArray();

            var obj = Expression.Variable(typeof(T), "obj");
            var objIsNewT = Expression.Assign(obj, Expression.New(typeof(T)));
            var readerParam = Expression.Parameter(typeof(TdsColumnReader), "reader");

            var statements = new List<Expression>();
            statements.Add(objIsNewT);
            foreach (var map in mapping)
            {
                var property = typeof(T).GetProperty(map.PropertyName);
                var setProperty = Expression.Property(obj, property);
                var readValue = GetValue(readerParam, map.SqlIndex, map.TdsType);
                try
                {
                    statements.Add(Expression.Assign(setProperty, Expression.Convert(readValue, property.PropertyType)));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not create a mapping between property {map.PropertyName}:{property.PropertyType.Name} and column index:{map.SqlIndex}:{readValue.Type}", ex);
                }
            }
            var returnStatement = obj;
            statements.Add(returnStatement);

            var body = Expression.Block(new[] { obj }, statements.ToArray()); //declare a block variable. A variable as first statement doesn't work
            var function = Expression.Lambda<Func<TdsColumnReader, T>>(body, readerParam);

            return function.Compile();
        }

        private static MethodCallExpression GetValue(Expression reader, int columnIndex, int tdsType)
        {
            return Expression.Call(reader, SqlTypes.ContainsKey(tdsType) ? SqlTypes[tdsType] : throw new Exception($"TdsType not supported:{tdsType} index:{columnIndex}"), Expression.Constant(columnIndex));
        }

        private static ReaderColumn[] GetDefaultMapping(ColumnsMetadata metadata)
        {
            return Enumerable.Range(0, metadata.Length).Select(x => new ReaderColumn { SqlName = metadata[x].Column, SqlIndex = x, TdsType = metadata[x].TdsType}).ToArray();
        }

        [DebuggerDisplay("SqlIndex:{SqlIndex} ClrType:{ClrType} PropertyName:{PropertyName} TdsType:{TdsType}")]
        public class Mapping
        {
            public int SqlIndex { get; set; }
            public Type ClrType { get; set; }
            public string PropertyName { get; set; }
            public byte TdsType { get; set; }
        }

        internal class ReaderColumn
        {
            public string SqlName { get; set; }
            public int SqlIndex { get; set; }
            public byte TdsType { get; set; }
        }
    }
}