// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

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
		
		protected TestCollection NestedTestCollection {
			get {
				EnsureNestedTestsInitialized();
				return nestedTests;
			}
		}
		
		public IModelCollection<ITest> NestedTests {
			get {
				EnsureNestedTestsInitialized();
				return nestedTests;
			}
		}
		
		public virtual bool CanExpandNestedTests {
			get {
				EnsureNestedTestsInitialized();
				return nestedTests.Count != 0;
			}
		}
		
		/// <summary>
		/// Gets whether the nested test list was initialized.
		/// Accessing the <see cref="NestedTests"/> property will automatically initialize the list.
		/// </summary>
		public bool NestedTestsInitialized {
			get { return nestedTests != null; }
		}
		
		public void EnsureNestedTestsInitialized()
		{
			if (nestedTests == null) {
				nestedTests = InitializeNestedTests();
				OnNestedTestsInitialized();
			}
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
		
		public virtual ImmutableStack<ITest> FindPathToDescendant(ITest test)
		{
			if (test == this)
				return ImmutableStack<ITest>.Empty;
			if (test == null)
				return null;
			if (!NestedTestsInitialized)
				return null; // we don't have any nested tests (yet)
			foreach (ITest nestedTest in NestedTests) {
				var stack = nestedTest.FindPathToDescendant(test);
				if (stack != null)
					return stack.Push(nestedTest);
			}
			return null;
		}
		
		public virtual ICommand GoToDefinition {
			get { return NotAvailableCommand.Instance; }
		}
	}
}
