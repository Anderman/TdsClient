using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Medella.TdsClient.TDS.Row
{
	public static class TypeExtensions
	{
		public static Dictionary<string, PropertyInfo> GetPublicProperties(this Type type)
		{
			return type.GetProperties().Where(p => p.GetSetMethod(false) != null).ToDictionary(x => x.Name, x => x);
		}
	}
}