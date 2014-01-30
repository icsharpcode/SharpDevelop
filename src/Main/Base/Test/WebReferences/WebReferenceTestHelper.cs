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
