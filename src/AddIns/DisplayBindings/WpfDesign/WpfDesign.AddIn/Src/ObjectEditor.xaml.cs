using System;
using System.Windows;
using ICSharpCode.WpfDesign.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.AddIn
{
	[PropertyEditor(typeof(FrameworkElement), "DataContext")]
	public partial class ObjectEditor
	{
		public ObjectEditor()
		{
			InitializeComponent();
		}

		public PropertyNode PropertyNode {
			get { return DataContext as PropertyNode; }
		}

		protected override void OnClick()
		{
			var s = PropertyNode.Services.GetService<ChooseClassServiceBase>();
			if (s != null) {
				var c = s.ChooseClass();
				if (c != null) {
					PropertyNode.Value = Activator.CreateInstance(c);
				}
			}
		}
	}
}