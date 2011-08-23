// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageOperationMessageTests
	{
		[Test]
		public void Level_CreateInfoMessage_CreatesMessageWithMessageLevelSetToInfo()
		{
			var message = new PackageOperationMessage(MessageLevel.Info, "test");
			
			Assert.AreEqual(MessageLevel.Info, message.Level);
		}
		
		[Test]
		public void Level_CreateWarningMessage_CreatesMessageWithMessageLevelSetToWarning()
		{
			var message = new PackageOperationMessage(MessageLevel.Warning, "test");
			
			Assert.AreEqual(MessageLevel.Warning, message.Level);
		}
		
		[Test]
		public void ToString_CreateWarningMessage_ReturnsMessage()
		{
			var message = new PackageOperationMessage(MessageLevel.Warning, "test");
			var text = message.ToString();
			
			Assert.AreEqual("test", text);
		}
		
		[Test]
		public void ToString_CreateFormattedWarningMessage_ReturnsFormattedMessage()
		{
			string format = "Test '{0}'.";
			var message = new PackageOperationMessage(MessageLevel.Warning, format, "A");
			var text = message.ToString();
			
			var expectedText = "Test 'A'.";
			Assert.AreEqual(expectedText, text);
		}
	}
}
