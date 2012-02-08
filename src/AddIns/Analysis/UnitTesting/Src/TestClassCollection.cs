// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class TestClassCollection : KeyedCollection<string, TestClass>
	{
		TestResultType testResult = TestResultType.None;
		
		Dictionary<string, TestClass> passedTestClasses = new Dictionary<string, TestClass>();
		Dictionary<string, TestClass> failedTestClasses = new Dictionary<string, TestClass>();
		Dictionary<string, TestClass> ignoredTestClasses = new Dictionary<string, TestClass>();
		
		/// <summary>
		/// Raised when the test result for this collection of 
		/// classes has changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		/// <summary>
		/// Raised when a class is added to this collection.
		/// </summary>
		public event TestClassEventHandler TestClassAdded;
		
		/// <summary>
		/// Raised when a class is removed from this collection.
		/// </summary>
		public event TestClassEventHandler TestClassRemoved;

		/// <summary>
		/// Gets the overall test results for the collection of 
		/// test classes.
		/// </summary>
		public TestResultType Result {
			get {
				return testResult;
			}
		}
		
		/// <summary>
		/// Sets all the test class test results back to none.
		/// </summary>
		public void ResetTestResults()
		{			
			passedTestClasses.Clear();
			failedTestClasses.Clear();
			ignoredTestClasses.Clear();
			
			foreach (TestClass c in this) {
				c.ResetTestResults();
			}
			
			SetTestResult(TestResultType.None);
		}
		
		/// <summary>
		/// Updates the test method with the specified test result.
		/// </summary>
		public void UpdateTestResult(TestResult testResult)
		{
			TestClass testClass = GetTestClassFromTestMemberName(testResult.Name);
			if (testClass != null) {
				testClass.UpdateTestResult(testResult);
			}
		}
		
		/// <summary>
		/// Gets the matching test member from this set of classes.
		/// </summary>
		/// <param name="fullyQualifiedName">The fully qualified 
		/// method name (e.g. Namespace.ClassName.MethodName).</param>
		/// <returns>Null if the method cannot be found.</returns>
		public TestMember GetTestMember(string fullyQualifiedName)
		{
			string className = TestMember.GetQualifiedClassName(fullyQualifiedName);
			if (className != null) {
				if (Contains(className)) {
					TestClass testClass = this[className];
					string memberName = TestMember.GetMemberName(fullyQualifiedName);
					if (memberName != null) {
						return testClass.GetTestMember(memberName);
					}
				} else {
					LoggingService.Debug("TestClass not found: " + className);
				}
			} else {
				LoggingService.Debug("Invalid test member name: " + fullyQualifiedName);
			}
			return null;
		}
		
		protected override string GetKeyForItem(TestClass item)
		{
			return item.QualifiedName;
		}
		
		protected override void InsertItem(int index, TestClass item)
		{
			item.ResultChanged += TestClassResultChanged;
			base.InsertItem(index, item);
			TestClassResultChanged(item, new EventArgs());
			OnTestClassAdded(item);
		}
		
		protected override void RemoveItem(int index)
		{
			TestClass c = this[index];
			c.ResultChanged -= TestClassResultChanged;
			base.RemoveItem(index);
			OnTestResultNone(c.Name);
			OnTestClassRemoved(c);
		}
		
		protected void OnTestClassAdded(TestClass testClass)
		{
			if (TestClassAdded != null) {
				TestClassAdded(this, new TestClassEventArgs(testClass));
			}
		}
		
		protected void OnTestClassRemoved(TestClass testClass)
		{
			if (TestClassRemoved != null) {
				TestClassRemoved(this, new TestClassEventArgs(testClass));
			}
		}
		
		void TestClassResultChanged(object source, EventArgs e)
		{
			TestClass c = (TestClass)source;
			switch (c.Result) {
				case TestResultType.None:
					OnTestResultNone(c.QualifiedName);
					break;
				case TestResultType.Failure:
					SetTestResult(TestResultType.Failure);
					failedTestClasses.Add(c.QualifiedName, c);
					break;
				case TestResultType.Success:
					passedTestClasses.Add(c.QualifiedName, c);
					if (passedTestClasses.Count == Count) {
						SetTestResult(TestResultType.Success);
					} else if (passedTestClasses.Count + ignoredTestClasses.Count == Count) {
						SetTestResult(TestResultType.Ignored);
					}
					break;
				case TestResultType.Ignored:
					ignoredTestClasses.Add(c.QualifiedName, c);
					if (ignoredTestClasses.Count == Count ||
					    ignoredTestClasses.Count + passedTestClasses.Count == Count) {
						SetTestResult(TestResultType.Ignored);
					}
					break;
			}
		}
		
		void SetTestResult(TestResultType value)
		{
			TestResultType previousTestResult = testResult;
			testResult = value;
			if (testResult != previousTestResult) {
				OnResultChanged();
			}
		}
		
		void OnResultChanged()
		{
			if (ResultChanged != null) {
				ResultChanged(this, new EventArgs());
			}
		}
		
		/// <summary>
		/// Removes the specified test class from the list of
		/// failed, passed and ignored tests and updates the 
		/// test result state of the test class collection.
		/// </summary>
		void OnTestResultNone(string qualifiedName)
		{
			passedTestClasses.Remove(qualifiedName);
			failedTestClasses.Remove(qualifiedName);
			ignoredTestClasses.Remove(qualifiedName);
			if (ignoredTestClasses.Count + failedTestClasses.Count == 0) {
				SetTestResult(TestResultType.None);
			}
		}
		
		/// <summary>
		/// Gets the test class from the specified test result.
		/// </summary>
		TestClass GetTestClassFromTestMemberName(string memberName)
		{
			if (memberName != null) {
				string className = TestMember.GetQualifiedClassName(memberName);
				if (className != null) {
					if (Contains(className)) {
						return this[className];
					} else {
						LoggingService.Debug("TestClass not found: " + className);
						return GetTestClassFromTestMemberName(className);
					}
				} else {
					LoggingService.Debug("Invalid TestMember.Name: " + memberName);
				}
			}
			return null;
		}
	}
}
