// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
				ClassBrowserFilter filter = ClassBrowser.Instance.Filter;
				return (filter & ClassBrowserFilter.ShowProjectReferences) != 0;
			}
		}
		
		public ReferenceFolderNode(IProject project)
		{
			sortOrder = 0;
			
			this.project = project;
			Text = "References";
			
			OpenedIcon = "ProjectBrowser.ReferenceFolder.Open";
			ClosedIcon = "ProjectBrowser.ReferenceFolder.Closed";
			
			Nodes.Add(new TreeNode(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode}")));
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Nodes.Clear();
			foreach (ProjectItem item in project.Items) {
				if (item.ItemType == ItemType.Reference) {
					new ReferenceNode(item).AddTo(this);
				}
			}
		}
	}
	
	public class ReferenceNode : ProjectNode
	{
		ProjectItem item;
		
		public ReferenceNode(ProjectItem item)
		{
			this.item = item;
			Text = item.Include;
			
			SetIcon("Icons.16x16.Reference");
			
			Nodes.Add(new TreeNode(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode}")));
		}
		
		protected override void Initialize()
		{
			isInitialized = true;
			
			Assembly assembly = null;
			try {
				assembly = Assembly.ReflectionOnlyLoadFrom(item.FileName);
			} catch (Exception) {
				try {
					assembly = Assembly.LoadWithPartialName(item.Include);
				} catch (Exception e) {
					Console.WriteLine("Can't load assembly '{0}' : " + e.Message, item.Include);
				}
			}
			if (assembly != null) {
				Nodes.Clear();
				foreach (Type type in assembly.GetTypes()) {
					if (!type.FullName.StartsWith("<") && type.IsPublic) {
						IClass c = new ReflectionClass(type);
						TreeNode node = GetNodeByPath(c.Namespace, true);
						new ClassNode(item.Project, c).AddTo(node);
					}
				}
			}
			
		}
	}
}
