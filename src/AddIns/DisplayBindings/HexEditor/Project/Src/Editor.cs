// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using HexEditor.Util;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

//using HexEditor.Commands;

namespace HexEditor
{
	/// <summary>
	/// Hexadecimal editor control.
	/// </summary>
	public partial class Editor : UserControl
	{
		// TODO : Make big files compatible (data structures are bad)

		/// <summary>
		/// number of the first visible line (first line = 0)
		/// </summary>
		int topline;
		
		public int TopLine {
			get { return topline; }
			set { topline = value; }
		}
		int charwidth, hexinputmodepos;
		int underscorewidth, underscorewidth3, fontheight;
		bool insertmode, hexinputmode, selectionmode, handled, moved;
		
		public bool Initializing { get; set; }
		
		Point oldMousePos = new Point(0,0);
		
		Rectangle[] selregion;
		Point[] selpoints;
		BufferManager buffer;
		Caret caret;
		
		public Caret Caret {
			get { return caret; }
		}
		
		SelectionManager selection;
		UndoManager undoStack;

		Panel activeView;
		
		public Panel ActiveView {
			get { return activeView; }
			set { activeView = value; }
		}
		
		private Bitmap bHeader, bSide, bHex, bText;
		private Graphics gbHeader, gbSide, gbHex, gbText,
		gHeader, gSide, gHex, gText;
		
		/// <summary>
		/// Event fired every time something is changed in the editor.
		/// </summary>
		[Browsable(true)]
		public event EventHandler DocumentChanged;
		
		/// <summary>
		/// On-method for DocumentChanged-event.
		/// </summary>
		/// <param name="e">The eventargs for the event</param>
		protected virtual void OnDocumentChanged(EventArgs e)
		{
			if (DocumentChanged != null) {
				DocumentChanged(this, e);
			}
		}
		
		/// <summary>
		/// Creates a new HexEditor Control with basic settings and initialises all components.
		/// </summary>
		public Editor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			buffer = new BufferManager(this);
			selection = new SelectionManager(ref buffer);
			undoStack = new UndoManager();
			insertmode = true;
			underscorewidth = MeasureStringWidth(this.CreateGraphics(), "_", Settings.DataFont);
			underscorewidth3 = underscorewidth * 3;
			fontheight = GetFontHeight(Settings.DataFont);
			selregion = new Rectangle[] {};
			selpoints = new Point[] {};
			headertext = GetHeaderText();
			
			this.ActiveView = this.hexView;
			
			UpdatePainters();
			
			caret = new Caret(this.gbHex, 1, fontheight, 0);
			
			HexEditSizeChanged(null, EventArgs.Empty);
			AdjustScrollBar();
			
			this.Invalidate();
		}

		#region Measure functions
		static int GetFontHeight(Font font)
		{
			int height1 = TextRenderer.MeasureText("_", font).Height;
			int height2 = (int)Math.Ceiling(font.GetHeight());
			return Math.Max(height1, height2) + 1;
		}

		static int MeasureStringWidth(Graphics g, string word, Font font)
		{
			return TextRenderer.MeasureText(g, word, font, new Size(short.MaxValue, short.MaxValue),
			                                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
			                                TextFormatFlags.PreserveGraphicsClipping).Width;
		}
		#endregion

		/// <summary>
		/// used to store headertext for calculation.
		/// </summary>
		string headertext = String.Empty;
		
		/// <summary>
		/// Used to get the arrow keys to the keydown event.
		/// </summary>
		/// <param name="keyData">The pressed keys.</param>
		/// <returns>true if keyData is an arrow key, otherwise false.</returns>
		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.Down:
				case Keys.Up:
				case Keys.Left:
				case Keys.Right:
				case Keys.Tab:
					return true;
			}
			return false;
		}

		#region Properties
		ViewMode viewMode = ViewMode.Hexadecimal;
		int bytesPerLine = 16;
		bool fitToWindowWidth;
		string fileName;
		Encoding encoding = Encoding.Default;

		/// <summary>
		/// ProgressBar used to display the progress of loading saving, outside of the control.
		/// </summary>
		/// <remarks>Currently not in use</remarks>
		private ToolStripProgressBar progressBar;
		
		/// <summary>
		/// ProgressBar used to display the progress of loading saving, outside of the control.
		/// </summary>
		/// <remarks>Currently not in use</remarks>
		public ToolStripProgressBar ProgressBar {
			get { return progressBar; }
			set { progressBar = value; }
		}
		
		/// <summary>
		/// Represents the current buffer of the editor.
		/// </summary>
		public BufferManager Buffer {
			get { return buffer; }
		}
		
		/// <summary>
		/// Offers access to the current selection
		/// </summary>
		public SelectionManager Selection {
			get { return selection; }
		}
		
		/// <summary>
		/// Represents the undo stack of the editor.
		/// </summary>
		public UndoManager UndoStack {
			get { return undoStack; }
		}
		
		new public bool Enabled {
			get { return base.Enabled; }
			set {
				if (this.InvokeRequired) {
					base.Enabled = this.VScrollBar.Enabled = this.hexView.Enabled = this.textView.Enabled = this.side.Enabled = this.header.Enabled = value;
				} else {
					base.Enabled = this.VScrollBar.Enabled = this.hexView.Enabled = this.textView.Enabled = this.side.Enabled = this.header.Enabled = value;
				}
			}
		}
		
		/// <summary>
		/// Returns the name of the currently loaded file.
		/// </summary>
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}
		
		/// <summary>
		/// Property for future use to allow user to select encoding.
		/// </summary>
		/// <remarks>Currently not in use.</remarks>
		public Encoding Encoding {
			get { return encoding; }
			set { encoding = value; }
		}
		
		/// <summary>
		/// The font used for all data displays in the hex editor.
		/// </summary>
		public Font DataFont {
			get { return Settings.DataFont; }
			set {
				Settings.DataFont = value;
				underscorewidth = MeasureStringWidth(this.CreateGraphics(), "_", value);
				underscorewidth3 = underscorewidth * 3;
				fontheight = GetFontHeight(value);
				this.Invalidate();
			}
		}
		
		/// <summary>
		/// The font used for all offset displays in the hex editor.
		/// </summary>
		public Font OffsetFont {
			get { return Settings.OffsetFont; }
			set {
				Settings.OffsetFont = value;
				underscorewidth = MeasureStringWidth(this.CreateGraphics(), "_", value);
				underscorewidth3 = underscorewidth * 3;
				fontheight = GetFontHeight(value);
				this.Invalidate();
			}
		}
		
		new public Font Font {
			get { return null; }
			set {

			}
		}
		
		/// <summary>
		/// The ViewMode used in the hex editor.
		/// </summary>
		public ViewMode ViewMode
		{
			get { return viewMode; }
			set {
				viewMode = value;
				UpdateViews();
				
				this.headertext = GetHeaderText();

				this.Invalidate();
			}
		}
		
		/// <summary>
		/// "Auto-fit width" setting.
		/// </summary>
		public bool FitToWindowWidth
		{
			get { return fitToWindowWidth; }
			set {
				fitToWindowWidth = value;
				if (value) this.BytesPerLine = CalculateMaxBytesPerLine();
			}
		}
		
		/// <summary>
		/// Gets or sets how many bytes (chars) are displayed per line.
		/// </summary>
		public int BytesPerLine
		{
			get { return bytesPerLine; }
			set {
				if (value < 1) value = 1;
				if (!Initializing && value > CalculateMaxBytesPerLine()) value = CalculateMaxBytesPerLine();
				bytesPerLine = value;
				UpdateViews();
				
				this.headertext = GetHeaderText();
				
				this.Invalidate();
			}
		}
		
		/// <summary>
		/// Generates the current header text
		/// </summary>
		/// <returns>the header text</returns>
		string GetHeaderText()
		{
			StringBuilder text = new StringBuilder();
			for (int i = 0; i < this.BytesPerLine; i++) {
				switch (this.ViewMode) {
					case ViewMode.Decimal:
						text.Append(' ', 3 - GetLength(i));
						text.Append(i.ToString());
						break;
					case ViewMode.Hexadecimal:
						text.Append(' ', 3 - string.Format("{0:X}", i).Length);
						text.AppendFormat("{0:X}", i);
						break;
					case ViewMode.Octal:
						int tmp = i;
						string num = "";
						if (tmp == 0) num = "0";
						while (tmp != 0)
						{
							num = (tmp % 8).ToString() + num;
							tmp = (int)(tmp / 8);
						}
						text.Append(' ', 3 - num.Length);
						text.Append(num);
						break;
				}
			}
			
			return text.ToString();
		}
		#endregion
		
		#region MouseActions/Focus/ScrollBar
		/// <summary>
		/// Used to update the scrollbar.
		/// </summary>
		void AdjustScrollBar()
		{
			int linecount = this.GetMaxLines();
			
			if (linecount > GetMaxVisibleLines()) {
				// Set Vertical scrollbar
				VScrollBar.Enabled = true;
				VScrollBar.Maximum = linecount - 1;
				VScrollBar.Minimum = 0;
			} else {
				VScrollBar.Value = 0;
				VScrollBar.Enabled = false;
			}
		}
		
		/// <summary>
		/// Handles the vertical scrollbar.
		/// </summary>
		void VScrollBarScroll(object sender, ScrollEventArgs e)
		{
			UpdateViews();
			this.topline = VScrollBar.Value;
			Point pos = GetPositionForOffset(caret.Offset, charwidth);
			caret.SetToPosition(pos);
			
			this.Invalidate();
		}
		
		/// <summary>
		/// Handles the mouse wheel
		/// </summary>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (!this.VScrollBar.Enabled) return;

			int delta = -(e.Delta / 3 / 10);
			int oldvalue = 0;

			if ((VScrollBar.Value + delta) > VScrollBar.Maximum) {
				oldvalue = VScrollBar.Value;
				VScrollBar.Value = VScrollBar.Maximum;
				this.VScrollBarScroll(null, new ScrollEventArgs(ScrollEventType.Last, oldvalue, VScrollBar.Value, ScrollOrientation.VerticalScroll));
			} else if ((VScrollBar.Value + delta) < VScrollBar.Minimum) {
				oldvalue = VScrollBar.Value;
				VScrollBar.Value = 0;
				this.VScrollBarScroll(null, new ScrollEventArgs(ScrollEventType.First, oldvalue, VScrollBar.Value, ScrollOrientation.VerticalScroll));
			} else {
				oldvalue = VScrollBar.Value;
				VScrollBar.Value += delta;
				if (delta > 0) this.VScrollBarScroll(null, new ScrollEventArgs(ScrollEventType.SmallIncrement, oldvalue, VScrollBar.Value, ScrollOrientation.VerticalScroll));
				if (delta < 0) this.VScrollBarScroll(null, new ScrollEventArgs(ScrollEventType.SmallDecrement, oldvalue, VScrollBar.Value, ScrollOrientation.VerticalScroll));
			}
		}
		
		/// <summary>
		/// Handles when the hexeditor was click (hexview)
		/// </summary>
		void HexViewMouseClick(object sender, MouseEventArgs e)
		{
			this.Focus();
			this.ActiveView = this.hexView;
			this.charwidth = 3;
			this.caret.Width = 1;
			if (!insertmode)
				this.caret.Width  = underscorewidth * 2;

			caret.Graphics  = this.gbHex;
			
			if (e.Button != MouseButtons.Right) {
				if (!moved) {
					selection.HasSomethingSelected = false;
					selectionmode = false;
				} else {
					moved = false;
					return;
				}
			}
		}
		
		/// <summary>
		/// Handles when the hexeditor was click (textview)
		/// </summary>
		void TextViewMouseClick(object sender, MouseEventArgs e)
		{
			this.Focus();
			hexinputmode = false;
			this.ActiveView = this.textView;
			this.charwidth = 1;
			this.caret.Width = 1;
			if (!insertmode) this.caret.Width = underscorewidth;
			caret.Graphics = this.gbText;
			
			if (e.Button != MouseButtons.Right) {
				if (!moved) {
					selection.HasSomethingSelected = false;
					selectionmode = false;
				} else {
					moved = false;
					return;
				}
			}
			
			this.Focus();
			hexinputmode = false;
			this.ActiveView = this.textView;
			this.charwidth = 1;
			this.caret.Width = 1;
			if (!insertmode) this.caret.Width = underscorewidth;
		}
		#endregion

		#region Painters
		/// <summary>
		/// General painting, using double buffering.
		/// </summary>
		void HexEditPaint(object sender, PaintEventArgs e)
		{
			// Refresh selection.
			CalculateSelectionRegions();
			
			// Calculate views and reset scrollbar.
			UpdateViews();
			AdjustScrollBar();
			
			// Paint using double buffering for better painting!
			
			// Do painting.
			PaintHex(gbHex, VScrollBar.Value);
			PaintOffsetNumbers(gbSide, VScrollBar.Value);
			PaintHeader(gbHeader);
			PaintText(gbText, VScrollBar.Value);
			PaintPointer(gbHex, gbText);
			PaintSelection(gbHex, gbText, true);
			
			if (activeView == hexView)
				this.caret.Graphics = gbHex;
			else
				this.caret.Graphics = gbText;
			
			this.caret.DrawCaret();
			
			// Paint on device ...
			this.gHeader.DrawImageUnscaled(bHeader, 0, 0);
			this.gSide.DrawImageUnscaled(bSide, 0, 0);
			this.gHex.DrawImageUnscaled(bHex, 0, 0);
			this.gText.DrawImageUnscaled(bText, 0, 0);
		}
		
		/// <summary>
		/// Draws the header text ("Offset 0 1 2 3 ...")
		/// </summary>
		/// <param name="g">The graphics device to draw on.</param>
		void PaintHeader(System.Drawing.Graphics g)
		{
			g.Clear(Color.White);
			TextRenderer.DrawText(g, headertext, Settings.OffsetFont, new Rectangle(1, 1, this.hexView.Width + 5, fontheight),
			                      Settings.OffsetForeColor.ToSystemDrawing(), this.BackColor, TextFormatFlags.Left & TextFormatFlags.Top);
		}

		/// <summary>
		/// Draws the offset numbers for each visible line.
		/// </summary>
		/// <param name="g">The graphics device to draw on.</param>
		/// <param name="top">The top line to start.</param>
		void PaintOffsetNumbers(System.Drawing.Graphics g, int top)
		{
			g.Clear(Color.White);
			string text = String.Empty;
			int count = top + this.GetMaxVisibleLines();

			StringBuilder builder = new StringBuilder(StringParser.Parse("${res:AddIns.HexEditor.Display.Elements.Offset}\n"));
			if (count == 0)
				builder.Append("0\n");

			for (int i = top; i < count; i++) {
				if ((i * this.BytesPerLine) <= this.buffer.BufferSize) {
					switch (this.ViewMode) {
						case ViewMode.Decimal:
							builder.AppendLine((i * this.BytesPerLine).ToString());
							break;
						case ViewMode.Hexadecimal:
							builder.AppendFormat("{0:X}", i * this.BytesPerLine);
							builder.AppendLine();
							break;
						case ViewMode.Octal:
							int tmp = i * this.BytesPerLine;
							if (tmp == 0) {
								builder.AppendLine("0");
							} else {
								StringBuilder num = new StringBuilder();
								while (tmp != 0) {
									num.Insert(0, (tmp % 8).ToString());
									tmp = (int)(tmp / 8);
								}
								builder.AppendLine(num.ToString());
							}

							break;
					}
				}
			}

			text = builder.ToString();
			builder = null;
			
			TextRenderer.DrawText(g, text, Settings.OffsetFont,this.side.ClientRectangle,
			                      Settings.OffsetForeColor.ToSystemDrawing(), Color.White, TextFormatFlags.Right);
		}
		
		/// <summary>
		/// Draws the hexadecimal view of the data.
		/// </summary>
		/// <param name="g">The graphics device to draw on.</param>
		/// <param name="top">The top line to start.</param>
		void PaintHex(System.Drawing.Graphics g, int top)
		{
			g.Clear(Color.White);
			StringBuilder builder = new StringBuilder();

			int offset = GetOffsetForLine(top);

			for (int i = 0; i < GetMaxVisibleLines(); i++) {
				builder.AppendLine(GetHex(buffer.GetBytes(offset, this.BytesPerLine)));
				offset = GetOffsetForLine(top + i + 1);
			}

			TextRenderer.DrawText(g, builder.ToString(), Settings.DataFont, new Rectangle(0, 0, this.hexView.Width, this.hexView.Height), Settings.DataForeColor.ToSystemDrawing(), Color.White, TextFormatFlags.Left & TextFormatFlags.Top);
		}
		
		/// <summary>
		/// Draws the normal text view of the data.
		/// </summary>
		/// <param name="g">The graphics device to draw on.</param>
		/// <param name="top">The top line to start.</param>
		void PaintText(System.Drawing.Graphics g, int top)
		{
			g.Clear(Color.White);

			int offset = GetOffsetForLine(top);
			
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < GetMaxVisibleLines(); i++) {
				builder.AppendLine(GetText(buffer.GetBytes(offset, this.BytesPerLine)));
				offset = GetOffsetForLine(top + i + 1);
			}
			TextRenderer.DrawText(g, builder.ToString(), Settings.DataFont, new Point(0, 0), Settings.DataForeColor.ToSystemDrawing(), Color.White);
		}
		
		/// <summary>
		/// Draws a pointer to show the cursor in the opposite view panel.
		/// </summary>
		/// <param name="hexView">the graphics device for the hex view panel</param>
		/// <param name="textView">the graphics device for the text view panel</param>
		void PaintPointer(System.Drawing.Graphics hexView, System.Drawing.Graphics textView)
		{
			// Paint a rectangle as a pointer in the view without the focus ...
			if (selection.HasSomethingSelected) return;
			if (this.ActiveView == this.hexView) {
				Point pos = this.GetPositionForOffset(caret.Offset, 1);
				if (hexinputmode) pos = this.GetPositionForOffset(caret.Offset - 1, 1);
				Size size = new Size(underscorewidth, fontheight);
				Pen p = new Pen(Color.Black, 1f);
				p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
				textView.DrawRectangle(p, new Rectangle(pos, size));
			} else {
				Point pos = this.GetPositionForOffset(caret.Offset, 3);
				pos.Offset(0, 1);
				Size size = new Size(underscorewidth * 2, fontheight);
				Pen p = new Pen(Color.Black, 1f);
				p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
				hexView.DrawRectangle(p, new Rectangle(pos, size));
			}
		}
		
		/// <summary>
		/// Recalculates the current selection regions for drawing.
		/// </summary>
		void CalculateSelectionRegions()
		{
			ArrayList al = new ArrayList();
			
			int lines = Math.Abs(GetLineForOffset(selection.End) - GetLineForOffset(selection.Start));
			int start, end;
			
			if (selection.End > selection.Start) {
				start = selection.Start;
				end = selection.End;
			} else {
				start = selection.End;
				end = selection.Start;
			}
			
			int start_dummy = start;
			
			if (start < GetOffsetForLine(topline)) {
				start = GetOffsetForLine(topline) - 2;
				start_dummy = GetOffsetForLine(topline - 2);
			}
			
			if (end > GetOffsetForLine(topline + GetMaxVisibleLines())) end = GetOffsetForLine(topline + GetMaxVisibleLines() + 1);
			
			int tmp_start = start;
			if (((selection.End % this.bytesPerLine) == 0) && (selection.End < selection.Start))
				tmp_start++;
			
			if (this.activeView == this.hexView)
			{
				if (GetLineForOffset(end) == GetLineForOffset(tmp_start)) {
					Point pt = GetPositionForOffset(start, 3);
					al.Add(new Rectangle(new Point(pt.X - 4, pt.Y), new Size((end - start) * underscorewidth3 + 2, fontheight)));
				} else {
					// First Line
					Point pt = GetPositionForOffset(start, 3);
					al.Add(new Rectangle(new Point(pt.X - 4, pt.Y), new Size((this.BytesPerLine - (start - this.BytesPerLine * GetLineForOffset(start))) * underscorewidth3 + 2, fontheight)));
					
					// Lines between
					Point pt2 = GetPositionForOffset((1 + GetLineForOffset(start)) * this.BytesPerLine, 3);
					al.Add(new Rectangle(new Point(pt2.X - 4, pt2.Y), new Size(this.BytesPerLine * underscorewidth3 + 2, fontheight * (lines - 1) - lines + 1)));
					
					// Last Line
					Point pt3 = GetPositionForOffset(GetLineForOffset(end) * this.BytesPerLine, 3);
					al.Add(new Rectangle(new Point(pt3.X - 4, pt3.Y), new Size((end - GetLineForOffset(end) * this.BytesPerLine) * underscorewidth3 + 2, fontheight)));
				}
				
				this.selregion = (Rectangle[])al.ToArray(typeof (Rectangle));
				
				al.Clear();
				
				start = start_dummy;
				
				if (GetLineForOffset(end) == GetLineForOffset(tmp_start)) {
					Point pt = GetPositionForOffset(start, 1);
					al.Add(new Point(pt.X - 1, pt.Y));
					al.Add(new Point(pt.X - 1, pt.Y + fontheight));
					al.Add(new Point(pt.X - 1 + (end - start + 1) * underscorewidth - 8, pt.Y + fontheight));
					al.Add(new Point(pt.X - 1 + (end - start + 1) * underscorewidth - 8, pt.Y));
				} else {
					// First Line
					Point pt = GetPositionForOffset(start, 1);
					pt = new Point(pt.X - 1, pt.Y);
					al.Add(pt);
					pt = new Point(pt.X, pt.Y + fontheight - 1);
					al.Add(pt);

					// Second Line
					pt = GetPositionForOffset(GetOffsetForLine(GetLineForOffset(start) + 1), 1);
					pt = new Point(pt.X - 1, pt.Y);
					al.Add(pt);

					//last
					pt = GetPositionForOffset(GetOffsetForLine(GetLineForOffset(end)), 1);
					if ((end % this.BytesPerLine) != 0) {
						pt = new Point(pt.X - 1, pt.Y + fontheight);
					} else {
						pt = new Point(pt.X - 1, pt.Y + fontheight - 1);
					}
					al.Add(pt);
					
					if ((end % this.BytesPerLine) != 0) {
						//last
						pt = GetPositionForOffset(end, 1);
						pt = new Point(pt.X, pt.Y + fontheight);
						al.Add(pt);

						//last
						pt = GetPositionForOffset(end, 1);
						pt = new Point(pt.X, pt.Y);
						al.Add(pt);
						
					}

					//last
					pt = GetPositionForOffset(end + (this.BytesPerLine - (end % this.BytesPerLine)) - 1, 1);
					pt = new Point(pt.X + underscorewidth, pt.Y);
					al.Add(pt);

					//last
					pt = GetPositionForOffset(end + (this.BytesPerLine - (end % this.BytesPerLine)) - 1, 1);
					pt = new Point(pt.X + underscorewidth, GetPositionForOffset(start, 1).Y);
					al.Add(pt);
				}
				
				selpoints = (Point[])al.ToArray(typeof(Point));
			} else {
				if (GetLineForOffset(end) == GetLineForOffset(tmp_start)) {
					Point pt = GetPositionForOffset(start, 1);
					al.Add(new Rectangle(new Point(pt.X - 4, pt.Y), new Size((end - start) * underscorewidth + 3, fontheight)));
				} else {
					// First Line
					Point pt = GetPositionForOffset(start, 1);
					al.Add(new Rectangle(new Point(pt.X - 4, pt.Y), new Size((this.BytesPerLine - (start - this.BytesPerLine * GetLineForOffset(start))) * underscorewidth + 3, fontheight)));
					
					// Lines between
					Point pt2 = GetPositionForOffset((1 + GetLineForOffset(start)) * this.BytesPerLine, 3);
					al.Add(new Rectangle(new Point(pt2.X - 4, pt2.Y), new Size(this.BytesPerLine * underscorewidth + 3, fontheight * (lines - 1) - lines + 1)));
					
					// Last Line
					Point pt3 = GetPositionForOffset(GetLineForOffset(end) * this.BytesPerLine, 1);
					al.Add(new Rectangle(new Point(pt3.X - 4, pt3.Y), new Size((end - GetLineForOffset(end) * this.BytesPerLine) * underscorewidth + 3, fontheight)));
				}
				
				selregion = (Rectangle[])al.ToArray(typeof(Rectangle));
				
				al.Clear();
				
				start = start_dummy;
				
				if (GetLineForOffset(end) == GetLineForOffset(tmp_start)) {
					Point pt = GetPositionForOffset(start, 3);
					al.Add(new Point(pt.X - 1, pt.Y));
					al.Add(new Point(pt.X - 1, pt.Y + fontheight));
					al.Add(new Point(pt.X - 1 + (end - start) * underscorewidth3 - 5, pt.Y + fontheight));
					al.Add(new Point(pt.X - 1 + (end - start) * underscorewidth3 - 5, pt.Y));
				} else {
					// First Line
					Point pt = GetPositionForOffset(start, 3);
					pt = new Point(pt.X - 1, pt.Y);
					al.Add(pt);
					pt = new Point(pt.X, pt.Y + fontheight - 1);
					al.Add(pt);

					// Second Line
					pt = GetPositionForOffset(GetOffsetForLine(GetLineForOffset(start) + 1), 3);
					pt = new Point(pt.X - 1, pt.Y);
					al.Add(pt);

					//last
					pt = GetPositionForOffset(GetOffsetForLine(GetLineForOffset(end)), 3);
					if ((end % this.BytesPerLine) != 0) {
						pt = new Point(pt.X - 1, pt.Y + fontheight);
					} else {
						pt = new Point(pt.X - 1, pt.Y + fontheight - 1);
					}
					al.Add(pt);
					
					if ((end % this.BytesPerLine) != 0) {

						//last
						pt = GetPositionForOffset(end, 3);
						pt = new Point(pt.X - 5, pt.Y + fontheight);
						al.Add(pt);

						//last
						pt = GetPositionForOffset(end, 3);
						pt = new Point(pt.X - 5, pt.Y);
						al.Add(pt);
					}

					//last
					pt = GetPositionForOffset(end + (this.BytesPerLine - (end % this.BytesPerLine)) - 1, 3);
					pt = new Point(pt.X - 5 + underscorewidth3, pt.Y);
					al.Add(pt);

					//last
					pt = GetPositionForOffset(end + (this.BytesPerLine - (end % this.BytesPerLine)) - 1, 3);
					pt = new Point(pt.X - 5 + underscorewidth3, GetPositionForOffset(start, 3).Y);
					al.Add(pt);
				}
				
				selpoints = (Point[])al.ToArray(typeof(Point));
			}
		}
		
		/// <summary>
		/// Draws the current selection
		/// </summary>
		/// <param name="hexView">The graphics device for the hex view panel</param>
		/// <param name="textView">The graphics device for the text view panel</param>
		/// <param name="paintMarker">If true the marker is painted, otherwise not.</param>
		void PaintSelection(Graphics hexView, Graphics textView, bool paintMarker)
		{
			if (!selection.HasSomethingSelected) return;
			
			int lines = Math.Abs(GetLineForOffset(selection.End) - GetLineForOffset(selection.Start)) + 1;
			int start, end;
			
			if (selection.End > selection.Start) {
				start = selection.Start;
				end = selection.End;
			} else {
				start = selection.End;
				end = selection.Start;
			}
			
			if (start > GetOffsetForLine(topline + GetMaxVisibleLines())) return;
			
			if (start < GetOffsetForLine(topline)) start = GetOffsetForLine(topline) - 2;
			if (end > GetOffsetForLine(topline + GetMaxVisibleLines())) end = GetOffsetForLine(topline + GetMaxVisibleLines() + 1);
			
			if (this.activeView == this.hexView) {
				StringBuilder builder = new StringBuilder();
				
				for (int i = GetLineForOffset(start) + 1; i < GetLineForOffset(end); i++) {
					builder.AppendLine(GetLineHex(i));
				}
				
				if (selregion.Length == 3) {
					TextRenderer.DrawText(hexView, GetHex(buffer.GetBytes(start, this.BytesPerLine)), Settings.DataFont, (Rectangle)selregion[0], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
					TextRenderer.DrawText(hexView, builder.ToString(), Settings.DataFont, (Rectangle)selregion[1], Color.White, SystemColors.Highlight, TextFormatFlags.Left);
					TextRenderer.DrawText(hexView, GetLineHex(GetLineForOffset(end)), Settings.DataFont, (Rectangle)selregion[2], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
				} else if (selregion.Length == 2) {
					TextRenderer.DrawText(hexView, GetHex(buffer.GetBytes(start, this.BytesPerLine)), Settings.DataFont, (Rectangle)selregion[0], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
					TextRenderer.DrawText(hexView, GetLineHex(GetLineForOffset(end)), Settings.DataFont, (Rectangle)selregion[1], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
				} else {
					TextRenderer.DrawText(hexView, GetHex(buffer.GetBytes(start, this.BytesPerLine)), Settings.DataFont, (Rectangle)selregion[0], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
				}
			} else {
				StringBuilder builder = new StringBuilder();
				
				for (int i = GetLineForOffset(start) + 1; i < GetLineForOffset(end); i++) {
					builder.AppendLine(GetLineText(i));
				}
				
				if (selregion.Length == 3) {
					TextRenderer.DrawText(textView, GetText(buffer.GetBytes(start, this.BytesPerLine)), Settings.DataFont, (Rectangle)selregion[0], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
					TextRenderer.DrawText(textView, builder.ToString(), Settings.DataFont, (Rectangle)selregion[1], Color.White, SystemColors.Highlight, TextFormatFlags.Left);
					TextRenderer.DrawText(textView, GetLineText(GetLineForOffset(end)), Settings.DataFont, (Rectangle)selregion[2], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
				} else if (selregion.Length == 2) {
					TextRenderer.DrawText(textView, GetText(buffer.GetBytes(start, this.BytesPerLine)), Settings.DataFont, (Rectangle)selregion[0], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
					TextRenderer.DrawText(textView, GetLineText(GetLineForOffset(end)), Settings.DataFont, (Rectangle)selregion[1], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
				} else {
					TextRenderer.DrawText(textView, GetText(buffer.GetBytes(start, this.BytesPerLine)), Settings.DataFont, (Rectangle)selregion[0], Color.White, SystemColors.Highlight, TextFormatFlags.Left & TextFormatFlags.SingleLine);
				}
			}
			
			if (!paintMarker) return;
			
			GraphicsPath path = new GraphicsPath(FillMode.Winding);
			
			if (GetLineForOffset(start) == GetLineForOffset(end) || ((start % this.bytesPerLine) == 0 && GetLineForOffset(start) + 1 == GetLineForOffset(end))) {
				if (this.selpoints.Length == 8) {
					path.AddLine(this.selpoints[0], this.selpoints[1]);
					path.AddLine(this.selpoints[3], this.selpoints[4]);
					path.AddLine(this.selpoints[4], this.selpoints[5]);
					path.AddLine(this.selpoints[5], this.selpoints[0]);
				} else
					path.AddPolygon(this.selpoints);
			} else {
				if ((GetLineForOffset(start) == GetLineForOffset(end) - 1) && (start % this.bytesPerLine >= end % this.bytesPerLine)) {
					if (this.selpoints.Length < 8) {
						path.AddPolygon(this.selpoints);
					} else {
						path.AddLine(this.selpoints[0], this.selpoints[1]);
						path.AddLine(this.selpoints[6], this.selpoints[7]);
						path.CloseFigure();
						path.AddLine(this.selpoints[2], this.selpoints[3]);
						path.AddLine(this.selpoints[4], this.selpoints[5]);
						path.CloseFigure();
					}
				} else
					path.AddPolygon(this.selpoints);
			}
			
			
			if (this.activeView == this.hexView)
				textView.DrawPath(Pens.Black, path);
			else
				hexView.DrawPath(Pens.Black, path);
		}
		#endregion
		
		#region Undo/Redo
		/*
		 * Undo/Redo handling for the buffer.
		 * */
		public void Redo()
		{
			EventArgs e2 = new EventArgs();
			OnDocumentChanged(e2);

			UndoStep step = undoStack.Redo(ref buffer);
			hexinputmode = false;
			hexinputmodepos = 0;
			selection.Clear();
			if (step != null) caret.SetToPosition(GetPositionForOffset(step.Start, this.charwidth));
			this.Invalidate();
		}
		
		public void Undo()
		{
			EventArgs e2 = new EventArgs();
			OnDocumentChanged(e2);

			UndoStep step = undoStack.Undo(ref buffer);
			hexinputmode = false;
			hexinputmodepos = 0;
			selection.Clear();
			if (step != null) {
				int offset = step.Start;
				if (offset > buffer.BufferSize) offset = buffer.BufferSize;
				caret.SetToPosition(GetPositionForOffset(offset, this.charwidth));
			}
			this.Invalidate();
		}
		
		public bool CanUndo {
			get { return undoStack.CanUndo; }
		}
		
		public bool CanRedo {
			get { return undoStack.CanRedo; }
		}
		#endregion
		
		#region Selection
		/*
		 * Selection handling
		 * */
		public void SetSelection(int start, int end)
		{
			if (start > buffer.BufferSize) start = buffer.BufferSize;
			if (start < 0) start = 0;
			selection.Start = start;
			if (end > buffer.BufferSize) end = buffer.BufferSize;
			selection.End = end;
			selection.HasSomethingSelected = true;
			hexinputmode = false;
			hexinputmodepos = 0;
			
			CalculateSelectionRegions();
			
			this.Invalidate();
		}
		
		public bool HasSomethingSelected {
			get { return selection.HasSomethingSelected; }
		}
		
		public void SelectAll()
		{
			SetSelection(0, this.buffer.BufferSize);
		}
		#endregion
		
		#region Clipboard Actions
		/*
		 * Clipboard handling
		 * */
		public string Copy()
		{
			string text = selection.SelectionText;
			
			if (text.Contains("\0"))
				text = text.Replace("\0", "");
			
			return text;
		}
		
		public string CopyAsHexString()
		{
			return GetHex(selection.GetSelectionBytes());
		}
		
		public string CopyAsBinary()
		{
			return Copy();
		}
		
		public void Paste(string text)
		{
			if (caret.Offset > buffer.BufferSize) caret.Offset = buffer.BufferSize;
			if (selection.HasSomethingSelected) {
				byte[] old = selection.GetSelectionBytes();
				int start = selection.Start;
				
				if (selection.Start > selection.End) start = selection.End;
				
				buffer.RemoveBytes(start, Math.Abs(selection.End - selection.Start));
				buffer.SetBytes(start, this.Encoding.GetBytes(text.ToCharArray()), false);
				undoStack.AddOverwriteStep(start, this.Encoding.GetBytes(text.ToCharArray()), old);

				caret.Offset = start + ClipboardManager.Paste().Length;
				selection.Clear();
			} else {
				buffer.SetBytes(caret.Offset, this.Encoding.GetBytes(text.ToCharArray()), false);
				
				undoStack.AddRemoveStep(caret.Offset, this.Encoding.GetBytes(text.ToCharArray()));

				caret.Offset += ClipboardManager.Paste().Length;
			}
			if (GetLineForOffset(caret.Offset) < this.topline) this.topline = GetLineForOffset(caret.Offset);
			if (GetLineForOffset(caret.Offset) > this.topline + this.GetMaxVisibleLines() - 2) this.topline = GetLineForOffset(caret.Offset) - this.GetMaxVisibleLines() + 2;
			if (this.topline < 0) this.topline = 0;
			if (this.topline > VScrollBar.Maximum) {
				AdjustScrollBar();
				if (this.topline > VScrollBar.Maximum) this.topline = VScrollBar.Maximum;
			}
			VScrollBar.Value = this.topline;
			this.Invalidate();
			
			EventArgs e2 = new EventArgs();
			OnDocumentChanged(e2);
		}
		
		public void Delete()
		{
			if (hexinputmode) return;
			if (selection.HasSomethingSelected) {
				byte[] old = selection.GetSelectionBytes();
				buffer.RemoveBytes(selection.Start, Math.Abs(selection.End - selection.Start));
				caret.Offset = selection.Start;
				
				undoStack.AddInsertStep(selection.Start, old);
				
				selection.Clear();
			}
			this.Invalidate();
			
			EventArgs e2 = new EventArgs();
			OnDocumentChanged(e2);
		}
		#endregion
		
		#region TextProcessing
		/// <summary>
		/// Generates a string out of a byte array. Unprintable chars are replaced by a ".".
		/// </summary>
		/// <param name="bytes">An array of bytes to convert to a string.</param>
		/// <returns>A string containing all bytes in the byte array.</returns>
		string GetText(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++) {
				if (bytes[i] < 32) bytes[i] = 46;
			}

			string text = this.Encoding.GetString(bytes);
			return text.Replace("&", "&&");
		}

		/// <summary>
		/// Gets the text from a line.
		/// </summary>
		/// <param name="line">The line number to get the text from.</param>
		/// <returns>A string, which contains the text on the given line.</returns>
		string GetLineText(int line)
		{
			return GetText(buffer.GetBytes(GetOffsetForLine(line), this.BytesPerLine));
		}
		
		/// <summary>
		/// Returns the text from a line in hex.
		/// </summary>
		/// <param name="line">The line number to get the text from.</param>
		/// <returns>A string, which contains the text on the given line in hex representation.</returns>
		string GetLineHex(int line)
		{
			return GetHex(buffer.GetBytes(GetOffsetForLine(line), this.BytesPerLine));
		}
		
		/// <summary>
		/// Converts a byte[] to its string representation.
		/// </summary>
		/// <param name="bytes">An array of bytes to convert</param>
		/// <returns>The string representation of the byte[]</returns>
		static string GetHex(byte[] bytes)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
				builder.Append(string.Format("{0:X2} ", bytes[i]));
			return builder.ToString();
		}
		#endregion
		
		/// <summary>
		/// Redraws the control after resizing.
		/// </summary>
		void HexEditSizeChanged(object sender, EventArgs e)
		{
			if (this.FitToWindowWidth) this.BytesPerLine = CalculateMaxBytesPerLine();
			
			this.Invalidate();
			UpdateViews();
			
		}
		
		void UpdatePainters()
		{
			gHeader = this.header.CreateGraphics();
			gSide = this.side.CreateGraphics();
			gHex = this.hexView.CreateGraphics();
			gText = this.textView.CreateGraphics();
			
			// Bitmaps for painting
			bHeader = new Bitmap(this.header.Width, this.header.Height, this.gHeader);
			bSide = new Bitmap(this.side.Width, this.side.Height, this.gSide);
			bHex = new Bitmap(this.hexView.Width, this.hexView.Height, this.gHex);
			bText = new Bitmap(this.textView.Width, this.textView.Height, this.gText);
			
			gbHeader = Graphics.FromImage(bHeader);
			gbSide = Graphics.FromImage(bSide);
			gbHex = Graphics.FromImage(bHex);
			gbText = Graphics.FromImage(bText);
		}
		
		/// <summary>
		/// Resets the current viewpanels to fit the new sizes and settings.
		/// </summary>
		void UpdateViews()
		{
			this.UpdatePainters();
			
			int sidetext = this.GetMaxLines() * this.BytesPerLine;
			int textwidth = MeasureStringWidth(this.textView.CreateGraphics(), new string('_', this.BytesPerLine + 1), Settings.DataFont);
			int hexwidth = underscorewidth3 * this.BytesPerLine;
			int top = hexView.Top;
			this.hexView.Top = fontheight - 1;
			this.textView.Top = fontheight - 1;
			this.header.Top = 0;
			this.header.Left = hexView.Left - 10;
			this.hexView.Height = this.Height - fontheight + top - 18;
			this.textView.Height = this.Height - fontheight + top - 18;

			string st = String.Empty;

			switch (this.ViewMode) {
				case ViewMode.Hexadecimal:
					if (sidetext.ToString().Length < 8) {
						st = "  Offset";
					} else {
						st = "  " + string.Format("{0:X}", sidetext);
					}
					break;
				case ViewMode.Octal:
					if (sidetext.ToString().Length < 8) {
						st = "  Offset";
					} else {
						int tmp = sidetext;
						while (tmp != 0) {
							st = (tmp % 8).ToString() + st;
							tmp = (int)(tmp / 8);
						}
					}

					st = "  " + st;
					break;
				case ViewMode.Decimal:
					if (sidetext.ToString().Length < 8) {
						st = "  Offset";
					} else {
						st = "  " + sidetext.ToString();
					}
					break;
			}

			this.side.Width = MeasureStringWidth(this.side.CreateGraphics(), st, Settings.OffsetFont);
			this.side.Left = 0;
			this.hexView.Left = this.side.Width + 10;

			if ((textwidth + hexwidth + 25) > this.Width - this.side.Width) {
				this.hexView.Width = this.Width - this.side.Width - textwidth - 30;
				this.textView.Width = textwidth;
				this.textView.Left = this.Width - textwidth - 16;
			} else {
				this.hexView.Width = hexwidth;
				this.textView.Width = textwidth;
				this.textView.Left = hexwidth + this.hexView.Left + 20;
			}
			
			this.caret.SetToPosition(GetPositionForOffset(this.caret.Offset, this.charwidth));
			this.header.Width = this.hexView.Width + 10;
			this.header.Height = this.fontheight;
			AdjustScrollBar();
		}
		
		/// <summary>
		/// General handling of keyboard events, for non printable keys.
		/// </summary>
		void HexEditKeyDown(object sender, KeyEventArgs e)
		{
			int start = selection.Start;
			int end = selection.End;
			
			if (selection.Start > selection.End) {
				start = selection.End;
				end = selection.Start;
			}
			
			if (this.activeView != this.hexView) {
				hexinputmode = false;
				hexinputmodepos = 0;
			}
			
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.Up:
						if (this.topline > 0)
							this.topline--;
						break;
					case Keys.Down:
						if (this.topline < this.GetMaxLines())
							this.topline++;
						break;
				}

				this.VScrollBar.Value = this.topline;

				this.handled = true;
			}
			
			switch (e.KeyCode) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
					if (!e.Control) {
						int oldoffset = caret.Offset;
						MoveCaret(e);
						if (GetLineForOffset(caret.Offset) < this.topline) this.topline = GetLineForOffset(caret.Offset);
						if (GetLineForOffset(caret.Offset) > this.topline + this.GetMaxVisibleLines() - 2) this.topline = GetLineForOffset(caret.Offset) - this.GetMaxVisibleLines() + 2;
						VScrollBar.Value = this.topline;
						
						if (e.Shift) {
							if (selection.HasSomethingSelected) {
								this.SetSelection(selection.Start, caret.Offset);
							} else {
								this.SetSelection(oldoffset, caret.Offset);
							}
						} else {
							this.selection.Clear();
						}
						handled = true;
					}
					break;
				case Keys.Insert:
					insertmode = !insertmode;
					if (!insertmode) {
						if (this.activeView == this.hexView)
							this.caret.Width = underscorewidth * 2;
						else
							this.caret.Width = underscorewidth;
					} else {
						this.caret.Width = 1;
					}
					
					caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
					handled = true;
					break;
				case Keys.Back:
					handled = true;
					if (hexinputmode) return;
					if (selection.HasSomethingSelected) {
						byte[] bytes = selection.GetSelectionBytes();
						
						buffer.RemoveBytes(start, Math.Abs(end - start));
						caret.Offset = start;
						
						undoStack.AddInsertStep(start, bytes);
						
						selection.Clear();
					} else {
						byte b = buffer.GetByte(caret.Offset - 1);
						
						if (buffer.RemoveByte(caret.Offset - 1))
						{
							if (caret.Offset > -1) caret.Offset--;
							if (GetLineForOffset(caret.Offset) < this.topline) this.topline = GetLineForOffset(caret.Offset);
							if (GetLineForOffset(caret.Offset) > this.topline + this.GetMaxVisibleLines() - 2) this.topline = GetLineForOffset(caret.Offset) - this.GetMaxVisibleLines() + 2;
							
							undoStack.AddInsertStep(caret.Offset, new byte[] {b});
						}
					}
					
					EventArgs e2 = new EventArgs();
					OnDocumentChanged(e2);
					break;
				case Keys.Delete:
					handled = true;
					if (hexinputmode) return;
					if (selection.HasSomethingSelected) {
						byte[] old = selection.GetSelectionBytes();
						buffer.RemoveBytes(start, Math.Abs(selection.End - selection.Start));
						caret.Offset = selection.Start;

						undoStack.AddInsertStep(selection.Start, old);
						
						selection.Clear();
					} else {
						byte b = buffer.GetByte(caret.Offset);
						
						buffer.RemoveByte(caret.Offset);
						
						undoStack.AddInsertStep(caret.Offset, new byte[] {b});
						
						if (GetLineForOffset(caret.Offset) < this.topline) this.topline = GetLineForOffset(caret.Offset);
						if (GetLineForOffset(caret.Offset) > this.topline + this.GetMaxVisibleLines() - 2) this.topline = GetLineForOffset(caret.Offset) - this.GetMaxVisibleLines() + 2;
					}
					
					e2 = new EventArgs();
					OnDocumentChanged(e2);
					break;
				case Keys.CapsLock:
				case Keys.ShiftKey:
				case Keys.ControlKey:
					break;
				case Keys.Tab:
					if (this.activeView == this.hexView) {
						this.activeView = this.textView;
						this.charwidth = 1;
					} else {
						this.activeView = this.hexView;
						this.charwidth = 3;
					}
					this.handled = true;
					break;
				default:
					byte asc = (byte)e.KeyValue;
					
					if (e.Control) {
						handled = true;
						switch (asc) {
								// Ctrl-A is pressed -> select all
							case 65 :
								this.SetSelection(0, buffer.BufferSize);
								break;
								// Ctrl-C is pressed -> copy text to ClipboardManager
							case 67 :
								ClipboardManager.Copy(this.Copy());
								break;
								// Ctrl-V is pressed -> paste from ClipboardManager
							case 86 :
								if (ClipboardManager.ContainsText) {
									this.Paste(ClipboardManager.Paste());
									if (GetLineForOffset(caret.Offset) < this.topline) this.topline = GetLineForOffset(caret.Offset);
									if (GetLineForOffset(caret.Offset) > this.topline + this.GetMaxVisibleLines() - 2) this.topline = GetLineForOffset(caret.Offset) - this.GetMaxVisibleLines() + 2;
									if (this.topline < 0) this.topline = 0;
									if (this.topline > VScrollBar.Maximum) {
										AdjustScrollBar();
										if (this.topline > VScrollBar.Maximum) this.topline = VScrollBar.Maximum;
									}
									VScrollBar.Value = this.topline;
								}
								break;
								// Ctrl-X is pressed -> cut from document
							case 88 :
								if (selection.HasSomethingSelected) {
									ClipboardManager.Copy(this.Copy());
									this.Delete();
								}
								break;
						}
						break;
					}
					if (this.activeView == this.hexView) {
						ProcessHexInput(e);
						handled = true;
						return;
					}

					break;
			}
			if (handled) {
				this.Invalidate();
				caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
			}
		}
		
		/// <summary>
		/// Handling of printable keys.
		/// </summary>
		void HexEditKeyPress(object sender, KeyPressEventArgs e)
		{
			if (handled) {
				handled = false;
			} else {
				byte[] old = buffer.GetBytes(caret.Offset, 1);
				try  {
					if (selection.HasSomethingSelected) {
						Delete();
						buffer.SetByte(caret.Offset, (byte)e.KeyChar, !insertmode);
					} else {
						buffer.SetByte(caret.Offset, (byte)e.KeyChar, !insertmode);
					}
				} catch (System.ArgumentOutOfRangeException) {}
				caret.Offset++;
				if (GetLineForOffset(caret.Offset) < this.topline) this.topline = GetLineForOffset(caret.Offset);
				if (GetLineForOffset(caret.Offset) > this.topline + this.GetMaxVisibleLines() - 2) this.topline = GetLineForOffset(caret.Offset) - this.GetMaxVisibleLines() + 2;
				VScrollBar.Value = this.topline;
				
				if (insertmode)
					undoStack.AddRemoveStep(caret.Offset - 1, new byte[] {(byte)e.KeyChar});
				else
					undoStack.AddOverwriteStep(caret.Offset - 1, new byte[] {(byte)e.KeyChar}, old);
				
				OnDocumentChanged(EventArgs.Empty);
			}
			caret.SetToPosition(GetPositionForOffset(caret.Offset, charwidth));
			

			
			this.Invalidate();
		}
		
		/// <summary>
		/// Sets the caret according to the input.
		/// </summary>
		/// <param name="input">Keyboard input</param>
		void MoveCaret(KeyEventArgs input)
		{
			if (!input.Control) {
				hexinputmode = false;
				hexinputmodepos = 0;
			}
			switch (input.KeyCode) {
				case Keys.Up:
					if (caret.Offset >= this.BytesPerLine) {
						caret.Offset -= this.BytesPerLine;
						caret.SetToPosition(this.GetPositionForOffset(caret.Offset, this.charwidth));
					}
					break;
				case Keys.Down:
					if (caret.Offset <= this.Buffer.BufferSize - this.BytesPerLine) {
						caret.Offset += this.BytesPerLine;
						caret.SetToPosition(this.GetPositionForOffset(caret.Offset, this.charwidth));
					} else {
						caret.Offset = this.Buffer.BufferSize;
						caret.SetToPosition(this.GetPositionForOffset(caret.Offset, this.charwidth));
					}
					break;
				case Keys.Left:
					if (caret.Offset >= 1) {
						if (this.activeView == this.hexView) {
							if (input.Control) {
								hexinputmode = false;
								if (hexinputmodepos == 0) {
									caret.Offset--;
									hexinputmodepos = 1;
									hexinputmode = true;
								} else {
									hexinputmodepos--;
								}
							} else {
								caret.Offset--;
							}
						} else {
							caret.Offset--;
						}
						caret.SetToPosition(this.GetPositionForOffset(caret.Offset, this.charwidth));
					}
					break;
				case Keys.Right:
					if (caret.Offset <= this.Buffer.BufferSize - 1) {
						if (this.activeView == this.hexView) {
							if (input.Control) {
								hexinputmode = true;
								if (hexinputmodepos == 1) {
									caret.Offset++;
									hexinputmodepos = 0;
									hexinputmode = false;
								} else {
									hexinputmodepos++;
								}
							} else {
								caret.Offset++;
							}
						} else {
							caret.Offset++;
						}

						caret.SetToPosition(this.GetPositionForOffset(caret.Offset, this.charwidth));
					}
					break;
			}

		}
		
		/// <summary>
		/// Processes only 0-9 and A-F keys and handles the special hex-inputmode.
		/// </summary>
		/// <param name="input">Keyboard input</param>
		void ProcessHexInput(KeyEventArgs input)
		{
			int start = selection.Start;
			int end = selection.End;
			
			if (selection.Start > selection.End) {
				start = selection.End;
				end = selection.Start;
			}
			
			if (((input.KeyValue > 47) & (input.KeyValue < 58)) | ((input.KeyValue > 64) & (input.KeyValue < 71))) {
				hexinputmode = true;
				if (insertmode) {
					byte[] old;
					if (selection.HasSomethingSelected) {
						old = selection.GetSelectionBytes();
						
						buffer.RemoveBytes(start, Math.Abs(end - start));
					} else {
						old = null;
					}
					string @in = "";
					if (hexinputmodepos == 1) {
						@in = string.Format("{0:X}", buffer.GetByte(caret.Offset));
						
						// if @in is like 4 or A then make 04 or 0A out of it.
						if (@in.Length == 1) @in = "0" + @in;
						
						undoStack.AddOverwriteStep(caret.Offset, new byte[] {(byte)(Convert.ToInt32(@in.Remove(1) + ((char)(input.KeyValue)).ToString(), 16))}, buffer.GetBytes(caret.Offset, 1));
						
						@in = @in.Remove(1) + ((char)(input.KeyValue)).ToString();
						hexinputmodepos = 0;
						hexinputmode = false;
						
						buffer.SetByte(caret.Offset, (byte)(Convert.ToInt32(@in, 16)), true);
						caret.Offset++;
						
						caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
					} else if (hexinputmodepos == 0) {
						UndoAction action;
						
						if (selection.HasSomethingSelected) {
							action = UndoAction.Overwrite;
							caret.Offset = start;
							selection.Clear();
						} else {
							action = UndoAction.Remove;
						}
						@in = (char)(input.KeyValue) + "0";
						if (caret.Offset > buffer.BufferSize) caret.Offset = buffer.BufferSize;
						buffer.SetByte(caret.Offset, (byte)(Convert.ToInt32(@in, 16)), false);
						hexinputmodepos = 1;
						
						undoStack.AddUndoStep(new UndoStep(new byte[] {(byte)(Convert.ToInt32(@in, 16))}, old, caret.Offset, action));

						caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
					}

					caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
				} else {
					UndoAction action;
					
					string @in = "";
					if (hexinputmodepos == 1) {
						byte[] _old = buffer.GetBytes(caret.Offset, 1);
						@in = string.Format("{0:X}", buffer.GetByte(caret.Offset));
						if (@in.Length == 1) @in = "0" + @in;
						@in = @in.Remove(1) + ((char)(input.KeyValue)).ToString();
						hexinputmodepos = 0;
						hexinputmode = false;
						buffer.SetByte(caret.Offset, (byte)(Convert.ToInt32(@in, 16)), true);
						caret.Offset++;
						
						if (insertmode) {
							action = UndoAction.Insert;
							_old = null;
						} else {
							action = UndoAction.Overwrite;
						}
						
						undoStack.AddUndoStep(new UndoStep(new byte[] {(byte)(Convert.ToInt32(@in, 16))}, _old, caret.Offset - 1, action));

						caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));

					} else if (hexinputmodepos == 0) {
						byte[] _old = buffer.GetBytes(caret.Offset, 1);
						@in = (char)(input.KeyValue) + "0";
						buffer.SetByte(caret.Offset, (byte)(Convert.ToInt32(@in, 16)), true);
						hexinputmodepos = 1;
						
						if (insertmode) {
							action = UndoAction.Insert;
							_old = null;
						} else {
							action = UndoAction.Overwrite;
						}
						
						undoStack.AddUndoStep(new UndoStep(new byte[] {(byte)(Convert.ToInt32(@in, 16))}, _old, caret.Offset, action));
					}
					
					caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
				}
				
				EventArgs e = new EventArgs();
				OnDocumentChanged(e);
				
				this.Invalidate();
			}
		}
		
		#region file functions
		/// <summary>
		/// Loads a file into the editor.
		/// </summary>
		/// <param name="file">The file-info to open.</param>
		/// <param name="stream">The stream to read from/write to.</param>
		public void LoadFile(OpenedFile file, Stream stream)
		{
			buffer.Load(file, stream);
			if (this.progressBar != null) {
				this.progressBar.Visible = false;
				this.progressBar.Available = false;
				this.progressBar.Value = 0;
			}
			//this.Cursor = Cursors.WaitCursor;
		}
		
		/// <summary>
		/// Called from the BufferManager when Loading is finished.
		/// </summary>
		/// <remarks>Currently not directly needed, because there's no thread in use to load the data.</remarks>
		internal void LoadingFinished()
		{
			if (this.InvokeRequired) {
				this.Invoke (new MethodInvoker (LoadingFinished));
				return;
			}
			this.FileName = fileName;
			selection.Clear();
			if (this.progressBar != null) {
				this.progressBar.Visible = false;
				this.progressBar.Available = false;
				this.progressBar.Value = 0;
			}
			
			this.side.Cursor = this.Cursor = this.header.Cursor = Cursors.Default;
			
			GC.Collect();
			this.Invalidate();
		}
		
		/// <summary>
		/// Saves the current buffer to a stream.
		/// </summary>
		public void SaveFile(OpenedFile file, Stream stream)
		{
			buffer.Save(file, stream);
		}
		#endregion
		
		/// <summary>
		/// Invalidates the control when the focus returns to it.
		/// </summary>
		private void HexEditGotFocus(object sender, EventArgs e)
		{
			this.Invalidate();
		}
		
		#region selection events
		/**
		 * Methods to control the current selection
		 * with mouse for both hex and text view.
		 * */
		
		void TextViewMouseDown(object sender, MouseEventArgs e)
		{
			this.activeView = this.textView;
			if (e.Button == MouseButtons.Left) {
				if (selection.HasSomethingSelected) {
					selection.Start = 0;
					selection.End = 0;
				}
				selectionmode = true;
				selection.Start = GetOffsetForPosition(e.Location, 1);
			}
		}
		
		void TextViewMouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && selectionmode && (e.Location != oldMousePos)) {
				int end = selection.End;
				selection.End = GetOffsetForPosition(e.Location, 1);
				this.activeView = this.textView;
				moved = true;
				selection.HasSomethingSelected = true;
				caret.Offset = GetOffsetForPosition(e.Location, 1);
				caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
				
				this.Invalidate();
			}
			
			oldMousePos = e.Location;
		}
		
		void TextViewMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) return;
			if (selectionmode) {
				selection.HasSomethingSelected = true;
				if ((selection.End == selection.Start) | ((selection.Start == 0) & (selection.End == 0))) {
					selection.HasSomethingSelected = false;
					selectionmode = false;
				}
			} else {
				if (!moved) {
					selection.HasSomethingSelected = false;
					selection.Start = 0;
					selection.End = 0;
				}
				moved = false;
			}
			caret.Offset = GetOffsetForPosition(e.Location, 1);
			caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
			
			selectionmode = false;
			
			this.Invalidate();
		}
		
		void HexViewMouseDown(object sender, MouseEventArgs e)
		{
			this.activeView = this.hexView;
			if (e.Button == MouseButtons.Left) {
				selectionmode = true;
				selection.Start = GetOffsetForPosition(e.Location, 3);
				selection.End = GetOffsetForPosition(e.Location, 3);
			}
		}
		
		void HexViewMouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && selectionmode && (e.Location != oldMousePos)) {
				int end = selection.End;
				selection.End = GetOffsetForPosition(e.Location, 3);
				selection.HasSomethingSelected = true;
				this.activeView = this.hexView;
				moved = true;
				
				caret.Offset = GetOffsetForPosition(e.Location, 3);
				caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
				
				this.Invalidate();
			}
			
			oldMousePos = e.Location;
		}
		
		void HexViewMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) return;

			if (selectionmode) {
				selection.HasSomethingSelected = true;
				if ((selection.End == selection.Start) || ((selection.Start == 0) && (selection.End == 0))) {
					selection.HasSomethingSelected = false;
					selectionmode = false;
				}
			} else {
				if (!moved) {
					selection.HasSomethingSelected = false;
					selection.End = 0;
					selection.Start = 0;
				}
				moved = false;
			}
			caret.Offset = GetOffsetForPosition(e.Location, 3);
			caret.SetToPosition(GetPositionForOffset(caret.Offset, this.charwidth));
			selectionmode = false;
			
			this.Invalidate();
		}
		#endregion
		
		/// <summary>
		/// Enables the control processing key commands.
		/// </summary>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData) {
				case Keys.Shift | Keys.Up :
				case Keys.Shift | Keys.Down :
				case Keys.Shift | Keys.Left :
				case Keys.Shift | Keys.Right :
					HexEditKeyDown(null, new KeyEventArgs(keyData));
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		/// <summary>
		/// Calculates the max possible bytes per line.
		/// </summary>
		/// <returns>Int32, containing the result</returns>
		internal int CalculateMaxBytesPerLine()
		{
			int width = this.Width - this.side.Width - 90;
			int textwidth = 0, hexwidth = 0;
			int count = 0;
			// while the width of the textview + the width of the hexview is
			// smaller than the width of the whole control.
			while ((textwidth + hexwidth) < width) {
				// update counter and recalculate the sizes
				count++;
				textwidth = underscorewidth * count;
				hexwidth = underscorewidth3 * count;
			}
			
			if (count < 1)
				count = 1;

			return count;
		}
		
		/// <summary>
		/// Calculates the offset for a position.
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="charwidth">the width of one char, for example in the
		/// hexview the width is 3 because one char needs 3 chars to be
		/// displayed ("A" in text = "41 " in hex)</param>
		/// <returns>the offset for the position</returns>
		internal int GetOffsetForPosition(Point position, int charwidth)
		{
			// calculate the line: vertical position (Y) divided by the height of
			// one line (height of font = fontheight) = physical line + topline = virtual line.
			int line = (int)Math.Round((float)position.Y / (float)fontheight) + topline;
			
			if (position.Y > (this.textView.Height / 2.0f))
				line++;
			
			// calculate the char: horizontal position (X) divided by the width of one char
			float col = ((float)position.X / (float)(charwidth * underscorewidth));
			float diff = (float)col - (float)((int)col);
			
			int ch = diff >= 0.75f ? (int)col + 1 : (int)col;
			
			if (ch > this.BytesPerLine) ch = this.BytesPerLine;
			if (ch < 0) ch = 0;
			
			// calculate offset
			int offset = line * this.BytesPerLine + ch;
			
			if ((diff > 0.35f) && (diff < 0.75f)) {
				this.hexinputmodepos = 1;
				this.hexinputmode = true;
			} else {
				this.hexinputmodepos = 0;
				this.hexinputmode = false;
			}
			
			// check
			if (offset < 0) return 0;
			if (offset < this.buffer.BufferSize) {
				return offset;
			} else {
				return this.buffer.BufferSize;
			}
		}
		
		/// <summary>
		/// Does the same as GetOffsetForPosition, but the other way round.
		/// </summary>
		/// <param name="offset">The offset to search</param>
		/// <param name="charwidth">the current width of one char.
		/// (Depends on the viewpanel we are using (3 in hex 1 in text view)</param>
		/// <returns>The Drawing.Point at which the offset is currently.</returns>
		internal Point GetPositionForOffset(int offset, int charwidth)
		{
			int line = (int)(offset / this.BytesPerLine) - this.topline;
			int pline = line * fontheight - 1 * (line - 1) - 1;
			int col = (offset % this.BytesPerLine) * underscorewidth * charwidth + 4;
			if (hexinputmode && !selectionmode && !selection.HasSomethingSelected && this.insertmode) col += (hexinputmodepos * underscorewidth);
			
			return new Point(col, pline);
		}
		
		/// <summary>
		/// Returns the starting offset of a line.
		/// </summary>
		/// <param name="line">The line in the file, countings starts at 0.</param>
		/// <returns>The starting offset for a line.</returns>
		internal int GetOffsetForLine(int line)
		{
			return line * this.BytesPerLine;
		}
		
		/// <summary>
		/// Calculates the line on which the given offset is.
		/// </summary>
		/// <param name="offset">The offset to look up the line for.</param>
		/// <returns>The line on which the given offset is.</returns>
		/// <remarks>returns 0 for first line ...</remarks>
		internal int GetLineForOffset(int offset)
		{
			if (offset == 0)
				return 0;
			return (int)Math.Ceiling((double)offset / (double)this.BytesPerLine) - 1;
		}
		
		/// <summary>
		/// Calculates the count of visible lines.
		/// </summary>
		/// <returns>The count of currently visible virtual lines.</returns>
		internal int GetMaxVisibleLines()
		{
			return (int)(this.hexView.Height / fontheight) + 3;
		}
		
		/// <summary>
		/// Calculates the count of all virtual lines in the buffer.
		/// </summary>
		/// <returns>Retrns 1 if the buffer is empty, otherwise the count of all virtual lines in the buffer.</returns>
		internal int GetMaxLines()
		{
			if (buffer == null) return 1;
			int lines = (int)(buffer.BufferSize / this.BytesPerLine);
			if ((buffer.BufferSize % this.BytesPerLine) != 0) lines++;
			return lines;
		}
		
		/// <summary>
		/// Calculates the count of digits of a given number.
		/// </summary>
		/// <param name="number">The number to calculate</param>
		/// <returns>the count of digits in the number</returns>
		static int GetLength(int number)
		{
			int count = 1;
			while (number > 9) {
				number = number / 10;
				count++;
			}
			return count;
		}
		
		/// <summary>
		/// Handles the context menu
		/// </summary>
		void HexEditControlContextMenuStripChanged(object sender, EventArgs e)
		{
			this.ContextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(ContextMenuStripClosed);
		}
		
		/// <summary>
		/// Invalidates the control after the context menu is closed.
		/// </summary>
		void ContextMenuStripClosed(object sender, EventArgs e)
		{
			this.Invalidate();
		}
	}
}
