// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a class that can be tested.
	/// </summary>
	public class TestClass : ViewModelBase
	{
		IList<IUnresolvedTypeDefinition> parts;
		readonly ObservableCollection<TestMember> testMembers;
		readonly ObservableCollection<TestClass> nestedClasses;
		
		public TestClass(TestProject project, ITypeDefinition definition, TestClass parent = null)
		{
			this.parts = new ObservableCollection<IUnresolvedTypeDefinition>();
			this.testMembers = new ObservableCollection<TestMember>();
			this.nestedClasses = new ObservableCollection<TestClass>();
			Project = project;
			Parent = parent;
			UpdateClass(definition);
		}
		
		public TestClass Parent { get; private set; }
		
		public TestProject Project { get; private set; }
		
		/// <summary>
		/// Gets the underlying IUnresolvedTypeDefinitions for this test class.
		/// </summary>
		public IList<IUnresolvedTypeDefinition> Parts {
			get { return parts; }
		}
		
		public ObservableCollection<TestMember> Members {
			get { return testMembers; }
		}
		
		public ObservableCollection<TestClass> NestedClasses {
			get { return nestedClasses; }
		}
		
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		public string Name {
			get { return parts[0].Name; }
		}
		
		/// <summary>
		/// Gets the fully qualified name of the class.
		/// </summary>
		public string QualifiedName {
			get { return parts[0].ReflectionName; }
		}
		
		/// <summary>
		/// Gets the namespace of this class.
		/// </summary>
		public string Namespace {
			get { return parts[0].Namespace; }
		}
		
		TestResultType testResult;
		
		/// <summary>
		/// Gets the test result for this class.
		/// </summary>
		public TestResultType TestResult {
			get { return testResult; }
			set { SetAndNotifyPropertyChanged(ref testResult, value); }
		}

		static TestResultType GetTestResult(TestClass testClass)
		{
			if (testClass.nestedClasses.Count == 0 && testClass.testMembers.Count == 0)
				return TestResultType.None;
			if (testClass.nestedClasses.Any(c => c.TestResult == TestResultType.Failure)
			    || testClass.testMembers.Any(m => m.TestResult == TestResultType.Failure))
				return TestResultType.Failure;
			if (testClass.nestedClasses.Any(c => c.TestResult == TestResultType.None)
			    || testClass.testMembers.Any(m => m.TestResult == TestResultType.None))
				return TestResultType.None;
			if (testClass.nestedClasses.Any(c => c.TestResult == TestResultType.Ignored)
			    || testClass.testMembers.Any(m => m.TestResult == TestResultType.Ignored))
				return TestResultType.Ignored;
			return TestResultType.Success;
		}
		
		/// <summary>
		/// Updates the test member with the specified test result.
		/// </summary>
		public void UpdateTestResult(TestResult testResult)
		{
			var member = testMembers.SingleOrDefault(m => m.Member.ReflectionName == testResult.Name);
			member.TestResult = testResult.ResultType;
			var parent = this;
			while (parent != null) {
				parent.TestResult = GetTestResult(parent);
				parent = parent.Parent;
			}
		}
		
		/// <summary>
		/// Resets all the test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			foreach (var testClass in TreeTraversal.PostOrder(this, c => c.NestedClasses)) {
				foreach (var member in testClass.Members)
					member.ResetTestResult();
				testClass.TestResult = TestResultType.None;
			}
			var parent = this;
			while (parent != null) {
				parent.TestResult = TestResultType.None;
				parent = parent.Parent;
			}
		}
		
		/// <summary>
		/// Updates the members and class based on the new class
		/// information that has been parsed.
		/// </summary>
		public void UpdateClass(ITypeDefinition typeDefinition)
		{
			this.parts = typeDefinition.Parts;
			
			testMembers.Clear();
			testMembers.AddRange(Project.TestFramework.GetTestMembersFor(Project, typeDefinition));
			
			
			/*var oldParts = this.parts;
			nestedClasses.UpdateTestClasses(testFrameworks,
			
			int i = 0;
			while (i < parts.Count) {
				var part = parts[i];
				if (!definition.Parts.Any(p => p.UnresolvedFile.FileName == part.UnresolvedFile.FileName && p.Region == part.Region))
					parts.RemoveAt(i);
				else
					i++;
			}
			
			foreach (var part in definition.Parts) {
				if (!parts.Any(p => p.UnresolvedFile.FileName == part.UnresolvedFile.FileName && p.Region == part.Region))
					parts.Add(part);
			}
			testMembers.RemoveWhere(m => !definition.Methods.Any(dm => dm.ReflectionName == m.Member.ReflectionName && testFrameworks.IsTestMethod(dm)));
			testMembers.AddRange(
				definition.Methods.Where(m => testFrameworks.IsTestMethod(m, definition.Compilation)
				                         && !testMembers.Any(dm => dm.Member.ReflectionName == m.ReflectionName))
				.Select(m => new TestMember(Project, (IUnresolvedMethod)m.UnresolvedMember, testFrameworks.IsTestCase(m, definition.Compilation))));
			
			var context = new SimpleTypeResolveContext(definition);
			nestedClasses.UpdateTestClasses(testFrameworks, nestedClasses.Select(tc => new DefaultResolvedTypeDefinition(context, tc.Parts.ToArray())).ToList(), definition.NestedTypes.Where(nt => testFrameworks.IsTestClass(nt, definition.Compilation)).ToList(), this, Project);
			 */
		}
		
		/// <summary>
		/// Gets the first dotted part of the namespace.
		/// </summary>
		static string GetRootNamespace(string ns)
		{
			int index = ns.IndexOf('.');
			if (index > 0) {
				return ns.Substring(0, index);
			}
			return ns;
		}
		
		public ITypeDefinition Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(Project.Project);
			return parts[0].Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
		}
		
		public override string ToString()
		{
			return string.Format("[TestClass TestResult={0}, Name={1}]", testResult, this.QualifiedName);
		}
	}
}