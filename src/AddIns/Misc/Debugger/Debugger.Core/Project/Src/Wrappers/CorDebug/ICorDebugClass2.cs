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

	public partial class ICorDebugClass2
	{
		public ICorDebugType GetParameterizedType(uint elementType, ICorDebugType[] ppTypeArgs)
		{
			return this.GetParameterizedType(elementType, (uint)ppTypeArgs.Length, ppTypeArgs);
		}
	}
}

#pragma warning restore 1591
