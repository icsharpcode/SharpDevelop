// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// </summary>
	public interface ICustomizedCommands
	{
		/// <summary>
		/// Returns true, if the save as command is handled, false otherwise
		/// </summary>
		bool SaveAsCommand();
	}
}
