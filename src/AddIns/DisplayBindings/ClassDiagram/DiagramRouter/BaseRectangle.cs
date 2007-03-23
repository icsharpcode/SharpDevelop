/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 03/11/2006
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Tools.Diagrams
{
	public abstract class BaseRectangle : IRectangle
	{
		private float x, y;
		private float w = float.NaN, h = float.NaN;
		private float b, p;
		private float aw = float.NaN, ah = float.NaN;
		private bool keepAspectRatio = false;
		
		private IRectangle container;
		
		public string Name;
		
		public IRectangle Container 
		{
			get { return container; }
			set
			{
				if (container != null)
					container.AbsolutePositionChanged -= HandleAbsolutePositionChanged;
				container = value;
				if (container != null)
					container.AbsolutePositionChanged += HandleAbsolutePositionChanged;
			}
		}
			
		#region Geometry
		
		public virtual float X
		{
			get { return x; }
			
			set
			{
				if (x == value) return;
				x = value;
				OnAbsolutePositionChanged();
			}
		}
	
		public virtual float Y
		{
			get { return y; }
			set
			{
				if (y == value) return;
				y = value;
				OnAbsolutePositionChanged();
			}
		}
		
		public virtual float AbsoluteX
		{
			get
			{
				if (container != null)
					return container.AbsoluteX + X;
				else
					return X;
			}
		}
	
		public virtual float AbsoluteY
		{
			get
			{
				if (container != null)
					return container.AbsoluteY + Y;
				else
					return Y;
			}
		}
		
		public void ResetActualSize()
		{
			aw = float.NaN;
			ah = float.NaN;
			OnActualSizeChanged();
		}
		
		public virtual float ActualWidth
		{
			get { return aw; }
			set
			{
				if (aw == value) return;
				aw = value;
				if (keepAspectRatio)
					ah = aw * (GetAbsoluteContentHeight() / GetAbsoluteContentWidth());
				OnActualSizeChanged();
				OnActualWidthChanged();
			}
		}
		
		public virtual float ActualHeight
		{
			get { return ah; }
			set
			{
				if (ah == value) return;
				ah = value;
				if (keepAspectRatio)
					aw = ah * (GetAbsoluteContentWidth() / GetAbsoluteContentHeight());
				OnActualSizeChanged();
				OnActualHeightChanged();
			}
		}
		
		public virtual float Width
		{
			get { return w; }
			set
			{
				if (w == value) return;
				w = value;
				OnSizeChanged();
				OnWidthChanged();
			}
		}

		public virtual float Height
		{
			get { return h; }
			set
			{
				if (h == value) return;
				h = value;
				OnSizeChanged();
				OnHeightChanged();
			}
		}
				
		#endregion
		
		public virtual float Border
		{
			get { return b; }
			set { b = value; }
		}
				
		public virtual float Padding
		{
			get { return p; }
			set { p = value; }
		}
		
		public virtual float GetAbsoluteContentWidth()
		{
			if (float.IsNaN(w) || w < 0)
				return 0;
			return w;
		}
		
		public virtual float GetAbsoluteContentHeight()
		{
			if (float.IsNaN(h) || h < 0)
				return 0;
			return h;
		}
		
		public bool KeepAspectRatio
		{
			get { return keepAspectRatio; }
			set { keepAspectRatio = value; }
		}
		
		protected virtual void HandleAbsolutePositionChanged(object sender, EventArgs e)
		{
			OnAbsolutePositionChanged();
		}
		
		protected virtual void OnAbsolutePositionChanged()
		{
			AbsolutePositionChanged(this, EventArgs.Empty);
		}
		
		protected virtual void OnSizeChanged() {}
		protected virtual void OnWidthChanged()
		{
			WidthChanged(this, EventArgs.Empty);
		}
		
		protected virtual void OnHeightChanged()
		{
			HeightChanged(this, EventArgs.Empty);
		}
		
		protected virtual void OnActualSizeChanged() {}
	
		protected virtual void OnActualWidthChanged()
		{
			ActualWidthChanged(this, EventArgs.Empty);
		}
			
		protected virtual void OnActualHeightChanged()
		{
			ActualHeightChanged(this, EventArgs.Empty);
		}
		
		public virtual bool IsHResizable
		{
			get { return true; }
		}
		
		public virtual bool IsVResizable
		{
			get { return true; }
		}
		
		public event EventHandler AbsolutePositionChanged = delegate {};
		public event EventHandler WidthChanged = delegate {};
		public event EventHandler HeightChanged = delegate {};
		public event EventHandler ActualWidthChanged = delegate {};
		public event EventHandler ActualHeightChanged = delegate {};
		
		public override string ToString()
		{
			return Name + " (" + this.GetType().ToString() + ")";
		}
		
	}
}
