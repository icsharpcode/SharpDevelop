// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class ConsoleHostFileConflictResolver : IConsoleHostFileConflictResolver
	{
		IPackageManagementEvents packageEvents;
		FileConflictResolution conflictResolution;
		
		public ConsoleHostFileConflictResolver(
			IPackageManagementEvents packageEvents,
			FileConflictAction fileConflictAction)
		{
			this.packageEvents = packageEvents;
			
			conflictResolution = GetFileConflictResolution(fileConflictAction);
			packageEvents.ResolveFileConflict += ResolveFileConflict;
		}
		
		void ResolveFileConflict(object sender, ResolveFileConflictEventArgs e)
		{
			e.Resolution = conflictResolution;
		}
		
		FileConflictResolution GetFileConflictResolution(FileConflictAction fileConflictAction)
		{
			switch (fileConflictAction) {
				case FileConflictAction.Overwrite:
					return FileConflictResolution.Overwrite;
				default:
					return FileConflictResolution.Ignore;
			}
		}
		
		public void Dispose()
		{
			packageEvents.ResolveFileConflict -= ResolveFileConflict;
		}
	}
}
