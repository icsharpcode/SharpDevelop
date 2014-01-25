// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Core.ReportViewer{
		
    public class NumericToolStripTextBox:ToolStripTextBox{

		private bool allowSpace;
       
		public event EventHandler <PageNavigationEventArgs> Navigate;
		
		public  NumericToolStripTextBox () {
        }
            
		protected override void OnKeyPress(KeyPressEventArgs e){
			base.OnKeyPress(e);
			NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
			string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
			string groupSeparator = numberFormatInfo.NumberGroupSeparator;
			string negativeSign = numberFormatInfo.NegativeSign;

			string keyInput = e.KeyChar.ToString();

			if (Char.IsDigit(e.KeyChar))
			{
				// Digits are OK
			}
			else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
			         keyInput.Equals(negativeSign))
			{
				// Decimal separator is OK
			}
			else if (e.KeyChar == '\b')
			{
				// Backspace key is OK
			}
			//    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
			//    {
			//     // Let the edit control handle control and alt key combinations
			//    }
			//test
			else if(e.KeyChar == (char)Keys.Return) {
				this.HandleNavigate();
			}
			else if (this.allowSpace && e.KeyChar == ' ')
			{

			}
			else
			{
				// Swallow this invalid key and beep
				e.Handled = true;
				//    MessageBeep
			}
		}
            
		
		protected override void OnLeave(EventArgs e){
			base.OnLeave(e);
			HandleNavigate();
		}
		
		private void HandleNavigate() {
			if (!String.IsNullOrEmpty(this.Text)) {
				if (this.Navigate != null) {
					int i = Convert.ToInt32(this.Text,CultureInfo.CurrentCulture);
					PageNavigationEventArgs pne = new PageNavigationEventArgs(i);
					EventHelper.Raise <PageNavigationEventArgs>(Navigate,this,pne);
				}
			}
		}
			
		[Browsable(true),
		 Category("Misc"),
		 Description("Enter spaces between numbers")]
      
		public bool AllowSpace {
			get { return allowSpace; }
			set { allowSpace = value; }
		}
       
    }
}
