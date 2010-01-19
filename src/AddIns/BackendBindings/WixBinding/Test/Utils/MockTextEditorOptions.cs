// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace WixBinding.Tests.Utils
{
	public class MockTextEditorOptions : ITextEditorOptions
	{
		string indentationString = "\t";
		int indentationSize = 4;
		bool convertTabsToSpaces;
		
		public MockTextEditorOptions()
		{
		}
		
		public string IndentationString {
			get { return indentationString; }
			set { indentationString = value; }
		}
		
		public bool AutoInsertBlockEnd {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool ConvertTabsToSpaces {
			get { return convertTabsToSpaces; }
			set { convertTabsToSpaces = value; }
		}
		
		public int IndentationSize {
			get { return indentationSize; }
			set { indentationSize = value; }
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
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add { } remove { } }
	}
}
