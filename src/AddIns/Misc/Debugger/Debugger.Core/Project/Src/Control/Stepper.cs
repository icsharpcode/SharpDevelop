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
	public class Stepper
	{
		public enum StepperOperation {Idle, StepIn, StepOver, StepOut};
			
		StackFrame stackFrame;
		string name;
		ICorDebugStepper corStepper;
		StepperOperation operation = StepperOperation.Idle;
		bool pauseWhenComplete = true;
		
		public event EventHandler<StepperEventArgs> StepComplete;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return stackFrame.Process;
			}
		}
		
		public StackFrame StackFrame {
			get {
				return stackFrame;
			}
		}
		
		public string Name {
			get {
				return name;
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
		
		public bool JustMyCode {
			set {
				if (corStepper.Is<ICorDebugStepper2>()) { // Is the debuggee .NET 2.0?
					corStepper.SetUnmappedStopMask(CorDebugUnmappedStop.STOP_NONE);
					corStepper.CastTo<ICorDebugStepper2>().SetJMC(value ? 1 : 0);
				}
			}
		}
		
		public Stepper(StackFrame stackFrame, string name): this(stackFrame)
		{
			this.name = name;
		}
		
		public Stepper(StackFrame stackFrame)
		{
			this.stackFrame = stackFrame;
			
			corStepper = stackFrame.CorILFrame.CreateStepper();
			
			JustMyCode = true;
			
			stackFrame.Thread.Steppers.Add(this);
		}
		
		protected internal virtual void OnStepComplete() {
			if (StepComplete != null) {
				StepComplete(this, new StepperEventArgs(this));
			}
		}
		
		internal bool IsCorStepper(ICorDebugStepper corStepper)
		{
			return this.corStepper == corStepper;
		}
		
		public void StepOut()
		{
			operation = StepperOperation.StepOut;
			JustMyCode = false; // Needed for multiple events. See docs\Stepping.txt
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
		
		public override string ToString()
		{
			return string.Format("{0} in {1} pause={2} \"{3}\"", Operation, StackFrame.ToString(), PauseWhenComplete, name);
		}
	}
	
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
