// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents the member that will handle the TestCollection's
	/// TestMemberAdded or TestMemberRemoved events.
	/// </summary>
	public delegate void TestMemberEventHandler(object source, TestMemberEventArgs e);
	
	/// <summary>
	/// Provides data for the TestCollection's TestMemberAdded and TestMemberRemoved events.
	/// </summary>
	public class TestMemberEventArgs
	{
		TestMember testMember;
		
		public TestMemberEventArgs(TestMember testMember)
		{
			this.testMember = testMember;
		}
		
		public TestMember TestMember {
			get { return testMember; }
		}
	}
}
