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

		public IList<FieldProps> EnumFields(uint classToken)
		{
			List<FieldProps> fields = new List<FieldProps>();

			IntPtr enumerator = IntPtr.Zero;
			while (true) {
				uint fieldToken;
				uint fieldCount;
				metaData.EnumFields(ref enumerator, classToken, out fieldToken, 1, out fieldCount);
				if (fieldCount == 0) {
					metaData.CloseEnum(enumerator);
					break;
				}
				fields.Add(GetFieldProps(fieldToken));
			}

			return fields;
		}
	}
}
