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
	public class ObjectValue: Value
	{
		Module finalCorClassModule;
		uint finalCorClassToken;
		
		Module module;
		ICorDebugClass corClass;
		TypeDefProps classProps;
		
		protected ICorDebugObjectValue CorObjectValue {
			get {
				return this.CorValue.CastTo<ICorDebugObjectValue>();
			}
		}
		
		public override string AsString {
			get {
				return "{" + Type + "}";
			}
		}
		
		public override string Type { 
			get{ 
				return classProps.Name;
			} 
		}
		
		public Module Module {
			get {
				return module;
			}
		}
		
		public uint ClassToken {
			get {
				return classProps.Token;
			}
		}
		
		public IEnumerable<ObjectValue> SuperClasses {
			get {
				ICorDebugClass currentClass = corClass;
				do {
					yield return new ObjectValue(debugger, this.PersistentValue, currentClass);
					currentClass = GetSuperClass(debugger, currentClass);
				} while (currentClass != null);
			}
		}
		
		internal bool IsSuperClass(ICorDebugClass corClass)
		{
			foreach(ObjectValue superClass in SuperClasses) {
				if (superClass.corClass == corClass) return true;
			}
			return false;
		}
		
		public ObjectValue BaseClass {
			get	{
				ICorDebugClass superClass = GetSuperClass(debugger, corClass);
				if (superClass == null) throw new DebuggerException("Does not have a base class");
				return new ObjectValue(debugger, this.PersistentValue, superClass);
			}
		}
		
		public bool HasBaseClass {
			get {
				return GetSuperClass(debugger, corClass) != null;
			}
		}
		
		internal ObjectValue(NDebugger debugger, PersistentValue pValue):base(debugger, pValue)
		{
			InitObjectValue(this.CorObjectValue.Class);
		}

		internal ObjectValue(NDebugger debugger, PersistentValue pValue, ICorDebugClass corClass):base(debugger, pValue)
		{
			InitObjectValue(corClass);
		}

		void InitObjectValue(ICorDebugClass corClass)
		{
			this.finalCorClassModule = debugger.GetModule(this.CorObjectValue.Class.Module);
			this.finalCorClassToken = this.CorObjectValue.Class.Token;
			
			this.module = debugger.GetModule(corClass.Module);
			this.corClass = corClass;
			this.classProps = Module.MetaData.GetTypeDefProps(corClass.Token);
		}
		
		bool IsCorValueCompatible {
			get {
				ObjectValue freshValue = this.FreshValue as ObjectValue;
				return freshValue != null &&
				       this.finalCorClassModule == freshValue.Module &&
				       this.finalCorClassToken == freshValue.ClassToken;
			}
		}
		
		public ObjectValue ObjectClass {
			get {
				ObjectValue objectClass = this;
				while (objectClass.HasBaseClass) {
					objectClass = objectClass.BaseClass;
				}
				return objectClass;
			}
		}
		
		IEnumerable<MethodProps> Methods {
			get {
				return this.Module.MetaData.EnumMethods(this.ClassToken);
			}
		}

		public override bool MayHaveSubVariables {
			get {
				return true;
			}
		}
		
		protected override IEnumerable<Variable> GetSubVariables()
		{
			if (HasBaseClass) {
				yield return GetBaseClassVariable();
			}
			
			foreach(Variable var in GetFieldVariables()) {
				yield return var;
			}
			
			foreach(Variable var in GetPropertyVariables()) {
				yield return var;
			}
		}
		
		public IEnumerable<Variable> GetFieldVariables()
		{
			foreach(FieldProps f in Module.MetaData.EnumFields(ClassToken)) {
				FieldProps field = f; // One per scope/delegate
				if (field.IsStatic && field.IsLiteral) continue; // Skip field
				if (!field.IsStatic && CorValue == null) continue; // Skip field
				yield return new Variable(field.Name,
				                          field.IsStatic,
				                          field.IsPublic,
				                          new PersistentValue(debugger,
				                                              new IExpirable[] {this.PersistentValue},
				                                              delegate { return GetCorValueOfField(field); }));
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
				throw new CannotGetValueException();
			}
		}
		
		public IEnumerable<Variable> GetPropertyVariables()
		{
			foreach(MethodProps m in Methods) {
				MethodProps method = m; // One per scope/delegate
				if (method.HasSpecialName && method.Name.StartsWith("get_") && method.Name != "get_Item") {
					Eval eval = new Eval(debugger,
					                     Module.CorModule.GetFunctionFromToken(method.Token),
					                     true, // reevaluateAfterDebuggeeStateChange
					                     method.IsStatic? null : this.PersistentValue,
					                     new PersistentValue[] {});
					yield return new Variable(method.Name.Remove(0, 4),
					                          method.IsStatic,
					                          method.IsPublic,
					                          eval.PersistentValue);
				}
			}
		}
		
		public Variable GetBaseClassVariable()
		{
			if (HasBaseClass) {
				return new Variable("<Base class>",
				                    new PersistentValue(debugger,
				                                        new IExpirable[] {this.PersistentValue},
				                                        delegate { return GetBaseClassValue(); }));
			} else {
				return null;
			}
		}
		
		Value GetBaseClassValue()
		{
			if (!IsCorValueCompatible) throw new CannotGetValueException("Object type changed");
			
			return this.BaseClass;
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
		
		/*
		// May return null
		public Eval ToStringEval { 
			get {
				ObjectValue baseClassObject = this.BaseClassObject;
				foreach(MethodProps method in baseClassObject.Methods) {
					if (method.Name == "ToString") {
						ICorDebugValue[] evalArgs;
						ICorDebugFunction evalCorFunction;
						baseClassObject.Module.CorModule.GetFunctionFromToken(method.Token, out evalCorFunction);
						evalArgs = new ICorDebugValue[] {this.SoftReference};
						return new Eval(debugger, evalCorFunction, evalArgs);
					}
				}
				throw new DebuggerException("ToString method not found");
			}
		}
		*/
	}
}
