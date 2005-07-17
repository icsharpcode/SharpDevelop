// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	public class ObjectVariable: Variable
	{
		ICorDebugClass corClass;
		ICorDebugModule corModule;
		MetaData metaData;
		ICorDebugModule corModuleSuperclass;
		ObjectVariable baseClass;

		TypeDefProps classProps;
		
		public override object Value { 
			get{ 
				return "{" + Type + "}"; 
			} 
		}

		public override string Type { 
			get{ 
				return classProps.Name;
			} 
		}


		internal unsafe ObjectVariable(NDebugger debugger, ICorDebugValue corValue, string name):base(debugger, corValue, name)
		{
			((ICorDebugObjectValue)this.corValue).GetClass(out corClass);
			InitObjectVariable();
		}

		internal unsafe ObjectVariable(NDebugger debugger, ICorDebugValue corValue, string name, ICorDebugClass corClass):base(debugger, corValue, name)
		{
			this.corClass = corClass;
			InitObjectVariable();
		}

		void InitObjectVariable ()
		{
			uint classToken;
			corClass.GetToken(out classToken);
			corClass.GetModule(out corModule);
			metaData = new Module(corModule).MetaData;

			classProps = metaData.GetTypeDefProps(classToken);

			corModuleSuperclass = corModule;
		}


		protected override unsafe VariableCollection GetSubVariables()
		{
			VariableCollection subVariables = new VariableCollection();

			foreach(FieldProps field in metaData.EnumFields(classProps.Token)) {

				ICorDebugValue filedValue;
				if (field.IsStatic) {
					if (field.IsLiteral) continue; // Try next field

					corClass.GetStaticFieldValue(field.Token, null, out filedValue);
				} else {
					if (corValue == null) continue; // Try next field

					((ICorDebugObjectValue)corValue).GetFieldValue(corClass, field.Token, out filedValue);
				}
				
				subVariables.Add(VariableFactory.CreateVariable(debugger, filedValue, field.Name));
			}

			return subVariables;
		}

		public unsafe ObjectVariable BaseClass {
			get	{
				if (baseClass == null) baseClass = GetBaseClass();
				if (baseClass == null) throw new UnableToGetPropertyException(this, "BaseClass", "Object doesn't have a base class");
				return baseClass;
			}
		}

		public bool HasBaseClass {
			get {
				if (baseClass == null) baseClass = GetBaseClass();
				return (baseClass != null);
			}
		}

		protected ObjectVariable GetBaseClass()
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
						m.MetaDataInterface.FindTypeDefByName(fullTypeName, 0, out classProps.SuperClassToken);
					}
					catch {
						continue;
					}
					corModuleSuperclass = m.CorModule;
					break; 
				}
				if (classProps.SuperClassToken == 0) throw new DebuggerException("Unable to get base class: " + fullTypeName);
			}

			// If it has no base class
			if ((classProps.SuperClassToken & 0x00FFFFFF) == 0)	{
				return null;
			} else {
				ICorDebugClass superClass;
				corModuleSuperclass.GetClassFromToken(classProps.SuperClassToken, out superClass);
				return new ObjectVariable(debugger, corValue, Name, superClass);
			}
		}
	}
}
