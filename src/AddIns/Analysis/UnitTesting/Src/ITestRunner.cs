// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public interface ITestRunner : IDisposable
	{
		event TestFinishedEventHandler TestFinished;
		event EventHandler AllTestsFinished;
		event MessageReceivedEventHandler MessageReceived;
		void Start(SelectedTests selectedTests);
		void Stop();
	}
}
