// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Allows a IViewContent to handle the save command on its own instead of using OpenedFile.
	/// </summary>
	public interface ICustomizedCommands
	{
		/// <summary>
		/// Returns true, if the save command is handled, false otherwise
		/// </summary>
		bool SaveCommand();
		
		/// <summary>
		/// Returns true, if the save as command is handled, false otherwise
		/// </summary>
		bool SaveAsCommand();
	}
}
