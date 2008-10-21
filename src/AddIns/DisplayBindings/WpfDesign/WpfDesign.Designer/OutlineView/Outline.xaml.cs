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

namespace ICSharpCode.WpfDesign.Designer.OutlineView
{
	public partial class Outline
	{
		public Outline()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ContextProperty =
		   DesignSurface.ContextProperty.AddOwner(typeof(Outline));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ContextProperty) {
				if (e.NewValue != null) {
					AttachContext(e.NewValue as DesignContext);
				}
				if (e.OldValue != null) {
					DetachContext(e.OldValue as DesignContext);
				}
			}
		}

		void AttachContext(DesignContext context)
		{
			UpdateRoot();
			context.ModelService.RootChanged += ModelService_RootChanged;
			context.ModelService.ModelChanged += ModelService_ModelChanged;
			context.SelectionService.SelectionChanged += SelectionService_SelectionChanged;
		}

		void DetachContext(DesignContext context)
		{
			UpdateRoot();
			context.ModelService.RootChanged -= ModelService_RootChanged;
			context.ModelService.ModelChanged -= ModelService_ModelChanged;
			context.SelectionService.SelectionChanged -= SelectionService_SelectionChanged;
		}

		void ModelService_RootChanged(object sender, EventArgs e)
		{
			UpdateRoot();
		}

		void ModelService_ModelChanged(object sender, ModelChangedEventArgs e)
		{
			if (e.Property.IsNameProperty) {
				OutlineNode.Create(e.Property.DesignItem).RaisePropertyChanged("Name");
			}
			else if (e.Property == e.Property.DesignItem.Content) {
				OutlineNode.Create(e.Property.DesignItem).RaisePropertyChanged("Children");
			}
		}

		void SelectionService_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			foreach (var item in e.Items) {
				OutlineNode.Create(item).RaisePropertyChanged("IsSelected");
			}
		}

		void UpdateRoot()
		{
			if (Context != null && Context.ModelService.Root != null) {
				uxTree.Root = OutlineNode.Create(Context.ModelService.Root);
			}
			else {
				uxTree.Root = null;
			}
		}
	}
}
