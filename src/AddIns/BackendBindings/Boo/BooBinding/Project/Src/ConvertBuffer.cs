// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NRefactoryToBooConverter;

namespace Grunwald.BooBinding
{
	public class ConvertBuffer : AbstractMenuCommand
	{
		public static ConverterSettings ApplySettings(string fileName, CompilerErrorCollection errors, CompilerWarningCollection warnings)
		{
			ConverterSettings settings = new ConverterSettings(fileName, errors, warnings);
			settings.SimplifyTypeNames = true;
			return settings;
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ActiveViewContent is IEditable) {
				CompilerErrorCollection errors = new CompilerErrorCollection();
				CompilerWarningCollection warnings = new CompilerWarningCollection();
				Module module;
				IList<ICSharpCode.NRefactory.ISpecial> specials;
				CompileUnit compileUnit = new CompileUnit();
				using (TextReader r = ((IEditable)window.ActiveViewContent).CreateSnapshot().CreateReader()) {
					string fileName = window.ActiveViewContent.PrimaryFileName;
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
				} else {
					FileService.NewFile("Generated.boo", CreateBooCode(errors, warnings, module, specials));
				}
			}
		}
		
		public static string CreateBooCode(CompilerErrorCollection errors, CompilerWarningCollection warnings,
		                                   Module module, IList<ICSharpCode.NRefactory.ISpecial> specials)
		{
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
				return w.ToString();
			}
		}
	}
}
