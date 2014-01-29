// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.PackageManagement
{
	public class ServiceWithWorkbenchOwner
	{
		Window owner;
		
		public Window Owner {
			get {
				if (owner == null) {
					owner = SD.Workbench.MainWindow;
				}
				return owner;
			}
			set { owner = value; }
		}
	}
}
