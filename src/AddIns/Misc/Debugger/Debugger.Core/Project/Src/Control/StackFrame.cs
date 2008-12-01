// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.MetaData;
using Debugger.Expressions;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;

namespace Debugger
{
	/// <summary>
	/// A stack frame which is being executed on some thread.
	/// Use to obtain arguments or local variables.
	/// </summary>
	public class StackFrame: DebuggerObject
	{	
		Process process;
		Thread thread;
		
		ICorDebugILFrame  corILFrame;
		object            corILFramePauseSession;
		ICorDebugFunction corFunction;
		
		MethodInfo methodInfo;
		uint chainIndex;
		uint frameIndex;
		
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
		
		/// <summary> Internal index of the stack chain.  The value is increasing with age. </summary>
		public uint ChainIndex {
			get { return chainIndex; }
		}
		
		/// <summary> Internal index of the stack frame.  The value is increasing with age. </summary>
		public uint FrameIndex {
			get { return frameIndex; }
		}
		
		
		/// <summary> True if the stack frame has symbols defined. 
		/// (That is has accesss to the .pdb file) </summary>
		public bool HasSymbols {
			get {
				return GetSegmentForOffet(0) != null;
			}
		}
		
		/// <summary> Returns true is this incance can not be used any more. </summary>
		public bool IsInvalid {
			get {
				try {
					object frame = this.CorILFrame;
					return false;
				} catch (DebuggerException) {
					return true;
				}
			}
		}
		
		internal StackFrame(Thread thread, ICorDebugILFrame corILFrame, uint chainIndex, uint frameIndex)
		{
			this.process = thread.Process;
			this.thread = thread;
			this.corILFrame = corILFrame;
			this.corILFramePauseSession = process.PauseSession;
			this.corFunction = corILFrame.Function;
			this.chainIndex = chainIndex;
			this.frameIndex = frameIndex;
			
			DebugType debugType = DebugType.Create(
				this.Process, 
				corFunction.Class,
				corILFrame.CastTo<ICorDebugILFrame2>().EnumerateTypeParameters().ToList().ToArray()
			);
			this.methodInfo = debugType.GetMethod(corFunction.Token);
		}
		
		/// <summary> Returns diagnostic description of the frame </summary>
		public override string ToString()
		{
			return this.MethodInfo.FullName;
		}
		
		internal ICorDebugILFrame CorILFrame {
			get {
				if (corILFramePauseSession != this.Process.PauseSession) {
					// Reobtain the stackframe
					StackFrame stackFrame = this.Thread.GetStackFrameAt(chainIndex, frameIndex);
					if (stackFrame.MethodInfo != this.MethodInfo) throw new DebuggerException("The stack frame on the thread does not represent the same method anymore");
					corILFrame = stackFrame.corILFrame;
					corILFramePauseSession = stackFrame.corILFramePauseSession;
				}
				return corILFrame;
			}
		}
		
		internal uint CorInstructionPtr {
			get {
				uint corInstructionPtr;
				CorILFrame.GetIP(out corInstructionPtr);
				return corInstructionPtr;
			}
		}
		
		SourcecodeSegment GetSegmentForOffet(uint offset)
		{
			return SourcecodeSegment.Resolve(this.MethodInfo.Module, corFunction, offset);
		}
		
		/// <summary> Step into next instruction </summary>
		public void StepInto()
		{
			AsyncStepInto();
			this.Process.WaitForPause();
		}
		
		/// <summary> Step over next instruction </summary>
		public void StepOver()
		{
			AsyncStepOver();
			this.Process.WaitForPause();
		}
		
		/// <summary> Step out of the stack frame </summary>
		public void StepOut()
		{
			AsyncStepOut();
			this.Process.WaitForPause();
		}
		
		/// <summary> Step into next instruction </summary>
		public void AsyncStepInto()
		{
			AsyncStep(true);
		}
		
		/// <summary> Step over next instruction </summary>
		public void AsyncStepOver()
		{
			AsyncStep(false);
		}
		
		/// <summary> Step out of the stack frame </summary>
		public void AsyncStepOut()
		{
			Stepper.StepOut(this, "normal");
			
			process.AsyncContinue(DebuggeeStateAction.Clear);
		}
		
		void AsyncStep(bool stepIn)
		{
			if (this.MethodInfo.Module.HasSymbols == false) {
				throw new DebuggerException("Unable to step. No symbols loaded.");
			}
			
			SourcecodeSegment nextSt = NextStatement;
			if (nextSt == null) {
				throw new DebuggerException("Unable to step. Next statement not aviable");
			}
			
			if (stepIn) {
				Stepper stepInStepper = Stepper.StepIn(this, nextSt.StepRanges, "normal");
				this.Thread.CurrentStepIn = stepInStepper;
				Stepper clearCurrentStepIn = Stepper.StepOut(this, "clear current step in");
				clearCurrentStepIn.StepComplete += delegate {
					if (this.Thread.CurrentStepIn == stepInStepper) {
						this.Thread.CurrentStepIn = null;
					}
				};
				clearCurrentStepIn.Ignore = true;
			} else {
				Stepper.StepOver(this, nextSt.StepRanges, "normal");
			}
			
			process.AsyncContinue(DebuggeeStateAction.Clear);
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
			
			SourcecodeSegment segment = SourcecodeSegment.Resolve(this.MethodInfo.Module, filename, null, line, column);
			
			if (segment != null && segment.CorFunction.Token == this.MethodInfo.MetadataToken) {
				try {
					if (simulate) {
						CorILFrame.CanSetIP((uint)segment.ILStart);
					} else {
						// Invalidates all frames and chains for the current thread
						CorILFrame.SetIP((uint)segment.ILStart);
						process.NotifyResumed(DebuggeeStateAction.Keep);
						process.NotifyPaused(PausedReason.SetIP);
						process.RaisePausedEvents();
					}
				} catch {
					return null;
				}
				return segment;
			}
			return null;
		}
		
		/// <summary> 
		/// Gets the instance of the class asociated with the current frame.
		/// That is, 'this' in C#.
		/// </summary>
		public Value GetThisValue()
		{
			return new Value(process, new ThisReferenceExpression(), GetThisCorValue());
		}
		
		ICorDebugValue GetThisCorValue()
		{
			if (this.MethodInfo.IsStatic) throw new GetValueException("Static method does not have 'this'.");
			ICorDebugValue corValue;
			try {
				corValue = CorILFrame.GetArgument(0);
			} catch (COMException e) {
				// System.Runtime.InteropServices.COMException (0x80131304): An IL variable is not available at the current native IP. (See Forum-8640)
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Not available in the current state");
				throw;
			}
			// This can be 'by ref' for value types
			if (corValue.Type == (uint)CorElementType.BYREF) {
				corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference();
			}
			return corValue;
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
		public Value GetArgumentValue(int index)
		{
			return new Value(process, new ParameterIdentifierExpression(this.MethodInfo, index), GetArgumentCorValue(index));
		}
		
		ICorDebugValue GetArgumentCorValue(int index)
		{
			ICorDebugValue corValue;
			try {
				// Non-static methods include 'this' as first argument
				corValue = CorILFrame.GetArgument((uint)(this.MethodInfo.IsStatic? index : (index + 1)));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
			// Method arguments can be passed 'by ref'
			if (corValue.Type == (uint)CorElementType.BYREF) {
				try {
					corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference();
				} catch (COMException e) {
					if ((uint)e.ErrorCode == 0x80131305) {
						// A reference value was found to be bad during dereferencing.
						// This can sometimes happen after a stack overflow
						throw new GetValueException("Bad reference");
					} else {
						throw;
					}
				}
			}
			return corValue;
		}
		
		#region Convenience methods
		
		/// <summary> Gets argument with a given name </summary>
		/// <returns> Null if not found </returns>
		public Value GetArgumentValue(string name)
		{
			for(int i = 0; i < this.ArgumentCount; i++) {
				if (this.MethodInfo.GetParameterName(i) == name) {
					return GetArgumentValue(i);
				}
			}
			return null;
		}
		
		/// <summary> Gets all arguments of the stack frame. </summary>
		public List<Value> GetArgumentValues()
		{
			List<Value> values = new List<Value>();
			for (int i = 0; i < ArgumentCount; i++) {
				values.Add(GetArgumentValue(i));
			}
			return values;
		}
		
		#endregion
		
		/// <summary> Returns value of give local variable </summary>
		public Value GetLocalVariableValue(ISymUnmanagedVariable symVar)
		{
			return new Value(this.Process, new LocalVariableIdentifierExpression(MethodInfo, symVar), GetLocalVariableCorValue(symVar));
		}
		
		ICorDebugValue GetLocalVariableCorValue(ISymUnmanagedVariable symVar)
		{
			try {
				return CorILFrame.GetLocalVariable((uint)symVar.AddressField1);
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
		}
		
		#region Convenience methods
		
		/// <summary> Get local variable with given name </summary>
		/// <returns> Null if not found </returns>
		public Value GetLocalVariableValue(string name)
		{
			foreach(ISymUnmanagedVariable symVar in this.MethodInfo.LocalVariables) {
				if (symVar.Name == name) {
					return GetLocalVariableValue(symVar);
				}
			}
			return null;
		}
		
		/// <summary> Returns all local variables of the stack frame. </summary>
		public List<Value> GetLocalVariableValues()
		{
			List<Value> values = new List<Value>();
			foreach(ISymUnmanagedVariable symVar in this.MethodInfo.LocalVariables) {
				values.Add(GetLocalVariableValue(symVar));
			}
			return values;
		}
		
		#endregion
		
		public override bool Equals(object obj)
		{
			StackFrame other = obj as StackFrame;
			return
				other != null &&
				other.Thread == this.Thread &&
				other.ChainIndex == this.ChainIndex &&
				other.FrameIndex == this.FrameIndex &&
				other.MethodInfo == this.methodInfo;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (thread != null) hashCode += 1000000009 * thread.GetHashCode(); 
				if (methodInfo != null) hashCode += 1000000093 * methodInfo.GetHashCode(); 
				hashCode += 1000000097 * chainIndex.GetHashCode();
				hashCode += 1000000103 * frameIndex.GetHashCode();
			}
			return hashCode;
		}
	}
}
