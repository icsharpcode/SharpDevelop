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
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Markup;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer
{
	public partial class Toolbox
	{
		public Toolbox()
		{
			InitializeComponent();
			uxList.SelectionChanged += new SelectionChangedEventHandler(uxList_SelectionChanged);
			ToolService.Instance.CurrentToolChanged += new EventHandler(Instance_CurrentToolChanged);
		}

		public static readonly DependencyProperty ContextProperty =
		   DesignSurface.ContextProperty.AddOwner(typeof(Toolbox));

		public DesignContext Context
		{
			get { return (DesignContext)GetValue(ContextProperty); }
			set { SetValue(ContextProperty, value); }
		}

		public void Load(string content)
		{
			var data = XamlReader.Parse(content) as ToolboxData;
			uxList.ItemsSource = data;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == ContextProperty) {
				OnContextChanged(e.OldValue as DesignContext, e.NewValue as DesignContext);
			}
		}

		void OnContextChanged(DesignContext oldContext, DesignContext newContext)
		{
			if (newContext != null) {
				ToolService.Instance.SwitchDesignPanel(newContext.DesignPanel);
			}
		}

		void Instance_CurrentToolChanged(object sender, EventArgs e)
		{
			var componentTool = ToolService.Instance.CurrentTool as CreateComponentTool;
			if (componentTool != null) {
				uxList.SelectedValue = componentTool.ComponentType;
			}
			else {
				uxList.UnselectAll();
			}
		}

		void uxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (uxList.SelectedValue != null) {
				ToolService.Instance.CurrentTool = new CreateComponentTool(uxList.SelectedValue as Type);
			}
			else {
				ToolService.Instance.Reset();
			}
		}
	}

	public class ToolboxData : List<ToolboxItem>
	{
	}

	public class ToolboxItem
	{
		public Type Type { get; set; }
		public Type FormsType { get; set; }

		public ImageSource Icon
		{
			get
			{
				if (FormsType != null) {
					var bitmap = ToolboxBitmapAttribute.Default.GetImage(FormsType) as Bitmap;
					return Imaging.CreateBitmapSourceFromHBitmap(
						bitmap.GetHbitmap(),
						IntPtr.Zero,
						Int32Rect.Empty,
						null);
				}
				return null;
			}
		}
	}
}
