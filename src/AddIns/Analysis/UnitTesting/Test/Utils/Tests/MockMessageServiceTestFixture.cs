// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockMessageServiceTestFixture
	{
		MockMessageService messageService;
		
		[SetUp]
		public void Init()
		{
			messageService = new MockMessageService();
		}
		
		[Test]
		public void CaptionIsNullByDefault()
		{
			Assert.IsNull(messageService.CaptionPassedToAskQuestion);
		}
		
		[Test]
		public void QuestionIsNullByDefault()
		{
			Assert.IsNull(messageService.QuestionPassedToAskQuestion);
		}
		
		[Test]
		public void QuestionIsSavedAfterAskQuestionMethodIsCalled()
		{
			string expectedQuestion = "question";
			messageService.AskQuestion(expectedQuestion, null);
			
			Assert.AreEqual(expectedQuestion, messageService.QuestionPassedToAskQuestion);
		}
		
		[Test]
		public void CaptionIsSavedAfterAskQuestionMethodIsCalled()
		{
			string expectedCaption = "caption";
			messageService.AskQuestion(null, expectedCaption);
			
			Assert.AreEqual(expectedCaption, messageService.CaptionPassedToAskQuestion);
		}
		
		[Test]
		public void AskQuestionMethodReturnsFalseByDefault()
		{
			Assert.IsFalse(messageService.AskQuestion(null, null));
		}
		
		[Test]
		public void AskQuestionMethodReturnsTrueWhenConfigured()
		{
			messageService.AskQuestionReturnValue = true;
			Assert.IsTrue(messageService.AskQuestion(null, null));
		}
	}
}
