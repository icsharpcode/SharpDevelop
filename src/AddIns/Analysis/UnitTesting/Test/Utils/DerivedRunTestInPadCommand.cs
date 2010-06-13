// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class DerivedRunTestInPadCommand : RunTestInPadCommand
	{
		public DerivedRunTestInPadCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		public Action GetCallRunTestCompletedAction()
		{
			Action action = base.TestRunCompleted;
			return action;
		}
	}
}
