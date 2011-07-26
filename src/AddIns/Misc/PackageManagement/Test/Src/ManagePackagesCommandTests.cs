// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ManagePackagesCommandTests
	{
		TestableManagePackagesCommand command;
		FakeManagePackagesView view;
		
		void CreateCommand()
		{
			command = new TestableManagePackagesCommand();
			view = command.FakeManagePackagesView;
		}
		
		[Test]
		public void Run_CommandExecuted_ManagePackagesViewIsDisplayed()
		{
			CreateCommand();
			command.Run();
			
			Assert.IsTrue(view.IsShowDialogCalled);
		}
		
		[Test]
		public void Run_CommandExecuted_ManagePackagesViewIsDisposed()
		{
			CreateCommand();
			command.Run();
			
			Assert.IsTrue(view.IsDisposeCalled);
		}
		
		[Test]
		public void Run_CommandExecuted_OutputMessagesViewIsCleared()
		{
			CreateCommand();
			command.Run();
			
			bool result = command.FakeOutputMessagesView.IsClearCalled;
			
			Assert.IsTrue(result);
		}
	}
}
