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

using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a unit test or a group of unit tests.
	/// </summary>
	public interface ITest
	{
		/// <summary>
		/// Gets the collection of nested tests.
		/// </summary>
		IModelCollection<ITest> NestedTests { get; }
		
		/// <summary>
		/// Gets whether this test allows expanding the list of nested tests.
		/// If possible, this property should return the same value as <c>NestedTests.Count &gt; 0</c>.
		/// However, when doing so is expensive (e.g. due to lazy initialization), this
		/// property may return true even if there are no nested tests.
		/// </summary>
		bool CanExpandNestedTests { get; }
		
		/// <summary>
		/// Gets the parent project that owns this test.
		/// </summary>
		ITestProject ParentProject { get; }
		
		/// <summary>
		/// Name to be displayed in the tests tree view.
		/// </summary>
		string DisplayName { get; }
		
		/// <summary>
		/// Raised when the <see cref="DisplayName"/> property changes.
		/// </summary>
		event EventHandler DisplayNameChanged;
		
		/// <summary>
		/// Gets the result of the previous run of this test.
		/// </summary>
		TestResultType Result { get; }
		
		/// <summary>
		/// Raised when the <see cref="Result"/> property changes.
		/// </summary>
		event EventHandler<TestResultTypeChangedEventArgs> ResultChanged;
		
		/// <summary>
		/// Resets the test results for this test and all nested tests.
		/// </summary>
		void ResetTestResults();
		
		/// <summary>
		/// Retrieves the path to the specified test, if it is a descendant of this test.
		/// Returns null if the specified test is not a descendant.
		/// Returns an empty stack if this is the test we are searching for.
		/// The top-most element on the stack (=first when enumerating the stack) will be
		/// a direct child of this test.
		/// </summary>
		ImmutableStack<ITest> FindPathToDescendant(ITest test);
		
		System.Windows.Input.ICommand GoToDefinition { get; }
	}
}
