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
		IMetaDataImport metaData;
		uint classToken;
		uint superCallsToken;
		ICorDebugModule corModuleSuperclass;
		ObjectVariable baseClass;
		string type;
		
		public override object Value { 
			get{ 
				return "{" + type + "}"; 
			} 
		}

		public override string Type { 
			get{ 
				return type; 
			} 
		}


		internal unsafe ObjectVariable(ICorDebugValue corValue, string name):base(corValue, name)
		{
			((ICorDebugObjectValue)this.corValue).GetClass(out corClass);
			InitObjectVariable();
		}

		internal unsafe ObjectVariable(ICorDebugValue corValue, string name, ICorDebugClass corClass):base(corValue, name)
		{
			this.corClass = corClass;
			InitObjectVariable();
		}

		void InitObjectVariable ()
		{
			corClass.GetToken(out classToken);
			corClass.GetModule(out corModule);
			metaData = new Module(corModule).MetaDataInterface; //TODO

			metaData.GetTypeDefProps(classToken,
			                         NDebugger.pString,
			                         NDebugger.pStringLen,
			                         out NDebugger.unused, // real string lenght
			                         out NDebugger.unused,
			                         out superCallsToken);
			type = NDebugger.pStringAsUnicode;
			corModuleSuperclass = corModule;
		}


		protected override unsafe VariableCollection GetSubVariables()
		{
			VariableCollection subVariables = new VariableCollection();

			IntPtr fieldsEnumPtr = IntPtr.Zero;
			for(;;)
			{
				uint fieldToken;
				uint fieldCount;
				metaData.EnumFields(ref fieldsEnumPtr, classToken, out fieldToken, 1, out fieldCount);
				if (fieldCount == 0)
				{
					metaData.CloseEnum(fieldsEnumPtr);
					break;
				}

				uint attrib;
				metaData.GetFieldProps(fieldToken,
				                       out NDebugger.unused,
			                           NDebugger.pString,
			                           NDebugger.pStringLen,
			                           out NDebugger.unused, // real string lenght
				                       out attrib,
				                       IntPtr.Zero,
				                       out NDebugger.unused,
				                       out NDebugger.unused,
				                       out NDebugger.unusedPtr,
				                       out NDebugger.unused);
				string name = NDebugger.pStringAsUnicode;

				ICorDebugValue innerValue;
				if ((attrib & (uint)ClassFieldAttribute.fdStatic)!=0)
				{
					if ((attrib & (uint)ClassFieldAttribute.fdLiteral)!=0) continue; // Get next field
					corClass.GetStaticFieldValue(fieldToken, null, out innerValue);
				}
				else
				{
					if (corValue == null) continue; // Try next field

					((ICorDebugObjectValue)corValue).GetFieldValue(corClass, fieldToken, out innerValue);
				}
				
				subVariables.Add(VariableFactory.CreateVariable(innerValue, name));
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
			if ((superCallsToken & 0x01000000) != 0)
			{
				metaData.GetTypeRefProps(superCallsToken,
				                         out NDebugger.unused,
				                         NDebugger.pString,
				                         NDebugger.pStringLen,
				                         out NDebugger.unused); // real string lenght
				fullTypeName = NDebugger.pStringAsUnicode;

				superCallsToken = 0;
				foreach (Module m in NDebugger.Modules)
				{
					// TODO: Does not work for nested
					//       see FindTypeDefByName in dshell.cpp
					// TODO: preservesig
					try	{
						m.MetaDataInterface.FindTypeDefByName(fullTypeName, 0, out superCallsToken);
					}
					catch {
						continue;
					}
					corModuleSuperclass = m.CorModule;
					break; 
				}
				if (superCallsToken == 0) throw new DebuggerException("Unable to get base class: " + fullTypeName);
			}

			// If it has no base class
			if ((superCallsToken & 0x00FFFFFF) == 0)
			{
				return null;
			}
			else
			{
				ICorDebugClass superClass;
				corModuleSuperclass.GetClassFromToken(superCallsToken, out superClass);
				return new ObjectVariable(corValue, Name, superClass);
			}
		}
	}
}
