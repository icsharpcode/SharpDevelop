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
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.RubyLanguage
{
	/// <summary>
	/// Tests the RubyProjectBinding class.
	/// </summary>
	[TestFixture]
	public class ProjetBindingTestFixture
	{
		RubyProjectBinding projectBinding;
		RubyProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			projectBinding = new RubyProjectBinding();
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.ProjectName = "Ruby";
			createInfo.OutputProjectFileName = @"C:\Projects\Ruby.rbproj";
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
			project = projectBinding.CreateProject(createInfo) as RubyProject;
		}
		
		[Test]
		public void Language()
		{
			Assert.AreEqual("Ruby", projectBinding.Language);
		}
		
		[Test]
		public void IsRubyProject()
		{
			Assert.IsNotNull(project);
		}

		[Test]
		public void ProjectName()
		{
			Assert.AreEqual("Ruby", project.Name);
		}
	}
}
