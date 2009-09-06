// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace PythonBinding.Tests.Utils
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
