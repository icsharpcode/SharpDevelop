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
	public class AppDomainCollection: CollectionWithEvents<AppDomain>
	{
		public AppDomainCollection(NDebugger dbgr): base(dbgr) {}
		
		public AppDomain this[ICorDebugAppDomain corAppDomain] {
			get {
				foreach(AppDomain a in this) {
					if (a.CorDebugAppDomain.Equals(corAppDomain)) {
						return a;
					}
				}
				throw new DebuggerException("AppDomain not found");
			}
		}
	}
}