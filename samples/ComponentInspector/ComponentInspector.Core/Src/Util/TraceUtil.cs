// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

namespace NoGoop.Util
{
	public class TraceUtil
	{

		// Error = 1
		// Warn = 2
		// Info = 3
		// Verbose = 4

		protected static TraceSwitch _allSwitch = new TraceSwitch(Constants.NOGOOP, "oakland software applications");

		static TraceUtil()
		{
			try {
				TextWriterTraceListener myWriter = new 
					TextWriterTraceListener(System.Console.Out);
				Trace.Listeners.Add(myWriter);
				//Debug.Listeners.Add(myWriter);
			} catch (Exception ex) {
				// Can't log this because log is not open
				Console.WriteLine("traceutil exception: " + ex);
			}
		}

		public static TraceLevel Level {
			get {
				return _allSwitch.Level;
			}
			set {
				_allSwitch.Level = value;
			}
		}

		public static void Init()
		{
			// Just used to have a hook to get the class loaded
		}

		// Used to wrap tracing/debugging.  In the future, we will
		// want to have a nested scheme were you can specify
		// tracing at the entire application, part of name space
		// and class level.  FIXME - fow now just the entire app.
		public static Boolean If(Object obj, TraceLevel level)
		{
			return If(obj.GetType(), level);
		}

		public static Boolean If(Type cls, TraceLevel level)
		{
			return _allSwitch.Level >= level;
		}

		public static void WriteLineIf(Object obj, TraceLevel level, String str)
		{
			WriteLineIf(obj.GetType(), level, str);
		}

		public static void WriteLineIf(Type cls, TraceLevel level, String str)
		{
			Trace.WriteLineIf(If(cls, level), str);
		}

		public static void WriteLineError(Object obj, String str)
		{
			WriteLineIf(obj, TraceLevel.Error, str);
		}

		public static void WriteLineError(Type cls, String str)
		{
			WriteLineIf(cls, TraceLevel.Error, str);
		}

		public static void WriteLineWarning(Object obj, String str)
		{
			WriteLineIf(obj, TraceLevel.Warning, str);
		}

		public static void WriteLineWarning(Type cls, String str)
		{
			WriteLineIf(cls, TraceLevel.Warning, str);
		}

		public static void WriteLineInfo(Object obj, String str)
		{
			WriteLineIf(obj, TraceLevel.Info, str);
		}

		public static void WriteLineInfo(Type cls, String str)
		{
			WriteLineIf(cls, TraceLevel.Info, str);
		}

		public static void WriteLineVerbose(Object obj, String str)
		{
			WriteLineIf(obj, TraceLevel.Verbose, str);
		}

		public static void WriteLineVerbose(Type cls, String str)
		{
			WriteLineIf(cls, TraceLevel.Verbose, str);
		}

		public static void WriteIf(Object obj, TraceLevel level, String str)
		{
			WriteIf(obj.GetType(), level, str);
		}

		public static void WriteIf(Type cls, TraceLevel level, String str)
		{
			Trace.WriteIf(If(cls, level), str);
		}
	}
}


