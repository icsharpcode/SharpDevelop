// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.UnitTesting
{
	public class TestMethodCollection : KeyedCollection<string, TestMethod>
	{	
		TestResultType testResult = TestResultType.None;
		Dictionary<string, TestMethod> passedTestMethods = new Dictionary<string, TestMethod>();
		Dictionary<string, TestMethod> failedTestMethods = new Dictionary<string, TestMethod>();
		Dictionary<string, TestMethod> ignoredTestMethods = new Dictionary<string, TestMethod>();
		
		/// <summary>
		/// Raised when the test result for this collection of 
		/// methods has changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		/// <summary>
		/// Raised when a method is added to this collection.
		/// </summary>
		public event TestMethodEventHandler TestMethodAdded;
		
		/// <summary>
		/// Raised when a method is removed from this collection.
		/// </summary>
		public event TestMethodEventHandler TestMethodRemoved;
		
		/// <summary>
		/// Gets the overall test results for the collection of 
		/// test methods.
		/// </summary>
		public TestResultType Result {
			get {
				return testResult;
			}
		}
		
		/// <summary>
		/// Sets all the test method test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			passedTestMethods.Clear();
			failedTestMethods.Clear();
			ignoredTestMethods.Clear();
			
			foreach (TestMethod method in this) {
				method.Result = TestResultType.None;
			}
			
			SetTestResult(TestResultType.None);
		}
		
		protected override void InsertItem(int index, TestMethod item)
		{
			item.ResultChanged += TestMethodResultChanged;
			base.InsertItem(index, item);
			TestMethodResultChanged(item, new EventArgs());
			OnTestMethodAdded(item);
		}
		
		protected override string GetKeyForItem(TestMethod item)
		{
			return item.Name;
		}
		
		protected override void RemoveItem(int index)
		{
			TestMethod method = this[index];
			method.ResultChanged -= TestMethodResultChanged;
			base.RemoveItem(index);
			OnTestResultNone(method.Name);
			OnTestMethodRemoved(method);
		}
		
		protected void OnTestMethodAdded(TestMethod testMethod)
		{
			if (TestMethodAdded != null) {
				TestMethodAdded(this, new TestMethodEventArgs(testMethod));
			}
		}
		
		protected void OnTestMethodRemoved(TestMethod testMethod)
		{
			if (TestMethodRemoved != null) {
				TestMethodRemoved(this, new TestMethodEventArgs(testMethod));
			}
		}
		
		void TestMethodResultChanged(object source, EventArgs e)
		{
			TestMethod method = (TestMethod)source;
			switch (method.Result) {
				case TestResultType.None:
					OnTestResultNone(method.Name);
					break;
				case TestResultType.Failure:
					SetTestResult(TestResultType.Failure);
					failedTestMethods.Add(method.Name, method);
					break;
				case TestResultType.Success:
					passedTestMethods.Add(method.Name, method);
					if (passedTestMethods.Count == Count) {
						SetTestResult(TestResultType.Success);
					} else if (passedTestMethods.Count + ignoredTestMethods.Count == Count) {
						SetTestResult(TestResultType.Ignored);
					}
					break;
				case TestResultType.Ignored:
					ignoredTestMethods.Add(method.Name, method);
					if (ignoredTestMethods.Count == Count ||
					    ignoredTestMethods.Count + passedTestMethods.Count == Count) {
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
		/// Removes the specified test method from the list of
		/// failed, passed and ignored tests and updates the 
		/// test result state of the test methods collection.
		/// </summary>
		void OnTestResultNone(string name)
		{
			passedTestMethods.Remove(name);
			failedTestMethods.Remove(name);
			ignoredTestMethods.Remove(name);
			if (ignoredTestMethods.Count + failedTestMethods.Count == 0) {
				SetTestResult(TestResultType.None);
			}
		}
	}
}
