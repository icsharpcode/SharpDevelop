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
	public class FileSystem : IFileSystem
	{
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}
		
		public void DeleteFile(string path)
		{
			File.Delete(path);
		}
		
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}
		
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}
		
		public TextReader CreateTextReader(string path)
		{
			return new StreamReader(path);
		}
	}
}
