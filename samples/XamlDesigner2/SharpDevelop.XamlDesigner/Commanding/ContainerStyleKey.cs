using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Reflection;

namespace SharpDevelop.XamlDesigner.Commanding
{
	public class ContainerStyleKey : ResourceKey
	{
		public ContainerStyleKey(Type containerType)
		{
			ContainerType = containerType;
		}

		public Type ContainerType { get; set; }

		public override Assembly Assembly
		{
			get { return ContainerType.Assembly; }
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public override bool Equals(object obj)
		{
			var key = obj as ContainerStyleKey;
			if (key != null) {
				return key.ContainerType == ContainerType;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ContainerType.GetHashCode();
		}
	}
}
