// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 320 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// Description of OverlayAdorner.
	/// </summary>
	public class OverlayAdorner : Adorner
	{
		UIElementCollection uiElements;
		UIElement child;
		
		public OverlayAdorner(UIElement adornedElement) : base(adornedElement)
		{
			this.uiElements = new UIElementCollection(this, this);
		}
		
		public UIElement Child {
			get { return child; }
			set {
				if (child != null) {
					uiElements.Remove(child);
				}
				child = value;
				if (value != null) {
					uiElements.Add(value);
				}
			}
		}
		
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return uiElements.GetEnumerator();
			}
		}
		
		protected override System.Windows.Media.Visual GetVisualChild(int index)
		{
			return uiElements[index];
		}
		
		protected override int VisualChildrenCount {
			get { return uiElements.Count; }
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (child != null) {
				child.Arrange(new Rect(new Point(0, 0), finalSize));
			}
			return base.ArrangeOverride(finalSize);
		}
	}
}
