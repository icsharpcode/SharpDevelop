// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Workbench;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

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
		
		[Test]
		public void OutputCategory_MessageViewCategoryAlreadyCreated_ReturnsOutputCategoryFromMessageViewCategory()
		{
			CreateCompilerMessageView();
			var messageCategoryView = new FakeMessageCategoryView();
			IOutputCategory expectedCategory = MockRepository.GenerateStub<IOutputCategory>();
			messageCategoryView.OutputCategory = expectedCategory;
			fakeCompilerMessageView.GetExistingReturnValue = messageCategoryView;
			CreateView(fakeCompilerMessageView);
			
			IOutputCategory outputCategory = view.OutputCategory;
			
			Assert.AreEqual(expectedCategory, outputCategory);
		}
	}
}
