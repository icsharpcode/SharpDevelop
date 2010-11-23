// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Base class for known layers.
	/// </summary>
	class Layer : Canvas
	{
		/// <summary>
		/// Text view.
		/// </summary>
		protected readonly TextView textView;
		
		/// <summary>
		/// Known layer.
		/// </summary>
		protected readonly KnownLayer knownLayer;
		
		/// <summary>
		/// Constructor for base layer class.
		/// </summary>
		/// <param name="textView">Text view</param>
		/// <param name="knownLayer">Known layer.</param>
		public Layer(TextView textView, KnownLayer knownLayer)
		{
			Debug.Assert(textView != null);
			this.textView = textView;
			this.knownLayer = knownLayer;
			this.Focusable = false;
		}
		
		/// <summary>
		/// Gets layer type
		/// </summary>
		public KnownLayer LayerType { get { return knownLayer; } }
		
		/// <summary>
		/// HitTestCore method.
		/// </summary>
		/// <param name="hitTestParameters">Hit test parameters.</param>
		/// <returns>A GeometryHitTestResult.</returns>
		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return null;
		}
		
		/// <summary>
		/// HitTestCore method.
		/// </summary>
		/// <param name="hitTestParameters">Hit test parameters.</param>
		/// <returns>A HitTestResult.</returns>
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}
		
		/// <summary>
		/// Override for Render.
		/// </summary>
		/// <param name="drawingContext">Drawing context.</param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			textView.RenderBackground(drawingContext, knownLayer);
		}
	}
}
