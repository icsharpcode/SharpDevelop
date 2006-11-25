// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Collections.Generic;
	
	
	public partial class ICorDebugChainEnum
	{
		public IEnumerable<ICorDebugChain> Enumerator {
			get {
				Reset();
				uint index = this.Count - 1;
				while (true) {
					ICorDebugChain corChain = Next();
					if (corChain != null) {
						corChain.Index = index--;
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

#pragma warning restore 1591
