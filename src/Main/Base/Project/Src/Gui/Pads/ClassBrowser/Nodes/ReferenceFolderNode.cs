// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class ReferenceFolderNode : ExtFolderNode
	{
		IProject project;
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public override bool Visible {
			get {
				ClassBrowserFilter filter = ClassBrowserPad.Instance.Filter;
				return (filter & ClassBrowserFilter.ShowProjectReferences) != 0;
			}
		}
		
		public ReferenceFolderNode(IProject project)
		{
			sortOrder = -1;
			
			this.project = project;
			Text = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ReferencesNodeText");
			
			OpenedIcon = "ProjectBrowser.ReferenceFolder.Open";
			ClosedIcon = "ProjectBrowser.ReferenceFolder.Closed";
			
			Nodes.Add(new TreeNode(ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode")));
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			UpdateReferenceNodes();
		}
		
		public void UpdateReferenceNodes()
		{
			Nodes.Clear();
			foreach (ProjectItem item in project.Items) {
				if (item.ItemType == ItemType.Reference) {
					new ReferenceNode((ReferenceProjectItem)item).AddTo(this);
				}
			}
		}
	}
	
	public class ReferenceNode : ProjectNode
	{
		ReferenceProjectItem item;
		
		public ReferenceNode(ReferenceProjectItem item)
		{
			this.item = item;
			Text = item.Name;
			
			SetIcon("Icons.16x16.Reference");
			
			Nodes.Add(new TreeNode(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode}")));
		}
		
		protected override void Initialize()
		{
			isInitialized = true;
			
			IProjectContent pc = AssemblyParserService.GetProjectContentForReference(item);
			if (pc != null) {
				Nodes.Clear();
				foreach (IClass c in pc.Classes) {
					TreeNode node = GetNodeByPath(c.Namespace, true);
					new ClassNode(item.Project, c).AddTo(node);
				}
			}
		}
		
		protected override string StripRootNamespace(string directory)
		{
			string rootNamespace = item.Include;
			int pos = rootNamespace.IndexOf(',');
			if (pos > 0)
				rootNamespace = rootNamespace.Substring(0, pos);
			if (directory.ToLowerInvariant().StartsWith(rootNamespace.ToLowerInvariant())) {
				directory = directory.Substring(rootNamespace.Length);
			}
			return directory;
		}
	}
}
