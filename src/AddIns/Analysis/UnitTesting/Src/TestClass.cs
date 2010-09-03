// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		TestMethodCollection testMethods;
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
			get { 
				if (c.DeclaringType != null) {
					return String.Concat(c.DeclaringType.Name, "+", c.Name);
				}
				return c.Name;
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
			get { 	
				if (c.DeclaringType != null) {
					return c.DeclaringType.Namespace;
				}
				return c.Namespace;
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
		/// Gets the test methods in this class.
		/// </summary>
		public TestMethodCollection TestMethods {
			get {
				if (testMethods == null) {
					GetTestMethods();
				}
				return testMethods;
			}
		}
		
		/// <summary>
		/// Gets the test method with the specified name.
		/// </summary>
		/// <returns>Null if the method cannot be found.</returns>
		public TestMethod GetTestMethod(string name)
		{
			if (TestMethods.Contains(name)) {
				return TestMethods[name];
			}
			return null;
		}
		
		/// <summary>
		/// Updates the test method with the specified test result.
		/// </summary>
		public void UpdateTestResult(TestResult testResult)
		{
			TestMethod method = null;
			string methodName = TestMethod.GetMethodName(testResult.Name);
			if (methodName != null) {
				method = GetTestMethod(methodName);
				if (method == null) {
					method = GetPrefixedTestMethod(testResult.Name);
				}
			}
			if (method != null) {
				method.Result = testResult.ResultType;
			}
		}
		
		/// <summary>
		/// Resets all the test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			Result = TestResultType.None;
			TestMethods.ResetTestResults();
		}
		
		/// <summary>
		/// Updates the methods and class based on the new class
		/// information that has been parsed.
		/// </summary>
		public void UpdateClass(IClass c)
		{
			this.c = c.GetCompoundClass();
			
			// Remove missing methods.
			TestMethodCollection newTestMethods = GetTestMethods(this.c);
			TestMethodCollection existingTestMethods = TestMethods;
			for (int i = existingTestMethods.Count - 1; i >= 0; --i) {
				TestMethod method = existingTestMethods[i];
				if (newTestMethods.Contains(method.Name)) {
					method.Update(newTestMethods[method.Name].Method);
				} else {
					existingTestMethods.RemoveAt(i);
				}
			}
			
			// Add new methods.
			foreach (TestMethod method in newTestMethods) {
				if (existingTestMethods.Contains(method.Name)) {
				} else {
					existingTestMethods.Add(method);
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
		/// Gets the test methods for the class.
		/// </summary>
		void GetTestMethods()
		{
			testMethods = GetTestMethods(c);
			testMethods.ResultChanged += TestMethodsResultChanged;
		}
		
		/// <summary>
		/// Gets the test methods for the specified class.
		/// </summary>
		TestMethodCollection GetTestMethods(IClass c)
		{
			TestMethodCollection testMethods = new TestMethodCollection();
			foreach (IMethod method in c.Methods) {
				if (IsTestMethod(method)) {
					if (!testMethods.Contains(method.Name)) {
						testMethods.Add(new TestMethod(method));
					}
				}
			}
			
			// Add base class test methods.
			IClass declaringType = c;
			while (c.BaseClass != null) {
				foreach (IMethod method in c.BaseClass.Methods) {
					if (IsTestMethod(method)) {
						BaseTestMethod baseTestMethod = new BaseTestMethod(declaringType, method);
						TestMethod testMethod = new TestMethod(c.BaseClass.Name, baseTestMethod);
						if (method.IsVirtual) {
							if (!testMethods.Contains(method.Name)) {
								testMethods.Add(testMethod);	
							}
						} else {
							if (!testMethods.Contains(testMethod.Name)) {
								testMethods.Add(testMethod);
							}
						}
					}
				}
				c = c.BaseClass;
			}
			return testMethods;
		}
		
		bool IsTestMethod(IMember method)
		{
			return testFrameworks.IsTestMethod(method);
		}
		
		/// <summary>
		/// Updates the test class's test result after the test method's
		/// test result has changed.
		/// </summary>
		void TestMethodsResultChanged(object source, EventArgs e)
		{
			Result = testMethods.Result;
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
		/// the corresponding test method.
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
		TestMethod GetPrefixedTestMethod(string testResultName)
		{
			IClass baseClass = c.BaseClass;
			while (baseClass != null) {
				string methodName = TestMethod.GetMethodName(testResultName);
				string actualMethodName = String.Concat(baseClass.Name, ".", methodName);
				TestMethod method = GetTestMethod(actualMethodName);
				if (method != null) {
					return method;
				}
				baseClass = baseClass.BaseClass;
			}
			return null;
		}
	}
}
