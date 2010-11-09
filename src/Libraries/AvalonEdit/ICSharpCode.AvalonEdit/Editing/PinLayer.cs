// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Pin layer class. This class handles the pinning and unpinning operations.
	/// </summary>
	public class PinLayer : Layer, IWeakEventListener
	{
		private Canvas pinningSurface;
		
		/// <summary>
		/// PinLayer constructor.
		/// </summary>
		/// <param name="textArea">Text area for this layer.</param>
		public PinLayer(TextArea textArea) : base(textArea.TextView, KnownLayer.Pins)
		{
			pinningSurface = new Canvas();
			pinningSurface.Background = Brushes.Red;
			
			pinningSurface.HorizontalAlignment = HorizontalAlignment.Stretch;
			pinningSurface.VerticalAlignment = VerticalAlignment.Stretch;
			
			TextViewWeakEventManager.VisualLinesChanged.AddListener(textArea.TextView, this);
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextViewWeakEventManager.VisualLinesChanged))
			{
				pinningSurface.InvalidateVisual();
				InvalidateVisual();
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Pins an element;
		/// </summary>
		/// <param name="element">Element to pin.</param>
		public void Pin(Popup element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");
			
			element.Placement = PlacementMode.Absolute;
			
			pinningSurface.SetValue(Canvas.TopProperty, element.VerticalOffset);
			pinningSurface.SetValue(Canvas.LeftProperty, element.HorizontalOffset);
			pinningSurface.Children.Add(element);
		}
		
		/// <summary>
		/// Unpins an element.
		/// </summary>
		/// <param name="element">Element to unpin.</param>
		public void Unpin(Popup element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");
			
			pinningSurface.Children.Remove(element);
		}
	}
}