// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	public class RunAllTestsInPadCommand : RunTestInPadCommand
	{
		public RunAllTestsInPadCommand()
		{
		}
		
		public RunAllTestsInPadCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		public override void Run()
		{
			// To make sure all tests are run we set the Owner to null.
			Owner = null;
			base.Run();
		}
	}
}
