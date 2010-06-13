// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
