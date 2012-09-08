// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public static class TestService
	{
		static IRegisteredTestFrameworks testFrameworks;
		static MessageViewCategory unitTestMessageView;
		
		public static IRegisteredTestFrameworks RegisteredTestFrameworks {
			get {
				CreateRegisteredTestFrameworks();
				return testFrameworks;
			}
			set { testFrameworks = value; }
		}
		
		static void CreateRegisteredTestFrameworks()
		{
			if (testFrameworks == null) {
				testFrameworks = new RegisteredTestFrameworks(SD.AddInTree);
			}
		}
		
		public static MessageViewCategory UnitTestMessageView {
			get {
				if (unitTestMessageView == null) {
					CreateUnitTestCategory();
				}
				return unitTestMessageView;
			}
		}
		
		static void CreateUnitTestCategory()
		{
			MessageViewCategory.Create(ref unitTestMessageView,
			                           "UnitTesting",
			                           "${res:ICSharpCode.NUnitPad.NUnitPadContent.PadName}");
		}
		
		static TestSolution solution;
		
		public static TestSolution Solution {
			get {
				SD.MainThread.VerifyAccess();
				if (solution == null)
					solution = new TestSolution(RegisteredTestFrameworks);
				return solution;
			}
		}
	}
}
