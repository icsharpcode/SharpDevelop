using System;
using System.Xml;
using System.Reflection;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	public class enumNodeBuilder : classNodeBuilder
	{
		public override string Postfix {
			get {
				return " enumeration";
			}
		}
		
		public override string ShortType {
			get {
				return "e";
			}
		}
		
		public override XmlNode buildNode(XmlDocument doc, Type type)
		{
			return createLinkNode(doc, type.Name + Postfix, ConvertLink(helpFileFormat.ClassTopic, type));
		}
	}
}
