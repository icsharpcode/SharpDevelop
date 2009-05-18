// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Python.
	/// </summary>
	public class ConvertToPythonMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			Run(WorkbenchSingleton.Workbench, SharpDevelopTextEditorProperties.Instance);
		}
		
		protected void Run(IWorkbench workbench, ITextEditorProperties textEditorProperties)
		{
			// Get the code to convert.
			IViewContent viewContent = workbench.ActiveWorkbenchWindow.ActiveViewContent;
			IEditable editable = viewContent as IEditable;
			
			// Generate the python code.
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(viewContent.PrimaryFileName);
			converter.IndentString = NRefactoryToPythonConverter.GetIndentString(textEditorProperties);
			string pythonCode = converter.Convert(editable.Text);
			
			// Show the python code in a new window.
			NewFile("Generated.py", "Python", pythonCode);
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
