// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	[Serializable]
	class StepperEventArgs: DebuggerEventArgs
	{
		Stepper stepper;
		
		public Stepper Stepper {
			get {
				return stepper;
			}
		}
		
		public StepperEventArgs(Stepper stepper): base(stepper.Debugger)
		{
			this.stepper = stepper;
		}
	}
}
