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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public partial class DesignerCanvas : Canvas
    {
    	 static DesignerCanvas()
        {
            ResourceDictionaryLoader.LoadResourceDictionary("/UserControls/DesignerCanvasResourceDictionary.xaml");
        }
        
        public DesignerCanvas(EDMDesignerViewContent container)
        {
            _container = container; 
            AllowDrop = true;
            Loaded += new RoutedEventHandler(DesignerCanvas_Loaded);
            InitContextMenuCommandBindings();
        }

        public EDMView EDMView
        {
            get { return (EDMView)GetValue(EDMViewProperty); }
            set { SetValue(EDMViewProperty, value); }
        }
        
        public static readonly DependencyProperty EDMViewProperty =
            DependencyProperty.Register("EDMView", typeof(EDMView), typeof(DesignerCanvas), new UIPropertyMetadata(null, 
                (sender, e) =>
                {
                    var designerCanvas = (DesignerCanvas)sender;
                    designerCanvas.EDMView.CSDL.TypeDeleted += uiType =>
                {
                    TypeBaseDesigner typeDesigner;
                    if ((typeDesigner = designerCanvas.Children.OfType<TypeBaseDesigner>().FirstOrDefault(tbd => tbd.UIType == uiType)) != null)
                    {
                        designerCanvas.Children.Remove(typeDesigner);
                        foreach (var relationContener in typeDesigner.RelationsContener)
                            designerCanvas.Children.Remove(relationContener);
                    }
                };
            }));

        public DesignerView DesignerView
        {
            get { return (DesignerView)GetValue(DesignerViewProperty); }
            set { SetValue(DesignerViewProperty, value); }
        }
        
        public static readonly DependencyProperty DesignerViewProperty =
            DependencyProperty.Register("DesignerView", typeof(DesignerView), typeof(DesignerCanvas), new UIPropertyMetadata(null, (sender, e) =>
            {
                var designerCanvas = (DesignerCanvas)sender;
                var designerView = (DesignerView)e.NewValue;
                foreach (TypeBaseDesigner typeBaseDesigner in designerView)
                    designerCanvas.Children.Add(typeBaseDesigner);
                //designerCanvas.Loaded +=
                //    delegate
                //    {
                //        VisualHelper.DoEvents();

                //        foreach (TypeBaseDesigner typeBaseDesigner in designerView)
                //            typeBaseDesigner.DrawRelations();
                //        designerCanvas.Zoom = designerView.Zoom;
                //    };
            }));

        private static Dictionary<DesignerView, DesignerCanvas> _designerCanvas = new Dictionary<DesignerView, DesignerCanvas>();
        
        public static DesignerCanvas GetDesignerCanvas(EDMDesignerViewContent container, EDMView edmView, DesignerView designerView)
        {
            DesignerCanvas designerCanvas = null;
                
            //if (designerView == null)
            //{
            //    EntityTypeDesigner.Init = true;
                
            //    designerView = new DesignerView();
            //    designerView.ArrangeTypeDesigners = true;
            //    designerView.Name = edmView.Name;
            //    designerView.Zoom = 100;

            //    if (edmView.CSDL.CSDL != null)
            //    {
            //        foreach (UIEntityType entityType in edmView.CSDL.EntityTypes)
            //        {
            //            designerView.AddTypeDesigner(new EntityTypeDesigner(entityType) { IsExpanded = true });
            //        }
            //    }

            //    EntityTypeDesigner.Init = false;
            //}
            
            if (designerView != null && _designerCanvas.ContainsKey(designerView))
            {
                designerCanvas = _designerCanvas[designerView];
                var parent = designerCanvas.Parent as DesignerCanvasPreview;
                if (parent != null)
                    parent.Content = null;
                else
                    ((ContentControl)designerCanvas.Parent).Content = null;
            }
            else
            {
                designerCanvas = new DesignerCanvas(container) { EDMView = edmView, DesignerView = designerView, Background = Brushes.White };
                _designerCanvas.Add(designerView, designerCanvas);
            }
            
            return designerCanvas;
        }
        public static IEnumerable<DesignerView> DesignerInCache
        {
            get
            {
                return _designerCanvas.Keys;
            }
        }

        private Rectangle _rectangle;
        private Point _startSelectionPoint;
        private bool _isSelecting;

        private Point _moveEntityTypeStartPoint;
        private bool _moveEntityType = false;

        public void AddType(IUIType uiType, Point position)
        {
            AddType(TypeBaseDesigner.Load(uiType), position, uiType);
        }
        public void AddType(UIEntityType uiEntityType, Point position)
        {
            AddType(new EntityTypeDesigner(uiEntityType), position, uiEntityType);
        }
        public void AddType(UIComplexType uiComplexType, Point position)
        {
            AddType(new ComplexTypeDesigner(uiComplexType), position, uiComplexType);
        }
        private void AddType(TypeBaseDesigner typeDesigner, Point position, IUIType uiType)
        {
            Children.Add(typeDesigner);
            Canvas.SetLeft(typeDesigner, position.X);
            Canvas.SetTop(typeDesigner, position.Y);
            DesignerView.AddTypeDesigner(typeDesigner);
            typeDesigner.SizeChanged += delegate { Resize(); };
            typeDesigner.Selected +=
                () =>
                {
                    if (!_isSelecting)
                        UnselectOtherTypes(typeDesigner);
                };
        }

        internal void UnselectOtherTypes(TypeBaseDesigner typeDesigner)
        {
            if (!KeyboardUtil.CtrlDown)
                foreach (var selectedType in TypesSelected.Where(ts => ts != typeDesigner).ToList())
                    selectedType.IsSelected = false;
        }

        public void UnselectAllTypes()
        {
            foreach (var selectedType in TypesSelected.ToList())
                selectedType.IsSelected = false;
        }

        public IEnumerable<TypeBaseDesigner> TypesVisibles
        {
            get
            {
                return Children.OfType<TypeBaseDesigner>();
            }
        }

        public IEnumerable<TypeBaseDesigner> TypesSelected
        {
            get
            {
                return TypesVisibles.Where(tv => tv.IsSelected);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var uiEntityType = e.Data.GetData(typeof(UIEntityType)) as UIEntityType;
            if (uiEntityType != null)
            {
                if (!DesignerView.ContainsEntityType(uiEntityType))
                    AddType(uiEntityType, e.GetPosition(this));
                return;
            }
            var uiComplexType = e.Data.GetData(typeof(UIComplexType)) as UIComplexType;
            if (uiComplexType != null)
            {
                if (!DesignerView.ContainsEntityType(uiComplexType))
                    AddType(uiComplexType, e.GetPosition(this));
            }
        }

        private void Resize()
        {
            var children = Children.OfType<FrameworkElement>().ToArray();
            double xToTranslate;
            double yToTranslate;
            if (children.Any())
            {
                xToTranslate = Math.Min(children.Min(fe => Canvas.GetLeft(fe)), 0.0);
                yToTranslate = Math.Min(children.Min(fe => Canvas.GetTop(fe)), 0.0);
            }
            else
            {
                xToTranslate = 0.0;
                yToTranslate = 0.0;
            }
            if (xToTranslate < 0 && yToTranslate < 0)
                foreach (var typeDesigner in children.OfType<EntityTypeDesigner>())
                {
                    Canvas.SetLeft(typeDesigner, Canvas.GetLeft(typeDesigner) - xToTranslate);
                    Canvas.SetTop(typeDesigner, Canvas.GetTop(typeDesigner) - yToTranslate);
                    typeDesigner.OnMove();
                }
            else if (xToTranslate < 0)
                foreach (var typeDesigner in children.OfType<EntityTypeDesigner>())
                {
                    Canvas.SetLeft(typeDesigner, Canvas.GetLeft(typeDesigner) - xToTranslate);
                    typeDesigner.OnMove();
                }
            else if (yToTranslate < 0)
                foreach (var typeDesigner in children.OfType<EntityTypeDesigner>())
                {
                    Canvas.SetTop(typeDesigner, Canvas.GetTop(typeDesigner) - yToTranslate);
                    typeDesigner.OnMove();
                }

            var scale = (double)Zoom / 100.0;
            var scrollViewer = Parent as ScrollViewer;
            if (scrollViewer != null)
            {
                if (WidthNeed * scale > scrollViewer.ActualWidth)
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    Width = WidthNeed;
                }
                else
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Width = scrollViewer.ActualWidth;
                    if (scale < 1)
                        Width /= scale;
                }
                if (HeightNeed * scale > scrollViewer.ActualHeight)
                {
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    Height = HeightNeed;
                }
                else
                {
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Height = scrollViewer.ActualHeight;
                    if (scale < 1)
                        Height /= scale;
                }
            }
        }
        public double WidthNeed
        {
            get
            {
                double value = 0;
                foreach (var control in Children.OfType<FrameworkElement>())
                {
                    var left = Canvas.GetLeft(control);
                    double maxRight = 0;
                    if (left >= 0)
                        maxRight = left + control.ActualWidth;
                    double right = Canvas.GetRight(control);
                    if (right > maxRight)
                        maxRight = right;
                    if (maxRight > value)
                        value = maxRight;
                }
                return value;
            }
        }
        public double HeightNeed
        {
            get
            {
                double value = 0;
                foreach (var control in Children.OfType<FrameworkElement>())
                {
                    var top = Canvas.GetTop(control);
                    double maxBottom = 0;
                    if (top >= 0)
                        maxBottom = top + control.ActualHeight;
                    double bottom = Canvas.GetBottom(control);
                    if (bottom > maxBottom)
                        maxBottom = bottom;
                    if (maxBottom > value)
                        value = maxBottom;
                }
                return value;
            }
        }

		private void DesignerCanvas_Loaded(object sender, RoutedEventArgs e)
		{            
            if (DesignerView.ArrangeTypeDesigners)
            {
                double left = 20, top = 20;
                double currentRowsMaxHeight = 0;

                EntityTypeDesigner.Init = true;
                EDMDesignerChangeWatcher.Init = true;

                foreach(EntityTypeDesigner entityTypeDesigner in DesignerView.TypeDesignersLocations)
                {
                    entityTypeDesigner.Left = left;
                    entityTypeDesigner.Top = top;
                     
                    if (entityTypeDesigner.ActualHeight > currentRowsMaxHeight)
                        currentRowsMaxHeight = entityTypeDesigner.ActualHeight;
                     
                    left += entityTypeDesigner.ActualWidth + 20;
                     
                    if (left > ActualWidth)
                    {
                        top += currentRowsMaxHeight + 20;
                        left = 20;		 
                    }
                }

                EntityTypeDesigner.Init = false;
                EDMDesignerChangeWatcher.Init = false;
            }

            foreach (TypeBaseDesigner typeBaseDesigner in DesignerView)
            {
                try
                {
                    typeBaseDesigner.DrawRelations();
                }
                catch { }
            }

            (sender as DesignerCanvas).Zoom = DesignerView.Zoom;
		}
        
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            _isSelecting = false;
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            foreach (var relation in VisualTreeHelperUtil.GetControlsDecendant<RelationBase>(this))
                relation.IsSelected = false;

            var typeDesigner = VisualTreeHelperUtil.GetControlAscendant<TypeBaseDesigner>(e.OriginalSource);
            _moveEntityType = typeDesigner != null && VisualTreeHelperUtil.GetControlAscendant<ListView>(e.OriginalSource) == null && VisualTreeHelperUtil.GetControlAscendant<TextBox>(e.OriginalSource) == null;
            if (_moveEntityType)
            {
                _moveEntityTypeStartPoint = e.GetPosition(this);
                Cursor = Cursors.ScrollAll;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && _moveEntityType && TypesSelected.Any())
            {
                var moveEntityTypeNewPosition = e.GetPosition(this);
                var translationVectorX = moveEntityTypeNewPosition.X - _moveEntityTypeStartPoint.X;
                if (translationVectorX < 0)
                {
                    var minX = TypesSelected.Min(ts => Canvas.GetLeft(ts));
                    if (minX < -translationVectorX)
                        translationVectorX = -minX;
                }
                var translationVectorY = moveEntityTypeNewPosition.Y - _moveEntityTypeStartPoint.Y;
                if (translationVectorY < 0)
                {
                    var minY = TypesSelected.Min(ts => Canvas.GetTop(ts));
                    if (minY < -translationVectorY)
                        translationVectorY = -minY;
                }
                foreach (var entityTypeControl in TypesSelected)
                {
                    Canvas.SetLeft(entityTypeControl, Canvas.GetLeft(entityTypeControl) + translationVectorX);
                    Canvas.SetTop(entityTypeControl, Canvas.GetTop(entityTypeControl) + translationVectorY);
                    entityTypeControl.OnMove();
                }
                _moveEntityTypeStartPoint = moveEntityTypeNewPosition;
                Resize();
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (_moveEntityType)
            {
                _moveEntityType = false;
                Cursor = Cursors.Arrow;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (VisualTreeHelperUtil.GetControlAscendant<EntityTypeDesigner>(e.OriginalSource) == null)
            {
                if (UITypeToAdd == null)
                {
                    UnselectAllTypes();
                    _startSelectionPoint = e.GetPosition(this);
                    _isSelecting = true;
                }
                else
                {
                    AddType(UITypeToAdd, e.GetPosition(this));
                    UITypeToAdd = null;
                }
            }
            else
                UITypeToAdd = null;
            Container.Selection = null;
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            UITypeToAdd = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && _isSelecting)
            {
                var rectangleEndPoint = e.GetPosition(this);
                if (_rectangle == null)
                {
                    _rectangle = new Rectangle { Height = Math.Abs(_startSelectionPoint.Y - rectangleEndPoint.Y), Width = Math.Abs(_startSelectionPoint.X - rectangleEndPoint.X), Stroke = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0xF5)), Fill = new SolidColorBrush(Color.FromArgb(0x3F, 0xAD, 0xD8, 0xE6)), StrokeDashArray = new DoubleCollection() { 2 } };
                    Children.Add(_rectangle);
                }
                else
                {
                    _rectangle.Height = Math.Abs(_startSelectionPoint.Y - rectangleEndPoint.Y);
                    _rectangle.Width = Math.Abs(_startSelectionPoint.X - rectangleEndPoint.X);
                }
                Canvas.SetLeft(_rectangle, Math.Min(_startSelectionPoint.X, rectangleEndPoint.X));
                Canvas.SetTop(_rectangle, Math.Min(_startSelectionPoint.Y, rectangleEndPoint.Y));
            }
        }

        public void SelectFromRectangle()
        {
            if (_rectangle != null)
            {
                var rectangleLeft = Canvas.GetLeft(_rectangle);
                var rectangleRight = rectangleLeft + _rectangle.ActualWidth;
                var rectangleTop = Canvas.GetTop(_rectangle);
                var rectangleBottom = rectangleTop + _rectangle.ActualHeight;
                foreach (var entityType in from etc in Children.OfType<TypeBaseDesigner>()
                                           let entityTypeLeft = Canvas.GetLeft(etc)
                                           let entityTypeRight = entityTypeLeft + etc.ActualWidth
                                           let entityTypeTop = Canvas.GetTop(etc)
                                           let entityTypeBottom = entityTypeTop + etc.ActualHeight
                                           where ((entityTypeLeft > rectangleLeft && entityTypeLeft < rectangleRight) || (rectangleLeft > entityTypeLeft && rectangleLeft < entityTypeRight)) && ((entityTypeTop > rectangleTop && entityTypeTop < rectangleBottom) || (rectangleTop > entityTypeTop && rectangleTop < entityTypeBottom))
                                           select etc)
                    entityType.IsSelected = true;
                Children.Remove(_rectangle);
                _rectangle = null;
            }
        }

        public void OnBackSpaceDown()
        {
            DeleteSelectedTypesFromDesigner(false);
            OnSelectionChanged();
        }

        private void DeleteSelectedTypesFromDesigner(bool deleteType)
        {
            foreach (var designerType in TypesSelected.ToList())
            {
                foreach (var relationContener in designerType.RelationsContener.ToList())
                {
                    Children.Remove(relationContener);
                    relationContener.OnRemove();
                }
                Children.Remove(designerType);
                DesignerView.RemoveTypeDesigner(designerType);
                if (deleteType)
                    designerType.UIType.View.DeleteType(designerType.UIType);
            }
        }
        public void DeleteSelectedTypes()
        {
            DeleteSelectedTypesFromDesigner(true);
        }
        public void RemoveFromDesigner()
        {
            DeleteSelectedTypesFromDesigner(false);
        }

        public bool OnDeleteDown(object originalSource)
        {
            if (originalSource is RelationBase)
                return false;
            var value = !(originalSource is ListViewItem);
            if (value)
                DeleteSelectedTypes();
            OnSelectionChanged();
            return value;
        }

        public void OnEscapeDown()
        {
            UITypeToAdd = null;
        }
        private IUIType _uiTypeToAdd;
        public IUIType UITypeToAdd
        {
            get { return _uiTypeToAdd; }
            set
            {
                _uiTypeToAdd = value;
                if (value == null)
                    Cursor = Cursors.Arrow;
                else
                    Cursor = Cursors.Cross;
            }
        }

        internal TypeBaseDesigner GetTypeDesigner(IUIType uiType)
        {
            return Children.OfType<TypeBaseDesigner>().FirstOrDefault(tbd => tbd.UIType == uiType);
        }

        public object Selection
        {
            get
            {
                var enumerator = TypesSelected.GetEnumerator();
                if (!enumerator.MoveNext())
                    return null;
                var typeBaseDesigner = enumerator.Current;
                if (enumerator.MoveNext())
                    return null;
                return typeBaseDesigner.Selection;
            }
        }
        internal void OnSelectionChanged()
        {
            Container.Selection = Selection;
        }
    }
}
