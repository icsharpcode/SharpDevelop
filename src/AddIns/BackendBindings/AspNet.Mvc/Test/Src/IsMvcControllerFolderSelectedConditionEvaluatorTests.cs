// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class IsMvcControllerFolderSelectedConditionEvaluatorTests
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
