// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using Debugger.Util;
using ICSharpCode.Core;

namespace Debugger.AddIn.TreeModel
{
	public static partial class Utils
	{
		/// <param name="process">Process on which to track debuggee state</param>
		public static void DoEvents(Process process)
		{
			if (process == null) return;
			DebuggeeState oldState = process.DebuggeeState;
			//using(new PrintTimes("Application.DoEvents()"))
			{
				Application.DoEvents();
			}
			{
			
			}
			DebuggeeState newState = process.DebuggeeState;
			if (oldState != newState) {
				LoggingService.Info("Aborted because debuggee resumed");
				throw new AbortedBecauseDebuggeeResumedException();
			}
		}
	}
	
	public class AbortedBecauseDebuggeeResumedException: System.Exception
	{
		public AbortedBecauseDebuggeeResumedException(): base()
		{
			
		}
	}
	
	public class PrintTimes: PrintTime
	{
		public PrintTimes(string text): base(text + " - end")
		{
			LoggingService.InfoFormatted("{0} - start", text);
		}
	}
	
	public class PrintTime: IDisposable
	{
		string text;
		DateTime start;
		
		public PrintTime(string text)
		{
			this.text = text;
			this.start = HighPrecisionTimer.Now;
		}
		
		public void Dispose()
		{
			TimeSpan dur = HighPrecisionTimer.Now - start;
			LoggingService.InfoFormatted("{0} ({1} ms)", text, dur.TotalMilliseconds);
		}
	}
}
