// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.UnitTesting
{
	public class TestMemberCollection : KeyedCollection<string, TestMember>
	{	
		TestResultType testResult = TestResultType.None;
		Dictionary<string, TestMember> passedTestMembers = new Dictionary<string, TestMember>();
		Dictionary<string, TestMember> failedTestMembers = new Dictionary<string, TestMember>();
		Dictionary<string, TestMember> ignoredTestMembers = new Dictionary<string, TestMember>();
		
		/// <summary>
		/// Raised when the test result for this collection of 
		/// members has changed.
		/// </summary>
		public event EventHandler ResultChanged;
		
		/// <summary>
		/// Raised when a member is added to this collection.
		/// </summary>
		public event TestMemberEventHandler TestMemberAdded;
		
		/// <summary>
		/// Raised when a member is removed from this collection.
		/// </summary>
		public event TestMemberEventHandler TestMemberRemoved;
		
		/// <summary>
		/// Gets the overall test results for the collection of 
		/// test members.
		/// </summary>
		public TestResultType Result {
			get { return testResult; }
		}
		
		/// <summary>
		/// Sets all the test members test results back to none.
		/// </summary>
		public void ResetTestResults()
		{
			passedTestMembers.Clear();
			failedTestMembers.Clear();
			ignoredTestMembers.Clear();
			
			foreach (TestMember member in this) {
				member.Result = TestResultType.None;
			}
			
			SetTestResult(TestResultType.None);
		}
		
		protected override void InsertItem(int index, TestMember item)
		{
			item.ResultChanged += TestMemberResultChanged;
			base.InsertItem(index, item);
			TestMemberResultChanged(item, new EventArgs());
			OnTestMemberAdded(item);
		}
		
		protected override string GetKeyForItem(TestMember item)
		{
			return item.Name;
		}
		
		protected override void RemoveItem(int index)
		{
			TestMember member = this[index];
			member.ResultChanged -= TestMemberResultChanged;
			base.RemoveItem(index);
			OnTestResultNone(member.Name);
			OnTestMemberRemoved(member);
		}
		
		protected void OnTestMemberAdded(TestMember testMember)
		{
			if (TestMemberAdded != null) {
				TestMemberAdded(this, new TestMemberEventArgs(testMember));
			}
		}
		
		protected void OnTestMemberRemoved(TestMember testMember)
		{
			if (TestMemberRemoved != null) {
				TestMemberRemoved(this, new TestMemberEventArgs(testMember));
			}
		}
		
		void TestMemberResultChanged(object source, EventArgs e)
		{
			TestMember member = (TestMember)source;
			switch (member.Result) {
				case TestResultType.None:
					OnTestResultNone(member.Name);
					break;
				case TestResultType.Failure:
					SetTestResult(TestResultType.Failure);
					failedTestMembers.Add(member.Name, member);
					break;
				case TestResultType.Success:
					passedTestMembers.Add(member.Name, member);
					if (passedTestMembers.Count == Count) {
						SetTestResult(TestResultType.Success);
					} else if (passedTestMembers.Count + ignoredTestMembers.Count == Count) {
						SetTestResult(TestResultType.Ignored);
					}
					break;
				case TestResultType.Ignored:
					ignoredTestMembers.Add(member.Name, member);
					if (ignoredTestMembers.Count == Count ||
					    ignoredTestMembers.Count + passedTestMembers.Count == Count) {
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
		/// Removes the specified test member from the list of
		/// failed, passed and ignored tests and updates the 
		/// test result state of the test members collection.
		/// </summary>
		void OnTestResultNone(string name)
		{
			passedTestMembers.Remove(name);
			failedTestMembers.Remove(name);
			ignoredTestMembers.Remove(name);
			if (ignoredTestMembers.Count + failedTestMembers.Count == 0) {
				SetTestResult(TestResultType.None);
			}
		}
	}
}
