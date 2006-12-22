// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;

namespace Debugger.Util
{
	/// <summary>
	/// HighPrecisionTimer can obtain much more accurate time measurement
	/// for performace optimization
	/// </summary>
	public static class HighPrecisionTimer
	{
		[DllImport("kernel32.dll")]
		static extern int QueryPerformanceFrequency(out long frequency);
		
		[DllImport("kernel32.dll")]
		static extern int QueryPerformanceCounter(out long count);
		
		static DateTime startTime;
		static long startTicks;
		
		static HighPrecisionTimer()
		{
			startTime = DateTime.Now;
			startTicks = Ticks;
		}
		
		static long Ticks {
			get {
				long frequency;
				long count;
				QueryPerformanceFrequency(out frequency);
				QueryPerformanceCounter(out count);
				return (count / frequency) * TimeSpan.TicksPerSecond + (count % frequency) * TimeSpan.TicksPerSecond / frequency;
			}
		}
		
		public static DateTime Now {
			get {
				return startTime.AddTicks(Ticks - startTicks);
			}
		}
	}
}
