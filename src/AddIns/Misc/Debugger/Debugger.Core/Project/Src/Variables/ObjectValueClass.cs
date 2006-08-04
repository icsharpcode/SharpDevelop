// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	public class ObjectValueClass: RemotingObjectBase
	{
		NDebugger debugger;
		
		ObjectValue objectValue;
		
		Module module;
		ICorDebugClass corClass;
		TypeDefProps classProps;
		
		ICorDebugObjectValue CorObjectValue {
			get {
				return objectValue.CorValue.As<ICorDebugObjectValue>();
			}
		}
		
		bool IsCorValueCompatible {
			get {
				return objectValue.IsCorValueCompatible;
			}
		}
		
		public Module Module {
			get {
				return module;
			}
		}
		
		public string Type { 
			get{ 
				return classProps.Name;
			} 
		}
		
		internal ICorDebugClass CorClass {
			get {
				return corClass;
			}
		}
		
		public uint ClassToken {
			get {
				return classProps.Token;
			}
		}
		
		public ObjectValueClass(ObjectValue objectValue, ICorDebugClass corClass)
		{
			this.debugger = objectValue.Debugger;
			this.objectValue = objectValue;
			this.module = debugger.GetModule(corClass.Module);
			this.corClass = corClass;
			this.classProps = Module.MetaData.GetTypeDefProps(corClass.Token);
		}
		
		public VariableCollection SubVariables {
			get {
				return new VariableCollection("Base class",
				                              "{" + Type + "}",
				                              SubCollections,
				                              GetSubVariables(Variable.Flags.Public, Variable.Flags.PublicStatic));
			}
		}
		
		IEnumerable<VariableCollection> SubCollections {
			get {
				ObjectValueClass baseClass = BaseClass;
				VariableCollection privateStatic = new VariableCollection("Private static members",
				                                                          String.Empty,
				                                                          new VariableCollection[0],
				                                                          GetSubVariables(Variable.Flags.Static, Variable.Flags.PublicStatic));
				VariableCollection privateInstance = new VariableCollection("Private members",
				                                                            String.Empty,
				                                                            privateStatic.IsEmpty? new VariableCollection[0] : new VariableCollection[] {privateStatic},
					                                                        GetSubVariables(Variable.Flags.None, Variable.Flags.PublicStatic));
				VariableCollection publicStatic = new VariableCollection("Static members",
				                                                         String.Empty,
					                                                     new VariableCollection[0],
					                                                     GetSubVariables(Variable.Flags.PublicStatic, Variable.Flags.PublicStatic));
				if (baseClass != null) {
					yield return baseClass.SubVariables;
				}
				if (!privateInstance.IsEmpty) {
					yield return privateInstance;
				}
				if (!publicStatic.IsEmpty) {
					yield return publicStatic;
				}
			}
		}
		
		IEnumerable<Variable> GetSubVariables(Variable.Flags requiredFlags, Variable.Flags mask) {
			foreach(Variable var in GetFieldVariables()) {
				if ((var.VariableFlags & mask) == requiredFlags) {
					yield return var;
				}
			}
			
			foreach(Variable var in GetPropertyVariables()) {
				if ((var.VariableFlags & mask) == requiredFlags) {
					yield return var;
				}
			}
		}
		
		public ObjectValueClass BaseClass {
			get {
				ICorDebugClass superClass = GetSuperClass(debugger, corClass);
				if (superClass != null) {
					return new ObjectValueClass(objectValue, superClass);
				} else {
					return null;
				}
			}
		}
		
		public IEnumerable<Variable> GetFieldVariables()
		{
			foreach(FieldProps f in Module.MetaData.EnumFields(ClassToken)) {
				FieldProps field = f; // One per scope/delegate
				if (field.IsStatic && field.IsLiteral) continue; // Skip field
				yield return new Variable(debugger,
				                          field.Name,
				                          (field.IsStatic ? Variable.Flags.Static : Variable.Flags.None) |
				                          (field.IsPublic ? Variable.Flags.Public : Variable.Flags.None),
				                          new IExpirable[] {this.objectValue.Variable},
				                          new IMutable[] {this.objectValue.Variable},
				                          delegate { return GetCorValueOfField(field); });
			}
		}
		
		ICorDebugValue GetCorValueOfField(FieldProps field)
		{
			if (!IsCorValueCompatible) throw new CannotGetValueException("Object type changed");

			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (debugger.IsPaused && debugger.SelectedThread != null && debugger.SelectedThread.LastFunction != null && debugger.SelectedThread.LastFunction.CorILFrame != null) {
				curFrame = debugger.SelectedThread.LastFunction.CorILFrame.CastTo<ICorDebugFrame>();
			}
			
			try {
				if (field.IsStatic) {
					return corClass.GetStaticFieldValue(field.Token, curFrame);
				} else {
					return CorObjectValue.GetFieldValue(corClass, field.Token);
				}
			} catch {
				throw new CannotGetValueException("Can not get value of field");
			}
		}
		
		IEnumerable<MethodProps> Methods {
			get {
				return this.Module.MetaData.EnumMethods(this.ClassToken);
			}
		}
		
		public IEnumerable<Variable> GetPropertyVariables()
		{
			foreach(MethodProps m in Methods) {
				MethodProps method = m; // One per scope/delegate
				if (method.HasSpecialName && method.Name.StartsWith("get_") && method.Name != "get_Item") {
					Eval eval = Eval.CallFunction(debugger,
					                              Module.CorModule.GetFunctionFromToken(method.Token),
					                              true, // reevaluateAfterDebuggeeStateChange
					                              method.IsStatic? null : this.objectValue.Variable,
					                              new Variable[] {});
					Variable var = eval.Result;
					var.Name = method.Name.Remove(0, 4);
					var.VariableFlags = (method.IsStatic ? Variable.Flags.Static : Variable.Flags.None) |
					                    (method.IsPublic ? Variable.Flags.Public : Variable.Flags.None);
					yield return var;
				}
			}
		}
		
		protected static ICorDebugClass GetSuperClass(NDebugger debugger, ICorDebugClass currClass)
		{
			Module currModule = debugger.GetModule(currClass.Module);
			uint superToken = currModule.MetaData.GetTypeDefProps(currClass.Token).SuperClassToken;
			
			// It has no base class
			if ((superToken & 0x00FFFFFF) == 0x00000000) return null;
			
			// TypeDef - Localy defined
			if ((superToken & 0xFF000000) == 0x02000000) {
				return currModule.CorModule.GetClassFromToken(superToken);
			}
			
			// TypeRef - Referencing to external assembly
			if ((superToken & 0xFF000000) == 0x01000000) {
				string fullTypeName = currModule.MetaData.GetTypeRefProps(superToken).Name;
				
				foreach (Module superModule in debugger.Modules) {
					// TODO: Does not work for nested
					// TODO: preservesig
					try	{
						uint token = superModule.MetaData.FindTypeDefByName(fullTypeName, 0).Token;
						return superModule.CorModule.GetClassFromToken(token);
					} catch {
						continue;
					}
				}
			}
			
			throw new DebuggerException("Superclass not found");
		}
	}
}
