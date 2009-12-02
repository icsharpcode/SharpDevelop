// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
