// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Debugger.Interop.CorDebug;
using Debugger.Interop.CorSym;
using Debugger.Interop.MetaData;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger.MetaData
{
	public class LocalVariable
	{
		public IMethod Method { get; private set; }
		/// <summary> Index of the local variable in method. -1 for captured variables </summary>
		public int Index { get; private set; }
		public IType Type { get; private set; }
		public string Name { get; internal set; }
		/// <summary> IL offset of the start of the variable scope (inclusive) </summary>
		public int StartOffset { get; private set; }
		/// <summary> IL offset of the end of the variable scope (exclusive) </summary>
		public int EndOffset { get; private set; }
		public bool IsThis { get; internal set; }
		public bool IsCaptured { get; internal set; }
		
		ValueGetter getter { get; set; }
		
		public LocalVariable(IMethod method, int index, IType type, string name, int startOffset, int endOffset, ValueGetter getter)
		{
			this.Method = method;
			this.Index = index;
			this.Type = type;
			this.Name = name;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
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
			string msg = this.Type.Name + " " + this.Name;
			if (IsCaptured)
				msg += " (captured)";
			return msg;
		}
		
		public static List<LocalVariable> GetLocalVariables(IMethod method, ISymUnmanagedMethod symMethod)
		{
			List<LocalVariable> localVariables;
			
			// Generated constructor may not have any symbols
			if (symMethod == null)
				return new List<LocalVariable>();
			
			localVariables = GetLocalVariablesInScope(method, symMethod.GetRootScope());
			
			if (method.DeclaringType.IsDisplayClass()) {
				// Get display class from self
				AddCapturedLocalVariables(
					localVariables,
					method,
					0,
					int.MaxValue,
					context => context.GetThisValue(false),
					method.DeclaringType
				);
				// Get dispaly classes from fields
				foreach(IField fieldInfo in method.DeclaringType.GetFields(f => f.Name.StartsWith("CS$"), GetMemberOptions.None)) {
					IField fieldInfoCopy = fieldInfo;
					AddCapturedLocalVariables(
						localVariables,
						method,
						0,
						int.MaxValue,
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
						0,
						int.MaxValue,
						context => context.GetThisValue(false)
					);
					thisVar.IsThis = true;
					localVariables.Add(thisVar);
				}
			}
			return localVariables;
		}
		
		static void AddCapturedLocalVariables(List<LocalVariable> vars, IMethod method, int scopeStartOffset, int scopeEndOffset, ValueGetter getCaptureClass, IType captureClassType)
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
						scopeStartOffset,
						scopeEndOffset,
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
		
		static List<LocalVariable> GetLocalVariablesInScope(IMethod method, ISymUnmanagedScope symScope)
		{
			List<LocalVariable> vars = new List<LocalVariable>();
			foreach (ISymUnmanagedVariable symVar in symScope.GetLocals()) {
				uint address = (uint)symVar.GetAddressField1();
				IType type = method.GetLocalVariableType((int)address);
				// Compiler generated?
				// NB: Display class does not have the compiler-generated flag
				if ((symVar.GetAttributes() & 1) == 1 || symVar.GetName().StartsWith("CS$")) {
					// Get display class from local variable
					AddCapturedLocalVariables(
						vars,
						method,
						(int)symScope.GetStartOffset(),
						(int)symScope.GetEndOffset(),
						context => GetLocalVariableValue(context, address),
						type
					);
				} else {
					LocalVariable locVar = new LocalVariable(
						method,
						(int)address,
						type,
						symVar.GetName(),
						// symVar also has Get*Offset methods, but the are not implemented
						(int)symScope.GetStartOffset(),
						(int)symScope.GetEndOffset(),
						context => GetLocalVariableValue(context, address)
					);
					vars.Add(locVar);
				}
			}
			foreach(ISymUnmanagedScope childScope in symScope.GetChildren()) {
				vars.AddRange(GetLocalVariablesInScope(method, childScope));
			}
			return vars;
		}
		
		public static Value GetLocalVariableValue(StackFrame context, uint address)
		{
			context.Process.AssertPaused();
			try {
				return new Value(context.AppDomain, context.CorILFrame.GetLocalVariable(address));
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
		}
	}
}
