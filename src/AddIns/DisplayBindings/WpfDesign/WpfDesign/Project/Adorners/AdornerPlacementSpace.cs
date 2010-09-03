// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
