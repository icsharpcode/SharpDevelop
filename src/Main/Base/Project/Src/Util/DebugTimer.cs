// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides static helpers methods that measure time from
	/// Start() to Stop() and output the timespan to the logging service.
	/// Calls to DebugTimer are only compiled in debug builds.
	/// [Conditional("DEBUG")]
	/// </summary>
	public static class DebugTimer
	{
		[ThreadStatic]
		static Stopwatch stopWatch;
		
		[Conditional("DEBUG")]
		public static void Start()
		{
			if (stopWatch == null) {
				stopWatch = new Stopwatch();
			}
			stopWatch.Start();
		}
		
		[Conditional("DEBUG")]
		public static void Stop(string desc)
		{
			stopWatch.Stop();
			LoggingService.Debug("\"" + desc + "\" took " + (stopWatch.ElapsedMilliseconds) + " ms");
			stopWatch.Reset();
		}
	}
}
