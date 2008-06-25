// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.MetaData;
using Debugger.Expressions;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;

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
		int depth;
		
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
		
		/// <summary>
		/// The depth of this frame.  Most recent stack frame has depth 0.
		/// The frame that called it has depth 1 and so on.
		/// </summary>
		public int Depth {
			get { return depth; }
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
				return this.corILFramePauseSession != process.PauseSession;
			}
		}
		
		internal StackFrame(Thread thread, ICorDebugILFrame corILFrame, int depth)
		{
			this.process = thread.Process;
			this.thread = thread;
			this.corILFrame = corILFrame;
			this.corILFramePauseSession = process.PauseSession;
			this.corFunction = corILFrame.Function;
			this.depth = depth;
			
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
				if (HasExpired) throw new DebuggerException("StackFrame has expired");
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
			new Stepper(this, "StackFrame step out").StepOut();
			process.AsyncContinue(DebuggeeStateAction.Clear);
		}
		
		void AsyncStep(bool stepIn)
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
							process.NotifyResumed(DebuggeeStateAction.Keep);
							process.NotifyPaused(PausedReason.SetIP);
							process.RaisePausedEvents();
						}
					} catch {
						return null;
					}
					return GetSegmentForOffet((uint)ilOffset);
				}
			}
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
			if (this.HasExpired) throw new GetValueException("StackFrame has expired");
			try {
				return CorILFrame.GetArgument(0);
			} catch (COMException e) {
				// System.Runtime.InteropServices.COMException (0x80131304): An IL variable is not available at the current native IP. (See Forum-8640)
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Not available in the current state");
				throw;
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
		public Value GetArgumentValue(int index)
		{
			return new Value(process, new ParameterIdentifierExpression(this.MethodInfo, index), GetArgumentCorValue(index));
		}
		
		ICorDebugValue GetArgumentCorValue(int index)
		{
			if (this.HasExpired) throw new GetValueException("StackFrame has expired");
			
			try {
				// Non-static methods include 'this' as first argument
				return CorILFrame.GetArgument((uint)(this.MethodInfo.IsStatic? index : (index + 1)));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
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
		public Value[] GetArgumentValues()
		{
			List<Value> values = new List<Value>();
			for (int i = 0; i < ArgumentCount; i++) {
				values.Add(GetArgumentValue(i));
			}
			return values.ToArray();
		}
		
		#endregion
		
		public Value GetLocalVariableValue(ISymUnmanagedVariable symVar)
		{
			return new Value(this.Process, new LocalVariableIdentifierExpression(MethodInfo, symVar), GetLocalVariableCorValue(symVar));
		}
		
		ICorDebugValue GetLocalVariableCorValue(ISymUnmanagedVariable symVar)
		{
			if (this.HasExpired) throw new GetValueException("StackFrame has expired");
			
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
		
		/// <summary> Gets all local variables of the stack frame. </summary>
		public Value[] GetLocalVariableValues()
		{
			List<Value> values = new List<Value>();
			foreach(ISymUnmanagedVariable symVar in this.MethodInfo.LocalVariables) {
				values.Add(GetLocalVariableValue(symVar));
			}
			return values.ToArray();
		}
		
		#endregion
	}
}
