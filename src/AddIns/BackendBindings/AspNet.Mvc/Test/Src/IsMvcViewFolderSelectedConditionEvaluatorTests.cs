// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class IsMvcViewFolderSelectedConditionEvaluatorTests
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
			var node = new DirectoryNode("Views");
			return node;
		}
		
		ProjectNode CreateProjectNode()
		{
			var project = TestableProject.CreateProject();
			return new ProjectNode(project);
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
	}
}