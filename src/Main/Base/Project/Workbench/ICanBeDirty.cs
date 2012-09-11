// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

// Not sure if this should be in the Workbench namespace...
// It would sort of belong there, but it's also REALLY commonly used which probably
// means it should be in the root namespace.
namespace ICSharpCode.SharpDevelop
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
