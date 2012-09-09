// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;

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
		TestCollection NestedTests { get; }
		
		/// <summary>
		/// Gets the parent project that owns this test.
		/// </summary>
		ITestProject ParentProject { get; }
		
		/// <summary>
		/// Name to be displayed in the tests tree view.
		/// </summary>
		string DisplayName { get; }
		
		/// <summary>
		/// Raised when the <see cref="Name"/> property changes.
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
		
		bool SupportsGoToDefinition { get; }
		
		void GoToDefinition();
		
		UnitTestNode CreateTreeNode();
	}
}
