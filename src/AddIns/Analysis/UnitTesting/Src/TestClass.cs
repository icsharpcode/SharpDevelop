// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a class that can be tested.
	/// </summary>
	public class TestClass
	{
		IClass c;
		TestMemberCollection testMembers;
		TestResultType testResultType;
		IRegisteredTestFrameworks testFrameworks;
		
		/// <summary>
		/// Raised when the test class result is changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		public TestClass(IClass c, IRegisteredTestFrameworks testFrameworks)
		{
			this.c = c;
			this.testFrameworks = testFrameworks;
		}
		
		/// <summary>
		/// Gets the underlying IClass for this test class.
		/// </summary>
		public IClass Class {
			get { return c; }
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
		/// Gets all child namespaces that starts with the specified string.
		/// </summary>
		/// <remarks>
		/// If the starts with string is 'ICSharpCode' and there is a code coverage
		/// method with a namespace of 'ICSharpCode.XmlEditor.Tests', then this
		/// method will return 'XmlEditor' as one of its strings.
		/// </remarks>
		public static string[] GetChildNamespaces(ICollection<TestClass> classes, string parentNamespace) {
			List<string> items = new List<string>();
			foreach (TestClass c in classes) {
				string ns = c.GetChildNamespace(parentNamespace);
				if (ns.Length > 0) {
					if (!items.Contains(ns)) {
						items.Add(ns);
					}
				}
			}
			return items.ToArray();
		}
		
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		public string Name {
			get
			{
				var currentClass = c;
				var name = c.Name;
				while(currentClass.DeclaringType != null)
				{
					name = String.Concat(currentClass.DeclaringType.Name, "+", name);
					currentClass = currentClass.DeclaringType;
				}
				return name;
			}
		}
		
		/// <summary>
		/// Gets the fully qualified name of the class.
		/// </summary>
		public string QualifiedName {
			get { return c.DotNetName; }
		}
		
		/// <summary>
		/// Gets the namespace of this class.
		/// </summary>
		public string Namespace {
			get
			{
				var currentClass = c;
				while (currentClass.DeclaringType != null)
					currentClass = currentClass.DeclaringType;
				return currentClass.Namespace;
			}
		}
		
		/// <summary>
		/// Gets the root namespace for this class.
		/// </summary>
		public string RootNamespace {
			get { return GetRootNamespace(c.Namespace); }
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
		/// Gets the child namespace from the specified namespace
		/// based on the parent namespace.
		/// </summary>
		/// <param name="parentNamespace">Can contain multiple namespaces
		/// (e.g. ICSharpCode.XmlEditor).</param>
		public static string GetChildNamespace(string ns, string parentNamespace)
		{
			if (parentNamespace.Length > 0) {
				if (ns.StartsWith(String.Concat(parentNamespace, "."))) {
					string end = ns.Substring(parentNamespace.Length + 1);
					return GetRootNamespace(end);
				}
				return String.Empty;
			}
			return ns;
		}
		
		/// <summary>
		/// Gets the child namespace based on the parent namespace
		/// from this class.
		/// </summary>
		/// <param name="parentNamespace">Can contain multiple namespaces
		/// (e.g. ICSharpCode.XmlEditor).</param>
		public string GetChildNamespace(string parentNamespace)
		{
			return GetChildNamespace(Namespace, parentNamespace);
		}
		
		/// <summary>
		/// Gets the test members in this class.
		/// </summary>
		public TestMemberCollection TestMembers {
			get {
				if (testMembers == null) {
					GetTestMembers();
				}
				return testMembers;
			}
		}
		
		/// <summary>
		/// Gets the test member with the specified name.
		/// </summary>
		/// <returns>Null if the member cannot be found.</returns>
		public TestMember GetTestMember(string name)
		{
			if (TestMembers.Contains(name)) {
				return TestMembers[name];
			}
			return null;
		}
		
		/// <summary>
		/// Updates the test member with the specified test result.
		/// </summary>
		public void UpdateTestResult(TestResult testResult)
		{
			TestMember member = null;
			string memberName = TestMember.GetMemberName(testResult.Name);
			if (memberName != null) {
				member = GetTestMember(memberName);
				if (member == null) {
					member = GetPrefixedTestMember(testResult.Name);
				}
			}
			if (member != null) {
				member.Result = testResult.ResultType;
			}
		}
		
		/// <summary>
		/// Resets all the test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			Result = TestResultType.None;
			TestMembers.ResetTestResults();
		}
		
		/// <summary>
		/// Updates the members and class based on the new class
		/// information that has been parsed.
		/// </summary>
		public void UpdateClass(IClass c)
		{
			this.c = c.GetCompoundClass();
			
			// Remove missing members.
			TestMemberCollection newTestMembers = GetTestMembers(this.c);
			TestMemberCollection existingTestMembers = TestMembers;
			for (int i = existingTestMembers.Count - 1; i >= 0; --i) {
				TestMember member = existingTestMembers[i];
				if (newTestMembers.Contains(member.Name)) {
					member.Update(newTestMembers[member.Name].Member);
				} else {
					existingTestMembers.RemoveAt(i);
				}
			}
			
			// Add new members.
			foreach (TestMember member in newTestMembers) {
				if (existingTestMembers.Contains(member.Name)) {
				} else {
					existingTestMembers.Add(member);
				}
			}
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
			testMembers = GetTestMembers(c);
			testMembers.ResultChanged += TestMembersResultChanged;
		}
		
		/// <summary>
		/// Gets the test members for the specified class.
		/// </summary>
		TestMemberCollection GetTestMembers(IClass c)
		{
			TestMemberCollection testMembers = new TestMemberCollection();
			foreach (var member in testFrameworks.GetTestMembersFor(c))
				if (!testMembers.Contains(member.Name)) {
					testMembers.Add(member);
				}
			
			// Add base class test members.
			IClass declaringType = c;
			while (c.BaseClass != null)
			{
				foreach (var testMember in testFrameworks.GetTestMembersFor(c.BaseClass)) {
					BaseTestMember baseTestMethod = new BaseTestMember(declaringType, testMember.Member);
					TestMember testMethod = new TestMember(c.BaseClass, baseTestMethod);
					if (testMember.Member.IsVirtual) {
						if (!testMembers.Contains(testMember.Name)) {
							testMembers.Add(testMethod);
						}
					} else {
						if (!testMembers.Contains(testMethod.Name)) {
							testMembers.Add(testMethod);
						}
					}
				}
				c = c.BaseClass;
			}

			baseClassesFQNames.Clear();
			foreach (var memberDeclaringClass in testMembers.Select(member => member.DeclaringType).Distinct())
				if (memberDeclaringClass.CompareTo(declaringType) != 0)
					baseClassesFQNames.Add(memberDeclaringClass.FullyQualifiedName);
			return testMembers;
		}
		
		/// <summary>
		/// Updates the test class's test result after the test member's
		/// test result has changed.
		/// </summary>
		void TestMembersResultChanged(object source, EventArgs e)
		{
			Result = testMembers.Result;
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
		TestMember GetPrefixedTestMember(string testResultName)
		{
			IClass baseClass = c.BaseClass;
			while (baseClass != null) {
				string memberName = TestMember.GetMemberName(testResultName);
				string actualMemberName = String.Concat(baseClass.Name, ".", memberName);
				TestMember member = GetTestMember(actualMemberName);
				if (member != null) {
					return member;
				}
				baseClass = baseClass.BaseClass;
			}
			return null;
		}

		public bool IsDerivedFrom(IClass c)
		{
			return baseClassesFQNames.Contains(c.FullyQualifiedName);
		}
	}
}
