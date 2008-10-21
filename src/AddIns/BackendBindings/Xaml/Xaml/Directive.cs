using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ICSharpCode.Xaml
{
	public class Directive
	{
		static Directive()
		{
			Name = CreateXamlDirective("Name");
			Key = CreateXamlDirective("Key");
			Uid = CreateXamlDirective("Uid");
			Class = CreateXamlDirective("Class");
			ClassModifier = CreateXamlDirective("ClassModifier");
			FieldModifier = CreateXamlDirective("FieldModifier");
			TypeArguments = CreateXamlDirective("TypeArguments");
			XmlLang = CreateDirective(XNamespace.Xml + "lang");
			XmlSpace = CreateDirective(XNamespace.Xml + "space");
		}

		public static XamlMember Name;
		public static XamlMember Key;
		public static XamlMember Uid;
		public static XamlMember Class;
		public static XamlMember ClassModifier;
		public static XamlMember FieldModifier;
		public static XamlMember TypeArguments;
		public static XamlMember XmlLang;
		public static XamlMember XmlSpace;

		static XamlMember CreateXamlDirective(string name)
		{
			return CreateDirective(XamlConstants.XamlNamespace + name);
		}

		static XamlMember CreateDirective(XName name)
		{
			var result = new IntristicMember(name.LocalName);
			directiveFromName[name] = result;
			nameFromDirective[result] = name;
			return result;
		}

		static Dictionary<XName, XamlMember> directiveFromName = new Dictionary<XName, XamlMember>();
		static Dictionary<XamlMember, XName> nameFromDirective = new Dictionary<XamlMember, XName>();

		public static XamlMember GetDirective(XName name)
		{
			XamlMember result;
			if (directiveFromName.TryGetValue(name, out result)) {
				return result;
			}
			return null;
		}

		public static XName GetDirectiveName(XamlMember member)
		{
			XName result;
			if (nameFromDirective.TryGetValue(member, out result)) {
				return result;
			}
			return null;
		}
	}
}
