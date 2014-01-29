// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestProject : TestProjectBase
	{
		public MSTestProject(IProject project)
			: base(project)
		{
		}
		
		public override ITestRunner CreateTestRunner(TestExecutionOptions options)
		{
			if (options.UseDebugger) {
				return new MSTestDebugger(options);
			}
			return new MSTestRunner(options);
		}
		
		protected override bool IsTestClass(ITypeDefinition typeDefinition)
		{
			return MSTestClass.IsTestClass(typeDefinition);
		}
		
		protected override ITest CreateTestClass(ITypeDefinition typeDefinition)
		{
			if (IsTestClass(typeDefinition)) {
				return new MSTestClass(this, typeDefinition.FullTypeName);
			}
			return null;
		}
		
		protected override void UpdateTestClass(ITest test, ITypeDefinition typeDefinition)
		{
			var testClass = test as MSTestClass;
			testClass.Update(typeDefinition);
		}
		
		public override IEnumerable<ITest> GetTestsForEntity(IEntity entity)
		{
			return new ITest[0];
		}
		
		public override void UpdateTestResult(TestResult result)
		{
			// Code duplication - taken from NUnitTestProject
			int lastDot = result.Name.LastIndexOf('.');
			if (lastDot < 0) {
				return;
			}
			
			string fixtureName = result.Name.Substring(0, lastDot);
			string memberName = result.Name.Substring(lastDot + 1);
			
			MSTestClass testClass = GetMSTestClass(new FullTypeName(fixtureName));
			MSTestMember test = testClass.FindTestMember(memberName);
			if (test != null) {
				test.UpdateTestResult(result);
			}
		}
		
		MSTestClass GetMSTestClass(FullTypeName fullTypeName)
		{
			return GetTestClass(fullTypeName.TopLevelTypeName) as MSTestClass;
		}
		
		bool IsTestMethod(IMethod method)
		{
			foreach (IAttribute attribute in method.Attributes) {
				if (IsMSTestMethodAttribute(attribute)) {
					return true;
				}
			}
			
			return false;
		}
		
		bool IsMSTestMethodAttribute(IAttribute attribute)
		{
			return IsMSTestMethodAttribute(attribute.AttributeType.FullName);
		}
		
		bool IsMSTestMethodAttribute(string name)
		{
			return
				name == "TestMethod" ||
				name == "TestMethodAttribute" ||
				name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute";
		}
		
		public IEnumerable<MSTestMember> GetTestMembersFor(ITypeDefinition typeDefinition)
		{
			return typeDefinition.Methods
				.Where(IsTestMethod)
				.Select(method => new MSTestMember(this, method));
		}
	}
}
