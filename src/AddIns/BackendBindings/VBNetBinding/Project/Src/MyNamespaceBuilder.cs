// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding
{
	public static class MyNamespaceBuilder
	{
		/// <summary>
		/// Builds Visual Basic's "My" namespace for the specified project.
		/// </summary>
		public static void BuildNamespace(VBNetProject project, IProjectContent pc)
		{
			if ("custom".Equals(project.GetEvaluatedProperty("MyType"), StringComparison.OrdinalIgnoreCase))
				return;
			
			ICompilationUnit cu = new DefaultCompilationUnit(pc);
			//cu.FileName = "GeneratedMyNamespace.vb"; // leave FileName null - fixes SD2-854
			string ns;
			if (project.RootNamespace == null || project.RootNamespace.Length == 0)
				ns = "My";
			else
				ns = project.RootNamespace + ".My";
			IClass myApp = CreateMyApplication(cu, project, ns);
			IClass myComp = CreateMyComputer(cu, ns);
			
			cu.Classes.Add(myApp);
			cu.Classes.Add(myComp);
			
			IClass myForms = null;
			if (project.OutputType == OutputType.WinExe) {
				myForms = CreateMyForms(cu, ns);
				cu.Classes.Add(myForms);
			}
			DefaultClass c = new DefaultClass(cu, ns + ".MyProject");
			c.ClassType = ClassType.Module;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Partial | ModifierEnum.Sealed | ModifierEnum.Synthetic;
			c.Attributes.Add(new DefaultAttribute(CreateTypeRef(cu, "Microsoft.VisualBasic.HideModuleNameAttribute")));
			
			// we need to use GetClassReturnType instead of DefaultReturnType because we need
			// a reference to the compound class.
			c.Properties.Add(new DefaultProperty("Application",
			                                     new GetClassReturnType(pc, myApp.FullyQualifiedName, 0),
			                                     ModifierEnum.Public | ModifierEnum.Static,
			                                     DomRegion.Empty, DomRegion.Empty, c));
			c.Properties.Add(new DefaultProperty("Computer",
			                                     new GetClassReturnType(pc, myComp.FullyQualifiedName, 0),
			                                     ModifierEnum.Public | ModifierEnum.Static,
			                                     DomRegion.Empty, DomRegion.Empty, c));
			if (myForms != null) {
				c.Properties.Add(new DefaultProperty("Forms",
				                                     new GetClassReturnType(pc, myForms.FullyQualifiedName, 0),
				                                     ModifierEnum.Public | ModifierEnum.Static,
				                                     DomRegion.Empty, DomRegion.Empty, c));
			}
			c.Properties.Add(new DefaultProperty("User",
			                                     new GetClassReturnType(pc, "Microsoft.VisualBasic.ApplicationServices.User", 0),
			                                     ModifierEnum.Public | ModifierEnum.Static,
			                                     DomRegion.Empty, DomRegion.Empty, c));
			cu.Classes.Add(c);
			pc.UpdateCompilationUnit(null, cu, cu.FileName);
		}
		
		static IClass CreateMyApplication(ICompilationUnit cu, VBNetProject project, string ns)
		{
			DefaultClass c = new DefaultClass(cu, ns + ".MyApplication");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Sealed | ModifierEnum.Partial | ModifierEnum.Synthetic;
			c.Attributes.Add(new DefaultAttribute(CreateTypeRef(cu, "Microsoft.VisualBasic.HideModuleNameAttribute")));
			switch (project.OutputType) {
				case OutputType.WinExe:
					c.BaseTypes.Add(CreateTypeRef(cu, "Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase"));
					if (project.GetEvaluatedProperty("MyType") == "WindowsForms") {
						c.Methods.Add(
							new DefaultMethod(c, "Main") {
								Modifiers = ModifierEnum.Internal | ModifierEnum.Static,
								ReturnType = c.ProjectContent.SystemTypes.Void,
								Parameters = new[] {
									new DefaultParameter(
										"args",
										new ArrayReturnType(c.ProjectContent, c.ProjectContent.SystemTypes.String, 1),
										DomRegion.Empty
									)
								}
							});
					}
					break;
				case OutputType.Exe:
					c.BaseTypes.Add(CreateTypeRef(cu, "Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase"));
					break;
				default:
					c.BaseTypes.Add(CreateTypeRef(cu, "Microsoft.VisualBasic.ApplicationServices.ApplicationBase"));
					break;
			}
			return c;
		}
		
		static IReturnType CreateTypeRef(ICompilationUnit cu, string fullName)
		{
			return new GetClassReturnType(cu.ProjectContent, fullName, 0);
		}
		
		static IClass CreateMyComputer(ICompilationUnit cu, string ns)
		{
			DefaultClass c = new DefaultClass(cu, ns + ".MyComputer");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Sealed | ModifierEnum.Partial | ModifierEnum.Synthetic;
			c.Attributes.Add(new DefaultAttribute(CreateTypeRef(cu, "Microsoft.VisualBasic.HideModuleNameAttribute")));
			c.BaseTypes.Add(CreateTypeRef(cu, "Microsoft.VisualBasic.Devices.Computer"));
			return c;
		}
		
		static IClass CreateMyForms(ICompilationUnit cu, string ns)
		{
			DefaultClass c = new MyFormsClass(cu, ns + ".MyForms");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Sealed | ModifierEnum.Synthetic;
			c.Attributes.Add(new DefaultAttribute(CreateTypeRef(cu, "Microsoft.VisualBasic.HideModuleNameAttribute")));
			return c;
		}
		
		class MyFormsClass : DefaultClass
		{
			public MyFormsClass(ICompilationUnit cu, string fullName) : base(cu, fullName) {}
			
			public override IList<IProperty> Properties {
				get {
					// TODO: This class must be immutable when frozen, we cannot change the properties!
					
					List<IProperty> properties = new List<IProperty>();
					IClass formClass = this.ProjectContent.GetClass("System.Windows.Forms.Form", 0);
					if (formClass == null)
						return properties;
					foreach (IClass c in this.ProjectContent.Classes) {
						if (c.IsTypeInInheritanceTree(formClass)) {
							properties.Add(new DefaultProperty(c.Name,
							                                   new GetClassReturnType(this.ProjectContent, c.FullyQualifiedName, 0),
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
