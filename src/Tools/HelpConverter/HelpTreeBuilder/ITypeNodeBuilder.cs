using System;
using System.Xml;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	public interface ITypeNodeBuilder
	{
		XmlNode buildNode(XmlDocument doc, Type type);
	}
}
