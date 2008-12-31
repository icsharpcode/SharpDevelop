using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using SharpDevelop.XamlDesigner.Extensibility;
using SharpDevelop.XamlDesigner.Extensibility.Attributes;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public class PropertyGridModel : ViewModel, IHasContext
	{
		internal PropertyGridModel(IHasContext contextHolder)
		{
			this.contextHolder = contextHolder;

			Categories = new ObservableCollection<CategoryModel>(new[] { 
				specialCategory, 
				popularCategory, 
				otherCategory
			});

			Events = new PropertyNodeCollection();
		}

		public ObservableCollection<CategoryModel> Categories { get; private set; }
		public PropertyNodeCollection Events { get; private set; }

		CategoryModel specialCategory = new CategoryModel("Special");
		CategoryModel popularCategory = new CategoryModel("Popular");
		CategoryModel otherCategory = new CategoryModel("Other");

		IHasContext contextHolder;
		HashSet<PropertyNode> selectionNodes = new HashSet<PropertyNode>();
		Dictionary<MemberId, PropertyNode> nodes = new Dictionary<MemberId, PropertyNode>();

		public DesignContext Context
		{
			get { return contextHolder.Context; }
		}

		DesignItem[] selection;

		public DesignItem[] Selection
		{
			get
			{
				return selection;
			}
			set
			{
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate {
					selection = value;
					Reload();
					RaisePropertyChanged("Selection");
				}));				
			}
		}

		DesignItem singleItem;

		public DesignItem SingleItem
		{
			get
			{
				return singleItem;
			}
			set
			{
				singleItem = value;
				RaisePropertyChanged("SingleItem");
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
				UpdateVisibility();
				RaisePropertyChanged("Filter");
			}
		}

		void Reload()
		{
			if (Selection != null && Selection.Length == 1) {
				SingleItem = Selection[0];
			}
			else {
				SingleItem = null;
			}

			foreach (var node in selectionNodes) {
				node.UnbindValue();
			}

			selectionNodes.Clear();

			foreach (var member in GetSelectionMembers()) {
				PropertyNode node;
				if (!nodes.TryGetValue(member, out node)) {
					node = new PropertyNode(this, member);
					if (node.MemberId is EventId) {
						Events.AddSorted(node);
					}
					else {
						node.Category = SelectCategory(node);
						node.Category.Properties.AddSorted(node);
					}
					nodes[member] = node;
				}
				selectionNodes.Add(node);
			}

			foreach (var node in selectionNodes) {
				node.BindValue();
			}

			UpdateVisibility();
		}

		void UpdateVisibility()
		{
			foreach (var c in Categories) {
				var categoryVisibility = false;
				foreach (var p in c.Properties) {
					p.IsVisible = selectionNodes.Contains(p) && PassesFilter(p.MemberId.Name);
					categoryVisibility = categoryVisibility || p.IsVisible;
				}
				c.IsVisible = categoryVisibility;
			}

			foreach (var p in Events) {
				p.IsVisible = selectionNodes.Contains(p) && PassesFilter(p.MemberId.Name);
			}
		}

		IEnumerable<MemberId> GetSelectionMembers()
		{
			if (Selection != null && Selection.Length > 0) {
				if (SingleItem != null) {
					return GetMembers(SingleItem);
				}
				else {
					return GetMembers(Selection[0]).OfType<PropertyId>()
						.Where(property => Selection.All(item => GetMembers(item).Contains(property)))
						.Cast<MemberId>();
				}
			}
			return Enumerable.Empty<MemberId>();
		}

		static IEnumerable<MemberId> GetMembers(DesignItem item)
		{
			return GetMembersCore(item).Where(member => !member.IsReadOnly && member.IsBrowsable);
		}

		static IEnumerable<MemberId> GetMembersCore(DesignItem item)
		{
			if (item.ParentItem != null) {
				return MemberId.GetMembers(item.Type)
					.Concat(MemberId.GetMembers(item.ParentItem.Type))
					.Where(member => member.IsAttachable);
			}
			else {
				return MemberId.GetMembers(item.Type);
			}
		}

		bool PassesFilter(string name)
		{
			return Utils.CamelFilter(name, Filter);
		}

		CategoryModel SelectCategory(PropertyNode node)
		{
			if (MetadataStore.GetAttributes<PopularAttribute>(node.MemberId).Any()) {
				return popularCategory;
			}
			var typeName = node.MemberId.OwnerType.FullName;
			if (typeName.StartsWith("System.Windows.") ||
				typeName.StartsWith("SharpDevelop.XamlDesigner.Controls.")) {
				return otherCategory;
			}
			return specialCategory;
		}
	}
}
