// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Data for events indicating an exception during package-related operations.
	/// </summary>
	public class AddInExceptionEventArgs : EventArgs
	{
		public AddInExceptionEventArgs(Exception exception)
		{
			Exception = exception;
		}
		
		public Exception Exception
		{
			get;
			private set;
		}
	}
}
