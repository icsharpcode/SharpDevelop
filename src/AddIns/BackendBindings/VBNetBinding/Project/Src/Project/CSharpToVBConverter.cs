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

namespace VBNetBinding
{
	public class CSharpToVBConverter : NRefactoryLanguageConverter
	{
		public override string TargetLanguageName {
			get {
				return VBNetLanguageBinding.LanguageName;
			}
		}
		
		protected override void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			ConvertFile(sourceItem, targetItem, ".cs", ".vb", SupportedLanguage.CSharp, new VBNetOutputVisitor());
		}
		
		protected override void ConvertAst(CompilationUnit compilationUnit, List<ISpecial> specials)
		{
			PreProcessingDirective.CSharpToVB(specials);
			new CSharpToVBNetConvertVisitor().Visit(compilationUnit, null);
		}
		
		protected override IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.ProjectBasePath = targetProjectDirectory;
			info.ProjectName = sourceProject.Name;
			info.OutputProjectFileName = Path.Combine(targetProjectDirectory, Path.GetFileNameWithoutExtension(sourceProject.FileName) + ".vbproj");
			return new VBNetProject(info);
		}
	}
}
