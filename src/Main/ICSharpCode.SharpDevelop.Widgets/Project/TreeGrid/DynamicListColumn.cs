// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public class DynamicListColumn : ICloneable
	{
		public const int DefaultWidth = 16;
		
		int width = DefaultWidth;
		int minimumWidth = DefaultWidth;
		bool allowGrow = true;
		bool autoSize = false;
		
		public static readonly Color DefaultBackColor = Color.FromArgb(247, 245, 233);
		public static readonly Brush DefaultBackBrush = new SolidBrush(DefaultBackColor);
		public static readonly Color DefaultRowHighlightBackColor = Color.FromArgb(221, 218, 203);
		public static readonly Brush DefaultRowHighlightBrush = new SolidBrush(DefaultRowHighlightBackColor);
		public static readonly Color DefaultInactiveBackColor = Color.FromArgb(242, 240, 228);
		public static readonly Brush DefaultInactiveBackBrush = new SolidBrush(DefaultInactiveBackColor);
		
		Brush backgroundBrush = DefaultBackBrush;
		Brush backgroundBrushInactive = DefaultInactiveBackBrush;
		Brush rowHighlightBrush = DefaultRowHighlightBrush;
		
		public virtual DynamicListColumn Clone()
		{
			return (DynamicListColumn)base.MemberwiseClone();
		}
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		
		#region Properties
		public int MinimumWidth {
			get {
				return minimumWidth;
			}
			set {
				if (value < 2)
					throw new ArgumentOutOfRangeException("value", value, "MinimumWidth must be at least 2");
				if (minimumWidth != value) {
					minimumWidth = value;
					if (MinimumWidthChanged != null) {
						MinimumWidthChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler MinimumWidthChanged;
		
		public int Width {
			get {
				return width;
			}
			set {
				if (value < 2)
					throw new ArgumentOutOfRangeException("value", value, "Width must be at least 2");
				if (width != value) {
					width = value;
					if (WidthChanged != null) {
						WidthChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler WidthChanged;
		
		public bool AllowGrow {
			get {
				return allowGrow;
			}
			set {
				allowGrow = value;
			}
		}
		
		public bool AutoSize {
			get {
				return autoSize;
			}
			set {
				autoSize = value;
			}
		}
		
		public Brush BackgroundBrush {
			get {
				return backgroundBrush;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				backgroundBrush = value;
			}
		}
		
		public Brush BackgroundBrushInactive {
			get {
				return backgroundBrushInactive;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				backgroundBrushInactive = value;
			}
		}
		
		public Brush RowHighlightBrush {
			get {
				return rowHighlightBrush;
			}
			set {
				rowHighlightBrush = value;
			}
		}
		
		Color columnSeperatorColor = Color.Empty;
		
		/// <summary>
		/// Sets the color that is used to the right of this column as separator color.
		/// </summary>
		public Color ColumnSeperatorColor {
			get {
				return columnSeperatorColor;
			}
			set {
				columnSeperatorColor = value;
			}
		}
		
		Color columnSeperatorColorInactive = Color.Empty;
		
		/// <summary>
		/// Sets the color that is used to the right of this column as separator color.
		/// </summary>
		public Color ColumnSeperatorColorInactive {
			get {
				return columnSeperatorColorInactive;
			}
			set {
				columnSeperatorColorInactive = value;
			}
		}
		#endregion
	}
}
