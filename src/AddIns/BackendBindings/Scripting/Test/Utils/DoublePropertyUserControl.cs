// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class DoublePropertyUserControl : UserControl
	{
		double doubleValue = -1.1;
			
		public DoublePropertyUserControl()
		{
		}
		
		public double DoubleValue {
			get { return doubleValue; }
			set { doubleValue = value; }
		}		
	}
}
