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

		Establish ctx = () =>
		{
			testProject = fake.an<IProject>();
			var mspecReference = MockRepository.GenerateStub<ReferenceProjectItem>(testProject);
			mspecReference.setup(x => x.ShortName).Return(MSpecAssemblyName);
			testProject.setup(x => x.Items).Return(new ReadOnlyCollection<ProjectItem>(new[] { mspecReference }));

			nonTestProject = fake.an<IProject>();
			var otherReference = MockRepository.GenerateStub<ReferenceProjectItem>(nonTestProject);
			mspecReference.setup(x => x.ShortName).Return("System.Configuration");
			nonTestProject.setup(x => x.Items).Return(new ReadOnlyCollection<ProjectItem>(new[] { otherReference }));
		};

		Because of = () =>
		{
			resultForTestProject = sut.IsTestProject(testProject);
			resultForNonTestProject = sut.IsTestProject(nonTestProject);
		};

		It should_return_true_for_project_which_has_reference_to_test_framework = () =>
			resultForTestProject.ShouldBeTrue();

		It should_return_false_for_project_which_has_no_reference_to_test_framework = () =>
			resultForNonTestProject.ShouldBeFalse();
	}

	public abstract class MSpecTestFrameworkFieldsConcern : Observes<MSpecTestFramework>
	{
		protected static ICompilationUnit CompilationUnit;

		Establish ctx = () =>
		{
			var ProjectContent = fake.an<IProjectContent>();
			ProjectContent.setup(x => x.SystemTypes).Return(new SystemTypes(ProjectContent));
			CompilationUnit = new DefaultCompilationUnit(ProjectContent);
		};

		protected const string MSpecItTypeName = "Machine.Specifications.It";
		protected const string MSpecBehavesTypeName = "Machine.Specifications.Behaves_like";
		protected const string MSpecBehaviorTypeName = "Machine.Specifications.BehaviorsAttribute";

		protected static IClass SetupClass(bool isAbstract, IList<IField> fields, IList<IAttribute> attributes)
		{
			var c = fake.an<IClass>();
			c.setup(x => x.IsAbstract).Return(isAbstract);
			c.setup(x => x.Fields).Return(fields);
			c.setup(x => x.Attributes).Return(attributes);
			return c;
		}

		protected static IField SetupField(string returnTypeName)
		{
			var field = fake.an<IField>();
			field.ReturnType = SetupReturnType(returnTypeName);
			return field;
		}

		protected static IAttribute SetupBehaviorAttribute()
		{
			var attribute = fake.an<IAttribute>();
			attribute.setup(x => x.AttributeType).Return(SetupReturnType(MSpecBehaviorTypeName));
			return attribute;
		}

		protected static IReturnType SetupReturnType(string typeName)
		{
			var returnType = fake.an<IReturnType>();
			returnType.Stub(x => x.FullyQualifiedName).Return(typeName);
			return returnType;
		}
	}

	[Subject(typeof(MSpecTestFramework))]
	public class When_checking_if_is_a_test_class : MSpecTestFrameworkFieldsConcern
	{
		static IClass classWithoutSpecificationMembers;
		static IClass classWithSpecificationMembers;
		static IClass classWithBehavior;
		static IClass classWithSpecificationMembersAndBehaviorAttribute;

		static bool resultForClassWithBehaviorAttribute;
		static bool resultForClassWithSpecifications;
		static bool resultForClassWithBehavior;
		static bool resultForClassWithoutSpecifications;

		Establish ctx = () =>
		{
			classWithoutSpecificationMembers = SetupClass(false, new IField[0], new IAttribute[0]);
			classWithSpecificationMembers = SetupClass(false, new IField[] { SetupField(MSpecItTypeName) }, new IAttribute[0]);
			classWithBehavior = SetupClass(false, new IField[] { SetupField(MSpecBehavesTypeName) }, new IAttribute[0]);
			classWithSpecificationMembersAndBehaviorAttribute = SetupClass(false, new IField[] { SetupField(MSpecItTypeName) }, new IAttribute[] { SetupBehaviorAttribute() });
		};

		Because of = () =>
		{
			resultForClassWithoutSpecifications = sut.IsTestClass(classWithoutSpecificationMembers);
			resultForClassWithSpecifications = sut.IsTestClass(classWithSpecificationMembers);
			resultForClassWithBehavior = sut.IsTestClass(classWithBehavior);
			resultForClassWithBehaviorAttribute = sut.IsTestClass(classWithSpecificationMembersAndBehaviorAttribute);
		};

		It should_return_false_for_class_without_specification_members = () =>
			resultForClassWithoutSpecifications.ShouldBeFalse();

		It should_return_true_for_class_with_specification_members = () =>
			resultForClassWithSpecifications.ShouldBeTrue();

		It should_return_true_for_class_with_behavior = () =>
			resultForClassWithBehavior.ShouldBeTrue();

		It should_return_false_for_class_with_behavior_attribute = () =>
			resultForClassWithBehaviorAttribute.ShouldBeFalse();
	}

	public class When_enumerating_test_members : MSpecTestFrameworkFieldsConcern
	{
		static IClass behaviorClass;
		static IField testSpecificationInBehavior;

		static IClass testClass;
		static IField testSpecification;
		static IField otherField;
		static IField behavesLikeField;

		static IEnumerable<TestMember> result;

		const string BehaviorClassName = "Test.Behavior";

		Establish ctx = () =>
		{
			var itReturnType = SetupReturnType(MSpecItTypeName);
			
			behaviorClass = new DefaultClass(CompilationUnit, "BehaviorClass");
			testSpecificationInBehavior = new DefaultField(itReturnType, "testSpecificationInBehavior", ModifierEnum.None, DomRegion.Empty, behaviorClass);
			behaviorClass.Fields.Add(testSpecificationInBehavior);

			testClass = new DefaultClass(CompilationUnit, "TestClass");
			testSpecification = new DefaultField(itReturnType, "testSpecification", ModifierEnum.None, DomRegion.Empty, testClass);
			testClass.Fields.Add(testSpecification);
			otherField = new DefaultField(fake.an<IReturnType>(), "OtherField", ModifierEnum.None, DomRegion.Empty, testClass);
			testClass.Fields.Add(otherField);

			var behavesLikeReturnType = new ConstructedReturnType(SetupReturnType(MSpecBehavesTypeName), new List<IReturnType>{new DefaultReturnType(behaviorClass)});
			behavesLikeField = new DefaultField(behavesLikeReturnType, "behavesLikeField", ModifierEnum.None, new DomRegion(), testClass);
			testClass.Fields.Add(behavesLikeField);
		};

		Because of = () => result = sut.GetTestMembersFor(testClass);

		It should_contain_field_with_it_return_type = () =>
			result.Select(m => m.Member).ShouldContain(testSpecification);

		It should_not_contain_field_with_arbitrary_return_type = () =>
			result.Select(m => m.Member).ShouldNotContain(otherField);

		It should_contain_imported_field_from_behavior = () =>
			result.Select(m => m.Member).ShouldContain(member => member.FullyQualifiedName == "TestClass.testSpecificationInBehavior");
	}
}