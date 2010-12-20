// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#include "main.h"
#include "global.h"
#include "Profiler.h"
#include "windows.h"
#include "CircularBuffer.h"
#include "allocator.h"
#include "FunctionInfo.h"
#include "LightweightList.h"
#include "Callback.h"
#include <cassert>

const ULONG FAT_HEADER_SIZE = 0xC;     /* 12 bytes = WORD + WORD + DWORD + DWORD */
const ULONG TINY_HEADER_SIZE = 0x1;    /* 1 byte */
const ULONG MAX_CODE_SIZE_TINY = 0x40; /* 64 bytes */
const WORD  DEFAULT_MAX_STACK = 0x8;   /* default stack depth in slots */

void CProfiler::Rewrite(FunctionID functionID, int type, int nameId) {
	ModuleID moduleID;
	mdToken token;
	HRESULT hr = S_OK;
	
	hr = pICorProfilerInfo2->GetFunctionInfo(functionID, 0, &moduleID, &token);
	
	if (!SUCCEEDED(hr))
		return;
	
	LPCBYTE header;
	ULONG length;

	hr = pICorProfilerInfo2->GetILFunctionBody(moduleID, token, &header, &length);
	
	if (!SUCCEEDED(hr))
		return;
		
	IMethodMalloc *allocator = nullptr;
	
	hr = pICorProfilerInfo2->GetILFunctionBodyAllocator(moduleID, &allocator);
	
	if (!SUCCEEDED(hr) || allocator == nullptr)
		return;
	
	methodMapCriticalSection.Enter();
	TMethodTokenMap::iterator it = this->methodMap.find(moduleID);
	
	mdMethodDef deactivateCall = nullptr;
	mdMethodDef activateCall = nullptr;
	mdMethodDef loggerCall = nullptr;
	
	if (it != this->methodMap.end()) {
		deactivateCall = it->second.deactivator;
		activateCall = it->second.activator;
		loggerCall = it->second.logger;
	}
	
	methodMapCriticalSection.Leave();
	
	if (deactivateCall == nullptr || activateCall == nullptr || loggerCall == nullptr)
		return;
		
	IMetaDataImport *metaData = nullptr;
	hr = pICorProfilerInfo->GetModuleMetaData(moduleID, ofRead, IID_IMetaDataImport, (LPUNKNOWN *) &metaData);

	if (!SUCCEEDED(hr) || metaData == nullptr)
		return;

	byte *codeBuf = 0;
	
	codeBuf = (byte *)allocator->Alloc(length + 128);
    allocator->Release(); 
	
	if (codeBuf == nullptr)
		return;
	
	int newLength = 0;
	
	unsigned char newCode[64] = { 0 };
	
	int injectionLen = SetInjectionCode(metaData, newCode, &newLength, activateCall, loggerCall, deactivateCall, type, nameId);
	
	assert(injectionLen == newLength);
	
	DebugWriteLine(L"rewrite size %d", injectionLen);

	if (((COR_ILMETHOD_TINY *)header)->IsTiny()) {
	    COR_ILMETHOD_TINY *method = (COR_ILMETHOD_TINY *)header;

		if (method->GetCodeSize() + injectionLen < MAX_CODE_SIZE_TINY) {
			DebugWriteLine(L"rewrite normal");
			// Copy the header elements.
			memcpy(&codeBuf[0], method, TINY_HEADER_SIZE);
			newLength = TINY_HEADER_SIZE;
			
			// copy new code
			memcpy(&codeBuf[newLength],
					&newCode[0],
					injectionLen);
					
			newLength += injectionLen;
			
			// copy old code
			memcpy(&codeBuf[newLength],
					&header[TINY_HEADER_SIZE],
					length - TINY_HEADER_SIZE);
			
			newLength += (length - TINY_HEADER_SIZE);
			
			// update code size in header
			// LLLLLLUU
			// 2 upper bits are the tiny header 6 lower bits are length
			codeBuf[0] = (byte)((newLength - 1) << 2 | 0x2); 
        } else {
        	DebugWriteLine(L"rewrite convert");
			ConvertToFat((byte *)codeBuf, &newLength);
			
			// copy new code
			memcpy(&codeBuf[newLength],
					&newCode[0],
					injectionLen);
			
			newLength += injectionLen;
						
			// copy old code
			memcpy(&codeBuf[newLength], &header[TINY_HEADER_SIZE], length - TINY_HEADER_SIZE);
			newLength += (length - TINY_HEADER_SIZE);
			
			COR_ILMETHOD_FAT *target = (COR_ILMETHOD_FAT *)codeBuf;
			target->CodeSize = newLength - FAT_HEADER_SIZE;
        }
	} else if (((COR_ILMETHOD_FAT *)header)->IsFat()) {
		DebugWriteLine(L"rewrite fat");
		COR_ILMETHOD_FAT *method = (COR_ILMETHOD_FAT *)header;
		COR_ILMETHOD_FAT *target = (COR_ILMETHOD_FAT *)codeBuf;
		
		// Copy the header elements.
		newLength = FAT_HEADER_SIZE;
		memcpy(&codeBuf[0], method, newLength);

		// copy new code
		memcpy(&codeBuf[newLength],
				&newCode[0],
				injectionLen);
		
		newLength += injectionLen;
		
		// Copy the remainder of the method body.
		memcpy(&codeBuf[newLength], &header[FAT_HEADER_SIZE], length - FAT_HEADER_SIZE);
		newLength += (length - FAT_HEADER_SIZE);
		
		// Reset the size of the code in the header itself.
		target->CodeSize = newLength - FAT_HEADER_SIZE;
		
		// Increase MaxStack to allow our injection code to run.
		target->MaxStack = (method->MaxStack > 8) ? method->MaxStack : 8;
		
		// Fix SEH sections
		FixSEHSections(target->GetSect(), injectionLen); 
    } else {
    	DebugWriteLine(L"neither fat nor tiny ... invalid header???");
    }
    
    if (codeBuf != nullptr && newLength > 0) {
        hr = pICorProfilerInfo2->SetILFunctionBody(moduleID, token, codeBuf);
    }
}

int CProfiler::SetInjectionCode(IMetaDataImport *metaData, byte *buffer, int *size,
									mdMethodDef activateCall, mdMethodDef loggerCall, mdMethodDef deactivateCall,
									int type, int nameId) {
	HRESULT hr = S_OK;
	
	int start = *size;

	buffer[(*size)++] = 0x28; //call
	*(mdMethodDef*)(&buffer[*size]) = deactivateCall;
	*size += sizeof(deactivateCall);
	
	buffer[(*size)++] = 0x20; //ldc.i4
	*(int*)(&buffer[*size]) = type;
	*size += sizeof(type);
	buffer[(*size)++] = 0x20; //ldc.i4
	*(int*)(&buffer[*size]) = nameId;
	*size += sizeof(nameId);
	
	mdTypeDef controlType = 0;
	mdMethodDef getNameToken = 0;
	mdMethodDef getTextToken = 0;
	
	switch (type) {
		case 1: // Console
			buffer[(*size)++] = 0x14; //ldnull
			buffer[(*size)++] = 0x14; //ldnull
			break;
		case 2: // Windows Forms
			hr = metaData->FindTypeDefByName(L"System.Windows.Forms.Control", nullptr, &controlType);
			
			if (!SUCCEEDED(hr) || controlType == 0) {
				DebugWriteLine(L"FindTypeDefByName failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			hr = metaData->FindMethod(controlType, L"get_Name", getNameSig, sizeof(getNameSig), &getNameToken);
			
			if (!SUCCEEDED(hr) || getNameToken == 0) {
				DebugWriteLine(L"FindMethod failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			hr = metaData->FindMethod(controlType, L"get_Text", getNameSig, sizeof(getNameSig), &getTextToken);
			
			if (!SUCCEEDED(hr) || getTextToken == 0) {
				DebugWriteLine(L"FindMethod failed");
				buffer[(*size)++] = 0x14; //ldnull
				buffer[(*size)++] = 0x14; //ldnull
				break;
			}
			
			buffer[(*size)++] = 0x02; //ldarg.0
			buffer[(*size)++] = 0x28; //call
			*(mdMethodDef*)(&buffer[*size]) = getNameToken;
			*size += sizeof(getNameToken);

			buffer[(*size)++] = 0x02; //ldarg.0
			buffer[(*size)++] = 0x6F; //callvirt
			*(mdMethodDef*)(&buffer[*size]) = getTextToken;
			*size += sizeof(getTextToken);
			break;
		case 3: // WPF
			buffer[(*size)++] = 0x14; //ldnull
			buffer[(*size)++] = 0x14; //ldnull
			break;
	}
	
	buffer[(*size)++] = 0x28; //call
	*(mdMethodDef*)(&buffer[*size]) = loggerCall;
	*size += sizeof(loggerCall);

	buffer[(*size)++] = 0x28; //call
	*(mdMethodDef*)(&buffer[*size]) = activateCall;
	*size += sizeof(activateCall);
	
	return *size - start;
}

void CProfiler::ConvertToFat(byte *target, int *size) {
	COR_ILMETHOD_FAT *targetTable =  (COR_ILMETHOD_FAT *)target;
    memset(targetTable, 0, FAT_HEADER_SIZE);
    targetTable->Flags = CorILMethod_FatFormat;
    targetTable->Size = FAT_HEADER_SIZE / sizeof(DWORD);
    targetTable->MaxStack = DEFAULT_MAX_STACK;
    *size = FAT_HEADER_SIZE;
}

/// <summary>
/// Moves all offsets in SEH section tables by the given count of bytes.
/// </summary>
void CProfiler::FixSEHSections(const COR_ILMETHOD_SECT *sections, int offset) {
    while (sections) {
        if (sections->Kind() == CorILMethod_Sect_EHTable) {
            COR_ILMETHOD_SECT_EH *peh = (COR_ILMETHOD_SECT_EH *) sections;
			if (peh->IsFat()) {
                COR_ILMETHOD_SECT_EH_FAT *pFat = (COR_ILMETHOD_SECT_EH_FAT*) peh;

                for (unsigned int i = 0;  i < peh->EHCount();  i++) {
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT *pfeh = &pFat->Clauses[i];

                    if (pfeh->Flags & COR_ILEXCEPTION_CLAUSE_FILTER)
                        pfeh->FilterOffset += offset;

                    pfeh->HandlerOffset += offset;
                    pfeh->TryOffset += offset;
                }
            } else {
                COR_ILMETHOD_SECT_EH_SMALL *pSmall = (COR_ILMETHOD_SECT_EH_SMALL *) peh;

                for (unsigned int i = 0;  i < peh->EHCount();  i++) {
                    IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL *pseh = &pSmall->Clauses[i];

                    if (pseh->Flags & COR_ILEXCEPTION_CLAUSE_FILTER)
                        pseh->FilterOffset += offset;

                    pseh->TryOffset += offset;
                    pseh->HandlerOffset += offset;
                }
            }
        }
        sections = sections->Next();
    } 
}

bool CProfiler::CreateMethod(IMetaDataEmit *emitter, const WCHAR *name, PCCOR_SIGNATURE sig, mdMethodDef *methodDefinition, mdToken moduleRef) {
	HRESULT hr = emitter->DefineMethod(mdTokenNil, name, mdPublic | mdStatic | mdPinvokeImpl, sig, sizeof(sig), 0, miIL | miManaged, methodDefinition);
	
	if (!SUCCEEDED(hr))
		return false;
	
	hr = emitter->DefinePinvokeMap(*methodDefinition, pmNoMangle | pmCallConvStdcall, name, moduleRef);
	
	if (!SUCCEEDED(hr))
		return false;
	
	return true;
}

STDMETHODIMP CProfiler::ModuleLoadFinished(ModuleID moduleID, HRESULT /*hrStatus*/) {
	HRESULT hr = S_OK;
	
	IMetaDataEmit *pIMetaDataEmit = nullptr;
	IMetaDataAssemblyImport *asmMetaData = nullptr;
	
	WCHAR moduleName[NAME_BUFFER_SIZE];
	WCHAR assemblyName[NAME_BUFFER_SIZE];
	
	AssemblyID assemblyID;
	
	ULONG actualLen = 0;
	
	hr = pICorProfilerInfo->GetModuleInfo(moduleID, nullptr, NAME_BUFFER_SIZE, &actualLen, moduleName, &assemblyID);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
	
	hr = pICorProfilerInfo->GetAssemblyInfo(assemblyID, NAME_BUFFER_SIZE, &actualLen, assemblyName, nullptr, nullptr);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
	
	DebugWriteLine(L"\n----------------- Module Load finished -----------------\n");
	DebugWriteLine(L"Module Name: '%s'\n", moduleName);
	DebugWriteLine(L"Assembly Name: '%s'\n", assemblyName);
	DebugWriteLine(L"--------------------------------------------------------\n");
	
	bool found = false;
	
	for (int i = 0; i < ASSEMBLY_INJECTION_NAME_LIST_LENGTH; i++) {
		if (wcscmp(assemblyInjectionNameList[i], assemblyName) == 0) {
			found = true;
			break;
		}
	}
	
	if (!found) {
		DebugWriteLine(L"Assembly Name '%s' not found in list!\n", assemblyName);
		goto CLEANUP;
	}
	
	hr = pICorProfilerInfo->GetModuleMetaData(moduleID, ofRead | ofWrite, IID_IMetaDataEmit, (LPUNKNOWN *) &pIMetaDataEmit);
		
	if (!SUCCEEDED(hr) || pIMetaDataEmit == nullptr)
		goto CLEANUP;
	
	hr = pICorProfilerInfo->GetModuleMetaData(moduleID, ofRead | ofWrite, IID_IMetaDataAssemblyImport, (LPUNKNOWN *) &asmMetaData);

	if (!SUCCEEDED(hr) || asmMetaData == nullptr)
		goto CLEANUP;
		
	mdToken moduleRef;
	hr = pIMetaDataEmit->DefineModuleRef(HOOK_MODULE_NAME, &moduleRef);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
		
	mdMethodDef deactivator;
	if (!CreateMethod(pIMetaDataEmit, L"DeactivateProfiler", rgSig, &deactivator, moduleRef))
		goto CLEANUP;
		
	mdMethodDef activator;
	if (!CreateMethod(pIMetaDataEmit, L"ActivateProfiler", rgSig, &activator, moduleRef))
		goto CLEANUP;

	mdMethodDef logger;
	hr = pIMetaDataEmit->DefineMethod(mdTokenNil, L"LogEvent", mdPublic | mdStatic | mdPinvokeImpl, loggerSig, sizeof(loggerSig), 0, miIL | miManaged, &logger);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
		
	hr = pIMetaDataEmit->DefinePinvokeMap(logger, pmNoMangle | pmCallConvStdcall | pmCharSetUnicode, L"LogEvent", moduleRef);
	
	if (!SUCCEEDED(hr))
		goto CLEANUP;
	
	{
		methodMapCriticalSection.Enter();
		TMethodTokenMap::iterator it = this->methodMap.find(moduleID);
		if (it == this->methodMap.end()) {
			this->methodMap.insert(TMethodTokenPair(moduleID, HandlerInfo(activator, deactivator, logger)));
		}
		methodMapCriticalSection.Leave();
	}
	
CLEANUP:
	#ifdef DEBUG
	if (!SUCCEEDED(hr)) {
		MessageBox(nullptr, TEXT("Crashed in ModuleLoadFinished"), TEXT("Crash!"), MB_OK);
		__debugbreak();
	}
	#endif
	
	if (pIMetaDataEmit != nullptr)
		pIMetaDataEmit->Release();
	
	if (asmMetaData != nullptr)
		asmMetaData->Release();
	
	return S_OK;
}

STDMETHODIMP CProfiler::JITCompilationStarted(FunctionID functionID, BOOL /*fIsSafeToBlock*/) {
	WCHAR *name;
	int nameId = (int)MapFunction(functionID, (const WCHAR **)(&name));
	
	if (name == nullptr)
		return S_OK;
	
	for (int i = 0; i < CONSOLE_GROUP_LENGTH; i++) {
		if (wcsstr(consoleGroupList[i], name) != nullptr) {
			DebugWriteLine(L"Rewriting %s", name);
			Rewrite(functionID, 0x1, nameId);
		}
	}
	
	for (int i = 0; i < WINFORMS_GROUP_LENGTH; i++) {
		if (wcsstr(winFormsGroupList[i], name) != nullptr) {
			DebugWriteLine(L"Rewriting %s", name);
			Rewrite(functionID, 0x2, nameId);
		}
	}
	
	for (int i = 0; i < WPF_GROUP_LENGTH; i++) {
		if (wcsstr(wpfGroupList[i], name) != nullptr) {
			DebugWriteLine(L"Rewriting %s", name);
			Rewrite(functionID, 0x3, nameId);
		}
	}

	return S_OK;
}