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
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	/// <summary>
	/// Represents an item visible in the highlighting editor.
	/// </summary>
	public interface IHighlightingItem : INotifyPropertyChanged
	{
		string Name { get; }
		
		/// <summary>
		/// Gets/Sets whether the element uses bold font.
		/// </summary>
		bool Bold { get; set; }
		
		/// <summary>
		/// Gets/Sets whether the element uses italic font.
		/// </summary>
		bool Italic { get; set; }
		
		/// <summary>
		/// Gets/Sets whether the element uses an underlined font.
		/// </summary>
		bool Underline { get; set; }
		
		Color Foreground { get; set; }
		bool UseDefaultForeground { get; set; }
		Color Background { get; set; }
		bool UseDefaultBackground { get; set; }
		
		bool CanUseDefaultColors { get; }
		bool CanSetForeground { get; }
		bool CanSetBackground { get; }
		bool CanSetFont { get; }
		
		bool IsCustomized { get; }
		void Reset();
		
		/// <summary>
		/// Parent definition to use in conjunction with the example.
		/// </summary>
		IHighlightingDefinition ParentDefinition { get; }
		
		/// <summary>
		/// Shows an example
		/// </summary>
		void ShowExample(TextArea exampleTextArea);
	}
}
