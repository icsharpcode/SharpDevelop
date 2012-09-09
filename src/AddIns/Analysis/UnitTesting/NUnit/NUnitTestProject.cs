// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test project.
	/// </summary>
	public class NUnitTestProject : TestProjectBase
	{
		public NUnitTestProject(IProject project) : base(project)
		{
		}
		
		public override ITestRunner CreateTestRunner(TestExecutionOptions options)
		{
			if (options.UseDebugger)
				return new NUnitTestDebugger();
			else
				return new NUnitTestRunner();
		}
		
		public override void UpdateTestClass(ITest test, ITypeDefinition typeDefinition)
		{
			((NUnitTestClass)test).UpdateTestClass(typeDefinition);
		}
		
		public override bool IsTestClass(ITypeDefinition typeDefinition)
		{
			return NUnitTestFramework.IsTestClass(typeDefinition);
		}
		
		public override ITest GetTestForEntity(IEntity entity)
		{
			if (entity.DeclaringTypeDefinition != null) {
				ITest testClass = GetTestForEntity(entity.DeclaringTypeDefinition);
				throw new NotImplementedException();
			} else if (entity is ITypeDefinition) {
				// top-level type definition
				ITypeDefinition typeDef = (ITypeDefinition)entity;
				return GetTestClass(new FullNameAndTypeParameterCount(typeDef.Namespace, typeDef.Name, typeDef.TypeParameterCount));
			} else {
				return null;
			}
		}
		
		public override ITest CreateTestClass(ITypeDefinition typeDefinition)
		{
			if (NUnitTestFramework.IsTestClass(typeDefinition))
				return new NUnitTestClass(this, typeDefinition);
			else
				return null;
		}
		
		public override void UpdateTestResult(TestResult result)
		{
			int pos = result.Name.LastIndexOf('.');
			if (pos < 0)
				return;
			string fixtureName = result.Name.Substring(0, pos);
			string methodName = result.Name.Substring(pos + 1);
			var testClass = FindTestClass(fixtureName);
			if (testClass != null) {
				var testMethod = testClass.NestedTests.OfType<NUnitTestMethod>().FirstOrDefault(m => m.Name == methodName);
				if (testMethod != null) {
					testMethod.UpdateTestResult(result);
				}
			}
		}
		
		NUnitTestClass FindTestClass(string fixtureName)
		{
			ITypeReference r = ReflectionHelper.ParseReflectionName(fixtureName);
			return FindTestClass(r);
		}
		
		NUnitTestClass FindTestClass(ITypeReference r)
		{
			var gctr = r as GetClassTypeReference;
			if (gctr != null) {
				return (NUnitTestClass)GetTestClass(new FullNameAndTypeParameterCount(gctr.Namespace, gctr.Name, gctr.TypeParameterCount));
			}
			var ntc = r as NestedTypeReference;
			if (ntc != null) {
				
			}
			return null;
		}
	}
}
