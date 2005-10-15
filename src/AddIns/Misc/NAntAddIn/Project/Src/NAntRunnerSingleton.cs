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
	/// Single NAntRunner that is used by all commands.
	/// </summary>
	/// <remarks>
	/// The NAnt add-in only allows one build to be run at a time.
	/// </remarks>
	public class NAntRunnerSingleton
	{
		static NAntRunner runner;
		
		NAntRunnerSingleton()
		{
		}
		
		/// <summary>
		/// Gets the <see cref="NAntRunner"/> instance.
		/// </summary>
		public static NAntRunner Runner {
			get {
				if (runner == null) {
					runner = new NAntRunner();
				}
				
				return runner;
			}
		}
	}
}
