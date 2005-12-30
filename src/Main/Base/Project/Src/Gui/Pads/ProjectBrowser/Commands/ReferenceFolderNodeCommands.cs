// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Web.Services.Discovery;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = Owner as AbstractProjectBrowserTreeNode;
			IProject project = (node != null) ? node.Project : ProjectService.CurrentProject;
			if (project == null) {
				return;
			}
			LoggingService.Info("Show add reference dialog for " + project.FileName);
			using (SelectReferenceDialog selDialog = new SelectReferenceDialog(project)) {
				if (selDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (ReferenceProjectItem reference in selDialog.ReferenceInformations) {
						ProjectService.AddProjectItem(project, reference);
					}
					project.Save();
				}
			}
		}
	}
	
	public class RefreshWebReference : AbstractMenuCommand
	{
		public override void Run()
		{
			WebReferenceNode node = Owner as WebReferenceNode;
			if (node != null && node.Project != null && node.ProjectItem != null) {
				WebReferenceUrl url = (WebReferenceUrl)node.ProjectItem;
				try {
					// Discover web services at url.
					DiscoveryClientProtocol protocol = new DiscoveryClientProtocol();
					protocol.DiscoverAny(url.UpdateFromURL);
					protocol.ResolveOneLevel();
					
					// Save web services.
					WebReference webReference = new WebReference(url.Project, url.UpdateFromURL, node.Text, url.Project.RootNamespace, protocol);
					webReference.Save();
					
					// Update project.
					WebReferenceChanges changes = webReference.GetChanges(url.Project);
					if (changes.Changed) {
						foreach (ProjectItem itemRemoved in changes.ItemsRemoved) {
							ProjectService.RemoveProjectItem(url.Project, itemRemoved);
							FileService.RemoveFile(itemRemoved.FileName, false);
						}
						foreach (ProjectItem newItem in changes.NewItems) {
							ProjectService.AddProjectItem(url.Project, newItem);
							FileNode fileNode = new FileNode(newItem.FileName, FileNodeStatus.InProject);
							fileNode.AddTo(node);
						}
						ProjectBrowserPad.Instance.ProjectBrowserControl.TreeView.Sort();
						url.Project.Save();
					}
					
					// Update code completion.
					ParserService.ParseFile(webReference.WebProxyFileName);
				} catch (WebException ex) {
					MessageService.ShowError(ex, String.Format(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.RefreshWebReference.ReadServiceDescriptionError}"), url.UpdateFromURL));
				}
			}
		}
	}
	
	public class AddWebReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = Owner as AbstractProjectBrowserTreeNode;
			if (node != null && node.Project != null) {
				using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(node.Project)) {
					refDialog.NamespacePrefix = node.Project.RootNamespace;
					if (refDialog.ShowDialog() == DialogResult.OK) {
						// Do not overwrite existing web references.
						refDialog.WebReference.Name = WebReference.GetReferenceName(refDialog.WebReference.WebReferencesDirectory, refDialog.WebReference.Name);
						refDialog.WebReference.Save();
					
						foreach (ProjectItem item in refDialog.WebReference.Items) {
							ProjectService.AddProjectItem(node.Project, item);
						}
						
						AddWebReferenceToProjectBrowser(node, refDialog.WebReference);
						
						// Add proxy to code completion.
						ParserService.ParseFile(refDialog.WebReference.WebProxyFileName);

						node.Project.Save();
					}
				}		
			}
		}
		
		void AddWebReferenceToProjectBrowser(AbstractProjectBrowserTreeNode node, WebReference webReference)
		{
			TreeNode webReferencesNode = null;
			if (node is ProjectNode) {
				webReferencesNode = AddWebReferenceToProjectNode((ProjectNode)node, webReference);
			} else if (node is WebReferencesFolderNode) {
				webReferencesNode = node;
				WebReferenceNodeBuilder.AddWebReference((WebReferencesFolderNode)webReferencesNode, webReference);
			} else if (node is ReferenceFolder && node.Parent != null && node.Parent is ProjectNode) {
				webReferencesNode = AddWebReferenceToProjectNode((ProjectNode)node.Parent, webReference);
			} else {
				LoggingService.Error("AddWebReferenceToProjectBrowser: Selected node type is not handled.");
			}
			
			if (webReferencesNode != null) {
				ProjectBrowserPad.Instance.ProjectBrowserControl.TreeView.Sort();
				webReferencesNode.Expand();
				webReferencesNode.EnsureVisible();
			}
		}
		
		TreeNode GetWebReferencesFolderNode(ProjectNode projectNode)
		{
			foreach (TreeNode node in projectNode.Nodes) {
				WebReferencesFolderNode webReferencesNode = node as WebReferencesFolderNode;
				if (webReferencesNode != null) {
					return webReferencesNode;
				}
			}
			return null;
		}
		
		TreeNode AddWebReferenceToProjectNode(ProjectNode node, WebReference webReference)
		{
			TreeNode webReferencesNode = WebReferenceNodeBuilder.AddWebReferencesFolderNode(node, webReference);
			if (webReferencesNode == null) {
				webReferencesNode = GetWebReferencesFolderNode((ProjectNode)node);
				if (webReferencesNode != null) {
					WebReferenceNodeBuilder.AddWebReference((WebReferencesFolderNode)webReferencesNode, webReference);
				}
			}
			return webReferencesNode;
		}
	}
}
