
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomPad
{
	public class MyCustomPad : AbstractPadContent
	{
		Panel panel     = new Panel();
		Label testLabel = new Label();
		
		public MyCustomPad()
		{
			testLabel.Text     = "Hello World!";
			testLabel.Location = new Point(8, 8);
			panel.Controls.Add(testLabel);
		}
				
		public override Control Control {
			get {
				return panel;
			}
		}
	}
}
