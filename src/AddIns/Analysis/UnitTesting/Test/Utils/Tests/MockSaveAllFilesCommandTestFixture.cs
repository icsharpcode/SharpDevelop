// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
