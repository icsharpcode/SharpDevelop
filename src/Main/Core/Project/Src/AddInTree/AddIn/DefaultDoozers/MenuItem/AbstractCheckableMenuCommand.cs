// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	public abstract class AbstractCheckableMenuCommand : AbstractMenuCommand, ICheckableMenuCommand
	{
		bool isChecked = false;
		
		public virtual bool IsChecked {
			get {
				return isChecked;
			}
			set {
				isChecked = value;
			}
		}
		public override void Run()
		{
			IsChecked = !IsChecked;
		}
	}
}
