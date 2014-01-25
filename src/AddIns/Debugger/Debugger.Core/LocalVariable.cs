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
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.TypeSystem;
using Debugger.Interop.CorDebug;

namespace Debugger.MetaData
{
	public class LocalVariable
	{
		delegate Value Getter(StackFrame context);
	
		public IMethod Method { get; private set; }
		/// <summary> Index of the local variable in method. -1 for captured variables </summary>
		public int Index { get; private set; }
		public IType Type { get; private set; }
		public string Name { get; private set; }
		public ILRange[] ILRanges { get; private set; }
		public bool IsThis { get; private set; }
		public bool IsCaptured { get; private set; }
		
		Getter getter { get; set; }
		
		LocalVariable(IMethod method, int index, IType type, string name, ILRange[] ilranges, Getter getter)
		{
			this.Method = method;
			this.Index = index;
			this.Type = type;
			this.Name = name;
			this.ILRanges = ilranges;
			this.getter = getter;
		}
		
		public Value GetValue(StackFrame context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (context.MethodInfo != this.Method)
				throw new GetValueException("Expected stack frame: " + this.Method.Name);
			
			return getter(context);
		}
		
		public override string ToString()
		{
			return this.Type.Name + " " + this.Name + (this.IsCaptured ? " (captured)" : "");
		}
		
		public static List<LocalVariable> GetLocalVariables(ISymbolSource symbolSource, IMethod method)
		{
			var module = method.ParentAssembly.GetModule();
			
			List<LocalVariable> localVariables = new List<LocalVariable>();
			
			foreach (ILLocalVariable ilvar in symbolSource.GetLocalVariables(method)) {
				int index = ilvar.Index;
				// NB: Display class does not have the compiler-generated flag
				if (ilvar.IsCompilerGenerated || ilvar.Name.StartsWith("CS$")) {
					// Get display class from local variable
					AddCapturedLocalVariables(
						localVariables,
						method,
						ilvar.ILRanges,
						context => GetLocalVariableValue(context, index),
						ilvar.Type
					);
				} else {
					LocalVariable locVar = new LocalVariable(
						method,
						ilvar.Index,
						ilvar.Type,
						ilvar.Name,
						ilvar.ILRanges,
						context => GetLocalVariableValue(context, index)
					);
					localVariables.Add(locVar);
				}
			}
			
			if (method.DeclaringType.IsDisplayClass()) {
				// Get display class from 'this'
				AddCapturedLocalVariables(
					localVariables,
					method,
					new [] { new ILRange(0, int.MaxValue) },
					context => context.GetThisValue(false),
					method.DeclaringType
				);
				// Get display classes from fields
				foreach(IField fieldInfo in method.DeclaringType.GetFields(f => f.Name.StartsWith("CS$"), GetMemberOptions.None)) {
					IField fieldInfoCopy = fieldInfo;
					AddCapturedLocalVariables(
						localVariables,
						method,
						new [] { new ILRange(0, int.MaxValue) },
						// TODO: Use eval thread
						context => context.GetThisValue(false).GetFieldValue(context.Thread, fieldInfoCopy),
						fieldInfo.Type
					);
				}
			} else {
				// Add this
				if (!method.IsStatic) {
					LocalVariable thisVar = new LocalVariable(
						method,
						-1,
						method.DeclaringType,
						"this",
						new [] { new ILRange(0, int.MaxValue) },
						context => context.GetThisValue(false)
					);
					thisVar.IsThis = true;
					localVariables.Add(thisVar);
				}
			}
			
			return localVariables;
		}
		
		static void AddCapturedLocalVariables(List<LocalVariable> vars, IMethod method, ILRange[] ilranges, ValueGetter getCaptureClass, IType captureClassType)
		{
			if (captureClassType.IsDisplayClass()) {
				foreach(IField fieldInfo in captureClassType.GetFields()) {
					IField fieldInfoCopy = fieldInfo;
					if (fieldInfo.Name.StartsWith("CS$")) continue; // Ignore
					LocalVariable locVar = new LocalVariable(
						method,
						-1,
						fieldInfo.Type,
						fieldInfo.Name,
						ilranges,
						// TODO: Use eval thread
						context => getCaptureClass(context).GetFieldValue(context.Thread, fieldInfoCopy)
					);
					locVar.IsCaptured = true;
					if (locVar.Name.StartsWith("<>")) {
						if (locVar.Name.EndsWith("__this")) {
							locVar.Name = "this";
							locVar.IsThis = true;
						} else {
							continue; // Ignore
						}
					}
					if (locVar.Name.StartsWith("<")) {
						int endIndex = locVar.Name.IndexOf('>');
						if (endIndex == -1) continue; // Ignore
						locVar.Name = fieldInfo.Name.Substring(1, endIndex - 1);
					}
					vars.Add(locVar);
				}
			}
		}
		
		public static Value GetLocalVariableValue(StackFrame context, int index)
		{
			context.Process.AssertPaused();
			try {
				return new Value(context.AppDomain, context.CorILFrame.GetLocalVariable((uint)index));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
		}
	}
}
