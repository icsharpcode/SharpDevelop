// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Threading;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	public static partial class Utils
	{
		/// <param name="process">Process on which to track debuggee state</param>
		public static void DoEvents(Process process)
		{
			if (process == null) return;
			DebuggeeState oldState = process.DebuggeeState;
			WpfDoEvents();
			DebuggeeState newState = process.DebuggeeState;
			if (oldState != newState) {
				LoggingService.Info("Aborted because debuggee resumed");
				throw new AbortedBecauseDebuggeeResumedException();
			}
		}
		
		public static void WpfDoEvents()
		{
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
			Dispatcher.PushFrame(frame);
		}
		
		/// <summary>
		/// Evaluates System.Collections.ICollection.Count property on given object.
		/// </summary>
		/// <exception cref="GetValueException">Evaluating System.Collections.ICollection.Count on targetObject failed.</exception>
		public static int GetIListCount(Expression targetObject)
		{
			Value list = targetObject.Evaluate(WindowsDebugger.CurrentProcess);
			var iCollectionInterface = list.Type.GetInterface(typeof(ICollection).FullName);
			if (iCollectionInterface == null)
				throw new GetValueException(targetObject, targetObject.PrettyPrint() + " does not implement System.Collections.ICollection");
			PropertyInfo countProperty = iCollectionInterface.GetProperty("Count");
			// Do not get string representation since it can be printed in hex
			return (int)list.GetPropertyValue(countProperty).PrimitiveValue;
		}
		
		/// <summary>
		/// Prepends a cast to IList before the given Expression.
		/// </summary>
		public static Expression CastToIList(this Expression expr)
		{
			return new CastExpression(
				new TypeReference(typeof(IList).FullName),
				expr.Parenthesize(),
				CastType.Cast
			);
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
		Stopwatch stopwatch = new Stopwatch();
		
		public PrintTime(string text)
		{
			this.text = text;
			stopwatch.Start();
		}
		
		public void Dispose()
		{
			stopwatch.Stop();
			LoggingService.InfoFormatted("{0} ({1} ms)", text, stopwatch.ElapsedMilliseconds);
		}
	}
}
