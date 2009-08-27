// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using TypeResolutionService = ICSharpCode.FormsDesigner.Services.TypeResolutionService;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class MyTypeFinder : XamlTypeFinder
	{
		OpenedFile file;
		readonly TypeResolutionService typeResolutionService = new TypeResolutionService();
		
		public static MyTypeFinder Create(OpenedFile file)
		{
			MyTypeFinder f = new MyTypeFinder();
			f.file = file;
			f.ImportFrom(CreateWpfTypeFinder());
			return f;
		}
		
		public override Assembly LoadAssembly(string name)
		{
			if (string.IsNullOrEmpty(name)) {
				IProjectContent pc = GetProjectContent(file);
				if (pc != null) {
					return this.typeResolutionService.LoadAssembly(pc);
				}
				return null;
			} else {
				return base.LoadAssembly(name);
			}
		}
		
		public override XamlTypeFinder Clone()
		{
			MyTypeFinder copy = new MyTypeFinder();
			copy.file = this.file;
			copy.ImportFrom(this);
			return copy;
		}
		
		internal static IProjectContent GetProjectContent(OpenedFile file)
		{
			if (ProjectService.OpenSolution != null && file != null) {
				IProject p = ProjectService.OpenSolution.FindProjectContainingFile(file.FileName);
				if (p != null) {
					return ParserService.GetProjectContent(p);
				}
			}
			return ParserService.DefaultProjectContent;
		}
	}
}
