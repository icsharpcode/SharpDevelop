// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor;

namespace PythonBinding.Tests.Utils
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
	}
}
