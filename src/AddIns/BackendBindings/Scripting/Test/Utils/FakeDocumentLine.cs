// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeDocumentLine : IDocumentLine
	{
		public int Offset { get; set; }
		public int Length { get; set; }
		public int EndOffset { get; set; }
		public int TotalLength { get; set; }
		public int DelimiterLength { get; set; }
		public int LineNumber { get; set; }		
		public string Text { get; set; }
	}
}
