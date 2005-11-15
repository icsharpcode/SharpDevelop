// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
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
			get{ 
				return "{" + Type + "}"; 
			} 
		}

		public override string Type { 
			get{ 
				return classProps.Name;
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
			metaData = debugger.GetModule(corModule).MetaData;
			
			classProps = metaData.GetTypeDefProps(classToken);
			
			corModuleSuperclass = corModule;
		}

		public override bool MayHaveSubVariables {
			get {
				return true;
			}
		}
		
		protected override unsafe VariableCollection GetSubVariables()
		{
			VariableCollection subVariables = new VariableCollection(debugger);
			
			// Current frame is necessary to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame;
			if (debugger.CurrentThread == null || debugger.CurrentThread.LastFunction == null || debugger.CurrentThread.LastFunction.CorILFrame == null) {
				curFrame = null;
			} else {
				curFrame = debugger.CurrentThread.LastFunction.CorILFrame;
			}
			
			foreach(FieldProps field in metaData.EnumFields(classProps.Token)) {
				
				try {
					ICorDebugValue fieldValue;
					if (field.IsStatic) {
						if (field.IsLiteral) continue; // Try next field
						
						corClass.GetStaticFieldValue(field.Token, curFrame, out fieldValue);
					} else {
						if (corValue == null) continue; // Try next field
						
						((ICorDebugObjectValue)corValue).GetFieldValue(corClass, field.Token, out fieldValue);
					}
					
					subVariables.Add(VariableFactory.CreateVariable(debugger, fieldValue, field.Name));
				} catch {
					subVariables.Add(VariableFactory.CreateVariable(new UnavailableValue(debugger), field.Name));
				}
			}

			return subVariables;
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
