// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ScrollBox : UserControl
	{
		string[] text;
		int[]    textHeights;
		
		Image  image;
		Timer  timer;
		int    scroll = -220;
		
		public int ScrollY {
			get {
				return scroll;
			}
			set {
				scroll = value;
			}
		}
		
		public Image Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				timer.Stop();
				foreach (Control ctrl in Controls) {
					ctrl.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		public ScrollBox()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			Image = IconService.GetBitmap("Icons.AboutImage");
			
			Font = WinFormsResourceService.LoadFont("Tahoma", 10);
			text = new string[] {
				"\"The most successful method of programming is to begin a program as simply as possible, test it, and then add to the program until it performs the required job.\"\n    -- PDP8 handbook, Pg 9-64",
				"\"The primary purpose of the DATA statement is to give names to constants; instead of referring to pi as 3.141592653589793 at every\n appearance, the variable PI can be given that value with a DATA statement and used instead of the longer form of the constant. This also simplifies modifying the program, should the value of pi change.\"\n    -- FORTRAN manual for Xerox computers",
				"\"No proper program contains an indication which as an operator-applied occurrence identifies an operator-defining occurrence which as an indication-applied occurrence identifies an indication-defining occurrence different from the one identified by the given indication as an indication- applied occurrence.\"\n   -- ALGOL 68 Report",
				"\"The '#pragma' command is specified in the ANSI standard to have an arbitrary implementation-defined effect. In the GNU C preprocessor, `#pragma' first attempts to run the game rogue; if that fails, it tries to run the game hack; if that fails, it tries to run GNU Emacs displaying the Tower of Hanoi; if that fails, it reports a fatal error. In any case, preprocessing does not continue.\"\n   --From an old GNU C Preprocessor document",
				"\"There are two ways of constructing a software design: one way is to make it so simple that there are obviously no deficiencies; the other is to make it so complicated that there are no obvious deficiencies.\"\n    -- C.A.R. Hoare",
				"On two occasions, I have been asked [by members of Parliament], 'Pray, Mr. Babbage, if you put into the machine wrong figures, will the right answers come out?' I am not able to rightly apprehend the kind of confusion of ideas that could provoke such a question.\"\n   -- Charles Babbage (1791-1871)"
			};
			
			// randomize the order in which the texts are displayed
			Random rnd = new Random();
			for (int i = 0; i < text.Length; i++) {
				Swap(ref text[i], ref text[rnd.Next(i, text.Length)]);
			}
			
			timer = new Timer();
			timer.Interval = 40;
			timer.Tick += new EventHandler(ScrollDown);
			timer.Start();
		}
		
		void Swap(ref string a, ref string b)
		{
			string c = a;
			a = b;
			b = c;
		}
		
		void ScrollDown(object sender, EventArgs e)
		{
			++scroll;
			Refresh();
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			if (image != null) {
				pe.Graphics.DrawImage(image, 0, 0, Width, Height);
			}
		}
		int curText = 0;
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			if (textHeights == null) {
				textHeights = new int[text.Length];
				for (int i = 0; i < text.Length; ++i) {
					textHeights[i] = (int)g.MeasureString(text[i], Font, new SizeF(Width / 2, Height * 2)).Height;
				}
			}
			g.DrawString(text[curText],
			             Font,
			             Brushes.Black,
			             new Rectangle(Width / 2, 0 - scroll, Width / 2, Height * 2));
			
			if (scroll > textHeights[curText]) {
				curText = (curText + 1) % text.Length;
				scroll = -textHeights[curText] - Height;
			}
		}
	}
	
	public class CommonAboutDialog : XmlForm
	{
		public ScrollBox ScrollBox {
			get {
				return (ScrollBox)ControlDictionary["aboutPictureScrollBox"];
			}
		}
		
		public CommonAboutDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.CommonAboutDialog.xfrm"));
			var aca = (AssemblyCopyrightAttribute)typeof(CommonAboutDialog).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
			ControlDictionary["copyrightLabel"].Text = "Copyright " + aca.Copyright;
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
		}
	}
}
