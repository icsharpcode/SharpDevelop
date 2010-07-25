// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;

namespace NoGoop.Win32
{
	[ComVisible(true), Guid("00000118-0000-0000-C000-000000000046"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IOleClientSite {
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int SaveObject();
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetMoniker([In, MarshalAs(UnmanagedType.U4)] 
							   int dwAssign,
							   [In, MarshalAs(UnmanagedType.U4)] 
							   int dwWhichMoniker,
							   [Out, MarshalAs(UnmanagedType.Interface)] 
							   out Object ppmk);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetContainer([Out] out IOleContainer ppContainer);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int ShowObject();
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int OnShowWindow([In, MarshalAs(UnmanagedType.I4)] int fShow);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int RequestNewObjectLayout();
		}

	[ComVisible(true), ComImport(),
	Guid("00000112-0000-0000-C000-000000000046"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IOleObject {
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int SetClientSite([In, MarshalAs(UnmanagedType.Interface)]
								  IOleClientSite pClientSite);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetClientSite(out IOleClientSite site);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] 
								 String szContainerApp, 
								 [In, MarshalAs(UnmanagedType.LPWStr)] 
								 String szContainerObj);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int Close([In, MarshalAs(UnmanagedType.I4)] int dwSaveOption);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int SetMoniker([In, MarshalAs(UnmanagedType.U4)]
							   int dwWhichMoniker,
							   [In, MarshalAs(UnmanagedType.Interface)]
							   Object pmk);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetMoniker([In, MarshalAs(UnmanagedType.U4)]
							   int dwAssign,
							   [In, MarshalAs(UnmanagedType.U4)] 
							   int dwWhichMoniker, 
							   out Object moniker);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int InitFromData([In, MarshalAs(UnmanagedType.Interface)]
								 Object pDataObject,
								 [In, MarshalAs(UnmanagedType.I4)] 
								 int fCreation,
								 [In, MarshalAs(UnmanagedType.U4)]
								 int dwReserved);
			int GetClipboardData([In, MarshalAs(UnmanagedType.U4)]
								 int dwReserved,
								 out Object data);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int DoVerb([In, MarshalAs(UnmanagedType.I4)] 
						   int iVerb,
						   [In] IntPtr lpmsg,
						   [In, MarshalAs(UnmanagedType.Interface)] 
						   IOleClientSite pActiveSite,
						   [In, MarshalAs(UnmanagedType.I4)]
						   int lindex,
						   [In] IntPtr hwndParent,
						   [In] RECT
						   lprcPosRect);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int EnumVerbs(out Object e); // IEnumOLEVERB
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int OleUpdate();
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int IsUpToDate();
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetUserClassID([In, Out] ref Guid pClsid);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetUserType([In, MarshalAs(UnmanagedType.U4)]
								int dwFormOfType,
								[Out, MarshalAs(UnmanagedType.LPWStr)]
								out String userType);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int SetExtent([In, MarshalAs(UnmanagedType.U4)]
							  int dwDrawAspect,
							  [In] Object pSizel); // tagSIZEL
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetExtent([In, MarshalAs(UnmanagedType.U4)]
							  int dwDrawAspect,
							  [Out] Object pSizel); // tagSIZEL
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int Advise([In, MarshalAs(UnmanagedType.Interface)]
						   Object pAdvSink, 
						   out int cookie);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int EnumAdvise(out Object e);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)]
								  int dwAspect,
								  out int misc);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int SetColorScheme([In] Object pLogpal); // tagLOGPALETTE
		}

	[ComVisible(true), Guid("0000011B-0000-0000-C000-000000000046"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IOleContainer {
			void ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)]
								  Object pbc,
								  [In, MarshalAs(UnmanagedType.BStr)]
								  String pszDisplayName,
								  [Out, MarshalAs(UnmanagedType.LPArray)]
								  int[] pchEaten,
								  [Out, MarshalAs(UnmanagedType.LPArray)]
								  Object[] ppmkOut);
			void EnumObjects([In, MarshalAs(UnmanagedType.U4)] 
							 int grfFlags,
							 [Out, MarshalAs(UnmanagedType.LPArray)]
							 Object[] ppenum);
			void LockContainer([In, MarshalAs(UnmanagedType.I4)] int fLock);
		}

	[ComVisible(true), ComImport(),
	Guid("7FD52380-4E07-101B-AE2D-08002B2EC713"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPersistStreamInit {
			void GetClassID([In, Out] ref Guid pClassID);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int IsDirty();
			void Load([In, MarshalAs(UnmanagedType.Interface)] IStream pstm);
			void Save([In, MarshalAs(UnmanagedType.Interface)]
					  IStream pstm,
					  [In, MarshalAs(UnmanagedType.I4)]
					  int fClearDirty);
			void GetSizeMax([Out, MarshalAs(UnmanagedType.LPArray)]
							long pcbSize);
			void InitNew();
		}

	[ComVisible(true), Guid("0000000C-0000-0000-C000-000000000046"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IStream {
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int Read([In] IntPtr buf,
						 [In, MarshalAs(UnmanagedType.I4)] int len);
			[return: MarshalAs(UnmanagedType.I4)][PreserveSig]
				int Write([In] IntPtr buf,
						  [In, MarshalAs(UnmanagedType.I4)] int len);
			[return: MarshalAs(UnmanagedType.I8)]
				long Seek([In, MarshalAs(UnmanagedType.I8)] long dlibMove,
						  [In, MarshalAs(UnmanagedType.I4)] int dwOrigin);
			void SetSize([In, MarshalAs(UnmanagedType.I8)] long libNewSize);
			[return: MarshalAs(UnmanagedType.I8)]
				long CopyTo([In, MarshalAs(UnmanagedType.Interface)] 
							IStream pstm,
							[In, MarshalAs(UnmanagedType.I8)] 
							long cb, 
							[Out, MarshalAs(UnmanagedType.LPArray)]
							long[] pcbRead);
			void Commit([In, MarshalAs(UnmanagedType.I4)] int grfCommitFlags);
			void Revert();
			void LockRegion([In, MarshalAs(UnmanagedType.I8)] long libOffset,
							[In, MarshalAs(UnmanagedType.I8)] long cb,
							[In, MarshalAs(UnmanagedType.I4)] int dwLockType);
			void UnlockRegion([In, MarshalAs(UnmanagedType.I8)] long libOffset,
							  [In, MarshalAs(UnmanagedType.I8)] long cb,
							  [In, MarshalAs(UnmanagedType.I4)] 
							  int dwLockType);
			void Stat([Out] Object pstatstg,
					  [In, MarshalAs(UnmanagedType.I4)] int grfStatFlag);
			[return: MarshalAs(UnmanagedType.Interface)]
				IStream Clone();
		}

	[ComImport,
	Guid("00020400-0000-0000-c000-000000000046"), 
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IDispatch
		{
			int GetTypeInfoCount([Out] out int count);

			int GetTypeInfo([In] uint mustBeZero,
							[In] int localeId,
							[Out, MarshalAs(UnmanagedType.Interface)]
							out UCOMITypeInfo typeInfo);

			void GetIDsOfNames(Object refIidDummy,
							   String[] names,
							   uint namesCount,
							   int localeId,
							   // DISPID[] ids);
							   Object[] ids);

			void Invoke(//DISPID member,
						Object member,
						Object refIidDummy,
						int localeId,
						uint flags,
						DISPPARAMS[] dispParams,
						out Object result,
						out Object exception,
						out int errorInd);

		}

	[ComImport,
	Guid("00000001-0000-0000-c000-000000000046"), 
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IClassFactory
		{
			int CreateInstance([In, MarshalAs(UnmanagedType.Interface)]
							   IntPtr outerInt,
							   [In] ref Guid iid,
							   [Out, MarshalAs(UnmanagedType.Interface)]
							   out Object newObj);
		}

	[ComImport,
	Guid("00020411-0000-0000-C000-000000000046"), 
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface ITypeLib2
		{
			// Since interface inheritance is not supported, 
			// need to add all of the UCOMITypeLib methods
			// here, just add dummy methods
			void m1();
			void m2();
			void m3();
			void m4();
			void m5();
			void m6();
			void m7();
			void m8();
			void m9();
			void m10();

			int GetCustData(ref Guid guid,
							out Object data);
		}


	[ComImport,
	Guid("0002E013-0000-0000-C000-000000000046"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface ICatInformation
		{
			// Not used, definition not provided
			int EnumCategories(int locale,
							   [Out, MarshalAs(UnmanagedType.Interface)]
							   out IEnumGUID enumCat);
			
			// This one works
			int GetCategoryDesc([In] ref Guid catId,
								[In] int localeId,
								[Out, MarshalAs(UnmanagedType.LPWStr)]
								out String descStr);

			// This one works
			int EnumClassesOfCategories([In] uint cImplemented,
										[In, MarshalAs(UnmanagedType.LPArray)]
										Guid[] catIds,
										[In] uint cRequired,
										[In, MarshalAs(UnmanagedType.LPArray)]
										Guid[] catReqIds,
										[Out, MarshalAs(UnmanagedType.Interface)]
										out IEnumGUID enumClsId);

			
			// probably wrong, not tried, but not used
			int IsClassOfCategories(ref Guid classId,
									  uint cImplemented,
									  Guid[] implcatIds,
									  uint cRequired,
									  Guid[] reqCatIds);


			// Not used, so definition is not provided
			int EnumImplCategoriesOfClass();

			// Not used, so definition is not provided
			int EnumReqCategoriesOfClass();

		}

	[ComImport,
	Guid("0002E000-0000-0000-C000-000000000046"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IEnumGUID
		{
			int Next([In] uint reqNum,
					 [Out] out Guid elements,
					 [Out] out uint returnNum);
			int Skip([In] uint num);
			int Reset();
			int Clone([Out] out IEnumGUID newGuid);
		}


	public struct CORRECT_VARDESC
	{
		public int memid;
		public string lpstrSchema;
		public VARDESC.DESCUNION u;
		public ELEMDESC elemdescVar;
		public short wVarFlags;
		public VarEnum varkind;
	}


	public struct ARRAYDESC
	{
		public TYPEDESC tdescElem;
		public short cDims;

		// We don't care about the rest of this structure, only the first
		// part.  And its hard since its variable length.
		//public int[] rgbounds;
	}


	// ActiveX Interfaces
	public class ActiveX
	{ 

		// Class that implements ICatInformation
		public static Guid CategoriesMgrCLSID = 
		new Guid("{0002e005-0000-0000-c000-000000000046}");
	
		public static Guid IUnknownIID = 
		new Guid("{00000000-0000-0000-C000-000000000046}");

		public static Guid IClassFactoryIID = 
		new Guid("{00000001-0000-0000-C000-000000000046}");

		public const String COM_ROOT_TYPE_NAME = "System.__ComObject";
		public static Type COM_ROOT_TYPE = Type.GetType(COM_ROOT_TYPE_NAME);

		public static bool TypeEqualsComRoot(Type type)
		{
			if (type.FullName.StartsWith(COM_ROOT_TYPE_NAME))
				return true;
			return false;
		}

		public static String[,]          VTypes0 =
		{ 
			{ "void", "System.IntPtr" },
			{ "null", "null" }, 
			{ "short", "System.Int16" },
			{ "long", "System.Int32" },
			{ "single", "System.Single" },
			{ "double", "System.Double" },
			{ "CURRENCY", "System.Decimal" },
			{ "DATE", "System.DateTime" },
			{ "BSTR", "System.String" },
			{ "IDispatch", "System.Object" },
			{ "SCODE", "System.Int32" },
			{ "bool", "System.Int32" },
			{ "VARIANT", "System.Object" },
			{ "IUnknown", "System.Object" },
			{ "wchar_t", "System.UInt16" },
			{ "XXX", "XXX" },
			{ "char", "System.SByte" },
			{ "unsigned char", "System.Byte" },
			{ "unsigned short", "System.UInt16" },
			{ "unsigned long", "System.UInt32" },
			{ "int64", "System.Int64" },
			{ "uint64", "System.UInt64" },
			{ "int", "System.Int32" },
			{ "unsigned int", "System.UInt32" },
			{ "void", "System.IntPtr" },
			{ "HRESULT", "System.Int32" },
			{ "PTR", "System.Int32" },
			{ "SAFEARRAY", "" },
			{ "CARRAY", "" },
			{ "USERDEFINED", "" },
			{ "LPSTR", "System.String" },
			{ "LPWSTR", "System.String" },
			{ "", "" },
			{ "", "" },
			{ "", "" },
			{ "", "" },
			{ "RECORD", "" },
			{ "INT_PTR", "System.IntPtr" },
			{ "UINT_PTR", "System.IntPtr" }
		};

		public static String[,]          VTypes64 =
		{ 
			{ "FILETIME", "" },
			{ "BLOB", "" },
			{ "STREAM", "" },
			{ "STORAGE", "" },
			{ "STREAMED_OBJECT", "" },
			{ "STORED_OBJECT", "" },
			{ "BLOB_OBJECT", "" },
			{ "CF", "" },
			{ "CLSID", "Guid" },
			{ "VERSIONED_STREAM", "" }
		};

		/**
		public enum VarEnum
			{	VT_EMPTY	= 0,
				VT_NULL	= 1,
				VT_I2	= 2,
				VT_I4	= 3,
				VT_R4	= 4,
				VT_R8	= 5,
				VT_CY	= 6,
				VT_DATE	= 7,
				VT_BSTR	= 8,
				VT_DISPATCH	= 9,
				VT_ERROR	= 10,
				VT_BOOL	= 11,
				VT_VARIANT	= 12,
				VT_UNKNOWN	= 13,
				VT_DECIMAL	= 14,
				VT_I1	= 16,
				VT_UI1	= 17,
				VT_UI2	= 18,
				VT_UI4	= 19,
				VT_I8	= 20,
				VT_UI8	= 21,
				VT_INT	= 22,
				VT_UINT	= 23,
				VT_VOID	= 24,
				VT_HRESULT	= 25,
				VT_PTR	= 26,
				VT_SAFEARRAY	= 27,
				VT_CARRAY	= 28,
				VT_USERDEFINED	= 29,
				VT_LPSTR	= 30,
				VT_LPWSTR	= 31,
				VT_RECORD	= 36,
				VT_INT_PTR	= 37,
				VT_UINT_PTR	= 38,
				VT_FILETIME	= 64,
				VT_BLOB	= 65,
				VT_STREAM	= 66,
				VT_STORAGE	= 67,
				VT_STREAMED_OBJECT	= 68,
				VT_STORED_OBJECT	= 69,
				VT_BLOB_OBJECT	= 70,
				VT_CF	= 71,
				VT_CLSID	= 72,
				VT_VERSIONED_STREAM	= 73,
				VT_BSTR_BLOB	= 0xfff,
				VT_VECTOR	= 0x1000,
				VT_ARRAY	= 0x2000,
				VT_BYREF	= 0x4000,
				VT_RESERVED	= 0x8000,
				VT_ILLEGAL	= 0xffff,
				VT_ILLEGALMASKED	= 0xfff,
				VT_TYPEMASK	= 0xfff
			} ;
		**/


		public const int                VT_TYPEMASK = 0xfff;


		[DllImport("oleaut32.dll", 
				   CharSet = CharSet.Unicode, 
				   PreserveSig = false)]
			public static extern void 
			LoadTypeLibEx(String strTypeLibName, 
						  RegKind regKind, 
						  out UCOMITypeLib typeLib);


		[DllImport("oleaut32.dll", 
				   CharSet = CharSet.Unicode, 
				   PreserveSig = false)]
			public static extern void
			RegisterTypeLib(UCOMITypeLib typeLib,
							String fullPath,
							String helpDir);


		[DllImport("oleaut32.dll", 
				   CharSet = CharSet.Unicode, 
				   PreserveSig = false)]
			public static extern void 
			UnRegisterTypeLib(ref Guid guid,
							  short majVer,
							  short minVer,
							  int locale,
							  SYSKIND sysKind);


		[DllImport("Ole32.dll", 
				   EntryPoint="CreateBindCtx", CharSet=CharSet.Auto)]
			public static extern uint
			CreateBindCtx(uint reserved, 
						  out UCOMIBindCtx bc);

		public const int CLSCTX_INPROC_SERVER = 1;
		public const int CLSCTX_INPROC_HANDLER = 2;
		public const int CLSCTX_LOCAL_SERVER = 4;
		public const int CLSCTX_REMOTE_SERVER = 16;
		public const int CLSCTX_SERVER = 
			(CLSCTX_INPROC_SERVER |
			 CLSCTX_LOCAL_SERVER |
			 CLSCTX_REMOTE_SERVER);

		[DllImport("Ole32.dll")]
			public static extern int
			CoCreateInstance(ref Guid clsId,
							 IntPtr pUnkOuter,
							 int dwClsCOntext,
							 ref Guid refIid,
							 out IntPtr ppv);


		[DllImport("Ole32.dll")]
			public static extern int 
			CoGetClassObject([In] ref Guid clsId,
							 [In] int dwClsCOntext,
							 [In] IntPtr serverInfo,
							 [In] ref Guid refIid,
							 [Out] out IntPtr ppv);


		[DllImport("Ole32.dll")]
			public static extern int 
			CoFreeUnusedLibraries();


		[DllImport("Ole32.dll")]
			public static extern bool 
			IsEqualGUID(ref Guid clsId1,
						ref Guid clsId2);


		[DllImport("Ole32.dll")]
			public static extern int
			StringFromCLSID(ref Guid clsId1,
							out String str);

		[DllImport("ole32.dll", PreserveSig=false)]
			public static extern void
			CreateStreamOnHGlobal
			(IntPtr hGlobal, 
			 Boolean fDeleteOnRelease,
			 [Out] out IStream pStream);

	}
}
