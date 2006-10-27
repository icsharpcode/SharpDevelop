// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace NoGoop.Controls
{
	public class NumericTextBox : TextBox
	{
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.Style |= 0x2000; // ES_NUMBER
				return cp;
			}
		}
	}
}
