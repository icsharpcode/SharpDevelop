// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add { } remove { } }
		
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
