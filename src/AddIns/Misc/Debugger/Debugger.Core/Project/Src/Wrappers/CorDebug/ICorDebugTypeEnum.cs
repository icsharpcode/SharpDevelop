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
	
	
	public partial class ICorDebugTypeEnum
	{
		public IEnumerable<ICorDebugType> Enumerator {
			get {
				Reset();
				while (true) {
					ICorDebugType corType = Next();
					if (corType != null) {
						yield return corType;
					} else {
						break;
					}
				}
			}
		}
		
		public ICorDebugType Next()
		{
			ICorDebugType[] corTypes = new ICorDebugType[1];
			uint typesFetched = this.Next(1, corTypes);
			if (typesFetched == 0) {
				return null;
			} else {
				return corTypes[0];
			}
		}
		
		public List<ICorDebugType> ToList()
		{
			return new List<ICorDebugType>(this.Enumerator);
		}
	}
}

#pragma warning restore 1591
