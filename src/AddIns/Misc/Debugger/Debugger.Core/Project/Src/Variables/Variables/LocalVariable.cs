// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Represents a local variable in a function
	/// </summary>
	public class LocalVariable: NamedValue
	{
		internal LocalVariable(string name,
		                       Process process,
		                       IExpirable[] expireDependencies,
		                       IMutable[] mutateDependencies,
		                       CorValueGetter corValueGetter)
			:base (name,
			       process,
			       expireDependencies,
			       mutateDependencies,
			       corValueGetter)
		{
			
		}
	}
}
