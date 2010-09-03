// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using System;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    public partial class RelationContener : UserControl
    {
        public RelationContener(RelationBase relationControl)
        {
            RelationControl = relationControl;
            InitializeComponent();
        }

        public RelationBase RelationControl {get; private set;}

        public TypeBaseDesigner FromTypeDesigner 
        {
            get { return RelationControl.FromTypeDesigner; }
        }
        public TypeBaseDesigner ToTypeDesigner 
        {
            get { return RelationControl.ToTypeDesigner; }
        }

        internal void OnMove()
        {
            RelationControl.OnMove();
            if (GetBindingExpression(Canvas.LeftProperty) != null)
                GetBindingExpression(Canvas.LeftProperty).UpdateTarget();
            if (GetBindingExpression(Canvas.TopProperty) != null)
                GetBindingExpression(Canvas.TopProperty).UpdateTarget();
        }

        public void OnRemove()
        {
            if (Removed != null)
                Removed();
        }
        public event Action Removed;
    }
}
