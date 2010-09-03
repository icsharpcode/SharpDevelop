// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.VBNetBinding
{
	public class CSharpToVBNetConverter : NRefactoryLanguageConverter
	{
		public override string TargetLanguageName {
			get {
				return VBNetProjectBinding.LanguageName;
			}
		}
		
		IList<string> defaultImports = new[] { "Microsoft.VisualBasic", "System" };
		
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			VBNetProject project = (VBNetProject)base.CreateProject(targetProjectDirectory, sourceProject);
			IProjectItemListProvider provider = (IProjectItemListProvider)project;
			foreach (string import in defaultImports) {
				provider.AddProjectItem(new ImportProjectItem(project, import));
			}
			return project;
		}
		
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			ConvertFile(sourceItem, targetItem, ".cs", ".vb", SupportedLanguage.CSharp, new VBNetOutputVisitor());
		}
		
		string startupObject;
		
		protected override void ConvertAst(CompilationUnit compilationUnit, List<ISpecial> specials, FileProjectItem sourceItem)
		{
			PreprocessingDirective.CSharpToVB(specials);
			IProjectContent pc = ParserService.GetProjectContent(sourceItem.Project) ?? ParserService.CurrentProjectContent;
			CSharpToVBNetConvertVisitor visitor = new CSharpToVBNetConvertVisitor(pc, ParserService.GetParseInformation(sourceItem.FileName));
			visitor.RootNamespaceToRemove = sourceItem.Project.RootNamespace;
			visitor.DefaultImportsToRemove = defaultImports;
			visitor.StartupObjectToMakePublic = startupObject;
			compilationUnit.AcceptVisitor(visitor, null);
		}
		
		protected override void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			VBNetProject vbProject = (VBNetProject)targetProject;
			base.CopyProperties(sourceProject, targetProject);
			vbProject.ChangeProperty("DefineConstants", v => v.Replace(';', ','));
			vbProject.ChangeProperty("ProjectTypeGuids",
			                         v => v.Replace(ProjectTypeGuids.CSharp, ProjectTypeGuids.VBNet, StringComparison.OrdinalIgnoreCase));
			
			// determine the StartupObject
			startupObject = vbProject.GetEvaluatedProperty("StartupObject");
			if (string.IsNullOrEmpty(startupObject)) {
				IList<IClass> startupObjects = ICSharpCode.SharpDevelop.Gui.OptionPanels.ApplicationSettings.GetPossibleStartupObjects(sourceProject);
				if (startupObjects.Count == 1) {
					startupObject = startupObjects[0].FullyQualifiedName;
					if (vbProject.OutputType == OutputType.WinExe) {
						// we have to set StartupObject to prevent the VB compiler from choosing
						// the generated Main method.
						vbProject.SetProperty("StartupObject", startupObject);
					}
				}
			}
		}
	}
}
