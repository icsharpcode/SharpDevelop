// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	class Stepper
	{
		NDebugger debugger;
		ICorDebugStepper corStepper;
		bool pauseWhenComplete = true;
		
		public event EventHandler<StepperEventArgs> StepComplete;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public ICorDebugStepper CorStepper {
			get {
				return corStepper;
			}
		}
		
		public bool PauseWhenComplete {
			get {
				return pauseWhenComplete;
			}
			set {
				pauseWhenComplete = value;
			}
		}
		
		public Stepper(NDebugger debugger, ICorDebugStepper corStepper)
		{
			this.debugger = debugger;
			this.corStepper = corStepper;
		}
		
		protected internal virtual void OnStepComplete() {
			if (StepComplete != null) {
				StepComplete(this, new StepperEventArgs(this));
			}
		}
	}
}
