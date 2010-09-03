// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockSaveAllFilesCommand : IUnitTestSaveAllFilesCommand
	{
		bool saveAllFilesMethodCalled;
		
		public bool IsSaveAllFilesMethodCalled {
			get { return saveAllFilesMethodCalled; }
			set { saveAllFilesMethodCalled = value; }
		}
		
		public void SaveAllFiles()
		{
			saveAllFilesMethodCalled = true;
		}
	}
}
