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
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Python.
	/// </summary>
	public class ConvertToPythonMenuCommand : AbstractMenuCommand
	{
		ScriptingTextEditorViewContent view;
		
		public override void Run()
		{
			Run(new PythonWorkbench());
		}
		
		protected void Run(IScriptingWorkbench workbench)
		{
			view = new ScriptingTextEditorViewContent(workbench);
			string code = GeneratePythonCode();
			ShowPythonCodeInNewWindow(code);
		}
			
		string GeneratePythonCode()
		{
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(view.PrimaryFileName);
			converter.IndentString = view.TextEditorOptions.IndentationString;
			return converter.Convert(view.EditableView.Text);
		}
		
		void ShowPythonCodeInNewWindow(string code)
		{
			NewFile("Generated.py", "Python", code);
		}
		
		/// <summary>
		/// Creates a new file using the FileService by default.
		/// </summary>
		protected virtual void NewFile(string defaultName, string language, string content)
		{
			FileService.NewFile(defaultName, content);
		}		
	}
}
