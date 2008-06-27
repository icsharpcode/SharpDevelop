using System;
using System.Windows;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.AddIn
{
	[PropertyEditor(typeof(FrameworkElement), "DataContext")]
	public partial class ObjectEditor
	{
		public ObjectEditor(IPropertyEditorDataProperty property)
		{
			InitializeComponent();
			
			var s = property.OwnerDataSource.Services.GetService<ChooseClassService>();
			if (s != null) {
				Click += delegate {
					var c = s.ChooseClass();
					if (c != null) {
						MessageBox.Show(c.Name);
					}
				};
			}
		}
	}
}
