using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ICSharpCode.Xaml
{
	public abstract class XamlMember
	{
		public abstract string Name { get; }
		public abstract XamlType OwnerType { get; }
		public abstract XamlType ValueType { get; }
		public abstract bool IsReadOnly { get; }
		public abstract bool IsStatic { get; }
		public abstract bool IsAttachable { get; }
		public abstract XamlType TargetType { get; }
		public abstract AllowedLocation AllowedLocation { get; }
		public abstract bool IsEvent { get; }
		public abstract bool IsDirective { get; }
		public abstract bool HasTextSyntax { get; }

		public bool IsNameProperty
		{
			get
			{
				return this == Directive.Name || this == OwnerType.NameProperty;
			}
		}

		public abstract T GetAttribute<T>() where T : Attribute;

		public override string ToString()
		{
			return GetType().Name + ": " + Name;
		}
	}
}
