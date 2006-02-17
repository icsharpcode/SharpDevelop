// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Interop.MetaData;

namespace Debugger.Wrappers.MetaData
{
	class MetaData: IDisposable
	{
		IMetaDataImport metaData;
		
		public MetaData(ICorDebugModule pModule)
		{
			Guid guid = new Guid("{ 0x7dac8207, 0xd3ae, 0x4c75, { 0x9b, 0x67, 0x92, 0x80, 0x1a, 0x49, 0x7d, 0x44 } }");
			metaData = (IMetaDataImport)pModule.GetMetaDataInterface(ref guid);
		}
		
		public ISymUnmanagedReader GetSymReader(string fullname, string searchPath)
		{
			try {
				ISymUnmanagedBinder symBinder = new ISymUnmanagedBinder(new Debugger.Interop.CorSym.CorSymBinder_SxSClass());
				return symBinder.GetReaderForFile(metaData, fullname, searchPath);
			} catch {
				return null;
			}
		}
		
		public void Dispose()
		{
			try {
				Marshal.FinalReleaseComObject(metaData);
			} catch {
				Console.WriteLine("metaData release failed. (FinalReleaseComObject)");
			} finally {
				metaData = null;
			}
		}
		
		public TypeDefProps GetTypeDefProps(uint typeToken)
		{
			TypeDefProps typeDefProps = new TypeDefProps();
			
			typeDefProps.Token = typeToken;
			typeDefProps.Name = 
				Util.GetString(delegate(uint pStringLenght, out uint stringLenght, System.IntPtr pString) {
					metaData.GetTypeDefProps(typeDefProps.Token,
					                         pString, pStringLenght, out stringLenght, // The string to get
					                         out typeDefProps.Flags,
					                         out typeDefProps.SuperClassToken);
				});
			
			return typeDefProps;
		}
		
		public TypeRefProps GetTypeRefProps(uint typeToken)
		{
			TypeRefProps typeRefProps = new TypeRefProps();
			
			typeRefProps.Token = typeToken;
			typeRefProps.Name =
				Util.GetString(delegate(uint pStringLenght, out uint stringLenght, System.IntPtr pString) {
					uint unused;
					metaData.GetTypeRefProps(typeRefProps.Token,
					                         out unused,
					                         pString, pStringLenght,out stringLenght // The string to get
					                         );
				});
			
			return typeRefProps;
		}
		
		public IEnumerable<FieldProps> EnumFields(uint typeToken)
		{
			IntPtr enumerator = IntPtr.Zero;
			while (true) {
				uint fieldToken;
				uint fieldsFetched;
				metaData.EnumFields(ref enumerator, typeToken, out fieldToken, 1, out fieldsFetched);
				if (fieldsFetched == 0) {
					metaData.CloseEnum(enumerator);
					break;
				}
				yield return GetFieldProps(fieldToken);
			}
		}
		
		public FieldProps GetFieldProps(uint fieldToken)
		{
			FieldProps fieldProps = new FieldProps();
			
			fieldProps.Token = fieldToken;
			fieldProps.Name =
				Util.GetString(delegate(uint pStringLenght, out uint stringLenght, System.IntPtr pString) {
					uint unused;
					IntPtr unusedPtr = IntPtr.Zero;
					metaData.GetFieldProps(fieldProps.Token,
					                       out fieldProps.ClassToken,
					                       pString, pStringLenght, out stringLenght, // The string to get
					                       out fieldProps.Flags,
					                       IntPtr.Zero,
					                       out unused,
					                       out unused,
					                       out unusedPtr,
					                       out unused);
				});
			
			return fieldProps;
		}
		
		public IEnumerable<MethodProps> EnumMethods(uint typeToken)
		{
			IntPtr enumerator = IntPtr.Zero;
			while(true) {
				uint methodToken;
				uint methodsFetched;
				metaData.EnumMethods(ref enumerator, typeToken, out methodToken, 1, out methodsFetched);
				if (methodsFetched == 0) {
					metaData.CloseEnum(enumerator);
					break;
				}
				yield return GetMethodProps(methodToken);
			}
		}
		
		public unsafe MethodProps GetMethodProps(uint methodToken)
		{
			MethodProps methodProps = new MethodProps();
			
			methodProps.Token = methodToken;
			methodProps.Name =
				Util.GetString(delegate(uint pStringLenght, out uint stringLenght, System.IntPtr pString) {
					uint sigBlobSize;
					metaData.GetMethodProps(methodProps.Token,
					                        out methodProps.ClassToken,
					                        pString, pStringLenght, out stringLenght, // The string to get
					                        out methodProps.Flags,
					                        IntPtr.Zero,//new IntPtr(&pSigBlob),
					                        out sigBlobSize,
					                        out methodProps.CodeRVA,
					                        out methodProps.ImplFlags);
				});
			
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
			ParamProps paramProps = new ParamProps();
			
			paramProps.Token = paramToken;
			paramProps.Name =
				Util.GetString(delegate(uint pStringLenght, out uint stringLenght, System.IntPtr pString) {
					uint unused;
					metaData.GetParamProps(paramProps.Token,
					                       out paramProps.MethodToken,
					                       out paramProps.ParameterSequence,
					                       pString, pStringLenght, out stringLenght, // The string to get
					                       out paramProps.Flags,
					                       out unused,
					                       IntPtr.Zero,
					                       out unused);
				});
			
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
