// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger.Expressions;
using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;

namespace Debugger.MetaData
{
	/// <summary>
	/// Provides information about a method in a class
	/// </summary>
	public class MethodInfo: MemberInfo
	{
		MethodProps methodProps;
		
		/// <summary> Gets the name of this method </summary>
		public override string Name {
			get {
				return methodProps.Name;
			}
		}
		
		/// <summary> Gets a value indicating whether this member has the private access modifier</summary>
		public override bool IsPrivate  {
			get { return methodProps.IsPrivate; }
		}
		
		/// <summary> Gets a value indicating whether this member has the internal access modifier</summary>
		public override bool IsInternal  {
			get { return methodProps.IsInternal; }
		}
		
		/// <summary> Gets a value indicating whether this member has the protected access modifier</summary>
		public override bool IsProtected  {
			get { return methodProps.IsProtected; }
		}
		
		/// <summary> Gets a value indicating whether this member has the public access modifier</summary>
		public override bool IsPublic {
			get { return methodProps.IsPublic; }
		}
		
		/// <summary> Gets a value indicating whether the name of this method
		/// is marked as specail.</summary>
		/// <remarks> For example, property accessors are marked as special </remarks>
		public bool IsSpecialName {
			get {
				return methodProps.HasSpecialName;
			}
		}
		
		/// <summary> Gets a value indicating whether this method is static </summary>
		public override bool IsStatic {
			get {
				return methodProps.IsStatic;
			}
		}
		
		/// <summary> Gets the metadata token associated with this method </summary>
		[Debugger.Tests.Ignore]
		public override uint MetadataToken {
			get {
				return methodProps.Token;
			}
		}
		
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
		
		internal MethodInfo(DebugType declaringType, MethodProps methodProps):base (declaringType)
		{
			this.methodProps = methodProps;
		}
		
		// TODO: More accurate
		bool IsProperty {
			get {
				return this.Name.StartsWith("get_") || this.Name.StartsWith("set_");
			}
		}
		
		FieldInfo backingFieldCache;
		bool getBackingFieldCalled;
		
		internal FieldInfo BackingField {
			get {
				if (!getBackingFieldCalled) {
					backingFieldCache = GetBackingField();
					getBackingFieldCalled = true;
				}
				return backingFieldCache;
			}
		}
		
		// Is this method in form 'return this.field;'?
		FieldInfo GetBackingField()
		{
			if (this.IsStatic) return null; // TODO: Make work for static
			if (this.ParameterCount != 0) return null;
			
			ICorDebugCode corCode;
			try {
				corCode = this.CorFunction.ILCode;
			} catch {
				return null;
			}
			
			if (corCode == null) return null;
			if (corCode.Size != 7 && corCode.Size != 12) return null;
			
			byte[] code = corCode.GetCode();
			
			if (code == null) return null;
			
			/*
			string codeTxt = "";
			foreach(byte b in code) {
				codeTxt += b.ToString("X2") + " ";
			}
			this.Process.TraceMessage("Code of " + Name + ": " + codeTxt);
			*/
			
			uint token = 0;
			
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
				token = 
					((uint)code[06] << 24) +
					((uint)code[05] << 16) +
					((uint)code[04] << 8) +
					((uint)code[03]);
			}
			
			if (code.Length == 7 &&
			    code[00] == 0x02 && // ldarg.0
			    code[01] == 0x7B && // ldfld
			    code[05] == 0x04 && //   <field token>
			    code[06] == 0x2A)   // ret
			{
				token = 
					((uint)code[05] << 24) +
					((uint)code[04] << 16) +
					((uint)code[03] << 8) +
					((uint)code[02]);
			}
			
			if (token != 0) {
				// this.Process.TraceMessage("Token: " + token.ToString("x"));
				
				MemberInfo member = this.DeclaringType.GetMember(token);
				
				if (member == null) return null;
				if (!(member is FieldInfo)) return null;
				
				if (this.Process.Options.Verbose) {
					this.Process.TraceMessage(string.Format("Found backing field for {0}: {1}", this.FullName, member.Name));
				}
				return (FieldInfo)member;
			}
			
			return null;
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
				
				MetaDataImport metaData = this.Module.MetaData;
				hasDebuggerAttributeCache = false;
				// Look on the method
				foreach(CustomAttributeProps ca in metaData.EnumCustomAttributeProps(methodProps.Token, 0)) {
					MemberRefProps constructorMethod = metaData.GetMemberRefProps(ca.Type);
					TypeRefProps attributeType = metaData.GetTypeRefProps(constructorMethod.DeclaringType);
					if (attributeType.Name == "System.Diagnostics.DebuggerStepThroughAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerNonUserCodeAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerHiddenAttribute")
					{
						hasDebuggerAttributeCache = true;
					}
				}
				// Look on the type
				foreach(CustomAttributeProps ca in metaData.EnumCustomAttributeProps(this.DeclaringType.Token, 0)) {
					MemberRefProps constructorMethod = metaData.GetMemberRefProps(ca.Type);
					TypeRefProps attributeType = metaData.GetTypeRefProps(constructorMethod.DeclaringType);
					if (attributeType.Name == "System.Diagnostics.DebuggerStepThroughAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerNonUserCodeAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerHiddenAttribute")
					{
						hasDebuggerAttributeCache = true;
					}
				}
				return hasDebuggerAttributeCache.Value;
			}
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
		public static MethodInfo GetFromName(Process process, uint? domainID, System.Type type, string methodName, int paramCount)
		{
			if (type.IsNested) throw new DebuggerException("Not implemented for nested types");
			if (type.IsGenericType) throw new DebuggerException("Not implemented for generic types");
			if (type.IsGenericParameter) throw new DebuggerException("Type can not be generic parameter");
			
			DebugType debugType = DebugType.Create(process, domainID, type.FullName);
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
		[Debugger.Tests.Ignore]
		public int ParameterCount {
			get {
				return this.Module.MetaData.GetParamCount(this.MetadataToken);
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
		public string[] GetParameterNames()
		{
			List<string> names = new List<string>();
			for(int i = 0; i < ParameterCount; i++) {
				names.Add(GetParameterName(i));
			}
			return names.ToArray();
		}
		
		[Debugger.Tests.Ignore]
		public List<ISymUnmanagedVariable> LocalVariables {
			get {
				if (this.SymMethod != null) { // TODO: Is this needed?
					return GetLocalVariablesInScope(this.SymMethod.RootScope);
				} else {
					return new List<ISymUnmanagedVariable>();
				}
			}
		}
		
		List<ISymUnmanagedVariable> GetLocalVariablesInScope(ISymUnmanagedScope symScope)
		{
			List<ISymUnmanagedVariable> vars = new List<ISymUnmanagedVariable>();
			foreach (ISymUnmanagedVariable symVar in symScope.Locals) {
				if (!symVar.Name.StartsWith("CS$")) { // TODO: Generalize
					vars.Add(symVar);
				}
			}
			foreach(ISymUnmanagedScope childScope in symScope.Children) {
				vars.AddRange(GetLocalVariablesInScope(childScope));
			}
			return vars;
		}
		
		#region Convenience methods
		
		/// <summary> Returns expression for 'this' value of the stack frame </summary>
		public Expression GetExpressionForThis()
		{
			return new ThisReferenceExpression();
		}
		
		/// <summary> Returns expression for argument with the given index </summary>
		public Expression GetExpressionForParameter(int index)
		{
			return new ParameterIdentifierExpression(this, index);
		}
		
		/// <summary> Returns expression for argument with the given name </summary>
		public Expression GetExpressionForParameter(string name)
		{
			for(int i = 0; i < this.ParameterCount; i++) {
				if (this.GetParameterName(i) == name) {
					return GetExpressionForParameter(i);
				}
			}
			return null;
		}
		
		/// <summary> Returns expressions for all arguments of this stack frame </summary>
		public List<Expression> GetExpressionsForParameters()
		{
			List<Expression> ret = new List<Expression>();
			for (int i = 0; i < this.ParameterCount; i++) {
				ret.Add(GetExpressionForParameter(i));
			}
			ret.Sort();
			return ret;
		}
		
		/// <summary> Returns expression for the given local variable </summary>
		public Expression GetExpressionForLocalVariable(ISymUnmanagedVariable symVar)
		{
			return new LocalVariableIdentifierExpression(this, symVar);
		}
		
		/// <summary> Returns expression for local variable with the given name </summary>
		/// <returns> Null if not found </returns>
		public Expression GetExpressionForLocalVariable(string name)
		{
			foreach(ISymUnmanagedVariable symVar in this.LocalVariables) {
				if (symVar.Name == name) {
					return GetExpressionForLocalVariable(symVar);
				}
			}
			return null;
		}
		
		/// <summary> Returns expressions for all local variables </summary>
		public List<Expression> GetExpressionsForLocalVariables()
		{
			List<Expression> ret = new List<Expression>();
			foreach(ISymUnmanagedVariable symVar in this.LocalVariables) {
				ret.Add(GetExpressionForLocalVariable(symVar));
			}
			ret.Sort();
			return ret;
		}
		
		/// <summary> Returns a combined collection of all variables in this stack frame </summary>
		public List<Expression> GetExpressionsForAllVariables()
		{
			List<Expression> vars = new List<Expression>();
			
			if (!this.IsStatic) {
				vars.Add(GetExpressionForThis());
			}
			vars.AddRange(GetExpressionsForParameters());
			vars.AddRange(GetExpressionsForLocalVariables());
			
			return vars;
		}
		
		#endregion
	}
}
