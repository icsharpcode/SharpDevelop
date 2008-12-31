using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using SharpDevelop.XamlDesigner.Controls;
using System.Globalization;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Outline
{
	public partial class OutlineView : UserControl, IHasContext
	{
		public OutlineView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ContextProperty =
			DependencyProperty.Register("Context", typeof(DesignContext), typeof(OutlineView));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		public static bool GetIsExpanded(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsExpandedProperty);
		}

		public static void SetIsExpanded(DependencyObject obj, bool value)
		{
			obj.SetValue(IsExpandedProperty, value);
		}

		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(OutlineView),
			new PropertyMetadata(true));

		//protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		//{
		//    base.OnPropertyChanged(e);
		//    if (e.Property == DocumentProperty) {
		//        if (e.NewValue != null) {
		//            AttachDocument(e.NewValue as DesignContext);
		//        }
		//        if (e.OldValue != null) {
		//            DetachDocument(e.OldValue as DesignContext);
		//        }
		//    }
		//}

		//void AttachDocument(DesignContext doc)
		//{
		//    UpdateRoot();
		//    doc.Changed += Document_Changed;
		//}

		//void DetachDocument(DesignContext doc)
		//{
		//    UpdateRoot();
		//    doc.Changed -= Document_Changed;
		//}		

		//void Document_Changed(object sender, DocumentChangedEventArgs e)
		//{
		//    //if (e.Property == e.Property.ParentItem.Content) {
		//    //    uxTree.UpdateChildren(e.Property.ParentItem);
		//    //}
		//    //else 
		//    if (e.Property == null) {
		//        UpdateRoot();
		//    }
		//}

		//void UpdateRoot()
		//{
		//    if (Document != null && Document.Root != null) {
		//        uxTree.TreeSource = new[] { Document.Root };
		//    }
		//    else {
		//        uxTree.TreeSource = null;
		//    }
		//}
	}
}
