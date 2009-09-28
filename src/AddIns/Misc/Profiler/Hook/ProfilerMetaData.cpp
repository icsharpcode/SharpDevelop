// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

#include "main.h"
#include "Profiler.h"
#include <cassert>
#include "wchar.h"
#include "ProfilerMetaData.h"

SignatureReader::SignatureReader(ICorProfilerInfo *profilerInfo)
{
	this->profilerInfo = profilerInfo;
}

std::wstring SignatureReader::Parse(FunctionID functionID)
{
	mdToken funcToken;

	HRESULT hr = S_OK;
	PCCOR_SIGNATURE signature = 0;
	ULONG sigLength = 0;

	this->output.str(L"");
	this->className.str(L"");

	// get the token for the function which we will use to get its name
	hr = profilerInfo->GetTokenAndMetaDataFromFunction(functionID, IID_IMetaDataImport, (LPUNKNOWN *) &metaData, &funcToken);
	if(SUCCEEDED(hr))
	{
		mdTypeDef classTypeDef;
		ULONG cchFunction;
		ULONG cchClass;

		// retrieve the function properties based on the token
		hr = metaData->GetMethodProps(funcToken, &classTypeDef, szFunction, NAME_BUFFER_SIZE, &cchFunction, 0, &signature, &sigLength, 0, 0);
		if (SUCCEEDED(hr))
		{
			DWORD classFlags;
			std::stack<std::wstring> classStack;
			for (;;)
			{
				// get the class name
				hr = metaData->GetTypeDefProps(classTypeDef, szClass, NAME_BUFFER_SIZE, &cchClass, &classFlags, 0);
				if (!SUCCEEDED(hr))
					break;

				classStack.push(std::wstring(szClass));

				if (!IsTdNested(classFlags))
					break;

				hr = metaData->GetNestedClassProps(classTypeDef, &classTypeDef);

				if (!SUCCEEDED(hr))
					break;
			}

			while (!classStack.empty()) {
				this->className << classStack.top();
				classStack.pop();
				if (!classStack.empty())
					this->className << L"+";
			}

			if (SUCCEEDED(hr)) {
				this->data = (unsigned char *)signature;
				this->dataLength = sigLength;
				this->startPosition = this->data;
				this->endPosition = this->data + sigLength;
				this->methodDefiniton = funcToken;

				this->Parse();
			}
		}
		// release our reference to the metadata
		metaData->Release();
	}

	return this->output.str();
}

bool SignatureReader::Parse()
{
	byte elementType;
	
	if (!this->ReadByte(&elementType))
		return false;

	switch (elementType & 0xF)
	{
		case SIG_METHOD_DEFAULT:  // default calling convention
		case SIG_METHOD_C:        // C calling convention
		case SIG_METHOD_STDCALL:  // Stdcall calling convention
		case SIG_METHOD_THISCALL: // thiscall  calling convention
		case SIG_METHOD_FASTCALL: // fastcall calling convention
		case SIG_METHOD_VARARG:   // vararg calling convention
			return ReadMethodDefSig(elementType);
			break;
	}

	return true;
}

bool SignatureReader::ReadMethodDefSig(byte head)
{
	// MethodDefSig ::= [[HASTHIS] [EXPLICITTHIS]] (DEFAULT|VARARG|GENERIC GenParamCount) ParamCount RetType Param*

	int genericParamCount = 0;
	int paramCount = 0;

	if (head & SIG_GENERIC) {
		if (!this->ReadCompressedInt(&genericParamCount))
			return false;
	}

	if (!this->ReadCompressedInt(&paramCount))
		return false;
	
	// Read Return Type
	// RetType ::= CustomMod* ( VOID | TYPEDBYREF | [BYREF] Type )

	byte type;

	if (!this->ReadByte(&type))
		return false;
	
	for (;;) {
		if (type != ELEMENT_TYPE_CMOD_OPT && type != ELEMENT_TYPE_CMOD_REQD)
			break;

		int tmp;

		if (!this->ReadCompressedInt(&tmp)) return false;

		if (!this->ReadByte(&type)) return false;
	}

	switch (type) {
		case ELEMENT_TYPE_VOID:
			this->output << L"void";
			break;
		case ELEMENT_TYPE_TYPEDBYREF:
			// ignore
			break;
		case ELEMENT_TYPE_BYREF:
			// ignore
			if (!this->ReadByte(&type))
				return false;
		default:
			ReadType(type);
	}
	
	// Append Name
	
	this->output << L" ";
	this->output << '"';
	std::wstring name = className.str();
	AppendEscapedString(name);
	this->output << L".";
	AppendEscapedString(szFunction);
	this->output << '"';
	this->output << L" ";

	// Read Parameters
	// Param ::= CustomMod* ( TYPEDBYREF | [BYREF] Type )

	for (int i = 0; i < paramCount; i++) {
		if (!this->ReadByte(&type))
			return false;
		
		while (type == ELEMENT_TYPE_CMOD_OPT || type == ELEMENT_TYPE_CMOD_REQD) {
			int tmp2;

			if (!this->ReadCompressedInt(&tmp2))
				return false;

			if (!this->ReadByte(&type))
				return false;
		}
		
		mdParamDef paramDef;

		WCHAR paramName[256];
		ULONG pSequence, pchName, pcchValue;
		DWORD pdwAttr, pdwCPlusTypeFlag;
		UVCP_CONSTANT ppValue;

		if (!FAILED(this->metaData->GetParamForMethodIndex(this->methodDefiniton, i + 1, &paramDef))) {
			if (!FAILED(this->metaData->GetParamProps(paramDef, &this->methodDefiniton, &pSequence, paramName, 256, &pchName, &pdwAttr, &pdwCPlusTypeFlag, &ppValue, &pcchValue))) {
				this->output << '"';
				switch (type) {
					case ELEMENT_TYPE_TYPEDBYREF:
						// ignore
						break;
					case ELEMENT_TYPE_BYREF:
						if (!this->ReadByte(&type))
							return false;
						if ((pdwAttr & 0x2) !=  0) // if Bit 1 == 1 -> out
							this->output << L"out";
						else
							this->output << L"ref";
						this->output << L" ";
					default:
						ReadType(type);
				}
				
				this->output << L" ";
				AppendEscapedString(paramName);
				this->output << '"';
			}
		}

		if (i < paramCount - 1)
			this->output << L" ";
	}

	return true;
}

bool SignatureReader::ReadType(byte type)
{
    /*
    Type ::= ( BOOLEAN | CHAR | I1 | U1 | U2 | U2 | I4 | U4 | I8 | U8 | R4 | R8 | I | U |
                    | VALUETYPE TypeDefOrRefEncoded
                    | CLASS TypeDefOrRefEncoded
                    | STRING 
                    | OBJECT
                    | PTR CustomMod* VOID
                    | PTR CustomMod* Type
                    | FNPTR MethodDefSig
                    | FNPTR MethodRefSig
                    | ARRAY Type ArrayShape
                    | SZARRAY CustomMod* Type
                    | GENERICINST (CLASS | VALUETYPE) TypeDefOrRefEncoded GenArgCount Type *
                    | VAR Number
                    | MVAR Number
    */

	switch (type) {
		case ELEMENT_TYPE_VOID:
			this->output << L"void";
			break;
		case ELEMENT_TYPE_BOOLEAN:
			this->output << L"bool";
			break;
		case ELEMENT_TYPE_CHAR:
			this->output << L"char"; 
			break;
		case ELEMENT_TYPE_I:
			this->output << L"IntPtr";
			break;
		case ELEMENT_TYPE_U:
			this->output << L"UIntPtr";
			break;
		case ELEMENT_TYPE_I1:
			this->output << L"sbyte"; 
			break;
		case ELEMENT_TYPE_U1:
			this->output << L"byte"; 
			break;
		case ELEMENT_TYPE_I2:
			this->output << L"short"; 
			break;
		case ELEMENT_TYPE_U2:
			this->output << L"ushort"; 
			break;
		case ELEMENT_TYPE_I4:
			this->output << L"int"; 
			break;
		case ELEMENT_TYPE_U4:
			this->output << L"uint"; 
			break;
		case ELEMENT_TYPE_I8:
			this->output << L"long"; 
			break;
		case ELEMENT_TYPE_U8:
			this->output << L"ulong"; 
			break;
		case ELEMENT_TYPE_R4:
			this->output << L"float"; 
			break;
		case ELEMENT_TYPE_R8:
			this->output << L"double"; 
			break;
		case ELEMENT_TYPE_STRING:
			this->output << L"string"; 
			break;
		case ELEMENT_TYPE_OBJECT:
			this->output << L"object";
			break;
		case ELEMENT_TYPE_CLASS:
		case ELEMENT_TYPE_VALUETYPE:
			mdToken token;
			WCHAR zName[1024];
			ULONG length;
			CorSigUncompressToken(this->data, &token);
			HRESULT	hr;
			hr = this->metaData->GetTypeRefProps(token, nullptr, zName, 1024, &length);
			hr = this->metaData->GetTypeDefProps(token, zName, 1024, &length, nullptr, nullptr);
			if (SUCCEEDED(hr) && length > 0)
				AppendEscapedString(zName);
			else {
				hr = this->metaData->GetTypeRefProps(token, nullptr, zName, 1024, &length);
				hr = this->metaData->GetTypeDefProps(token, zName, 1024, &length, nullptr, nullptr);
				if (SUCCEEDED(hr) && length > 0)
					AppendEscapedString(zName);
				else
					this->output << L"?";
			}
			int tmp2;
			if (!this->ReadCompressedInt(&tmp2))
				return false;
			break;
		case ELEMENT_TYPE_ARRAY:
		case ELEMENT_TYPE_SZARRAY:
			this->ReadArrayType(type);
			break;
		case ELEMENT_TYPE_CMOD_OPT:
		case ELEMENT_TYPE_CMOD_REQD:
			this->output << L"cmod";
			break;
		case ELEMENT_TYPE_FNPTR:
			this->output << L"fnptr";
			break;
		case ELEMENT_TYPE_PTR:
			if (!this->ReadByte(&type))
				return false;
			while (type == ELEMENT_TYPE_CMOD_OPT || type == ELEMENT_TYPE_CMOD_REQD) {
				int tmp2;

				if (!this->ReadCompressedInt(&tmp2))
					return false;

				if (!this->ReadByte(&type))
					return false;
			}

			ReadType(type);
			this->output << L"*";
			break;
		case ELEMENT_TYPE_GENERICINST:
			if (!this->ReadByte(&type))
				return false;
			CorSigUncompressToken(this->data, &token);
			// don't ask me why this is necessary, but it doesn't work without either line of these.
			hr = this->metaData->GetTypeRefProps(token, nullptr, zName, 1024, &length);
			hr = this->metaData->GetTypeDefProps(token, zName, 1024, &length, nullptr, nullptr);
			if (SUCCEEDED(hr) && length > 0)
				AppendEscapedString(zName);
			else
				this->output << L"?";
			if (!this->ReadCompressedInt(&tmp2))
				return false;
			this->output << L"<";
			int genArgCount;
			if (!this->ReadCompressedInt(&genArgCount))
				return false;
			for (int i = 0; i < genArgCount; i++) {
				if (!this->ReadByte(&type))
					return false;
				this->ReadType(type);
				if ((i + 1) < genArgCount)
					this->output << L",";
			}
			this->output << L">";
			break;
		case ELEMENT_TYPE_MVAR: // print "!!number"
			this->output << L"!";
		case ELEMENT_TYPE_VAR: // print "!number"
			this->output << L"!"; 
			int number;
			if (!this->ReadCompressedInt(&number))
				return false;
			this->output << number;
			break;
		default:
			this->output << L"?";
			break;
	}

	return true;
}

bool SignatureReader::ReadSzArray(byte type)
{
	// type ::= SZARRAY CustomMod* Type

	if (!this->ReadByte(&type))
		return false;
	while (type == ELEMENT_TYPE_CMOD_OPT || type == ELEMENT_TYPE_CMOD_REQD) {
		int tmp2;

		if (!this->ReadCompressedInt(&tmp2))
			return false;

		if (!this->ReadByte(&type))
			return false;
	}

	switch (type) {
		case ELEMENT_TYPE_ARRAY:
			this->ReadShapedArray(type);
			break;
		case ELEMENT_TYPE_SZARRAY:
			this->ReadSzArray(type);
			break;
		default:
			this->ReadType(type);
			break;
	}

	this->buffer.push(L']');
	this->buffer.push(L'[');

	return true;
}

bool SignatureReader::ReadShapedArray(byte type)
{
	// type ::= ARRAY Type ArrayShape
	if (type == ELEMENT_TYPE_ARRAY) {
		if (!this->ReadByte(&type))
			return false;
		switch (type) {
			case ELEMENT_TYPE_ARRAY:
				this->ReadShapedArray(type);
				break;
			case ELEMENT_TYPE_SZARRAY:
				this->ReadSzArray(type);
				break;
			default:
				this->ReadType(type);
				break;
		}

		// Read array shape
		// ArrayShape ::= Rank NumSizes Size* NumLoBounds LoBound*
		int rank, numSizes, numLoBounds, tmpSize, tmpLoBound;
		if (!ReadCompressedInt(&rank))
			return false;
		if (!ReadCompressedInt(&numSizes))
			return false;
		for (int i = 0; i < numSizes; i++) {
			if (!ReadCompressedInt(&tmpSize))
				return false;
		}
		if (!ReadCompressedInt(&numLoBounds))
			return false;
		for (int i = 0; i < numLoBounds; i++) {
			if (!ReadCompressedInt(&tmpLoBound))
				return false;
		}
		this->buffer.push(L']');
		for (int i = 1; i < rank; i++)
			this->buffer.push(L',');
		this->buffer.push(L'[');

		return true;
	}

	return false;
}

bool SignatureReader::ReadArrayType(byte type)
{
	if (type == ELEMENT_TYPE_ARRAY)
		this->ReadShapedArray(type);
	
	if (type == ELEMENT_TYPE_SZARRAY)
		this->ReadSzArray(type);

	while (!buffer.empty()) {
		this->output << buffer.top();
		buffer.pop();
	}

	return true;
}

bool SignatureReader::ReadByte(byte *out)
{
	if (this->data < this->endPosition) {
		*out = *this->data;
		this->data++;
		return true;
	}

	return false;
}

bool SignatureReader::ReadCompressedInt(int *out)
{
    byte byte1 = 0, byte2 = 0, byte3 = 0, byte4 = 0;
        
    if (!ReadByte(&byte1))
        return false;

    if (byte1 == 0xff) // = nullptr -> error
		return false;

    if ((byte1 & 0x80) == 0) {
        *out = (int)byte1;
        return true;
    }

    if (!ReadByte(&byte2))
        return false;

    if ((byte1 & 0x40) == 0) {
        *out = (((byte1 & 0x3f) << 8) | byte2);
        return true;
    }

    if ((byte1 & 0x20) != 0)        
        return false;

    if (!ReadByte(&byte3))
        return false;
    
    if (!ReadByte(&byte4))
        return false;

    *out = ((byte1 & 0x1f) << 24) | (byte2 << 16) | (byte3 << 8) | byte4;
    return true;
}

// public key token: b77a5c561934e089
const byte mscorlibkey[]      = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

// public key token: b03f5f7f11d50a3a
const byte systemdrawingkey[] = {
	0x00, 0x24, 0x00, 0x00, 0x04, 0x80, 0x00, 0x00, 0x94, 0x00, 0x00, 0x00, 0x06, 0x02, 0x00, 0x00, 0x00,
	0x24, 0x00, 0x00, 0x52, 0x53, 0x41, 0x31, 0x00, 0x04, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x07, 0xD1,
	0xFA, 0x57, 0xC4, 0xAE, 0xD9, 0xF0, 0xA3, 0x2E, 0x84, 0xAA, 0x0F, 0xAE, 0xFD, 0x0D, 0xE9, 0xE8, 0xFD,
	0x6A, 0xEC, 0x8F, 0x87, 0xFB, 0x03, 0x76, 0x6C, 0x83, 0x4C, 0x99, 0x92, 0x1E, 0xB2, 0x3B, 0xE7, 0x9A,
	0xD9, 0xD5, 0xDC, 0xC1, 0xDD, 0x9A, 0xD2, 0x36, 0x13, 0x21, 0x02, 0x90, 0x0B, 0x72, 0x3C, 0xF9, 0x80,
	0x95, 0x7F, 0xC4, 0xE1, 0x77, 0x10, 0x8F, 0xC6, 0x07, 0x77, 0x4F, 0x29, 0xE8, 0x32, 0x0E, 0x92, 0xEA,
	0x05, 0xEC, 0xE4, 0xE8, 0x21, 0xC0, 0xA5, 0xEF, 0xE8, 0xF1, 0x64, 0x5C, 0x4C, 0x0C, 0x93, 0xC1, 0xAB,
	0x99, 0x28, 0x5D, 0x62, 0x2C, 0xAA, 0x65, 0x2C, 0x1D, 0xFA, 0xD6, 0x3D, 0x74, 0x5D, 0x6F, 0x2D, 0xE5,
	0xF1, 0x7E, 0x5E, 0xAF, 0x0F, 0xC4, 0x96, 0x3D, 0x26, 0x1C, 0x8A, 0x12, 0x43, 0x65, 0x18, 0x20, 0x6D,
	0xC0, 0x93, 0x34, 0x4D, 0x5A, 0xD2, 0x93
};

// public key token: 31bf3856ad364e35
const byte wpfassemblieskey[] = {
	0x00, 0x24, 0x00, 0x00, 0x04, 0x80, 0x00, 0x00, 0x94, 0x00, 0x00, 0x00, 0x06, 0x02, 0x00, 0x00, 0x00,
	0x24, 0x00, 0x00, 0x52, 0x53, 0x41, 0x31, 0x00, 0x04, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0xB5, 0xFC,
	0x90, 0xE7, 0x02, 0x7F, 0x67, 0x87, 0x1E, 0x77, 0x3A, 0x8F, 0xDE, 0x89, 0x38, 0xC8, 0x1D, 0xD4, 0x02,
	0xBA, 0x65, 0xB9, 0x20, 0x1D, 0x60, 0x59, 0x3E, 0x96, 0xC4, 0x92, 0x65, 0x1E, 0x88, 0x9C, 0xC1, 0x3F,
	0x14, 0x15, 0xEB, 0xB5, 0x3F, 0xAC, 0x11, 0x31, 0xAE, 0x0B, 0xD3, 0x33, 0xC5, 0xEE, 0x60, 0x21, 0x67,
	0x2D, 0x97, 0x18, 0xEA, 0x31, 0xA8, 0xAE, 0xBD, 0x0D, 0xA0, 0x07, 0x2F, 0x25, 0xD8, 0x7D, 0xBA, 0x6F,
	0xC9, 0x0F, 0xFD, 0x59, 0x8E, 0xD4, 0xDA, 0x35, 0xE4, 0x4C, 0x39, 0x8C, 0x45, 0x43, 0x07, 0xE8, 0xE3,
	0x3B, 0x84, 0x26, 0x14, 0x3D, 0xAE, 0xC9, 0xF5, 0x96, 0x83, 0x6F, 0x97, 0xC8, 0xF7, 0x47, 0x50, 0xE5,
	0x97, 0x5C, 0x64, 0xE2, 0x18, 0x9F, 0x45, 0xDE, 0xF4, 0x6B, 0x2A, 0x2B, 0x12, 0x47, 0xAD, 0xC3, 0x65,
	0x2B, 0xF5, 0xC3, 0x08, 0x05, 0x5D, 0xA9
};


bool SignatureReader::IsNetInternal(FunctionID fid)
{
	mdToken funcToken;
	HRESULT hr = S_OK;
	IMetaDataAssemblyImport *asmMetaData;
	const void *publicKey;
	ULONG pKLength;

	hr = profilerInfo->GetTokenAndMetaDataFromFunction(fid, IID_IMetaDataAssemblyImport, (LPUNKNOWN *) &asmMetaData, &funcToken);
	if (SUCCEEDED(hr)) {
		mdAssembly assembly;
		hr = asmMetaData->GetAssemblyFromScope(&assembly);
		if (SUCCEEDED(hr)) {
			WCHAR assemblyName[NAME_BUFFER_SIZE];
			ULONG assemblyNameLength;

			hr = asmMetaData->GetAssemblyProps(assembly, &publicKey, &pKLength, nullptr, assemblyName, NAME_BUFFER_SIZE, &assemblyNameLength, nullptr, nullptr);
			const byte *b = (const byte *)publicKey;
			
			if (pKLength == sizeof(mscorlibkey) && memcmp(mscorlibkey, b, sizeof(mscorlibkey)) == 0)
				return true;
			if (pKLength == sizeof(systemdrawingkey) && memcmp(systemdrawingkey, b, sizeof(systemdrawingkey)) == 0)
				return true;
			if (pKLength == sizeof(wpfassemblieskey) && memcmp(wpfassemblieskey, b, sizeof(wpfassemblieskey)) == 0)
				return true;
		}
	}
	
	return false;
}

void SignatureReader::AppendEscapedString(const WCHAR *input)
{	
	for (const WCHAR *ptr = input; *ptr != 0; ptr++) {
		WCHAR c = *ptr;
		if (c == '"')
			this->output << "\"\"";
		else
			this->output << c;
	}
}

void SignatureReader::AppendEscapedString(const std::wstring &input_string)
{
	AppendEscapedString(input_string.c_str());
}