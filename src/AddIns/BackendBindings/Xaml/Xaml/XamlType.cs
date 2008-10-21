using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	public abstract class XamlType
	{
		public abstract string Name { get; }
		public abstract bool IsDefaultConstructible { get; }
		public abstract bool IsNullable { get; }
		public abstract IEnumerable<XamlMember> Members { get; }
		public abstract XamlMember ContentProperty { get; }
		public abstract XamlMember DictionaryKeyProperty { get; }
		public abstract XamlMember NameProperty { get; }
		public abstract XamlMember XmlLangProperty { get; }
		public abstract bool TrimSurroundingWhitespace { get; }
		public abstract bool IsWhitespaceSignificantCollection { get; }
		public abstract bool IsCollection { get; }
		public abstract bool IsDictionary { get; }
		public abstract IEnumerable<XamlType> AllowedTypes { get; }
		public abstract IEnumerable<XamlType> AllowedKeyTypes { get; }
		public abstract bool IsXData { get; }
		public abstract bool IsNameScope { get; }
		public abstract IEnumerable<Constructor> Constructors { get; }
		public abstract XamlType ReturnValueType { get; }
		public abstract bool HasTextSyntax { get; }

		public abstract string Namespace { get; }
		public abstract XamlAssembly Assembly { get; }
		public abstract XamlMember Member(string name);
		public abstract IEnumerable<XamlType> ContentWrappers { get; }
		public abstract Type SystemType { get; }

		public abstract bool IsAssignableFrom(XamlType type);
		public abstract T GetAttribute<T>() where T : Attribute;

		public bool IsMarkupExtension
		{
			get { return IntristicType.MarkupExtension.IsAssignableFrom(this); }
		}

		public override string ToString()
		{
			return GetType().Name + ": " + Name;
		}
	}
}
