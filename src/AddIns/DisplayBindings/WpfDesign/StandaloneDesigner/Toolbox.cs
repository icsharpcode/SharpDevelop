using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer.Services;

namespace StandaloneDesigner
{
	public partial class Toolbox : ListBox
	{
		public Toolbox()
		{
			this.SelectionMode = SelectionMode.Single;
		}
		
		IToolService toolService;
		
		public IToolService ToolService {
			get { return toolService; }
			set {
				if (toolService != null) {
					toolService.CurrentToolChanged -= OnCurrentToolChanged;
				}
				toolService = value;
				this.Items.Clear();
				if (toolService != null) {
					AddTool("Pointer", toolService.PointerTool);
					AddTool(typeof(Button));
					AddTool(typeof(TextBox));
					AddTool(typeof(CheckBox));
					AddTool(typeof(Label));
					AddTool(typeof(Canvas));
					AddTool(typeof(Grid));
					toolService.CurrentToolChanged += OnCurrentToolChanged;
					OnCurrentToolChanged(null, null);
				}
			}
		}
		
		void AddTool(Type componentType)
		{
			AddTool(componentType.Name, new CreateComponentTool(componentType));
		}
		
		void AddTool(string title, ITool tool)
		{
			ListBoxItem item = new ListBoxItem();
			item.Content = title;
			item.Tag = tool;
			this.Items.Add(item);
		}
		
		void OnCurrentToolChanged(object sender, EventArgs e)
		{
			Debug.WriteLine("Toolbox.OnCurrentToolChanged");
			for (int i = 0; i < this.Items.Count; i++) {
				if (((ListBoxItem)this.Items[i]).Tag == toolService.CurrentTool) {
					this.SelectedIndex = i;
					return;
				}
			}
			this.SelectedIndex = -1;
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			
			Debug.WriteLine("Toolbox.OnSelectionChanged");
			if (toolService != null && this.SelectedItem is ListBoxItem) {
				toolService.CurrentTool = (ITool)(this.SelectedItem as ListBoxItem).Tag;
			}
		}
		
		Point startPos;
		bool canStartDrag;
		
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			startPos = e.GetPosition(this);
			canStartDrag = true;
			
			base.OnPreviewMouseLeftButtonDown(e);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (canStartDrag && e.LeftButton == MouseButtonState.Pressed) {
				if ((e.GetPosition(this) - startPos).LengthSquared > 4) {
					canStartDrag = false;
					
					if (this.SelectedItem == null)
						return;
					
					if (toolService != null && this.SelectedItem is ListBoxItem) {
						ITool tool = (ITool)(this.SelectedItem as ListBoxItem).Tag;
						if (tool is CreateComponentTool) {
							DragDrop.DoDragDrop((ListBoxItem)this.SelectedItem, tool, DragDropEffects.Copy);
						}
					}
				}
			}
			
			base.OnMouseMove(e);
		}
	}
}
