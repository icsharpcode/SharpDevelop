// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class VBConvertBuffer : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content.PrimaryFileName != null && content is IEditable) {
				
				IParser p = ParserFactory.CreateParser(SupportedLanguage.CSharp, ((IEditable)content).CreateSnapshot().CreateReader());
				p.Parse();
				if (p.Errors.Count > 0) {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Convert.CorrectSourceCodeErrors}\n" + p.Errors.ErrorOutput);
					return;
				}
				
				VBNetOutputVisitor vbv = new VBNetOutputVisitor();
				
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				PreprocessingDirective.CSharpToVB(specials);
				IAstVisitor v = new CSharpToVBNetConvertVisitor(ParserService.CurrentProjectContent,
				                                                ParserService.GetParseInformation(content.PrimaryFileName));
				v.VisitSyntaxTree(p.SyntaxTree, null);
				using (SpecialNodesInserter.Install(specials, vbv)) {
					vbv.VisitSyntaxTree(p.SyntaxTree, null);
				}
				
				FileService.NewFile("Generated.vb", vbv.Text);
			}
		}
	}
}
*/