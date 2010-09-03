// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
