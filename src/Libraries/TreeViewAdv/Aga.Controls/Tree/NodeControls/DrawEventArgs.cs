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

		public DrawEventArgs(TreeNodeAdv node, DrawContext context)
			: base(node)
		{
			_context = context;
		}
	}
}
