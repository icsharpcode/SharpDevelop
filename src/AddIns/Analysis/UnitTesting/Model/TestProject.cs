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
		IProject project;
		IRegisteredTestFrameworks testFrameworks;
		readonly ObservableCollection<TestClass> testClasses;
		
		public TestProject(IProject project)
		{
			this.project = project;
			this.testFrameworks = TestService.RegisteredTestFrameworks;
			project.ParseInformationUpdated += project_ParseInformationUpdated;
			var compilation = SD.ParserService.GetCompilation(project);
			var classes = project.ProjectContent
				.Resolve(compilation.TypeResolveContext)
				.TopLevelTypeDefinitions
				.Where(td => testFrameworks.IsTestClass(td, compilation))
				.Select(g => new TestClass(testFrameworks, g.ReflectionName, g));
			testClasses = new ObservableCollection<TestClass>(classes);
		}

		void project_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			var context = new SimpleTypeResolveContext(SD.ParserService.GetCompilation(project).MainAssembly);
			IEnumerable<ITypeDefinition> @new;
			if (e.NewParsedFile != null)
				@new = e.NewParsedFile.TopLevelTypeDefinitions.Select(utd => utd.Resolve(context).GetDefinition()).Where(x => x != null && testFrameworks.IsTestClass(x, SD.ParserService.GetCompilation(project)));
			else
				@new = Enumerable.Empty<ITypeDefinition>();
			testClasses.UpdateTestClasses(testFrameworks, testClasses.Where(tc => tc.Parts.Any(td => td.ParsedFile.FileName == e.OldParsedFile.FileName)).Select(tc => new DefaultResolvedTypeDefinition(context, tc.Parts.ToArray())).ToList(), @new.ToList());
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public ObservableCollection<TestClass> TestClasses {
			get { return testClasses; }
		}
		
		public TestMember GetTestMethod(string fullName)
		{
			foreach (var tc in testClasses) {
				var result = TreeTraversal.PostOrder(tc, c => c.NestedClasses)
					.SelectMany(c => c.Members)
					.SingleOrDefault(m => fullName.Equals(m.Method.ReflectionName, StringComparison.Ordinal));
				if (result != null)
					return result;
			}
			return null;
		}
		
		public bool FindTestInfo(string fullName, out TestClass testClass, out TestMember method)
		{
			testClass = null;
			method = null;
			foreach (var tc in testClasses) {
				foreach (var c in TreeTraversal.PostOrder(tc, c => c.NestedClasses)) {
					testClass = c;
					method = c.SingleOrDefault(m => fullName.Equals(m.Method.ReflectionName, StringComparison.Ordinal));
					if (method != null)
						return true;
				}
			}
			return false;
		}
		
		public void UpdateTestResult(TestResult result)
		{
			TestMember member;
			TestClass testClass;
			if (FindTestInfo(result.Name, out testClass, out member)) {
				member.TestResult = result.ResultType;
				testClass.TestResult = r
			}
		}
		
		public void ResetTestResults()
		{
			foreach (var member in testClasses.SelectMany(tc => TreeTraversal.PostOrder(tc, c => c.NestedClasses)).SelectMany(c => c.Members))
				member.TestResult = TestResultType.None;
		}
		
		TestResultType testResult;
		
		/// <summary>
		/// Gets the test result for this project.
		/// </summary>
		public TestResultType TestResult {
			get { return testResult; }
			set {
				if (testResult != value) {
					testResult = value;
					OnPropertyChanged();
				}
			}
		}
	}
}
