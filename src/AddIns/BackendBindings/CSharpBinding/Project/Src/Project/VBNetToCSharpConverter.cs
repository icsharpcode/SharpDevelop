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

/*
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace CSharpBinding
{
	public class VBToCSharpConverter : NRefactoryLanguageConverter
	{
		public override string TargetLanguageName {
			get {
				return CSharpProjectBinding.LanguageName;
			}
		}
		
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			CSharpProject project = (CSharpProject)base.CreateProject(targetProjectDirectory, sourceProject);
			IProjectItemListProvider provider = (IProjectItemListProvider)project;
			provider.AddProjectItem(new ReferenceProjectItem(project, "Microsoft.VisualBasic"));
			
			FileProjectItem fileItem = new FileProjectItem(project, ItemType.Compile, "MyNamespaceSupportForCSharp.cs");
			provider.AddProjectItem(fileItem);
			try {
				File.WriteAllText(fileItem.FileName, CSharpMyNamespaceBuilder.BuildMyNamespaceCode((CompilableProject)sourceProject));
			} catch (Exception ex) {
				conversionLog.AppendLine(ex.ToString());
			}
			
			return project;
		}
		
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			ConvertFile(sourceItem, targetItem, ".vb", ".cs", SupportedLanguage.VBNet, new CSharpOutputVisitor());
		}
		
		protected override void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			base.CopyProperties(sourceProject, targetProject);
			
			CSharpProject project = (CSharpProject)targetProject;
			
			// 1591 = missing XML comment - the VB compiler does not have this warning
			// we disable it by default because many VB projects have XML documentation turned on
			// even though only few members are commented
			// (we replace existing NoWarn entries because VB and C# error codes don't match)
			project.SetProperty("NoWarn", "1591");
			
			project.ChangeProperty("DefineConstants",
			                       v => v.Replace(',', ';'));
			
			project.ChangeProperty("ProjectTypeGuids",
			                       v => v.Replace(ProjectTypeGuids.VBNet, ProjectTypeGuids.CSharp, StringComparison.OrdinalIgnoreCase));
		}
		
		protected override void ConvertAst(SyntaxTree compilationUnit, List<ISpecial> specials, FileProjectItem sourceItem)
		{
			PreprocessingDirective.VBToCSharp(specials);
			CompilableProject project = (CompilableProject)sourceItem.Project;
			RemoveWindowsFormsSpecificCode(compilationUnit, specials, project.OutputType == OutputType.WinExe);
			
			IProjectContent pc = ParserService.GetProjectContent(sourceItem.Project) ?? ParserService.CurrentProjectContent;
			VBNetToCSharpConvertVisitor visitor = new VBNetToCSharpConvertVisitorWithMyFormsSupport(pc, ParserService.GetParseInformation(sourceItem.FileName), sourceItem.Project.RootNamespace);
						
			// set project options
			visitor.OptionInfer = (project.GetEvaluatedProperty("OptionInfer") ?? "Off")
				.Equals("On", StringComparison.OrdinalIgnoreCase);
			visitor.OptionStrict = (project.GetEvaluatedProperty("OptionStrict") ?? "Off")
				.Equals("On", StringComparison.OrdinalIgnoreCase);
			
			compilationUnit.AcceptVisitor(visitor, null);
		}
		
		void RemoveWindowsFormsSpecificCode(SyntaxTree compilationUnit, List<ISpecial> specials, bool keepCode)
		{
			for (int i = 0; i < specials.Count; i++) {
				PreprocessingDirective ppd = specials[i] as PreprocessingDirective;
				if (ppd != null && ppd.Cmd == "#if") {
					if (ppd.Arg == "_MyType = \"WindowsForms\"") {
						int depth = 1;
						for (int j = i + 1; j < specials.Count; j++) {
							ppd = specials[j] as PreprocessingDirective;
							if (ppd != null) {
								if (ppd.Cmd == "#if") {
									depth++;
								} else if (ppd.Cmd == "#endif") {
									depth--;
									if (depth == 0) {
										if (keepCode) {
											// keep code, remove only the ifdef
											specials.RemoveAt(j);
											specials.RemoveAt(i);
										} else {
											// remove ifdef including the code
											compilationUnit.AcceptVisitor(new RemoveMembersInRangeVisitor(
												new DomRegion(specials[i].StartPosition, specials[j].EndPosition)), null);
											specials.RemoveRange(i, j - i + 1);
										}
										i--;
										break;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
*/
