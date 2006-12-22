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
	public struct ParamProps
	{
		public uint Token;
		public string Name;
		public uint MethodToken;
		public uint ParameterSequence;
		public uint Flags;
	}
}

#pragma warning restore 1591
