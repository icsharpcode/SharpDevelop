// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestWorkbench
	{
		PadDescriptor GetPad(Type type);
		void SafeThreadAsyncCall(Action method);
		void SafeThreadAsyncCall<A>(Action<A> method, A arg1);
	}
}
