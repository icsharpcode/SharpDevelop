// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// EventArgs for local AddIn installation events.
	/// </summary>
	public class AddInInstallationEventArgs : EventArgs
	{
		public AddInInstallationEventArgs(AddIn addIn)
		{
			AddIn = addIn;
			PreviousVersionRemains = false;
		}
		
		public AddIn AddIn
		{
			get;
			private set;
		}
		
		public bool PreviousVersionRemains
		{
			get;
			set;
		}
	}
}
