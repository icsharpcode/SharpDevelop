// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.MetaData
{
	public enum CorTokenType: uint
	{
		Module               = 0x00000000,       //          
		TypeRef              = 0x01000000,       //          
		TypeDef              = 0x02000000,       //          
		FieldDef             = 0x04000000,       //           
		MethodDef            = 0x06000000,       //       
		ParamDef             = 0x08000000,       //           
		InterfaceImpl        = 0x09000000,       //  
		MemberRef            = 0x0a000000,       //       
		CustomAttribute      = 0x0c000000,       //      
		Permission           = 0x0e000000,       //       
		Signature            = 0x11000000,       //       
		Event                = 0x14000000,       //           
		Property             = 0x17000000,       //           
		ModuleRef            = 0x1a000000,       //       
		TypeSpec             = 0x1b000000,       //           
		Assembly             = 0x20000000,       //
		AssemblyRef          = 0x23000000,       //
		File                 = 0x26000000,       //
		ExportedType         = 0x27000000,       //
		ManifestResource     = 0x28000000,       //
		
		String               = 0x70000000,       //          
		Name                 = 0x71000000,       //
		BaseType             = 0x72000000,       // Leave this on the high end value. This does not correspond to metadata table
	}
}
