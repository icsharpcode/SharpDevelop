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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Base class for <see cref="ITestProject"/> implementations.
	/// 
	/// This implementation will show a tree of namespaces, with each namespace
	/// containing a list of test fixtures (ITests created from type definitions).
	/// </summary>
	public abstract class TestProjectBase : TestBase, ITestProject
	{
		IProject project;
		Dictionary<TopLevelTypeName, ITest> topLevelTestClasses = new Dictionary<TopLevelTypeName, ITest>();
		
		protected TestProjectBase(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			BindResultToCompositeResultOfNestedTests();
		}
		
		public abstract ITestRunner CreateTestRunner(TestExecutionOptions options);
		public abstract IEnumerable<ITest> GetTestsForEntity(IEntity entity);
		public abstract void UpdateTestResult(TestResult result);
		
		// Test class management methods
		protected abstract bool IsTestClass(ITypeDefinition typeDefinition);
		protected abstract ITest CreateTestClass(ITypeDefinition typeDefinition);
		protected abstract void UpdateTestClass(ITest test, ITypeDefinition typeDefinition);
		protected virtual void OnTestClassRemoved(ITest test) {}
		
		public IProject Project {
			get { return project; }
		}
		
		public override ITestProject ParentProject {
			get { return this; }
		}
		
		public override string DisplayName {
			get { return project.Name; }
		}
		
		public virtual bool IsBuildNeededBeforeTestRun {
			get { return true; }
		}
		
		public override ImmutableStack<ITest> FindPathToDescendant(ITest test)
		{
			if (test == null || test.ParentProject != this)
				return null;
			return base.FindPathToDescendant(test);
		}
		
		#region NotifyParseInformationChanged
		HashSet<TopLevelTypeName> dirtyTypeDefinitions = new HashSet<TopLevelTypeName>();
		
		public void NotifyParseInformationChanged(IUnresolvedFile oldUnresolvedFile, IUnresolvedFile newUnresolvedFile)
		{
			// We use delay-loading: the nested tests of a project are
			// initialized only when the NestedTests collection is actually accessed
			// (e.g. when the test tree node is expanded)
			if (!NestedTestsInitialized)
				return;
			// dirtyTypeDefinitions = new HashSet<FullNameAndTypeParameterCount>();
			AddToDirtyList(oldUnresolvedFile);
			AddToDirtyList(newUnresolvedFile);
			ProcessUpdates();
		}
		
		public override bool CanExpandNestedTests {
			get { return true; }
		}
		
		protected override void OnNestedTestsInitialized()
		{
			var compilation = SD.ParserService.GetCompilation(project);
			foreach (var typeDef in compilation.MainAssembly.TopLevelTypeDefinitions) {
				UpdateType(new TopLevelTypeName(typeDef.Namespace, typeDef.Name, typeDef.TypeParameterCount), typeDef);
			}
			base.OnNestedTestsInitialized();
		}
		
		void AddToDirtyList(IUnresolvedFile unresolvedFile)
		{
			if (unresolvedFile != null) {
				foreach (var td in unresolvedFile.TopLevelTypeDefinitions) {
					AddToDirtyList(new TopLevelTypeName(td.Namespace, td.Name, td.TypeParameters.Count));
				}
			}
		}
		
		protected virtual void AddToDirtyList(TopLevelTypeName className)
		{
			dirtyTypeDefinitions.Add(className);
		}
		
		void ProcessUpdates()
		{
			var compilation = SD.ParserService.GetCompilation(project);
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			
			foreach (var dirtyTypeDef in dirtyTypeDefinitions) {
				ITypeDefinition typeDef = compilation.MainAssembly.GetTypeDefinition(dirtyTypeDef.Namespace, dirtyTypeDef.Name, dirtyTypeDef.TypeParameterCount);
				UpdateType(dirtyTypeDef, typeDef);
			}
			dirtyTypeDefinitions.Clear();
		}
		
		/// <summary>
		/// Adds/Updates/Removes the test class for the type definition.
		/// </summary>
		void UpdateType(TopLevelTypeName dirtyTypeDef, ITypeDefinition typeDef)
		{
			ITest test;
			if (topLevelTestClasses.TryGetValue(dirtyTypeDef, out test)) {
				if (typeDef == null) {
					// Test class was removed completely (no parts left)
					RemoveTestClass(dirtyTypeDef, test);
				} else {
					// Test class was modified
					// Check if it's still a test class:
					if (IsTestClass(typeDef))
						UpdateTestClass(test, typeDef);
					else
						RemoveTestClass(dirtyTypeDef, test);
				}
			} else if (typeDef != null) {
				// Test class was added
				var testClass = CreateTestClass(typeDef);
				if (testClass != null)
					AddTestClass(dirtyTypeDef, testClass);
			}
		}
		#endregion
		
		#region Namespace Management
		protected ITest GetTestClass(TopLevelTypeName fullName)
		{
			EnsureNestedTestsInitialized();
			return topLevelTestClasses.GetOrDefault(fullName);
		}
		
		void AddTestClass(TopLevelTypeName fullName, ITest test)
		{
			topLevelTestClasses.Add(fullName, test);
			TestCollection testNamespace = FindOrCreateNamespace(NestedTestCollection, project.RootNamespace, fullName.Namespace);
			testNamespace.Add(test);
		}
		
		void RemoveTestClass(TopLevelTypeName fullName, ITest test)
		{
			topLevelTestClasses.Remove(fullName);
			TestCollection testNamespace = FindNamespace(NestedTestCollection, project.RootNamespace, fullName.Namespace);
			if (testNamespace != null) {
				testNamespace.Remove(test);
				if (testNamespace.Count == 0) {
					// Remove the namespace
					RemoveTestNamespace(NestedTestCollection, project.RootNamespace, fullName.Namespace);
				}
			}
			OnTestClassRemoved(test);
		}
		
		TestCollection FindOrCreateNamespace(TestCollection collection, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return collection;
			foreach (var node in collection.OfType<TestNamespace>()) {
				if (@namespace == node.NamespaceName)
					return node.NestedTests;
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					return FindOrCreateNamespace(node.NestedTests, node.NamespaceName, @namespace);
				}
			}
			// Create missing namespace node:
			
			// Figure out which part of the namespace we can remove due to the parent namespace:
			int startPos = 0;
			if (@namespace.StartsWith(parentNamespace + ".", StringComparison.Ordinal)) {
				startPos = parentNamespace.Length + 1;
			}
			// Get the next dot
			int dotPos = @namespace.IndexOf('.', startPos);
			if (dotPos < 0) {
				var newNode = new TestNamespace(this, @namespace);
				collection.Add(newNode);
				return newNode.NestedTests;
			} else {
				var newNode = new TestNamespace(this, @namespace.Substring(0, dotPos));
				collection.Add(newNode);
				return FindOrCreateNamespace(newNode.NestedTests, newNode.NamespaceName, @namespace);
			}
		}
		
		static TestCollection FindNamespace(TestCollection collection, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return collection;
			foreach (var node in collection.OfType<TestNamespace>()) {
				if (@namespace == node.NamespaceName)
					return node.NestedTests;
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					return FindNamespace(node.NestedTests, node.NamespaceName, @namespace);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Removes the target namespace and all parent namespaces that are empty after the removal.
		/// </summary>
		static void RemoveTestNamespace(TestCollection collection, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return;
			foreach (var node in collection.OfType<TestNamespace>()) {
				if (@namespace == node.NamespaceName) {
					collection.Remove(node);
					return;
				}
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					RemoveTestNamespace(node.NestedTests, node.NamespaceName, @namespace);
					if (node.NestedTests.Count == 0) {
						collection.Remove(node);
					}
					return;
				}
			}
		}
		#endregion
	}
}
