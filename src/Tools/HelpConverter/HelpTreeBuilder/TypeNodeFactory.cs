using System;
using System.Xml;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	public class TypeNodeFactory
	{
		public static XmlNode CreateNode(Type type, XmlDocument doc)
		{
			ITypeNodeBuilder builder;
			
			if(type.IsClass == true && type.IsSubclassOf(typeof(MulticastDelegate)) == false) {
				builder = new classNodeBuilder();
			} else if(type.IsClass == true && type.IsSubclassOf(typeof(MulticastDelegate)) == true) {
				builder = new delegateNodeBuilder();
			} else if(type.IsEnum) {
				builder = new enumNodeBuilder();
			} else if(type.IsInterface) {
				builder = new interfaceNodeBuilder();
			} else if(type.IsValueType == true) {
				builder = new structNodeBuilder();
			} else {
				throw new Exception("Generation for this type is currently not supported: " + type.Name);
			}
			
			return builder.buildNode(doc, type);
		}
	}
}
