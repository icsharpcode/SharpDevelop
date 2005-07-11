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
		ICorDebugModule corModuleSuperclass;
		ObjectVariable baseClass;
		uint classToken;
		uint superCallsToken;
		uint typeDefFlags;
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
			corClass.GetToken(out classToken);
			corClass.GetModule(out corModule);
			metaData = new Module(corModule).MetaDataInterface; //TODO

			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;
			// Get length of string 'type'
			metaData.GetTypeDefProps(classToken,
									 pString,
									 pStringLenght,
									 out pStringLenght,
									 out typeDefFlags,
			                         out superCallsToken);
			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);
			// Get properties
			metaData.GetTypeDefProps(classToken,
									 pString,
									 pStringLenght,
									 out pStringLenght,
									 out typeDefFlags,
									 out superCallsToken);
			type = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);

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

				uint unused;
				IntPtr unusedPtr = IntPtr.Zero;
				uint pStringLenght = 0; // Terminating character included in pStringLenght
				IntPtr pString = IntPtr.Zero;
				uint attrib;
				metaData.GetFieldProps(fieldToken,
				                       out unused,
			                           pString,
									   pStringLenght,
									   out pStringLenght, // real string lenght
				                       out attrib,
									   IntPtr.Zero,
				                       out unused,
				                       out unused,
				                       out unusedPtr,
				                       out unused);
				// Allocate string buffer
				pString = Marshal.AllocHGlobal((int)pStringLenght * 2);

				metaData.GetFieldProps(fieldToken,
									   out unused,
									   pString,
									   pStringLenght,
									   out pStringLenght, // real string lenght
									   out attrib,
									   IntPtr.Zero,
									   out unused,
									   out unused,
									   out unusedPtr,
									   out unused);

				string name = Marshal.PtrToStringUni(pString);
				Marshal.FreeHGlobal(pString);

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
				
				subVariables.Add(VariableFactory.CreateVariable(debugger, innerValue, name));
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
				uint unused;
				uint pStringLenght = 0; // Terminating character included in pStringLenght
				IntPtr pString = IntPtr.Zero;
				metaData.GetTypeRefProps(superCallsToken,
				                         out unused,
				                         pString,
				                         pStringLenght,
										 out pStringLenght); // real string lenght
				// Allocate string buffer
				pString = Marshal.AllocHGlobal((int)pStringLenght * 2);

				metaData.GetTypeRefProps(superCallsToken,
										 out unused,
										 pString,
										 pStringLenght,
										 out pStringLenght); // real string lenght

				fullTypeName = Marshal.PtrToStringUni(pString);
				Marshal.FreeHGlobal(pString);

				superCallsToken = 0;
				foreach (Module m in debugger.Modules)
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
				return new ObjectVariable(debugger, corValue, Name, superClass);
			}
		}
	}
}
