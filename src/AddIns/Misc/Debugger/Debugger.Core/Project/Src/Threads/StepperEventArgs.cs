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
	public class StepperEventArgs: ProcessEventArgs
	{
		Stepper stepper;
		
		public Stepper Stepper {
			get {
				return stepper;
			}
		}
		
		public StepperEventArgs(Stepper stepper): base(stepper.Process)
		{
			this.stepper = stepper;
		}
	}
}
