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
		public static IEnumerable<ICorDebugType> GetEnumerator(this ICorDebugTypeEnum corTypeEnum)
		{
			corTypeEnum.Reset();
			while (true) {
				ICorDebugType corType = corTypeEnum.Next();
				if (corType != null) {
					yield return corType;
				} else {
					break;
				}
			}
		}
		
		public static ICorDebugType Next(this ICorDebugTypeEnum corTypeEnum)
		{
			ICorDebugType[] corTypes = new ICorDebugType[1];
			uint typesFetched = corTypeEnum.Next(1, corTypes);
			if (typesFetched == 0) {
				return null;
			} else {
				return corTypes[0];
			}
		}
		
		public static List<ICorDebugType> ToList(this ICorDebugTypeEnum corTypeEnum)
		{
			return new List<ICorDebugType>(corTypeEnum.GetEnumerator());
		}
	}
}

#pragma warning restore 1591
