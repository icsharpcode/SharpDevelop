// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;
using ICSharpCode.Data.Core.UI;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType
{
    public abstract partial class TypeBaseDesigner : UserControl, INotifyPropertyChanged, ITypeDesigner
    {
        protected TypeBaseDesigner(IUIType uiType)
        {
            InitializeComponent();
            UIType = uiType;
            DataContext = UIType;
            InitContextMenuCommandBindings();
            entityTypeExpander.PreviewMouseDown +=
                (sender, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed && (MoreThanOneTypeSelected || ! (VisualTreeHelperUtil.GetControlAscendant<ListView>(e.OriginalSource) == propertiesListView)))
                        SelectOrUnselect();
                    else if (!IsSelected)
                        IsSelected = true;
                };
            if (!Init)
            {
                SizeChangedEventHandler sizeChangedHandler = null;
                sizeChangedHandler = delegate
                {
                    DrawRelations();
                    SizeChanged -= sizeChangedHandler;
                };
                SizeChanged += sizeChangedHandler;
            }
            SizeChanged += delegate { OnMove(); };
            uiType.RelatedPropertyDeleted += uiRelatedProperty =>
                {
                    if (RelationsContenerByRelatedProperty.ContainsKey(uiRelatedProperty))
                    {
                        Designer.Children.Remove(RelationsContenerByRelatedProperty[uiRelatedProperty]);
                        RelationsContenerByRelatedProperty.Remove(uiRelatedProperty);
                    }
                };
            var uiEntityType = uiType as UIEntityType;
            if (uiEntityType != null)
                uiEntityType.AbstractChanged += delegate { entityTypeExpander.GetBindingExpression(TypeBaseExpander.BorderBrushProperty).UpdateTarget(); };
        }

        public static bool Init { get; set; }

        protected internal DesignerCanvas Designer
        {
            get
            {
                return (DesignerCanvas)Parent;
            }
        }

        protected internal virtual void DrawRelations()
        {
            if (UIType is UIEntityType)
                foreach (var relatedProperty in UIType.Properties.OfType<UIRelatedProperty>().Where(uirp => !RelationsContenerByRelatedProperty.ContainsKey(uirp) && Designer.DesignerView.ContainsEntityType(uirp.RelatedType)))
                    DrawRelation(relatedProperty);
            else if (UIType is UIComplexType)
            {
                foreach (var relatedProperty in Designer.DesignerView.SelectMany(dv => dv.UIType.Properties).OfType<UIRelatedProperty>().Where(uirp => uirp.RelatedType == UIType && !RelationsContenerByRelatedProperty.ContainsKey(uirp)))
                {
                    var otherType = Designer.Children.OfType<TypeBaseDesigner>().FirstOrDefault(td => td.UIType.Properties.Contains(relatedProperty));
                    if (otherType != null)
                        otherType.DrawRelation(relatedProperty);
                }
            }
        }
        protected virtual void DrawRelation(UIRelatedProperty relatedProperty)
        {
            var otherTypeDesigner = GetRelatedTypeDesigner(relatedProperty);

            var navigationProperty = (relatedProperty.BusinessInstance as NavigationProperty);
            if (navigationProperty != null)
                DrawAssociation(otherTypeDesigner, navigationProperty);

            if (relatedProperty.BusinessInstance is ComplexProperty)
                DrawComplexPropertyRelation(otherTypeDesigner, relatedProperty);
        }

        internal void DrawAssociation(TypeBaseDesigner otherTypeDesigner, NavigationProperty navigationProperty)
        {
            var csdlAssociation = navigationProperty.Association;
            int otherTypeDesignerItemIndex;
            var otherNavigationProperty = csdlAssociation.PropertiesEnd.First(np => np != navigationProperty);
            var otherListViewItem = otherTypeDesigner.GetListViewItem(otherNavigationProperty, out otherTypeDesignerItemIndex);
            int typeDesignerItemIndex;
            var typeDesignerListViewItem = GetListViewItem(navigationProperty, out typeDesignerItemIndex);
            ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations.Association association;
            if (csdlAssociation.PropertyEnd1 == navigationProperty)
                association = new ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations.Association(csdlAssociation, Designer, this, otherTypeDesigner, typeDesignerListViewItem, otherListViewItem, typeDesignerItemIndex, otherTypeDesignerItemIndex);
            else
                association = new ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations.Association(csdlAssociation, Designer, otherTypeDesigner, this, otherListViewItem, typeDesignerListViewItem, otherTypeDesignerItemIndex, typeDesignerItemIndex);
            var relationContener = new RelationContener(association);
            Designer.Children.Add(relationContener);
            relationContener.SetBinding(Canvas.LeftProperty, new Binding { Source = association, Path = new PropertyPath("CanvasLeft") });
            relationContener.SetBinding(Canvas.TopProperty, new Binding { Source = association, Path = new PropertyPath("CanvasTop") });
            AddRelationContenerByRelatedProperty(UIType.Properties[navigationProperty], relationContener);
            otherTypeDesigner.AddRelationContenerByRelatedProperty(otherTypeDesigner.UIType.Properties[otherNavigationProperty], relationContener);
            NotifyCollectionChangedEventHandler mappingCollectionChanged = delegate
            {
                VisualTreeHelperUtil.GetControlsDecendant<EditableTextBlock>(otherListViewItem).First().GetBindingExpression(EditableTextBlock.OpacityProperty).UpdateTarget();
                VisualTreeHelperUtil.GetControlsDecendant<EditableTextBlock>(typeDesignerListViewItem).First().GetBindingExpression(EditableTextBlock.OpacityProperty).UpdateTarget();
            };
            csdlAssociation.PropertyEnd1.Mapping.CollectionChanged += mappingCollectionChanged;
            csdlAssociation.PropertyEnd2.Mapping.CollectionChanged += mappingCollectionChanged;
        }

        private TypeBaseDesigner GetRelatedTypeDesigner(UIRelatedProperty relatedProperty)
        {
            return GetTypeDesigner(relatedProperty.RelatedType);
        }
        private TypeBaseDesigner GetTypeDesigner(IUIType uiType)
        {
            return Designer.GetTypeDesigner(uiType);
        }

        internal void DrawComplexPropertyRelation(TypeBaseDesigner otherTypeDesigner, UIRelatedProperty relatedProperty)
        {
            if (RelationsContenerByRelatedProperty.ContainsKey(relatedProperty) || otherTypeDesigner.RelationsContenerByRelatedProperty.ContainsKey(relatedProperty))
                return;
            var complexProperty = (ComplexProperty)relatedProperty.BusinessInstance;
            int typeDesignerItemIndex;
            var complexPropertyRelation = new ComplexPropertyRelation(Designer, this, otherTypeDesigner, GetListViewItem(complexProperty, out typeDesignerItemIndex), typeDesignerItemIndex);
            var relationContener = new RelationContener(complexPropertyRelation);
            Designer.Children.Add(relationContener);
            relationContener.SetBinding(Canvas.LeftProperty, new Binding { Source = complexPropertyRelation, Path = new PropertyPath("CanvasLeft") });
            relationContener.SetBinding(Canvas.TopProperty, new Binding { Source = complexPropertyRelation, Path = new PropertyPath("CanvasTop") });
            AddRelationContenerByRelatedProperty(UIType.Properties[complexProperty], relationContener);
            otherTypeDesigner.AddRelationContenerWithoutRelatedProperty(relationContener);
        }

        private Dictionary<UIRelatedProperty, RelationContener> _relationsContenerByRelatedProperty;
        private Dictionary<UIRelatedProperty, RelationContener> RelationsContenerByRelatedProperty
        {
            get
            {
                if (_relationsContenerByRelatedProperty == null)
                    _relationsContenerByRelatedProperty = new Dictionary<UIRelatedProperty, RelationContener>();
                return _relationsContenerByRelatedProperty;
            }
        }
        protected void AddRelationContenerByRelatedProperty(UIProperty uiProperty, RelationContener relationContener)
        {
            var uiRelatedProperty = uiProperty as UIRelatedProperty;
            if (!(uiRelatedProperty == null || RelationsContenerByRelatedProperty.ContainsKey(uiRelatedProperty)))
            {
                RelationsContenerByRelatedProperty.Add(uiRelatedProperty, relationContener);
                relationContener.Removed += () => RelationsContenerByRelatedProperty.Remove(uiRelatedProperty);
            }
        }
        private List<RelationContener> _relationsContenerWithoutRelatedProperty;
        private List<RelationContener> RelationsContenerWithoutRelatedProperty
        {
            get
            {
                if (_relationsContenerWithoutRelatedProperty == null)
                    _relationsContenerWithoutRelatedProperty = new List<RelationContener>();
                return _relationsContenerWithoutRelatedProperty;
            }
        }
        protected void AddRelationContenerWithoutRelatedProperty(RelationContener relationContener)
        {
            if (!(relationContener == null || RelationsContenerWithoutRelatedProperty.Contains(relationContener)))
            {
                RelationsContenerWithoutRelatedProperty.Add(relationContener);
                relationContener.Removed += () => RelationsContenerWithoutRelatedProperty.Remove(relationContener);
            }
        }
        public IEnumerable<RelationContener> RelationsContener
        {
            get
            {
                return RelationsContenerByRelatedProperty.Values.Union(RelationsContenerWithoutRelatedProperty).Distinct();
            }
        }

        internal ListViewItem GetListViewItem(NavigationProperty navigationProperty, out int index)
        {
            foreach (ListViewItem lvia in VisualTreeHelperUtil.GetControlsDecendant<ListViewItem>(propertiesListView))
            {
                lvia.ToString();
            }
            
            var value = (from lvi in VisualTreeHelperUtil.GetControlsDecendant<ListViewItem>(propertiesListView)
                         let uiRelatedProperty = lvi.Content as UIRelatedProperty
                         where uiRelatedProperty != null
                         select new { ListViewItem = lvi, UIRelatedProperty = uiRelatedProperty }).Select((lvi, i) => new { ListViewItem = lvi, Index = i }).First(lvi => lvi.ListViewItem.UIRelatedProperty.BusinessInstance == navigationProperty);
            index = value.Index; // +navigationProperty.EntityType.ComplexProperties.Count;
            return value.ListViewItem.ListViewItem;
        }
        internal ListViewItem GetListViewItem(ComplexProperty complexProperty, out int index)
        {
            var value = (from lvi in VisualTreeHelperUtil.GetControlsDecendant<ListViewItem>(propertiesListView)
                         let uiRelatedProperty = lvi.Content as UIRelatedProperty
                         where uiRelatedProperty != null
                         select new { ListViewItem = lvi, UIRelatedProperty = uiRelatedProperty }).Select((lvi, i) => new { ListViewItem = lvi, Index = i }).First(lvi => lvi.ListViewItem.UIRelatedProperty.BusinessInstance == complexProperty);
            index = value.Index;
            return value.ListViewItem.ListViewItem;
        }
        internal ListViewItem GetListViewItem(PropertyBase property)
        {
            return (from lvi in VisualTreeHelperUtil.GetControlsDecendant<ListViewItem>(propertiesListView)
                    let uiProperty = lvi.Content as UIProperty
                    where uiProperty != null && uiProperty.BusinessInstance == property
                    select lvi).First();
        }

        internal void OnMove()
        {
            foreach (var relationContener in RelationsContener)
                if (Designer.DesignerView.ContainsEntityType(relationContener.FromTypeDesigner.UIType) && Designer.DesignerView.ContainsEntityType(relationContener.ToTypeDesigner.UIType))
                    relationContener.OnMove();
        }

        private void SelectOrUnselect()
        {
            if (KeyboardUtil.CtrlDown)
                IsSelected ^= true;
            else if (!IsSelected)
                IsSelected = true;
        }

        public IUIType UIType { get; private set; }

        public static TypeBaseDesigner Load(IUIType uiType)
        {
            var uiEntityType = uiType as UIEntityType;
            if (uiEntityType != null)
                return new EntityTypeDesigner(uiEntityType);
            var uiComplexType = uiType as UIComplexType;
            if (uiComplexType != null)
                return new ComplexTypeDesigner(uiComplexType);
            throw new NotImplementedException();
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                if (value)
                    OnSelected();
            }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TypeBaseDesigner), new UIPropertyMetadata(false, (sender, e) => ((TypeBaseDesigner)sender).Designer.OnSelectionChanged()));

        protected virtual void OnSelected()
        {
            if (Selected != null)
                Selected();
        }

        public event Action Selected;

        public bool IsExpanded
        {
            get
            {
                return entityTypeExpander.IsExpanded;
            }
            set
            {
                entityTypeExpander.IsExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        private void EntityTypeExpander_Click()
        {
            Focus();
        }

        private void EntityTypeExpander_CollapsedOrExpand(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("IsExpanded");
        }

        public object Selection
        {
            get { return PropertiesTreeViewFocused && propertiesListView.SelectedValue != null ? (object)((UIProperty)propertiesListView.SelectedValue).BusinessInstance : (object)UIType.BusinessInstance; }
        }
        private void PropertiesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Designer != null)
                Designer.OnSelectionChanged();
        }
        private void PropertiesListView_GotFocus(object sender, RoutedEventArgs e)
        {
            if (propertiesListView.SelectedValue != null)
                Designer.OnSelectionChanged();
        }
        private void PropertiesListView_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Designer.IsFocused || VisualTreeHelperUtil.GetFocusedElement(Designer) != null)
                Designer.OnSelectionChanged();
        }
        private void TypeBaseDesigner_GotFocus(object sender, RoutedEventArgs e)
        {
            Designer.OnSelectionChanged();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            EDMDesignerChangeWatcher.ObjectChanged(this);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        IUIType ITypeDesigner.UIType
        {
            get { return UIType; }
            set { UIType = value; }
        }

        public double Left
        {
            get { return Canvas.GetLeft(this); }
            set 
            { 
                Canvas.SetLeft(this, value);
                OnPropertyChanged("Left");
            }
        }
        public double Top
        {
            get { return Canvas.GetTop(this); }
            set 
            { 
                Canvas.SetTop(this, value);
                OnPropertyChanged("Top");
            }
        }
    }
}
