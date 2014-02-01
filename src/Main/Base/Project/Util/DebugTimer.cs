// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;

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
		static Stack<Stopwatch> stopWatches;
		
		[Conditional("DEBUG")]
		public static void Start()
		{
			if (stopWatches == null) {
				stopWatches = new Stack<Stopwatch>();
			}
			stopWatches.Push(Stopwatch.StartNew());
		}
		
		[Conditional("DEBUG")]
		public static void Stop(string desc)
		{
			Stopwatch watch = stopWatches.Pop();
			watch.Stop();
			LoggingService.Debug("\"" + desc + "\" took " + (watch.ElapsedMilliseconds) + " ms");
		}
		
		/// <summary>
		/// Starts a new stopwatch and stops it when the returned object is disposed.
		/// </summary>
		/// <returns>
		/// Returns disposable object that stops the timer (in debug builds); or null (in release builds).
		/// </returns>
		public static IDisposable Time(string text)
		{
			#if DEBUG
			Stopwatch watch = Stopwatch.StartNew();
			return new CallbackOnDispose(
				delegate {
					watch.Stop();
					LoggingService.Debug("\"" + text + "\" took " + (watch.ElapsedMilliseconds) + " ms");
				}
			);
			#else
			return null;
			#endif
		}
	}
}
