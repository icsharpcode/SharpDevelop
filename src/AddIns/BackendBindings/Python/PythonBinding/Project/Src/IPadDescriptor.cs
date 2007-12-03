// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// The interface that a pad descriptor should implement.
	/// Note that the actual PadDescriptor class does not 
	/// have an interface.
	/// </summary>
	public interface IPadDescriptor
	{
		/// <summary>
		/// Brings the pad to the front of the IDE.
		/// </summary>
		void BringPadToFront();
	}
}
