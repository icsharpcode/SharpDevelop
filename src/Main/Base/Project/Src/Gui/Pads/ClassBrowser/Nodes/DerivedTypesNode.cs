// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// class browser.
	/// </summary>
	public class DerivedTypesNode : ExtFolderNode
	{
		IProject project;
		IClass   c;
		
		public override bool Visible {
			get {
				ClassBrowserFilter filter = ClassBrowser.Instance.Filter;
				return (filter & ClassBrowserFilter.ShowBaseAndDerivedTypes) != 0;
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public DerivedTypesNode(IProject project, IClass c)
		{
			sortOrder = 1;
			
			this.project = project;
			this.c       = c;
			Text         = "Derived types";
			
			OpenedIcon = "ProjectBrowser.Folder.Open";
			ClosedIcon = "ProjectBrowser.Folder.Closed";
			
			Nodes.Add(new TreeNode(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode}")));
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Nodes.Clear();
			
			List<IProjectContent> contentList = new List<IProjectContent>(1);
			contentList.Add(null);
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						contentList[0] = projectContent;
						foreach (IClass derivedClass in RefactoringService.FindDerivedClasses(c, contentList)) {
							new ClassNode(project, derivedClass).AddTo(this);
						}
					}
				}
			}
			
			if (Nodes.Count == 0) {
				SetIcon(ClosedIcon);
				OpenedIcon = ClosedIcon = null;
			}
		}
	}
}
