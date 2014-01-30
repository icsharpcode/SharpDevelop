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

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestProject : TestProjectBase
	{
		public const string MSpecAssemblyName = "Machine.Specifications";
		
		const string MSpecItFQName = MSpecAssemblyName + ".It";
		const string MSpecBehavesLikeFQName = MSpecAssemblyName + ".Behaves_like";
		const string MSpecBehaviorsAttributeFQName = MSpecAssemblyName + ".BehaviorsAttribute";
		
		public MSpecTestProject(ITestSolution parentSolution, IProject project)
			: base(project)
		{
		}
		
		public override void UpdateTestResult(TestResult result)
		{
			// Code duplication - taken from NUnitTestProject
			int lastDot = result.Name.LastIndexOf('.');
			if (lastDot < 0)
				return;
			
			string fixtureName = result.Name.Substring(0, lastDot);
			string memberName = result.Name.Substring(lastDot + 1);
			
			var testClass = GetMSpecTestClass(new FullTypeName(fixtureName)) as MSpecTestClass;
			MSpecTestMember test = testClass.FindTestMember(memberName);
			if (test != null)
				test.UpdateTestResult(result);
		}
		
		MSpecTestClass GetMSpecTestClass(FullTypeName fullTypeName)
		{
			return GetTestClass(fullTypeName.TopLevelTypeName) as MSpecTestClass;
		}
		
		protected override void UpdateTestClass(ITest test, ITypeDefinition typeDefinition)
		{
			var mspecTest = test as MSpecTestClass;
			mspecTest.Update(typeDefinition);
		}
		
		protected override bool IsTestClass(ITypeDefinition typeDefinition)
		{
			return HasSpecificationMembers(typeDefinition) && !HasBehaviorAttribute(typeDefinition);
		}
		
		public override IEnumerable<ITest> GetTestsForEntity(IEntity entity)
		{
			return new ITest[0];
		}
		
		public override ITestRunner CreateTestRunner(TestExecutionOptions options)
		{
			if (options.UseDebugger)
				return new MSpecTestDebugger();
			
			return new MSpecTestRunner(options);
		}
		
		protected override ITest CreateTestClass(ITypeDefinition typeDefinition)
		{
			if (IsTestClass(typeDefinition))
				return new MSpecTestClass(this, typeDefinition.FullTypeName);
			
			return null;
		}
		
		public bool IsTestMember(IMember member)
		{
			return member is IField && HasItReturnType(member as IField);
		}
		
		public IEnumerable<MSpecTestMember> GetTestMembersFor(ITypeDefinition typeDefinition)
		{
			return GetTestMembers(typeDefinition, typeDefinition.Fields);
		}

		IEnumerable<MSpecTestMember> GetTestMembers(ITypeDefinition testClass, IEnumerable<IField> fields)
		{
			List<MSpecTestMember> result = fields.Where(HasItReturnType).Select(field => new MSpecTestMember(this, field)).ToList();
			foreach (IField field in fields) {
				if (HasBehavesLikeReturnType(field)) {
					IEnumerable<IField> behaviorFields = ResolveBehaviorFieldsOf(field);
					IEnumerable<IField> behaviorMembers = behaviorFields.Where(HasItReturnType);
					IEnumerable<BehaviorImportedTestMember> testMembersFromBehavior = behaviorMembers.Select(testField => 
						new BehaviorImportedTestMember(this, field, testField));
					result.AddRange(testMembersFromBehavior);
				}
			}
			return result;
		}
		
		IEnumerable<IField> ResolveBehaviorFieldsOf(IField field)
		{
			IType fieldReturnType = field.ReturnType;
			if (fieldReturnType == null) return new List<IField>();
			if (fieldReturnType.TypeArguments.Count != 1)
				LoggingService.Error(string.Format("Expected behavior specification {0} to have one type argument but {1} found.", field.FullName, fieldReturnType.TypeArguments.Count));
			IType behaviorClassType = fieldReturnType.TypeArguments.FirstOrDefault();

			return behaviorClassType != null ? behaviorClassType.GetFields() : new List<IField>();
		}
		
		bool HasSpecificationMembers(ITypeDefinition typeDefinition)
		{
			return !typeDefinition.IsAbstract
				&& typeDefinition.Fields.Any(f => HasItReturnType(f) || HasBehavesLikeReturnType(f));
		}
		
		bool HasBehavesLikeReturnType(IField field)
		{
			return MSpecBehavesLikeFQName.Equals(field.ReturnType.FullName);
		}
		
		bool HasItReturnType(IField field)
		{
			return MSpecItFQName.Equals(field.ReturnType.FullName);
		}
		
		bool HasBehaviorAttribute(ITypeDefinition typeDefinition)
		{
			return typeDefinition.Attributes.Any(
				attribute => MSpecBehaviorsAttributeFQName.Equals(attribute.AttributeType.FullName));
		}
	}
}
