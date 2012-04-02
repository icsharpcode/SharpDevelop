// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
				UnitTestAddInTree addInTree = new UnitTestAddInTree();
				testFrameworks = new RegisteredTestFrameworks(addInTree);
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

	}
}
