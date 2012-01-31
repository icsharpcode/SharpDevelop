// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class AddMvcViewToProjectCommandTests
	{
		TestableAddMvcViewToProjectCommand command;
		FakeAddMvcItemToProjectView fakeView;
		
		void CreateCommand()
		{
			command = new TestableAddMvcViewToProjectCommand();
			fakeView = command.FakeAddMvcViewToProjectView;
		}
		
		[Test]
		public void Run_CommandExecuted_ViewIsDisplayed()
		{
			CreateCommand();
			command.Run();
			
			Assert.IsTrue(fakeView.IsShowDialogCalled);
		}
		
		[Test]
		public void Run_CommandExecuted_ViewDataContextIsSet()
		{
			CreateCommand();
			object expectedDataContext = new object();
			command.FakeDataContext = expectedDataContext;
			command.Run();
			
			object dataContext = fakeView.DataContext;
			
			Assert.AreEqual(expectedDataContext, dataContext);
		}
	}
}
