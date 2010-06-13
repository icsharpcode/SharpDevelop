// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestFileService
	{
		void OpenFile(string fileName);
		
		void JumpToFilePosition(string fileName, int line, int column);
	}
}
