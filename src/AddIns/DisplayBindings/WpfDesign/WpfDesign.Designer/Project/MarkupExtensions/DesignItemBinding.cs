// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using ICSharpCode.WpfDesign.UIExtensions;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "ICSharpCode.WpfDesign.Designer.MarkupExtensions")]


namespace ICSharpCode.WpfDesign.Designer.MarkupExtensions
{
	/// <summary>
	/// A Binding to a DesignItem of Object
	/// 
	/// This can be used for Example your own Property Pages for Designer Objects
	/// </summary>
	public class DesignItemBinding : MarkupExtension
	{
		private string _propertyName;
		private Binding _binding;
		private DesignItemSetConverter _converter;
		private DependencyProperty _targetProperty;
		private FrameworkElement _targetObject;

		public bool SingleItemProperty { get; set; }
		
		public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

		public DesignItemBinding(string path)
		{
			this._propertyName = path;
			
			UpdateSourceTrigger = UpdateSourceTrigger.Default;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			_targetObject = service.TargetObject as FrameworkElement;
			_targetProperty = service.TargetProperty as DependencyProperty;
			
			_targetObject.DataContextChanged += targetObject_DataContextChanged;

			return null;
		}

		void targetObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var ctx = ((FrameworkElement) sender).DataContext as FrameworkElement;

			var surface = ctx.TryFindParent<DesignSurface>();

			if (surface != null)
			{
				_binding = new Binding(_propertyName);
				_binding.Source = ctx;
				_binding.UpdateSourceTrigger = UpdateSourceTrigger;
				_binding.Mode = BindingMode.TwoWay;

				var designItem = surface.DesignContext.Services.Component.GetDesignItem(ctx);

				_converter = new DesignItemSetConverter(designItem, _propertyName, SingleItemProperty);
				_binding.Converter = _converter;

				_targetObject.SetBinding(_targetProperty, _binding);
			}
			else
			{
				_targetObject.ClearValue(_targetProperty);
			}
		}

		private class DesignItemSetConverter : IValueConverter
		{
			private DesignItem _designItem;
			private string _property;
			private bool _singleItemProperty;

			public DesignItemSetConverter(DesignItem desigItem, string property, bool singleItemProperty)
			{
				this._designItem = desigItem;
				this._property = property;
				this._singleItemProperty = singleItemProperty;
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				var changeGroup = _designItem.OpenGroup("Property: " + _property);

				try
				{
					var property = _designItem.Properties.GetProperty(_property);

					property.SetValue(value);


					if (!_singleItemProperty && _designItem.Services.Selection.SelectedItems.Count > 1)
					{
						var msg = MessageBox.Show("Apply changes to all selected Items","", MessageBoxButton.YesNo);
						if (msg == MessageBoxResult.Yes)
						{
							foreach (var item in _designItem.Services.Selection.SelectedItems)
							{
								try
								{
									property = item.Properties.GetProperty(_property);
								}
								catch(Exception)
								{ }
								if (property != null)
									property.SetValue(value);
							}
						}
					}

					changeGroup.Commit();
				}
				catch (Exception)
				{
					changeGroup.Abort();
				}

				return value;
			}
		}
	}
}
