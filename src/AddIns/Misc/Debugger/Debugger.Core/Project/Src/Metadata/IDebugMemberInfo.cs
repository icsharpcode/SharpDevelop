// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger.MetaData
{
	public interface IDebugMemberInfo
	{
		Type DeclaringType { get; }
		string Name { get; }
		bool IsStatic { get; }
		bool IsPublic { get; }
	}
}
