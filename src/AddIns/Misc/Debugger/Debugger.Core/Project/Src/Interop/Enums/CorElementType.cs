// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	public enum CorElementType: uint
	{
		END            = 0x0,  
		VOID           = 0x1,  
		BOOLEAN        = 0x2,  
		CHAR           = 0x3,  
		I1             = 0x4,  
		U1             = 0x5, 
		I2             = 0x6,  
		U2             = 0x7,  
		I4             = 0x8,  
		U4             = 0x9,  
		I8             = 0xa,  
		U8             = 0xb,  
		R4             = 0xc,  
		R8             = 0xd,  
		STRING         = 0xe,  

		// every type above PTR will be simple type 
		PTR            = 0xf,      // PTR <type>   
		BYREF          = 0x10,     // BYREF <type> 

		// Please use VALUETYPE. VALUECLASS is deprecated.
		VALUETYPE      = 0x11,     // VALUETYPE <class Token> 
		CLASS          = 0x12,     // CLASS <class Token>  

		ARRAY          = 0x14,     // MDARRAY <type> <rank> <bcount> <bound1> ... <lbcount> <lb1> ...  

		TYPEDBYREF     = 0x16,     // This is a simple type.   

		I              = 0x18,     // native integer size  
		U              = 0x19,     // native unsigned integer size 
		FNPTR          = 0x1B,     // FNPTR <complete sig for the corFunction including calling convention>
		OBJECT         = 0x1C,     // Shortcut for System.Object
		SZARRAY        = 0x1D,     // Shortcut for single dimension zero lower bound array
		// SZARRAY <type>

		// This is only for binding
		CMOD_REQD      = 0x1F,     // required C modifier : E_T_CMOD_REQD <mdTypeRef/mdTypeDef>
		CMOD_OPT       = 0x20,     // optional C modifier : E_T_CMOD_OPT <mdTypeRef/mdTypeDef>

		// This is for signatures generated internally (which will not be persisted in any way).
		INTERNAL       = 0x21,     // INTERNAL <typehandle>

		// Note that this is the max of base type excluding modifiers   
		MAX            = 0x22,     // first invalid element type   


		MODIFIER       = 0x40, 
		SENTINEL       = 0x01 | MODIFIER, // sentinel for varargs
		PINNED         = 0x05 | MODIFIER,

	}
}
