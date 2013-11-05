// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Model
{
	public class PackageListDownloadEndedEventArgs : EventArgs
	{
		public PackageListDownloadEndedEventArgs(bool wasSuccessful, bool wasCancelled)
		{
			this.WasSuccessful = wasSuccessful;
			this.WasCancelled = wasCancelled;
		}
		
		public bool WasSuccessful
		{
			get;
			set;
		}
		
		public bool WasCancelled
		{
			get;
			set;
		}
	}
}
