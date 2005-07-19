/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.07.2005
 * Time: 18:01
 */

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
		public static void BuildNamespace(IProject project, IProjectContent pc)
		{
			ICompilationUnit cu = new DefaultCompilationUnit(pc);
			const string ns = "My";
			IClass myApp = CreateMyApplication(cu, project, ns);
			IClass myComp = CreateMyComputer(cu, project, ns);
			cu.Classes.Add(myApp);
			cu.Classes.Add(myComp);
			DefaultClass c = new DefaultClass(cu, ns + ".MyProject");
			c.ClassType = ClassType.Module;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Partial | ModifierEnum.Sealed;
			c.Properties.Add(new DefaultProperty("Application",
			                                     myApp.DefaultReturnType,
			                                     ModifierEnum.Internal | ModifierEnum.Static,
			                                     null, null, c));
			c.Properties.Add(new DefaultProperty("Computer",
			                                     myComp.DefaultReturnType,
			                                     ModifierEnum.Internal | ModifierEnum.Static,
			                                     null, null, c));
			c.Properties.Add(new DefaultProperty("User",
			                                     new GetClassReturnType(pc, "Microsoft.VisualBasic.ApplicationServices.User"),
			                                     ModifierEnum.Internal | ModifierEnum.Static,
			                                     null, null, c));
			cu.Classes.Add(c);
			pc.UpdateCompilationUnit(null, cu, "GeneratedMyNamespace.vb", false);
		}
		
		static IClass CreateMyApplication(ICompilationUnit cu, IProject project, string ns)
		{
			DefaultClass c = new DefaultClass(cu, ns + ".MyApplication");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Partial | ModifierEnum.Sealed;
			switch (project.OutputType) {
				case OutputType.WinExe:
					c.BaseTypes.Add("Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase");
					break;
				case OutputType.Exe:
					c.BaseTypes.Add("Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase");
					break;
				default:
					c.BaseTypes.Add("Microsoft.VisualBasic.ApplicationServices.ApplicationBase");
					break;
			}
			return c;
		}
		
		static IClass CreateMyComputer(ICompilationUnit cu, IProject project, string ns)
		{
			DefaultClass c = new DefaultClass(cu, ns + ".MyComputer");
			c.ClassType = ClassType.Class;
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Partial | ModifierEnum.Sealed;
			c.BaseTypes.Add("Microsoft.VisualBasic.Devices.Computer");
			return c;
		}
	}
}
