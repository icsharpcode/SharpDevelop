// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Options used for test execution.
	/// </summary>
	public class TestExecutionOptions
	{
		/// <summary>
		/// Gets/Sets whether to debug the test execution.
		/// The default is <c>false</c>.
		/// </summary>
		public bool UseDebugger { get; set; }
	}
}
