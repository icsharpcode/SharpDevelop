// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    public partial class ComplexPropertyRelation : RelationBase
    {
        public ComplexPropertyRelation(DesignerCanvas canvas, TypeBaseDesigner fromTypeDesigner, TypeBaseDesigner toTypeDesigner, FrameworkElement fromItem, int fromItemIndex)
            : base(canvas, fromTypeDesigner, toTypeDesigner)
        {
            FromItem = fromItem;
            FromItemIndex = fromItemIndex;
            InitializeComponent();
            SetBinding(XYProperty, new Binding { Source = this, Converter = new ComplexPropertyRelationToXYConverter() });
        }

        public FrameworkElement FromItem { get; set; }
        public int FromItemIndex { get; private set; }

        public double X5
        {
            get
            {
                return XY[5][0];
            }
        }
        public double Y5
        {
            get
            {
                return XY[5][1];
            }
        }

        public double X1Arrow
        {
            get
            {
                return XY[6][0];
            }
        }
        public double Y1Arrow
        {
            get
            {
                return XY[6][1];
            }
        }

        public double X2Arrow
        {
            get
            {
                return XY[7][0];
            }
        }
        public double Y2Arrow
        {
            get
            {
                return XY[7][1];
            }
        }
        public double XLast
        {
            get
            {
                return FivePoints ? X5 : ThreePoints ? X3 : X2;
            }
        }
        public double YLast
        {
            get
            {
                return FivePoints ? Y5 : ThreePoints ? Y3 : Y2;
            }
        }

        public bool ThreePoints
        {
            get
            {
                return ! (X3 == 0 || FivePoints);
            }
        }
        public bool FivePoints
        {
            get
            {
                return ! (X4 == 0 && X5 == 0);
            }
        }

        protected override void OnAssociationCoordonatesChanged()
        {
            base.OnAssociationCoordonatesChanged();
            OnPropertyChanged("ThreePoints");
            OnPropertyChanged("FivePoints");
            OnPropertyChanged("X5");
            OnPropertyChanged("Y5");
            OnPropertyChanged("X1Arrow");
            OnPropertyChanged("Y1Arrow");
            OnPropertyChanged("X2Arrow");
            OnPropertyChanged("Y2Arrow");
            OnPropertyChanged("XLast");
            OnPropertyChanged("YLast");
        }

        private void line_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            line2.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
            line3.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
            line4.GetBindingExpression(Line.VisibilityProperty).UpdateTarget();
        }

        internal override void OnMove()
        {
            base.OnMove();
            LineTargetUpdated(line);
            LineTargetUpdated(line2);
            LineTargetUpdated(line3);
            LineTargetUpdated(line4);
            LineTargetUpdated(lineArrow1);
            LineTargetUpdated(lineArrow2);
        }
    }
}
