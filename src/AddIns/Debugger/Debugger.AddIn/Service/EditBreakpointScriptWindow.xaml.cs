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

using ICSharpCode.SharpDevelop.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using Debugger.AddIn.Breakpoints;

namespace Debugger.AddIn
{
	/// <summary>
	/// Interaction logic for EditBreakpointScriptWindow.xaml
	/// </summary>
	public partial class EditBreakpointScriptWindow : Window
	{
		BreakpointBookmark data;
		
		public EditBreakpointScriptWindow(BreakpointBookmark data)
		{
			InitializeComponent();
			
			this.data = data;
			
			string language = ProjectService.CurrentProject != null ? ProjectService.CurrentProject.Language : "C#";
			this.codeEditor.Document.Text = data.Condition ?? string.Empty;
			this.codeEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(language);
		}
		
		bool CheckSyntax()
		{
			#warning reimplement this!
//			SupportedLanguage language = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), this.cmbLanguage.SelectedItem.ToString(), true);
//			using (var parser = ParserFactory.CreateParser(language, new StringReader(this.codeEditor.Document.Text))) {
//				parser.ParseExpression();
//				if (parser.Errors.Count > 0) {
//					MessageService.ShowError(parser.Errors.ErrorOutput);
//					return false;
//				}
//			}
//			
			return true;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			if (!this.CheckSyntax())
				return;
			
			this.data.Condition = this.codeEditor.Document.Text;
			DialogResult = true;
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
