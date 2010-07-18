// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.CodeCoverage;

namespace ICSharpCode.CodeCoverage.Tests.Utils
{
	public class MockFileSystem : IFileSystem
	{
		public bool FileExistsReturnValue;
		public string FileExistsPathParameter;
		public string DeleteFilePathParameter;
		public bool DirectoryExistsReturnValue;
		public string DirectoryExistsPathParameter;
		public string CreateDirectoryPathParameter;		
		public TextReader CreateTextReaderReturnValue;
		public string CreateTextReaderPathParameter;
		
		public bool FileExists(string path)
		{
			FileExistsPathParameter = path;
			return FileExistsReturnValue;
		}
		
		public void DeleteFile(string path)
		{
			DeleteFilePathParameter = path;
		}
		
		public bool DirectoryExists(string path)
		{
			DirectoryExistsPathParameter = path;
			return DirectoryExistsReturnValue;
		}
		
		public void CreateDirectory(string path)
		{
			CreateDirectoryPathParameter = path;
		}
		
		public TextReader CreateTextReader(string path)
		{
			CreateTextReaderPathParameter = path;
			return CreateTextReaderReturnValue;
		}
	}
}
