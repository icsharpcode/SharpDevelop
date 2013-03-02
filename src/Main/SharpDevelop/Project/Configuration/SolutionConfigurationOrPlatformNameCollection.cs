// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.Core;
using Microsoft.Build.Logging;

namespace ICSharpCode.SharpDevelop.Project
{
	class SolutionConfigurationOrPlatformNameCollection : ObservableCollection<string>, IConfigurationOrPlatformNameCollection
	{
		readonly Solution solution;
		readonly bool isPlatform;
		
		public SolutionConfigurationOrPlatformNameCollection(Solution solution, bool isPlatform)
		{
			this.solution = solution;
			this.isPlatform = isPlatform;
		}
		
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
			Add(newName);
		}
		
		int GetIndex(string name)
		{
			for (int i = 0; i < this.Count; i++) {
				if (ConfigurationAndPlatform.ConfigurationNameComparer.Equals(this[i], name))
					return i;
			}
			return -1;
		}
		
		void IConfigurationOrPlatformNameCollection.Remove(string name)
		{
			int pos = GetIndex(name);
			if (pos >= 0)
				RemoveAt(pos);
		}
		
		void IConfigurationOrPlatformNameCollection.Rename(string oldName, string newName)
		{
			newName = ValidateName(newName);
			if (newName == null)
				throw new ArgumentException();
			int pos = GetIndex(oldName);
			if (pos < 0)
				throw new ArgumentException();
			foreach (var project in solution.Projects) {
				throw new NotImplementedException();
				//project.ConfigurationMapping.RenameSolutionConfig(oldName, newName, isPlatform);
			}
			this[pos] = newName;
		}
		
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			solution.IsDirty = true;
		}
	}
}
