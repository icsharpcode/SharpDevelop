// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

//
// ============== CLASS STRUCTURE DECLARATION ==================
//
.namespace DebuggerInterop.MetaData
{
  .class interface public abstract auto ansi import IMetaDataImport
  {
  } // end of class IMetaDataImport

  .class public sequential ansi sealed beforefieldinit COR_FIELD_OFFSET
         extends [mscorlib]System.ValueType
  {
  } // end of class COR_FIELD_OFFSET

} // end of namespace MetaData


// =============================================================


// =============== GLOBAL FIELDS AND METHODS ===================


// =============================================================


// =============== CLASS MEMBERS DECLARATION ===================
//   note that class flags, 'extends' and 'implements' clauses
//          are provided here for information only

.namespace DebuggerInterop.MetaData
{
  .class interface public abstract auto ansi import IMetaDataImport
  {
    .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 37 44 41 43 38 32 30 37 2D 44 33 41 45   // ..$7DAC8207-D3AE
                                                                                                    2D 34 43 37 35 2D 39 42 36 37 2D 39 32 38 30 31   // -4C75-9B67-92801
                                                                                                    41 34 39 37 44 34 34 00 00 )                      // A497D44..
    .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig newslot abstract virtual 
            instance void  CloseEnum(native int hEnum) runtime managed preservesig internalcall
    {
    } // end of method IMetaDataImport::CloseEnum

    .method public hidebysig newslot abstract virtual 
            instance void  CountEnum(native int hEnum,
                                     unsigned int32& pulCount) runtime managed internalcall
    {
    } // end of method IMetaDataImport::CountEnum

    .method public hidebysig newslot abstract virtual 
            instance void  ResetEnum(native int hEnum,
                                     unsigned int32 ulPos) runtime managed internalcall
    {
    } // end of method IMetaDataImport::ResetEnum

    .method public hidebysig newslot abstract virtual 
            instance void  EnumTypeDefs(native int& phEnum,
                                        unsigned int32& rTypeDefs,
                                        unsigned int32 cMax,
                                        unsigned int32& pcTypeDefs) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumTypeDefs

    .method public hidebysig newslot abstract virtual 
            instance void  EnumInterfaceImpls(native int& phEnum,
                                              unsigned int32 td,
                                              unsigned int32& rImpls,
                                              unsigned int32 cMax,
                                              unsigned int32& pcImpls) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumInterfaceImpls

    .method public hidebysig newslot abstract virtual 
            instance void  EnumTypeRefs(native int& phEnum,
                                        unsigned int32& rTypeRefs,
                                        unsigned int32 cMax,
                                        unsigned int32& pcTypeRefs) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumTypeRefs

    .method public hidebysig newslot abstract virtual 
            instance void  FindTypeDefByName([in] string  marshal( lpwstr) szTypeDef,
                                             [in] unsigned int32 tkEnclosingClass,
                                             [out] unsigned int32& ptd) runtime managed internalcall
    {
    } // end of method IMetaDataImport::FindTypeDefByName

    .method public hidebysig newslot abstract virtual 
            instance void  GetScopeProps([out] string  marshal( lpwstr) szName,
                                         [in] unsigned int32 cchName,
                                         [out] unsigned int32& pchName,
                                         [out] valuetype [mscorlib]System.Guid& pmvid) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetScopeProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetModuleFromScope([out] unsigned int32& pmd) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetModuleFromScope

    .method public hidebysig newslot abstract virtual 
            instance void  GetTypeDefProps([in] unsigned int32 td,
                                           [out] native int szTypeDef,
                                           [in] unsigned int32 cchTypeDef,
                                           [out] unsigned int32& pchTypeDef,
                                           [out] unsigned int32& pdwTypeDefFlags,
                                           [out] unsigned int32& ptkExtends) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetTypeDefProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetInterfaceImplProps([in] unsigned int32 iiImpl,
                                                 [out] unsigned int32& pClass,
                                                 [out] unsigned int32& ptkIface) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetInterfaceImplProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetTypeRefProps([in] unsigned int32 tr,
                                           [out] unsigned int32& ptkResolutionScope,
                                           [out] native int szName,
                                           [in] unsigned int32 cchName,
                                           [out] unsigned int32& pchName) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetTypeRefProps

    .method public hidebysig newslot abstract virtual 
            instance void  ResolveTypeRef(unsigned int32 tr,
                                          valuetype [mscorlib]System.Guid& riid,
                                          object&  marshal( iunknown) ppIScope,
                                          unsigned int32& ptd) runtime managed internalcall
    {
    } // end of method IMetaDataImport::ResolveTypeRef

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMembers([in] native int& phEnum,
                                       [in] unsigned int32 cl,
                                       [out] unsigned int32& rMembers,
                                       [in] unsigned int32 cMax,
                                       [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMembers

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMembersWithName([in][out] native int& phEnum,
                                               [in] unsigned int32 cl,
                                               [in] string  marshal( lpwstr) szName,
                                               [out] unsigned int32& rMembers,
                                               [in] unsigned int32 cMax,
                                               [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMembersWithName

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMethods([in] native int& phEnum,
                                       [in] unsigned int32 cl,
                                       [out] unsigned int32& rMethods,
                                       [in] unsigned int32 cMax,
                                       [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMethods

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMethodsWithName([in] native int& phEnum,
                                               [in] unsigned int32 cl,
                                               [in] string  marshal( lpwstr) szName,
                                               [out] unsigned int32& rMethods,
                                               [in] unsigned int32 cMax,
                                               [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMethodsWithName

    .method public hidebysig newslot abstract virtual 
            instance void  EnumFields([in][out] native int& phEnum,
                                      [in] unsigned int32 cl,
                                      [out] unsigned int32& rFields,
                                      [in] unsigned int32 cMax,
                                      [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumFields

    .method public hidebysig newslot abstract virtual 
            instance void  EnumFieldsWithName([in][out] native int& phEnum,
                                              [in] unsigned int32 cl,
                                              [in] string  marshal( lpwstr) szName,
                                              [out] unsigned int32& rFields,
                                              [in] unsigned int32 cMax,
                                              [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumFieldsWithName

    .method public hidebysig newslot abstract virtual 
            instance void  EnumParams([in] native int& phEnum,
                                      [in] unsigned int32 mb,
                                      [out] unsigned int32& rParams,
                                      [in] unsigned int32 cMax,
                                      [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumParams

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMemberRefs([in] native int& phEnum,
                                          [in] unsigned int32 tkParent,
                                          [out] unsigned int32& rMemberRefs,
                                          [in] unsigned int32 cMax,
                                          [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMemberRefs

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMethodImpls([in][out] native int& phEnum,
                                           [in] unsigned int32 td,
                                           [out] unsigned int32& rMethodBody,
                                           [out] unsigned int32& rMethodDecl,
                                           [in] unsigned int32 cMax,
                                           [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMethodImpls

    .method public hidebysig newslot abstract virtual 
            instance void  EnumPermissionSets([in][out] native int& phEnum,
                                              [in] unsigned int32 tk,
                                              [in] unsigned int32 dwActions,
                                              [out] unsigned int32& rPermission,
                                              [in] unsigned int32 cMax,
                                              [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumPermissionSets

    .method public hidebysig newslot abstract virtual 
            instance void  FindMember([in] unsigned int32 td,
                                      [in] string  marshal( lpwstr) szName,
                                      [in] unsigned int8& pvSigBlob,
                                      [in] unsigned int32 cbSigBlob,
                                      [out] unsigned int32& pmb) runtime managed internalcall
    {
    } // end of method IMetaDataImport::FindMember

    .method public hidebysig newslot abstract virtual 
            instance void  FindMethod([in] unsigned int32 td,
                                      [in] string  marshal( lpwstr) szName,
                                      [in] unsigned int8& pvSigBlob,
                                      [in] unsigned int32 cbSigBlob,
                                      [out] unsigned int32& pmb) runtime managed internalcall
    {
    } // end of method IMetaDataImport::FindMethod

    .method public hidebysig newslot abstract virtual 
            instance void  FindField([in] unsigned int32 td,
                                     [in] string  marshal( lpwstr) szName,
                                     [in] unsigned int8& pvSigBlob,
                                     [in] unsigned int32 cbSigBlob,
                                     [out] unsigned int32& pmb) runtime managed internalcall
    {
    } // end of method IMetaDataImport::FindField

    .method public hidebysig newslot abstract virtual 
            instance void  FindMemberRef([in] unsigned int32 td,
                                         [in] string  marshal( lpwstr) szName,
                                         [in] unsigned int8& pvSigBlob,
                                         [in] unsigned int32 cbSigBlob,
                                         [out] unsigned int32& pmr) runtime managed internalcall
    {
    } // end of method IMetaDataImport::FindMemberRef

    .method public hidebysig newslot abstract virtual 
            instance void  GetMethodProps([in] unsigned int32 mb,
                                          [out] unsigned int32& pClass,
                                          [out] native int szMethod,
                                          [in] unsigned int32 cchMethod,
                                          [out] unsigned int32& pchMethod,
                                          [out] unsigned int32& pdwAttr,
                                          [out] native int ppvSigBlob,
                                          [out] unsigned int32& pcbSigBlob,
                                          [out] unsigned int32& pulCodeRVA,
                                          [out] unsigned int32& pdwImplFlags) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetMethodProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetMemberRefProps([in] unsigned int32 mr,
                                             [out] unsigned int32& ptk,
                                             [out] string  marshal( lpwstr) szMember,
                                             [in] unsigned int32 cchMember,
                                             [out] unsigned int32& pchMember,
                                             [out] native int ppvSigBlob,
                                             [out] unsigned int32& pbSig) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetMemberRefProps

    .method public hidebysig newslot abstract virtual 
            instance void  EnumProperties([in] native int& phEnum,
                                          [in] unsigned int32 td,
                                          [out] unsigned int32& rProperties,
                                          [in] unsigned int32 cMax,
                                          [out] unsigned int32& pcProperties) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumProperties

    .method public hidebysig newslot abstract virtual 
            instance void  EnumEvents([in][out] native int& phEnum,
                                      [in] unsigned int32 td,
                                      [out] unsigned int32& rEvents,
                                      [in] unsigned int32 cMax,
                                      [out] unsigned int32& pcEvents) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumEvents

    .method public hidebysig newslot abstract virtual 
            instance void  GetEventProps([in] unsigned int32 ev,
                                         [out] unsigned int32& pClass,
                                         [out] string  marshal( lpwstr) szEvent,
                                         [in] unsigned int32 cchEvent,
                                         [out] unsigned int32& pchEvent,
                                         [out] unsigned int32& pdwEventFlags,
                                         [out] unsigned int32& ptkEventType,
                                         [out] unsigned int32& pmdAddOn,
                                         [out] unsigned int32& pmdRemoveOn,
                                         [out] unsigned int32& pmdFire,
                                         [out] unsigned int32& rmdOtherMethod,
                                         [in] unsigned int32 cMax,
                                         [out] unsigned int32& pcOtherMethod) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetEventProps

    .method public hidebysig newslot abstract virtual 
            instance void  EnumMethodSemantics([in][out] native int& phEnum,
                                               [in] unsigned int32 mb,
                                               [out] unsigned int32& rEventProp,
                                               [in] unsigned int32 cMax,
                                               [out] unsigned int32& pcEventProp) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumMethodSemantics

    .method public hidebysig newslot abstract virtual 
            instance void  GetMethodSemantics([in] unsigned int32 mb,
                                              [in] unsigned int32 tkEventProp,
                                              [out] unsigned int32& pdwSemanticsFlags) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetMethodSemantics

    .method public hidebysig newslot abstract virtual 
            instance void  GetClassLayout([in] unsigned int32 td,
                                          [out] unsigned int32& pdwPackSize,
                                          [out] valuetype DebuggerInterop.MetaData.COR_FIELD_OFFSET& rFieldOffset,
                                          [in] unsigned int32 cMax,
                                          [out] unsigned int32& pcFieldOffset,
                                          [out] unsigned int32& pulClassSize) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetClassLayout

    .method public hidebysig newslot abstract virtual 
            instance void  GetFieldMarshal([in] unsigned int32 tk,
                                           [out] native int ppvNativeType,
                                           [out] unsigned int32& pcbNativeType) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetFieldMarshal

    .method public hidebysig newslot abstract virtual 
            instance void  GetRVA([in] unsigned int32 tk,
                                  [out] unsigned int32& pulCodeRVA,
                                  [out] unsigned int32& pdwImplFlags) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetRVA

    .method public hidebysig newslot abstract virtual 
            instance void  GetPermissionSetProps([in] unsigned int32 pm,
                                                 [out] unsigned int32& pdwAction,
                                                 [out] native int& ppvPermission,
                                                 [out] unsigned int32& pcbPermission) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetPermissionSetProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetSigFromToken([in] unsigned int32 mdSig,
                                           [out] native int ppvSig,
                                           [out] unsigned int32& pcbSig) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetSigFromToken

    .method public hidebysig newslot abstract virtual 
            instance void  GetModuleRefProps([in] unsigned int32 mur,
                                             [out] string  marshal( lpwstr) szName,
                                             [in] unsigned int32 cchName,
                                             [out] unsigned int32& pchName) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetModuleRefProps

    .method public hidebysig newslot abstract virtual 
            instance void  EnumModuleRefs([in][out] native int& phEnum,
                                          [out] unsigned int32& rModuleRefs,
                                          [in] unsigned int32 cMax,
                                          [out] unsigned int32& pcModuleRefs) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumModuleRefs

    .method public hidebysig newslot abstract virtual 
            instance void  GetTypeSpecFromToken([in] unsigned int32 typespec,
                                                [out] native int ppvSig,
                                                [out] unsigned int32& pcbSig) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetTypeSpecFromToken

    .method public hidebysig newslot abstract virtual 
            instance void  GetNameFromToken([in] unsigned int32 tk,
                                            [out] native int pszUtf8NamePtr) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetNameFromToken

    .method public hidebysig newslot abstract virtual 
            instance void  EnumUnresolvedMethods([in][out] native int& phEnum,
                                                 [out] unsigned int32& rMethods,
                                                 [in] unsigned int32 cMax,
                                                 [out] unsigned int32& pcTokens) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumUnresolvedMethods

    .method public hidebysig newslot abstract virtual 
            instance void  GetUserString([in] unsigned int32 stk,
                                         [out] string  marshal( lpwstr) szString,
                                         [in] unsigned int32 cchString,
                                         [out] unsigned int32& pchString) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetUserString

    .method public hidebysig newslot abstract virtual 
            instance void  GetPinvokeMap([in] unsigned int32 tk,
                                         [out] unsigned int32& pdwMappingFlags,
                                         [out] string  marshal( lpwstr) szImportName,
                                         [in] unsigned int32 cchImportName,
                                         [out] unsigned int32& pchImportName,
                                         [out] unsigned int32& pmrImportDLL) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetPinvokeMap

    .method public hidebysig newslot abstract virtual 
            instance void  EnumSignatures([in][out] native int& phEnum,
                                          [out] unsigned int32& rSignatures,
                                          [in] unsigned int32 cMax,
                                          [out] unsigned int32& pcSignatures) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumSignatures

    .method public hidebysig newslot abstract virtual 
            instance void  EnumTypeSpecs([in][out] native int& phEnum,
                                         [out] unsigned int32& rTypeSpecs,
                                         [in] unsigned int32 cMax,
                                         [out] unsigned int32& pcTypeSpecs) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumTypeSpecs

    .method public hidebysig newslot abstract virtual 
            instance void  EnumUserStrings([in][out] native int& phEnum,
                                           [out] unsigned int32& rStrings,
                                           [in] unsigned int32 cMax,
                                           [out] unsigned int32& pcStrings) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumUserStrings

    .method public hidebysig newslot abstract virtual 
            instance void  GetParamForMethodIndex([in] unsigned int32 md,
                                                  [in] unsigned int32 ulParamSeq,
                                                  [in] unsigned int32& ppd) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetParamForMethodIndex

    .method public hidebysig newslot abstract virtual 
            instance void  EnumCustomAttributes([in][out] native int& phEnum,
                                                [in] unsigned int32 tk,
                                                [in] unsigned int32 tkType,
                                                [out] unsigned int32& rCustomAttributes,
                                                [in] unsigned int32 cMax,
                                                [out] unsigned int32& pcCustomAttributes) runtime managed internalcall
    {
    } // end of method IMetaDataImport::EnumCustomAttributes

    .method public hidebysig newslot abstract virtual 
            instance void  GetCustomAttributeProps([in] unsigned int32 cv,
                                                   [out] unsigned int32& ptkObj,
                                                   [out] unsigned int32& ptkType,
                                                   [out] native int& ppBlob,
                                                   [out] unsigned int32& pcbSize) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetCustomAttributeProps

    .method public hidebysig newslot abstract virtual 
            instance void  FindTypeRef([in] unsigned int32 tkResolutionScope,
                                       [in] string  marshal( lpwstr) szName,
                                       [out] unsigned int32& ptr) runtime managed internalcall
    {
    } // end of method IMetaDataImport::FindTypeRef

    .method public hidebysig newslot abstract virtual 
            instance void  GetMemberProps([in] unsigned int32 mb,
                                          [out] unsigned int32& pClass,
                                          [in] string  marshal( lpwstr) szMember,
                                          [in] unsigned int32 cchMember,
                                          [out] unsigned int32& pchMember,
                                          [out] unsigned int32& pdwAttr,
                                          [out] native int ppvSigBlob,
                                          [out] unsigned int32& pcbSigBlob,
                                          [out] unsigned int32& pulCodeRVA,
                                          [out] unsigned int32& pdwImplFlags,
                                          [out] unsigned int32& pdwCPlusTypeFlag,
                                          [out] native int& ppValue,
                                          [out] unsigned int32& pcchValue) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetMemberProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetFieldProps([in] unsigned int32 mb,
                                         [out] unsigned int32& pClass,
                                         [in] native int szField,
                                         [in] unsigned int32 cchField,
                                         [out] unsigned int32& pchField,
                                         [out] unsigned int32& pdwAttr,
                                         [out] native int ppvSigBlob,
                                         [out] unsigned int32& pcbSigBlob,
                                         [out] unsigned int32& pdwCPlusTypeFlag,
                                         [out] native int& ppValue,
                                         [out] unsigned int32& pcchValue) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetFieldProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetPropertyProps([in] unsigned int32 prop,
                                            [out] unsigned int32& pClass,
                                            [out] string  marshal( lpwstr) szProperty,
                                            [in] unsigned int32 cchProperty,
                                            [out] unsigned int32& pchProperty,
                                            [out] unsigned int32& pdwPropFlags,
                                            [out] native int ppvSig,
                                            [out] unsigned int32& pbSig,
                                            [out] unsigned int32& pdwCPlusTypeFlag,
                                            [out] native int& ppDefaultValue,
                                            [out] unsigned int32& pcchDefaultValue,
                                            [out] unsigned int32& pmdSetter,
                                            [out] unsigned int32& pmdGetter,
                                            [out] unsigned int32& rmdOtherMethod,
                                            [in] unsigned int32 cMax,
                                            [out] unsigned int32& pcOtherMethod) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetPropertyProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetParamProps([in] unsigned int32 tk,
                                         [out] unsigned int32& pmd,
                                         [out] unsigned int32& pulSequence,
                                         [out] native int szName,
                                         [in] unsigned int32 cchName,
                                         [out] unsigned int32& pchName,
                                         [out] unsigned int32& pdwAttr,
                                         [out] unsigned int32& pdwCPlusTypeFlag,
                                         [out] native int ppValue,
                                         [out] unsigned int32& pcchValue) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetParamProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetCustomAttributeByName([in] unsigned int32 tkObj,
                                                    [in] string  marshal( lpwstr) szName,
                                                    [out] native int& ppData,
                                                    [out] unsigned int32& pcbData) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetCustomAttributeByName

    .method public hidebysig newslot abstract virtual 
            instance int32  IsValidToken(unsigned int32 tk) runtime managed preservesig internalcall
    {
    } // end of method IMetaDataImport::IsValidToken

    .method public hidebysig newslot abstract virtual 
            instance void  GetNestedClassProps([in] unsigned int32 tdNestedClass,
                                               [out] unsigned int32& ptdEnclosingClass) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetNestedClassProps

    .method public hidebysig newslot abstract virtual 
            instance void  GetNativeCallConvFromSig([in] native int pvSig,
                                                    [in] unsigned int32 cbSig,
                                                    [out] unsigned int32& pCallConv) runtime managed internalcall
    {
    } // end of method IMetaDataImport::GetNativeCallConvFromSig

    .method public hidebysig newslot abstract virtual 
            instance void  IsGlobal([in] unsigned int32 pd,
                                    [out] int32& pbGlobal) runtime managed internalcall
    {
    } // end of method IMetaDataImport::IsGlobal

  } // end of class IMetaDataImport

  .class public sequential ansi sealed beforefieldinit COR_FIELD_OFFSET
         extends [mscorlib]System.ValueType
  {
    .pack 4
    .size 0
    .field public unsigned int32 ridOfField
    .field public unsigned int32 ulOffset
  } // end of class COR_FIELD_OFFSET


// =============================================================

} // end of namespace MetaData

//*********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file DebuggerInterop.MetaData.res
