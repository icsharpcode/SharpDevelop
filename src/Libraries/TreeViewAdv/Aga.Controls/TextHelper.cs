using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Aga.Controls
{
	public static class TextHelper
	{
		public static StringAlignment TranslateAligment(HorizontalAlignment aligment)
		{
			if (aligment == HorizontalAlignment.Left)
				return StringAlignment.Near;
			else if (aligment == HorizontalAlignment.Right)
				return StringAlignment.Far;
			else
				return StringAlignment.Center;
		}
	}
}
