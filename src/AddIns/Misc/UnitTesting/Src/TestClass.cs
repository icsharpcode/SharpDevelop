// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a class that can be tested. In order for a 
	/// class to be considered to be testable it needs to have the
	/// [TestFixture] attribute.
	/// </summary>
	public class TestClass
	{
		IClass c;
		TestMethodCollection testMethods;
		TestResultType testResultType;
		
		/// <summary>
		/// Raised when the test class result is changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		public TestClass(IClass c)
		{
			this.c = c;
		}
		
		/// <summary>
		/// Gets the underlying IClass for this test class.
		/// </summary>
		public IClass Class {
			get {
				return c;
			}
		}
		
		/// <summary>
		/// Determines whether the class is a test fixture. A class
		/// is considered to be a test class if it contains certain
		/// test attributes.
		/// </summary>
		public static bool IsTestClass(IClass c)
		{
			StringComparer nameComparer = GetNameComparer(c);
			if (nameComparer != null) {
				TestAttributeName testAttributeName = new TestAttributeName("TestFixture", nameComparer);
				foreach (IAttribute attribute in c.Attributes) {
					if (testAttributeName.IsEqual(attribute.Name)) {
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Returns the name comparer for the specified class.
		/// </summary>
		public static StringComparer GetNameComparer(IClass c)
		{
			if (c != null) {
				IProjectContent projectContent = c.ProjectContent;
				if (projectContent != null) {
					LanguageProperties language = projectContent.Language;
					if (language != null) {
						return language.NameComparer;
					}
				}
			}
			return null;
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
				return c.Name;
			}
		}
		
		/// <summary>
		/// Gets the fully qualified name of the class.
		/// </summary>
		public string QualifiedName {
			get {
				return c.FullyQualifiedName;
			}
		}
		
		/// <summary>
		/// Gets the namespace of this class.
		/// </summary>
		public string Namespace {
			get {
				return c.Namespace;
			}
		}
		
		/// <summary>
		/// Gets the root namespace for this class.
		/// </summary>
		public string RootNamespace {
			get {
				return GetRootNamespace(c.Namespace);
			}
		}
		
		/// <summary>
		/// Gets the test result for this class.
		/// </summary>
		public TestResultType Result {
			get {
				return testResultType;
			}
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
				if (!newTestMethods.Contains(method.Name)) {
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
		static TestMethodCollection GetTestMethods(IClass c)
		{
			TestMethodCollection testMethods = new TestMethodCollection();
			foreach (IMethod method in c.Methods) {
				if (TestMethod.IsTestMethod(method)) {
					if (!testMethods.Contains(method.Name)) {
						testMethods.Add(new TestMethod(method));
					}
				}
			}
			
			// Add base class test methods.
			if (c.BaseClass != null) {
				foreach (IMethod method in c.BaseClass.Methods) {
					if (TestMethod.IsTestMethod(method)) {
						TestMethod testMethod = new TestMethod(c.BaseClass.Name, method);
						if (!testMethods.Contains(testMethod.Name)) {
							testMethods.Add(testMethod);
						}
					}
				}
			}
			return testMethods;
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
		/// Updates the result of this class based on the results of
		/// its methods.
		/// </summary>
		void UpdateResult()
		{
			bool failure = false;
			bool ignored = false;
			bool notRun = false;
			foreach (TestMethod method in TestMethods) {
				switch (method.Result) {
					case TestResultType.None:
						notRun = true;
						break;
					case TestResultType.Failure:
						failure = true;
						break;
					case TestResultType.Ignored:
						ignored = true;
						break;
				}
				if (failure) {
					break;
				}
			}
			if (failure) {
				Result = TestResultType.Failure;
			} else if (ignored) {
				Result = TestResultType.Ignored;
			} else if (!notRun) {
				Result = TestResultType.Success;
			}
		}
		
		/// <summary>
		/// First tries the last dotted part of the test result name as the
		/// method name. If there is no matching method the preceding dotted
		/// part is prefixed to the method name until a match is found. 
		/// 
		/// Given a test result of:
		/// 
		/// RootNamespace.ClassName.BaseClass1.BaseClass2.TestMethod
		/// 
		/// The method names tried are:
		/// 
		/// TestMethod
		/// BaseClass2.TestMethod
		/// BaseClass2.BaseClass1.TestMethod
		/// etc.
		/// </summary>
		TestMethod GetPrefixedTestMethod(string testResultName)
		{
			int index = 0;
			string methodName = TestMethod.GetMethodName(testResultName);
			string className = TestMethod.GetQualifiedClassName(testResultName);
			do {
				index = className.LastIndexOf('.');
				if (index > 0) {
					methodName = String.Concat(className.Substring(index + 1), ".", methodName);
					TestMethod method = GetTestMethod(methodName);
					if (method != null) {
						return method;
					}
					className = className.Substring(0, index);
				}
			} while (index > 0);
			return null;
		}
	}
}
