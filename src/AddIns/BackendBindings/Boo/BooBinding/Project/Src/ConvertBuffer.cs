// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
				StringBuilder errorBuilder = new StringBuilder();
				
				CompilerErrorCollection errors = new CompilerErrorCollection();
				CompilerWarningCollection warnings = new CompilerWarningCollection();
				Module module;
				IList<ICSharpCode.NRefactory.Parser.ISpecial> specials;
				CompileUnit compileUnit = new CompileUnit();
				using (StringReader r = new StringReader(((IEditable)window.ViewContent).Text)) {
					module = Parser.ParseModule(compileUnit, r, ApplySettings(window.ViewContent.FileName, errors, warnings), out specials);
				}
				if (errors.Count > 0) {
					foreach (CompilerError error in errors) {
						errorBuilder.AppendLine(error.ToString());
					}
				} else {
					if (warnings.Count > 0) {
						foreach (CompilerWarning warning in warnings) {
							errorBuilder.AppendLine(warning.ToString());
						}
					}
					using (StringWriter w = new StringWriter()) {
						BooPrinterVisitorWithComments printer = new BooPrinterVisitorWithComments(specials, w);
						printer.OnModule(module);
						printer.Finish();
						FileService.NewFile("Generated.boo", "Boo", w.ToString());
					}
				}
				if (errorBuilder.Length > 0) {
					MessageService.ShowMessage(errorBuilder.ToString());
				}
			}
		}
	}
}
