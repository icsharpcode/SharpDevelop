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
	public class Function: RemotingObjectBase, IExpirable
	{	
		Process process;
		
		Module module;
		ICorDebugFunction corFunction;
		ICorDebugILFrame  corILFrame;
		object            corILFramePauseSession;
		
		Stepper stepOutStepper;
		
		bool steppedOut = false;
		Thread thread;
		FrameID frameID;
		
		MethodProps methodProps;
		
		public Process Process {
			get {
				return process;
			}
		}

		public string Name { 
			get { 
				return methodProps.Name;
			} 
		}
		
		public uint Token {
			get {
				return methodProps.Token;
			}
		}
		
		public Module Module { 
			get { 
				return module; 
			} 
		}
		
		public Thread Thread {
			get {
				return thread;
			}
		}

		public bool IsStatic {
			get {
				return methodProps.IsStatic;
			}
		}
		
		public bool HasSymbols {
			get {
				return GetSegmentForOffet(0) != null;
			}
		}

		internal ICorDebugClass ContaingClass {
			get {
				return corFunction.Class;
			}
		}
		
		/// <summary>
		/// True if function stepped out and is not longer valid.
		/// </summary>
		public bool HasExpired {
			get {
				return steppedOut || Module.Unloaded;
			}
		}
		
		/// <summary>
		/// Occurs when function expires and is no longer usable
		/// </summary>
		public event EventHandler Expired;
		
		internal protected virtual void OnExpired(EventArgs e)
		{
			if (!steppedOut) {
				steppedOut = true;
				process.TraceMessage("Function " + this.ToString() + " expired");
				if (Expired != null) {
					Expired(this, e);
				}
			}
		}
		
		internal Function(Thread thread, FrameID frameID, ICorDebugILFrame corILFrame)
		{
			this.process = thread.Process;
			this.thread = thread;
			this.frameID = frameID;
			this.CorILFrame = corILFrame;
			corFunction = corILFrame.Function;
			module = process.GetModule(corFunction.Module);
			
			methodProps = module.MetaData.GetMethodProps(corFunction.Token);
			
			// Force some callback when function steps out so that we can expire it
			stepOutStepper = new Stepper(this, "Function Tracker");
			stepOutStepper.StepOut();
			stepOutStepper.PauseWhenComplete = false;
			
			process.TraceMessage("Function " + this.ToString() + " created");
		}
		
		public override string ToString()
		{
			return methodProps.Name + "(" + frameID.ToString() + ")";
		}
		
		internal ICorDebugILFrame CorILFrame {
			get {
				if (HasExpired) throw new DebuggerException("Function has expired");
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
		
		internal uint corInstructionPtr {
			get	{
				uint corInstructionPtr;
				CorILFrame.GetIP(out corInstructionPtr);
				return corInstructionPtr;
			}
		}
		
		internal ISymUnmanagedReader symReader {
			get	{
				if (module.SymbolsLoaded == false) return null;
				if (module.SymReader == null) return null;
				return module.SymReader;
			}
		}
		
		internal ISymUnmanagedMethod symMethod {
			get	{
				if (symReader == null) {
					return null;
				} else {
					try {
						return symReader.GetMethod(methodProps.Token);
					} catch {
						return null;
					}
				}
			}
		}
		
		public void StepInto()
		{
			Step(true);
		}		

		public void StepOver()
		{
			Step(false);
		}

		public void StepOut()
		{
			new Stepper(this, "Function step out").StepOut();
			process.Continue();
		}

		private unsafe void Step(bool stepIn)
		{
			if (Module.SymbolsLoaded == false) {
				throw new DebuggerException("Unable to step. No symbols loaded.");
			}

			SourcecodeSegment nextSt;
				
			nextSt = NextStatement;
			if (nextSt == null) {
				throw new DebuggerException("Unable to step. Next statement not aviable");
			}
			
			if (stepIn) {
				new Stepper(this, "Function step in").StepIn(nextSt.StepRanges);
				// Without JMC step in which ends in code without symblols is cotinued.
				// The next step over ensures that we at least do step over.
				new Stepper(this, "Safety step over").StepOver(nextSt.StepRanges);
			} else {
				new Stepper(this, "Function step over").StepOver(nextSt.StepRanges);
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
				return GetSegmentForOffet(corInstructionPtr);
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
			ISymUnmanagedMethod symMethod;
			
			symMethod = this.symMethod;
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
					
					retVal.ModuleFilename = module.FullPath;
					
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
		
		public SourcecodeSegment CanSetIP(string filename, int line, int column)
		{
			return SetIP(true, filename, line, column);
		}
			
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
			if (!suggestion.GetFunctionAndOffset(this.Module, false, out corFunction, out ilOffset)) {
				return null;
			} else {
				if (corFunction.Token != methodProps.Token) {
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
		
		public VariableCollection Variables {
			get {
				return new VariableCollection(GetVariables());
			}
		}
		
		IEnumerable<Variable> GetVariables() 
		{
			if (!IsStatic) {
				yield return new Variable("this", ThisValue);
			}
			foreach(Variable var in ArgumentVariables) {
				yield return var;
			}
			foreach(Variable var in LocalVariables) {
				yield return var;
			}
			foreach(Variable var in ContaingClassVariables) {
				yield return var;
			}
		}
		
		public Value ThisValue {
			get {
				return new Value(
					process,
					new IExpirable[] {this},
					new IMutable[] {},
					delegate { return ThisCorValue; }
				);
			}
		}
		
		ICorDebugValue ThisCorValue {
			get {
				if (IsStatic) throw new DebuggerException("Static method does not have 'this'.");
				if (this.HasExpired) throw new CannotGetValueException("Function has expired");
				try {
					return CorILFrame.GetArgument(0);
				} catch (COMException e) {
					// System.Runtime.InteropServices.COMException (0x80131304): An IL variable is not available at the current native IP. (See Forum-8640)
					if ((uint)e.ErrorCode == 0x80131304) throw new CannotGetValueException("Not available in the current state");
					throw;
				}
			}
		}
		
		public IEnumerable<Variable> ContaingClassVariables {
			get {
				// TODO: Should work for static
				if (!IsStatic) {
					foreach(Variable var in ThisValue.ValueProxy.SubVariables) {
						yield return var;
					}
				}
			}
		}
		
		public string GetParameterName(int index)
		{
			// index = 0 is return parameter
			try {
				return module.MetaData.GetParamForMethodIndex(methodProps.Token, (uint)index + 1).Name;
			} catch {
				return String.Empty;
			}
		}
		
		public int ArgumentCount {
			get {
				ICorDebugValueEnum argumentEnum = CorILFrame.EnumerateArguments();
				uint argCount = argumentEnum.Count;
				if (!IsStatic) {
					argCount--; // Remove 'this' from count
				}
				return (int)argCount;
			}
		}
		
		public MethodArgument GetArgumentVariable(int index)
		{
			return new MethodArgument(
				GetParameterName(index),
				index,
				new Value(
					process,
					new IExpirable[] {this},
					new IMutable[] {process.DebugeeState},
					delegate { return GetArgumentCorValue(index); }
				)
			);
		}
		
		ICorDebugValue GetArgumentCorValue(int index)
		{
			if (this.HasExpired) throw new CannotGetValueException("Function has expired");
			
			try {
				// Non-static functions include 'this' as first argument
				return CorILFrame.GetArgument((uint)(IsStatic? index : (index + 1)));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new CannotGetValueException("Unavailable in optimized code");
				throw;
			}
		}
		
		public IEnumerable<MethodArgument> ArgumentVariables {
			get {
				for (int i = 0; i < ArgumentCount; i++) {
					yield return GetArgumentVariable(i);
				}
			}
		}
		
		public IEnumerable<LocalVariable> LocalVariables {
			get {
				if (symMethod != null) { // TODO: Is this needed?
					ISymUnmanagedScope symRootScope = symMethod.RootScope;
					foreach(LocalVariable var in GetLocalVariablesInScope(symRootScope)) {
						if (!var.Name.StartsWith("CS$")) { // TODO: Generalize
							yield return var;
						}
					}
				}
			}
		}
		
		IEnumerable<LocalVariable> GetLocalVariablesInScope(ISymUnmanagedScope symScope)
		{
			foreach (ISymUnmanagedVariable symVar in symScope.Locals) {
				yield return GetLocalVariable(symVar);
			}
			foreach(ISymUnmanagedScope childScope in symScope.Children) {
				foreach(LocalVariable var in GetLocalVariablesInScope(childScope)) {
					yield return var;
				}
			}
		}
		
		LocalVariable GetLocalVariable(ISymUnmanagedVariable symVar)
		{
			return new LocalVariable(
				symVar.Name,
				new Value(
					process,
					new IExpirable[] {this},
					new IMutable[] {process.DebugeeState},
					delegate { return GetCorValueOfLocalVariable(symVar); }
				)
			);
		}
		
		ICorDebugValue GetCorValueOfLocalVariable(ISymUnmanagedVariable symVar)
		{
			if (this.HasExpired) throw new CannotGetValueException("Function has expired");
			
			try {
				return CorILFrame.GetLocalVariable((uint)symVar.AddressField1);
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new CannotGetValueException("Unavailable in optimized code");
				throw;
			}
		}
	}
}
