using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class ChooseClass : INotifyPropertyChanged
	{
		public ChooseClass(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
			
			AddClassesRecursive("");
			
			projectClasses.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
			
			classes = new ListCollectionView(projectClasses);
			classes.Filter = FilterPredicate;
		}
		
		IProjectContent projectContent;
		List<IClass> projectClasses = new List<IClass>();
		
		void AddClassesRecursive(string ns)
		{
			foreach (var item in projectContent.GetNamespaceContents(ns)) {
				if (item is string) {
					AddClassesRecursive(ns.Length == 0 ? item.ToString() : ns + "." + item);
				} else if (item is IClass) {
					IClass c = item as IClass;
					if (c.IsPartial) {
						if (projectClasses.Contains(c)) continue;
					}
					if (c.ClassType == ClassType.Class && c.IsPublic) {
						projectClasses.Add(c);
					}
				}
			}
		}
		
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
		
		public IClass CurrentClass {
			get { return Classes.CurrentItem as IClass; }
		}
		
		bool FilterPredicate(object item)
		{
			IClass c = item as IClass;
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
