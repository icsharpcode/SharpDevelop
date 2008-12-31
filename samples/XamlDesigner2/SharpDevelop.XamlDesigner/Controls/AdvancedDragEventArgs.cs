using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class AdvancedDragEventArgs
	{
		internal AdvancedDragEventArgs(AdvancedThumb thumb)
		{
			this.thumb = thumb;
		}

		AdvancedThumb thumb;

		public Point StartPoint
		{
			get { return thumb.StartPoint; }
		}

		public Vector Delta
		{
			get { return thumb.Delta; }
		}

		public bool IsCancel
		{
			get { return thumb.IsCancel; }
		}
	}

	public delegate void AdvancedDragEventHandler(object sender, AdvancedDragEventArgs e);
}
