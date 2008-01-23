// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public static partial class Utils
	{
		public static void DoEvents(DebuggeeState debuggeeState)
		{
			if (debuggeeState == null) {
				throw new ArgumentNullException();
			}
			if (debuggeeState.HasExpired) {
				throw new System.Exception("State is expired before DoEvents");
			}
			//using(new PrintTimes("Application.DoEvents()"))
			{
				Application.DoEvents();
			}
			if (debuggeeState.HasExpired) {
				LoggingService.Info("Aborted because debuggee resumed");
				throw new AbortedBecauseDebuggeeResumedException();
			}
		}
		
		public static WindowsDebugger WindowsDebugger {
			get {
				return (WindowsDebugger)DebuggerService.CurrentDebugger;
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
			this.start = Debugger.Util.HighPrecisionTimer.Now;
		}
		
		public void Dispose()
		{
			TimeSpan dur = Debugger.Util.HighPrecisionTimer.Now - start;
			LoggingService.InfoFormatted("{0} ({1} ms)", text, dur.TotalMilliseconds);
		}
	}
}
