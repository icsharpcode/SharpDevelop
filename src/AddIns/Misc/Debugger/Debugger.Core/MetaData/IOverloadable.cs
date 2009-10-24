// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

namespace Debugger.MetaData
{
	interface IOverloadable
	{
		ParameterInfo[] GetParameters();
		IntPtr GetSignarture();
	}
}
