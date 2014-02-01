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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.TypeSystem;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;

namespace Debugger
{
	/// <summary>
	/// A stack frame which is being executed on some thread.
	/// Use to obtain arguments or local variables.
	/// </summary>
	public class StackFrame: DebuggerObject
	{
		ICorDebugILFrame  corILFrame;
		long              corILFramePauseSession;
		
		/// <summary> The process in which this stack frame is executed </summary>
		public AppDomain AppDomain { get; private set; }
		
		public Process Process { get; private set; }
		
		/// <summary> A thread in which the stack frame is executed </summary>
		public Thread Thread { get; private set; }
		
		/// <summary> Internal index of the stack chain.  The value is increasing with age. </summary>
		public uint ChainIndex { get; private set; }
		
		/// <summary> Internal index of the stack frame.  The value is increasing with age. </summary>
		public uint FrameIndex { get; private set; }
		
		[Debugger.Tests.Ignore]
		public Module Module { get; private set; }
		
		public IMethod MethodInfo { get; private set; }
		
		internal ICorDebugFunction CorFunction { get; private set; }
		
		/// <summary> Returns true if this instance can not be used any more. </summary>
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
			this.Process = thread.Process;
			this.Thread = thread;
			this.AppDomain = this.Process.GetAppDomain(corILFrame.GetFunction().GetClass().GetModule().GetAssembly().GetAppDomain());
			this.corILFrame = corILFrame;
			this.corILFramePauseSession = this.Process.PauseSession;
			this.CorFunction = corILFrame.GetFunction();
			this.ChainIndex = chainIndex;
			this.FrameIndex = frameIndex;
			
			// Class parameters are first, then the method ones
			List<ICorDebugType> typeArgs = ((ICorDebugILFrame2)corILFrame).EnumerateTypeParameters().ToList();
			
			this.Module = thread.Process.GetModule(this.CorFunction.GetModule());
			this.MethodInfo = this.Module.Assembly.Compilation.Import(this.CorFunction, typeArgs);
		}
		
		/// <summary> Returns diagnostic description of the frame </summary>
		public override string ToString()
		{
			return this.MethodInfo.ToString();
		}
		
		internal ICorDebugILFrame CorILFrame {
			get {
				if (corILFramePauseSession != this.Process.PauseSession) {
					// Reobtain the stackframe
					StackFrame stackFrame = this.Thread.GetStackFrameAt(this.ChainIndex, this.FrameIndex);
					if (stackFrame.MethodInfo.UnresolvedMember != this.MethodInfo.UnresolvedMember)
						throw new DebuggerException("The stack frame on the thread does not represent the same method anymore");
					corILFrame = stackFrame.corILFrame;
					corILFramePauseSession = stackFrame.corILFramePauseSession;
				}
				return corILFrame;
			}
		}
		
		[Debugger.Tests.Ignore]
		public int IP {
			get {
				uint corInstructionPtr;
				CorDebugMappingResult mappingResult;
				CorILFrame.GetIP(out corInstructionPtr, out mappingResult);
				return (int)corInstructionPtr;
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
			Stepper.StepOut(this, "normal");
			
			this.Process.AsyncContinue(DebuggeeStateAction.Clear);
		}
		
		void AsyncStep(bool stepIn)
		{
			List<ILRange> stepRanges = new List<ILRange>();
			var symbolSource = this.Process.GetSymbolSource(this.MethodInfo);
			var seq = symbolSource.GetSequencePoint(this.MethodInfo, this.IP);
			if (seq != null) {
				Process.TraceMessage("Step over: {0} IL:{1}", seq, string.Join(" ", seq.ILRanges));
				stepRanges.AddRange(seq.ILRanges);
				stepRanges.AddRange(symbolSource.GetIgnoredILRanges(this.MethodInfo));
			}
			
			// Remove overlapping and connected ranges
			List<int> fromToList = new List<int>();
			foreach(var range in stepRanges.OrderBy(r => r.From)) {
				if (fromToList.Count > 0 && range.From <= fromToList[fromToList.Count - 1]) {
					fromToList[fromToList.Count - 1] = Math.Max(range.To, fromToList[fromToList.Count - 1]);
				} else {
					fromToList.Add(range.From);
					fromToList.Add(range.To);
				}
			}

			if (stepIn) {
				Stepper stepInStepper = Stepper.StepIn(this, fromToList.ToArray(), "normal");
				this.Thread.CurrentStepIn = stepInStepper;
				Stepper clearCurrentStepIn = Stepper.StepOut(this, "clear current step in");
				clearCurrentStepIn.StepComplete += delegate {
					if (this.Thread.CurrentStepIn == stepInStepper) {
						this.Thread.CurrentStepIn = null;
					}
				};
				clearCurrentStepIn.Ignore = true;
			} else {
				Stepper.StepOver(this, fromToList.ToArray(), "normal");
			}
			
			this.Process.AsyncContinue(DebuggeeStateAction.Clear);
		}
		
		/// <summary>
		/// Get the information about the next statement to be executed.
		/// 
		/// Returns null on error.
		/// </summary>
		public SequencePoint NextStatement {
			get {
				var symbolSource = this.Process.GetSymbolSource(this.MethodInfo);
				return symbolSource.GetSequencePoint(this.MethodInfo, this.IP);
			}
		}
		
		public bool SetIP(string filename, int line, int column, bool dryRun)
		{
			this.Process.AssertPaused();
			
			foreach(var symbolSource in this.Process.Debugger.SymbolSources) {
				foreach (var seq in symbolSource.GetSequencePoints(this.Module, filename, line, column)) {
					if (seq.MethodDefToken == this.MethodInfo.GetMetadataToken()) {
						try {
							if (dryRun) {
								CorILFrame.CanSetIP((uint)seq.ILOffset);
							} else {
								CorILFrame.SetIP((uint)seq.ILOffset);
								// Invalidates all frames and chains for the current thread
								this.Process.NotifyResumed(DebuggeeStateAction.Keep);
								this.Process.NotifyPaused();
							}
						} catch {
							return false;
						}
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary> Get instance of 'this'. </summary>
		/// <param name="followCapture"> Try to find captured 'this' for delegates and enumerators. </param>
		[Debugger.Tests.Ignore]
		public Value GetThisValue(bool followCapture)
		{
			if (followCapture) {
				foreach(LocalVariable loc in GetLocalVariables(this.IP)) {
					if (loc.IsThis)
						return loc.GetValue(this);
				}
				throw new GetValueException("The current method does not have 'this'.");
			} else {
				return new Value(this.AppDomain, GetThisCorValue());
			}
		}
		
		ICorDebugValue GetThisCorValue()
		{
			if (this.MethodInfo.IsStatic)
				throw new GetValueException("Static method does not have 'this'.");
			ICorDebugValue corValue;
			try {
				corValue = CorILFrame.GetArgument(0);
			} catch (COMException e) {
				// System.Runtime.InteropServices.COMException (0x80131304): An IL variable is not available at the current native IP. (See Forum-8640)
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Not available in the current state");
				throw;
			}
			// This can be 'by ref' for value types
			if (corValue.GetTheType() == (uint)CorElementType.BYREF) {
				corValue = ((ICorDebugReferenceValue)corValue).Dereference();
			}
			return corValue;
		}
		
		/// <summary> Total number of arguments (excluding implicit 'this' argument) </summary>
		public int ArgumentCount {
			get {
				ICorDebugValueEnum argumentEnum = CorILFrame.EnumerateArguments();
				uint argCount = argumentEnum.GetCount();
				if (!this.MethodInfo.IsStatic) {
					argCount--; // Remove 'this' from count
				}
				return (int)argCount;
			}
		}
		
		/// <summary> Gets argument with a given name </summary>
		public Value GetArgumentValue(string name)
		{
			for (int i = 0; i < this.MethodInfo.Parameters.Count; i++) {
				if (this.MethodInfo.Parameters[i].Name == name) {
					LocalVariable capturedVar;
					if (HasCapturedVariable(name, out capturedVar))
						return capturedVar.GetValue(this);
					return GetArgumentValue(i);
				}
			}
			throw new GetValueException("Argument \"{0}\" not found", name);
		}
		
		/// <summary> Gets argument with a given index </summary>
		/// <param name="index"> Zero-based index </param>
		public Value GetArgumentValue(int index)
		{
			var param = this.MethodInfo.Parameters[index];
			LocalVariable capturedVariable;
			if (HasCapturedVariable(param.Name, out capturedVariable))
				return capturedVariable.GetValue(this);
			return new Value(this.AppDomain, GetArgumentCorValue(index));
		}
		
		ICorDebugValue GetArgumentCorValue(int index)
		{
			this.Process.AssertPaused();
			
			ICorDebugValue corValue;
			try {
				// Non-static methods include 'this' as first argument
				corValue = CorILFrame.GetArgument((uint)(this.MethodInfo.IsStatic? index : (index + 1)));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
			// Method arguments can be passed 'by ref'
			if (corValue.GetTheType() == (uint)CorElementType.BYREF) {
				try {
					corValue = ((ICorDebugReferenceValue)corValue).Dereference();
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
		
		/// <summary> Gets whether a captured variable with the <paramref name="name"/> exists. </summary>
		public bool HasCapturedVariable(string name, out LocalVariable variable)
		{
			variable = GetLocalVariables(this.IP).FirstOrDefault(v => v.IsCaptured && v.Name == name);
			return variable != null;
		}
		
		/// <summary> Get all local variables </summary>
		public IEnumerable<LocalVariable> GetLocalVariables()
		{
			// TODO: This needs caching, however, note that the symbols might be chaning
			var symbolSource = this.Process.GetSymbolSource(this.MethodInfo);
			return LocalVariable.GetLocalVariables(symbolSource, this.MethodInfo);
		}
		
		/// <summary> Get local variables valid at the given IL offset </summary>
		public IEnumerable<LocalVariable> GetLocalVariables(int offset)
		{
			return GetLocalVariables().Where(v => v.ILRanges.Any(r => r.From <= offset && offset < r.To));
		}
		
		/// <summary> Get local variable with given name which is valid at the current IP </summary>
		/// <returns> Null if not found </returns>
		public Value GetLocalVariableValue(string name)
		{
			var loc = GetLocalVariables(this.IP).FirstOrDefault(v => v.Name == name);
			if (loc == null)
				throw new GetValueException("Local variable \"{0}\" not found", name);
			return loc.GetValue(this);
		}
		
		/// <summary> Gets value indicating whether this method should be stepped over according to current options </summary>
		public bool IsNonUserCode {
			get {
				Options opt = this.Process.Options;
				var symbolSource = this.Process.GetSymbolSource(this.MethodInfo);
				
				if (!symbolSource.Handles(this.MethodInfo))
					return true;
				if (symbolSource.IsCompilerGenerated(this.MethodInfo))
					return true;
				if (opt.StepOverDebuggerAttributes && this.MethodInfo.HasStepOverAttribute())
					return true;
				if (opt.StepOverAllProperties && this.MethodInfo.IsAccessor)
					return true;
				if (opt.StepOverFieldAccessProperties && this.MethodInfo.IsAccessor && this.MethodInfo.GetBackingFieldToken() != 0)
					return true;
				return false;
			}
		}
		
		internal void MarkAsNonUserCode()
		{
			((ICorDebugFunction2)this.CorFunction).SetJMCStatus(0 /* false */);
		}
		
		public override bool Equals(object obj)
		{
			StackFrame other = obj as StackFrame;
			return
				other != null &&
				other.Thread == this.Thread &&
				other.ChainIndex == this.ChainIndex &&
				other.FrameIndex == this.FrameIndex &&
				other.MethodInfo == this.MethodInfo;
		}
		
		public override int GetHashCode()
		{
			return (int)(this.MethodInfo.GetMetadataToken() ^ this.FrameIndex);
		}
	}
}
