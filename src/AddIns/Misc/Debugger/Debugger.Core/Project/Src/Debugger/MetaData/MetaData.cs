// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using DebuggerInterop.MetaData;
using System.Runtime.InteropServices;

namespace DebuggerLibrary
{
	class MetaData
	{
		IMetaDataImport metaData;

		public MetaData(IMetaDataImport metaData)
		{
			this.metaData = metaData;
		}

		public TypeDefProps GetTypeDefProps(uint typeToken)
		{
			TypeDefProps typeDefProps;

			typeDefProps.Token = typeToken;

			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;

			// Get length of string
			metaData.GetTypeDefProps(typeDefProps.Token,
									 pString,
									 pStringLenght,
									 out pStringLenght,
									 out typeDefProps.Flags,
			                         out typeDefProps.SuperClassToken);

			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);

			// Get properties
			metaData.GetTypeDefProps(typeDefProps.Token,
									 pString,
									 pStringLenght,
									 out pStringLenght,
									 out typeDefProps.Flags,
			                         out typeDefProps.SuperClassToken);

			typeDefProps.Name = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);

			return typeDefProps;
		}

		public TypeRefProps GetTypeRefProps(uint typeToken)
		{
			TypeRefProps typeRefProps;

			typeRefProps.Token = typeToken;

			uint unused;
			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;
			metaData.GetTypeRefProps(typeRefProps.Token,
			                         out unused,
			                         pString,
			                         pStringLenght,
									 out pStringLenght); // real string lenght
			
			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);

			metaData.GetTypeRefProps(typeRefProps.Token,
									 out unused,
									 pString,
									 pStringLenght,
									 out pStringLenght); // real string lenght

			typeRefProps.Name = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);

			return typeRefProps;
		}

		public IList<FieldProps> EnumFields(uint typeToken)
		{
			List<FieldProps> fields = new List<FieldProps>();

			IntPtr enumerator = IntPtr.Zero;
			while (true) {
				uint fieldToken;
				uint fieldsFetched;
				metaData.EnumFields(ref enumerator, typeToken, out fieldToken, 1, out fieldsFetched);
				if (fieldsFetched == 0) {
					metaData.CloseEnum(enumerator);
					break;
				}
				fields.Add(GetFieldProps(fieldToken));
			}

			return fields;
		}

		public FieldProps GetFieldProps(uint fieldToken)
		{
			FieldProps fieldProps;

			fieldProps.Token = fieldToken;

			uint unused;
			IntPtr unusedPtr = IntPtr.Zero;
			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;
			metaData.GetFieldProps(fieldProps.Token,
			                       out fieldProps.ClassToken,
			                       pString,
			                       pStringLenght,
			                       out pStringLenght, // real string lenght
			                       out fieldProps.Flags,
			                       IntPtr.Zero,
			                       out unused,
			                       out unused,
			                       out unusedPtr,
			                       out unused);

			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);

			metaData.GetFieldProps(fieldProps.Token,
			                       out fieldProps.ClassToken,
			                       pString,
			                       pStringLenght,
			                       out pStringLenght, // real string lenght
			                       out fieldProps.Flags,
			                       IntPtr.Zero,
			                       out unused,
			                       out unused,
			                       out unusedPtr,
			                       out unused);

			fieldProps.Name = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);

			return fieldProps;
		}

		public IList<MethodProps> EnumMethods(uint typeToken)
		{
			IList<MethodProps> methods = new List<MethodProps>();

			IntPtr enumerator = IntPtr.Zero;
			while(true) {
				uint methodToken;
				uint methodsFetched;
				metaData.EnumMethods(ref enumerator, typeToken, out methodToken, 1, out methodsFetched);
				if (methodsFetched == 0) {
					metaData.CloseEnum(enumerator);
					break;
				}
				methods.Add(GetMethodProps(methodToken));
			}

			return methods;
		}

		public unsafe MethodProps GetMethodProps(uint methodToken)
		{
			MethodProps methodProps;

			methodProps.Token = methodToken;

			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;
			//IntPtr pSigBlob;
			uint sigBlobSize;
			metaData.GetMethodProps(methodProps.Token,
			                        out methodProps.ClassToken,
			                        pString,
			                        pStringLenght,
			                        out pStringLenght, // real string lenght
			                        out methodProps.Flags,
			                        IntPtr.Zero,//new IntPtr(&pSigBlob),
			                        out sigBlobSize,
			                        out methodProps.CodeRVA,
			                        out methodProps.ImplFlags);

			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);

			metaData.GetMethodProps(methodProps.Token,
			                        out methodProps.ClassToken,
			                        pString,
			                        pStringLenght,
			                        out pStringLenght, // real string lenght
			                        out methodProps.Flags,
			                        IntPtr.Zero,//new IntPtr(&pSigBlob),
			                        out sigBlobSize,
			                        out methodProps.CodeRVA,
			                        out methodProps.ImplFlags);

			methodProps.Name = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);
			
			methodProps.Signature = null;
			//methodProps.Signature = new SignatureStream(pSigBlob, sigBlobSize);
			
			//Marshal.FreeCoTaskMem(pSigBlob);

			return methodProps;
		}

		public ParamProps GetParamForMethodIndex(uint methodToken, uint parameterSequence)
		{
			uint paramToken = 0;
			metaData.GetParamForMethodIndex(methodToken, parameterSequence, ref paramToken);
			return GetParamProps(paramToken);
		}

		public ParamProps GetParamProps(uint paramToken)
		{
			ParamProps paramProps;

			paramProps.Token = paramToken;

			uint unused;
			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;
			metaData.GetParamProps(paramProps.Token,
			                       out paramProps.MethodToken,
			                       out paramProps.ParameterSequence,
			                       pString,
			                       pStringLenght,
			                       out pStringLenght, // real string lenght
			                       out paramProps.Flags,
			                       out unused,
			                       IntPtr.Zero,
			                       out unused);

			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);
			
			metaData.GetParamProps(paramProps.Token,
			                       out paramProps.MethodToken,
			                       out paramProps.ParameterSequence,
			                       pString,
			                       pStringLenght,
			                       out pStringLenght, // real string lenght
			                       out paramProps.Flags,
			                       out unused,
			                       IntPtr.Zero,
			                       out unused);

			paramProps.Name = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);

			return paramProps;
		}

		public TypeDefProps FindTypeDefByName(string typeName, uint enclosingClassToken)
		{
			uint typeDefToken;
			metaData.FindTypeDefByName(typeName, enclosingClassToken, out typeDefToken);
			return GetTypeDefProps(typeDefToken);
		}
	}
}
