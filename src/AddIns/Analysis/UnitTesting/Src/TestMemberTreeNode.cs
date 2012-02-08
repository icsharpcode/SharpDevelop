// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a member that has the [Test] attribute associated with it.
	/// </summary>
	public class TestMemberTreeNode : TestTreeNode
	{
		TestMember testMember;
		
		public TestMemberTreeNode(TestProject project, TestMember testMember) 
			: base(project, testMember.Name)
		{
			this.testMember = testMember;
			testMember.ResultChanged += TestMemberResultChanged;
			UpdateImageListIndex(testMember.Result);
		}
		
		/// <summary>
		/// Gets the underlying IMember for this test member.
		/// </summary>
		public IMember Member {
			get { return testMember.Member; }
		}
		
		/// <summary>
		/// Removes the TestMember.ResultChanged event handler.
		/// </summary>
		public override void Dispose()
		{
			if (!IsDisposed) {
				testMember.ResultChanged -= TestMemberResultChanged;
			}
			base.Dispose();
		}
		
		/// <summary>
		/// Updates the node's icon after the test member result
		/// has changed.
		/// </summary>
		void TestMemberResultChanged(object source, EventArgs e)
		{
			UpdateImageListIndex(testMember.Result);
		}
	}
}
