// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	class SolutionConfigurationOrPlatformNameCollection : IConfigurationOrPlatformNameCollection
	{
		public event ModelCollectionChangedEventHandler<string> CollectionChanged;

		readonly List<string> list = new List<string>();
		readonly Solution solution;
		readonly bool isPlatform;
		
		public SolutionConfigurationOrPlatformNameCollection(Solution solution, bool isPlatform)
		{
			this.solution = solution;
			this.isPlatform = isPlatform;
		}
		
		#region IReadOnlyCollection implementation

		public int Count {
			get { return list.Count; }
		}

		public IEnumerator<string> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
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
			newName = ValidateName(newName);
			if (newName == null)
				throw new ArgumentException();
			list.Add(newName);
			if (copyFrom != null)
				throw new NotImplementedException();
			OnCollectionChanged(EmptyList<string>.Instance, new[] { newName });
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
			int pos = GetIndex(name);
			if (pos >= 0) {
				name = list[pos]; // get the name in original case
				list.RemoveAt(pos);
				OnCollectionChanged(new[] { name }, EmptyList<string>.Instance);
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
			foreach (var project in solution.Projects) {
				throw new NotImplementedException();
				//project.ConfigurationMapping.RenameSolutionConfig(oldName, newName, isPlatform);
			}
			list[pos] = newName;
			OnCollectionChanged(new[] { oldName }, new[] { newName });
		}
		
		void OnCollectionChanged(IReadOnlyCollection<string> oldItems, IReadOnlyCollection<string> newItems)
		{
			if (CollectionChanged != null)
				CollectionChanged(oldItems, newItems);
			solution.IsDirty = true;
		}
	}
}
