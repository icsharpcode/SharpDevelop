// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace RubyBinding.Tests.Utils
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
