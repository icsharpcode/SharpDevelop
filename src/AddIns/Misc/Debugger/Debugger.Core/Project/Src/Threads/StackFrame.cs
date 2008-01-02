// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	/// <summary>
	/// A stack frame which is being executed on some thread.
	/// Use to obtain arguments or local variables.
	/// </summary>
	public class StackFrame: DebuggerObject, IExpirable
	{	
		Process process;
		
		MethodInfo methodInfo;
		
		ICorDebugFunction corFunction;
		ICorDebugILFrame  corILFrame;
		object            corILFramePauseSession;
		
		Stepper stepOutStepper;
		
		bool steppedOut = false;
		Thread thread;
		FrameID frameID;
		
		/// <summary> The process in which this stack frame is executed </summary>
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		/// <summary> Get the method which this stack frame is executing </summary>
		public MethodInfo MethodInfo {
			get { return methodInfo; }
		}
		
		/// <summary> A thread in which the stack frame is executed </summary>
		[Debugger.Tests.Ignore]
		public Thread Thread {
			get {
				return thread;
			}
		}
		
		/// <summary> True if the stack frame has symbols defined. 
		/// (That is has accesss to the .pdb file) </summary>
		public bool HasSymbols {
			get {
				return GetSegmentForOffet(0) != null;
			}
		}
		
		/// <summary> True if stack frame stepped out and is not longer valid. </summary>
		public bool HasExpired {
			get {
				return steppedOut || this.MethodInfo.Module.Unloaded;
			}
		}
		
		/// <summary> Occurs when stack frame expires and is no longer usable </summary>
		public event EventHandler Expired;
		
		/// <summary> Is called when stack frame expires and is no longer usable </summary>
		internal protected virtual void OnExpired(EventArgs e)
		{
			if (!steppedOut) {
				steppedOut = true;
				process.TraceMessage("StackFrame " + this.ToString() + " expired");
				if (Expired != null) {
					Expired(this, e);
				}
			}
		}
		
		internal StackFrame(Thread thread, FrameID frameID, ICorDebugILFrame corILFrame)
		{
			this.process = thread.Process;
			this.thread = thread;
			this.frameID = frameID;
			this.CorILFrame = corILFrame;
			corFunction = corILFrame.Function;
			
			DebugType debugType = DebugType.Create(
				this.Process, 
				corFunction.Class,
				corILFrame.CastTo<ICorDebugILFrame2>().EnumerateTypeParameters().ToList()
			);
			MethodProps methodProps = process.GetModule(corFunction.Module).MetaData.GetMethodProps(corFunction.Token);
			this.methodInfo = new MethodInfo(debugType, methodProps);
			                                 
			// Force some callback when stack frame steps out so that we can expire it
			stepOutStepper = new Stepper(this, "StackFrame Tracker");
			stepOutStepper.StepOut();
			stepOutStepper.PauseWhenComplete = false;
			
			process.TraceMessage("StackFrame " + this.ToString() + " created");
		}
		
		/// <summary> Returns diagnostic description of the frame </summary>
		public override string ToString()
		{
			return this.MethodInfo.Name + "(" + frameID.ToString() + ")";
		}
		
		internal ICorDebugILFrame CorILFrame {
			get {
				if (HasExpired) throw new DebuggerException("StackFrame has expired");
				if (corILFramePauseSession != process.PauseSession) {
					CorILFrame = thread.GetFrameAt(frameID).As<ICorDebugILFrame>();
				}
				return corILFrame;
			}
			set {
				if (value == null) throw new DebuggerException("Can not set frame to null");
				corILFrame = value;
				corILFramePauseSession = process.PauseSession;
			}
		}
		
		internal uint CorInstructionPtr {
			get {
				uint corInstructionPtr;
				CorILFrame.GetIP(out corInstructionPtr);
				return corInstructionPtr;
			}
		}
		
		/// <summary> Step into next instruction </summary>
		public void StepInto()
		{
			Step(true);
		}
		
		/// <summary> Step over next instruction </summary>
		public void StepOver()
		{
			Step(false);
		}
		
		/// <summary> Step out of the stack frame </summary>
		public void StepOut()
		{
			new Stepper(this, "StackFrame step out").StepOut();
			process.Continue();
		}

		private unsafe void Step(bool stepIn)
		{
			if (this.MethodInfo.Module.SymbolsLoaded == false) {
				throw new DebuggerException("Unable to step. No symbols loaded.");
			}

			SourcecodeSegment nextSt;
				
			nextSt = NextStatement;
			if (nextSt == null) {
				throw new DebuggerException("Unable to step. Next statement not aviable");
			}
			
			if (stepIn) {
				new Stepper(this, "StackFrame step in").StepIn(nextSt.StepRanges);
				// Without JMC step in which ends in code without symblols is cotinued.
				// The next step over ensures that we at least do step over.
				new Stepper(this, "Safety step over").StepOver(nextSt.StepRanges);
			} else {
				new Stepper(this, "StackFrame step over").StepOver(nextSt.StepRanges);
			}
			
			process.Continue();
		}
		
		/// <summary>
		/// Get the information about the next statement to be executed.
		/// 
		/// Returns null on error.
		/// </summary>
		public SourcecodeSegment NextStatement {
			get {
				return GetSegmentForOffet(CorInstructionPtr);
			}
		}

		/// <summary>
		/// Returns null on error.
		/// 
		/// 'ILStart &lt;= ILOffset &lt;= ILEnd' and this range includes at least
		/// the returned area of source code. (May incude some extra compiler generated IL too)
		/// </summary>
		SourcecodeSegment GetSegmentForOffet(uint offset)
		{
			ISymUnmanagedMethod symMethod = this.MethodInfo.SymMethod;
			
			if (symMethod == null) {
				return null;
			}
			
			uint sequencePointCount = symMethod.SequencePointCount;
			SequencePoint[] sequencePoints = symMethod.SequencePoints;
			
			SourcecodeSegment retVal = new SourcecodeSegment();
			
			// Get i for which: offsets[i] <= offset < offsets[i + 1]
			// or fallback to first element if  offset < offsets[0]
			for (int i = (int)sequencePointCount - 1; i >= 0; i--) // backwards
				if (sequencePoints[i].Offset <= offset || i == 0) {
					// Set inforamtion about current IL range
					int codeSize = (int)corFunction.ILCode.Size;
					
					retVal.ILOffset = (int)offset;
					retVal.ILStart = (int)sequencePoints[i].Offset;
					retVal.ILEnd = (i + 1 < sequencePointCount) ? (int)sequencePoints[i+1].Offset : codeSize;
					
					// 0xFeeFee means "code generated by compiler"
					// If we are in generated sequence use to closest real one instead,
					// extend the ILStart and ILEnd to include the 'real' sequence
					
					// Look ahead for 'real' sequence
					while (i + 1 < sequencePointCount && sequencePoints[i].Line == 0xFeeFee) {
						i++;
						retVal.ILEnd = (i + 1 < sequencePointCount) ? (int)sequencePoints[i+1].Offset : codeSize;
					}
					// Look back for 'real' sequence
					while (i - 1 >= 0 && sequencePoints[i].Line == 0xFeeFee) {
						i--;
						retVal.ILStart = (int)sequencePoints[i].Offset;
					}
					// Wow, there are no 'real' sequences
					if (sequencePoints[i].Line == 0xFeeFee) {
						return null;
					}
					
					retVal.ModuleFilename = this.MethodInfo.Module.FullPath;
					
					retVal.SourceFullFilename = sequencePoints[i].Document.URL;
					
					retVal.StartLine   = (int)sequencePoints[i].Line;
					retVal.StartColumn = (int)sequencePoints[i].Column;
					retVal.EndLine     = (int)sequencePoints[i].EndLine;
					retVal.EndColumn   = (int)sequencePoints[i].EndColumn;
					
					
					List<int> stepRanges = new List<int>();
					for (int j = 0; j < sequencePointCount; j++) {
						// Step over compiler generated sequences and current statement
						// 0xFeeFee means "code generated by compiler"
						if (sequencePoints[j].Line == 0xFeeFee || j == i) {
							// Add start offset or remove last end (to connect two ranges into one)
							if (stepRanges.Count > 0 && stepRanges[stepRanges.Count - 1] == sequencePoints[j].Offset) {
								stepRanges.RemoveAt(stepRanges.Count - 1);
							} else {
								stepRanges.Add((int)sequencePoints[j].Offset);
							}
							// Add end offset | handle last sequence point
							if (j + 1 < sequencePointCount) {
								stepRanges.Add((int)sequencePoints[j+1].Offset);
							} else {
								stepRanges.Add(codeSize);
							}
						}
					}
					
					retVal.StepRanges = stepRanges.ToArray();
					
					return retVal;
				}
			return null;
		}
		
		/// <summary>
		/// Determine whether the instrustion pointer can be set to given location
		/// </summary>
		/// <returns> Best possible location. Null is not possible. </returns>
		public SourcecodeSegment CanSetIP(string filename, int line, int column)
		{
			return SetIP(true, filename, line, column);
		}
		
		/// <summary>
		/// Set the instrustion pointer to given location
		/// </summary>
		/// <returns> Best possible location. Null is not possible. </returns>
		public SourcecodeSegment SetIP(string filename, int line, int column)
		{
			return SetIP(false, filename, line, column);
		}
		
		SourcecodeSegment SetIP(bool simulate, string filename, int line, int column)
		{
			process.AssertPaused();
			
			SourcecodeSegment suggestion = new SourcecodeSegment(filename, line, column, column);
			ICorDebugFunction corFunction;
			int ilOffset;
			if (!suggestion.GetFunctionAndOffset(this.MethodInfo.Module, false, out corFunction, out ilOffset)) {
				return null;
			} else {
				if (corFunction.Token != this.MethodInfo.MetadataToken) {
					return null;
				} else {
					try {
						if (simulate) {
							CorILFrame.CanSetIP((uint)ilOffset);
						} else {
							// invalidates all frames and chains for the current thread
							CorILFrame.SetIP((uint)ilOffset);
							process.NotifyPaused(new PauseSession(PausedReason.SetIP));
							process.Pause(false);
						}
					} catch {
						return null;
					}
					return GetSegmentForOffet((uint)ilOffset);
				}
			}
		}
		
		/// <summary> Gets value of given name which is accessible from this stack frame </summary>
		/// <returns> Null if not found </returns>
		public Value GetValue(string name)
		{
			if (name == "this") {
				return ThisValue;
			}
			if (Arguments.Contains(name)) {
				return Arguments[name];
			}
			if (LocalVariables.Contains(name)) {
				return LocalVariables[name];
			}
			if (ContaingClassVariables.Contains(name)) {
				return ContaingClassVariables[name];
			}
			return null;
		}
		
		/// <summary>
		/// Gets all variables in the lexical scope of the stack frame. 
		/// That is, arguments, local variables and varables of the containing class.
		/// </summary>
		[Debugger.Tests.Ignore] // Accessible though others
		public ValueCollection Variables {
			get {
				return new ValueCollection(GetVariables());
			}
		}
		
		IEnumerable<Value> GetVariables() 
		{
			if (!this.MethodInfo.IsStatic) {
				yield return ThisValue;
			}
			foreach(Value val in Arguments) {
				yield return val;
			}
			foreach(Value val in LocalVariables) {
				yield return val;
			}
			foreach(Value val in ContaingClassVariables) {
				yield return val;
			}
		}
		
		Value thisValueCache;
		
		/// <summary> 
		/// Gets the instance of the class asociated with the current frame.
		/// That is, 'this' in C#.
		/// </summary>
		public Value ThisValue {
			get {
				if (this.MethodInfo.IsStatic) throw new DebuggerException("Static method does not have 'this'.");
				if (thisValueCache == null) {
					thisValueCache = new Value(process, "this", ThisCorValue);
				}
				return thisValueCache;
			}
		}
		
		ICorDebugValue ThisCorValue {
			get {
				if (this.HasExpired) throw new CannotGetValueException("StackFrame has expired");
				try {
					return CorILFrame.GetArgument(0);
				} catch (COMException e) {
					// System.Runtime.InteropServices.COMException (0x80131304): An IL variable is not available at the current native IP. (See Forum-8640)
					if ((uint)e.ErrorCode == 0x80131304) throw new CannotGetValueException("Not available in the current state");
					throw;
				}
			}
		}
		
		/// <summary>
		/// Gets all accessible members of the class that defines this stack frame.
		/// </summary>
		public ValueCollection ContaingClassVariables {
			get {
				// TODO: Should work for static
				if (!this.MethodInfo.IsStatic) {
					return ThisValue.GetMembers();
				} else {
					return ValueCollection.Empty;
				}
			}
		}
		
		/// <summary> Total number of arguments (excluding implicit 'this' argument) </summary>
		public int ArgumentCount {
			get {
				ICorDebugValueEnum argumentEnum = CorILFrame.EnumerateArguments();
				uint argCount = argumentEnum.Count;
				if (!this.MethodInfo.IsStatic) {
					argCount--; // Remove 'this' from count
				}
				return (int)argCount;
			}
		}
		
		/// <summary> Gets argument with a given index </summary>
		/// <param name="index"> Zero-based index </param>
		public Value GetArgument(int index)
		{
			return new Value(process, this.MethodInfo.GetParameterName(index), GetArgumentCorValue(index));
		}
		
		ICorDebugValue GetArgumentCorValue(int index)
		{
			if (this.HasExpired) throw new CannotGetValueException("StackFrame has expired");
			
			try {
				// Non-static methods include 'this' as first argument
				return CorILFrame.GetArgument((uint)(this.MethodInfo.IsStatic? index : (index + 1)));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new CannotGetValueException("Unavailable in optimized code");
				throw;
			}
		}
		
		ValueCollection argumentsCache;
		
		/// <summary> Gets all arguments of the stack frame. </summary>
		public ValueCollection Arguments {
			get {
				if (argumentsCache == null) {
					DateTime startTime = Util.HighPrecisionTimer.Now;
					
					argumentsCache = new ValueCollection(ArgumentsEnum);
					
					TimeSpan totalTime = Util.HighPrecisionTimer.Now - startTime;
					process.TraceMessage("Loaded Arguments for " + this.ToString() + " (" + totalTime.TotalMilliseconds + " ms)");
				}
				return argumentsCache;
			}
		}
		
		IEnumerable<Value> ArgumentsEnum {
			get {
				for (int i = 0; i < ArgumentCount; i++) {
					yield return GetArgument(i);
				}
			}
		}
		
		ValueCollection localVariablesCache;
		
		/// <summary> Gets all local variables of the stack frame. </summary>
		public ValueCollection LocalVariables {
			get {
				if (localVariablesCache == null) {
					DateTime startTime = Util.HighPrecisionTimer.Now;
					
					localVariablesCache = new ValueCollection(LocalVariablesEnum);
					
					TimeSpan totalTime = Util.HighPrecisionTimer.Now - startTime;
					process.TraceMessage("Loaded LocalVariables for " + this.ToString() + " (" + totalTime.TotalMilliseconds + " ms)");
				}
				return localVariablesCache;
			}
		}
		
		IEnumerable<Value> LocalVariablesEnum {
			get {
				foreach(ISymUnmanagedVariable symVar in this.MethodInfo.LocalVariables) {
					yield return new Value(process, symVar.Name, GetCorValueOfLocalVariable(symVar));
				}
			}
		}
		
		ICorDebugValue GetCorValueOfLocalVariable(ISymUnmanagedVariable symVar)
		{
			if (this.HasExpired) throw new CannotGetValueException("StackFrame has expired");
			
			try {
				return CorILFrame.GetLocalVariable((uint)symVar.AddressField1);
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new CannotGetValueException("Unavailable in optimized code");
				throw;
			}
		}
	}
}
