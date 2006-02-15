// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Build.Tasks;
using System;

namespace ICSharpCode.Build.Tasks.Tests
{
	/// <summary>
	/// Helper class that allows us to test protected methods of the
	/// Mbas class.
	/// </summary>
	public class MockMbas : Mbas
	{
		/// <summary>
		/// Generates the Mbas command line arguments via the protected
		/// GenerateCommandLineArguments method.
		/// </summary>
		public string GetCommandLine()
		{
			return base.GenerateCommandLineArguments();
		}
	}
}
