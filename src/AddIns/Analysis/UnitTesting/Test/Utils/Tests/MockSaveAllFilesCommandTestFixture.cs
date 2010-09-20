// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockSaveAllFilesCommandTestFixture
	{
		MockSaveAllFilesCommand saveAllFilesCommand;
		
		[SetUp]
		public void Init()
		{
			saveAllFilesCommand = new MockSaveAllFilesCommand();
		}
		
		[Test]
		public void IsSaveAllFilesMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(saveAllFilesCommand.IsSaveAllFilesMethodCalled);
		}
		
		[Test]
		public void IsSaveAllFilesMethodCalledReturnsTrueAfterMethodIsCalled()
		{
			saveAllFilesCommand.SaveAllFiles();
			Assert.IsTrue(saveAllFilesCommand.IsSaveAllFilesMethodCalled);
		}
	}
}
