// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3047 $</version>
// </file>

using System;

namespace Debugger
{
	/// <summary>
	/// Interface shared by Debugger.Exception and Debugger.DebugeeInnerException.
	/// </summary>
	public interface IDebugeeException
	{
		DebugeeInnerException InnerException { get; }
		string Message { get; }
		string Type { get; }
	}
}
