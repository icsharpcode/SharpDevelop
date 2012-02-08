// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Antlr.Runtime;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptRegionStart
	{
		public static readonly string RegionStartText = "//#region ";
		public static readonly int RegionStartTextLength = RegionStartText.Length;
		
		IToken token;
		string name;
		
		public JavaScriptRegionStart(IToken token)
		{
			this.token = token;
		}
		
		public string Name {
			get {
				if (name == null) {
					GetName();
				}
				return name;
			}
		}
		
		void GetName()
		{
			string text = token.Text;
			int index = text.IndexOf(RegionStartText);
			name = text.Substring(index + RegionStartTextLength);
		}
		
		public static bool IsRegionStart(IToken token)
		{
			return token.Text.StartsWith(RegionStartText);
		}
		
		public int Line {
			get { return token.Line; }
		}
		
		public int StartColumn {
			get { return token.BeginColumn(); }
		}
	}
}
