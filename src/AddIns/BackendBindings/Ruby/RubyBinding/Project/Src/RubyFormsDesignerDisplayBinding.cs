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
using ICSharpCode.FormsDesigner;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Forms designer display binding for Ruby files.
	/// </summary>
	public class RubyFormsDesignerDisplayBinding : ISecondaryDisplayBinding
	{
		public RubyFormsDesignerDisplayBinding()
		{
		}
		
		/// <summary>
		/// Returns true so that the CreateSecondaryViewContent method
		/// is called after the LoadSolutionProjects thread has finished.
		/// </summary>
		public bool ReattachWhenParserServiceIsReady {
			get { return true; }
		}
		
		public bool CanAttachTo(IViewContent content)
		{
			ITextEditorProvider textEditorProvider = content as ITextEditorProvider;
			if (textEditorProvider != null) {
				if (IsRubyFile(content.PrimaryFileName)) {
					ParseInformation parseInfo = GetParseInfo(content.PrimaryFileName, textEditorProvider.TextEditor.Document);
					return IsDesignable(parseInfo);
				}
			}
			return false;
		}

		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			ScriptingTextEditorViewContent textEditorView = new ScriptingTextEditorViewContent(viewContent);
			return CreateSecondaryViewContent(viewContent, textEditorView.TextEditorOptions);
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent, ITextEditorOptions textEditorOptions)
		{
			foreach (IViewContent existingView in viewContent.SecondaryViewContents) {
				if (existingView.GetType() == typeof(FormsDesignerViewContent)) {
					return new IViewContent[0];
				}
			}
			
			IDesignerLoaderProvider loader = new RubyDesignerLoaderProvider();
			IDesignerGenerator generator = new RubyDesignerGenerator(textEditorOptions);
			return new IViewContent[] { new FormsDesignerViewContent(viewContent, loader, generator) };
		}
		
		/// <summary>
		/// Gets the parse information from the parser service
		/// for the specified file.
		/// </summary>
		protected virtual ParseInformation GetParseInfo(string fileName, ITextBuffer textContent)
		{
			return ParserService.ParseFile(fileName, textContent);
		}
		
		/// <summary>
		/// Determines whether the specified parse information contains
		/// a class which is designable.
		/// </summary>
		protected virtual bool IsDesignable(ParseInformation parseInfo)
		{
			return FormsDesignerSecondaryDisplayBinding.IsDesignable(parseInfo);
		}
				
		/// <summary>
		/// Checks the file's extension represents a Ruby file.
		/// </summary>
		static bool IsRubyFile(string fileName)
		{
			RubyParser parser = new RubyParser();
			return parser.CanParse(fileName);
		}
	}
}
