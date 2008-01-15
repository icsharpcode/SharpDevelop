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
	public static partial class Util
	{
		static DateTime nextDoEventsTime = Debugger.Util.HighPrecisionTimer.Now;
		const double workLoad    = 0.75; // Fraction of getting variables vs. repainting
		const double maxFPS      = 30;   // this prevents too much drawing on good machine
		const double maxWorkTime = 250;  // ms  this ensures minimal response on bad machine
		
		public static void DoEvents()
		{
			if (Debugger.Util.HighPrecisionTimer.Now > nextDoEventsTime) {
				DateTime start = Debugger.Util.HighPrecisionTimer.Now;
				LoggingService.InfoFormatted("Application.DoEvents()");
				Application.DoEvents();
				DateTime end = Debugger.Util.HighPrecisionTimer.Now;
				double doEventsDuration = (end - start).TotalMilliseconds;
				double minWorkTime = 1000 / maxFPS - doEventsDuration; // ms
				double workTime = (doEventsDuration / (1 - workLoad)) * workLoad;
				workTime = Math.Max(minWorkTime, Math.Min(maxWorkTime, workTime)); // Clamp
				nextDoEventsTime = end.AddMilliseconds(workTime);
				double fps = 1000 / (doEventsDuration + workTime);
				// LoggingService.InfoFormatted("Rendering: {0} ms => work budget: {1} ms ({2:f1} FPS)", doEventsDuration, workTime, fps);
			}
		}
		
		public static AbstractNode CreateNode(Expression expression)
		{
			return new ValueNode(expression.Evaluate(WindowsDebugger.DebuggedProcess.SelectedStackFrame));
		}
		
		public static WindowsDebugger WindowsDebugger {
			get {
				return (WindowsDebugger)DebuggerService.CurrentDebugger;
			}
		}
	}
	
	public class AbortedBecauseDebugeeStateExpiredException: System.Exception
	{
		public AbortedBecauseDebugeeStateExpiredException(): base()
		{
			
		}
	}
}
