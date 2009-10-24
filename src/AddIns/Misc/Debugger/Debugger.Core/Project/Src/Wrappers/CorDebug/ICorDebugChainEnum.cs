// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorDebug
{
	using System;
	using System.Collections.Generic;
	
	
	public static partial class CorDebugExtensionMethods
	{
		public static IEnumerable<ICorDebugChain> GetEnumerator(this ICorDebugChainEnum corChainEnum)
		{
			corChainEnum.Reset();
			while (true) {
				ICorDebugChain corChain = corChainEnum.Next();
				if (corChain != null) {
					yield return corChain;
				} else {
					break;
				}
			}
		}
			
		public static ICorDebugChain Next(this ICorDebugChainEnum corChainEnum)
		{
			ICorDebugChain[] corChains = new ICorDebugChain[1];
			uint chainsFetched = corChainEnum.Next(1, corChains);
			if (chainsFetched == 0) {
				return null;
			} else {
				return corChains[0];
			}
		}
	}
}

#pragma warning restore 1591
