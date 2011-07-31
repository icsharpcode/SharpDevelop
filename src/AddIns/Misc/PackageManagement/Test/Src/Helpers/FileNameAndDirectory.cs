// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace PackageManagement.Tests.Helpers
{
	public class FileNameAndDirectory
	{
		public FileNameAndDirectory()
		{
		}
		
		public FileNameAndDirectory(string fileName, string folder)
		{
			this.FileName = fileName;
			this.Folder = folder;
		}
		
		public string FileName;
		public string Folder;
		
		public override bool Equals(object obj)
		{
			FileNameAndDirectory rhs = obj as FileNameAndDirectory;
			if (rhs != null) {
				return (FileName == rhs.FileName) &&
					(Folder == rhs.Folder);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format("[FileNameAndDirectory FileName={0}, Folder={1}]", FileName, Folder);
		}
	}
}
