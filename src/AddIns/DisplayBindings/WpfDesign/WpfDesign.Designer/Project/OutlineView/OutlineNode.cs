// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.TreeView;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public class OutlineNode : SharpTreeNode, INotifyPropertyChanged
	{
		//Used to check if element can enter other containers
		public static PlacementType DummyPlacementType;
		
		static OutlineNode()
		{
			DummyPlacementType = PlacementType.Register("DummyPlacement");            
		}

		public static OutlineNode Create(DesignItem designItem)
		{
			OutlineNode node;
			if (!outlineNodes.TryGetValue(designItem, out node)) {
				node = new OutlineNode(designItem);
				outlineNodes[designItem] = node;
			}
			return node;
		}

		//TODO: Reset with DesignContext
		static Dictionary<DesignItem, OutlineNode> outlineNodes = new Dictionary<DesignItem, OutlineNode>();

		OutlineNode(DesignItem designItem)
		{
            PropertyChanged += OutlineNode_PropertyChanged;
		    IsExpanded = true;

            DesignItem = designItem;
            UpdateChildren();

		    var hidden = designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).ValueOnInstance;
		    if (hidden != null && (bool) hidden == true)
		    {
		        this._isDesignTimeVisible = false;
		        ((FrameworkElement) this.DesignItem.Component).Visibility = Visibility.Hidden;
		    }			

			//TODO
			DesignItem.NameChanged += DesignItem_NameChanged;
			DesignItem.PropertyChanged += DesignItem_PropertyChanged;
			SelectionService.SelectionChanged += Selection_SelectionChanged;
		}

		public DesignItem DesignItem { get; private set; }

		public ISelectionService SelectionService {
			get { return DesignItem.Services.Selection; }
		}
       	
	    public override object Text
        {
            get
            {
                return Name;
            }
        }

		public string Name {
			get  {
				if (string.IsNullOrEmpty(DesignItem.Name)) {
					return DesignItem.ComponentType.Name;
				}
				return DesignItem.ComponentType.Name + " (" + DesignItem.Name + ")";
			}
		}

        void OutlineNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                SelectionService.SetSelectedComponents(new[] { DesignItem }, IsSelected ? SelectionTypes.Add : SelectionTypes.Remove);
            }        
        }

        private bool _isDesignTimeVisible = true;
        public bool IsDesignTimeVisible
	    {
	        get { return _isDesignTimeVisible; }
	        set
	        {
	            _isDesignTimeVisible = value;
	            var ctl = DesignItem.Component as UIElement;
                ctl.Visibility = _isDesignTimeVisible ? Visibility.Visible : Visibility.Hidden;

                RaisePropertyChanged("IsDesignTimeVisible");

	            if (!value)
	                DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).SetValue(true);
	                //ctl.SetValue(DesignTimeProperties.IsHiddenProperty, true);
                else
                    DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).Reset();
                    //ctl.ClearValue(DesignTimeProperties.IsHiddenProperty);	            
	        }
	    }

	    void Selection_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
	    {
	        IsSelected = DesignItem.Services.Selection.IsComponentSelected(DesignItem);
	    }

		void DesignItem_NameChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged("Name");
            RaisePropertyChanged("Text");
		}

		void DesignItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == DesignItem.ContentPropertyName) {
				UpdateChildren();
			}
		}

		void UpdateChildren()
		{
			Children.Clear();

			if (DesignItem.ContentPropertyName != null) {
				var content = DesignItem.ContentProperty;
				if (content.IsCollection) {
					UpdateChildrenCore(content.CollectionElements);
				}
				else {
					if (content.Value != null) {
						UpdateChildrenCore(new[] { content.Value });
					}
				}
			}
		}

		void UpdateChildrenCore(IEnumerable<DesignItem> items)
		{
			foreach (var item in items) {
				if (ModelTools.CanSelectComponent(item)) {
					var node = OutlineNode.Create(item);
					Children.Add(node);
				}
			}
		}

        #region Drag Drop

        public override bool CanDrag(SharpTreeNode[] nodes)
        {
            return true;
        }

        public override IDataObject Copy(SharpTreeNode[] nodes)
        {
            var data = new DataObject();
            data.SetData(typeof(OutlineNode[]), nodes.Cast<OutlineNode>().ToArray());
            return data;
        }

        public bool CanInsert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
        {
            var placementBehavior = DesignItem.GetBehavior<IPlacementBehavior>();
            if (placementBehavior == null)
                return false;
            var operation = PlacementOperation.Start(nodes.Select(node => node.DesignItem).ToArray(), DummyPlacementType);
            if (operation != null)
            {
                bool canEnter = placementBehavior.CanEnterContainer(operation);
                operation.Abort();
                return canEnter;
            }
            return false;
        }

	    public override bool CanDelete()
	    {
	        return true;
	    }

        public override void Delete()
        {
            //Todo!
        }

	    public override void DeleteCore()
	    {
	        //Todo!
	    }

	    public override bool CanDrop(DragEventArgs e, int index)
        {
            var copy = (e.KeyStates & DragDropKeyStates.ControlKey) > 0;
            e.Effects = copy ? DragDropEffects.Copy : DragDropEffects.Move;

            var nodes = e.Data.GetData(typeof(OutlineNode[])) as OutlineNode[];

            var placementBehavior = DesignItem.GetBehavior<IPlacementBehavior>();
            if (placementBehavior == null)
                return false;
            var operation = PlacementOperation.Start(nodes.Select(node => node.DesignItem).ToArray(), DummyPlacementType);
            if (operation != null)
            {
                bool canEnter = placementBehavior.CanEnterContainer(operation);
                operation.Abort();
                return canEnter;
            }
            return false;
        }

        public override void Drop(DragEventArgs e, int index)
        {
            var copy = (e.KeyStates & DragDropKeyStates.ControlKey) > 0;
            e.Effects = copy ? DragDropEffects.Copy : DragDropEffects.Move;

            var nodes = (e.Data.GetData(typeof(OutlineNode[])) as OutlineNode[]).ToList();
            
            Insert(nodes, Children[index] as OutlineNode, copy);            
        }

        public void Insert(IEnumerable<OutlineNode> nodes, OutlineNode after, bool copy)
		{
			using (var moveTransaction = DesignItem.Context.OpenGroup("Item moved in outline view", nodes.Select(n => n.DesignItem).ToList())) {
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
        #endregion
        
        ContextMenu menu;
	    public override ContextMenu GetContextMenu()
        {
            if (menu == null)
            {
                menu = new ContextMenu();
                menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut });
                menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy });
                menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste });
                menu.Items.Add(new Separator());
                menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Delete });
            }
            return menu;
        }	
	}
}
