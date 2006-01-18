// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace CSharpBinding
{
	public class VBToCSharpConverter : NRefactoryLanguageConverter
	{
		public override string TargetLanguageName {
			get {
				return CSharpLanguageBinding.LanguageName;
			}
		}
		
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			ConvertFile(sourceItem, targetItem, ".vb", ".cs", SupportedLanguage.VBNet, new CSharpOutputVisitor());
		}
		
		protected override void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			base.CopyProperties(sourceProject, targetProject);
			FixProperty((CSharpProject)targetProject, "DefineConstants",
			            delegate(string v) { return v.Replace(',', ';'); });
		}
		
		protected override void ConvertAst(CompilationUnit compilationUnit, List<ISpecial> specials)
		{
			PreProcessingDirective.VBToCSharp(specials);
			new VBNetToCSharpConvertVisitor().Visit(compilationUnit, null);
		}
		
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.ProjectBasePath = targetProjectDirectory;
			info.ProjectName = sourceProject.Name;
			info.OutputProjectFileName = Path.Combine(targetProjectDirectory, Path.GetFileNameWithoutExtension(sourceProject.FileName) + ".csproj");
			return new CSharpProject(info);
		}
	}
}
