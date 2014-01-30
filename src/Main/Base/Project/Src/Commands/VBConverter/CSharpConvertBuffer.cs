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
/* TODO: Reimplement C#<->VB converter

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class CSharpConvertBuffer : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content.PrimaryFileName != null && content is IEditable) {
				
				IParser p = ParserFactory.CreateParser(SupportedLanguage.VBNet, ((IEditable)content).CreateSnapshot().CreateReader());
				p.Parse();

				if (p.Errors.Count > 0) {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Convert.CorrectSourceCodeErrors}\n" + p.Errors.ErrorOutput);
					return;
				}
				ICSharpCode.NRefactory.PrettyPrinter.CSharpOutputVisitor output = new ICSharpCode.NRefactory.PrettyPrinter.CSharpOutputVisitor();
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				PreprocessingDirective.VBToCSharp(specials);
				IAstVisitor v = new VBNetToCSharpConvertVisitor(ParserService.CurrentProjectContent,
				                                                ParserService.GetParseInformation(content.PrimaryFileName));
				v.VisitSyntaxTree(p.SyntaxTree, null);
				using (SpecialNodesInserter.Install(specials, output)) {
					output.VisitSyntaxTree(p.SyntaxTree, null);
				}
				
				FileService.NewFile("Generated.cs", output.Text);
			}
		}
	}
}
*/
