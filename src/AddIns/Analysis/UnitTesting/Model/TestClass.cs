// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a class that can be tested.
	/// </summary>
	public class TestClass
	{
		string fullName;
		ObservableCollection<IUnresolvedTypeDefinition> parts;
//		TestMemberCollection testMembers;
		TestResultType testResultType;
		IRegisteredTestFrameworks testFrameworks;
		
		/// <summary>
		/// Raised when the test class result is changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		public TestClass(IRegisteredTestFrameworks testFrameworks, string fullName, IEnumerable<IUnresolvedTypeDefinition> parts)
		{
			this.parts = new ObservableCollection<IUnresolvedTypeDefinition>(parts);
			this.testFrameworks = testFrameworks;
			this.fullName = fullName;
		}
		
		/// <summary>
		/// Gets the underlying IClass for this test class.
		/// </summary>
		public IEnumerable<IUnresolvedTypeDefinition> Parts {
			get { return parts; }
		}
		
		public string FullName {
			get { return fullName; }
		}

		/// <summary>
		/// Gets the list of other (e.g. base types) classes where from which test members included in this test class come from.
		/// </summary>
		private readonly ICollection<string> baseClassesFQNames = new List<string>();
		
		/// <summary>
		/// Gets the test classes that exist in the specified namespace.
		/// </summary>		
		public static TestClass[] GetTestClasses(ICollection<TestClass> classes, string ns)
		{
			List<TestClass> matchedClasses = new List<TestClass>();
			foreach (TestClass c in classes) {
				if (c.Namespace == ns) {
					matchedClasses.Add(c);
				}
			}
			return matchedClasses.ToArray();
		}
		
		/// <summary>
		/// Gets the test classes that namespaces starts with the specified 
		/// string.
		/// </summary>		
		public static TestClass[] GetAllTestClasses(ICollection<TestClass> classes, string namespaceStartsWith)
		{
			List<TestClass> matchedClasses = new List<TestClass>();
			foreach (TestClass c in classes) {
				if (c.Namespace.StartsWith(namespaceStartsWith)) {
					matchedClasses.Add(c);
				}
			}
			return matchedClasses.ToArray();
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
//			this.c = c.GetCompoundClass();
//			
//			// Remove missing members.
//			TestMemberCollection newTestMembers = GetTestMembers(this.c);
//			TestMemberCollection existingTestMembers = TestMembers;
//			for (int i = existingTestMembers.Count - 1; i >= 0; --i) {
//				TestMember member = existingTestMembers[i];
//				if (newTestMembers.Contains(member.Name)) {
//					member.Update(newTestMembers[member.Name].Member);
//				} else {
//					existingTestMembers.RemoveAt(i);
//				}
//			}
//			
//			// Add new members.
//			foreach (TestMember member in newTestMembers) {
//				if (existingTestMembers.Contains(member.Name)) {
//				} else {
//					existingTestMembers.Add(member);
//				}
//			}
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

		public bool IsDerivedFrom(IUnresolvedTypeDefinition c)
		{
			return baseClassesFQNames.Contains(c.FullName);
		}
	}
}
