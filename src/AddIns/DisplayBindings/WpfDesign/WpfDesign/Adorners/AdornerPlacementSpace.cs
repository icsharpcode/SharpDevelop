// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2415 $</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Adorners
{
	/// <summary>
	/// Describes the space in which an adorner is placed.
	/// </summary>
	public enum AdornerPlacementSpace
	{
		/// <summary>
		/// The adorner is affected by the render transform of the adorned element.
		/// </summary>
		Render,
		/// <summary>
		/// The adorner is affected by the layout transform of the adorned element.
		/// </summary>
		Layout,
		/// <summary>
		/// The adorner is not affected by transforms of designed controls.
		/// </summary>
		Designer
	}
}
