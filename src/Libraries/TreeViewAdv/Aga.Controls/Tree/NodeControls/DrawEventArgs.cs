using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Aga.Controls.Tree.NodeControls
{
	public class DrawEventArgs : NodeEventArgs
	{
		private DrawContext _context;
		public DrawContext Context
		{
			get { return _context; }
		}

		private Brush _textBrush;
		[Obsolete("Use TextColor")]
		public Brush TextBrush
		{
			get { return _textBrush; }
			set { _textBrush = value; }
		}

		private Brush _backgroundBrush;
		public Brush BackgroundBrush
		{
            get { return _backgroundBrush; }
			set { _backgroundBrush = value; }
		}

		private Font _font;
		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private Color _textColor;
		public Color TextColor
		{
			get { return _textColor; }
			set { _textColor = value; }
		}

		private string _text;
		public string Text
		{
			get { return _text; }
		}

		public DrawEventArgs(TreeNodeAdv node, DrawContext context, string text)
			: base(node)
		{
			_context = context;
			_text = text;
		}
	}
}
