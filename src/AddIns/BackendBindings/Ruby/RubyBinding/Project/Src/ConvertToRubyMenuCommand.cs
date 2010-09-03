// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		ScriptingTextEditorViewContent view;
		
		public override void Run()
		{
			Run(new RubyWorkbench());
		}
		
		protected void Run(IScriptingWorkbench workbench)
		{
			view = new ScriptingTextEditorViewContent(workbench);
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
