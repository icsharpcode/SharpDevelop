// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
