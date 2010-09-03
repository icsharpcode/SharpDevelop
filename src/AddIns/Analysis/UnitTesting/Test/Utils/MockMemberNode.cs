// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using System;

namespace UnitTesting.Tests.Utils
{
	public class MockMemberNode : MemberNode
	{
		public MockMemberNode(IMethod method) : base(method)
		{
		}
		
		protected override IAmbience GetAmbience()
		{
			return new MockAmbience();
		}
	}
}
