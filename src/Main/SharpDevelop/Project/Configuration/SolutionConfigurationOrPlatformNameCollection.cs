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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Build.Logging;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Implementation for the <see cref="ISolution.ConfigurationNames"/> and <see cref="ISolution.PlatformNames"/> lists.
	/// </summary>
	/// <remarks>
	/// This class provides thread-safe read access even if there is a concurrent write operation.
	/// However, multiple concurrent writes are not supported.
	/// The intended use is that only the main thread will write to this collection; but background threads may read at any time.
	/// </remarks>
	class SolutionConfigurationOrPlatformNameCollection : IConfigurationOrPlatformNameCollection
	{
		readonly ModelCollectionChangedEvent<string> collectionChangedEvent;
		readonly List<string> list = new List<string>();
		volatile IReadOnlyList<string> listSnapshot = EmptyList<string>.Instance;
		readonly ISolution solution;
		readonly bool isPlatform;
		
		public SolutionConfigurationOrPlatformNameCollection(ISolution solution, bool isPlatform)
		{
			this.solution = solution;
			this.isPlatform = isPlatform;
			collectionChangedEvent = new ModelCollectionChangedEvent<string>();
		}
		
		void OnCollectionChanged(IReadOnlyCollection<string> oldItems, IReadOnlyCollection<string> newItems)
		{
			this.listSnapshot = list.ToArray();
			collectionChangedEvent.Fire(oldItems, newItems);
		}
		
		public event ModelCollectionChangedEventHandler<string> CollectionChanged
		{
			add {
				collectionChangedEvent.AddHandler(value);
			}
			remove {
				collectionChangedEvent.RemoveHandler(value);
			}
		}
		
		#region IReadOnlyCollection implementation
		
		public IReadOnlyCollection<string> CreateSnapshot()
		{
			return listSnapshot;
		}
		
		public int Count {
			get {
				return listSnapshot.Count;
			}
		}
		
		public IEnumerator<string> GetEnumerator()
		{
			return listSnapshot.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return listSnapshot.GetEnumerator();
		}
		
		#endregion
		
		public string ValidateName(string name)
		{
			if (name == null)
				return null;
			name = name.Trim();
			if (!ConfigurationAndPlatform.IsValidName(name))
				return null;
			if (isPlatform)
				return MSBuildInternals.FixPlatformNameForSolution(name);
			else
				return name;
		}

		void IConfigurationOrPlatformNameCollection.Add(string newName, string copyFrom)
		{
			SD.MainThread.VerifyAccess();
			newName = ValidateName(newName);
			if (newName == null)
				throw new ArgumentException();
			list.Add(newName);
			OnCollectionChanged(EmptyList<string>.Instance, new[] { newName });
			
			if (copyFrom != null) {
				foreach (var project in solution.Projects) {
					var mapping = project.ConfigurationMapping;
					foreach (string otherName in isPlatform ? solution.ConfigurationNames : solution.PlatformNames) {
						var sourceSolutionConfig = isPlatform ? new ConfigurationAndPlatform(otherName, copyFrom) : new ConfigurationAndPlatform(copyFrom, otherName);
						var newSolutionConfig = isPlatform ? new ConfigurationAndPlatform(otherName, newName) : new ConfigurationAndPlatform(newName, otherName);
						mapping.CopySolutionConfiguration(sourceSolutionConfig, newSolutionConfig);
					}
				}
			}
		}
		
		int GetIndex(string name)
		{
			for (int i = 0; i < list.Count; i++) {
				if (ConfigurationAndPlatform.ConfigurationNameComparer.Equals(list[i], name))
					return i;
			}
			return -1;
		}
		
		void IConfigurationOrPlatformNameCollection.Remove(string name)
		{
			SD.MainThread.VerifyAccess();
			int pos = GetIndex(name);
			if (pos < 0)
				return;
			name = list[pos]; // get the name in original case
			list.RemoveAt(pos);
			OnCollectionChanged(new[] { name }, EmptyList<string>.Instance);
			
			foreach (var project in solution.Projects) {
				var mapping = project.ConfigurationMapping;
				foreach (string otherName in isPlatform ? solution.ConfigurationNames : solution.PlatformNames) {
					var oldSolutionConfig = isPlatform ? new ConfigurationAndPlatform(otherName, name) : new ConfigurationAndPlatform(name, otherName);
					mapping.Remove(oldSolutionConfig);
				}
			}
		}
		
		void IConfigurationOrPlatformNameCollection.Rename(string oldName, string newName)
		{
			newName = ValidateName(newName);
			if (newName == null)
				throw new ArgumentException();
			int pos = GetIndex(oldName);
			if (pos < 0)
				throw new ArgumentException();
			oldName = list[pos]; // get oldName in original case
			list[pos] = newName;
			listSnapshot = null;
			OnCollectionChanged(new[] { oldName }, new[] { newName });
			
			foreach (var project in solution.Projects) {
				var mapping = project.ConfigurationMapping;
				foreach (string otherName in isPlatform ? solution.ConfigurationNames : solution.PlatformNames) {
					var oldSolutionConfig = isPlatform ? new ConfigurationAndPlatform(otherName, oldName) : new ConfigurationAndPlatform(oldName, otherName);
					var newSolutionConfig = isPlatform ? new ConfigurationAndPlatform(otherName, newName) : new ConfigurationAndPlatform(newName, otherName);
					mapping.RenameSolutionConfiguration(oldSolutionConfig, newSolutionConfig);
				}
			}
		}
	}
}
