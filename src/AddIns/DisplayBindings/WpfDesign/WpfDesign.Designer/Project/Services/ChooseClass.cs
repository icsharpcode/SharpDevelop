// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.IO;
using System.Reflection;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	public class ChooseClass : INotifyPropertyChanged
	{
		public ChooseClass(IEnumerable<Assembly> assemblies)
		{
			foreach (var a in assemblies) {
				foreach (var t in a.GetExportedTypes()) {
					if (t.IsClass) {
						if (t.IsAbstract) continue;
						if (t.IsNested) continue;
						if (t.IsGenericTypeDefinition) continue;
						if (t.GetConstructor(Type.EmptyTypes) == null) continue;
						projectClasses.Add(t);
					}
				}
			}

			projectClasses.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
			classes = new ListCollectionView(projectClasses);
			classes.Filter = FilterPredicate;
		}

		List<Type> projectClasses = new List<Type>();		

		ListCollectionView classes;

		public ICollectionView Classes {
			get { return classes; }
		}

		string filter;

		public string Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				Classes.Refresh();
				RaisePropertyChanged("Filter");
			}
		}

		bool showSystemClasses;

		public bool ShowSystemClasses {
			get {
				return showSystemClasses;
			}
			set {
				showSystemClasses = value;
				Classes.Refresh();
				RaisePropertyChanged("ShowSystemClasses");
			}
		}

		public Type CurrentClass {
			get { return Classes.CurrentItem as Type; }
		}

		bool FilterPredicate(object item)
		{
			Type c = item as Type;
			if (!ShowSystemClasses) {
				if (c.Namespace.StartsWith("System") || c.Namespace.StartsWith("Microsoft")) {
					return false;
				}
			}
			return Match(c.Name, Filter);
		}

		static bool Match(string className, string filter)
		{
			if (string.IsNullOrEmpty(filter))
				return true;
			else
				return className.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase);
		}

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
		#endregion
	}
}
