// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This service is used to summarize important information
	/// about the state of the application when an exception occurs.
	/// </summary>
	public static class ApplicationStateInfoService
	{
		static readonly Dictionary<string, StateGetter> stateGetters = new Dictionary<string, StateGetter>(StringComparer.InvariantCulture);
		
		/// <summary>
		/// Registers a new method to be invoked to get information about the current state of the application.
		/// </summary>
		/// <param name="title">The title of the new state entry.</param>
		/// <param name="stateGetter">The method to be invoked to get the state value.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
		/// <exception cref="ArgumentException">A state getter with the specified <paramref name="title"/> is already registered.</exception>
		public static void RegisterStateGetter(string title, StateGetter stateGetter)
		{
			lock(stateGetters) {
				stateGetters.Add(title, stateGetter);
			}
		}
		
		/// <summary>
		/// Determines whether a state getter with the specified title is already registered.
		/// </summary>
		/// <param name="title">The title to look for.</param>
		/// <returns><c>true</c>, if a state getter with the specified title is already registered, otherwise <c>false</c>.</returns>
		public static bool IsRegistered(string title)
		{
			lock(stateGetters) {
				return stateGetters.ContainsKey(title);
			}
		}
		
		/// <summary>
		/// Unregisters a state getter.
		/// </summary>
		/// <param name="title">The title of the state entry to remove.</param>
		/// <returns><c>true</c> if the specified title was found and removed, otherwise <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
		public static bool UnregisterStateGetter(string title)
		{
			lock(stateGetters) {
				return stateGetters.Remove(title);
			}
		}
		
		/// <summary>
		/// Gets a snapshot of the current application state information from all registered state getters.
		/// </summary>
		/// <returns>A dictionary with the titles and results of all registered state getters.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static IDictionary<string, object> GetCurrentApplicationStateInfo()
		{
			Dictionary<string, object> state = new Dictionary<string, object>(stateGetters.Count, stateGetters.Comparer);
			lock(stateGetters) {
				foreach (KeyValuePair<string, StateGetter> entry in stateGetters) {
					try {
						state.Add(entry.Key, entry.Value());
					} catch (Exception ex) {
						state.Add(entry.Key, new StateGetterExceptionInfo(ex));
					}
				}
			}
			return state;
		}
		
		/// <summary>
		/// Appends the current application state information from all registered state getters
		/// to the specified <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> to append the state information to.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static void AppendFormatted(StringBuilder sb)
		{
			IFormattable f;
			Exception e;
			StateGetterExceptionInfo exceptionInfo;
			
			foreach (KeyValuePair<string, object> entry in GetCurrentApplicationStateInfo()) {
				e = null;
				sb.Append(entry.Key);
				sb.Append(": ");
				
				if (entry.Value == null) {
					sb.AppendLine("<null>");
				} else {
					f = entry.Value as IFormattable;
					if (f != null) {
						try {
							sb.AppendLine(f.ToString(null, CultureInfo.InvariantCulture));
						} catch (Exception ex) {
							sb.AppendLine("--> Exception thrown by IFormattable.ToString:");
							e = ex;
						}
					} else {
						exceptionInfo = entry.Value as StateGetterExceptionInfo;
						if (exceptionInfo != null) {
							sb.AppendLine("--> Exception thrown by the state getter:");
							e = exceptionInfo.Exception;
						} else {
							try {
								sb.AppendLine(entry.Value.ToString());
							} catch (Exception ex) {
								sb.AppendLine("--> Exception thrown by ToString:");
								e = ex;
							}
						}
					}
				}
				
				if (e != null) {
					sb.AppendLine(e.ToString());
				}
			}
		}
		
		sealed class StateGetterExceptionInfo
		{
			readonly Exception exception;
			
			internal StateGetterExceptionInfo(Exception exception)
			{
				if (exception == null)
					throw new ArgumentNullException("exception");
				this.exception = exception;
			}
			
			internal Exception Exception {
				get { return exception; }
			}
			
			public override string ToString()
			{
				return "StateGetterExceptionInfo: " + this.exception.ToString();
			}
		}
	}
	
	/// <summary>
	/// A delegate used to get information about the current state of the application.
	/// </summary>
	public delegate object StateGetter();
}
