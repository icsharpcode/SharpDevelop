// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3496 $</version>
// </file>

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
		public bool GrayOutDesignSurfaceExceptParentContainerWhenDragging = false;
	}
}
