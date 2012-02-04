// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingStringParser : ITextTemplatingStringParser
	{
		public string GetValue(string propertyName)
		{
			return StringParser.GetValue(propertyName);
		}
	}
}
