// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

using System;

namespace Debugger.Wrappers.MetaData
{
	public struct InterfaceImplProps
	{
		public uint Token;
		public uint classTypeDef;
		public uint ptkIface;
		
		public override string ToString()
		{
			return string.Format("[InterfaceImplProps Token={0:X} ClassTypeDef={1:X} PtkIface={2:X}]", this.Token, this.classTypeDef, this.ptkIface);
		}
	}
}
