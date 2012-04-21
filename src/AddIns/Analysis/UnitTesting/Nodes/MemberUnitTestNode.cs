// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;

namespace ICSharpCode.UnitTesting
{
	public class MemberUnitTestNode : UnitTestBaseNode
	{
		TestMember testMember;
		
		public TestMember TestMember {
			get { return testMember; }
		}
		
		public MemberUnitTestNode(TestMember testMember)
		{
			this.testMember = testMember;
		}
		
		public override object Text {
			get { return testMember.Method.Name; }
		}
	}
}
