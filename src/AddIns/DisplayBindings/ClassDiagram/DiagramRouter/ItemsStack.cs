/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 20:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

namespace Tools.Diagrams
{
	public enum Axis {X, Y, Z};

	public class ItemsStack : ItemsStack<IRectangle> {}
	
	public class ItemsStack<T> : BaseRectangle where T : IRectangle
	{
		List<T> items = new List<T>();
		Axis axis = Axis.Y;
		bool dontHandleResize;
		
		public ItemsStack()
		{
			dontHandleResize = true;
			base.Width = float.NaN;
			base.Height = float.NaN;
			dontHandleResize = false;
		}
		
		float minWidth = 0, minHeight = 0;
		float spacing = 0;
		bool modified = false;
		
		public void Add (T item)
		{
//			System.Diagnostics.Debug.WriteLine("ItemStack.Add");
			items.Add(item);
			item.Container = this;
			modified = true;
		}

		public void Remove (T item)
		{
//			System.Diagnostics.Debug.WriteLine("ItemStack.Remove");
			items.Remove(item);
			item.Container = null;
			modified = true;
		}
		
		public void Clear()
		{
//			System.Diagnostics.Debug.WriteLine("ItemStack.Clear");
			items.Clear();
			modified = true;
		}
		
		public Axis OrientationAxis
		{
			get { return axis; }
			set
			{
//				System.Diagnostics.Debug.WriteLine("ItemStack.set_OrientationAxis curr: " + axis + "; new: " + value);
				axis = value;
				modified = true;
			}
		}

		#region Width Calculations

		private float FindHeight()
		{
			if (!float.IsNaN(base.Height) || base.Height < 0) return base.Height;
			if (!float.IsNaN(base.ActualHeight) || base.ActualHeight < 0) return base.ActualHeight;
			float h = 0;
			foreach (IRectangle r in items)
				if (!float.IsNaN(r.ActualHeight) && r.ActualHeight >= 0)
					h = Math.Max(h, r.ActualHeight);
			return h;
		}
		
		private bool IsItemWidthValid(IRectangle r)
		{
			bool ret = true;
			if (float.IsNaN(r.Width) || r.Width < 0)
			{
				if (!r.KeepAspectRatio)
					ret = false;
			}
			return ret;
		}

		private float CalcUsedWidthSpace()
		{
			float usedSpace = 0.0f;
			
			foreach (IRectangle r in items)
			{
				if (IsItemWidthValid(r))
					usedSpace += (r.ActualWidth + spacing);
			}
			
			return usedSpace;
		}
			
		private float CalcWidthPerUndefined()
		{
			float width = base.ActualWidth;
			
			if (float.IsNaN(width) || width < 0)
				width = base.Width;

			if (float.IsNaN(width) || width < 0)
				return 0;
			
			int count = 0;
			foreach (IRectangle r in items)
				if (!IsItemWidthValid(r))
					count++;
			if (count == 0) return 0;
			
			float usedSpace = CalcUsedWidthSpace();
			return (width - usedSpace - (count-1) * spacing) / count;
		}

		private void HRecalc()
		{
			float x = Padding, w = Padding;
			float h = Math.Max(FindHeight(), minHeight);
			
			foreach (IRectangle r in items)
				r.ActualHeight = h - (r.Border + Padding) * 2;

			float spacePerUndefined = CalcWidthPerUndefined();

			foreach (IRectangle r in items)
			{
				r.X = x + r.Border;
				r.Y = r.Border + Padding;
				
				if (!IsItemWidthValid(r))
					r.ActualWidth = Math.Max(spacePerUndefined - r.Border * 2, 0);
				else if (float.IsNaN(r.ActualWidth) || r.ActualWidth < 0)
					r.ActualWidth = r.Width;
				
				w += r.ActualWidth + spacing + r.Border * 2;
				x = w;
			}
			
			dontHandleResize = true;
			base.ActualWidth = Math.Max(w - spacing, minWidth);
			base.ActualHeight = h;
			dontHandleResize = false;
		}
		
		#endregion

		#region Height Calculations
		
		private float FindWidth()
		{
			if (!float.IsNaN(base.Width) || base.Width < 0) return base.Width;
			if (!float.IsNaN(base.ActualWidth) || base.ActualWidth < 0) return base.ActualWidth;
			float w = 0;
			foreach (IRectangle r in items)
				if (!float.IsNaN(r.ActualWidth) && r.ActualWidth >= 0)
					w = Math.Max(w, r.ActualWidth);
			return w;
		}
				
		private bool IsItemHeightValid(IRectangle r)
		{
			bool ret = true;
			if (float.IsNaN(r.Height) || r.Height < 0)
			{
				if (!r.KeepAspectRatio)
					ret = false;
			}
			return ret;
		}
		
		private float CalcUsedHeightSpace()
		{
			float usedSpace = 0;
			
			foreach (IRectangle r in items)
				if (IsItemHeightValid(r))
					usedSpace += r.ActualHeight + spacing;
			
			return usedSpace;
		}
		
		private float CalcHeightPerUndefined()
		{
			float height = base.ActualHeight;
			
			if (float.IsNaN(height) || height < 0)
				height = base.Height;

			if (float.IsNaN(height) || height < 0)
				return 0;
			
			int count = 0;
			foreach (IRectangle r in items)
				if (!IsItemHeightValid(r))
					count++;
			if (count == 0) return 0;
			
			float usedSpace = CalcUsedHeightSpace();
			return(height - usedSpace - (count-1) * spacing) / count;
		}

		private void VRecalc()
		{
			if (items.Count == 0) return;

			float y = Padding, h = Padding;
			float w = Math.Max(FindWidth(), minWidth);

			foreach (IRectangle r in items)
				r.ActualWidth = w - (r.Border + Padding) * 2;

			float spacePerUndefined = CalcHeightPerUndefined();

			foreach (IRectangle r in items)
			{
				r.X = r.Border + Padding;
				r.Y = y + r.Border;
				
				if (!IsItemHeightValid(r))
					r.ActualHeight = Math.Max (spacePerUndefined - r.Border * 2, 0);
				else if (float.IsNaN(r.ActualHeight) || r.ActualHeight < 0)
					r.ActualHeight = r.Height;
					
				h += r.ActualHeight + spacing + r.Border * 2;
				y = h;
			}

			dontHandleResize = true;
			base.ActualWidth = w;
			base.ActualHeight = Math.Max(h - spacing + Padding, minHeight);
			dontHandleResize = false;
		}
			
		#endregion
		
		private void ZRecalc()
		{
			float w = Math.Max(FindWidth(), minWidth);
			float h = Math.Max(FindHeight(), minHeight);
			
			foreach (IRectangle r in items)
			{
				float bp = r.Border + Padding;
				r.X = bp;
				r.Y = bp;
				r.ActualWidth = w - bp * 2;
				r.ActualHeight = h - bp * 2;
			}
			
			dontHandleResize = true;
			base.ActualWidth = w;
			base.ActualHeight = h;
			dontHandleResize = false;
		}
		
		public void Recalculate()
		{
			if (!modified) return;
			if (dontHandleResize) return;
			
			if (axis == Axis.X)
				HRecalc();
			else if (axis == Axis.Y)
				VRecalc();
			else if (axis == Axis.Z)
				ZRecalc();
			
			modified = false;
		}
		
		protected override void OnActualSizeChanged()
		{
//			System.Diagnostics.Debug.WriteLine("ItemStack.OnActualSizeChanged");
			modified = true;
		}
		
		protected override void OnSizeChanged()
		{
			ResetActualSize();
		}
		
		#region Geometry

		public float MinHeight
		{
			get { return minHeight; }
			set
			{
//				System.Diagnostics.Debug.WriteLine("ItemStack.set_MinHeight curr: " + minHeight + "; new: " + value);
				minHeight = value;
				modified = true;
			}
		}
		
		public float MinWidth
		{
			get { return minWidth; }
			set
			{
//				System.Diagnostics.Debug.WriteLine("ItemStack.set_MinWidth curr: " + minWidth + "; new: " + value);
				minWidth = value;
				modified = true;
			}
		}
		
		public override float Width
		{
			get
			{
				Recalculate();
				return base.Width;
			}
			set { base.Width = value; }
		}
		
		public override float Height 
		{
			get
			{
				Recalculate();
				return base.Height;
			}
			set { base.Height = value; }
		}
		
		public override float ActualWidth
		{
			get 
			{
				Recalculate();
				return base.ActualWidth;
			}
			set { base.ActualWidth = value; }
		}
		
		public override float ActualHeight
		{
			get
			{
				Recalculate();
				return base.ActualHeight;
			}
			set { base.ActualHeight = value; }
		}
		#endregion
		
		public float Spacing
		{
			get { return spacing; }
			set
			{
//				System.Diagnostics.Debug.WriteLine("ItemStack.set_Spacing curr: " + spacing + "; new: " + value);
				spacing = value;
				modified = true;
			}
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return items.GetEnumerator();
		}
		
		public int Count
		{
			get { return items.Count; }
		}
		
		public override float GetAbsoluteContentWidth()
		{
			float w = 0;
			if (axis == Axis.X || axis == Axis.Z)
			{
				foreach (T item in items)
					w += item.GetAbsoluteContentWidth() + item.Border * 2 + spacing;
				w = Math.Max(w - spacing, 0);
			}
			else if (axis == Axis.Y)
			{
				foreach (T item in items)
					w = Math.Max(w, item.GetAbsoluteContentWidth() + item.Border * 2);
			}
			return w;
		}
		
		public override float GetAbsoluteContentHeight()
		{
			float h = 0;
			if (axis == Axis.X || axis == Axis.Z)
			{
				foreach (T item in items)
					h = Math.Max(h, item.GetAbsoluteContentHeight() + item.Border * 2);
			}
			else if (axis == Axis.Y)
			{
				foreach (T item in items)
					h += item.GetAbsoluteContentHeight() + item.Border * 2 + spacing;
				h = Math.Max(h - spacing, 0);
			}
			return h;
		}
		
		public override string ToString()
		{
			if (items.Count > 0)
				return "ItemStack - first item: " + items[0].ToString();
			else
				return base.ToString();
		}
	}
}
