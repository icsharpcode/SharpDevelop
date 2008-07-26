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
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors
{
	[TypeEditor(typeof(MulticastDelegate))]
	public partial class EventEditor
	{
		public EventEditor()
		{
			InitializeComponent();
		}

		public PropertyNode PropertyNode {
			get { return DataContext as PropertyNode; }
		}

		public void Commit()
		{
			PropertyNode.Value = Text;

			IEventHandlerService s = PropertyNode.Services.GetService<IEventHandlerService>();
			if (s != null) {
				s.CreateEventHandler(PropertyNode.FirstProperty);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				Commit();
			}
			else if (e.Key == Key.Escape) {
				BindingOperations.GetBindingExpression(this, TextProperty).UpdateTarget();
			}
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			Commit();
		}

		protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (PropertyNode != null && PropertyNode.Value != Text) {
				if (string.IsNullOrEmpty(Text)) {
					PropertyNode.Reset();
				}
				else {
					Commit();
				}
			}
		}
	}
}
