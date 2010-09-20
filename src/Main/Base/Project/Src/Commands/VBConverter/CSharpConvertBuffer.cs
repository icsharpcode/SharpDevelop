// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

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
				v.VisitCompilationUnit(p.CompilationUnit, null);
				using (SpecialNodesInserter.Install(specials, output)) {
					output.VisitCompilationUnit(p.CompilationUnit, null);
				}
				
				FileService.NewFile("Generated.cs", output.Text);
			}
		}
	}
}
