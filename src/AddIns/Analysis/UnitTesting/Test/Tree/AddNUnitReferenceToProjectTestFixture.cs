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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class AddNUnitReferenceToProjectTestFixture
	{
		MockCSharpProject project;
		ReferenceProjectItem referenceProjectItem;
		AddNUnitReferenceCommand command = new AddNUnitReferenceCommand();
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
			
			command.Run(project);
			
			if (project.Items.Count > 0) {
				referenceProjectItem = project.Items[0] as ReferenceProjectItem;
			}
		}
		
		[Test]
		public void ProjectHasNUnitFrameworkReferenceAdded()
		{
			Assert.AreEqual("nunit.framework", referenceProjectItem.Name);
		}
		
		[Test]
		public void NUnitReferenceProjectPropertyEqualsProjectPassedToRunMethod()
		{
			Assert.AreEqual(project, referenceProjectItem.Project);
		}
		
		[Test]
		public void ProjectIsSaved()
		{
			Assert.IsTrue(project.IsSaved);
		}
		
		[Test]
		public void NoExceptionThrownWhenProjectIsNull()
		{
			Assert.DoesNotThrow(delegate { command.Run(null); });
		}
	}
}
