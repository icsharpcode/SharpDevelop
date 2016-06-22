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

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test fixture.
	/// </summary>
	public class NUnitTestClass : TestBase
	{
		readonly NUnitTestProject parentProject;
		FullTypeName fullTypeName;
		List<FullTypeName> baseClassNames = new List<FullTypeName>();
		
		public NUnitTestClass(NUnitTestProject parentProject, FullTypeName fullTypeName)
		{
			if (parentProject == null)
				throw new ArgumentNullException("parentProject");
			this.parentProject = parentProject;
			this.fullTypeName = fullTypeName;
			BindResultToCompositeResultOfNestedTests();
			// No need to call UpdateTestClass() here as NestedTestsInitialized still is false
		}
		
		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return fullTypeName.Name; }
		}
		
		public FullTypeName FullTypeName {
			get { return fullTypeName; }
		}
		
		public string ClassName {
			get { return fullTypeName.Name; }
		}
		
		public int TypeParameterCount {
			get { return fullTypeName.TypeParameterCount; }
		}
		
		public string ReflectionName {
			get { return fullTypeName.ReflectionName; }
		}
		
		public override System.Windows.Input.ICommand GoToDefinition {
			get {
				return new RelayCommand(
					delegate {
						ITypeDefinition typeDefinition = Resolve();
						if (typeDefinition != null)
							NavigationService.NavigateTo(typeDefinition);
					});
			}
		}
		
		public ITypeDefinition Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			IType type = compilation.MainAssembly.GetTypeDefinition(fullTypeName);
			return type.GetDefinition();
		}
		
		public ITypeDefinition Resolve(ISolutionSnapshotWithProjectMapping solutionSnapshot)
		{
			ICompilation compilation = solutionSnapshot.GetCompilation(parentProject.Project);
			IType type = compilation.MainAssembly.GetTypeDefinition(fullTypeName);
			return type.GetDefinition();
		}
		
		public NUnitTestClass FindNestedTestClass(string name, int typeParameterCount)
		{
			foreach (var t in this.NestedTests.OfType<NUnitTestClass>()) {
				if (t.ClassName == name && t.TypeParameterCount == typeParameterCount)
					return t;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the test method with the specified <see cref="NUnitTestMethod.MethodNameWithDeclaringTypeForInheritedTests"/>.
		/// For inherited tests, this is a string in the form '<c>DeclaringTypeName.MethodName</c>'
		/// </summary>
		public NUnitTestMethod FindTestMethod(string name)
		{
			foreach (var t in this.NestedTests.OfType<NUnitTestMethod>()) {
				if (t.MethodNameWithDeclaringTypeForInheritedTests == name)
					return t;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the test method with the specified name.
		/// This will return inherited tests assuming they are
		/// not hidden by tests in the derived class.
		/// </summary>
		public NUnitTestMethod FindTestMethodWithShortName(string name)
		{
			// Use last match because base class tests come first
			return this.NestedTestCollection.OfType<NUnitTestMethod>().LastOrDefault(method => method.MethodName == name);
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
			fullTypeName = typeDefinition.FullTypeName;
			if (this.NestedTestsInitialized) {
				int baseClassIndex = 0;
				foreach (IType baseType in typeDefinition.GetNonInterfaceBaseTypes()) {
					ITypeDefinition baseTypeDef = baseType.GetDefinition();
					// Check that the base type isn't equal to System.Object or the current class itself
					if (baseTypeDef == null || baseTypeDef == typeDefinition || baseTypeDef.KnownTypeCode == KnownTypeCode.Object)
						continue;
					if (baseTypeDef.DeclaringTypeDefinition != null)
						continue; // we only support inheriting from top-level classes
					var baseClassName = baseTypeDef.FullTypeName;
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
						nestedTestClass = new NUnitTestClass(parentProject, nestedClass.FullTypeName);
						this.NestedTestCollection.Add(nestedTestClass);
					}
					newOrUpdatedNestedTests.Add(nestedTestClass);
				}
				// Get methods (not operators etc.)
				foreach (IMethod method in typeDefinition.GetMethods(m => m.SymbolKind == SymbolKind.Method)) {
					if (!NUnitTestFramework.IsTestMethod(method))
						continue;
					
					IUnresolvedMethod unresolvedMethod = (IUnresolvedMethod)method.UnresolvedMember;
					string methodNameWithDeclaringType;
					FullTypeName derivedFixture;
					if (method.DeclaringTypeDefinition == typeDefinition) {
						derivedFixture = default(FullTypeName); // method is not inherited
						methodNameWithDeclaringType = method.Name;
					} else {
						if (method.DeclaringTypeDefinition == null)
							continue;
						derivedFixture = fullTypeName; // method is inherited
						methodNameWithDeclaringType = method.DeclaringTypeDefinition.Name + "." + method.Name;
					}
					
					NUnitTestMethod testMethod;
					if (method.IsOverride) {
						testMethod = FindTestMethodWithShortName(method.Name);
					} else {
						testMethod = FindTestMethod(methodNameWithDeclaringType);
					}
					if (testMethod != null) {
						testMethod.UpdateTestMethod(unresolvedMethod, derivedFixture);
					} else {
						testMethod = new NUnitTestMethod(parentProject, unresolvedMethod, derivedFixture);
						this.NestedTestCollection.Add(testMethod);
					}
					newOrUpdatedNestedTests.Add(testMethod);
				}
				// Remove all tests that weren't contained in the new type definition anymore:
				this.NestedTestCollection.RemoveAll(t => !newOrUpdatedNestedTests.Contains(t));
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
