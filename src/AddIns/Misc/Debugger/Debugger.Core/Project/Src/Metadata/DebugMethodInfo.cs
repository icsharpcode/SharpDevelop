// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;
using ICSharpCode.NRefactory.Ast;
using Mono.Cecil.Signatures;

namespace Debugger.MetaData
{
	public class DebugMethodInfo: System.Reflection.MethodInfo
	{
		DebugType declaringType;
		MethodProps methodProps;
		
		internal DebugMethodInfo(DebugType declaringType, MethodProps methodProps)
		{
			this.declaringType = declaringType;
			this.methodProps = methodProps;
		}
		
		public override Type DeclaringType {
			get {
				return declaringType;
			}
		}
		
		/// <summary> The AppDomain in which this member is loaded </summary>
		public AppDomain AppDomain {
			get {
				return declaringType.AppDomain;
			}
		}
		
		/// <summary> The Process in which this member is loaded </summary>
		public Process Process {
			get {
				return declaringType.Process;
			}
		}
		
		public override int MetadataToken {
			get {
				return (int)methodProps.Token;
			}
		}
		//		public virtual Module Module { get; }
		
		public string FullName {
			get {
				return this.DeclaringType.FullName + "." + this.Name;
			}
		}
		
		public override string Name {
			get {
				return methodProps.Name;
			}
		}
		
		public override Type ReflectedType {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}
		
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual Type[] GetGenericArguments();
		//		public virtual MethodBody GetMethodBody();
		//		internal virtual RuntimeMethodHandle GetMethodHandle();
		
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return (MethodImplAttributes)methodProps.ImplFlags;
		}
		
		//		internal virtual uint GetOneTimeFlags();
		//		internal virtual uint GetOneTimeSpecificFlags();
		
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		
		public override MethodAttributes Attributes {
			get {
				return (MethodAttributes)methodProps.Flags;
			}
		}
		
		//		public virtual CallingConventions CallingConvention { get; }
		//		public virtual bool ContainsGenericParameters { get; }
		//		public virtual bool IsGenericMethod { get; }
		//		public virtual bool IsGenericMethodDefinition { get; }
		//		internal virtual bool IsOverloaded { get; }
		
		public override RuntimeMethodHandle MethodHandle {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override MethodInfo GetBaseDefinition()
		{
			throw new NotSupportedException();
		}
		
		//		public override Type[] GetGenericArguments();
		//		public virtual MethodInfo GetGenericMethodDefinition();
		//		internal virtual MethodInfo GetParentDefinition();
		//		internal override Type GetReturnType();
		//		public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments);
		//		public override bool ContainsGenericParameters { get; }
		//		public override bool IsGenericMethod { get; }
		//		public override bool IsGenericMethodDefinition { get; }
		//		public virtual ParameterInfo ReturnParameter { get; }
		
		public Type GetReturnType()
		{
			if (this.MethodDefSig.RetType.Void) return null;
			if (returnType == null) {
				returnType = DebugType.CreateFromSignature(this.Module, this.MethodDefSig.RetType.Type, declaringType);
			}
			return returnType;
		}
		
		public override ICustomAttributeProvider ReturnTypeCustomAttributes {
			get {
				throw new NotSupportedException();
			}
		}
		
		MethodDefSig methodDefSig;
		
		MethodDefSig MethodDefSig {
			get {
				if (methodDefSig == null) {
					SignatureReader sigReader = new SignatureReader(methodProps.SigBlob.GetData());
					methodDefSig = sigReader.GetMethodDefSig(0);
				}
				return methodDefSig;
			}
		}
		
		DebugType returnType;
		
		ParameterInfo[] parameters;
		
		public override ParameterInfo[] GetParameters()
		{
			if (parameters == null) {
				parameters = new ParameterInfo[this.MethodDefSig.ParamCount];
				for(int i = 0; i < parameters.Length; i++) {
					parameters[i] =
						new DebugParameterInfo(
							this,
							this.GetParameterName(i),
							DebugType.CreateFromSignature(this.Module, this.MethodDefSig.Parameters[i].Type, declaringType),
							i
						);
				}
			}
			return parameters;
		}
		
		//		internal virtual Type[] GetParameterTypes();
		//		internal virtual ParameterInfo[] GetParametersNoCopy();
		
		internal ICorDebugFunction CorFunction {
			get {
				return this.Module.CorModule.GetFunctionFromToken(this.MetadataToken);
			}
		}
		
		/// <summary> Gets value indicating whether this method should be stepped over
		/// accoring to current options </summary>
		public bool StepOver {
			get {
				Options opt = this.Process.Options;
				if (opt.StepOverNoSymbols) {
					if (this.SymMethod == null) return true;
				}
				if (opt.StepOverDebuggerAttributes) {
					if (this.HasDebuggerAttribute) return true;
				}
				if (opt.StepOverAllProperties) {
					if (this.IsProperty)return true;
				}
				if (opt.StepOverSingleLineProperties) {
					if (this.IsProperty && this.IsSingleLine) return true;
				}
				if (opt.StepOverFieldAccessProperties) {
					if (this.IsProperty && this.BackingField != null) return true;
				}
				return false;
			}
		}
		
		// TODO: More accurate
		bool IsProperty {
			get {
				return this.Name.StartsWith("get_") || this.Name.StartsWith("set_");
			}
		}
		
		DebugFieldInfo backingFieldCache;
		bool getBackingFieldCalled;
		
		/// <summary>
		/// Backing field that can be used to obtain the same value as by calling this method.
		/// </summary>
		internal DebugFieldInfo BackingField {
			get {
				if (!getBackingFieldCalled) {
					backingFieldCache = GetBackingField();
					getBackingFieldCalled = true;
				}
				return backingFieldCache;
			}
		}
		
		// Is this method in form 'return this.field;'?
		DebugFieldInfo GetBackingField()
		{
			if (this.IsStatic) return null; // TODO: Make work for static, 
											// the code size for static is 10/11 opposed to instance 11/12 - the ldarg.0 is missing
			if (this.ParameterCount != 0) return null;
			
			ICorDebugCode corCode;
			try {
				corCode = this.CorFunction.ILCode;
			} catch {
				return null;
			}
			
			if (corCode == null) return null;
			if (corCode.Size != 7 && corCode.Size != 12 && corCode.Size != 11) return null;
			
			byte[] code = corCode.GetCode();
			
			if (code == null) return null;
			
			/*
			string codeTxt = "";
			foreach(byte b in code) {
				codeTxt += b.ToString("X2") + " ";
			}
			process.TraceMessage("Code of " + Name + ": " + codeTxt);
			 */
			
			uint token = 0;
		
			// code generated for 'return this.field'
			if (code.Length == 12 &&
			    code[00] == 0x00 && // nop
			    code[01] == 0x02 && // ldarg.0
			    code[02] == 0x7B && // ldfld
			    code[06] == 0x04 && //   <field token>
			    code[07] == 0x0A && // stloc.0
			    code[08] == 0x2B && // br.s
			    code[09] == 0x00 && //   offset+00
			    code[10] == 0x06 && // ldloc.0
			    code[11] == 0x2A)   // ret
			{
				token = getTokenFromIL(code, 06);
			}
			
			// code generated for getter 'public int Prop { get; [set;] }'
			// (same as above, just leading nop is missing)
			if (code.Length == 11 &&
			    code[00] == 0x02 && // ldarg.0
			    code[01] == 0x7B && // ldfld
			    code[05] == 0x04 && //   <field token>
			    code[06] == 0x0A && // stloc.0
			    code[07] == 0x2B && // br.s
			    code[08] == 0x00 && //   offset+00
			    code[09] == 0x06 && // ldloc.0
			    code[10] == 0x2A)   // ret
			{
				token = getTokenFromIL(code, 05);
			}
			
			if (code.Length == 7 &&
			    code[00] == 0x02 && // ldarg.0
			    code[01] == 0x7B && // ldfld
			    code[05] == 0x04 && //   <field token>
			    code[06] == 0x2A)   // ret
			{
				token = getTokenFromIL(code, 05);
			}
			
			if (token != 0) {
				// process.TraceMessage("Token: " + token.ToString("x"));
				
				MemberInfo member = this.DeclaringType.GetMember(token);
				
				if (member == null) return null;
				if (!(member is DebugFieldInfo)) return null;
				
				if (this.Process.Options.Verbose) {
					this.Process.TraceMessage(string.Format("Found backing field for {0}: {1}", this.FullName, member.Name));
				}
				return (DebugFieldInfo)member;
			}
			
			return null;
		}
		
		/// <summary>
		/// Gets token from IL code.
		/// </summary>
		/// <param name="ilCode">Bytes representing the code.</param>
		/// <param name="tokenEndIndex">Index of last byte of the token.</param>
		/// <returns>IL token.</returns>
		uint getTokenFromIL(byte[] ilCode, uint tokenEndIndex)
		{
			return  ((uint)ilCode[tokenEndIndex] << 24) +
					((uint)ilCode[tokenEndIndex - 1] << 16) +
					((uint)ilCode[tokenEndIndex - 2] << 8) +
					((uint)ilCode[tokenEndIndex - 3]);
		}
		
		bool? isSingleLineCache;
		
		bool IsSingleLine {
			get {
				// Note symbols might get loaded manually later by the user
				ISymUnmanagedMethod symMethod = this.SymMethod;
				if (symMethod == null) return false; // No symbols - can not determine
				
				if (isSingleLineCache.HasValue) return isSingleLineCache.Value;
				
				List<SequencePoint> seqPoints = new List<SequencePoint>(symMethod.SequencePoints);
				seqPoints.Sort();
				
				// Remove initial "{"
				if (seqPoints.Count > 0 &&
				    seqPoints[0].Line == seqPoints[0].EndLine &&
				    seqPoints[0].EndColumn - seqPoints[0].Column <= 1) {
					seqPoints.RemoveAt(0);
				}
				
				// Remove last "}"
				int listIndex = seqPoints.Count - 1;
				if (seqPoints.Count > 0 &&
				    seqPoints[listIndex].Line == seqPoints[listIndex].EndLine &&
				    seqPoints[listIndex].EndColumn - seqPoints[listIndex].Column <= 1) {
					seqPoints.RemoveAt(listIndex);
				}
				
				// Is single line
				isSingleLineCache = seqPoints.Count == 0 || seqPoints[0].Line == seqPoints[seqPoints.Count - 1].EndLine;
				return isSingleLineCache.Value;
			}
		}
		
		bool? hasDebuggerAttributeCache;
		
		bool HasDebuggerAttribute {
			get {
				if (hasDebuggerAttributeCache.HasValue) return hasDebuggerAttributeCache.Value;
				
				hasDebuggerAttributeCache =
					// Look on the method
					HasAnyAttribute(this.Module.MetaData, methodProps.Token,
					                typeof(System.Diagnostics.DebuggerStepThroughAttribute),
					                typeof(System.Diagnostics.DebuggerNonUserCodeAttribute),
					                typeof(System.Diagnostics.DebuggerHiddenAttribute))
					||
					// Look on the type
					HasAnyAttribute(this.Module.MetaData, (uint)this.DeclaringType.MetadataToken,
					                typeof(System.Diagnostics.DebuggerStepThroughAttribute),
					                typeof(System.Diagnostics.DebuggerNonUserCodeAttribute),
					                typeof(System.Diagnostics.DebuggerHiddenAttribute));
				return hasDebuggerAttributeCache.Value;
			}
		}
		
		internal static bool HasAnyAttribute(MetaDataImport metaData, uint token, params Type[] wantedAttrTypes)
		{
			foreach(CustomAttributeProps ca in metaData.EnumCustomAttributeProps(token, 0)) {
				CorTokenType tkType = (CorTokenType)(ca.Type & 0xFF000000);
				string attributeName;
				if (tkType == CorTokenType.MemberRef) {
					MemberRefProps constructorMethod = metaData.GetMemberRefProps(ca.Type);
					attributeName = metaData.GetTypeRefProps(constructorMethod.DeclaringType).Name;
				} else if (tkType == CorTokenType.MethodDef) {
					MethodProps constructorMethod = metaData.GetMethodProps(ca.Type);
					attributeName = metaData.GetTypeDefProps(constructorMethod.ClassToken).Name;
				} else {
					throw new DebuggerException("Not expected: " + tkType);
				}
				foreach(Type wantedAttrType in wantedAttrTypes) {
					if (attributeName == wantedAttrType.FullName)
						return true;
				}
			}
			return false;
		}
		
		internal void MarkAsNonUserCode()
		{
			this.CorFunction.CastTo<ICorDebugFunction2>().SetJMCStatus(0 /* false */);
			
			if (this.Process.Options.Verbose) {
				this.Process.TraceMessage("Funciton {0} marked as non-user code", this.FullName);
			}
		}
		
		/// <summary>
		/// Get a method from a managed type, method name and argument count
		/// </summary>
		public static MethodInfo GetFromName(AppDomain appDomain, System.Type type, string methodName, int paramCount)
		{
			if (type.IsNested) throw new DebuggerException("Not implemented for nested types");
			if (type.IsGenericType) throw new DebuggerException("Not implemented for generic types");
			if (type.IsGenericParameter) throw new DebuggerException("Type can not be generic parameter");
			
			DebugType debugType = DebugType.CreateFromType(appDomain, type);
			if (debugType == null) {
				throw new DebuggerException("Type " + type.FullName + " not found");
			}
			
			foreach(MethodInfo methodInfo in debugType.GetMethods(methodName)) {
				if (methodInfo.ParameterCount == paramCount) {
					return methodInfo;
				}
			}
			throw new DebuggerException("Method " + methodName + " not found");
		}
		
		internal ISymUnmanagedMethod SymMethod {
			get {
				if (this.Module.SymReader == null) return null;
				try {
					return this.Module.SymReader.GetMethod(this.MetadataToken);
				} catch {
					return null;
				}
			}
		}
		
		/// <summary> Gets the number of paramters of this method </summary>
		public int ParameterCount {
			get {
				return this.MethodDefSig.ParamCount;
			}
		}
		
		/// <summary> Gets the name of given parameter </summary>
		/// <param name="index"> Zero-based index </param>
		public string GetParameterName(int index)
		{
			// index = 0 is return parameter
			try {
				return this.Module.MetaData.GetParamPropsForMethodIndex(this.MetadataToken, (uint)index + 1).Name;
			} catch {
				return String.Empty;
			}
		}
		
		/// <summary> Get names of all parameters in order </summary>
		public string[] ParameterNames {
			get {
				List<string> names = new List<string>();
				for(int i = 0; i < ParameterCount; i++) {
					names.Add(GetParameterName(i));
				}
				return names.ToArray();
			}
		}
		
		public List<DebugLocalVariableInfo> LocalVariables {
			get {
				if (this.SymMethod != null) { // TODO: Is this needed?
					return GetLocalVariables();
				} else {
					return new List<DebugLocalVariableInfo>();
				}
			}
		}
		
		public string[] LocalVariableNames {
			get {
				List<DebugLocalVariableInfo> vars = this.LocalVariables;
				List<string> names = new List<string>();
				for(int i = 0; i < vars.Count; i++) {
					names.Add(vars[i].Name);
				}
				names.Sort();
				return names.ToArray();
			}
		}
		
		List<DebugLocalVariableInfo> localVariables; // Cache
		
		List<DebugLocalVariableInfo> GetLocalVariables()
		{
			if (localVariables != null) return localVariables;
			
			localVariables = GetLocalVariablesInScope(this.SymMethod.RootScope);
			if (declaringType.IsDisplayClass || declaringType.IsYieldEnumerator) {
				// Get display class from self
				AddCapturedLocalVariables(
					localVariables,
					delegate(StackFrame context) {
						return context.GetThisValue();
					},
					declaringType
				);
				// Get dispaly classes from fields
				foreach(DebugFieldInfo fieldInfo in this.DeclaringType.GetFields()) {
					DebugFieldInfo fieldInfoCopy = fieldInfo;
					if (fieldInfo.Name.StartsWith("CS$")) {
						AddCapturedLocalVariables(
							localVariables,
							delegate(StackFrame context) {
								return context.GetThisValue().GetFieldValue(fieldInfoCopy);
							},
							(DebugType)fieldInfo.FieldType
						);
					}
				}
			} else {
				// Add this
				if (!this.IsStatic) {
					DebugLocalVariableInfo thisVar = new DebugLocalVariableInfo(
						"this",
						this.DeclaringType,
						delegate(StackFrame context) {
							return context.GetThisValue();
						}
					);
					thisVar.IsThis = true;
					localVariables.Add(thisVar);
				}
			}
			return localVariables;
		}
		
		static void AddCapturedLocalVariables(List<DebugLocalVariableInfo> vars, ValueGetter getCaptureClass, DebugType captureClassType)
		{
			if (captureClassType.IsDisplayClass || captureClassType.IsYieldEnumerator) {
				foreach(DebugFieldInfo fieldInfo in captureClassType.GetFields()) {
					DebugFieldInfo fieldInfoCopy = fieldInfo;
					if (fieldInfo.Name.StartsWith("CS$")) continue; // Ignore
					DebugLocalVariableInfo locVar = new DebugLocalVariableInfo(
						fieldInfo.Name,
						fieldInfo.FieldType,
						delegate(StackFrame context) {
							return getCaptureClass(context).GetFieldValue(fieldInfoCopy);
						}
					);
					locVar.IsCaptured = true;
					if (locVar.Name.StartsWith("<>")) {
						bool hasThis = false;
						foreach(DebugLocalVariableInfo l in vars) {
							if (l.IsThis) {
								hasThis = true;
								break;
							}
						}
						if (!hasThis && locVar.Name.EndsWith("__this")) {
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
		
		List<DebugLocalVariableInfo> GetLocalVariablesInScope(ISymUnmanagedScope symScope)
		{
			List<DebugLocalVariableInfo> vars = new List<DebugLocalVariableInfo>();
			foreach (ISymUnmanagedVariable symVar in symScope.Locals) {
				ISymUnmanagedVariable symVarCopy = symVar;
				int start;
				SignatureReader sigReader = new SignatureReader(symVar.Signature);
				LocalVarSig.LocalVariable locVarSig = sigReader.ReadLocalVariable(sigReader.Blob, 0, out start);
				DebugType locVarType = DebugType.CreateFromSignature(this.Module, locVarSig.Type, declaringType);
				// Compiler generated?
				// NB: Display class does not have the compiler-generated flag
				if ((symVar.Attributes & 1) == 1 || symVar.Name.StartsWith("CS$")) {
					// Get display class from local variable
					if (locVarType.IsDisplayClass) {
						AddCapturedLocalVariables(
							vars,
							delegate(StackFrame context) {
								return GetLocalVariableValue(context, symVarCopy);
							},
							locVarType
						);
					}
				} else {
					DebugLocalVariableInfo locVar = new DebugLocalVariableInfo(
						symVar.Name,
						locVarType,
						delegate(StackFrame context) {
							return GetLocalVariableValue(context, symVarCopy);
						}
					);
					vars.Add(locVar);
				}
			}
			foreach(ISymUnmanagedScope childScope in symScope.Children) {
				vars.AddRange(GetLocalVariablesInScope(childScope));
			}
			return vars;
		}
		
		static Value GetLocalVariableValue(StackFrame context, ISymUnmanagedVariable symVar)
		{
			ICorDebugValue corVal;
			try {
				corVal = context.CorILFrame.GetLocalVariable((uint)symVar.AddressField1);
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131304) throw new GetValueException("Unavailable in optimized code");
				throw;
			}
			return new Value(context.AppDomain, corVal);
		}
	}
}
