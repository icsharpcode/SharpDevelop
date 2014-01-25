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
