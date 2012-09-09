// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Collection of tests that monitors the amount of tests for each result type,
	/// allowing efficient updates of the overall result.
	/// </summary>
	public class TestCollection : ObservableCollection<ITest>
	{
		#region Struct TestCounts
		struct TestCounts
		{
			internal int indeterminate;
			internal int successful;
			internal int failed;
			internal int ignored;
			
			public TestResultType CompositeResult {
				get {
					if (failed > 0)
						return TestResultType.Failure;
					if (indeterminate > 0)
						return TestResultType.None;
					if (ignored > 0)
						return TestResultType.Ignored;
					if (successful > 0)
						return TestResultType.Success;
					return TestResultType.None;
				}
			}
			
			public void Add(TestResultType result)
			{
				switch (result) {
					case TestResultType.None:
						indeterminate++;
						break;
					case TestResultType.Success:
						successful++;
						break;
					case TestResultType.Failure:
						failed++;
						break;
					case TestResultType.Ignored:
						ignored++;
						break;
					default:
						throw new NotSupportedException("Invalid value for TestResultType");
				}
			}
			
			public void Remove(TestResultType result)
			{
				switch (result) {
					case TestResultType.None:
						indeterminate--;
						break;
					case TestResultType.Success:
						successful--;
						break;
					case TestResultType.Failure:
						failed--;
						break;
					case TestResultType.Ignored:
						ignored--;
						break;
					default:
						throw new NotSupportedException("Invalid value for TestResultType");
				}
			}
		}
		#endregion
		
		public static TestResultType GetCompositeResult(IEnumerable<ITest> tests)
		{
			TestCounts c = new TestCounts();
			foreach (ITest test in tests) {
				c.Add(test.Result);
			}
			return c.CompositeResult;
		}
		
		#region Properties
		TestCounts counts;
		
		public int IndeterminateNestedTestCount {
			get { return counts.indeterminate; }
		}
		
		public int SuccessfulNestedTestCount {
			get { return counts.successful; }
		}
		
		public int FailedNestedTestCount {
			get { return counts.failed; }
		}
		
		public int IgnoredNestedTestCount {
			get { return counts.ignored; }
		}
		
		public TestResultType CompositeResult {
			get { return counts.CompositeResult; }
		}
		
		static readonly PropertyChangedEventArgs indeterminateArgs = new PropertyChangedEventArgs("IndeterminateNestedTestCount");
		static readonly PropertyChangedEventArgs successfulArgs = new PropertyChangedEventArgs("SuccessfulNestedTestCount");
		static readonly PropertyChangedEventArgs failedArgs = new PropertyChangedEventArgs("FailedNestedTestCount");
		static readonly PropertyChangedEventArgs ignoredArgs = new PropertyChangedEventArgs("IgnoredNestedTestCount");
		static readonly PropertyChangedEventArgs compositeResultArgs = new PropertyChangedEventArgs("CompositeResult");
		
		void RaiseCountChangeEvents(TestCounts old)
		{
			if (old.indeterminate != counts.indeterminate)
				OnPropertyChanged(indeterminateArgs);
			if (old.successful != counts.successful)
				OnPropertyChanged(successfulArgs);
			if (old.failed != counts.failed)
				OnPropertyChanged(failedArgs);
			if (old.ignored != counts.ignored)
				OnPropertyChanged(ignoredArgs);
			if (old.CompositeResult != counts.CompositeResult)
				OnPropertyChanged(compositeResultArgs);
		}
		
		// change visibility of PropertyChanged event to public
		public new event PropertyChangedEventHandler PropertyChanged {
			add { base.PropertyChanged += value; }
			remove { base.PropertyChanged -= value; }
		}
		#endregion
		
		void item_ResultChanged(object sender, TestResultTypeChangedEventArgs e)
		{
			TestCounts old = counts;
			counts.Remove(e.OldResult);
			counts.Add(e.NewResult);
			RaiseCountChangeEvents(old);
		}
		
		protected override void ClearItems()
		{
			TestCounts old = counts;
			counts = default(TestCounts);
			foreach (ITest test in Items) {
				counts.Remove(test.Result);
				test.ResultChanged -= item_ResultChanged;
			}
			base.ClearItems();
			RaiseCountChangeEvents(old);
		}
		
		protected override void InsertItem(int index, ITest item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			TestCounts old = counts;
			counts.Add(item.Result);
			base.InsertItem(index, item);
			item.ResultChanged += item_ResultChanged;
			RaiseCountChangeEvents(old);
		}
		
		protected override void RemoveItem(int index)
		{
			TestCounts old = counts;
			ITest oldItem = Items[index];
			counts.Remove(oldItem.Result);
			oldItem.ResultChanged -= item_ResultChanged;
			base.RemoveItem(index);
			RaiseCountChangeEvents(old);
		}
		
		protected override void SetItem(int index, ITest item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			TestCounts old = counts;
			
			ITest oldItem = Items[index];
			counts.Remove(oldItem.Result);
			oldItem.ResultChanged -= item_ResultChanged;
			
			counts.Add(item.Result);
			item.ResultChanged += item_ResultChanged;
			
			base.SetItem(index, item);
			RaiseCountChangeEvents(old);
		}
	}
}
