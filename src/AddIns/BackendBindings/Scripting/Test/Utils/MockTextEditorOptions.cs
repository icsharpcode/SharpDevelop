// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockTextEditorOptions : TextEditorOptions, ITextEditorOptions
	{
		public MockTextEditorOptions()
		{
		}
		
		public bool AutoInsertBlockEnd {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int VerticalRulerColumn {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool UnderlineErrors {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string FontFamily {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
