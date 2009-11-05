// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	public interface IFileSystem
	{
		bool FileExists(string fileName);
		void DeleteFile(string fileName);
		void CopyFile(string source, string destination);
		void CreateDirectory(string path);
		bool DirectoryExists(string path);
		string[] GetFilesInDirectory(string path, string searchPattern);
	}
}
