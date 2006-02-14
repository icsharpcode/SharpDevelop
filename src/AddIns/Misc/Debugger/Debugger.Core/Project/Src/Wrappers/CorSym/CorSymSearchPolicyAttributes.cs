// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public enum CorSymSearchPolicyAttributes : int
	{
		
		AllowRegistryAccess = 1,
		
		AllowSymbolServerAccess = 2,
		
		AllowOriginalPathAccess = 4,
		
		AllowReferencePathAccess = 8,
	}
}
