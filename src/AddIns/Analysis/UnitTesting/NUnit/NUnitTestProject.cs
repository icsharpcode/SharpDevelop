// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
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
		
		protected override bool IsTestClass(ITypeDefinition typeDefinition)
		{
			return NUnitTestFramework.IsTestClass(typeDefinition);
		}
		
		protected override ITest CreateTestClass(ITypeDefinition typeDefinition)
		{
			if (NUnitTestFramework.IsTestClass(typeDefinition))
				return new NUnitTestClass(this, typeDefinition);
			else
				return null;
		}
		
		protected override void UpdateTestClass(ITest test, ITypeDefinition typeDefinition)
		{
			((NUnitTestClass)test).UpdateTestClass(typeDefinition);
		}
		
		protected override void OnTestClassRemoved(ITest test)
		{
			((NUnitTestClass)test).OnRemoved();
			base.OnTestClassRemoved(test);
		}
		
		public override IEnumerable<ITest> GetTestsForEntity(IEntity entity)
		{
			ITypeDefinition typeDefinition = entity as ITypeDefinition ?? entity.DeclaringTypeDefinition;
			IEnumerable<ITest> tests;
			if (typeDefinition.IsAbstract) {
				tests = from c in entity.ParentAssembly.GetAllTypeDefinitions()
					where c.IsDerivedFrom(typeDefinition)
					select GetTestForEntityInClass(FindTestClass(c.ToTypeReference()), entity);
			} else {
				NUnitTestClass c = FindTestClass(typeDefinition.ToTypeReference());
				tests = new [] { GetTestForEntityInClass(c, entity) };
			}
			// GetTestForEntityInClass might return null, so filter those out:
			return tests.Where(t => t != null);
		}
		
		ITest GetTestForEntityInClass(NUnitTestClass c, IEntity entity)
		{
			if (c == null)
				return null;
			if (entity.EntityType == EntityType.TypeDefinition)
				return c;
			else if (entity.EntityType == EntityType.Method)
				return c.FindTestMethod(entity.Name);
			else
				return null;
		}
		
		public override void UpdateTestResult(TestResult result)
		{
			int lastDot = result.Name.LastIndexOf('.');
			if (lastDot < 0)
				return;
			string fixtureName = result.Name.Substring(0, lastDot);
			string methodName = result.Name.Substring(lastDot + 1);
			NUnitTestClass testClass = FindTestClass(fixtureName);
			if (testClass == null) {
				// maybe it's an inherited test
				int secondToLastDot = result.Name.LastIndexOf('.', lastDot - 1);
				if (secondToLastDot >= 0) {
					string fixtureName2 = result.Name.Substring(0, secondToLastDot);
					testClass = FindTestClass(fixtureName2);
				}
			}
			if (testClass != null) {
				NUnitTestMethod testMethod = testClass.FindTestMethod(methodName);
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
				NUnitTestClass declaringTestClass = FindTestClass(ntc.DeclaringTypeReference);
				if (declaringTestClass != null)
					return declaringTestClass.FindNestedTestClass(ntc.Name, declaringTestClass.TypeParameterCount + ntc.AdditionalTypeParameterCount);
			}
			return null;
		}
		
		#region Test Inheritance
		MultiDictionary<FullNameAndTypeParameterCount, NUnitTestClass> inheritedTestClasses = new MultiDictionary<FullNameAndTypeParameterCount, NUnitTestClass>();
		
		public void RegisterInheritedClass(FullNameAndTypeParameterCount baseClassName, NUnitTestClass inheritedClass)
		{
			inheritedTestClasses.Add(baseClassName, inheritedClass);
		}
		
		public void RemoveInheritedClass(FullNameAndTypeParameterCount baseClassName, NUnitTestClass inheritedClass)
		{
			inheritedTestClasses.Remove(baseClassName, inheritedClass);
		}
		
		protected override void AddToDirtyList(FullNameAndTypeParameterCount className)
		{
			// When a base class is invalidated, also invalidate all derived classes
			base.AddToDirtyList(className);
			foreach (var derivedClass in inheritedTestClasses[className]) {
				base.AddToDirtyList(derivedClass.TopLevelClassName);
			}
		}
		#endregion
	}
}
