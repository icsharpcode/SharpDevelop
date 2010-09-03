// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Converts VB.NET or C# code to Ruby.
	/// </summary>
	public class ConvertToRubyMenuCommand : AbstractMenuCommand
	{
		RubyTextEditorViewContent view;
		
		public override void Run()
		{
			Run(new RubyWorkbench());
		}
		
		protected void Run(IScriptingWorkbench workbench)
		{
			view = new RubyTextEditorViewContent(workbench);
			string code = GenerateRubyCode();
			ShowRubyCodeInNewWindow(code);
		}
		
		string GenerateRubyCode()
		{
			ParseInformation parseInfo = GetParseInformation(view.PrimaryFileName);
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(view.PrimaryFileName, parseInfo);
			converter.IndentString = view.TextEditorOptions.IndentationString;
			return converter.Convert(view.EditableView.Text);
		}
		
		void ShowRubyCodeInNewWindow(string code)
		{
			NewFile("Generated.rb", "Ruby", code);
		}
		
		protected virtual ParseInformation GetParseInformation(string fileName)
		{
			return ParserService.GetParseInformation(fileName);
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
