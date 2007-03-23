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
	public interface IRectangle
	{
		/// <summary>
		/// The X position of the rectangular item, relative to its container.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
		float X { get; set; }
		
		/// <summary>
		/// The Y position of the rectangular item, relative to its container.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
		float Y { get; set; }
		
			
		/// <summary>
		/// The X position of the rectangular item, relative to the root container.
		/// </summary>
		float AbsoluteX { get; }
		
		/// <summary>
		/// The Y position of the rectangular item, relative to the root container.
		/// </summary>
		float AbsoluteY { get; }
		
		/// <summary>
		/// The visible width of the item.
		/// Layout managers such as ItemsStack change this value to define the width of
		/// the item.
		/// The value returned from this property must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>
		float ActualWidth { get; set; }

		/// <summary>
		/// The visible height of the item.
		/// Layout managers such as ItemsStack change this value to define the height of
		/// the item.
		/// The value returned from this property must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>
		float ActualHeight { get; set; }
		
		/// <summary>
		/// The defined width of the item.
		/// </summary>
		/// <remarks>
		/// A negative value means the the width is undefined, and is due to change by
		/// layout managers, such as ItemsStack. In that case, ActualWidth is set to the
		/// wanted value minus twice the border size.
		/// </remarks>
		float Width { get; set; }
		
		/// <summary>
		/// The defined height of the item.
		/// </summary>
		/// <remarks>
		/// A negative value means the the height is undefined, and is due to change by
		/// layout managers, such as ItemsStack. In that case, ActualHeight is set to the
		/// wanted value minus twice the border size.
		/// </remarks>
		float Height { get; set; }
		
		/// <summary>
		/// The distance between the item borders to its container's content borders.
		/// </summary>
		float Border { get; set; }

		/// <summary>
		/// The distance between the item borders to its content.
		/// </summary>
		float Padding { get; set; }
		
		/// <summary>
		/// The width of the item content disregarding defined of visible modifying values,
		/// such as border or width.
		/// The value returned must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>
		float GetAbsoluteContentWidth ();
		
		/// <summary>
		/// The height of the item content disregarding defined of visible modifying values,
		/// such as border or height.
		/// The value returned must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>		
		float GetAbsoluteContentHeight ();

		bool KeepAspectRatio { get; set; }
		
		/// <summary>
		/// If the implementing object is contained within another rectangle,
		/// this property points to the container.
		/// </summary>
		IRectangle Container { get; set; }
		
		bool IsHResizable { get; }
		bool IsVResizable { get; }
		
		event EventHandler AbsolutePositionChanged;
		event EventHandler WidthChanged;
		event EventHandler HeightChanged;
		event EventHandler ActualWidthChanged;
		event EventHandler ActualHeightChanged;
	}
}
