// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ResolveFileConflictEventArgs : EventArgs
	{
		public ResolveFileConflictEventArgs(string message)
		{
			this.Message = message;
			this.Resolution = FileConflictResolution.Ignore;
		}
		
		public string Message { get; private set; }
		public FileConflictResolution Resolution { get; set; }
	}
}
