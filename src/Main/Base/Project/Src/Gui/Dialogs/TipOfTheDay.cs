// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class TipOfTheDayView : UserControl
	{
		readonly int ICON_DISTANCE = 16;
		Bitmap   icon      = null;
		Font     titlefont;
		Font     textfont;
		string[] tips;
		int      curtip = 0;
		
		string didyouknowtext;
		
		
		public TipOfTheDayView(XmlElement el)
		{
			titlefont = ResourceService.LoadFont("Times new Roman", 15, FontStyle.Bold);
			textfont  = ResourceService.LoadFont("Times new Roman", 12);
			
			this.didyouknowtext = ResourceService.GetString("Dialog.TipOfTheDay.DidYouKnowText");
			
			icon   = ResourceService.GetBitmap("Icons.TipOfTheDayIcon");
			
			//			XmlNodeList nodes = el.GetElementsByTagName("TIP");
			XmlNodeList nodes = el.ChildNodes;
			
			
			tips = new string[nodes.Count];
			for (int i = 0; i < nodes.Count; ++i) {
				tips[i] = StringParser.Parse(nodes[i].InnerText);
			}
			
			curtip = (new Random().Next()) % nodes.Count;
			
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			
			g.FillRectangle(Brushes.Gray, 0,
			                0,
			                icon.Width + ICON_DISTANCE,
			                Height);
			g.FillRectangle(Brushes.White, 0 + icon.Width + ICON_DISTANCE,
			                0,
			                Width - icon.Width - ICON_DISTANCE,
			                Height);
			g.DrawImage(icon, 2 + ICON_DISTANCE / 2, 4);
			
			g.DrawString(didyouknowtext, titlefont, Brushes.Black, icon.Width + ICON_DISTANCE + 4, 8);
			
			g.DrawLine(Pens.Black, new Point(icon.Width + ICON_DISTANCE, 8 + titlefont.Height + 2),
			           new Point(Width, 8 + titlefont.Height + 2));
			drawrect = new Rectangle(icon.Width + ICON_DISTANCE, 8 + titlefont.Height + 6,
			                         Width - icon.Width - ICON_DISTANCE, Height - (8 + titlefont.Height + 6));
			
			g.DrawString(tips[curtip], textfont, Brushes.Black, drawrect);
		}
		Rectangle drawrect;
		
		public void NextTip()
		{
			curtip = (curtip + 1) % tips.Length;
			Invalidate(drawrect);
			Update();
		}
	}
	
	public class TipOfTheDayDialog : Form
	{
		CheckBox viewTipsAtStartCheckBox;
		Button   closeButton;
		Button   nextTipButton;
		
		Panel panel = new Panel();
		TipOfTheDayView tipview;
		
		
		
		void NextTip(object sender, EventArgs e)
		{
			tipview.NextTip();
		}
		
		void CheckChange(object sender, EventArgs e)
		{
			PropertyService.Set("ShowTipsAtStartup", viewTipsAtStartCheckBox.Checked);
		}
		
		public TipOfTheDayDialog()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			
			Icon = null;
			
			
			
			XmlDocument doc = new XmlDocument();
			doc.Load(PropertyService.DataDirectory +
			         Path.DirectorySeparatorChar + "options" +
			         Path.DirectorySeparatorChar + "TipsOfTheDay.xml" );
			
			tipview = new TipOfTheDayView(doc.DocumentElement);
			panel.Controls.Add(tipview);
			//			panel.FormBorderStyle = FormBorderStyle.Fixed3D;
			Controls.Add(panel);
			
			panel.Width  = tipview.Width  = Width - 24;
			panel.Height = tipview.Height = nextTipButton.Top - 15;
			panel.Location = new Point(8, 5);
			nextTipButton.Click += new EventHandler(NextTip);
			
			viewTipsAtStartCheckBox.CheckedChanged += new EventHandler(CheckChange);
			viewTipsAtStartCheckBox.Checked = PropertyService.Get("ShowTipsAtStartup", true);
			
			MaximizeBox  = MinimizeBox = false;
			ShowInTaskbar = false;
		}
		
		void ExitDialog(object sender, EventArgs e)
		{
			Close();
			Dispose();
		}
		
		private void InitializeComponent()
		{
			this.closeButton = new Button();
			this.viewTipsAtStartCheckBox = new CheckBox();
			this.nextTipButton = new Button();
			
			closeButton.Location = new System.Drawing.Point(328, 232);
			closeButton.Click  += new EventHandler(ExitDialog);
			closeButton.Size = new System.Drawing.Size(80, 24);
			closeButton.TabIndex = 1;
			closeButton.Text = ResourceService.GetString("Global.CloseButtonText");
			closeButton.FlatStyle = FlatStyle.System;
			
			viewTipsAtStartCheckBox.Location = new System.Drawing.Point(8, 232);
			viewTipsAtStartCheckBox.Text = ResourceService.GetString("Dialog.TipOfTheDay.checkBox1Text");
			viewTipsAtStartCheckBox.Size = new System.Drawing.Size(210, 24);
			//			viewTipsAtStartCheckBox.AccessibleRole = AccessibleRoles.CheckButton;
			viewTipsAtStartCheckBox.TabIndex = 2;
			viewTipsAtStartCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			viewTipsAtStartCheckBox.FlatStyle = FlatStyle.System;
			
			this.Text = ResourceService.GetString("Dialog.TipOfTheDay.DialogName");
			//@design this.TrayLargeIcon = true;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			//@design this.TrayHeight = 0;
			this.ClientSize = new System.Drawing.Size(418, 263);
			
			nextTipButton.Location = new System.Drawing.Point(240 - 16, 232);
			nextTipButton.Size = new System.Drawing.Size(96, 24);
			nextTipButton.TabIndex = 0;
			nextTipButton.Text = ResourceService.GetString("Dialog.TipOfTheDay.button1Text");
			nextTipButton.FlatStyle = FlatStyle.System;
			
			this.Controls.Add(viewTipsAtStartCheckBox);
			this.Controls.Add(closeButton);
			this.Controls.Add(nextTipButton);
		}
	}
}
