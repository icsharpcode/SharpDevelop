// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	public class RunAllTestsWithCodeCoverageCommand : RunTestWithCodeCoverageCommand
	{
		public RunAllTestsWithCodeCoverageCommand()
		{
		}
		
		/// <summary>
		/// Set Owner to null so all tests are run.
		/// </summary>
		public override void Run()
		{
			Owner = null;
			base.Run();
		}
	}
}
