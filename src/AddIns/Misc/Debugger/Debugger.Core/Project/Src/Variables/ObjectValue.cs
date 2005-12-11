// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;
using Debugger.Interop.MetaData;

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
						// We need to pass reference
						ICorDebugHeapValue2 heapValue = this.CorValue as ICorDebugHeapValue2;
						if (heapValue == null) { // TODO: Investigate
							return null;
						}
						ICorDebugHandleValue corHandle;
						heapValue.CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION, out corHandle);
						evalArgs = new ICorDebugValue[] {corHandle};
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
		
		internal unsafe ObjectValue(NDebugger debugger, ICorDebugValue corValue):base(debugger, corValue)
		{
			((ICorDebugObjectValue)this.corValue).GetClass(out corClass);
			InitObjectVariable();
		}

		internal unsafe ObjectValue(NDebugger debugger, ICorDebugValue corValue, ICorDebugClass corClass):base(debugger, corValue)
		{
			this.corClass = corClass;
			InitObjectVariable();
		}

		void InitObjectVariable ()
		{
			uint classToken;
			corClass.GetToken(out classToken);
			corClass.GetModule(out corModule);
			metaData = Module.MetaData;
			
			classProps = metaData.GetTypeDefProps(classToken);
			
			corModuleSuperclass = corModule;
		}

		public override bool MayHaveSubVariables {
			get {
				return true;
			}
		}
		
		public override IEnumerable<Variable> GetSubVariables(ValueGetter getter)
		{
			if (HasBaseClass) {
				yield return BaseClassVariable;
			}
			
			foreach(FieldProps f in metaData.EnumFields(classProps.Token)) {
				FieldProps field = f; // One per scope/delegate
				if (field.IsStatic && field.IsLiteral) continue; // Skip field
				if (!field.IsStatic && corValue == null) continue; // Skip field
				yield return new Variable(debugger,
				                          field.Name,
				                          delegate {
				                          	Value updatedVal = getter();
				                          	if (this.IsEquivalentValue(updatedVal)) {
				                          		return GetValue(updatedVal, field);
				                          	} else {
				                          		return new UnavailableValue(debugger, "Object type changed");
				                          	}
				                          });
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
			if (debugger.CurrentThread != null && debugger.CurrentThread.LastFunction != null && debugger.CurrentThread.LastFunction.CorILFrame != null) {
				curFrame = debugger.CurrentThread.LastFunction.CorILFrame;
			}
			
			try {
				ICorDebugValue fieldValue;
				if (field.IsStatic) {
					corClass.GetStaticFieldValue(field.Token, curFrame, out fieldValue);
				} else {
					((ICorDebugObjectValue)val.CorValue).GetFieldValue(corClass, field.Token, out fieldValue);
				}
				return Value.CreateValue(debugger, fieldValue);
			} catch {
				return new UnavailableValue(debugger);
			}
		}
		
		public Variable BaseClassVariable {
			get {
				if (HasBaseClass) {
					return new Variable(this.BaseClass, "<Base class>");
				} else {
					return null;
				}
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
				ICorDebugClass superClass;
				corModuleSuperclass.GetClassFromToken(classProps.SuperClassToken, out superClass);
				return new ObjectValue(debugger, corValue, superClass);
			}
		}
	}
}
