// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2185 $</version>
// </file>

namespace Debugger.Wrappers.MetaData
{
	enum CorTokenType: uint
	{
	    mdtModule               = 0x00000000,       //          
	    mdtTypeRef              = 0x01000000,       //          
	    mdtTypeDef              = 0x02000000,       //          
	    mdtFieldDef             = 0x04000000,       //           
	    mdtMethodDef            = 0x06000000,       //       
	    mdtParamDef             = 0x08000000,       //           
	    mdtInterfaceImpl        = 0x09000000,       //  
	    mdtMemberRef            = 0x0a000000,       //       
	    mdtCustomAttribute      = 0x0c000000,       //      
	    mdtPermission           = 0x0e000000,       //       
	    mdtSignature            = 0x11000000,       //       
	    mdtEvent                = 0x14000000,       //           
	    mdtProperty             = 0x17000000,       //           
	    mdtModuleRef            = 0x1a000000,       //       
	    mdtTypeSpec             = 0x1b000000,       //           
	    mdtAssembly             = 0x20000000,       //
	    mdtAssemblyRef          = 0x23000000,       //
	    mdtFile                 = 0x26000000,       //
	    mdtExportedType         = 0x27000000,       //
	    mdtManifestResource     = 0x28000000,       //
	
	    mdtString               = 0x70000000,       //          
	    mdtName                 = 0x71000000,       //
	    mdtBaseType             = 0x72000000,       // Leave this on the high end value. This does not correspond to metadata table
	}
}
