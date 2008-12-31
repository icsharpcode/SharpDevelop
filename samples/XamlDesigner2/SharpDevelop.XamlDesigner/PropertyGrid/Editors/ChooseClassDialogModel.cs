using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public class ChooseClassDialogModel : ViewModel
	{
		public ChooseClassDialogModel(IEnumerable<Type> types)
		{
			ClassesView = CollectionViewSource.GetDefaultView(types);
			ClassesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
			ClassesView.Filter = FilterPredicate;
		}

		public ICollectionView ClassesView { get; private set; }

		Type selectedClass;

		public Type SelectedClass
		{
			get
			{
				return selectedClass;
			}
			set
			{
				selectedClass = value;
				RaisePropertyChanged("SelectedClass");
			}
		}

		string filter;

		public string Filter
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
				ClassesView.Refresh();
				RaisePropertyChanged("Filter");
			}
		}

		bool showSystemClasses;

		public bool ShowSystemClasses
		{
			get
			{
				return showSystemClasses;
			}
			set
			{
				showSystemClasses = value;
				ClassesView.Refresh();
				RaisePropertyChanged("ShowSystemClasses");
			}
		}

		Type baseClass;

		public Type BaseClass
		{
			get
			{
				return baseClass;
			}
			set
			{
				baseClass = value;
				RaisePropertyChanged("BaseType");

				if (IsSytemClass(baseClass) && baseClass != typeof(object)) {
					ShowSystemClasses = true;
				}
			}
		}

		bool FilterPredicate(object item)
		{
			Type type = item as Type;
			if (!ShowSystemClasses && IsSytemClass(type)) {
				return false;
			}
			if (BaseClass == typeof(ICommand) && type == typeof(RoutedCommand)) {
			}
			if (BaseClass != null) {
				if (!BaseClass.IsAssignableFrom(type)) {
					return false;
				}
			}
			return Utils.CamelFilter(type.Name, Filter);
		}

		static bool IsSytemClass(Type type)
		{
			return
				type.FullName.StartsWith("System.") ||
				type.FullName.StartsWith("Microsoft.") ||
				type.FullName.StartsWith("XamlGeneratedNamespace.");
		}
	}
}
