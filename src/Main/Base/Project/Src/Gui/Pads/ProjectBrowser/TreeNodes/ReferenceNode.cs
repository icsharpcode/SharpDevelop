// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project.InnerExpand;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceNode : CustomFolderNode
	{
		ReferenceProjectItem referenceProjectItem;
		
		public ReferenceProjectItem ReferenceProjectItem {
			get {
				return referenceProjectItem;
			}
		}
		
		public ReferenceNode(ReferenceProjectItem referenceProjectItem)
		{
			this.referenceProjectItem = referenceProjectItem;
			Tag = referenceProjectItem;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceNode";
			SetIcon("Icons.16x16.Reference");
			Text = referenceProjectItem.ShortName;
			
			this.PerformInitialization();
			
			ParserService.LoadSolutionProjectsThreadEnded += delegate { ShowNamespaces(); };
		}
		
		Dictionary<string, List<TypeDefinition>> namespaces = new Dictionary<string, List<TypeDefinition>>();
		
		public void ShowNamespaces(bool forceRefresh = false)
		{
			if (!SharpDevelopUIOptions.ExpandReferences) 
				return;
			
			if (namespaces.Count > 0 && !forceRefresh)
				return;
			
			namespaces.Clear();
			Nodes.Clear();
			
			if (string.IsNullOrEmpty(referenceProjectItem.FileName)) return;
			if (!File.Exists(referenceProjectItem.FileName)) return;
			
			if (Path.GetExtension(referenceProjectItem.FileName) == ".dll" ||
			    Path.GetExtension(referenceProjectItem.FileName) == ".exe")
			{
				var asm = AssemblyDefinition.ReadAssembly(referenceProjectItem.FileName);
				foreach (var module in asm.Modules) {
					if (!module.HasTypes) continue;

					foreach (var type in module.Types) {
						
						if (string.IsNullOrEmpty(type.Namespace)) continue;
						
						if (!namespaces.ContainsKey(type.Namespace))
							namespaces.Add(type.Namespace, new List<TypeDefinition>());

						namespaces[type.Namespace].Add(type);
					}

					foreach (var ns in namespaces.Keys)
					{
						var nsNode = new NamespaceNode(ns, namespaces[ns]);
						nsNode.InsertSorted(this);
					}
				}
			}
			
			if (Path.GetExtension(referenceProjectItem.FileName).EndsWith("proj")) {
				// use parser service
			}
		}
		
		public override void Expanding()
		{
			foreach (var node in Nodes) {
				if (!(node is NamespaceNode)) continue;
				
				var n = (NamespaceNode)node;
				n.RefreshNodes();
			}
				
			base.Expanding();
		}
		
		public override void Refresh()
		{
			ShowNamespaces(true);
			base.Refresh();
		}
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			IProject project = Project;
			ProjectService.RemoveProjectItem(referenceProjectItem.Project, referenceProjectItem);
			Debug.Assert(Parent != null);
			Debug.Assert(Parent is ReferenceFolder);
			((ReferenceFolder)Parent).ShowReferences();
			project.Save();
		}
		#endregion
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
