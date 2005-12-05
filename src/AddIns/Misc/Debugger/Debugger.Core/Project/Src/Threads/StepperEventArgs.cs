// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
