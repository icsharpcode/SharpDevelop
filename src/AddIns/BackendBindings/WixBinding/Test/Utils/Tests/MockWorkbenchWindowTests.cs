// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockWorkbenchWindowTests
	{
		MockWorkbenchWindow workbenchWindow;
		
		[SetUp]
		public void Init()
		{
			workbenchWindow = new MockWorkbenchWindow();
		}
		
		[Test]
		public void WorkbenchWindowSelectWindowMethodCalledIsInitiallyFalse()
		{
			Assert.IsFalse(workbenchWindow.SelectWindowMethodCalled);
		}
		
		[Test]
		public void WorkbenchWindowSelectMethodBeingCalledIsSaved()
		{
			workbenchWindow.SelectWindow();
			Assert.IsTrue(workbenchWindow.SelectWindowMethodCalled);
		}
	}
}
