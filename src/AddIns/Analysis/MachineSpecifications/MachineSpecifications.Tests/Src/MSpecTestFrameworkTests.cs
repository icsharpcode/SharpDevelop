/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 15:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using developwithpassion.specifications.rhinomocks;
using ICSharpCode.SharpDevelop.Project;
using Machine.Specifications;

namespace ICSharpCode.MachineSpecifications.Tests
{
	public abstract class MSpecTestFrameworkConcern : Observes<MSpecTestFramework> {
		protected static void EstablishContext() {
			
		}
		
		protected static IProject testProject;
		protected static IProject nonTestProject;
	}
	
	[Subject(typeof(MSpecTestFramework))]
	public class When_checking_if_is_a_test_project : MSpecTestFrameworkConcern {
		static bool resultForTestProject;
		static bool resultForNonTestProject;
		
		Establish ctx = EstablishContext;
		
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