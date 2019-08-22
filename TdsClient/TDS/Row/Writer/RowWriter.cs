using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;

namespace Medella.TdsClient.TDS.Row.Writer
{
    public class RowWriter
    {
        public static Dictionary<int, MethodInfo> SqlTypes = new Dictionary<int, MethodInfo>
        {
            { TdsEnums.SQLDECIMALN, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDecimal)) },
            { TdsEnums.SQLNUMERICN, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDecimal)) },
            { TdsEnums.SQLUDT, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBinary)) },
            { TdsEnums.SQLBINARY, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBinary)) },
            { TdsEnums.SQLBIGBINARY, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBinary)) },
            { TdsEnums.SQLBIGVARBINARY, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBinary)) },
            { TdsEnums.SQLVARBINARY, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBinary)) },
            { TdsEnums.SQLIMAGE, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBinary)) },
            { TdsEnums.SQLCHAR, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },
            { TdsEnums.SQLBIGCHAR, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },
            { TdsEnums.SQLVARCHAR, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },
            { TdsEnums.SQLBIGVARCHAR, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },
            { TdsEnums.SQLTEXT, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },

            { TdsEnums.SQLNCHAR, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },
            { TdsEnums.SQLNVARCHAR, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },
            { TdsEnums.SQLNTEXT, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },

            { TdsEnums.SQLXMLTYPE, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlString)) },

            { TdsEnums.SQLDATE, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDate)) },
            { TdsEnums.SQLTIME, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlTime)) },
            { TdsEnums.SQLDATETIME2, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDateTime2)) },
            { TdsEnums.SQLDATETIMEOFFSET, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDateTimeOffset)) },

            { TdsEnums.SQLBIT, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlBit)) },
            { TdsEnums.SQLBITN, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlBit)) },

            { TdsEnums.SQLINTN, null },
            { TdsEnums.SQLINT1, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlByte)) },
            { TdsEnums.SQLINT2, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlInt16)) },
            { TdsEnums.SQLINT4, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlInt32)) },
            { TdsEnums.SQLINT8, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlInt64)) },

            { TdsEnums.SQLFLTN, null },
            { TdsEnums.SQLFLT4, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlFloat)) },
            { TdsEnums.SQLFLT8, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlDouble)) },

            { TdsEnums.SQLMONEYN, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlMoneyN)) },
            { TdsEnums.SQLMONEY, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlMoney)) },
            { TdsEnums.SQLMONEY4, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlMoney4)) },

            { TdsEnums.SQLDATETIMN, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDateTime)) },
            { TdsEnums.SQLDATETIM4, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlDateTime4)) },
            { TdsEnums.SQLDATETIME, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteSqlDateTime)) },

            { TdsEnums.SQLUNIQUEID, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlUniqueId)) },
            { TdsEnums.SQLVARIANT, typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlVariant)) }
        };

        public static Dictionary<Type, MethodInfo> ClrTypes = new Dictionary<Type, MethodInfo>
        {
            { typeof(byte?), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlByte)) },
            { typeof(short?), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlInt16)) },
            { typeof(int?), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlInt32)) },
            { typeof(long?), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlInt64)) },
            { typeof(float?), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlFloat)) },
            { typeof(double?), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDouble)) },
                        
            // Allow not nullable type to write nullable sqltype
            { typeof(byte), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlByte)) },
            { typeof(short), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlInt16)) },
            { typeof(int), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlInt32)) },
            { typeof(long), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlInt64)) },
            { typeof(float), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlFloat)) },
            { typeof(double), typeof(TdsColumnWriter).GetMethod(nameof(TdsColumnWriter.WriteNullableSqlDouble)) }
        };

        public static Action<TdsColumnWriter, T> GetComplexWriter<T>(TdsColumnWriter writer)
        {
            var newMapping = GetPreMappedColumns(writer);
            if (newMapping.Length == 0)
                newMapping = GetDefaultMapping<T>(writer);
            return _GetWriter<T>(newMapping);
        }

        private static Mapping[] GetDefaultMapping<T>(TdsColumnWriter writer)
        {
            var sqlTableColumns = GetDefaultMapping(writer.MetaData);
            var typeColumns = typeof(T).GetPublicProperties();
            var columnsNotMapped = sqlTableColumns.Select(x => x.SqlName).Except(typeColumns.Keys).ToArray();
            if (columnsNotMapped.Any())
                throw new ArgumentException($"Not all columns are mapped to class properties. The follow columns could not mapped: {string.Join(",", columnsNotMapped)}");

            var newMapping = sqlTableColumns.Select(x => new Mapping { SqlIndex = x.SqlIndex, PropertyInfo = typeColumns[x.SqlName], SqlName = x.SqlName, TdsType = x.TdsType }).ToArray();
            return newMapping;
        }

        private static Mapping[] GetPreMappedColumns(TdsColumnWriter writer)
        {
            return writer.MetaData.Where(x => x.PropertyInfo != null).Select((x, i) => new Mapping() { PropertyInfo = x.PropertyInfo, SqlName = x.Column, TdsType = x.TdsType, SqlIndex = i }).ToArray();
        }

        private static Action<TdsColumnWriter, T> _GetWriter<T>(Mapping[] mapping)
        {
            //create a method with lambda expressions
            /*
			 BulkWriter(TdsColumnWriter writer, T obj)
			 {
			    writer.Write[nullable][SqlType](t.propertyX, 0)
			    writer.Write[nullable][SqlType](t.propertyY, 1)
			}
			 */

            var writerParam = Expression.Parameter(typeof(TdsColumnWriter), "writer");
            var obj = Expression.Parameter(typeof(T), "obj");

            var statements = new List<Expression>();
            foreach (var map in mapping)
            {
                var property = map.PropertyInfo;
                var getProperty = Expression.Property(obj, property);
                var writeValue = GetValue(writerParam, getProperty, map.SqlIndex, map.TdsType);
                try
                {
                    statements.Add(writeValue);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not create a mapping between property ({map.PropertyInfo.PropertyType}){map.SqlName}=>({map.TdsType}){property.PropertyType.Name} and column type:{writeValue.Type}", ex);
                }
            }

            var body = Expression.Block(statements.ToArray()); //declare a block variable. A variable as first statement doesn't work
            var function = Expression.Lambda<Action<TdsColumnWriter, T>>(body, writerParam, obj);

            return function.Compile();
        }

        private static MethodCallExpression GetValue(Expression writer, Expression property, int columnIndex, int tdsType)
        {
            if (!SqlTypes.TryGetValue(tdsType, out var method)) throw new Exception($"TdsType not supported:{tdsType} index:{columnIndex}");
            if (method == null && !ClrTypes.TryGetValue(property.Type, out method)) throw new Exception($"TdsType not supported:{tdsType} index:{columnIndex}");
            var type = method.GetParameters()[0].ParameterType;
            Expression convertedProperty = Expression.Convert(property, type);
            return Expression.Call(writer, method, new[] { convertedProperty, Expression.Constant(columnIndex) });
        }

        private static WriterColumn[] GetDefaultMapping(MetadataBulkCopy[] metadata)
        {
            return Enumerable.Range(0, metadata.Length).Select(x => new WriterColumn { SqlName = metadata[x].Column, SqlIndex = x, TdsType = metadata[x].TdsType }).ToArray();
        }

        [DebuggerDisplay("SqlIndex:{SqlIndex} ClrType:{PropertyInfo.PropertyType} PropertyName:{PropertyInfo.Name} TdsType:{TdsType}")]
        public class Mapping
        {
            public int SqlIndex { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public byte TdsType { get; set; }
            public string SqlName { get; set; }
        }

        [DebuggerDisplay("SqlIndex:{SqlIndex} SqlName:{SqlName} TdsType:{TdsType}")]
        internal class WriterColumn
        {
            public string SqlName { get; set; }
            public int SqlIndex { get; set; }
            public byte TdsType { get; set; }
        }
    }
}