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
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.MachineSpecifications.Tests
{
	[Subject(typeof(MSpecTestFramework))]
    public class When_checking_if_is_a_test_project : Observes<MSpecTestFramework>
    {
        static IProject testProject;
        static IProject nonTestProject;
        
        static bool resultForTestProject;
		static bool resultForNonTestProject;
		
		Establish ctx;
		
		Because of = () => {
            resultForTestProject = sut.IsTestProject(testProject);
            resultForNonTestProject = sut.IsTestProject(nonTestProject);
		};

        It should_return_true_for_project_which_has_reference_to_test_framework = () =>
            resultForTestProject.ShouldBeTrue();

        It should_return_false_for_project_which_has_no_reference_to_test_framework = () =>
            resultForNonTestProject.ShouldBeFalse();
	}

    [Subject(typeof(MSpecTestFramework))]
    public class When_checking_if_is_a_test_class : Observes<MSpecTestFramework>
    {
        static IClass classWithoutSpecificationMembers;
        static IClass classWithSpecificationMembers;
        static IClass classWithSpecificationMembersAndBehaviorAttribute;

        static bool resultForClassWithBehaviorAttribute;
        static bool resultForClassWithSpecifications;
        static bool resultForClassWithoutSpecifications;

        Establish ctx;

        Because of = () =>
        {
            resultForClassWithoutSpecifications = sut.IsTestClass(classWithoutSpecificationMembers);
            resultForClassWithSpecifications = sut.IsTestClass(classWithSpecificationMembers);
            resultForClassWithBehaviorAttribute = sut.IsTestClass(classWithSpecificationMembersAndBehaviorAttribute);
        };

        It should_return_false_for_class_without_specification_members = () =>
            resultForClassWithoutSpecifications.ShouldBeFalse();

        It should_return_true_for_class_with_specification_members = () =>
            resultForClassWithSpecifications.ShouldBeTrue();

        It should_return_false_for_class_with_behavior_attribute = () =>
            resultForClassWithBehaviorAttribute.ShouldBeFalse();
    }
}