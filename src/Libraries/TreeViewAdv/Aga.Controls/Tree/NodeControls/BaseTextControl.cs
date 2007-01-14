using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class BaseTextControl : EditableControl
	{
		private StringFormat _format;
		private Pen _focusPen;

		#region Properties

		private Font _font = null;
		public Font Font
		{
			get
			{
				if (_font == null)
					return Control.DefaultFont;
				else
					return _font;
			}
			set
			{
				if (value == Control.DefaultFont)
					_font = null;
				else
					_font = value;
			}
		}

		protected bool ShouldSerializeFont()
		{
			return (_font != null);
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set { _textAlign = value; }
		}

		private StringTrimming _trimming = StringTrimming.None;
		[DefaultValue(StringTrimming.None)]
		public StringTrimming Trimming
		{
			get { return _trimming; }
			set { _trimming = value; }
		}

		private bool _displayHiddenContentInToolTip = true;
		[DefaultValue(true)]
		public bool DisplayHiddenContentInToolTip
		{
			get { return _displayHiddenContentInToolTip; }
			set { _displayHiddenContentInToolTip = value; }
		}

		#endregion

		protected BaseTextControl()
		{
			IncrementalSearchEnabled = true;
			_focusPen = new Pen(Color.Black);
			_focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

			_format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox);
			_format.LineAlignment = StringAlignment.Center;
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context);
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context)
		{
			Font font = context.Font;
			if (DrawText != null)
			{
				DrawEventArgs args = new DrawEventArgs(node, context);
				args.Font = context.Font;
				OnDrawText(args);
				font = args.Font;
			}
			SizeF s = context.Graphics.MeasureString(GetLabel(node), font);
			if (!s.IsEmpty)
				return new Size((int)s.Width, (int)s.Height);
			else
				return new Size(10, Font.Height);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if (context.CurrentEditorOwner == this && node == Parent.CurrentNode)
				return;

			_format.Alignment = TextHelper.TranslateAligment(TextAlign);
			_format.Trimming = Trimming;
			string label = GetLabel(node);
			Rectangle bounds = GetBounds(node, context);
			Rectangle focusRect = new Rectangle(context.Bounds.Location,
				new Size(bounds.Width, context.Bounds.Height));

			Brush textBrush, backgroundBrush;
			Font font;
			CreateBrushes(node, context, out textBrush, out backgroundBrush, out font);

			if (backgroundBrush != null)
				context.Graphics.FillRectangle(backgroundBrush, focusRect);
			if (context.DrawFocus)
			{
				focusRect.Width--;
				focusRect.Height--;
				context.Graphics.DrawRectangle(Pens.Gray, focusRect);
				context.Graphics.DrawRectangle(_focusPen, focusRect);
			}
			context.Graphics.DrawString(label, font, textBrush, bounds, _format);
		}

		private void CreateBrushes(TreeNodeAdv node, DrawContext context, out Brush textBrush, out Brush backgroundBrush, out Font font)
		{
			textBrush = SystemBrushes.ControlText;
			backgroundBrush = null;
			font = context.Font;
			if (context.DrawSelection == DrawSelectionMode.Active)
			{
				textBrush = SystemBrushes.HighlightText;
				backgroundBrush = SystemBrushes.Highlight;
			}
			else if (context.DrawSelection == DrawSelectionMode.Inactive)
			{
				textBrush = SystemBrushes.ControlText;
				backgroundBrush = SystemBrushes.InactiveBorder;
			}
			else if (context.DrawSelection == DrawSelectionMode.FullRowSelect)
				textBrush = SystemBrushes.HighlightText;

			if (!context.Enabled)
				textBrush = SystemBrushes.GrayText;

			if (DrawText != null)
			{
				DrawEventArgs args = new DrawEventArgs(node, context);
				args.TextBrush = textBrush;
				args.BackgroundBrush = backgroundBrush;
				args.Font = font;
				OnDrawText(args);
				textBrush = args.TextBrush;
				backgroundBrush = args.BackgroundBrush;
				font = args.Font;
			}
		}

		public string GetLabel(TreeNodeAdv node)
		{
			if (node.Tag != null)
			{
				object obj = GetValue(node);
				if (obj != null)
					return FormatLabel(obj);
			}
			return string.Empty;
		}

		protected virtual string FormatLabel(object obj)
		{
			return obj.ToString();
		}

		public void SetLabel(TreeNodeAdv node, string value)
		{
			SetValue(node, value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_focusPen.Dispose();
				_format.Dispose();
			}
		}

		/// <summary>
		/// Fires when control is going to draw a text. Can be used to change text or back color
		/// </summary>
		public event EventHandler<DrawEventArgs> DrawText;
		protected virtual void OnDrawText(DrawEventArgs args)
		{
			if (DrawText != null)
				DrawText(this, args);
		}
	}
}
