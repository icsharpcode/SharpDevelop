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
	public class MockTextEditorOptions : ITextEditorOptions
	{
		public MockTextEditorOptions()
		{
			IndentationString = "\t";
		}
		
		public string IndentationString { get; set; }
		
		public bool AutoInsertBlockEnd {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool ConvertTabsToSpaces {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int IndentationSize {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int VerticalRulerColumn {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
