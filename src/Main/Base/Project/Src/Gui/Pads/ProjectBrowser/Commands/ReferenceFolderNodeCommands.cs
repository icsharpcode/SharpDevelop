// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.Services.Discovery;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using ICSharpCode.SharpDevelop.Parser;

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
				if (selDialog.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
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
					DiscoveryClientProtocol protocol = DiscoverWebServices(url.UpdateFromURL);
					if (protocol != null) {
						// Save web services.
						WebReference webReference = new WebReference(url.Project, url.UpdateFromURL, node.Text, url.Namespace, protocol);
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
								fileNode.InsertSorted(node);
							}
							ProjectBrowserPad.Instance.ProjectBrowserControl.TreeView.Sort();
							url.Project.Save();
						}
						
						// Update code completion.
						SD.ParserService.ParseFileAsync(FileName.Create(webReference.WebProxyFileName), parentProject: url.Project).FireAndForget();
					}
				} catch (WebException ex) {
					LoggingService.Debug(ex);
					MessageService.ShowError(GetRefreshWebReferenceErrorMessage(ex, url.UpdateFromURL));
				}
			}
		}
		
		string GetRefreshWebReferenceErrorMessage(WebException ex, string updateFromURL)
		{
			return String.Format(
				"{0}\r\n\r\n{1}\r\n{2}",
				String.Format(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.RefreshWebReference.ReadServiceDescriptionError}"), updateFromURL),
				ex.Message,
				GetInnerExceptionMessage(ex));
		}
		
		string GetInnerExceptionMessage(WebException ex)
		{
			if (ex.InnerException != null) {
				return ex.InnerException.Message;
			}
			return String.Empty;
		}
		
		DiscoveryClientProtocol DiscoverWebServices(string url)
		{
			WebServiceDiscoveryClientProtocol protocol = new WebServiceDiscoveryClientProtocol();
			NetworkCredential credential = CredentialCache.DefaultNetworkCredentials;
			bool retry = true;
			while (retry) {
				try {
					protocol.Credentials = credential;
					protocol.DiscoverAny(url);
					protocol.ResolveOneLevel();
					return protocol;
				} catch (WebException ex) {
					if (protocol.IsAuthenticationRequired) {
						using (UserCredentialsDialog dialog = new UserCredentialsDialog(url, protocol.GetAuthenticationHeader().AuthenticationType)) {
							if (dialog.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
								credential = dialog.Credential;
							} else {
								retry = false;
							}
						}
					} else {
						throw ex;
					}
				}
			}
			return null;
		}
	}
	
	public class AddWebReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node != null && node.Project != null) {
				using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(node.Project)) {
					refDialog.NamespacePrefix = node.Project.RootNamespace;
					if (refDialog.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
						// Do not overwrite existing web references.
						refDialog.WebReference.Name = WebReference.GetReferenceName(refDialog.WebReference.WebReferencesDirectory, refDialog.WebReference.Name);
						refDialog.WebReference.Save();
						
						foreach (ProjectItem item in refDialog.WebReference.Items) {
							ProjectService.AddProjectItem(node.Project, item);
						}
						
						AddWebReferenceToProjectBrowser(node, refDialog.WebReference);
						
						// Add proxy to code completion.
						SD.ParserService.ParseFileAsync(FileName.Create(refDialog.WebReference.WebProxyFileName)).FireAndForget();

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
				LoggingService.Warn("AddWebReferenceToProjectBrowser: Selected node type is not handled.");
				AddWebReferenceToProjectBrowser(node.Parent as AbstractProjectBrowserTreeNode, webReference);
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
	
	public class AddServiceReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			var node = Owner as AbstractProjectBrowserTreeNode;
			IProject project = (node != null) ? node.Project : ProjectService.CurrentProject;
			if (project == null) {
				return;
			}
			
			var vm = new AddServiceReferenceViewModel(project);
			var dialog = new AddServiceReferenceDialog();
			dialog.DataContext = vm;
			dialog.Owner = SD.Workbench.MainWindow;
			if (dialog.ShowDialog() ?? true) {
				vm.AddServiceReference();
			}
		}
	}
	
	public class RefreshReference : AbstractMenuCommand
	{
		public override void Run()
		{
			var node = Owner as ReferenceNode;
			if (node != null) {
				ReferenceProjectItem item = node.ReferenceProjectItem;
				if (item != null) {
					SD.AssemblyParserService.RefreshAssembly(item.FileName);
				}
			}
		}
	}
}
