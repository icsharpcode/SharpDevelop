// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
