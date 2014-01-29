// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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