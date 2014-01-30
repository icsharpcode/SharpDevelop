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
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class IsMvcControllerFolderSelectedConditionEvaluatorTests : MvcTestsBase
	{
		IsMvcControllerFolderSelectedConditionEvaluator conditionEvaluator;
		
		void CreateConditionEvaluator()
		{
			conditionEvaluator = new IsMvcControllerFolderSelectedConditionEvaluator();
		}
		
		bool IsValid(object owner)
		{
			CreateConditionEvaluator();
			return conditionEvaluator.IsValid(owner, null);
		}
		
		DirectoryNode CreateControllersFolderNode()
		{
			string path = @"d:\projects\MyAspNetProject\Controllers";
			return new DirectoryNode(path);
		}
		
		DirectoryNode CreateControllersFolderNodeWithUpperCaseName()
		{
			string path = @"d:\projects\MyAspNetProject\CONTROLLERS";
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
		
		DirectoryNode CreateControllersChildFolderNode()
		{
			DirectoryNode controllersNode = CreateControllersFolderNode();
			
			string path = @"d:\projects\MyAspNetProject\Controllers\Child";
			var childNode = new DirectoryNode(path);
			childNode.AddTo(controllersNode);
			return childNode;
		}
		
		DirectoryNode CreatePropertiesFolderNodeWithParentProjectNodeCalledControllers()
		{
			var projectNode = CreateProjectNode(@"d:\projects\MyProject\Controllers.csproj", "Controllers");
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
		public void IsValid_ControllersFolderNodePassed_ReturnsTrue()
		{
			DirectoryNode owner = CreateControllersFolderNode();
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
		public void IsValid_ControllersChildFolderNodePassed_ReturnsTrue()
		{
			DirectoryNode owner = CreateControllersChildFolderNode();
			bool valid = IsValid(owner);
			
			Assert.IsTrue(valid);
		}
		
		[Test]
		public void IsValid_PropertiesFolderNodeWithParentProjectCalledControllersPassed_ReturnsFalse()
		{
			DirectoryNode owner = CreatePropertiesFolderNodeWithParentProjectNodeCalledControllers();
			bool valid = IsValid(owner);
			
			Assert.IsFalse(valid);
		}
		
		[Test]
		public void IsValid_ControllersFolderNodeWithUpperCaseNamePassed_ReturnsTrue()
		{
			DirectoryNode owner = CreateControllersFolderNodeWithUpperCaseName();
			bool valid = IsValid(owner);
			
			Assert.IsTrue(valid);
		}
	}
}
