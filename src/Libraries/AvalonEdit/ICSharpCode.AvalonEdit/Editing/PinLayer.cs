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
	public class PinLayer : Layer
	{
		private Canvas pinningSurface;
		
		/// <summary>
		/// PinLayer constructor.
		/// </summary>
		/// <param name="textArea">Text area for this layer.</param>
		public PinLayer(TextArea textArea) : base(textArea.TextView, KnownLayer.Pins)
		{
			pinningSurface = new Canvas();
			this.AddVisualChild(pinningSurface);
			textView.VisualLinesChanged += new EventHandler(textView_VisualLinesChanged);	
		}

		void textView_VisualLinesChanged(object sender, EventArgs e)
		{
			Console.WriteLine(textView.HorizontalOffset + " " + textView.VerticalOffset);
		}
		
		/// <summary>
		/// Pins an element;
		/// </summary>
		/// <param name="element">Element to pin.</param>
		public void Pin(Popup element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");
			
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