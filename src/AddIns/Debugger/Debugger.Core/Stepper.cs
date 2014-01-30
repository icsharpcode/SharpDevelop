// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Debugger.Interop.CorDebug;

namespace Debugger
{
	enum StepperOperation {StepIn, StepOver, StepOut};
	
	/// <remarks>
	/// - During evaluation some chains may be temporarly removed
	/// - When two events are invoked and JMC is active, step out skips the second function
	/// - Step out and step over works properly for exceptions
	/// - Evaluation kills stepper overs on active frame
	/// - StepRange callbacks go first (probably in order), StepOut callback are called after that
	/// - StepRange is much slower then StepOut
	/// </remarks>
	class Stepper
	{
		StackFrame stackFrame;
		StepperOperation operation;
		int[] stepRanges;
		string name;
		
		ICorDebugStepper corStepper;
		
		bool ignore;
		
		public event EventHandler<StepperEventArgs> StepComplete;
		
		public ICorDebugStepper CorStepper {
			get { return corStepper; }
		}
		
		public Process Process {
			get { return stackFrame.Process; }
		}
		
		public StackFrame StackFrame {
			get { return stackFrame; }
		}
		
		public StepperOperation Operation {
			get { return operation; }
		}
		
		public int[] StepRanges {
			get { return stepRanges; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public bool Ignore {
			get { return ignore; }
			set { ignore = value; }
		}
		
		private Stepper(StackFrame stackFrame, StepperOperation operation, int[] stepRanges, string name, bool justMyCode)
		{
			this.stackFrame = stackFrame;
			this.operation = operation;
			this.stepRanges = stepRanges;
			this.name = name;
			
			this.corStepper = stackFrame.CorILFrame.CreateStepper();
			this.ignore = false;
			this.StackFrame.Process.Steppers.Add(this);
			
			if (justMyCode) {
				corStepper.SetUnmappedStopMask(CorDebugUnmappedStop.STOP_NONE);
				((ICorDebugStepper2)corStepper).SetJMC(1);
			}
		}
		
		protected internal virtual void OnStepComplete(CorDebugStepReason reason) {
			this.corStepper = null;
			if (StepComplete != null) {
				StepComplete(this, new StepperEventArgs(this, reason));
			}
		}
		
		internal bool IsCorStepper(ICorDebugStepper corStepper)
		{
			return this.corStepper == corStepper;
		}
		
		internal bool IsInStepRanges(int offset)
		{
			for(int i = 0; i < stepRanges.Length / 2; i++) {
				if (stepRanges[2*i] <= offset && offset < stepRanges[2*i + 1]) {
					return true;
				}
			}
			return false;
		}
		
		public static Stepper StepOut(StackFrame stackFrame, string name)
		{
			// JMC off - Needed for multiple events. See docs\Stepping.txt
			Stepper stepper = new Stepper(stackFrame, StepperOperation.StepOut, null, name, false);
			stepper.corStepper.StepOut();
			return stepper;
		}
		
		public static Stepper StepIn(StackFrame stackFrame, int[] stepRanges, string name)
		{
			Stepper stepper = new Stepper(stackFrame, StepperOperation.StepIn, stepRanges, name, stackFrame.Process.Options.EnableJustMyCode);
			stepper.corStepper.StepRange(true /* step in */, stepRanges);
			return stepper;
		}
		
		public static Stepper StepOver(StackFrame stackFrame, int[] stepRanges, string name)
		{
			Stepper stepper = new Stepper(stackFrame, StepperOperation.StepOver, stepRanges, name, stackFrame.Process.Options.EnableJustMyCode);
			stepper.corStepper.StepRange(false /* step over */, stepRanges);
			return stepper;
		}
		
		public override string ToString()
		{
			return string.Format("{0} from {1} name=\"{2}\"", this.Operation, this.StackFrame.ToString(), this.Name);
		}
	}
	
	[Serializable]
	class StepperEventArgs: EventArgs
	{
		Stepper stepper;
		CorDebugStepReason reason;
		
		public Stepper Stepper {
			get { return stepper; }
		}
		
		public CorDebugStepReason Reason {
			get { return reason; }
		}
		
		public StepperEventArgs(Stepper stepper, CorDebugStepReason reason)
		{
			this.stepper = stepper;
			this.reason = reason;
		}
	}
}
