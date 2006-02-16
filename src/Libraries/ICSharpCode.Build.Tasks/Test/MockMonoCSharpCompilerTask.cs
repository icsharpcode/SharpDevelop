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
	/// MonoCSharpCompilerTask class.
	/// </summary>
	public class MockMonoCSharpCompilerTask : MonoCSharpCompilerTask
	{
		/// <summary>
		/// Generates the MonoCSharpCompilerTask command line arguments via the 
		/// protected GenerateCommandLineArguments method.
		/// </summary>
		public string GetCommandLine()
		{
			return base.GenerateResponseFileCommands();
		}
		
		protected override string ToolName {
			get {
				return "MonoCSharp.exe";
			}
		}
	}
}
