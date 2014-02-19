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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/*
	/// <summary>
	/// This class handles errors in the Load method and prevents destroying invalid files.
	/// Scenario:
	/// open a .resx file both with the resource editor and the text editor. Add a string entry using the
	/// resource editor. Modify it using the text editor, and introduce invalid XML syntax (e.g. use a
	/// &lt; in the edited value. Close the text editor without saving the changes. The resource editor
	/// will show the load error. Close the resource editor, this time saving the changes.
	/// The resource editor is now expected to write the invalid file to disk.
	/// This class handles this scenario by displaying an error message for invalid files, and
	/// holding the invalid data that got copied from the text editor to the resource editor in memory.
	/// So saving during a load error works as expected.
	/// </summary>
	public abstract class AbstractViewContentHandlingLoadErrors : AbstractViewContent
	{
		ContentPresenter contentControl = new ContentPresenter();
		object userContent;
		
		protected AbstractViewContentHandlingLoadErrors()
		{
		}
		
		protected AbstractViewContentHandlingLoadErrors(OpenedFile file) : base(file)
		{
		}
		
		public sealed override object Control {
			get { return contentControl; }
		}
		
		protected object UserContent {
			get { return userContent; }
			set {
				if (userContent != value) {
					userContent = value;
					if (errorList.Count == 0) {
						SD.WinForms.SetContent(contentControl, userContent, this);
					}
				}
			}
		}
		
		TextBox errorTextBox;
		
		protected void ShowError(Exception ex)
		{
			if (errorTextBox == null) {
				errorTextBox = new TextBox();
				errorTextBox.IsReadOnly = true;
				errorTextBox.Background = SystemColors.WindowBrush;
			}
			errorTextBox.Text = String.Concat(this.LoadErrorHeaderText, ex.ToString());
			SD.WinForms.SetContent(contentControl, errorTextBox, this);
		}
		
		/// <summary>
		/// Gets a text to be shown above the exception when a load error occurs.
		/// The default is an empty string.
		/// </summary>
		protected virtual string LoadErrorHeaderText {
			get { return String.Empty; }
		}
	}
	*/
}
