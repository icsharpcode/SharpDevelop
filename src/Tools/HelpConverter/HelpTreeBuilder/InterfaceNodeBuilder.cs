using System;
using System.Xml;
using System.Reflection;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	class interfaceNodeBuilder : classNodeBuilder
	{
		public override string ShortType {
			get {
				return "i";
			}
		}
		
		public override string Postfix
		{
			get {
				return " interface";
			}
		}
	}
}
