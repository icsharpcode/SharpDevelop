// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	public interface ITestResultsMonitor : IDisposable
	{
		event TestFinishedEventHandler TestFinished;
		
		string FileName { get; set; }
		
		void Stop();
		void Start();
		void Read();
	}
}
