// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// class browser.
	/// </summary>
	public class BaseTypesNode : ExtFolderNode
	{
		IProject project;
		IClass   c;
		
		public override bool Visible {
			get {
				ClassBrowserFilter filter = ClassBrowserPad.Instance.Filter;
				return (filter & ClassBrowserFilter.ShowBaseAndDerivedTypes) != 0;
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		
		public BaseTypesNode(IProject project, IClass c)
		{
			sortOrder = 0;
			this.project = project;
			this.c       = c;
			Text         = ResourceService.GetString("MainWindow.Windows.ClassBrowser.BaseTypes");
			
			OpenedIcon = "ProjectBrowser.Folder.Open";
			ClosedIcon = "ProjectBrowser.Folder.Closed";
			
			Nodes.Add(new TreeNode(ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode")));
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Nodes.Clear();
			
			IProjectContent content = c.ProjectContent;
			if (content != null) {
				int count = c.BaseTypes.Count;
				for (int i = 0; i < count; i++) {
					IReturnType baseType = c.GetBaseType(i);
					IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
					if (baseClass != null) {
						new ClassNode(project, baseClass).AddTo(this);
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
