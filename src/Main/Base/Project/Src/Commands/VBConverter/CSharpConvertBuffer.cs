// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
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
				ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(SupportedLanguages.VBNet, new StringReader(((IEditable)window.ViewContent).Text));
				p.Parse();

				if (p.Errors.count > 0) {
					
					MessageService.ShowError("Correct source code errors first (only correct source code would convert).");
					return;
				}
				ICSharpCode.NRefactory.PrettyPrinter.CSharpOutputVisitor vbv = new ICSharpCode.NRefactory.PrettyPrinter.CSharpOutputVisitor();
				vbv.Visit(p.CompilationUnit, null);
				
				
				FileService.NewFile("Generated.CS", "C#", vbv.Text);
			}
		}
	}
}
