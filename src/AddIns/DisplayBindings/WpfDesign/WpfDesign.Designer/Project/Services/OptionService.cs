// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// Contains a set of options regarding the default designer components.
	/// </summary>
	public sealed class OptionService
	{
		/// <summary>
		/// Gets/Sets whether the design surface should be grayed out while dragging/selection.
		/// </summary>
		public bool GrayOutDesignSurfaceExceptParentContainerWhenDragging = true;
	}
}
