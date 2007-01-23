// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Interface for classes that implement the IsDirty property and the DirtyChanged event.
	/// </summary>
	public interface ICanBeDirty
	{
		/// <summary>
		/// If this property returns true the content has changed since
		/// the last load/save operation.
		/// </summary>
		bool IsDirty {
			get;
		}
		
		/// <summary>
		/// Is called when the content is changed after a save/load operation
		/// and this signals that changes could be saved.
		/// </summary>
		event EventHandler IsDirtyChanged;
	}
}
