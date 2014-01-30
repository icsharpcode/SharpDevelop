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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class SelectedTestsTestFixture
	{
		SelectedTests selectedTests;
		MockCSharpProject project;
		string namespaceFilter = "MyNamespace.Tests";
		MockClass c;
		MockMethod method;
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
			c = new MockClass();
			method = new MockMethod(c);
			selectedTests = new SelectedTests(project, namespaceFilter, c, method);
		}
		
		[Test]
		public void ProjectPropertyMatchesProjectPassedToConstructor()
		{
			Assert.AreEqual(project, selectedTests.Project);
		}
		
		[Test]
		public void ClassPropertyMatchesClassPassedToConstructor()
		{
			Assert.AreEqual(c, selectedTests.Class);
		}
		
		[Test]
		public void MethodPropertyMatchesMethodPassedToConstructor()
		{
			Assert.AreEqual(method, selectedTests.Member);
		}
		
		[Test]
		public void ProjectsReturnsSingleItemContainingProjectPassedToConstructor()
		{
			var projects = selectedTests.Projects.ToArray();
			IProject[] expectedProjects = new IProject[] { project };
			
			Assert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void HasProjectsReturnsTrue()
		{
			Assert.IsTrue(selectedTests.HasProjects);
		}
		
		[Test]
		public void NamespaceFilterPropertyMatchesNamespaceFilterPassedToConstructor()
		{
			Assert.AreEqual(namespaceFilter, selectedTests.NamespaceFilter);
		}
		
		[Test]
		public void RemoveFirstProjectLeavesNoProjects()
		{
			selectedTests.RemoveFirstProject();
			Assert.AreEqual(0, selectedTests.ProjectsCount);
		}
		
		[Test]
		public void HasProjectReturnsFalseAfterRemoveFirstProjectCalled()
		{
			selectedTests.RemoveFirstProject();
			Assert.IsFalse(selectedTests.HasProjects);
		}
		
		[Test]
		public void RemovingFirstProjectTwiceDoesNotThrowException()
		{
			selectedTests.RemoveFirstProject();
			Assert.DoesNotThrow(delegate { selectedTests.RemoveFirstProject(); });
		}
	}
}
