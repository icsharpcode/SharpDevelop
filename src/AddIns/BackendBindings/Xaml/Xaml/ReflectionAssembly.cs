using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	public class ReflectionAssembly : XamlAssembly
	{
		internal ReflectionAssembly(Assembly assembly)
		{
			Assembly = assembly;
		}

		public Assembly Assembly;

		public override string Name
		{
			get { return Assembly.GetName().Name; }
		}

		public override IEnumerable<XmlnsDefinitionAttribute> XmlnsDefinitions
		{
			get { return Assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), false) as XmlnsDefinitionAttribute[]; }
		}

		public override XamlType GetType(string fullName)
		{
			var type = Assembly.GetType(fullName);
			if (type != null) {
				return ReflectionMapper.GetXamlType(type);
			}
			return null;
		}
	}
}
