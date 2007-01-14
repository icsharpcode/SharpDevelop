using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Imaging;

namespace Aga.Controls.Tree
{
	[TypeConverter(typeof(TreeColumn.TreeColumnConverter)), DesignTimeVisible(false), ToolboxItem(false)]
	public class TreeColumn : Component
	{
		private class TreeColumnConverter : ComponentConverter
		{
			public TreeColumnConverter()
				: base(typeof(TreeColumn))
			{
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return false;
			}

		}

		private const int HeaderMargin = 5;
		private const int SortOrderMarkMargin = 5;
		//private const int SortOrderMarkWidth = 7;

		private StringFormat _headerFormat;

		#region Properties

		private TreeColumnCollection _owner;
		internal TreeColumnCollection Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

		[Browsable(false)]
		public int Index
		{
			get 
			{
				if (Owner != null)
					return Owner.IndexOf(this);
				else
					return -1;
			}
		}

		private string _header;
		[Localizable(true)]
		public string Header
		{
			get { return _header; }
			set 
			{ 
				_header = value;
				OnHeaderChanged();
			}
		}

		private int _width;
		[DefaultValue(50), Localizable(true)]
		public int Width
		{
			get { return _width; }
			set 
			{
				if (_width != value)
				{
					if (value < 0)
						throw new ArgumentOutOfRangeException("value");

					_width = value;
					OnWidthChanged();
				}
			}
		}

		private bool _visible = true;
		[DefaultValue(true)]
		public bool IsVisible
		{
			get { return _visible; }
			set 
			{ 
				_visible = value;
				OnIsVisibleChanged();
			}
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set 
			{
				if (value != _textAlign)
				{
					_textAlign = value;
					_headerFormat.Alignment = TextHelper.TranslateAligment(value);
					OnHeaderChanged();
				}
			}
		}


		private SortOrder _sort_order = SortOrder.None;
		public SortOrder SortOrder
		{
			get { return _sort_order; }
			set
			{
				if (value == _sort_order)
					return;
				_sort_order = value;
				OnSortOrderChanged();
			}
		}

		public Size SortMarkSize
		{
			get
			{
				if (Application.RenderWithVisualStyles)
					return new Size(9, 5);
				else
					return new Size(7, 4);
			}
		}
		#endregion

		public TreeColumn(): 
			this(string.Empty, 50)
		{
		}

		public TreeColumn(string header, int width)
		{
			_header = header;
			_width = width;

			_headerFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
			_headerFormat.LineAlignment = StringAlignment.Center;
			_headerFormat.Alignment = StringAlignment.Near;
			_headerFormat.Trimming = StringTrimming.EllipsisCharacter;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Header))
				return GetType().Name;
			else
				return Header;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				_headerFormat.Dispose();
		}

		#region Draw

		private static VisualStyleRenderer _normalRenderer;
		private static VisualStyleRenderer _pressedRenderer;
		private static VisualStyleRenderer _hotRenderer;

		private static void CreateRenderers()
		{
			if (Application.RenderWithVisualStyles && _normalRenderer == null)
			{
				_normalRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
				_pressedRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Pressed);
				_hotRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Hot);
			}
		}

		internal Bitmap CreateGhostImage(Rectangle bounds, Font font)
		{
			Bitmap b = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
			Graphics gr = Graphics.FromImage(b);
			gr.FillRectangle(SystemBrushes.ControlDark, bounds);
			DrawContent(gr, bounds, font);
			BitmapHelper.SetAlphaChanelValue(b, 150);
			return b;
		}

		internal void Draw(Graphics gr, Rectangle bounds, Font font, bool pressed, bool hot)
		{
			DrawBackground(gr, bounds, pressed, hot);
			DrawContent(gr, bounds, font);
		}

		private void DrawContent(Graphics gr, Rectangle bounds, Font font)
		{
			bounds = new Rectangle(bounds.X + HeaderMargin, bounds.Y, bounds.Width - HeaderMargin * 2, bounds.Height);

			if (SortOrder != SortOrder.None)
				bounds.Width -= (SortMarkSize.Width + SortOrderMarkMargin);

			SizeF textSize = gr.MeasureString(Header, font, bounds.Size, _headerFormat);
			gr.DrawString(Header, font, SystemBrushes.WindowText, bounds, _headerFormat);

			if (SortOrder != SortOrder.None)
			{
				int tw = (int)textSize.Width;
				int x = 0;
				if (TextAlign == HorizontalAlignment.Left)
					x = bounds.X + tw + SortOrderMarkMargin;
				else if (TextAlign == HorizontalAlignment.Right)
					x = bounds.Right + SortOrderMarkMargin;
				else
					x = bounds.X + tw + (bounds.Width - tw) / 2 + SortOrderMarkMargin;
				DrawSortMark(gr, new Rectangle(x, bounds.Y, 0, bounds.Height));
			}
		}

		private void DrawSortMark(Graphics gr, Rectangle bounds)
		{
			int x = bounds.X;
			int y = bounds.Y + bounds.Height / 2 - 2;

			int w2 = SortMarkSize.Width / 2;
			if (SortOrder == SortOrder.Ascending)
			{
				Point[] points = new Point[] { new Point(x, y), new Point(x + SortMarkSize.Width, y), new Point(x + w2, y + SortMarkSize.Height) };
				gr.FillPolygon(SystemBrushes.ControlDark, points);
			}
			else if (SortOrder == SortOrder.Descending)
			{
				Point[] points = new Point[] { new Point(x - 1, y + SortMarkSize.Height), new Point(x + SortMarkSize.Width, y + SortMarkSize.Height), new Point(x + w2, y - 1) };
				gr.FillPolygon(SystemBrushes.ControlDark, points);
			}
		}

		internal static void DrawDropMark(Graphics gr, Rectangle rect)
		{
			gr.FillRectangle(SystemBrushes.HotTrack, rect.X-1, rect.Y, 2, rect.Height);
		}

		internal static void DrawBackground(Graphics gr, Rectangle bounds, bool pressed, bool hot)
		{
			if (Application.RenderWithVisualStyles)
			{
				CreateRenderers();
				if (pressed)
					_pressedRenderer.DrawBackground(gr, bounds);
				else if (hot)
					_hotRenderer.DrawBackground(gr, bounds);
				else
					_normalRenderer.DrawBackground(gr, bounds);
			}
			else
			{
				gr.FillRectangle(SystemBrushes.Control, bounds);
				Pen p1 = SystemPens.ControlLightLight;
				Pen p2 = SystemPens.ControlDark;
				Pen p3 = SystemPens.ControlDarkDark;
				if (pressed)
					gr.DrawRectangle(p2, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				else
				{
					gr.DrawLine(p1, bounds.X, bounds.Y, bounds.Right, bounds.Y);
					gr.DrawLine(p3, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
					gr.DrawLine(p3, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
					gr.DrawLine(p1, bounds.Left, bounds.Y + 1, bounds.Left, bounds.Bottom - 2);
					gr.DrawLine(p2, bounds.Right - 2, bounds.Y + 1, bounds.Right - 2, bounds.Bottom - 2);
					gr.DrawLine(p2, bounds.X, bounds.Bottom - 1, bounds.Right - 2, bounds.Bottom - 1);
				}
			}
		}

		#endregion

		#region Events

		public event EventHandler HeaderChanged;
		private void OnHeaderChanged()
		{
			if (HeaderChanged != null)
				HeaderChanged(this, EventArgs.Empty);
		}

		public event EventHandler SortOrderChanged;
		private void OnSortOrderChanged()
		{
			if (SortOrderChanged != null)
				SortOrderChanged(this, EventArgs.Empty);
		}

		public event EventHandler IsVisibleChanged;
		private void OnIsVisibleChanged()
		{
			if (IsVisibleChanged != null)
				IsVisibleChanged(this, EventArgs.Empty);
		}

		public event EventHandler WidthChanged;
		private void OnWidthChanged()
		{
			if (WidthChanged != null)
				WidthChanged(this, EventArgs.Empty);
		}

		#endregion
	}
}
