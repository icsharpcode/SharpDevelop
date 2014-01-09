// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;
using ICSharpCode.SharpDevelop.Tests.Utils;

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
				: base(new ProjectCreateInformation(MockSolution.Create(), FileName.Create("c:\\temp\\TestProject.csproj")))
			{
				this.languageName = languageName;
			}
			
			public override string Language {
				get { return languageName; }
			}
			
			public override bool IsReadOnly {
				get { return readOnly; }
			}
			
			protected override ProjectBehavior CreateDefaultBehavior()
			{
				return new DotNetStartBehavior(this, null);
			}
		}
		
		public static void InitializeProjectBindings()
		{
			SD.Services.AddStrictMockService<IProjectService>();
			var projectBinding = MockRepository.GenerateStrictMock<IProjectBinding>();
			
			ProjectBindingDescriptor[] descriptors = {
				new ProjectBindingDescriptor(projectBinding, "C#", ".csproj", ProjectTypeGuids.CSharp, new[] { ".cs" }),
				new ProjectBindingDescriptor(projectBinding, "VB", ".vbproj", ProjectTypeGuids.VB, new[] { ".vb" }),
			};
			SD.ProjectService.Stub(p => p.ProjectBindings).Return(descriptors);
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
