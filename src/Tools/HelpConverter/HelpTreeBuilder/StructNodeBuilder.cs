using System;
using System.Xml;
using System.Reflection;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	class structNodeBuilder : classNodeBuilder
	{
		public override string ShortType {
			get {
				return "s";
			}
		}
		
		public override string Postfix
		{
			get {
				return " structure";
			}
		}
	}
}
