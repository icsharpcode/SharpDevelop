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
		public enum StepperOperation {Idle, StepIn, StepOver, StepOut};
			
		Function function;
		ICorDebugStepper corStepper;
		StepperOperation operation = StepperOperation.Idle;
		bool pauseWhenComplete = true;
		
		public event EventHandler<StepperEventArgs> StepComplete;
		
		public NDebugger Debugger {
			get {
				return function.Debugger;
			}
		}
		
		public Function Function {
			get {
				return function;
			}
		}
		
		public StepperOperation Operation {
			get {
				return operation;
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
		
		public Stepper(Function function)
		{
			this.function = function;
			
			corStepper = function.CorILFrame.CreateStepper();
			
			// Turn on Just-My-Code
			if (corStepper.Is<ICorDebugStepper2>()) { // Is the debuggee .NET 2.0?
				corStepper.SetUnmappedStopMask(CorDebugUnmappedStop.STOP_NONE);
				corStepper.CastTo<ICorDebugStepper2>().SetJMC(1 /* true */);
			}
			
			function.Thread.Steppers.Add(this);
		}
		
		protected internal virtual void OnStepComplete() {
			if (StepComplete != null) {
				StepComplete(this, new StepperEventArgs(this));
			}
		}
		
		public bool IsCorStepper(ICorDebugStepper corStepper)
		{
			return this.corStepper == corStepper;
		}
		
		public void StepOut()
		{
			operation = StepperOperation.StepOut;
			corStepper.StepOut();
		}
		
		public void StepIn(int[] ranges)
		{
			operation = StepperOperation.StepIn;
			corStepper.StepRange(true /* step in */, ranges);
		}
		
		public void StepOver(int[] ranges)
		{
			operation = StepperOperation.StepOver;
			corStepper.StepRange(false /* step over */, ranges);
		}
	}
}
