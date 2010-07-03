// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Python.
	/// </summary>
	public class ConvertToPythonMenuCommand : AbstractMenuCommand
	{
		PythonTextEditorViewContent view;
		
		public override void Run()
		{
			Run(WorkbenchSingleton.Workbench);
		}
		
		protected void Run(IWorkbench workbench)
		{
			view = new PythonTextEditorViewContent(workbench);
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
