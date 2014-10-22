// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	sealed class SimpleHighlightingItem : IHighlightingItem
	{
		readonly Action<TextArea> onShowExample;
		
		public SimpleHighlightingItem(string name, Action<TextArea> onShowExample)
		{
			this.Name = name;
			this.onShowExample = onShowExample;
		}
		
		public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }
		
		public string Name { get; private set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public bool Underline { get; set; }
		
		public Color Foreground { get; set; }
		public bool UseDefaultForeground { get; set; }
		public Color Background { get; set; }
		public bool UseDefaultBackground { get; set; }
		
		public bool CanUseDefaultColors { get; set; }
		
		public bool CanSetForeground {
			get { return false; }
		}
		
		public bool CanSetBackground {
			get { return false; }
		}
		
		public bool CanSetFont {
			get { return false; }
		}
		
		public bool IsCustomized {
			get { return false; }
		}
		
		public IHighlightingDefinition ParentDefinition { get; set; }
		
		public void Reset()
		{
		}
		
		public void ShowExample(TextArea exampleTextArea)
		{
			if (onShowExample != null)
				onShowExample(exampleTextArea);
		}
	}
}
