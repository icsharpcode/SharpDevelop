// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.CodeCoverage
{
	public interface IFileSystem : ICSharpCode.UnitTesting.IFileSystem
	{
		void DeleteFile(string path);
		
		bool DirectoryExists(string path);
		void CreateDirectory(string path);
		
		TextReader CreateTextReader(string path);
	}
}
