// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test fixture.
	/// </summary>
	public class NUnitTestClass : TestBase
	{
		readonly NUnitTestProject parentProject;
		IUnresolvedTypeDefinition primaryPart;
		List<FullNameAndTypeParameterCount> baseClassNames = new List<FullNameAndTypeParameterCount>();
		
		public NUnitTestClass(NUnitTestProject parentProject, ITypeDefinition typeDefinition)
		{
			if (parentProject == null)
				throw new ArgumentNullException("parentProject");
			this.parentProject = parentProject;
			UpdateTestClass(typeDefinition);
			BindResultToCompositeResultOfNestedTests();
		}
		
		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return primaryPart.Name; }
		}
		
		/// <summary>
		/// For top-level classes: returns the full name of the class.
		/// For nested classes: returns the full name of the top-level class that contains this test class.
		/// </summary>
		public FullNameAndTypeParameterCount TopLevelClassName {
			get {
				IUnresolvedTypeDefinition top = primaryPart;
				while (top.DeclaringTypeDefinition != null)
					top = top.DeclaringTypeDefinition;
				return new FullNameAndTypeParameterCount(top.Namespace, top.Name, top.TypeParameters.Count);
			}
		}
		
		public string ClassName {
			get { return primaryPart.Name; }
		}
		
		public int TypeParameterCount {
			get { return primaryPart.TypeParameters.Count; }
		}
		
		public string ReflectionName {
			get { return primaryPart.ReflectionName; }
		}
		
		public override bool SupportsGoToDefinition {
			get { return true; }
		}
		
		ITypeDefinition Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			IType type = primaryPart.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
			return type.GetDefinition();
		}
		
		public override void GoToDefinition()
		{
			ITypeDefinition typeDefinition = Resolve();
			if (typeDefinition != null)
				NavigationService.NavigateTo(typeDefinition);
		}
		
		public NUnitTestClass FindNestedTestClass(string name, int typeParameterCount)
		{
			foreach (var t in this.NestedTests.OfType<NUnitTestClass>()) {
				if (t.ClassName == name && t.TypeParameterCount == typeParameterCount)
					return t;
			}
			return null;
		}
		
		public NUnitTestMethod FindTestMethod(string name)
		{
			foreach (var t in this.NestedTests.OfType<NUnitTestMethod>()) {
				if (t.MethodName == name)
					return t;
			}
			return null;
		}
		
		/// <summary>
		/// Notifies the test class that it was removed from the project.
		/// </summary>
		internal void OnRemoved()
		{
			RemoveBaseClasses(0);
		}
		
		void RemoveBaseClasses(int startIndex)
		{
			for (int i = startIndex; i < baseClassNames.Count; i++) {
				parentProject.RemoveInheritedClass(baseClassNames[i], this);
			}
			baseClassNames.RemoveRange(startIndex, baseClassNames.Count - startIndex);
		}
		
		public void UpdateTestClass(ITypeDefinition typeDefinition)
		{
			primaryPart = typeDefinition.Parts[0];
			if (this.NestedTestsInitialized) {
				int baseClassIndex = 0;
				foreach (IType baseType in typeDefinition.GetNonInterfaceBaseTypes()) {
					ITypeDefinition baseTypeDef = baseType.GetDefinition();
					// Check that the base type isn't equal to System.Object or the current class itself
					if (baseTypeDef == null || baseTypeDef == typeDefinition || baseTypeDef.KnownTypeCode == KnownTypeCode.Object)
						continue;
					if (baseTypeDef.DeclaringTypeDefinition != null)
						continue; // we only support inheriting from top-level classes
					var baseClassName = new FullNameAndTypeParameterCount(baseTypeDef.Namespace, baseTypeDef.Name, baseTypeDef.TypeParameterCount);
					if (baseClassIndex < baseClassNames.Count && baseClassName == baseClassNames[baseClassIndex]) {
						// base class is already in the list, just keep it
						baseClassIndex++;
					} else {
						// base class is not in the list, or the remaining portion of the list differs
						// remove remaining portion of the list:
						RemoveBaseClasses(baseClassIndex);
						// Add new base class:
						parentProject.RegisterInheritedClass(baseClassName, this);
						baseClassNames.Add(baseClassName);
						baseClassIndex++;
					}
				}
				
				HashSet<ITest> newOrUpdatedNestedTests = new HashSet<ITest>();
				// Update nested test classes:
				foreach (ITypeDefinition nestedClass in typeDefinition.NestedTypes) {
					if (!NUnitTestFramework.IsTestClass(nestedClass))
						continue;
					
					NUnitTestClass nestedTestClass = FindNestedTestClass(nestedClass.Name, nestedClass.TypeParameterCount);
					if (nestedTestClass != null) {
						nestedTestClass.UpdateTestClass(nestedClass);
					} else {
						nestedTestClass = new NUnitTestClass(parentProject, nestedClass);
						this.NestedTests.Add(nestedTestClass);
					}
					newOrUpdatedNestedTests.Add(nestedTestClass);
				}
				// Get methods (not operators etc.)
				foreach (IMethod method in typeDefinition.GetMethods(m => m.EntityType == EntityType.Method)) {
					if (!NUnitTestFramework.IsTestMethod(method))
						continue;
					
					IUnresolvedMethod unresolvedMethod = (IUnresolvedMethod)method.UnresolvedMember;
					IUnresolvedTypeDefinition derivedFixture;
					if (method.DeclaringTypeDefinition == typeDefinition)
						derivedFixture = null; // method is not inherited
					else
						derivedFixture = primaryPart; // method is inherited
					
					NUnitTestMethod testMethod = FindTestMethod(method.Name);
					if (testMethod != null) {
						testMethod.UpdateTestMethod(unresolvedMethod, derivedFixture);
					} else {
						testMethod = new NUnitTestMethod(parentProject, unresolvedMethod, derivedFixture);
						this.NestedTests.Add(testMethod);
					}
					newOrUpdatedNestedTests.Add(testMethod);
				}
				// Remove all tests that weren't contained in the new type definition anymore:
				this.NestedTests.RemoveAll(t => !newOrUpdatedNestedTests.Contains(t));
			}
		}
		
		public override bool CanExpandNestedTests {
			get { return true; }
		}
		
		protected override void OnNestedTestsInitialized()
		{
			ITypeDefinition typeDefinition = Resolve();
			if (typeDefinition != null) {
				UpdateTestClass(typeDefinition);
			}
			base.OnNestedTestsInitialized();
		}
	}
}
