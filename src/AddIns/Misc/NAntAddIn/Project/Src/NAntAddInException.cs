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
	/// The exception that is thrown when a non-fatal 
	/// error occurs in the NAnt add-in.
	/// </summary>
	public class NAntAddInException : ApplicationException
	{
		public NAntAddInException()
		{
		}
		
		public NAntAddInException(string message)
			: base(message)
		{
		}
	}
}
