// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.PackageManagement
{
	[Serializable]
	public class FileExistsException : Exception
	{
		public FileExistsException(string fileName)
			: base(GetErrorMessage(fileName))
		{
		}
		
		static string GetErrorMessage(string fileName)
		{
			fileName = Path.GetFileName(fileName);
			string message = "Unable to add '{0}'. A file with that name already exists.";
			return String.Format(message, fileName);
		}
	}
}
