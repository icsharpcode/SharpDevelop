// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#pragma once

#include <sstream>
#include <stack>

#define ELEMENT_TYPE_END 0x00 //Marks end of a list
#define ELEMENT_TYPE_VOID 0x01
#define ELEMENT_TYPE_BOOLEAN 0x02
#define ELEMENT_TYPE_CHAR 0x03
#define ELEMENT_TYPE_I1 0x04
#define ELEMENT_TYPE_U1 0x05
#define ELEMENT_TYPE_I2 0x06
#define ELEMENT_TYPE_U2 0x07
#define ELEMENT_TYPE_I4 0x08
#define ELEMENT_TYPE_U4 0x09
#define ELEMENT_TYPE_I8 0x0a
#define ELEMENT_TYPE_U8 0x0b
#define ELEMENT_TYPE_R4 0x0c
#define ELEMENT_TYPE_R8 0x0d
#define ELEMENT_TYPE_STRING 0x0e
#define ELEMENT_TYPE_PTR 0x0f // Followed by type
#define ELEMENT_TYPE_BYREF 0x10 // Followed by type
#define ELEMENT_TYPE_VALUETYPE 0x11 // Followed by TypeDef or TypeRef token
#define ELEMENT_TYPE_CLASS 0x12 // Followed by TypeDef or TypeRef token
#define ELEMENT_TYPE_VAR 0x13 // Generic parameter in a generic type definition, represented as number
#define ELEMENT_TYPE_ARRAY 0x14 // type rank boundsCount bound1 … loCount lo1 …
#define ELEMENT_TYPE_GENERICINST 0x15 // Generic type instantiation. Followed by type type-arg-count type-1 ... type-n
#define ELEMENT_TYPE_TYPEDBYREF 0x16
#define ELEMENT_TYPE_I 0x18 // System.IntPtr
#define ELEMENT_TYPE_U 0x19 // System.UIntPtr
#define ELEMENT_TYPE_FNPTR 0x1b // Followed by full method signature
#define ELEMENT_TYPE_OBJECT 0x1c // System.Object
#define ELEMENT_TYPE_SZARRAY 0x1d // Single-dim array with 0 lower bound

#define ELEMENT_TYPE_MVAR 0x1e // Generic parameter in a generic method definition,represented as number
#define ELEMENT_TYPE_CMOD_REQD 0x1f // Required modifier : followed by a TypeDef or TypeRef token
#define ELEMENT_TYPE_CMOD_OPT 0x20 // Optional modifier : followed by a TypeDef or TypeRef token
#define ELEMENT_TYPE_INTERNAL 0x21 // Implemented within the CLI
#define ELEMENT_TYPE_MODIFIER 0x40 // Or’d with following element types
#define ELEMENT_TYPE_SENTINEL 0x41 // Sentinel for vararg method signature
#define ELEMENT_TYPE_PINNED 0x45 // Denotes a local variable that points at a pinned object

#define SIG_METHOD_DEFAULT  0x0 // default calling convention
#define SIG_METHOD_C  0x1       // C calling convention
#define SIG_METHOD_STDCALL  0x2 // Stdcall calling convention
#define SIG_METHOD_THISCALL 0x3 // thiscall  calling convention
#define SIG_METHOD_FASTCALL 0x4 // fastcall calling convention
#define SIG_METHOD_VARARG  0x5  // vararg calling convention
#define SIG_FIELD 0x6           // encodes a field
#define SIG_LOCAL_SIG 0x7       // used for the .locals directive
#define SIG_PROPERTY 0x8        // used to encode a property

#define SIG_GENERIC 0x10 // used to indicate that the method has one or more generic parameters.
#define SIG_HASTHIS 0x20  // used to encode the keyword instance in the calling convention
#define SIG_EXPLICITTHIS  0x40 // used to encode the keyword explicit in the calling convention

#define SIG_INDEX_TYPE_TYPEDEF 0    // ParseTypeDefOrRefEncoded returns this as the out index type for typedefs
#define SIG_INDEX_TYPE_TYPEREF 1    // ParseTypeDefOrRefEncoded returns this as the out index type for typerefs
#define SIG_INDEX_TYPE_TYPESPEC 2  // ParseTypeDefOrRefEncoded returns this as the out index type for typespecs

#define NAME_BUFFER_SIZE 1024

class SignatureReader {
	ICorProfilerInfo *profilerInfo;
	IMetaDataImport *metaData;
	mdToken methodDefiniton;
	
	byte *data;
	byte *startPosition;
	byte *endPosition;
	ULONG dataLength;

	std::wostringstream className;
	WCHAR szFunction[NAME_BUFFER_SIZE];
	WCHAR szClass[NAME_BUFFER_SIZE];

	ULONG cchFunction;
	ULONG cchClass;

	std::wostringstream output;
	std::stack<WCHAR> buffer;

	bool Parse();

	// Lexer
	bool ReadByte(byte *); // only access byte *data here
	bool ReadCompressedInt(int *);

	// Parser
	bool ReadMethodDefSig(byte);
	bool ReadType(byte);
	bool ReadShapedArray(byte);
	bool ReadSzArray(byte);
	bool ReadArrayType(byte);
	
	/// <summary>
	/// Creates a separated string using the specified separator. The string is encoded
	/// so that it can be split into the original parts even if the inputs contain the separator.
	/// </summary>
	void AppendEscapedString(const WCHAR *input);
	void AppendEscapedString(const std::wstring &input);
public:
	std::wstring Parse(FunctionID);	
	bool IsNetInternal(FunctionID);
	SignatureReader(ICorProfilerInfo *);
};

