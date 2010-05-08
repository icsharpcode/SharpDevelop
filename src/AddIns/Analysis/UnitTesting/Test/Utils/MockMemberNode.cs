// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
