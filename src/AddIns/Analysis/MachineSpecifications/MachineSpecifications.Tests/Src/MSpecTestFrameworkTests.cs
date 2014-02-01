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
using System.Collections.ObjectModel;
using System.Linq;
using developwithpassion.specifications.extensions;
using developwithpassion.specifications.dsl;
using developwithpassion.specifications.rhinomocks;
using ICSharpCode.SharpDevelop.Project;
using Machine.Specifications;
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace ICSharpCode.MachineSpecifications.Tests
{
	[Subject(typeof(MSpecTestFramework))]
	public class When_checking_if_is_a_test_project : Observes<MSpecTestFramework>
	{
		static IProject testProject;
		static IProject nonTestProject;

		static bool resultForTestProject;
		static bool resultForNonTestProject;

		const string MSpecAssemblyName = "Machine.Specifications";

		Establish ctx = () => {
			testProject = fake.an<IProject>();
			var mspecReference = MockRepository.GenerateStub<ReferenceProjectItem>(testProject);
			mspecReference.setup(x => x.ShortName).Return(MSpecAssemblyName);
			testProject.setup(x => x.Items).Return(new SimpleModelCollection<ProjectItem>(new[] { mspecReference }));

			nonTestProject = fake.an<IProject>();
			var otherReference = MockRepository.GenerateStub<ReferenceProjectItem>(nonTestProject);
			mspecReference.setup(x => x.ShortName).Return("System.Configuration");
			nonTestProject.setup(x => x.Items).Return(new SimpleModelCollection<ProjectItem>(new[] { otherReference }));
		};

		Because of = () => {
			resultForTestProject = sut.IsTestProject(testProject);
			resultForNonTestProject = sut.IsTestProject(nonTestProject);
		};

		It should_return_true_for_project_which_has_reference_to_test_framework = () =>
			resultForTestProject.ShouldBeTrue();

		It should_return_false_for_project_which_has_no_reference_to_test_framework = () =>
			resultForNonTestProject.ShouldBeFalse();
	}
}
