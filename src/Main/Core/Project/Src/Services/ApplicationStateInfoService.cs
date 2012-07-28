// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class ApplicationStateInfoService
	{
		readonly Dictionary<string, Func<object>> stateGetters = new Dictionary<string, Func<object>>(StringComparer.InvariantCulture);
		
		/// <summary>
		/// Registers a new method to be invoked to get information about the current state of the application.
		/// </summary>
		/// <param name="title">The title of the new state entry.</param>
		/// <param name="stateGetter">The method to be invoked to get the state value.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
		/// <exception cref="ArgumentException">A state getter with the specified <paramref name="title"/> is already registered.</exception>
		public void RegisterStateGetter(string title, Func<object> stateGetter)
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
		public bool IsRegistered(string title)
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
		public bool UnregisterStateGetter(string title)
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
		public IReadOnlyDictionary<string, object> GetCurrentApplicationStateInfo()
		{
			Dictionary<string, object> state = new Dictionary<string, object>(stateGetters.Count, stateGetters.Comparer);
			lock(stateGetters) {
				foreach (var entry in stateGetters) {
					try {
						state.Add(entry.Key, entry.Value());
					} catch (Exception ex) {
						state.Add(entry.Key, ex);
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
		public void AppendFormatted(StringBuilder sb)
		{
			foreach (KeyValuePair<string, object> entry in GetCurrentApplicationStateInfo()) {
				sb.Append(entry.Key);
				sb.Append(": ");
				
				if (entry.Value == null) {
					sb.AppendLine("<null>");
				} else {
					IFormattable f = entry.Value as IFormattable;
					if (f != null) {
						try {
							sb.AppendLine(f.ToString(null, CultureInfo.InvariantCulture));
						} catch (Exception ex) {
							sb.AppendLine("--> Exception thrown by IFormattable.ToString:");
							sb.AppendLine(ex.ToString());
						}
					} else {
						try {
							sb.AppendLine(entry.Value.ToString());
						} catch (Exception ex) {
							sb.AppendLine("--> Exception thrown by ToString:");
							sb.AppendLine(ex.ToString());
						}
					}
				}
			}
		}
	}
}
