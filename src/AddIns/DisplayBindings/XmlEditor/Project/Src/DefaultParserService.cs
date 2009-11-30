// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XmlEditor
{
	public class DefaultParserService : IParserService
	{
		public ParseInformation GetExistingParseInformation(IProjectContent content, string fileName)
		{
			return ParserService.GetExistingParseInformation(content, fileName);
		}
	}
}
