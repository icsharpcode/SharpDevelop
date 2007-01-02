// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Provides a static method to create a binding between a dependency property
	/// and a data property.
	/// </summary>
	public static class PropertyEditorBindingHelper
	{
		/// <summary>
		/// Binds editor.property to dataProperty.Value.
		/// </summary>
		public static Binding CreateBinding(FrameworkElement editor, IPropertyEditorDataProperty dataProperty)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (dataProperty == null)
				throw new ArgumentNullException("dataProperty");
			
			CustomBinding customBinding = new CustomBinding(dataProperty);
			editor.Loaded += customBinding.OnLoaded;
			editor.Unloaded += customBinding.OnUnloaded;
			if (editor.IsLoaded) {
				customBinding.OnLoaded(editor, null);
			}
			
			Binding b = new Binding("BoundValue");
			b.Source = customBinding;
			b.ConverterCulture = CultureInfo.InvariantCulture;
			return b;
		}
		
		sealed class CustomBinding : INotifyPropertyChanged
		{
			readonly IPropertyEditorDataProperty dataProperty;
			
			public CustomBinding(IPropertyEditorDataProperty dataProperty)
			{
				this.dataProperty = dataProperty;
			}
			
			internal void OnLoaded(object sender, RoutedEventArgs e)
			{
				dataProperty.ValueChanged += OnValueChanged;
			}
			
			internal void OnUnloaded(object sender, RoutedEventArgs e)
			{
				dataProperty.ValueChanged -= OnValueChanged;
			}
			
			public object BoundValue {
				get { return dataProperty.Value; }
				set { dataProperty.Value = value; }
			}
			
			public event PropertyChangedEventHandler PropertyChanged;
			
			void OnValueChanged(object sender, EventArgs e)
			{
				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs("BoundValue"));
				}
			}
		}
	}
}
