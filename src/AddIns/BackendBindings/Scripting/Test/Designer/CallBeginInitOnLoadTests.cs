// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	public abstract class CallBeginInitOnLoadTestsBase : LoadFormTestsBase
	{
		public SupportInitCustomControl Control {
			get { return Form.Controls[0] as SupportInitCustomControl; }
		}
		
		public SupportInitCustomControl LocalControl {
			get { return base.ComponentCreator.GetInstance("localVariable") as SupportInitCustomControl; }
		}
		
		[Test]
		public void BeginInitCalled()
		{
			Assert.IsTrue(Control.IsBeginInitCalled);
		}
		
		[Test]
		public void EndInitCalled()
		{
			Assert.IsTrue(Control.IsEndInitCalled);
		}
		
		[Test]
		public void BeginInitCalledOnLocalVariable()
		{
			Assert.IsTrue(LocalControl.IsBeginInitCalled);
		}
		
		[Test]
		public void EndInitCalledOnLocalVariable()
		{
			Assert.IsTrue(LocalControl.IsEndInitCalled);
		}
	}
}
