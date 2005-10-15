// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// An exception thrown by a <see cref="ProcessRunner"/>
	/// instance.
	/// </summary>
	public class ProcessRunnerException : ApplicationException
	{
		public ProcessRunnerException(string message)
			: base(message)
		{
		}
	}
}
