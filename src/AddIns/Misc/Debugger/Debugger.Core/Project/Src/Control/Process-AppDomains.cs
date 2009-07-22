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
	public partial class Process
	{
		List<AppDomain> appDomainCollection = new List<AppDomain>();
		
		public AppDomain GetAppDomain(ICorDebugAppDomain corAppDomain)
		{
			foreach(AppDomain a in appDomainCollection) {
				if (a.CorDebugAppDomain.Equals(corAppDomain)) {
					return a;
				}
			}
			throw new DebuggerException("AppDomain not found");
		}
	}
}