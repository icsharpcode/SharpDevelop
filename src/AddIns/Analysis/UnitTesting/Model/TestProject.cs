// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a project that has a reference to a unit testing
	/// framework assembly. Currently only NUnit is supported.
	/// </summary>
	public class TestProject : ViewModelBase
	{
		readonly IProject project;
		readonly ITestFramework testFramework;
		readonly ObservableCollection<TestClass> testClasses;
		
		public TestProject(IProject project, ITestFramework testFramework)
		{
			this.testFramework = testFramework;
			this.project = project;
			var compilation = SD.ParserService.GetCompilation(project);
			testClasses = new ObservableCollection<TestClass>(
				from td in compilation.MainAssembly.TopLevelTypeDefinitions
				where testFramework.IsTestClass(td)
				select new TestClass(this, td)
			);
		}
		
		HashSet<FullNameAndTypeParameterCount> dirtyTypeDefinitions = new HashSet<FullNameAndTypeParameterCount>();
		
		public void NotifyParseInformationChanged(IUnresolvedFile oldUnresolvedFile, IUnresolvedFile newUnresolvedFile)
		{
			AddToDirtyList(oldUnresolvedFile);
			AddToDirtyList(newUnresolvedFile);
			ProcessUpdates();
		}
		
		void AddToDirtyList(IUnresolvedFile unresolvedFile)
		{
			if (unresolvedFile != null) {
				foreach (var td in unresolvedFile.TopLevelTypeDefinitions) {
					dirtyTypeDefinitions.Add(new FullNameAndTypeParameterCount(td.Namespace, td.Name, td.TypeParameters.Count));
				}
			}
		}
		
		void ProcessUpdates()
		{
			var compilation = SD.ParserService.GetCompilation(project);
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			
			var testClassNameToIndex = new Dictionary<FullNameAndTypeParameterCount, int>();
			for (int i = 0; i < testClasses.Count; i++) {
				var primaryPart = testClasses[i].Parts[0];
				testClassNameToIndex[new FullNameAndTypeParameterCount(primaryPart.Namespace, primaryPart.Name, primaryPart.TypeParameters.Count)] = i;
			}
			
			List<int> testClassesToRemove = new List<int>();
			foreach (var dirtyTypeDef in dirtyTypeDefinitions) {
				ITypeDefinition typeDef = compilation.MainAssembly.GetTypeDefinition(dirtyTypeDef.Namespace, dirtyTypeDef.Name, dirtyTypeDef.TypeParameterCount);
				int pos;
				if (testClassNameToIndex.TryGetValue(dirtyTypeDef, out pos)) {
					if (typeDef == null) {
						// Test class was removed completely (no parts left)
						
						// Removing the class messes up the indices stored in the dictionary,
						// so we'll just remember that we need to remove the class
						testClassesToRemove.Add(pos);
					} else {
						// Test class was modified
						// Check if it's still a test class:
						if (testFramework.IsTestClass(typeDef))
							testClasses[pos].UpdateClass(typeDef);
						else
							testClassesToRemove.Add(pos);
					}
				} else if (typeDef != null && testFramework.IsTestClass(typeDef)) {
					// Test class was added
					testClasses.Add(new TestClass(this, typeDef));
				}
			}
			dirtyTypeDefinitions.Clear();
			// Now remove the outdated test classes
			testClassesToRemove.Sort();
			foreach (int index in testClassesToRemove) {
				testClasses.RemoveAt(index);
			}
		}
		
		public ITestFramework TestFramework {
			get { return testFramework; }
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public ObservableCollection<TestClass> TestClasses {
			get { return testClasses; }
		}
		
		public TestMember GetTestMember(IMember member)
		{
			if (member != null)
				return GetTestMember(member.ReflectionName);
			else
				return null;
		}
		
		public TestMember GetTestMember(string reflectionName)
		{
			foreach (var tc in testClasses) {
				var result = TreeTraversal.PostOrder(tc, c => c.NestedClasses)
					.SelectMany(c => c.Members)
					.SingleOrDefault(m => reflectionName.Equals(m.Member.ReflectionName, StringComparison.Ordinal));
				if (result != null)
					return result;
			}
			return null;
		}
		
		public TestClass GetTestClass(ITypeDefinition typeDefinition)
		{
			if (typeDefinition != null)
				return GetTestClass(typeDefinition.ReflectionName);
			else
				return null;
		}
		
		public TestClass GetTestClass(string reflectionName)
		{
			foreach (var tc in testClasses) {
				foreach (var c in TreeTraversal.PostOrder(tc, c => c.NestedClasses)) {
					var method = c.Members.SingleOrDefault(m => reflectionName.Equals(m.Member.ReflectionName, StringComparison.Ordinal));
					if (method != null)
						return c;
				}
			}
			return null;
		}
		
		public void UpdateTestResult(TestResult result)
		{
			TestClass testClass = GetTestClass(result.Name);
			if (testClass != null) {
				testClass.UpdateTestResult(result);
				TestResult = GetTestResult(testClasses);
			}
		}
		
		public void ResetTestResults()
		{
			foreach (var testClass in testClasses)
				testClass.ResetTestResults();
			TestResult = TestResultType.None;
		}
		
		TestResultType testResult;
		
		/// <summary>
		/// Gets the test result for this project.
		/// </summary>
		public TestResultType TestResult {
			get { return testResult; }
			set { SetAndNotifyPropertyChanged(ref testResult, value); }
		}

		static TestResultType GetTestResult(IList<TestClass> testClasses)
		{
			if (testClasses.Count == 0)
				return TestResultType.None;
			if (testClasses.Any(c => c.TestResult == TestResultType.Failure))
				return TestResultType.Failure;
			if (testClasses.Any(c => c.TestResult == TestResultType.None))
				return TestResultType.None;
			if (testClasses.Any(c => c.TestResult == TestResultType.Ignored))
				return TestResultType.Ignored;
			return TestResultType.Success;
		}
	}
}
