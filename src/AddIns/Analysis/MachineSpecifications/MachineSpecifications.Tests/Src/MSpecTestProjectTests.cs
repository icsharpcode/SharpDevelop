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

using developwithpassion.specifications.extensions;
using developwithpassion.specifications.rhinomocks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.UnitTesting;
using Machine.Specifications;
using Rhino.Mocks;

namespace ICSharpCode.MachineSpecifications.Tests
{
	public abstract class MSpecTestProjectFieldsConcern : Observes<TestableMSpecTestProject>
	{
		Establish ctx = () => {
		};
	
		protected const string MSpecItTypeName = "Machine.Specifications.It";
		protected const string MSpecBehavesTypeName = "Machine.Specifications.Behaves_like";
		protected const string MSpecBehaviorTypeName = "Machine.Specifications.BehaviorsAttribute";
	
		protected static ITypeDefinition SetupClass(bool isAbstract, IList<IField> fields, IList<IAttribute> attributes)
		{
			var c = fake.an<ITypeDefinition>();
			c.setup(x => x.IsAbstract).Return(isAbstract);
			c.setup(x => x.Fields).Return(fields);
			c.setup(x => x.Attributes).Return(attributes);
			return c;
		}
	
		protected static IField SetupField(string returnTypeName)
		{
			var field = fake.an<IField>();
			field.setup(f => f.ReturnType).Return(SetupReturnType(returnTypeName));
			return field;
		}
	
		protected static IAttribute SetupBehaviorAttribute()
		{
			var attribute = fake.an<IAttribute>();
			attribute.setup(x => x.AttributeType).Return(SetupReturnType(MSpecBehaviorTypeName));
			return attribute;
		}
	
		protected static IType SetupReturnType(string typeName)
		{
			var returnType = fake.an<IType>();
			returnType.Stub(x => x.FullName).Return(typeName);
			return returnType;
		}
	}
	
	[Subject(typeof(MSpecTestProject))]
	public class When_checking_if_is_a_test_class : MSpecTestProjectFieldsConcern
	{
		static ITypeDefinition classWithoutSpecificationMembers;
		static ITypeDefinition classWithSpecificationMembers;
		static ITypeDefinition classWithBehavior;
		static ITypeDefinition classWithSpecificationMembersAndBehaviorAttribute;

		static bool resultForClassWithBehaviorAttribute;
		static bool resultForClassWithSpecifications;
		static bool resultForClassWithBehavior;
		static bool resultForClassWithoutSpecifications;

		Establish ctx = () => {
			classWithoutSpecificationMembers = SetupClass(false, new IField[0], new IAttribute[0]);
			classWithSpecificationMembers = SetupClass(false, new IField[] { SetupField(MSpecItTypeName) }, new IAttribute[0]);
			classWithBehavior = SetupClass(false, new IField[] { SetupField(MSpecBehavesTypeName) }, new IAttribute[0]);
			classWithSpecificationMembersAndBehaviorAttribute = SetupClass(false, new IField[] { SetupField(MSpecItTypeName) }, new IAttribute[] { SetupBehaviorAttribute() });
		};

		Because of = () => {
			resultForClassWithoutSpecifications = sut.CallIsTestClass(classWithoutSpecificationMembers);
			resultForClassWithSpecifications = sut.CallIsTestClass(classWithSpecificationMembers);
			resultForClassWithBehavior = sut.CallIsTestClass(classWithBehavior);
			resultForClassWithBehaviorAttribute = sut.CallIsTestClass(classWithSpecificationMembersAndBehaviorAttribute);
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

	public class When_enumerating_test_members : MSpecTestProjectFieldsConcern
	{
		static ITypeDefinition behaviorClass;
		static IField testSpecificationInBehavior;

		static ITypeDefinition testClass;
		static IField testSpecification;
		static IField otherField;
		static IField behavesLikeField;

		static IEnumerable<MSpecTestMember> result;

		const string BehaviorClassName = "Test.Behavior";

		Establish ctx = () => {
			var itReturnType = SetupReturnType(MSpecItTypeName);
			
			testSpecificationInBehavior = fake.an<IField>();
			testSpecificationInBehavior.setup(x => x.ReturnType).Return(itReturnType);
			var behaviorClassFields = new IField[] { testSpecificationInBehavior };
			behaviorClass = SetupClass(false, behaviorClassFields, new IAttribute[0]);
			behaviorClass.Stub(b => b.GetFields()).Return(behaviorClassFields);

			testSpecification = fake.an<IField>();
			testSpecification.setup(f => f.FullName).Return("TestClass.testSpecificationInBehavior");
			testSpecification.setup(f => f.ReturnType).Return(itReturnType);
			otherField = fake.an<IField>();
			otherField.setup(f => f.ReturnType).Return(fake.an<IType>());

			var behavesLikeReturnType = SetupReturnType(MSpecBehavesTypeName);
			
			behavesLikeReturnType.setup(t => t.TypeArguments).Return(new List<IType> { behaviorClass });
			behavesLikeField = fake.an<IField>();
			behavesLikeField.setup(f => f.ReturnType).Return(behavesLikeReturnType);
			
			testClass = SetupClass(false, new IField[] { testSpecification, otherField, behavesLikeField }, new IAttribute[0]);
		};

		Because of = () => result = sut.GetTestMembersFor(testClass);

		It should_contain_field_with_it_return_type = () =>
			result.Select(m => m.Member).ShouldContain(testSpecification);

		It should_not_contain_field_with_arbitrary_return_type = () =>
			result.Select(m => m.Member).ShouldNotContain(otherField);

		It should_contain_imported_field_from_behavior = () =>
			result.Select(m => m.Member).ShouldContain(member => member.FullName == "TestClass.testSpecificationInBehavior");
	}
}
