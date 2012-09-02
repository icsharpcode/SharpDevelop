// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.IconEditor
{
	public partial class PickFormatDialog : Form
	{
		public PickFormatDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			colorDepthComboBox.SelectedIndex = Array.IndexOf(colorDepths, 32); // 32-bit is default
		}
		
		public int IconWidth {
			get { return (int)widthUpDown.Value; }
		}
		
		public int IconHeight {
			get { return (int)heightUpDown.Value; }
		}
		
		int[] colorDepths = { 1, 4, 8, 24, 32 };
		
		public int ColorDepth {
			get { return colorDepths[colorDepthComboBox.SelectedIndex]; }
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			Close();
		}
	}
}
