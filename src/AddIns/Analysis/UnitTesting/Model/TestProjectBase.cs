// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		Dictionary<FullNameAndTypeParameterCount, ITest> topLevelTestClasses = new Dictionary<FullNameAndTypeParameterCount, ITest>();
		
		public TestProjectBase(IProject project)
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
		
		public virtual IBuildable GetBuildableForTesting()
		{
			return project;
		}
		
		public override ImmutableStack<ITest> FindPathToDescendant(ITest test)
		{
			if (test == null || test.ParentProject != this)
				return null;
			return base.FindPathToDescendant(test);
		}
		
		#region NotifyParseInformationChanged
		HashSet<FullNameAndTypeParameterCount> dirtyTypeDefinitions = new HashSet<FullNameAndTypeParameterCount>();
		
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
				UpdateType(new FullNameAndTypeParameterCount(typeDef.Namespace, typeDef.Name, typeDef.TypeParameterCount), typeDef);
			}
			base.OnNestedTestsInitialized();
		}
		
		void AddToDirtyList(IUnresolvedFile unresolvedFile)
		{
			if (unresolvedFile != null) {
				foreach (var td in unresolvedFile.TopLevelTypeDefinitions) {
					AddToDirtyList(new FullNameAndTypeParameterCount(td.Namespace, td.Name, td.TypeParameters.Count));
				}
			}
		}
		
		protected virtual void AddToDirtyList(FullNameAndTypeParameterCount className)
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
		void UpdateType(FullNameAndTypeParameterCount dirtyTypeDef, ITypeDefinition typeDef)
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
		protected ITest GetTestClass(FullNameAndTypeParameterCount fullName)
		{
			EnsureNestedTestsInitialized();
			return topLevelTestClasses.GetOrDefault(fullName);
		}
		
		void AddTestClass(FullNameAndTypeParameterCount fullName, ITest test)
		{
			topLevelTestClasses.Add(fullName, test);
			ITest testNamespace = FindOrCreateNamespace(this, project.RootNamespace, fullName.Namespace);
			testNamespace.NestedTests.Add(test);
		}
		
		void RemoveTestClass(FullNameAndTypeParameterCount fullName, ITest test)
		{
			topLevelTestClasses.Remove(fullName);
			ITest testNamespace = FindNamespace(this, project.RootNamespace, fullName.Namespace);
			if (testNamespace != null) {
				testNamespace.NestedTests.Remove(test);
				if (testNamespace.NestedTests.Count == 0) {
					// Remove the namespace
					RemoveTestNamespace(this, project.RootNamespace, fullName.Namespace);
				}
			}
			OnTestClassRemoved(test);
		}
		
		ITest FindOrCreateNamespace(ITest parent, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return parent;
			foreach (var node in parent.NestedTests.OfType<TestNamespace>()) {
				if (@namespace == node.NamespaceName)
					return node;
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					return FindOrCreateNamespace(node, node.NamespaceName, @namespace);
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
				parent.NestedTests.Add(newNode);
				return newNode;
			} else {
				var newNode = new TestNamespace(this, @namespace.Substring(0, dotPos));
				parent.NestedTests.Add(newNode);
				return FindOrCreateNamespace(newNode, newNode.NamespaceName, @namespace);
			}
		}
		
		static ITest FindNamespace(ITest parent, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return parent;
			foreach (var node in parent.NestedTests.OfType<TestNamespace>()) {
				if (@namespace == node.NamespaceName)
					return node;
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					return FindNamespace(node, node.NamespaceName, @namespace);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Removes the target namespace and all parent namespaces that are empty after the removal.
		/// </summary>
		static void RemoveTestNamespace(ITest parent, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return;
			foreach (var node in parent.NestedTests.OfType<TestNamespace>()) {
				if (@namespace == node.NamespaceName) {
					parent.NestedTests.Remove(node);
					return;
				}
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					RemoveTestNamespace(node, node.NamespaceName, @namespace);
					if (node.NestedTests.Count == 0) {
						parent.NestedTests.Remove(node);
					}
					return;
				}
			}
		}
		#endregion
	}
}
