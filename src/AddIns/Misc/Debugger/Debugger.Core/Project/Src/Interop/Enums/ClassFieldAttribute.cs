// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.MetaData
{
	[System.Flags()]
	enum ClassFieldAttribute: uint
	{
		// member access mask - Use this mask to retrieve accessibility information.
		fdFieldAccessMask           =   0x0007,
		fdPrivateScope              =   0x0000,     // Member not referenceable.
		fdPrivate                   =   0x0001,     // Accessible only by the parent type.  
		fdFamANDAssem               =   0x0002,     // Accessible by sub-types only in this Assembly.
		fdAssembly                  =   0x0003,     // Accessibly by anyone in the Assembly.
		fdFamily                    =   0x0004,     // Accessible only by type and sub-types.    
		fdFamORAssem                =   0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
		fdPublic                    =   0x0006,     // Accessibly by anyone who has visibility to this scope.    
		// end member access mask

		// field contract attributes.
		fdStatic                    =   0x0010,     // Defined on type, else per instance.
		fdInitOnly                  =   0x0020,     // Field may only be initialized, not written to after init.
		fdLiteral                   =   0x0040,     // Value is compile time constant.
		fdNotSerialized             =   0x0080,     // Field does not have to be serialized when type is remoted.

		fdSpecialName               =   0x0200,     // field is special.  Name describes how.

		// interop attributes
		fdPinvokeImpl               =   0x2000,     // Implementation is forwarded through pinvoke.

		// Reserved flags for runtime use only.
		fdReservedMask              =   0x9500,
		fdRTSpecialName             =   0x0400,     // Runtime(metadata internal APIs) should check name encoding.
		fdHasFieldMarshal           =   0x1000,     // Field has marshalling information.
		fdHasDefault                =   0x8000,     // Field has default.
		fdHasFieldRVA               =   0x0100,     // Field has RVA.
	}
}
