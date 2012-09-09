// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Base class for <see cref="ITest"/> implementations.
	/// </summary>
	public abstract class TestBase : ITest
	{
		public abstract string DisplayName { get; }
		public virtual event EventHandler DisplayNameChanged { add {} remove {} }
		
		public abstract ITestProject ParentProject { get; }
		
		#region Result
		TestResultType result;
		bool useCompositeResultsOfNestedTests;
		
		public event EventHandler<TestResultTypeChangedEventArgs> ResultChanged;
		
		/// <summary>
		/// Gets/Sets the test result.
		/// </summary>
		/// <remarks>
		/// If the result is bound to the composite of the nested tests, setting this
		/// property will remove the binding.
		/// </remarks>
		public TestResultType Result {
			get { return result; }
			protected set {
				if (useCompositeResultsOfNestedTests) {
					nestedTests.PropertyChanged -= nestedTestsCollection_PropertyChanged;
					useCompositeResultsOfNestedTests = false;
				}
				ChangeResult(value);
			}
		}
		
		void ChangeResult(TestResultType newResult)
		{
			TestResultType oldResult = result;
			if (oldResult != newResult) {
				result = newResult;
				OnResultChanged(new TestResultTypeChangedEventArgs(oldResult, newResult));
			}
		}
		
		/// <summary>
		/// Binds the result to the composite result of the nested tests.
		/// </summary>
		protected void BindResultToCompositeResultOfNestedTests()
		{
			if (!useCompositeResultsOfNestedTests) {
				useCompositeResultsOfNestedTests = true;
				if (nestedTests != null) {
					nestedTests.PropertyChanged += nestedTestsCollection_PropertyChanged;
					ChangeResult(nestedTests.CompositeResult);
				} else {
					ChangeResult(TestResultType.None);
				}
			}
		}
		
		void nestedTestsCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == null || e.PropertyName == "CompositeResult") {
				ChangeResult(nestedTests.CompositeResult);
			}
		}
		
		protected virtual void OnResultChanged(TestResultTypeChangedEventArgs e)
		{
			if (ResultChanged != null) {
				ResultChanged(this, e);
			}
		}
		#endregion
		
		#region NestedTests
		TestCollection nestedTests;
		
		public TestCollection NestedTests {
			get {
				if (nestedTests == null) {
					nestedTests = InitializeNestedTests();
					OnNestedTestsInitialized();
				}
				return nestedTests;
			}
		}
		
		protected bool NestedTestsInitialized {
			get { return nestedTests != null; }
		}
		
		protected virtual TestCollection InitializeNestedTests()
		{
			return new TestCollection();
		}
		
		protected virtual void OnNestedTestsInitialized()
		{
			// If we're supposed to be bound to the composite result of the nested tests,
			// but the nested test collection wasn't initialized yet,
			// we will recreate the binding.
			if (useCompositeResultsOfNestedTests) {
				useCompositeResultsOfNestedTests = false;
				BindResultToCompositeResultOfNestedTests();
			}
		}
		#endregion
		
		public virtual void ResetTestResults()
		{
			if (nestedTests != null) {
				foreach (ITest test in nestedTests) {
					test.ResetTestResults();
				}
			}
			if (!useCompositeResultsOfNestedTests)
				this.Result = TestResultType.None;
		}
		
		public virtual bool SupportsGoToDefinition {
			get { return false; }
		}
		
		public void GoToDefinition()
		{
			throw new NotSupportedException();
		}
		
		public virtual UnitTestNode CreateTreeNode()
		{
			return new UnitTestNode(this);
		}
	}
}
