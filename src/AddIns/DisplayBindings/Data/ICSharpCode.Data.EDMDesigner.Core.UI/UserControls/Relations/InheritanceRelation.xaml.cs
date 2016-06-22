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
