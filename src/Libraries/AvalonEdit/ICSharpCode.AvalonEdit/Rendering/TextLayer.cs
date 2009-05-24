// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// The control that contains the text.
	/// 
	/// This control is used to allow other UIElements to be placed inside the TextView but
	/// behind the text.
	/// The text rendering process (VisualLine creation) is controlled by the TextView, this
	/// class simply displays the created Visual Lines.
	/// </summary>
	/// <remarks>
	/// This class does not contain any input handling and is invisible to hit testing. Input
	/// is handled by the TextView.
	/// This allows UIElements that are displayed behind the text, but still can react to mouse input.
	/// </remarks>
	sealed class TextLayer : Layer
	{
		public TextLayer(TextView textView) : base(textView, KnownLayer.Text)
		{
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			textView.RenderTextLayer(drawingContext);
		}
		
		#region Inline object handling
		internal List<InlineObjectRun> inlineObjects = new List<InlineObjectRun>();
		
		/// <summary>
		/// Adds a new inline object.
		/// </summary>
		internal void AddInlineObject(InlineObjectRun inlineObject)
		{
			inlineObjects.Add(inlineObject);
			AddVisualChild(inlineObject.Element);
			inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		}
		
		List<VisualLine> visualLinesWithOutstandingInlineObjects = new List<VisualLine>();
		
		internal void RemoveInlineObjects(VisualLine visualLine)
		{
			// Delay removing inline objects:
			// A document change immediately invalidates affected visual lines, but it does not
			// cause an immediate redraw.
			// To prevent inline objects from flickering when they are recreated, we delay removing
			// inline objects until the next redraw.
			visualLinesWithOutstandingInlineObjects.Add(visualLine);
		}
		
		internal void RemoveInlineObjectsNow()
		{
			inlineObjects.RemoveAll(
				ior => {
					if (visualLinesWithOutstandingInlineObjects.Contains(ior.VisualLine)) {
						ior.VisualLine = null;
						RemoveVisualChild(ior.Element);
						return true;
					}
					return false;
				});
			visualLinesWithOutstandingInlineObjects.Clear();
		}
		
		/// <summary>
		/// Removes the inline object that displays the specified UIElement.
		/// </summary>
		internal void RemoveInlineObject(UIElement element)
		{
			inlineObjects.RemoveAll(
				ior => {
					if (ior.Element == element) {
						ior.VisualLine = null;
						RemoveVisualChild(ior.Element);
						return true;
					}
					return false;
				});
		}
		
		/// <inheritdoc/>
		protected override int VisualChildrenCount {
			get { return inlineObjects.Count; }
		}
		
		/// <inheritdoc/>
		protected override Visual GetVisualChild(int index)
		{
			return inlineObjects[index].Element;
		}
		#endregion
	}
}
