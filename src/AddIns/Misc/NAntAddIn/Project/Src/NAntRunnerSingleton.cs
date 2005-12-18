// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
