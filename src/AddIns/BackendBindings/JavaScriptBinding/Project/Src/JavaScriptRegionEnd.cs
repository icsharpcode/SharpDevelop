// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Antlr.Runtime;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptRegionEnd
	{
		public static readonly string RegionEndText = "//#endregion";
		public static readonly int RegionEndTextLength = RegionEndText.Length;
		
		IToken token;
		
		public JavaScriptRegionEnd(IToken token)
		{
			this.token = token;
		}
		
		public static bool IsRegionEnd(IToken token)
		{
			return token.Text.StartsWith(RegionEndText);
		}
		
		public int Line {
			get { return token.Line; }
		}
		
		public int EndColumn {
			get { return token.BeginColumn() + RegionEndTextLength; }
		}
	}
}
