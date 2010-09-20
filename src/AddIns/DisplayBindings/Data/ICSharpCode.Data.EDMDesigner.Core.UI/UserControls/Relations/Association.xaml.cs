// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    public partial class Association : RelationBase
    {
        public Association(ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association.Association csdlAssociation, DesignerCanvas canvas, TypeBaseDesigner fromTypeDesigner, TypeBaseDesigner toTypeDesigner, FrameworkElement fromItem, FrameworkElement toItem, int fromItemIndex, int toItemIndex)
            : base(canvas, fromTypeDesigner, toTypeDesigner)
        {
            CSDLAssociation = csdlAssociation;
            FromItem = fromItem;
            ToItem = toItem;
            FromItemIndex = fromItemIndex;
            ToItemIndex = toItemIndex;
            InitializeComponent();
            InitContextMenuCommandBindings();
            SetBinding(XYProperty, new Binding { Source = this, Converter = new AssociationToXYConverter() });
            SetBinding(OpacityProperty, new Binding("IsCompletlyMapped") { Source = CSDLAssociation.Mapping, Converter = new BoolToOpacityConverter(), ConverterParameter = 0.3 });
        }

        public FrameworkElement FromItem { get; set; }
        public FrameworkElement ToItem { get; set; }

        public int FromItemIndex { get; private set; }
        public int ToItemIndex { get; private set; }

        public bool FourPoints
        {
            get
            {
                return X3 != 0 || X4 != 0;
            }
        }

        private ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association.Association _csdlAssociation;
        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association.Association CSDLAssociation
        {
            get { return _csdlAssociation; }
            private set
            {
                _csdlAssociation = value;
                if (value != null)
                {
                    value.PropertyEnd1.CardinalityChanged += () => OnPropertyChanged("FromCardinality");
                    value.PropertyEnd2.CardinalityChanged += () => OnPropertyChanged("ToCardinality");
                }
            }
        }

        public string FromCardinality
        {
            get { return CardinalityStringConverter.CardinalityToString(CSDLAssociation.PropertyEnd1.Cardinality); }
        }
        public string ToCardinality
        {
            get { return CardinalityStringConverter.CardinalityToString(CSDLAssociation.PropertyEnd2.Cardinality); }
        }

        protected override void OnAssociationCoordonatesChanged()
        {
            base.OnAssociationCoordonatesChanged();
            OnPropertyChanged("FourPoints");
        }

        private void line_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            tb1.GetBindingExpression(TextBlock.HorizontalAlignmentProperty).UpdateTarget();
            tb1.GetBindingExpression(TextBlock.MarginProperty).UpdateTarget();
            tb2.GetBindingExpression(TextBlock.HorizontalAlignmentProperty).UpdateTarget();
            tb2.GetBindingExpression(TextBlock.MarginProperty).UpdateTarget();
            line.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
            line2.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
            line3.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
            selectionRectangle1.GetBindingExpression(Rectangle.MarginProperty).UpdateTarget();
            selectionRectangle2.GetBindingExpression(Rectangle.MarginProperty).UpdateTarget();
        }

        internal override void OnMove()
        {
            if (((EntityType)FromTypeDesigner.UIType.BusinessInstance).NavigationProperties.Intersect(CSDLAssociation.PropertiesEnd).Any())
            {
                base.OnMove();
                LineTargetUpdated(line);
                LineTargetUpdated(line2);
                LineTargetUpdated(line3);
            }
        }

        private void Line_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Canvas.Container.Selection = CSDLAssociation;
            Canvas.UnselectAllTypes();
            e.Handled = true;
        }

        private void AssociationPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.Container.Selection = CSDLAssociation;
            Focus();
            IsSelected = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = true;
        }
    }
}
