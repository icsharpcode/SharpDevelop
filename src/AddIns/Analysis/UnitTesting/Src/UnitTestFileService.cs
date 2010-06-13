// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestFileService : IUnitTestFileService
	{
		public void OpenFile(string fileName)
		{
			FileService.OpenFile(fileName);
		}
		
		public void JumpToFilePosition(string fileName, int line, int column)
		{
			FileService.JumpToFilePosition(fileName, line, column);
		}
	}
}
