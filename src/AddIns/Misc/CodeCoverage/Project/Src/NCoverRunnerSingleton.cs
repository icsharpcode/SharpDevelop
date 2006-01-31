// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Single NCover runner that is used by all commands.
	/// </summary>
	/// <remarks>
	public class NCoverRunnerSingleton
	{
		static NCoverRunner runner;
		
		NCoverRunnerSingleton()
		{
		}
		
		/// <summary>
		/// Gets the <see cref="NCoverRunner"/> instance.
		/// </summary>
		public static NCoverRunner Runner {
			get {
				if (runner == null) {
					runner = new NCoverRunner();
				}
				return runner;
			}
		}
	}
}
