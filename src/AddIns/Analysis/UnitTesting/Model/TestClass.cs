// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a class that can be tested.
	/// </summary>
	public class TestClass
	{
		string fullName;
		ObservableCollection<IUnresolvedTypeDefinition> parts;
		readonly ObservableCollection<TestMember> testMembers;
		readonly ObservableCollection<TestClass> nestedClasses;
		TestResultType testResultType;
		IRegisteredTestFrameworks testFrameworks;
		
		/// <summary>
		/// Raised when the test class result is changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		public TestClass(IRegisteredTestFrameworks testFrameworks, string fullName, ITypeDefinition definition)
		{
			this.parts = new ObservableCollection<IUnresolvedTypeDefinition>();
			this.testMembers = new ObservableCollection<TestMember>();
			this.nestedClasses = new ObservableCollection<TestClass>();
			this.testFrameworks = testFrameworks;
			this.fullName = fullName;
			UpdateClass(definition);
		}
		
		/// <summary>
		/// Gets the underlying IClass for this test class.
		/// </summary>
		public IEnumerable<IUnresolvedTypeDefinition> Parts {
			get { return parts; }
		}
		
		public ObservableCollection<TestMember> Members {
			get { return testMembers; }
		}
		
		public ObservableCollection<TestClass> NestedClasses {
			get { return nestedClasses; }
		}
		
		public string FullName {
			get { return fullName; }
		}

		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		public string Name {
			get { return parts.First().Name; }
		}
		
		/// <summary>
		/// Gets the fully qualified name of the class.
		/// </summary>
		public string QualifiedName {
			get { return parts.First().ReflectionName; }
		}
		
		/// <summary>
		/// Gets the namespace of this class.
		/// </summary>
		public string Namespace {
			get { return parts.First().Namespace; }
		}
		
		/// <summary>
		/// Gets the test result for this class.
		/// </summary>
		public TestResultType Result {
			get { return testResultType; }
			set {
				TestResultType previousTestResultType = testResultType;
				testResultType = value;
				if (previousTestResultType != testResultType) {
					OnResultChanged();
				}
			}
		}
		
		/// <summary>
		/// Updates the test member with the specified test result.
		/// </summary>
		public void UpdateTestResult(TestResult testResult)
		{
//			TestMember member = null;
//			string memberName = TestMember.GetMemberName(testResult.Name);
//			if (memberName != null) {
//				member = GetTestMember(memberName);
//				if (member == null) {
//					member = GetPrefixedTestMember(testResult.Name);
//				}
//			}
//			if (member != null) {
//				member.Result = testResult.ResultType;
//			}
		}
		
		/// <summary>
		/// Resets all the test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			Result = TestResultType.None;
//			TestMembers.ResetTestResults();
		}
		
		/// <summary>
		/// Updates the members and class based on the new class
		/// information that has been parsed.
		/// </summary>
		public void UpdateClass(ITypeDefinition definition)
		{
			int i = 0;
			while (i < parts.Count) {
				var part = parts[i];
				if (!definition.Parts.Any(p => p.ParsedFile.FileName == part.ParsedFile.FileName && p.Region == part.Region))
					parts.RemoveAt(i);
				else
					i++;
			}
			
			foreach (var part in definition.Parts) {
				if (!parts.Any(p => p.ParsedFile.FileName == part.ParsedFile.FileName && p.Region == part.Region))
					parts.Add(part);
			}
			testMembers.RemoveWhere(m => !definition.Methods.Any(dm => dm.ReflectionName == m.Method.ReflectionName && dm.IsTestMethod(definition.Compilation)));
			testMembers.AddRange(definition.Methods.Where(m => m.IsTestMethod(definition.Compilation) && !testMembers.Any(dm => dm.Method.ReflectionName == m.ReflectionName)).Select(m => new TestMember((IUnresolvedMethod)m.UnresolvedMember)));
			
			var context = new SimpleTypeResolveContext(definition);
			nestedClasses.UpdateTestClasses(testFrameworks, nestedClasses.Select(tc => new DefaultResolvedTypeDefinition(context, tc.Parts.ToArray())).ToList(), definition.NestedTypes.Where(nt => nt.HasTests(definition.Compilation)).ToList());
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
		
		/// <summary>
		/// Gets the test members for the class.
		/// </summary>
		void GetTestMembers()
		{
//			testMembers = GetTestMembers(c);
//			testMembers.ResultChanged += TestMembersResultChanged;
		}
		
		/// <summary>
		/// Updates the test class's test result after the test member's
		/// test result has changed.
		/// </summary>
		void TestMembersResultChanged(object source, EventArgs e)
		{
//			Result = testMembers.Result;
		}
		
		/// <summary>
		/// Raises the ResultChanged event.
		/// </summary>
		void OnResultChanged()
		{
			if (ResultChanged != null) {
				ResultChanged(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// This function adds the base class as a prefix and tries to find
		/// the corresponding test member.
		/// 
		/// Actual method name:
		/// 
		/// RootNamespace.TestFixture.TestFixtureBaseClass.TestMethod
		/// </summary>
		/// <remarks>
		/// NUnit 2.4 uses the correct test method name when a test
		/// class uses a base class with test methods. It does
		/// not prefix the test method name with the base class name
		/// in the test results returned from nunit-console. It still
		/// displays the name in the NUnit GUI with the base class
		/// name prefixed. Older versions of NUnit-console (2.2.9) returned
		/// the test result with the test method name as follows:
		/// 
		/// RootNamespace.TestFixture.BaseTestFixture.TestMethod
		/// 
		/// The test method name would have the base class name prefixed
		/// to it.
		/// </remarks>
//		TestMember GetPrefixedTestMember(string testResultName)
//		{
//			IClass baseClass = c.BaseClass;
//			while (baseClass != null) {
//				string memberName = TestMember.GetMemberName(testResultName);
//				string actualMemberName = String.Concat(baseClass.Name, ".", memberName);
//				TestMember member = GetTestMember(actualMemberName);
//				if (member != null) {
//					return member;
//				}
//				baseClass = baseClass.BaseClass;
//			}
//			return null;
//		}
	}
}
