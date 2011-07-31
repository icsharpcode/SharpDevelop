// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementOutputMessagesViewTests
	{
		PackageManagementOutputMessagesView view;
		FakeCompilerMessageView fakeCompilerMessageView;
		FakeMessageCategoryView fakeMessageCategoryView;
		PackageManagementEvents packageManagementEvents;
		
		void CreateView()
		{
			CreateCompilerMessageView();
			CreateView(fakeCompilerMessageView);
		}
		
		void CreateCompilerMessageView()
		{
			fakeCompilerMessageView = new FakeCompilerMessageView();
			fakeMessageCategoryView = fakeCompilerMessageView.FakeMessageCategoryView;
		}
		
		void CreateView(FakeCompilerMessageView fakeCompilerMessageView)
		{
			packageManagementEvents = new PackageManagementEvents();
			view = new PackageManagementOutputMessagesView(fakeCompilerMessageView, packageManagementEvents);
		}
		
		void LogInfoMessage(string message)
		{
			packageManagementEvents.OnPackageOperationMessageLogged(MessageLevel.Info, message);
		}
		
		[Test]
		public void Log_InfoMessage_CreatesMessageViewCategoryForPackageManagement()
		{
			CreateView();
			LogInfoMessage("Test");
			
			string expectedCategryName = PackageManagementOutputMessagesView.CategoryName;
			string actualCategoryName = fakeCompilerMessageView.FirstMessageViewCategoryCreated;
			
			Assert.AreEqual(expectedCategryName, actualCategoryName);
		}
		
		[Test]
		public void Log_InfoMessage_MessageLoggedToMessageCategoryView()
		{
			CreateView();
			LogInfoMessage("Test");
			
			Assert.AreEqual("Test", fakeMessageCategoryView.FirstLineAppended);
		}
		
		[Test]
		public void Clear_AttemptingToClearMessages_MessagesClearedFromMessageViewCategory()
		{
			CreateView();
			view.Clear();
			
			Assert.IsTrue(fakeMessageCategoryView.IsClearCalled);
		}
		
		[Test]
		public void Constructor_MessageViewCategoryAlreadyCreated_MessageViewCategoryNotCreatedAgain()
		{
			CreateCompilerMessageView();
			fakeCompilerMessageView.GetExistingReturnValue = new FakeMessageCategoryView();
			CreateView(fakeCompilerMessageView);
			
			Assert.AreEqual(0, fakeCompilerMessageView.MessageViewCategoriesCreated.Count);
		}
		
		[Test]
		public void Constructor_MessageViewCategoryAlreadyCreated_PackageManagementMessageCategoryNameCheckedToSeeIfItExists()
		{
			CreateCompilerMessageView();
			fakeCompilerMessageView.GetExistingReturnValue = new FakeMessageCategoryView();
			CreateView(fakeCompilerMessageView);
			
			string expectedCategryName = PackageManagementOutputMessagesView.CategoryName;
			string actualCategoryName = fakeCompilerMessageView.CategoryNamePassedToCategoryExists;
			
			Assert.AreEqual(expectedCategryName, actualCategoryName);
		}
		
		[Test]
		public void OnPackageOperationMessageLogged_InfoMessageUsingFormatStringPassed_FullyFormattedStringAddedToMessageViewCategory()
		{
			CreateView();
			
			packageManagementEvents.OnPackageOperationMessageLogged(MessageLevel.Info, "Test {0}", 1);
			
			Assert.AreEqual("Test 1", fakeMessageCategoryView.FirstLineAppended);
		}
	}
}
