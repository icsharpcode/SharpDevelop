using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ICSharpCode.Xaml
{
	public static class ReflectionMapper
	{
		static Dictionary<object, XamlAssembly> assemblies = new Dictionary<object, XamlAssembly>();
		static Dictionary<object, XamlType> types = new Dictionary<object, XamlType>();
		static Dictionary<object, XamlMember> members = new Dictionary<object, XamlMember>();

		public static XamlAssembly GetXamlAssembly(Assembly key)
		{
			XamlAssembly result;
			if (!assemblies.TryGetValue(key, out result)) {
				result = new ReflectionAssembly(key);
				assemblies[key] = result;
			}
			return result;
		}

		public static XamlType GetXamlType(Type key)
		{
			XamlType result;
			if (!types.TryGetValue(key, out result)) {
				result = new ReflectionType(key);
				types[key] = result;
			}
			return result;
		}

		public static XamlMember GetXamlMember(ReflectionMemberInfo key)
		{
			XamlMember result;
			if (!members.TryGetValue(key, out result)) {
				result = new ReflectionMember(key);
				members[key] = result;
			}
			return result;
		}
	}
}
