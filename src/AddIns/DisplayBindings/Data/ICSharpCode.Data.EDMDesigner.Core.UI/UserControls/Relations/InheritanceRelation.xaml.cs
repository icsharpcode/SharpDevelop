// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Windows.Data;
using System.Windows.Shapes;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using System;
using System.Windows.Input;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    public partial class InheritanceRelation : RelationBase
    {
        public InheritanceRelation(DesignerCanvas canvas, EntityTypeDesigner fromTypeDesigner, EntityTypeDesigner toTypeDesigner, Func<EntityTypeDesigner> getToEntityTypeDesigner)
            :base(canvas, fromTypeDesigner, toTypeDesigner)
        {
            InitializeComponent();
            InitContextMenuCommandBindings();
            SetBinding(XYProperty, new Binding { Source = this, Converter = new InheritanceRelationToXYConverter() });
            fromTypeDesigner.EntityType.BaseTypeChanged +=
                () =>
                {
                    ToTypeDesigner = getToEntityTypeDesigner();
                    if (ToTypeDesigner != null)
                    {
                        RedoXYBinding();
                        OnAssociationCoordonatesChanged();
                        RedoBindings();
                    }
                };
        }

        public double X1Arrow
        {
            get
            {
                return XY[5][0];
            }
        }
        public double Y1Arrow
        {
            get
            {
                return XY[5][1];
            }
        }

        public double X2Arrow
        {
            get
            {
                return XY[6][0];
            }
        }
        public double Y2Arrow
        {
            get
            {
                return XY[6][1];
            }
        }

        public double X3Arrow
        {
            get
            {
                return XY[7][0];
            }
        }
        public double Y3Arrow
        {
            get
            {
                return XY[7][1];
            }
        }

        public bool FourPoints
        {
            get
            {
                return Y3 != 0 || Y4 != 0;
            }
        }

        protected override void OnAssociationCoordonatesChanged()
        {
            base.OnAssociationCoordonatesChanged();
            OnPropertyChanged("FourPoints");
            OnPropertyChanged("X1Arrow");
            OnPropertyChanged("Y1Arrow");
            OnPropertyChanged("X2Arrow");
            OnPropertyChanged("Y2Arrow");
            OnPropertyChanged("X3Arrow");
            OnPropertyChanged("Y3Arrow");
        }

        private void line_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            line2.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
            line3.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
        }

        internal override void OnMove()
        {
            base.OnMove();
            RedoBindings();
        }

        private void RedoBindings()
        {
            LineTargetUpdated(line);
            LineTargetUpdated(line2);
            LineTargetUpdated(line3);
            LineTargetUpdated(lineArrow1);
            LineTargetUpdated(lineArrow2);
            LineTargetUpdated(lineArrow3);
            selectionRectangle1.GetBindingExpression(Rectangle.MarginProperty).UpdateTarget();
            selectionRectangle2.GetBindingExpression(Rectangle.MarginProperty).UpdateTarget();
        }

        private void InheritanceRelationPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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
