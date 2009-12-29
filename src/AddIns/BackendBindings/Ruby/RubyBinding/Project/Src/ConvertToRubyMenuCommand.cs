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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Ruby.
	/// </summary>
	public class ConvertToRubyMenuCommand : AbstractMenuCommand
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
			
			// Generate the ruby code.
			ParseInformation parseInfo = GetParseInformation(viewContent.PrimaryFileName);
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(viewContent.PrimaryFileName, parseInfo);
			converter.IndentString = NRefactoryToRubyConverter.GetIndentString(textEditorProperties);
			string pythonCode = converter.Convert(editable.Text);
			
			// Show the python code in a new window.
			NewFile("Generated.rb", "Ruby", pythonCode);
		}
		
		/// <summary>
		/// Creates a new file using the FileService by default.
		/// </summary>
		protected virtual void NewFile(string defaultName, string language, string content)
		{
			FileService.NewFile(defaultName, content);
		}
		
		protected virtual ParseInformation GetParseInformation(string fileName)
		{
			return ParserService.GetParseInformation(fileName);
		}
	}
}
