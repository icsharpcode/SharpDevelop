// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;

namespace Debugger {
	
	unsafe class SignatureStream {
		byte[] signature;
		int    currentPos;
		
		public SignatureStream (byte[] signature)
		{
			this.signature = signature;
			currentPos = 0;
		}
		
		public SignatureStream (IntPtr pSigBlob, uint sigBlobSize)
		{
			signature = new Byte[sigBlobSize];
			Marshal.Copy(pSigBlob, signature, 0, (int)sigBlobSize);
			currentPos = 0;
		}
		
		public bool EndOfStream {
			get {
				return currentPos >= signature.Length;
			}
		}
		
		byte PeekByte {
			get {
				if (EndOfStream) throw new BadSignatureException();
				return signature[currentPos];
			}
		}
		
		byte ReadByte {
			get {
				byte value = PeekByte;
				currentPos++;
				return value;
			}
		}
		
		
		uint ReadData(out int compressedSize)
		{
			if ((PeekByte & 0x80) == 0x00) {
				compressedSize = 1;
		        return ReadByte;
			}
			if ((PeekByte & 0xC0) == 0x80) {
				compressedSize = 2; 
		    	return (uint)(
		    	              (ReadByte & 0x3F) * 0x100 +
		    	              ReadByte
		    	             );
		    }   
		    if ((PeekByte & 0xE0) == 0xC0) {
		    	compressedSize = 4;
		    	return (uint)(
		    	              (ReadByte & 0x1F) * 0x1000000 + 
		                      ReadByte * 0x10000 +
		                      ReadByte * 0x100 +
		                      ReadByte
		    	             );
		    }   
		    throw new BadSignatureException();
		}
		
		
		public uint PeekData()
		{
			int oldPos = currentPos;
			uint res = ReadData();
			currentPos = oldPos;
			return res;
		}
		
		public uint ReadData()
		{
			int compressedSize;
			return ReadData(out compressedSize);
		}
		
		
		CorTokenType[] encodeTokenType = new CorTokenType[] {CorTokenType.mdtTypeDef, CorTokenType.mdtTypeRef, CorTokenType.mdtTypeSpec, CorTokenType.mdtBaseType};
		
		public uint PeekToken()
		{
			int oldPos = currentPos;
			uint res = ReadToken();
			currentPos = oldPos;
			return res;
		}
		
		public uint ReadToken()
		{
		    uint data; 
		    uint tokenType; 
		
		    data = ReadData();  
		    tokenType = (uint)encodeTokenType[data & 0x3]; 
		    return (data >> 2) | tokenType;  
		}
		
		
		public uint PeekSignedInt()
		{
			int oldPos = currentPos;
			uint res = ReadSignedInt();
			currentPos = oldPos;
			return res;
		}
		
		public uint ReadSignedInt()
		{
			int  compressedSize;
		    bool signed;   
		    uint data;  
		
		    data = ReadData(out compressedSize);  
		    signed = (data & 0x1) == 1; 
		    data = data >> 1; 
			if (signed) {
				switch (compressedSize) {
			    	case 1:
			            data |= 0xffffffc0; 
						break;
					case 2:
						data |= 0xffffe000;
						break;
			        case 4:
		            	data |= 0xf0000000;
						break;
		        	default:
		        		throw new BadSignatureException();
				}
		    }
		    return data;
		}
		
		
		public uint PeekCallingConv()
		{
			int oldPos = currentPos;
			uint res = ReadCallingConv();
			currentPos = oldPos;
			return res;
		}
		
		public uint ReadCallingConv()
		{
		    return ReadData();  
		}  
		
		
		public CorElementType PeekElementType()
		{
			int oldPos = currentPos;
			CorElementType res = ReadElementType();
			currentPos = oldPos;
			return res;
		}
		
		public CorElementType ReadElementType()
		{
		    return (CorElementType)ReadData();    
		}
		
		
		
		public void ReadCustomMod()
		{
			while (PeekElementType() == CorElementType.CMOD_OPT ||
			       PeekElementType() == CorElementType.CMOD_REQD) {
				ReadElementType();
			    ReadToken();
			}
		}
		
		public string ReadType()
		{
			CorElementType type = ReadElementType();
			switch(type)
			{
				case CorElementType.BOOLEAN:
				case CorElementType.CHAR:
				case CorElementType.I1:
				case CorElementType.U1:
				case CorElementType.I2:
				case CorElementType.U2:
				case CorElementType.I4:
				case CorElementType.U4:
				case CorElementType.I8:
				case CorElementType.U8:
				case CorElementType.R4:
				case CorElementType.R8:
				case CorElementType.I:
				case CorElementType.U:
				case CorElementType.STRING:
				case CorElementType.OBJECT:
					return type.ToString();	
				
				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
					ReadToken();
					return type.ToString();
				
				case CorElementType.PTR:
					ReadCustomMod();
					if (PeekElementType() == CorElementType.VOID) {
						ReadElementType();
						break;
					} else {
						return "Pointer:" + ReadType();
					}
					
				case CorElementType.FNPTR:
					ReadFunction();
					break;

				case CorElementType.ARRAY:
					ReadType();
					ReadArrayShape();
					break;
				
				case CorElementType.SZARRAY: // Short-cut for single dimension zero lower bound array
					ReadCustomMod();
					ReadType();
					break;

				default:
					throw new BadSignatureException();
			}		
			return "";
		}
		
		public void ReadArrayShape()
		{
			ReadData(); //rank
			uint numSizes = ReadData();
			for (int i = 0; i < numSizes; i++) {
				ReadData(); //size
			}
			if (numSizes > 0) {
				uint numLoBounds = ReadData();
				for (int i = 0; i < numLoBounds; i++) {
					ReadData(); //LoBound
				}				
			}
		}
		
		public void ReadFunction()
		{
			uint callConv = ReadCallingConv();
			uint paramCount = ReadData();
			
			// Read return type	
			switch (ReadElementType()) {
				case CorElementType.BYREF:
					ReadType();
					break;
				case CorElementType.TYPEDBYREF:
					break;
				case CorElementType.VOID:
					break;
				default:
		        	throw new BadSignatureException();
			}
			
			// Read params
			for (int i = 0; i < paramCount; i++) {
				ReadCustomMod();
				switch (PeekElementType()) {
					case CorElementType.BYREF:
						ReadElementType();
						ReadType();
						break;
					case CorElementType.TYPEDBYREF:
						ReadElementType();
						break;
					default:
			        	ReadType();
						break;
				}				
			}
		}
	}
}
