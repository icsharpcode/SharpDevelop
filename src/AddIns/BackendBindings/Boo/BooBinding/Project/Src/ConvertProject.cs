// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using NRefactoryToBooConverter;

namespace Grunwald.BooBinding
{
	public class ProjectToBooConverter : LanguageConverter
	{
		public override string TargetLanguageName {
			get {
				return BooProjectBinding.LanguageName;
			}
		}
		
		CompilerErrorCollection errors = new CompilerErrorCollection();
		CompilerWarningCollection warnings = new CompilerWarningCollection();
		
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			errors.Clear();
			warnings.Clear();
			
			return base.CreateProject(targetProjectDirectory, sourceProject);
		}
		
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			FixExtensionOfExtraProperties(targetItem, ".cs", ".boo");
			FixExtensionOfExtraProperties(targetItem, ".vb", ".boo");
			
			string ext = Path.GetExtension(sourceItem.FileName);
			if (".cs".Equals(ext, StringComparison.OrdinalIgnoreCase) || ".vb".Equals(ext, StringComparison.OrdinalIgnoreCase)) {
				Module module;
				IList<ICSharpCode.NRefactory.ISpecial> specials;
				CompileUnit compileUnit = new CompileUnit();
				using (TextReader r = ParserService.GetParseableFileContent(sourceItem.FileName).CreateReader()) {
					module = Parser.ParseModule(compileUnit, r, ConvertBuffer.ApplySettings(sourceItem.VirtualName, errors, warnings), out specials);
				}
				if (module == null) {
					conversionLog.AppendLine("Could not parse '" + sourceItem.FileName + "', see error list for details.");
					base.ConvertFile(sourceItem, targetItem);
				} else {
					using (StringWriter w = new StringWriter()) {
						BooPrinterVisitorWithComments printer = new BooPrinterVisitorWithComments(specials, w);
						printer.OnModule(module);
						printer.Finish();
						
						targetItem.Include = Path.ChangeExtension(targetItem.Include, ".boo");
						File.WriteAllText(targetItem.FileName, w.ToString());
					}
				}
			} else {
				base.ConvertFile(sourceItem, targetItem);
			}
		}
		
		protected override void AfterConversion(IProject targetProject)
		{
			base.AfterConversion(targetProject);
			
			if (errors.Count > 0) {
				conversionLog.AppendLine(errors.Count + " conversion errors:");
				foreach (CompilerError error in errors) {
					conversionLog.Append("  ");
					conversionLog.AppendLine(error.ToString());
				}
				conversionLog.AppendLine();
			}
			if (warnings.Count > 0) {
				conversionLog.AppendLine(warnings.Count + " warnings:");
				foreach (CompilerWarning warning in warnings) {
					conversionLog.Append("  ");
					conversionLog.AppendLine(warning.ToString());
				}
				conversionLog.AppendLine();
			}
			
			
			errors.Clear();
			warnings.Clear();
		}
	}
}
