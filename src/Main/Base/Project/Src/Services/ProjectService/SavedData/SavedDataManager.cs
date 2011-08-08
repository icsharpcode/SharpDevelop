// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project.SavedData
{
	/// <summary>
	/// Stores the project specific data.
	/// </summary>
	public static class SavedDataManager
	{
		static readonly List<IProjectSavedData> list;
		static object _syncObject = new object();
		
		static SavedDataManager()
		{
			list = new List<IProjectSavedData>();
			ProjectService.SolutionClosed += delegate { list.Clear(); };
		}
		
		/// <summary>
		/// Gets all saved data.
		/// </summary>
		/// <returns></returns>
		public static List<IProjectSavedData> GetSavedData()
		{
			return list;
		}
		
		/// <summary>
		/// Adds a new data.
		/// </summary>
		/// <param name="data"></param>
		public static void Add(IProjectSavedData data)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			
			if (!list.Contains(data)) {
				lock (_syncObject) {
					if (!list.Contains(data))
						list.Add(data);
				}
			}
		}
		
		/// <summary>
		/// Removes data.
		/// </summary>
		/// <param name="data"></param>
		public static void Remove(IProjectSavedData data)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			
			if (list.Contains(data)) {
				lock (_syncObject) {
					if (list.Contains(data))
						list.Remove(data);
				}
			}
		}
		
		/// <summary>
		/// Removes all data that satisfies a predicate.
		/// </summary>
		/// <param name="match"></param>
		public static void RemoveAll(Predicate<IProjectSavedData> match)
		{
			if (match == null)
				throw new ArgumentNullException("match");
			
			lock (_syncObject) {
				list.RemoveAll(match);
			}
		}
	}
}
