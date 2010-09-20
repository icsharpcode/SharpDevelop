// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class NullPropertyUserControl : UserControl
	{
		string fooBar;
		
		public string FooBar {
			get { return fooBar; }
			set { fooBar = value; }
		}
		
		public NullPropertyUserControl()
		{
		}
	}
}
