// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Gui.TreeGrid
{
	public class DynamicListColumn : ICloneable
	{
		public const int DefaultWidth = 16;
		
		int width = DefaultWidth;
		int minimumWidth = DefaultWidth;
		bool allowGrow = true;
		bool autoSize = false;
		
		Brush backgroundBrush = SystemBrushes.ControlLight;
		public static readonly Color DefaultBackColor = SystemColors.ControlLight;
		
		public DynamicListColumn()
		{
		}
		
		/// <summary>Copy constructor</summary>
		protected DynamicListColumn(DynamicListColumn col)
		{
			width = col.width;
			minimumWidth = col.minimumWidth;
			allowGrow = col.allowGrow;
			autoSize = col.autoSize;
			backgroundBrush = col.backgroundBrush;
		}
		
		public virtual DynamicListColumn Clone()
		{
			return new DynamicListColumn(this);
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
		#endregion
	}
}
