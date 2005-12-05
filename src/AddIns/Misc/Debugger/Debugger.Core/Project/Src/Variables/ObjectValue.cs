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
		internal string toString;
		Eval toStringEval;

		TypeDefProps classProps;
		
		public override string AsString { 
			get{
				if (toString != null) {
					return toString;
				} else {
					if (toStringEval == null) {
						// Set up eval of ToString()
						
						ObjectValue baseClass = this;
						while (baseClass.HasBaseClass) {
							baseClass = baseClass.BaseClass;
						}
						foreach(MethodProps method in baseClass.Module.MetaData.EnumMethods(baseClass.ClassToken)) {
							if (method.Name == "ToString") {
								ICorDebugValue[] evalArgs;
								ICorDebugFunction evalCorFunction;
								baseClass.Module.CorModule.GetFunctionFromToken(method.Token, out evalCorFunction);
								// We need to pass reference
								ICorDebugHeapValue2 heapValue = this.CorValue as ICorDebugHeapValue2;
								if (heapValue == null) {
									toString = "{" + Type + "}";
									return toString;
								}
								ICorDebugHandleValue corHandle;
								heapValue.CreateHandle(CorDebugHandleType.HANDLE_WEAK_TRACK_RESURRECTION, out corHandle);
								evalArgs = new ICorDebugValue[] {corHandle};
								toStringEval = new Eval(debugger, evalCorFunction, evalArgs);
								// Do not add evals if we just evaluated them, otherwise we get infinite loop
								if (debugger.IsPaused && debugger.PausedReason != PausedReason.AllEvalsComplete) {
									debugger.AddEval(toStringEval);
									//toStringEval.SetupEvaluation(debugger.CurrentThread);
								}
								toStringEval.EvalComplete += delegate {
									toString = toStringEval.Result.AsString;
									this.OnValueChanged();
								};
							}
						}
					}
					return "{" + Type + "}";
				}
			}
		}

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
		
		public override IEnumerable<Variable> SubVariables {
			get {
				if (HasBaseClass) {
					yield return BaseClassVariable;
				}
				
				// Current frame is necessary to resolve context specific static values (eg. ThreadStatic)
				ICorDebugFrame curFrame;
				if (debugger.CurrentThread == null || debugger.CurrentThread.LastFunction == null || debugger.CurrentThread.LastFunction.CorILFrame == null) {
					curFrame = null;
				} else {
					curFrame = debugger.CurrentThread.LastFunction.CorILFrame;
				}
				
				foreach(FieldProps field in metaData.EnumFields(classProps.Token)) {
					Variable var;
					try {
						ICorDebugValue fieldValue;
						if (field.IsStatic) {
							if (field.IsLiteral) continue; // Try next field
							
							corClass.GetStaticFieldValue(field.Token, curFrame, out fieldValue);
						} else {
							if (corValue == null) continue; // Try next field
							
							((ICorDebugObjectValue)corValue).GetFieldValue(corClass, field.Token, out fieldValue);
						}
						
						var = new Variable(debugger, fieldValue, field.Name);
					} catch {
						var = new Variable(new UnavailableValue(debugger), field.Name);
					}
					yield return var;
				}
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
