// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Collections.Generic;
	
	
	public partial class ICorDebugChainEnum
	{
		public IEnumerable<ICorDebugChain> Enumerator {
			get {
				while (true) {
					ICorDebugChain corChain = Next();
					if (corChain != null) {
						yield return corChain;
					} else {
						break;
					}
				}
			}
		}
			
		public ICorDebugChain Next()
		{
			ICorDebugChain[] corChains = new ICorDebugChain[1];
			uint chainsFetched = this.Next(1, corChains);
			if (chainsFetched == 0) {
				return null;
			} else {
				return corChains[0];
			}
		}
	}
}
