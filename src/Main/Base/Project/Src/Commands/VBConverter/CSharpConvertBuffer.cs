// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class CSharpConvertBuffer : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ViewContent is IEditable) {
				ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(((IEditable)window.ViewContent).Text));
				p.Parse();

				if (p.Errors.count > 0) {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Convert.CorrectSourceCodeErrors}\n" + p.Errors.ErrorOutput);
					return;
				}
				ICSharpCode.NRefactory.PrettyPrinter.CSharpOutputVisitor output = new ICSharpCode.NRefactory.PrettyPrinter.CSharpOutputVisitor();
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				PreprocessingDirective.VBToCSharp(specials);
				new VBNetToCSharpConvertVisitor().Visit(p.CompilationUnit, null);
				using (SpecialNodesInserter.Install(specials, output)) {
					output.Visit(p.CompilationUnit, null);
				}
				
				FileService.NewFile("Generated.CS", "C#", output.Text);
			}
		}
	}
}
