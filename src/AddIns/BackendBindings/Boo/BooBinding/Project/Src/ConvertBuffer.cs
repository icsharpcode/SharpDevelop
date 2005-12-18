// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using System.Text;

using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Parser;
using NRefactoryToBooConverter;

namespace Grunwald.BooBinding
{
	public class ConvertBuffer : AbstractMenuCommand
	{
		ConverterSettings ApplySettings(string fileName, CompilerErrorCollection errors, CompilerWarningCollection warnings)
		{
			ConverterSettings settings = new ConverterSettings(fileName, errors, warnings);
			settings.SimplifyTypeNames = true;
			return settings;
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ViewContent is IEditable) {
				CompilerErrorCollection errors = new CompilerErrorCollection();
				CompilerWarningCollection warnings = new CompilerWarningCollection();
				Module module;
				IList<ICSharpCode.NRefactory.Parser.ISpecial> specials;
				CompileUnit compileUnit = new CompileUnit();
				using (StringReader r = new StringReader(((IEditable)window.ViewContent).Text)) {
					string fileName = window.ViewContent.FileName ?? window.ViewContent.UntitledName;
					module = Parser.ParseModule(compileUnit, r, ApplySettings(fileName, errors, warnings), out specials);
				}
				if (module == null) {
					StringBuilder errorBuilder = new StringBuilder();
					foreach (CompilerError error in errors) {
						errorBuilder.AppendLine(error.ToString());
					}
					if (warnings.Count > 0) {
						foreach (CompilerWarning warning in warnings) {
							errorBuilder.AppendLine(warning.ToString());
						}
					}
					MessageService.ShowError(errorBuilder.ToString());
					return;
				}
				using (StringWriter w = new StringWriter()) {
					foreach (CompilerError error in errors) {
						w.WriteLine("ERROR: " + error.ToString());
					}
					if (errors.Count > 0)
						w.WriteLine();
					foreach (CompilerWarning warning in warnings) {
						w.WriteLine("# WARNING: " + warning.ToString());
					}
					if (warnings.Count > 0)
						w.WriteLine();
					BooPrinterVisitorWithComments printer = new BooPrinterVisitorWithComments(specials, w);
					printer.OnModule(module);
					printer.Finish();
					FileService.NewFile("Generated.boo", "Boo", w.ToString());
				}
			}
		}
	}
}
