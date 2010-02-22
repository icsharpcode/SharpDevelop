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
		Module DebugModule { get; }
		string Name { get; }
		int MetadataToken { get; }
		bool IsStatic { get; }
		bool IsPublic { get; }
		bool IsAssembly { get; }
		bool IsFamily { get; }
		bool IsPrivate { get; }
		DebugType MemberType { get; }
	}
}
