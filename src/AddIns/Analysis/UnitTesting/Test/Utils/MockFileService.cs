// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockFileService : IUnitTestFileService
	{
		public string FileNamePassedToFileExists;
		public bool FileExistsReturnValue = true;
		
		string fileOpened;
		FilePosition? filePositionJumpedTo;
		
		public string FileOpened {
			get { return fileOpened; }
		}
		
		public FilePosition? FilePositionJumpedTo {
			get { return filePositionJumpedTo; }
		}
		
		public void OpenFile(string fileName)
		{
			fileOpened = fileName;
		}
		
		public void JumpToFilePosition(string fileName, int line, int column)
		{
			filePositionJumpedTo = new FilePosition(fileName, line, column);
		}
		
		public bool FileExists(string fileName)
		{
			FileNamePassedToFileExists = fileName;
			return FileExistsReturnValue;
		}
	}
}
