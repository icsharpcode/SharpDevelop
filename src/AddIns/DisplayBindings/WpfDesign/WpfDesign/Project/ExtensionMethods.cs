// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Extension methods used in the WPF designer.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Rounds position and size of a Rect to PlacementInformation.BoundsPrecision digits. 
		/// </summary>
		public static Rect Round(this Rect rect)
		{
			return new Rect(
				Math.Round(rect.X, PlacementInformation.BoundsPrecision),
				Math.Round(rect.Y, PlacementInformation.BoundsPrecision),
				Math.Round(rect.Width, PlacementInformation.BoundsPrecision),
				Math.Round(rect.Height, PlacementInformation.BoundsPrecision)
			);
		}
	}
}
