// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Scripting.Tests
{
	[TestFixture]
	public class ConsoleHostFileConflictResolverTests
	{
		ConsoleHostFileConflictResolver resolver;
		PackageManagementEvents packageEvents;
		
		void CreateResolver(FileConflictAction action)
		{
			packageEvents = new PackageManagementEvents();
			resolver = new ConsoleHostFileConflictResolver(packageEvents, action);
		}
		
		FileConflictResolution ResolveConflict()
		{
			return packageEvents.OnResolveFileConflict(String.Empty);
		}
		
		[Test]
		public void ResolveConflict_FileConflictActionIsNone_ResolutionIsIgnore()
		{
			CreateResolver(FileConflictAction.None);
			
			FileConflictResolution resolution = ResolveConflict();
			
			Assert.AreEqual(FileConflictResolution.Ignore, resolution);
		}
		
		[Test]
		public void ResolveConflict_FileConflictActionIsIgnore_ResolutionIsIgnore()
		{
			CreateResolver(FileConflictAction.Ignore);
			
			FileConflictResolution resolution = ResolveConflict();
			
			Assert.AreEqual(FileConflictResolution.Ignore, resolution);
		}
		
		[Test]
		public void ResolveConflict_FileConflictActionIsOverwrite_ResolutionIsOverwrite()
		{
			CreateResolver(FileConflictAction.Overwrite);
			
			FileConflictResolution resolution = ResolveConflict();
			
			Assert.AreEqual(FileConflictResolution.Overwrite, resolution);
		}
		
		[Test]
		public void ResolveConflict_FileConflictActionIsOverwriteButResolverIsDisposed_ResolutionIsNotChangedByResolver()
		{
			CreateResolver(FileConflictAction.Overwrite);
			resolver.Dispose();
			
			FileConflictResolution resolution = ResolveConflict();
			
			Assert.AreEqual(FileConflictResolution.IgnoreAll, resolution);
		}
	}
}
