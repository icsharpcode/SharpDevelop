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
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class IsMvcViewFolderSelectedConditionEvaluatorTests : MvcTestsBase
	{
		IsMvcViewFolderSelectedConditionEvaluator conditionEvaluator;
		
		void CreateConditionEvaluator()
		{
			conditionEvaluator = new IsMvcViewFolderSelectedConditionEvaluator();
		}
		
		bool IsValid(object owner)
		{
			CreateConditionEvaluator();
			return conditionEvaluator.IsValid(owner, null);
		}
		
		DirectoryNode CreateViewsFolderNode()
		{
			string path = @"d:\projects\MyAspNetProject\Views";
			return new DirectoryNode(path);
		}
		
		DirectoryNode CreateViewsFolderNodeWithUpperCaseName()
		{
			string path = @"d:\projects\MyAspNetProject\VIEWS";
			return new DirectoryNode(path);
		}
		
		ProjectNode CreateProjectNode()
		{
			var project = TestableProject.CreateProject();
			return new ProjectNode(project);
		}
		
		ProjectNode CreateProjectNode(string fileName, string projectName)
		{
			var project = TestableProject.CreateProject(fileName, projectName);
			return new ProjectNode(project);
		}
		
		DirectoryNode CreatePropertiesFolderNode()
		{
			string path = @"d:\projects\MyAspNetProject\Properties";
			return new DirectoryNode(path);
		}
		
		DirectoryNode CreateViewsChildFolderNode()
		{
			DirectoryNode viewsNode = CreateViewsFolderNode();
			
			string path = @"d:\projects\MyAspNetProject\Views\Child";
			var childNode = new DirectoryNode(path);
			childNode.AddTo(viewsNode);
			return childNode;
		}
		
		DirectoryNode CreatePropertiesFolderNodeWithParentProjectNodeCalledViews()
		{
			var projectNode = CreateProjectNode(@"d:\projects\MyProject\Views.csproj", "Views");
			var propertiesNode = new DirectoryNode(@"d:\projects\MyProject\Properties");
			propertiesNode.AddTo(projectNode);
			return propertiesNode;
		}
		
		[Test]
		public void IsValid_NullOwnerIsPassed_ReturnsFalse()
		{
			bool valid = IsValid(owner: null);
			
			Assert.IsFalse(valid);
		}
		
		[Test]
		public void IsValid_ViewsFolderNodePassed_ReturnsTrue()
		{
			DirectoryNode owner = CreateViewsFolderNode();
			bool valid = IsValid(owner);
			
			Assert.IsTrue(valid);
		}
		
		[Test]
		public void IsValid_ProjectNodePassed_ReturnsFalse()
		{
			ProjectNode owner = CreateProjectNode();
			bool valid = IsValid(owner);
			
			Assert.IsFalse(valid);
		}
		
		[Test]
		public void IsValid_PropertiesFolderNodePassed_ReturnsFalse()
		{
			DirectoryNode owner = CreatePropertiesFolderNode();
			bool valid = IsValid(owner);
			
			Assert.IsFalse(valid);
		}
		
		[Test]
		public void IsValid_ViewsChildFolderNodePassed_ReturnsTrue()
		{
			DirectoryNode owner = CreateViewsChildFolderNode();
			bool valid = IsValid(owner);
			
			Assert.IsTrue(valid);
		}
		
		[Test]
		public void IsValid_PropertiesFolderNodeWithParentProjectCalledViewsPassed_ReturnsFalse()
		{
			DirectoryNode owner = CreatePropertiesFolderNodeWithParentProjectNodeCalledViews();
			bool valid = IsValid(owner);
			
			Assert.IsFalse(valid);
		}
		
		[Test]
		public void IsValid_ViewsFolderNodeWithUpperCaseNamePassed_ReturnsTrue()
		{
			DirectoryNode owner = CreateViewsFolderNodeWithUpperCaseName();
			bool valid = IsValid(owner);
			
			Assert.IsTrue(valid);
		}
	}
}
