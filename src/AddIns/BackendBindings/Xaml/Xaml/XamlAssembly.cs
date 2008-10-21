using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Diagnostics;

namespace ICSharpCode.Xaml
{
	[DebuggerDisplay("{Name}")]
	public abstract class XamlAssembly
	{
		//public abstract XamlType[] Types;
		public abstract IEnumerable<XmlnsDefinitionAttribute> XmlnsDefinitions { get; }
		public abstract string Name { get; }
		public abstract XamlType GetType(string fullName);
	}
}
