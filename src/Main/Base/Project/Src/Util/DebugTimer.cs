/*
 * Created by SharpDevelop.
 * User: tfssetup
 * Date: 10/27/2006
 * Time: 9:29 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	public static class DebugTimer
	{
		static int startTime;
		
		[Conditional("DEBUG")]
		public static void Start()
		{
			startTime = Environment.TickCount;
		}
		
		[Conditional("DEBUG")]
		public static void Stop(string desc)
		{
			int stopTime = Environment.TickCount;
			LoggingService.Debug('"' + desc + "\" took " + (stopTime - startTime) + " ms");
		}
	}
}
