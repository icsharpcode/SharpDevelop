// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	/// <summary>
	/// Helper methods used when testing web references
	/// </summary>
	public static class WebReferenceTestHelper
	{
		public static MSBuildBasedProject CreateTestProject(string languageName)
		{
			return new TestProject(languageName);
		}
		
		class TestProject : CompilableProject
		{
			string languageName;
			bool readOnly = false;
			
			public TestProject(string languageName)
				: base(new Solution())
			{
				this.languageName = languageName;
			}
			
			public override string Language {
				get { return languageName; }
			}
			
			public override bool ReadOnly {
				get { return readOnly; }
			}
			
			public override ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
				get { return ICSharpCode.SharpDevelop.Dom.LanguageProperties.CSharp; }
			}
		}
		
		public static void InitializeLanguageBindings()
		{
			Properties prop = new Properties();
			prop["id"] = "C#";
			prop["supportedextensions"] = ".cs";
			prop["projectfileextension"] = ".csproj";
			Codon codon1 = new Codon(null, "LanguageBinding", prop, new Condition[0]);
			prop = new Properties();
			prop["id"] = "VBNet";
			prop["supportedextensions"] = ".vb";
			prop["projectfileextension"] = ".vbproj";
			Codon codon2 = new Codon(null, "LanguageBinding", prop, new Condition[0]);
			LanguageBindingService.SetBindings(new LanguageBindingDescriptor[] {
			                                   	new LanguageBindingDescriptor(codon1),
			                                   	new LanguageBindingDescriptor(codon2)
			                                   });
		}
		
		public static ProjectItem GetProjectItem(List<ProjectItem> items, string include, ItemType itemType) {
			foreach (ProjectItem item in items) {
				if (item.ItemType == itemType) {
					if (item.Include == include) {
						return item;
					}
				}
			}
			return null;
		}
		
		public static FileProjectItem GetFileProjectItem(List<ProjectItem> items, string include, ItemType itemType) {
			foreach (ProjectItem item in items) {
				if (item.ItemType == itemType) {
					if (item.Include == include) {
						return (FileProjectItem)item;
					}
				}
			}
			return null;
		}
		
		public static ProjectItem GetProjectItem(List<ProjectItem> items, ItemType itemType)
		{
			foreach (ProjectItem item in items) {
				if (item.ItemType == itemType) {
					return item;
				}
			}
			return null;
		}
		
		public static WebReferencesFolderNode GetWebReferencesFolderNode(ProjectNode projectNode)
		{
			foreach (AbstractProjectBrowserTreeNode node in projectNode.Nodes) {
				if (node is WebReferencesFolderNode) {
					return (WebReferencesFolderNode)node;
				}
			}
			return null;
		}
		
		public static WebReferenceNode GetWebReferenceNode(WebReferencesFolderNode webReferencesFolderNode) {
			foreach (AbstractProjectBrowserTreeNode node in webReferencesFolderNode.Nodes) {
				if (node is WebReferenceNode) {
					return (WebReferenceNode)node;
				}
			}
			return null;
		}
		
		public static FileNode GetFileNode(AbstractProjectBrowserTreeNode parent, string fileName)
		{
			foreach (AbstractProjectBrowserTreeNode node in parent.Nodes) {
				FileNode fileNode = node as FileNode;
				if (fileNode != null && fileNode.FileName == fileName) {
					return fileNode;
				}
			}
			return null;
		}
	}
}
