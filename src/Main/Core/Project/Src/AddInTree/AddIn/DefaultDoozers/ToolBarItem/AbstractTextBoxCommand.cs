// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	public abstract class AbstractTextBoxCommand : AbstractCommand, ITextBoxCommand
	{
		bool isEnabled = true;
		
		public virtual bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
			}
		}
		
		public override void Run()
		{
			
		}
	}
}
