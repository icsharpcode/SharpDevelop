/*
 * Created by SharpDevelop.
 * User: trubra
 * Date: 2014-01-28
 * Time: 10:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	/// <summary>
	/// Description of OutlineNodeBase.
	/// </summary>
	public abstract class OutlineNodeBase : INotifyPropertyChanged, IOutlineNode
	{

		protected abstract void UpdateChildren();
		//Used to check if element can enter other containers
		protected static PlacementType DummyPlacementType;

		protected OutlineNodeBase(DesignItem designItem)
		{
			DesignItem = designItem;

			bool hidden = false;
			try
			{
				hidden = (bool)designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).ValueOnInstance;
			}
			catch (Exception)
			{ }
			if (hidden) {
				_isDesignTimeVisible = false;
			}

			bool locked = false;
			try
			{
				locked = (bool)designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).ValueOnInstance;
			}
			catch (Exception)
			{ }
			if (locked) {
				_isDesignTimeLocked = true;
			}

			//TODO

			DesignItem.NameChanged += new EventHandler(DesignItem_NameChanged);
			DesignItem.PropertyChanged += new PropertyChangedEventHandler(DesignItem_PropertyChanged);
		}

		public DesignItem DesignItem { get; set; }

		public ISelectionService SelectionService
		{
			get { return DesignItem.Services.Selection; }
		}

		bool isExpanded = true;

		public bool IsExpanded
		{
			get
			{
				return isExpanded;
			}
			set
			{
				isExpanded = value;
				RaisePropertyChanged("IsExpanded");
			}
		}

		bool isSelected;

		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				if (isSelected != value) {
					isSelected = value;
					SelectionService.SetSelectedComponents(new[] { DesignItem },
					                                       value ? SelectionTypes.Add : SelectionTypes.Remove);
					RaisePropertyChanged("IsSelected");
				}
			}
		}

		bool _isDesignTimeVisible = true;

		public bool IsDesignTimeVisible
		{
			get
			{
				return _isDesignTimeVisible;
			}
			set
			{
				_isDesignTimeVisible = value;

				RaisePropertyChanged("IsDesignTimeVisible");

				if (!value)
					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).SetValue(true);
				else
					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).Reset();
			}
		}

		bool _isDesignTimeLocked = false;

		public bool IsDesignTimeLocked
		{
			get
			{
				return _isDesignTimeLocked;
			}
			set
			{
				_isDesignTimeLocked = value;
				((XamlDesignItem)DesignItem).IsDesignTimeLocked = _isDesignTimeLocked;

				RaisePropertyChanged("IsDesignTimeLocked");

				//				if (value)
				//					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).SetValue(true);
				//				else
				//					DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).Reset();
			}
		}

		ObservableCollection<IOutlineNode> children = new ObservableCollection<IOutlineNode>();

		public ObservableCollection<IOutlineNode> Children
		{
			get { return children; }
		}

		public string Name
		{
			get
			{
				return DesignItem.Services.GetService<IOutlineNodeNameService>().GetOutlineNodeName(DesignItem);
			}
		}

		void DesignItem_NameChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("Name");
		}

		void DesignItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == DesignItem.ContentPropertyName) {
				UpdateChildren();
			}
		}

		

		public bool CanInsert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy)
		{
			var placementBehavior = DesignItem.GetBehavior<IPlacementBehavior>();
			if (placementBehavior == null)
				return false;
			var operation = PlacementOperation.Start(nodes.Select(node => node.DesignItem).ToArray(), DummyPlacementType);
			if (operation != null) {
				bool canEnter = placementBehavior.CanEnterContainer(operation, true);
				operation.Abort();
				return canEnter;
			}
			return false;
		}

		public virtual void Insert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy)
		{
			using (var moveTransaction = DesignItem.Context.OpenGroup("Item moved in outline view", nodes.Select(n => n.DesignItem).ToList()))
			{
				if (copy) {
					nodes = nodes.Select(n => OutlineNode.Create(n.DesignItem.Clone())).ToList();
				} else {
					foreach (var node in nodes) {
						node.DesignItem.Remove();
					}
				}

				var index = after == null ? 0 : Children.IndexOf(after) + 1;

				var content = DesignItem.ContentProperty;
				if (content.IsCollection) {
					foreach (var node in nodes) {
						content.CollectionElements.Insert(index++, node.DesignItem);
					}
				} else {
					content.SetValue(nodes.First().DesignItem);
				}
				moveTransaction.Commit();
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
