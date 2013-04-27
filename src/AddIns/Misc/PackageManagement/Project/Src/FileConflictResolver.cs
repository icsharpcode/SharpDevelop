// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class FileConflictResolver : IFileConflictResolver
	{
		string[] buttons = new string[] { "Yes", "Yes to All", "No", "No to All" };
		
		const int YesButtonIndex = 0;
		const int YesToAllButtonIndex = 1;
		const int NoButtonIndex = 2;
		const int NoToAllButtonIndex = 3;
		
		public FileConflictResolution ResolveFileConflict(string message)
		{
			int result = MessageService.ShowCustomDialog(
				"File Conflict",
				message,
				NoButtonIndex, // "No" is default accept button.
				-1,
				buttons);
			return MapResultToFileConflictResolution(result);
		}
		
		FileConflictResolution MapResultToFileConflictResolution(int result)
		{
			switch (result) {
				case YesButtonIndex:
					return FileConflictResolution.Overwrite;
				case YesToAllButtonIndex:
					return FileConflictResolution.OverwriteAll;
				case NoToAllButtonIndex:
					return FileConflictResolution.IgnoreAll;
				default:
					return FileConflictResolution.Ignore;
			}
		}
	}
}
