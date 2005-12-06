// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Threading;

using Debugger.Interop.CorDebug;
using Debugger.Interop.MetaData;
using System.Collections.Generic;


namespace Debugger
{
	public class Function: RemotingObjectBase
	{	
		NDebugger debugger;
		
		Module module;
		ICorDebugFunction corFunction;
		ICorDebugILFrame  corILFrame;
		object            corILFrameDebuggerSessionID;
		
		Thread thread;
		uint chainIndex;
		uint frameIndex;
		
		MethodProps methodProps;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}

		public string Name { 
			get { 
				return methodProps.Name; 
			} 
		}
		
		public Module Module { 
			get { 
				return module; 
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
				ICorDebugClass corClass;
				corFunction.GetClass(out corClass);
				return corClass;
			}
		}
		
		/// <summary>
		/// True if function stepped out and is not longer valid.
		/// </summary>
		public bool HasExpired {
			get {
				if (corILFrameDebuggerSessionID == debugger.SessionID) {
					return false; // valid
				} else {
					return thread == null;
				}
			}
		}

		public ObjectValue ThisValue {
			get {
				if (IsStatic) {
					throw new DebuggerException("Static method does not have 'this'.");
				} else {
					ICorDebugValue argThis = null;
					CorILFrame.GetArgument(0, out argThis);
					return new ObjectValue(debugger, argThis, ContaingClass);
				}
			}
		}
		
		internal Function(Thread thread, uint chainIndex, uint frameIndex, ICorDebugILFrame corILFrame)
		{
			this.debugger = thread.Debugger;
			this.thread = thread;
			this.chainIndex = chainIndex;
			this.frameIndex = frameIndex;
			this.corILFrame = corILFrame;
			this.corILFrameDebuggerSessionID = debugger.SessionID;
			corILFrame.GetFunction(out corFunction);
			uint functionToken;
			corFunction.GetToken(out functionToken);
			ICorDebugModule corModule;
			corFunction.GetModule(out corModule);
			module = debugger.GetModule(corModule);
			
			methodProps = module.MetaData.GetMethodProps(functionToken);
			
			// Expiry the function when it is finished
			Stepper tracingStepper = thread.CreateStepper();
			tracingStepper.CorStepper.StepOut();
			tracingStepper.PauseWhenComplete = false;
			tracingStepper.StepComplete += delegate {
				thread = null;
			};
		}

		#region Helpping proprerties

		internal ICorDebugILFrame CorILFrame {
			get	{
				if (HasExpired) throw new DebuggerException("Function has expired");
				if (corILFrameDebuggerSessionID != debugger.SessionID) {
					corILFrame = thread.GetFunctionAt(chainIndex, frameIndex).CorILFrame;
					corILFrameDebuggerSessionID = debugger.SessionID;
				}
				return corILFrame;
			}
		}

		internal uint corInstructionPtr {
			get	{
				uint corInstructionPtr;
				CorDebugMappingResult MappingResult;
				CorILFrame.GetIP(out corInstructionPtr,out MappingResult);
				return corInstructionPtr;
			}
		}
		
		// Helpping properties for symbols

		internal ISymbolReader symReader {
			get	{
				if (module.SymbolsLoaded == false) return null;
				if (module.SymReader == null) return null;
				return module.SymReader;
			}
		}

		internal ISymbolMethod symMethod {
			get	{
				if (symReader == null) {
					return null;
				} else {
					try {
						return symReader.GetMethod(new SymbolToken((int)methodProps.Token));
					} catch {
						return null;
					}
				}
			}
		}

		#endregion

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
			ICorDebugStepper stepper;
			CorILFrame.CreateStepper(out stepper);
			stepper.StepOut();

			debugger.CurrentThread.AddActiveStepper(stepper);

			debugger.Continue();
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
			
			ICorDebugStepper stepper;
			
			if (stepIn) {
				CorILFrame.CreateStepper(out stepper);
				
				if (stepper is ICorDebugStepper2) { // Is the debuggee .NET 2.0?
					stepper.SetUnmappedStopMask(CorDebugUnmappedStop.STOP_NONE);
					(stepper as ICorDebugStepper2).SetJMC(1 /* true */);
				}
				
				fixed (int* ranges = nextSt.StepRanges) {
					stepper.StepRange(1 /* true - step in*/ , (IntPtr)ranges, (uint)nextSt.StepRanges.Length / 2);
				}
				
				debugger.CurrentThread.AddActiveStepper(stepper);
			}
			
			// Mind that step in which ends in code without symblols is cotinued
			// so the next step over ensures that we atleast do step over
			
			CorILFrame.CreateStepper(out stepper);
			
			if (stepper is ICorDebugStepper2) { // Is the debuggee .NET 2.0?
				stepper.SetUnmappedStopMask(CorDebugUnmappedStop.STOP_NONE);
				(stepper as ICorDebugStepper2).SetJMC(1 /* true */);
			}
			
			fixed (int* ranges = nextSt.StepRanges) {
				stepper.StepRange(0 /* false - step over*/ , (IntPtr)ranges, (uint)nextSt.StepRanges.Length / 2);
			}
			
			debugger.CurrentThread.AddActiveStepper(stepper);
			
			debugger.Continue();
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
		/// 'ILStart <= ILOffset <= ILEnd' and this range includes at least
		/// the returned area of source code. (May incude some extra compiler generated IL too)
		/// </summary>
		SourcecodeSegment GetSegmentForOffet(uint offset)
		{
			ISymbolMethod symMethod;
			
			symMethod = this.symMethod;
			if (symMethod == null) {
				return null;
			}
			
			int sequencePointCount = symMethod.SequencePointCount;
			
			int[] offsets     = new int[sequencePointCount];
			int[] startLine   = new int[sequencePointCount];
			int[] startColumn = new int[sequencePointCount];
			int[] endLine     = new int[sequencePointCount];
			int[] endColumn   = new int[sequencePointCount];
			
			ISymbolDocument[] Doc = new ISymbolDocument[sequencePointCount];
			
			symMethod.GetSequencePoints(
				offsets,
				Doc,
				startLine,
				startColumn,
				endLine,
				endColumn
				);
			
			SourcecodeSegment retVal = new SourcecodeSegment();
			
			// Get i for which: offsets[i] <= offset < offsets[i + 1]
			// or fallback to first element if  offset < offsets[0]
			for (int i = sequencePointCount - 1; i >= 0; i--) // backwards
				if (offsets[i] <= offset || i == 0) {
					// Set inforamtion about current IL range
					ICorDebugCode code;
					corFunction.GetILCode(out code);
					uint codeSize;
					code.GetSize(out codeSize);
					
					retVal.ILOffset = (int)offset;
					retVal.ILStart = offsets[i];
					retVal.ILEnd = (i + 1 < sequencePointCount) ? offsets[i + 1] : (int)codeSize;
					
					// 0xFeeFee means "code generated by compiler"
					// If we are in generated sequence use to closest real one instead,
					// extend the ILStart and ILEnd to include the 'real' sequence
					
					// Look ahead for 'real' sequence
					while (i + 1 < sequencePointCount && startLine[i] == 0xFeeFee) {
						i++;
						retVal.ILEnd = (i + 1 < sequencePointCount) ? offsets[i + 1] : (int)codeSize;
					}
					// Look back for 'real' sequence
					while (i - 1 >= 0 && startLine[i] == 0xFeeFee) {
						i--;
						retVal.ILStart = offsets[i];
					}
					// Wow, there are no 'real' sequences
					if (startLine[i] == 0xFeeFee) {
						return null;
					}
					
					retVal.ModuleFilename = module.FullPath;
					
					retVal.SourceFullFilename = Doc[i].URL;
					
					retVal.StartLine = startLine[i];
					retVal.StartColumn = startColumn[i];
					retVal.EndLine = endLine[i];
					retVal.EndColumn = endColumn[i];
					
					
					List<int> stepRanges = new List<int>();
					for (int j = 0; j < sequencePointCount; j++) {
						// Step over compiler generated sequences and current statement
						// 0xFeeFee means "code generated by compiler"
						if (startLine[j] == 0xFeeFee || j == i) {
							// Add start offset or remove last end (to connect two ranges into one)
							if (stepRanges.Count > 0 && stepRanges[stepRanges.Count - 1] == offsets[j]) {
								stepRanges.RemoveAt(stepRanges.Count - 1);
							} else {
								stepRanges.Add(offsets[j]);
							}
							// Add end offset | handle last sequence point
							if (j + 1 < sequencePointCount) {
								stepRanges.Add(offsets[j + 1]);
							} else {
								stepRanges.Add((int)codeSize);
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
			debugger.AssertPaused();
			
			SourcecodeSegment suggestion = new SourcecodeSegment(filename, line, column, column);
			ICorDebugFunction corFunction;
			int ilOffset;
			if (!suggestion.GetFunctionAndOffset(debugger, false, out corFunction, out ilOffset)) {
				return null;
			} else {
				uint token;
				corFunction.GetToken(out token);
				if (token != methodProps.Token) {
					return null;
				} else {
					try {
						if (simulate) {
							CorILFrame.CanSetIP((uint)ilOffset);
						} else {
							CorILFrame.SetIP((uint)ilOffset);
							debugger.FakePause(PausedReason.SetIP, false);
						}
					} catch {
						return null;
					}
					return GetSegmentForOffet((uint)ilOffset);
				}
			}
		}
		
		public IEnumerable<Variable> Variables {
			get {
				foreach(Variable var in ContaingClassVariables) {
					yield return var;
				}
				foreach(Variable var in ArgumentVariables) {
					yield return var;
				}
				foreach(Variable var in LocalVariables) {
					yield return var;
				}
				foreach(Variable var in PropertyVariables) {
					yield return var;
				}
			}
		}
		
		public IEnumerable<Variable> ContaingClassVariables {
			get {
				if (!IsStatic) {
					foreach(Variable var in ThisValue.GetSubVariables(delegate{return ThisValue;})) {
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
				ICorDebugValueEnum argumentEnum;
				CorILFrame.EnumerateArguments(out argumentEnum);
				uint argCount;
				argumentEnum.GetCount(out argCount);
				if (!IsStatic) {
					argCount--; // Remove 'this' from count
				}
				return (int)argCount;
			}
		}
		
		internal ICorDebugValue GetArgumentValue(int index)
		{
			ICorDebugValue arg;
			// Non-static functions include 'this' as first argument
			CorILFrame.GetArgument((uint)(IsStatic? index : (index + 1)), out arg);
			return arg;
		}
		
		public Variable GetArgumentVariable(int index)
		{
			return new Variable(debugger,
			                    GetParameterName(index),
			                    delegate {
			                    	return Value.CreateValue(debugger, GetArgumentValue(index));
			                    });
		}
		
		public IEnumerable<Variable> ArgumentVariables {
			get {
				for (int i = 0; i < ArgumentCount; i++) {
					yield return GetArgumentVariable(i);
				}
			}
		}
		
		public IEnumerable<Variable> LocalVariables {
			get {
				if (symMethod != null) { // TODO: Is this needed?
					ISymbolScope symRootScope = symMethod.RootScope;
					foreach(Variable var in GetLocalVariablesInScope(symRootScope)) {
						yield return var;
					}
				}
			}
		}
		
		public IEnumerable<Variable> PropertyVariables {
			get {
				foreach(MethodProps method in module.MetaData.EnumMethods(methodProps.ClassToken)) {
					if (method.Name.StartsWith("get_") && method.HasSpecialName) {					
						ICorDebugValue[] evalArgs;
						ICorDebugFunction evalCorFunction;
						Module.CorModule.GetFunctionFromToken(method.Token, out evalCorFunction);
						if (IsStatic) {
							evalArgs = new ICorDebugValue[0];
						} else {
							evalArgs = new ICorDebugValue[] {ThisValue.CorValue};
						}
						Eval eval = new Eval(debugger, evalCorFunction, evalArgs);
						// Do not add evals if we just evaluated them, otherwise we get infinite loop
						if (debugger.PausedReason != PausedReason.AllEvalsComplete) {
							debugger.AddEval(eval);
						}
						yield return new PropertyVariable(eval, method.Name.Remove(0, 4));
					}
				}
			}
		} 
		
		IEnumerable<Variable> GetLocalVariablesInScope(ISymbolScope symScope)
		{
			foreach (ISymbolVariable symVar in symScope.GetLocals()) {
				yield return GetLocalVariable(symVar);
			}
			foreach(ISymbolScope childScope in symScope.GetChildren()) {
				foreach(Variable var in GetLocalVariablesInScope(childScope)) {
					yield return var;
				}
			}
		}
		
		Variable GetLocalVariable(ISymbolVariable symVar)
		{
			return new Variable(debugger,
			                    symVar.Name,
			                    delegate {
			                    	ICorDebugValue corValue;
			                    	CorILFrame.GetLocalVariable((uint)symVar.AddressField1, out corValue);
			                    	return Value.CreateValue(debugger, corValue);
			                    });
		}
	}
}
