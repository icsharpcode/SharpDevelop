using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ICSharpCode.Xaml
{
	public class IntristicMember : XamlMember
	{
		public IntristicMember(string name)
		{
			this.name = name;
		}

		public static XamlMember Items = new IntristicMember("Items");
		public static XamlMember ConsructorArgs = new IntristicMember("ConsructorArgs");
		public static XamlMember InitializationText = new IntristicMember("InitializationText");
		public static XamlMember DirectiveChildren = new IntristicMember("DirectiveChildren");

		string name;

		public override string Name
		{
			get { return name; }
		}

		public override XamlType OwnerType
		{
			get { return null; }
		}

		public override XamlType ValueType
		{
			get { return ReflectionMapper.GetXamlType(typeof(object)); }
		}

		public override XamlType TargetType
		{
			get { throw new NotImplementedException(); }
		}

		public override AllowedLocation AllowedLocation
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsEvent
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsDirective
		{
			get { return true; }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override bool IsStatic
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsAttachable
		{
			get { return false; }
		}

		public override T GetAttribute<T>()
		{
			throw new NotImplementedException();
		}

		public override bool HasTextSyntax
		{
			get { throw new NotImplementedException(); }
		}
	}
}
