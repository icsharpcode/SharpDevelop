// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using System.Windows.Shapes;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    public abstract class RelationBase : UserControl, INotifyPropertyChanged
    {
        public RelationBase(DesignerCanvas canvas, TypeBaseDesigner fromTypeDesigner, TypeBaseDesigner toTypeDesigner)
        {
            Canvas = canvas;
            FromTypeDesigner = fromTypeDesigner;
            ToTypeDesigner = toTypeDesigner;
        }

        public DesignerCanvas Canvas { get; private set; }

        public TypeBaseDesigner FromTypeDesigner { get; private set; }
        public TypeBaseDesigner ToTypeDesigner { get; protected set; }

        public double CanvasLeft
        {
            get
            {
                return XY[0][0];
            }
        }
        public double CanvasTop
        {
            get
            {
                return XY[0][1];
            }
        }
        public double X1
        {
            get
            {
                return XY[1][0];
            }
        }
        public double Y1
        {
            get
            {
                return XY[1][1];
            }
        }
        public double X2
        {
            get
            {
                return XY[2][0];
            }
        }
        public double Y2
        {
            get
            {
                return XY[2][1];
            }
        }
        public double X3
        {
            get
            {
                return XY[3][0];
            }
        }
        public double Y3
        {
            get
            {
                return XY[3][1];
            }
        }
        public double X4
        {
            get
            {
                return XY[4][0];
            }
        }
        public double Y4
        {
            get
            {
                return XY[4][1];
            }
        }

        private bool _isSelected;
        public bool IsSelected 
        {
            get { return _isSelected; }
            set 
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public double[][] XY
        {
            get { return (double[][])GetValue(XYProperty); }
            set { SetValue(XYProperty, value); }
        }
        public static readonly DependencyProperty XYProperty =
            DependencyProperty.Register("XY", typeof(double[][]), typeof(RelationContener), new UIPropertyMetadata(new double[][] { new[] { 0.0, (double)0 }, new[] { (double)0, (double)0 }, new[] { (double)0, (double)0 } }, (d, e) => ((RelationBase)d).OnAssociationCoordonatesChanged()));

        protected virtual void OnAssociationCoordonatesChanged()
        {
            OnPropertyChanged("X1");
            OnPropertyChanged("Y1");
            OnPropertyChanged("X2");
            OnPropertyChanged("Y2");
            OnPropertyChanged("X3");
            OnPropertyChanged("Y3");
            OnPropertyChanged("X4");
            OnPropertyChanged("Y4");
            OnPropertyChanged("CanvasLeft");
            OnPropertyChanged("CanvasTop");
        }

        internal virtual void OnMove()
        {
            RedoXYBinding();
        }

        protected void RedoXYBinding()
        {
            OnPropertyChanged("XY");
            GetBindingExpression(XYProperty).UpdateTarget();
        }

        protected void LineTargetUpdated(Line line)
        {
            line.GetBindingExpression(Line.X1Property).UpdateTarget();
            line.GetBindingExpression(Line.X2Property).UpdateTarget();
            line.GetBindingExpression(Line.Y1Property).UpdateTarget();
            line.GetBindingExpression(Line.Y2Property).UpdateTarget();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
