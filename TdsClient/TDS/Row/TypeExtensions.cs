using System;
using System.Collections.Generic;
using System.Linq;

namespace Medella.TdsClient.TDS.Row
{
	public static class TypeExtensions
	{
		public static Dictionary<string, Type> GetPublicProperties(this Type type)
		{
			return type.GetProperties().Where(p => p.GetSetMethod(false) != null).ToDictionary(x => x.Name, x => x.PropertyType);
		}
	}
}