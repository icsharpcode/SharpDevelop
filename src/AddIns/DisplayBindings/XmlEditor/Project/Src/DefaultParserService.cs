// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
