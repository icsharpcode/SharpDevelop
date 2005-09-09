// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace VBNetBinding
{
	public static class MyNamespaceBuilder
	{
		/// <summary>
		/// Builds Visual Basic's "My" namespace for the specified project.
		/// </summary>
		public static void BuildNamespace(IProject project, IProjectContent pc)
		{
			ICompilationUnit cu = new DefaultCompilationUnit(pc);
			cu.FileName = "GeneratedMyNamespace.vb";
			string ns;
			if (project.RootNamespace == null || project.RootNamespace.Length == 0)
				ns = "My";
			else
				ns = project.RootNamespace + ".My";
			IClass myApp = CreateMyApplication(cu, project, ns);
			IClass myComp = CreateMyComputer(cu, project, ns);
			
			cu.Classes.Add(myApp);
			cu.Classes.Add(myComp);
			
			IClass myForms = null;
			if (project.OutputType == OutputType.WinExe) {
				myForms = CreateMyForms(cu, project, ns);
				cu.Classes.Add(myForms);
			}
			DefaultClass c = new DefaultClass(cu, ns + ".MyProject");
			c.ClassType = ClassType.Module;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Partial | ModifierEnum.Sealed;
			c.Attributes.Add(new DefaultAttribute("Microsoft.VisualBasic.HideModuleNameAttribute"));
			
			// we need to use GetClassReturnType instead of DefaultReturnType because we need
			// a reference to the compound class.
			c.Properties.Add(new DefaultProperty("Application",
			                                     new GetClassReturnType(pc, myApp.FullyQualifiedName),
			                                     ModifierEnum.Public | ModifierEnum.Static,
			                                     DomRegion.Empty, DomRegion.Empty, c));
			c.Properties.Add(new DefaultProperty("Computer",
			                                     new GetClassReturnType(pc, myComp.FullyQualifiedName),
			                                     ModifierEnum.Public | ModifierEnum.Static,
			                                     DomRegion.Empty, DomRegion.Empty, c));
			if (myForms != null) {
				c.Properties.Add(new DefaultProperty("Forms",
				                                     new GetClassReturnType(pc, myForms.FullyQualifiedName),
				                                     ModifierEnum.Public | ModifierEnum.Static,
				                                     DomRegion.Empty, DomRegion.Empty, c));
			}
			c.Properties.Add(new DefaultProperty("User",
			                                     new GetClassReturnType(pc, "Microsoft.VisualBasic.ApplicationServices.User"),
			                                     ModifierEnum.Public | ModifierEnum.Static,
			                                     DomRegion.Empty, DomRegion.Empty, c));
			cu.Classes.Add(c);
			pc.UpdateCompilationUnit(null, cu, cu.FileName, false);
		}
		
		static IClass CreateMyApplication(ICompilationUnit cu, IProject project, string ns)
		{
			DefaultClass c = new DefaultClass(cu, ns + ".MyApplication");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Sealed | ModifierEnum.Partial;
			c.Attributes.Add(new DefaultAttribute("Microsoft.VisualBasic.HideModuleNameAttribute"));
			switch (project.OutputType) {
				case OutputType.WinExe:
					c.BaseTypes.Add(CreateBaseType(cu, "Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase"));
					break;
				case OutputType.Exe:
					c.BaseTypes.Add(CreateBaseType(cu, "Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase"));
					break;
				default:
					c.BaseTypes.Add(CreateBaseType(cu, "Microsoft.VisualBasic.ApplicationServices.ApplicationBase"));
					break;
			}
			return c;
		}
		
		static IReturnType CreateBaseType(ICompilationUnit cu, string fullName)
		{
			return new GetClassReturnType(cu.ProjectContent, fullName);
		}
		
		static IClass CreateMyComputer(ICompilationUnit cu, IProject project, string ns)
		{
			DefaultClass c = new DefaultClass(cu, ns + ".MyComputer");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Sealed | ModifierEnum.Partial;
			c.Attributes.Add(new DefaultAttribute("Microsoft.VisualBasic.HideModuleNameAttribute"));
			c.BaseTypes.Add(CreateBaseType(cu, "Microsoft.VisualBasic.Devices.Computer"));
			return c;
		}
		
		static IClass CreateMyForms(ICompilationUnit cu, IProject project, string ns)
		{
			DefaultClass c = new MyFormsClass(cu, ns + ".MyForms");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Sealed; //| ModifierEnum.Partial;
			c.Attributes.Add(new DefaultAttribute("Microsoft.VisualBasic.HideModuleNameAttribute"));
			return c;
		}
		
		class MyFormsClass : DefaultClass
		{
			public MyFormsClass(ICompilationUnit cu, string fullName) : base(cu, fullName) {}
			
			public override List<IProperty> Properties {
				get {
					List<IProperty> properties = new List<IProperty>();
					IProjectContent pc = ProjectContentRegistry.GetExistingProjectContent(new System.Reflection.AssemblyName("System.Windows.Forms"));
					if (pc == null)
						return properties;
					IClass formClass = pc.GetClass("System.Windows.Forms.Form");
					if (formClass == null)
						return properties;
					foreach (IClass c in this.ProjectContent.Classes) {
						if (c.BaseClass == formClass) {
							properties.Add(new DefaultProperty(c.Name,
							                                   new GetClassReturnType(this.ProjectContent, c.FullyQualifiedName),
							                                   ModifierEnum.Public | ModifierEnum.Static,
							                                   DomRegion.Empty, DomRegion.Empty, c));
						}
					}
					return properties;
				}
			}
		}
	}
}
