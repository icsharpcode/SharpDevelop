// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//#define DEBUG_ADORNERLAYER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A control that displays adorner panels.
	/// </summary>
	public sealed class AdornerLayer : Panel
	{
		#region AdornerPanelCollection
		internal sealed class AdornerPanelCollection : ICollection<AdornerPanel>, IReadOnlyCollection<AdornerPanel>
		{
			readonly AdornerLayer _layer;
			
			public AdornerPanelCollection(AdornerLayer layer)
			{
				this._layer = layer;
			}
			
			public int Count {
				get { return _layer.Children.Count; }
			}
			
			public bool IsReadOnly {
				get { return false; }
			}
			
			public void Add(AdornerPanel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				_layer.AddAdorner(item);
			}
			
			public void Clear()
			{
				_layer.ClearAdorners();
			}
			
			public bool Contains(AdornerPanel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				return VisualTreeHelper.GetParent(item) == _layer;
			}
			
			public void CopyTo(AdornerPanel[] array, int arrayIndex)
			{
				foreach (AdornerPanel panel in this)
					array[arrayIndex++] = panel;
			}
			
			public bool Remove(AdornerPanel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				return _layer.RemoveAdorner(item);
			}
			
			public IEnumerator<AdornerPanel> GetEnumerator()
			{
				foreach (AdornerPanel panel in _layer.Children) {
					yield return panel;
				}
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}
		#endregion
		
		AdornerPanelCollection _adorners;
		readonly UIElement _designPanel;
		
		#if DEBUG_ADORNERLAYER
		int _totalAdornerCount;
		#endif
		
		internal AdornerLayer(UIElement designPanel)
		{
			this._designPanel = designPanel;
			
			this.LayoutUpdated += OnLayoutUpdated;
			
			_adorners = new AdornerPanelCollection(this);
		}
		
		void OnLayoutUpdated(object sender, EventArgs e)
		{
			UpdateAllAdorners(false);
			#if DEBUG_ADORNERLAYER
			Debug.WriteLine("Adorner LayoutUpdated. AdornedElements=" + _dict.Count +
							", visible adorners=" + VisualChildrenCount + ", total adorners=" + (_totalAdornerCount));
			#endif
		}
		
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			UpdateAllAdorners(true);
		}
		
		internal AdornerPanelCollection Adorners {
			get {
				return _adorners;
			}
		}
		
		sealed class AdornerInfo
		{
			internal readonly List<AdornerPanel> adorners = new List<AdornerPanel>();
			internal bool isVisible;
			internal Rect position;
		}
		
		// adorned element => AdornerInfo
		Dictionary<UIElement, AdornerInfo> _dict = new Dictionary<UIElement, AdornerInfo>();
		
		void ClearAdorners()
		{
			if (_dict.Count == 0)
				return; // already empty
			
			this.Children.Clear();
			_dict = new Dictionary<UIElement, AdornerInfo>();
			
			#if DEBUG_ADORNERLAYER
			_totalAdornerCount = 0;
			Debug.WriteLine("AdornerLayer cleared.");
			#endif
		}
		
		AdornerInfo GetOrCreateAdornerInfo(UIElement adornedElement)
		{
			AdornerInfo info;
			if (!_dict.TryGetValue(adornedElement, out info)) {
				info = _dict[adornedElement] = new AdornerInfo();
				info.isVisible = adornedElement.IsDescendantOf(_designPanel);
			}
			return info;
		}
		
		AdornerInfo GetExistingAdornerInfo(UIElement adornedElement)
		{
			AdornerInfo info;
			_dict.TryGetValue(adornedElement, out info);
			return info;
		}
		
		void AddAdorner(AdornerPanel adornerPanel)
		{
			if (adornerPanel.AdornedElement == null)
				throw new DesignerException("adornerPanel.AdornedElement must be set");
			
			AdornerInfo info = GetOrCreateAdornerInfo(adornerPanel.AdornedElement);
			info.adorners.Add(adornerPanel);
			
			if (info.isVisible) {
				AddAdornerToChildren(adornerPanel);
			}
			
			#if DEBUG_ADORNERLAYER
			Debug.WriteLine("Adorner added. AdornedElements=" + _dict.Count +
							", visible adorners=" + VisualChildrenCount + ", total adorners=" + (++_totalAdornerCount));
			#endif
		}
		
		void AddAdornerToChildren(AdornerPanel adornerPanel)
		{
			UIElementCollection children = this.Children;
			int i = 0;
			for (i = 0; i < children.Count; i++) {
				AdornerPanel p = (AdornerPanel)children[i];
				if (p.Order > adornerPanel.Order) {
					break;
				}
			}
			children.Insert(i, adornerPanel);
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			foreach (AdornerPanel adorner in this.Children) {
				adorner.Measure(infiniteSize);
			}
			return new Size(0, 0);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (AdornerPanel adorner in this.Children) {
				if (adorner.AdornedElement.IsDescendantOf(_designPanel))
				{
					var transform = adorner.AdornedElement.TransformToAncestor(_designPanel);
					var rt =  transform as MatrixTransform;
					if (rt != null && adorner.AdornedDesignItem != null && adorner.AdornedDesignItem.Parent != null && adorner.AdornedDesignItem.Parent.View is Canvas && adorner.AdornedElement.RenderSize.Height == 0 && adorner.AdornedElement.RenderSize.Width == 0)
					{
						var width = ((FrameworkElement) adorner.AdornedElement).Width;
						width = width > 0 ? width : 2.0;
						var height = ((FrameworkElement)adorner.AdornedElement).Height;
						height = height > 0 ? height : 2.0;
						var xOffset = rt.Matrix.OffsetX - (width / 2);
						var yOffset = rt.Matrix.OffsetY - (height / 2);
						rt = new MatrixTransform(new Matrix(rt.Matrix.M11, rt.Matrix.M12, rt.Matrix.M21, rt.Matrix.M22, xOffset, yOffset));
					}
					else if (transform is GeneralTransformGroup)
					{
						//var intTrans = ((GeneralTransformGroup) transform).Children.FirstOrDefault(x => x.GetType().Name == "GeneralTransform2DTo3DTo2D");
						//var prp = intTrans.GetType().GetField("_worldTransformation", BindingFlags.Instance | BindingFlags.NonPublic);
						//var mtx = (Matrix3D) prp.GetValue(intTrans);
						//var mtx2D = new Matrix(mtx.M11, mtx.M12, mtx.M21, mtx.M22, mtx.OffsetX, mtx.OffsetY);
						//rt = new MatrixTransform(mtx2D);
						rt = ((GeneralTransformGroup)transform).Children.OfType<MatrixTransform>().LastOrDefault();
					}


					adorner.RenderTransform = rt;
				}

				adorner.Arrange(new Rect(new Point(0, 0), adorner.DesiredSize));
			}
			return finalSize;
		}
		
		bool RemoveAdorner(AdornerPanel adornerPanel)
		{
			if (adornerPanel.AdornedElement == null)
				return false;
			
			AdornerInfo info = GetExistingAdornerInfo(adornerPanel.AdornedElement);
			if (info == null)
				return false;
			
			if (info.adorners.Remove(adornerPanel)) {
				if (info.isVisible) {
					this.Children.Remove(adornerPanel);
				}
				
				if (info.adorners.Count == 0) {
					_dict.Remove(adornerPanel.AdornedElement);
				}
				
				#if DEBUG_ADORNERLAYER
				Debug.WriteLine("Adorner removed. AdornedElements=" + _dict.Count +
								", visible adorners=" + VisualChildrenCount + ", total adorners=" + (--_totalAdornerCount));
				#endif
				
				return true;
			} else {
				return false;
			}
		}
		
		public void UpdateAdornersForElement(UIElement element, bool forceInvalidate)
		{
			AdornerInfo info = GetExistingAdornerInfo(element);
			if (info != null) {
				UpdateAdornersForElement(element, info, forceInvalidate);
			}
		}
		
		Rect GetPositionCache(UIElement element)
		{
			var t = element.TransformToAncestor(_designPanel);
			var p = t.Transform(new Point(0, 0));
			return new Rect(p, element.RenderSize);
		}
		
		void UpdateAdornersForElement(UIElement element, AdornerInfo info, bool forceInvalidate)
		{
			if (element.IsDescendantOf(_designPanel)) {
				if (!info.isVisible) {
					info.isVisible = true;
					// make adorners visible:
					info.adorners.ForEach(AddAdornerToChildren);
				}
				Rect c = GetPositionCache(element);
				if (forceInvalidate || !info.position.Equals(c)) {
					info.position = c;
					foreach (AdornerPanel p in info.adorners) {
						p.InvalidateMeasure();
					}
					this.InvalidateArrange();
				}
			} else {
				if (info.isVisible) {
					info.isVisible = false;
					// make adorners invisible:
					info.adorners.ForEach(this.Children.Remove);
				}
			}
		}
		
		void UpdateAllAdorners(bool forceInvalidate)
		{
			foreach (KeyValuePair<UIElement, AdornerInfo> pair in _dict) {
				UpdateAdornersForElement(pair.Key, pair.Value, forceInvalidate);
			}
		}
	}
}
