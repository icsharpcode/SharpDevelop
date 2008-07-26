using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Globalization;
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid
{
	public enum PropertyGridTab
	{
		Properties,
		Events
	}

	public class PropertyGrid : INotifyPropertyChanged
	{
		static PropertyGrid()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			new BasicMetadata();
		}

		public PropertyGrid()
		{
			Categories = new ObservableCollection<Category>();
			Events = new ObservableCollection<PropertyNode>();
		}

		static SortedDictionary<string, Category> allCategories =
			new SortedDictionary<string, Category>(CategoryNameComparer.Instance);

		class CategoryNameComparer : IComparer<string>
		{
			public static CategoryNameComparer Instance = new CategoryNameComparer();

			public int Compare(string x, string y)
			{
				int i1 = Array.IndexOf(Metadata.CategoryOrder, x);
				if (i1 == -1) i1 = int.MaxValue;
				int i2 = Array.IndexOf(Metadata.CategoryOrder, y);
				if (i2 == -1) i2 = int.MaxValue;
				if (i1 == i2) return x.CompareTo(y);
				return i1.CompareTo(i2);
			}
		}

		public ObservableCollection<Category> Categories { get; private set; }
		public ObservableCollection<PropertyNode> Events { get; private set; }

		PropertyGridTab currentTab;

		public PropertyGridTab CurrentTab {
			get {
				return currentTab;
			}
			set {
				currentTab = value;
				RaisePropertyChanged("CurrentTab");
			}
		}

		string filter;

		public string Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				Reload();
				RaisePropertyChanged("Filter");
			}
		}

		DesignItem singleItem;

		public DesignItem SingleItem {
			get {
				return singleItem;
			}
			private set {
				singleItem = value;
				RaisePropertyChanged("SingleItem");
			}
		}

		IEnumerable<DesignItem> selectedItems;

		public IEnumerable<DesignItem> SelectedItems {
			get {
				return selectedItems;
			}
			set {
				selectedItems = value;
				Reload();
				RaisePropertyChanged("SelectedItems");
			}
		}

		public void ClearFilter()
		{
			Filter = null;
		}

		void Reload()
		{
			Categories.Clear();
			Events.Clear();
			SingleItem = null;

			foreach (var cat in allCategories.Values) {
				cat.Properties.Clear();
				cat.MoreProperties.Clear();
			}

			if (SelectedItems == null || SelectedItems.Count() == 0) return;

			List<MemberDescriptor> list = new List<MemberDescriptor>();

			if (SelectedItems.Count() == 1) {
				SingleItem = SelectedItems.First();

				foreach (MemberDescriptor d in TypeHelper.GetAvailableProperties(SingleItem.ComponentType)) {
					list.Add(d);
				}
				foreach (MemberDescriptor d in TypeHelper.GetAvailableEvents(SingleItem.ComponentType)) {
					list.Add(d);
				}
			}
			else {
				foreach (MemberDescriptor d in TypeHelper.GetCommonAvailableProperties(SelectedItems.Select(t => t.ComponentType))) {
					list.Add(d);
				}
			}

			var nodeList = list
				.Where(d => PassesFilter(d.Name))
				.OrderBy(d => d.Name)
				.Select(d => new PropertyNode(SelectedItems.Select(t => t.Properties[d.Name]).ToArray()));

			foreach (var node in nodeList) {
				if (node.IsEvent) {
					Events.Add(node);
				}
				else {
					string catName = Metadata.GetCategory(node.FirstProperty) ?? BasicMetadata.Category_Misc;
					Category cat;
					if (!allCategories.TryGetValue(catName, out cat)) {
						cat = new Category(catName);
						allCategories[catName] = cat;
					}
					if (Metadata.IsAdvanced(node.FirstProperty)) {
						cat.MoreProperties.Add(node);
					}
					else {
						cat.Properties.Add(node);
					}
				}
			}

			foreach (var cat in allCategories.Values) {
				if (cat.Properties.Count > 0 || cat.MoreProperties.Count > 0) {
					if (string.IsNullOrEmpty(Filter)) {
						if (cat.ShowMoreByFilter) {
							cat.ShowMore = false;
							cat.ShowMoreByFilter = false;
						}
					}
					else {
						cat.ShowMore = true;
						cat.ShowMoreByFilter = true;
					}
					Categories.Add(cat);
				}
			}
		}

		bool PassesFilter(string name)
		{
			if (string.IsNullOrEmpty(Filter)) return true;
			for (int i = 0; i < name.Length; i++) {
				if (i == 0 || char.IsUpper(name[i])) {
					if (string.Compare(name, i, Filter, 0, Filter.Length, true) == 0) {
						return true;
					}
				}
			}
			return false;
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
