namespace DebuggerInterop.MetaData
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType((short) 1), Guid("7DAC8207-D3AE-4C75-9B67-92801A497D44"), ComConversionLoss]
    public interface IMetaDataImport
    {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CloseEnum(IntPtr hEnum);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CountEnum(IntPtr hEnum, ref uint pulCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ResetEnum(IntPtr hEnum, uint ulPos);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumTypeDefs(ref IntPtr phEnum, ref uint rTypeDefs, uint cMax, ref uint pcTypeDefs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumInterfaceImpls(ref IntPtr phEnum, uint td, ref uint rImpls, uint cMax, ref uint pcImpls);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumTypeRefs(ref IntPtr phEnum, ref uint rTypeRefs, uint cMax, ref uint pcTypeRefs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FindTypeDefByName([In, MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, [In] uint tkEnclosingClass, out uint ptd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetScopeProps([Out, MarshalAs(UnmanagedType.LPWStr)] string szName, [In] uint cchName, out uint pchName, out Guid pmvid);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetModuleFromScope(out uint pmd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetTypeDefProps([In] uint td, [Out] IntPtr szTypeDef, [In] uint cchTypeDef, out uint pchTypeDef, out uint pdwTypeDefFlags, out uint ptkExtends);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetInterfaceImplProps([In] uint iiImpl, out uint pClass, out uint ptkIface);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetTypeRefProps([In] uint tr, out uint ptkResolutionScope, [Out] IntPtr szName, [In] uint cchName, out uint pchName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ResolveTypeRef(uint tr, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] ref object ppIScope, ref uint ptd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMembers([In] ref IntPtr phEnum, [In] uint cl, out uint rMembers, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMembersWithName([In, Out] ref IntPtr phEnum, [In] uint cl, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, out uint rMembers, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMethods([In] ref IntPtr phEnum, [In] uint cl, out uint rMethods, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMethodsWithName([In] ref IntPtr phEnum, [In] uint cl, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, out uint rMethods, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumFields([In, Out] ref IntPtr phEnum, [In] uint cl, out uint rFields, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumFieldsWithName([In, Out] ref IntPtr phEnum, [In] uint cl, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, out uint rFields, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumParams([In] ref IntPtr phEnum, [In] uint mb, out uint rParams, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMemberRefs([In] ref IntPtr phEnum, [In] uint tkParent, out uint rMemberRefs, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMethodImpls([In, Out] ref IntPtr phEnum, [In] uint td, out uint rMethodBody, out uint rMethodDecl, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumPermissionSets([In, Out] ref IntPtr phEnum, [In] uint tk, [In] uint dwActions, out uint rPermission, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FindMember([In] uint td, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, [In] ref byte pvSigBlob, [In] uint cbSigBlob, out uint pmb);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FindMethod([In] uint td, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, [In] ref byte pvSigBlob, [In] uint cbSigBlob, out uint pmb);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FindField([In] uint td, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, [In] ref byte pvSigBlob, [In] uint cbSigBlob, out uint pmb);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FindMemberRef([In] uint td, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, [In] ref byte pvSigBlob, [In] uint cbSigBlob, out uint pmr);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetMethodProps([In] uint mb, out uint pClass, [Out] IntPtr szMethod, [In] uint cchMethod, out uint pchMethod, out uint pdwAttr, [Out] IntPtr ppvSigBlob, out uint pcbSigBlob, out uint pulCodeRVA, out uint pdwImplFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetMemberRefProps([In] uint mr, out uint ptk, [Out, MarshalAs(UnmanagedType.LPWStr)] string szMember, [In] uint cchMember, out uint pchMember, [Out] IntPtr ppvSigBlob, out uint pbSig);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumProperties([In] ref IntPtr phEnum, [In] uint td, out uint rProperties, [In] uint cMax, out uint pcProperties);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumEvents([In, Out] ref IntPtr phEnum, [In] uint td, out uint rEvents, [In] uint cMax, out uint pcEvents);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetEventProps([In] uint ev, out uint pClass, [Out, MarshalAs(UnmanagedType.LPWStr)] string szEvent, [In] uint cchEvent, out uint pchEvent, out uint pdwEventFlags, out uint ptkEventType, out uint pmdAddOn, out uint pmdRemoveOn, out uint pmdFire, out uint rmdOtherMethod, [In] uint cMax, out uint pcOtherMethod);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumMethodSemantics([In, Out] ref IntPtr phEnum, [In] uint mb, out uint rEventProp, [In] uint cMax, out uint pcEventProp);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetMethodSemantics([In] uint mb, [In] uint tkEventProp, out uint pdwSemanticsFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetClassLayout([In] uint td, out uint pdwPackSize, out COR_FIELD_OFFSET rFieldOffset, [In] uint cMax, out uint pcFieldOffset, out uint pulClassSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldMarshal([In] uint tk, [Out] IntPtr ppvNativeType, out uint pcbNativeType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetRVA([In] uint tk, out uint pulCodeRVA, out uint pdwImplFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetPermissionSetProps([In] uint pm, out uint pdwAction, out IntPtr ppvPermission, out uint pcbPermission);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetSigFromToken([In] uint mdSig, [Out] IntPtr ppvSig, out uint pcbSig);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetModuleRefProps([In] uint mur, [Out, MarshalAs(UnmanagedType.LPWStr)] string szName, [In] uint cchName, out uint pchName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumModuleRefs([In, Out] ref IntPtr phEnum, out uint rModuleRefs, [In] uint cMax, out uint pcModuleRefs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetTypeSpecFromToken([In] uint typespec, [Out] IntPtr ppvSig, out uint pcbSig);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetNameFromToken([In] uint tk, [Out] IntPtr pszUtf8NamePtr);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumUnresolvedMethods([In, Out] ref IntPtr phEnum, out uint rMethods, [In] uint cMax, out uint pcTokens);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetUserString([In] uint stk, [Out, MarshalAs(UnmanagedType.LPWStr)] string szString, [In] uint cchString, out uint pchString);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetPinvokeMap([In] uint tk, out uint pdwMappingFlags, [Out, MarshalAs(UnmanagedType.LPWStr)] string szImportName, [In] uint cchImportName, out uint pchImportName, out uint pmrImportDLL);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumSignatures([In, Out] ref IntPtr phEnum, out uint rSignatures, [In] uint cMax, out uint pcSignatures);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumTypeSpecs([In, Out] ref IntPtr phEnum, out uint rTypeSpecs, [In] uint cMax, out uint pcTypeSpecs);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumUserStrings([In, Out] ref IntPtr phEnum, out uint rStrings, [In] uint cMax, out uint pcStrings);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetParamForMethodIndex([In] uint md, [In] uint ulParamSeq, [In] ref uint ppd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EnumCustomAttributes([In, Out] ref IntPtr phEnum, [In] uint tk, [In] uint tkType, out uint rCustomAttributes, [In] uint cMax, out uint pcCustomAttributes);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCustomAttributeProps([In] uint cv, out uint ptkObj, out uint ptkType, out IntPtr ppBlob, out uint pcbSize);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void FindTypeRef([In] uint tkResolutionScope, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, out uint ptr);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetMemberProps([In] uint mb, out uint pClass, [In, MarshalAs(UnmanagedType.LPWStr)] string szMember, [In] uint cchMember, out uint pchMember, out uint pdwAttr, [Out] IntPtr ppvSigBlob, out uint pcbSigBlob, out uint pulCodeRVA, out uint pdwImplFlags, out uint pdwCPlusTypeFlag, out IntPtr ppValue, out uint pcchValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetFieldProps([In] uint mb, out uint pClass, [In] IntPtr szField, [In] uint cchField, out uint pchField, out uint pdwAttr, [Out] IntPtr ppvSigBlob, out uint pcbSigBlob, out uint pdwCPlusTypeFlag, out IntPtr ppValue, out uint pcchValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetPropertyProps([In] uint prop, out uint pClass, [Out, MarshalAs(UnmanagedType.LPWStr)] string szProperty, [In] uint cchProperty, out uint pchProperty, out uint pdwPropFlags, [Out] IntPtr ppvSig, out uint pbSig, out uint pdwCPlusTypeFlag, out IntPtr ppDefaultValue, out uint pcchDefaultValue, out uint pmdSetter, out uint pmdGetter, out uint rmdOtherMethod, [In] uint cMax, out uint pcOtherMethod);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetParamProps([In] uint tk, out uint pmd, out uint pulSequence, [Out] IntPtr szName, [In] uint cchName, out uint pchName, out uint pdwAttr, out uint pdwCPlusTypeFlag, [Out] IntPtr ppValue, out uint pcchValue);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetCustomAttributeByName([In] uint tkObj, [In, MarshalAs(UnmanagedType.LPWStr)] string szName, out IntPtr ppData, out uint pcbData);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        int IsValidToken(uint tk);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetNestedClassProps([In] uint tdNestedClass, out uint ptdEnclosingClass);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetNativeCallConvFromSig([In] IntPtr pvSig, [In] uint cbSig, out uint pCallConv);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void IsGlobal([In] uint pd, out int pbGlobal);
    }
}

