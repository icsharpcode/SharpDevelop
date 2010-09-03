// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace XmlEditor.Tests.Utils
{
	public class MockDocumentLine : IDocumentLine
	{
		public MockDocumentLine()
		{
		}
		
		public int Offset {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int Length {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int EndOffset {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TotalLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int DelimiterLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int LineNumber { get; set; }
		
		public string Text {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
