using System;
using System.Xml;
using System.Reflection;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	class delegateNodeBuilder : classNodeBuilder
	{
		public override string Postfix {
			get {
				return " delegate";
			}
		}
		
		public override string ShortType {
			get {
				return "d";
			}
		}
		
		public override XmlNode buildNode(XmlDocument doc, Type type)
		{
			return createLinkNode(doc, type.Name + Postfix, ConvertLink(helpFileFormat.ClassTopic, type));
		}
	}
}
