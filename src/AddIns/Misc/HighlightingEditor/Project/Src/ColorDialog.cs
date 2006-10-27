// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui 
{

	public class ColorPaletteDialog : Form
	{		
		byte max = 40;	
		ColorPanel[] panel = new ColorPanel[40];	
		
		Color[] color = new Color[40]
		{
			//row 1
			Color.FromArgb(0,0,0), Color.FromArgb(153,51,0), Color.FromArgb(51,51,0), Color.FromArgb(0,51,0),
			Color.FromArgb(0,51,102), Color.FromArgb(0,0,128), Color.FromArgb(51,51,153), Color.FromArgb(51,51,51),
			
			//row 2
			Color.FromArgb(128,0,0), Color.FromArgb(255,102,0), Color.FromArgb(128,128,0), Color.FromArgb(0,128,0),
			Color.FromArgb(0,128,128), Color.FromArgb(0,0,255), Color.FromArgb(102,102,153), Color.FromArgb(128,128,128),
			
			//row 3
			Color.FromArgb(255,0,0), Color.FromArgb(255,153,0), Color.FromArgb(153,204,0), Color.FromArgb(51,153,102),
			Color.FromArgb(51,204,204), Color.FromArgb(51,102,255), Color.FromArgb(128,0,128), Color.FromArgb(153,153,153),
			
			//row 4
			Color.FromArgb(255,0,255), Color.FromArgb(255,204,0), Color.FromArgb(255,255,0), Color.FromArgb(0,255,0),
			Color.FromArgb(0,255,255), Color.FromArgb(0,204,255), Color.FromArgb(153,51,102), Color.FromArgb(192,192,192),
			
			//row 5
			Color.FromArgb(255,153,204), Color.FromArgb(255,204,153), Color.FromArgb(255,255,153), Color.FromArgb(204,255,204),
			Color.FromArgb(204,255,255), Color.FromArgb(153,204,255), Color.FromArgb(204,153,255), Color.FromArgb(255,255,255)						
		};	
		
		string[] colorName = new string[40]
		{
			"Black", "Brown", "Olive Green", "Dark Green", "Dark Teal", "Dark Blue", "Indigo", "Gray-80%",
			"Dark Red", "Orange", "Dark Yellow", "Green", "Teal", "Blue", "Blue-Gray", "Gray-50%",
			"Red", "Light Orange", "Lime", "Sea Green", "Aqua", "Light Blue", "Violet", "Gray-40%",
			"Pink", "Gold", "Yellow", "Bright Green", "Turquoise", "Sky Blue", "Plum", "Gray-25%",
			"Rose", "Tan", "Light Yellow", "Light Green", "Light Turquoise", "Pale Blue", "Lavender", "White"
		};	
		
		Button moreColorsButton = new Button();
		Button cancelButton = new Button();
		Color selectedColor;
		
		public ColorPaletteDialog(int x, int y)
		{
			Size = new Size(158, 158);					
			FormBorderStyle = FormBorderStyle.FixedDialog;	
			MinimizeBox = MaximizeBox = ControlBox = false;				
			ShowInTaskbar = false;					
			CenterToScreen();	
			Location = new Point(x, y);	
			
			BuildPalette();						
	
			moreColorsButton.Text = "More colors ...";			
			moreColorsButton.Size = new Size(142, 22);
			moreColorsButton.Location = new Point(5, 99);		
			moreColorsButton.Click += new EventHandler(moreColorsButton_Click);		
			moreColorsButton.FlatStyle = FlatStyle.Popup;
			Controls.Add(moreColorsButton);	
			
			//"invisible" button to cancel at Escape
			cancelButton.Text = "Cancel";
			cancelButton.Size = new Size(142, 22);
			cancelButton.Location = new Point(5, 125);		
			cancelButton.FlatStyle = FlatStyle.Popup;
			cancelButton.Click += new EventHandler(cancelButton_Click);				
			Controls.Add(cancelButton);	
			cancelButton.DialogResult = DialogResult.Cancel;
			this.CancelButton = cancelButton;
			
			moreColorsButton.TabIndex = 0;
		}
		
		public Color Color
		{
			get { return selectedColor; }
		}
		
		void BuildPalette()
		{
			byte pwidth = 16;
			byte pheight = 16;
			byte pdistance = 2;		
			byte border = 5;		
			int x = border, y = border;	
			ToolTip toolTip = new ToolTip();		
			
			for(int i = 0; i < max; i++)
			{
				panel[i] = new ColorPanel();			
				panel[i].Height = pwidth;
				panel[i].Width = pheight;			
				panel[i].Location = new Point(x, y);
				toolTip.SetToolTip(panel[i], colorName[i]);
						
				this.Controls.Add(panel[i]);			
				
				if(x < ( 7 * (pwidth + pdistance)))
					x += pwidth + pdistance;
				else
				{
					x = border;
					y += pheight + pdistance;
				}
				
				panel[i].BackColor = color[i];
				panel[i].MouseUp += new MouseEventHandler(OnPanelMouseUp);
			}
		}
		
		void OnPanelMouseUp(object sender, MouseEventArgs e)
		{
			Panel panel = (Panel)sender;
			selectedColor = panel.BackColor;
			DialogResult = DialogResult.OK;
			Close();
		}
		
		void moreColorsButton_Click(object sender, System.EventArgs e)
		{
			using (ColorDialog colDialog = new ColorDialog()) {
				colDialog.FullOpen = true;
				if (colDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					selectedColor = colDialog.Color;
					DialogResult = DialogResult.OK;
				}
			}
			
			Close();
		}
		
		void cancelButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		
		internal class ColorPanel : Panel
		{
			protected override void OnMouseEnter(EventArgs e)
			{
				base.OnMouseEnter(e);
				Draw(1, CreateGraphics());
			}
			
			protected override void OnMouseLeave(EventArgs e)
			{	
				base.OnMouseLeave(e);
				Draw(0, CreateGraphics());	
			}	
			
			protected override void OnMouseDown(MouseEventArgs e)
			{	
				base.OnMouseDown(e);
				Draw(2, CreateGraphics());				
			}
			
			protected override void OnPaint(PaintEventArgs e)
			{					
				Draw(0, e.Graphics);		
			} 
		
			void Draw(byte state, Graphics g)
			{		
				Pen pen1, pen2;
				
				if(state == 1) 		//mouse over
				{
					pen1 = new Pen( SystemColors.ControlLightLight ); 				
					pen2 = new Pen( SystemColors. ControlDarkDark);		
				}
				else if(state == 2)	//clicked
				{
					pen1 = new Pen( SystemColors.ControlDarkDark ); 				
					pen2 = new Pen( SystemColors.ControlLightLight );						
				}
				else				//neutral
				{
					pen1 = new Pen( SystemColors.ControlDark ); 				
					pen2 = new Pen( SystemColors.ControlDark );
					
				}	
				
				Rectangle r = ClientRectangle;
				Point p1 = new Point( r.Left, r.Top ); 				//top left
				Point p2 = new Point( r.Right -1, r.Top );			//top right
				Point p3 = new Point( r.Left, r.Bottom -1 );		//bottom left
				Point p4 = new Point( r.Right -1, r.Bottom -1 );	//bottom right
				
				g.DrawLine( pen1, p1, p2 ); 		
				g.DrawLine( pen1, p1, p3 ); 		
				g.DrawLine( pen2, p2, p4 ); 		
				g.DrawLine( pen2, p3, p4 ); 				
			}
			
		}
	}

}
