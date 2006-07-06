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
		ICorDebugClass corClass;
		ICorDebugModule corModule;
		MetaData metaData;
		ICorDebugModule corModuleSuperclass;
		ObjectValue baseClass;
		
		TypeDefProps classProps;
		
		public override string AsString {
			get {
				return "{" + Type + "}";
			}
		}
		
		public ObjectValue BaseClassObject {
			get {
				ObjectValue baseClass = this;
				while (baseClass.HasBaseClass) {
					baseClass = baseClass.BaseClass;
				}
				return baseClass;
			}
		}
		
		IEnumerable<MethodProps> Methods {
			get {
				return this.Module.MetaData.EnumMethods(this.ClassToken);
			}
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
		
		public override string Type { 
			get{ 
				return classProps.Name;
			} 
		}
		
		public Module Module {
			get {
				return debugger.GetModule(corModule);
			}
		}
		
		public uint ClassToken {
			get {
				return classProps.Token;
			}
		}
		
		internal unsafe ObjectValue(NDebugger debugger, PersistentCorValue pCorValue):base(debugger, pCorValue)
		{
			corClass = this.CorValue.CastTo<ICorDebugObjectValue>().Class;
			InitObjectVariable();
		}

		internal unsafe ObjectValue(NDebugger debugger, PersistentCorValue pCorValue, ICorDebugClass corClass):base(debugger, pCorValue)
		{
			this.corClass = corClass;
			InitObjectVariable();
		}

		void InitObjectVariable ()
		{
			corModule = corClass.Module;
			metaData = Module.MetaData;
			classProps = metaData.GetTypeDefProps(corClass.Token);
			corModuleSuperclass = corModule;
		}

		public override bool MayHaveSubVariables {
			get {
				return true;
			}
		}
		
		public override IEnumerable<Variable> GetSubVariables(PersistentValue pValue)
		{
			if (HasBaseClass) {
				yield return GetBaseClassVariable(pValue);
			}
			
			foreach(Variable var in GetFieldVariables(pValue)) {
				yield return var;
			}
			
			foreach(Variable var in GetPropertyVariables(pValue)) {
				yield return var;
			}
		}
		
		public IEnumerable<Variable> GetFieldVariables(PersistentValue pValue)
		{
			foreach(FieldProps f in metaData.EnumFields(ClassToken)) {
				FieldProps field = f; // One per scope/delegate
				if (field.IsStatic && field.IsLiteral) continue; // Skip field
				if (!field.IsStatic && CorValue == null) continue; // Skip field
				yield return new ClassVariable(debugger,
				                               field.Name,
				                               field.IsStatic,
				                               field.IsPublic,
				                               new PersistentValue(delegate { return GetValueOfField(field, pValue); }));
			}
		}
		
		Value GetValueOfField(FieldProps field, PersistentValue pValue)
		{
			Value updatedVal = pValue.Value;
			if (updatedVal is UnavailableValue) return updatedVal;
			if (this.IsEquivalentValue(updatedVal)) {
				return GetValue(updatedVal, field);
			} else {
				return new UnavailableValue(debugger, "Object type changed");
			}
		}
		
		public IEnumerable<Variable> GetPropertyVariables(PersistentValue pValue)
		{
			foreach(MethodProps m in Methods) {
				MethodProps method = m; // One per scope/delegate
				if (method.HasSpecialName && method.Name.StartsWith("get_") && method.Name != "get_Item") {
					yield return new PropertyVariable(debugger,
					                                  method.Name.Remove(0, 4),
					                                  method.IsStatic,
					                                  method.IsPublic,
					                                  delegate { return CreatePropertyEval(method, pValue); });
				}
			}
		}
		
		Eval CreatePropertyEval(MethodProps method, PersistentValue pValue)
		{
			Value updatedVal = pValue.Value;
			if (updatedVal is UnavailableValue) {
				return null;
			}
			if (this.IsEquivalentValue(updatedVal)) {
				ICorDebugFunction evalCorFunction = Module.CorModule.GetFunctionFromToken(method.Token);
				
				return new Eval(debugger, evalCorFunction, delegate { return GetArgsForEval(method, pValue); });
			} else {
				return null;
			}
		}
		
		ICorDebugValue[] GetArgsForEval(MethodProps method, PersistentValue pValue)
		{
			ObjectValue updatedVal = pValue.Value as ObjectValue;
			if (this.IsEquivalentValue(updatedVal)) {
				if (method.IsStatic) {
					return new ICorDebugValue[] {};
				} else {
					if (updatedVal.SoftReference != null) {
						return new ICorDebugValue[] {updatedVal.SoftReference.CastTo<ICorDebugValue>()};
					} else {
						return new ICorDebugValue[] {updatedVal.CorValue};
					}
				}
			} else {
				return null;
			}
		}
		
		public override bool IsEquivalentValue(Value val)
		{
			ObjectValue objVal = val as ObjectValue;
			return objVal != null &&
			       objVal.ClassToken == this.ClassToken;
		}
		
		Value GetValue(Value val, FieldProps field)
		{
			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (debugger.IsPaused && debugger.SelectedThread != null && debugger.SelectedThread.LastFunction != null && debugger.SelectedThread.LastFunction.CorILFrame != null) {
				curFrame = debugger.SelectedThread.LastFunction.CorILFrame.CastTo<ICorDebugFrame>();
			}
			
			try {
				ICorDebugValue fieldValue;
				if (field.IsStatic) {
					fieldValue = corClass.GetStaticFieldValue(field.Token, curFrame);
				} else {
					fieldValue = (val.CorValue.CastTo<ICorDebugObjectValue>()).GetFieldValue(corClass, field.Token);
				}
				return Value.CreateValue(debugger, fieldValue);
			} catch {
				return new UnavailableValue(debugger);
			}
		}
		
		public Variable GetBaseClassVariable(PersistentValue pValue)
		{
			if (HasBaseClass) {
				return new Variable(debugger,
				                    "<Base class>",
				                    new PersistentValue(delegate { return GetBaseClassValue(pValue); }));
			} else {
				return null;
			}
		}
		
		Value GetBaseClassValue(PersistentValue pValue)
		{
			Value updatedVal = pValue.Value;
			if (updatedVal is UnavailableValue) return updatedVal;
			if (this.IsEquivalentValue(updatedVal)) {
				return ((ObjectValue)updatedVal).BaseClass;
			} else {
				return new UnavailableValue(debugger, "Object type changed");
			}
		}
		
		public unsafe ObjectValue BaseClass {
			get	{
				if (baseClass == null) baseClass = GetBaseClass();
				if (baseClass == null) throw new DebuggerException("Object doesn't have a base class. You may use HasBaseClass to check this.");
				return baseClass;
			}
		}

		public bool HasBaseClass {
			get {
				if (baseClass == null) {
					try {
						baseClass = GetBaseClass();
					} catch (DebuggerException) {
						baseClass = null;
					}
				}
				return (baseClass != null);
			}
		}

		protected ObjectValue GetBaseClass()
		{
			string fullTypeName = "<>";

			// If referencing to external assembly
			if ((classProps.SuperClassToken & 0x01000000) != 0)	{

				fullTypeName = metaData.GetTypeRefProps(classProps.SuperClassToken).Name;

				classProps.SuperClassToken = 0;
				foreach (Module m in debugger.Modules)
				{
					// TODO: Does not work for nested
					//       see FindTypeDefByName in dshell.cpp
					// TODO: preservesig
					try	{
						classProps.SuperClassToken = m.MetaData.FindTypeDefByName(fullTypeName, 0).Token;
					} catch {
						continue;
					}
					corModuleSuperclass = m.CorModule;
					break; 
				}
			}

			// If it has no base class
			if ((classProps.SuperClassToken & 0x00FFFFFF) == 0)	{
				throw new DebuggerException("Unable to get base class: " + fullTypeName);
			} else {
				ICorDebugClass superClass = corModuleSuperclass.GetClassFromToken(classProps.SuperClassToken);
				if (corHandleValue != null) {
					return new ObjectValue(debugger, new PersistentCorValue(debugger, corHandleValue.As<ICorDebugValue>()), superClass);
				} else {
					return new ObjectValue(debugger, new PersistentCorValue(debugger, CorValue), superClass);
				}
			}
		}
	}
}
