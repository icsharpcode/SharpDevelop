// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Type editor used to edit properties using a text box and the type's default type converter.
	/// </summary>
	public sealed class TextBoxEditor : TextBox
	{
		readonly IPropertyEditorDataProperty property;
		bool isDirty;
		bool hasError;
		
		/// <summary>
		/// Creates a new TextBoxEditor instance.
		/// </summary>
		public TextBoxEditor(IPropertyEditorDataProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			this.property = property;
			UpdateFromSource();
			PropertyEditorBindingHelper.AddValueChangedEventHandler(
				this, property, delegate { UpdateFromSource(); }
			);
		}
		
		void UpdateFromSource()
		{
			if (property.IsAmbiguous) {
				this.Text = "";
				this.Background = SystemColors.ControlBrush;
			} else {
				this.Text = property.TypeConverter.ConvertToInvariantString(property.Value);
			}
			hasError = false;
			isDirty = false;
			if (property.IsSet) {
				this.Foreground = SystemColors.WindowTextBrush;
			} else {
				this.Foreground = SystemColors.GrayTextBrush;
			}
		}
		
		void SaveValueToSource()
		{
			isDirty = false;
			hasError = false;
			try {
				property.Value = property.TypeConverter.ConvertFromInvariantString(this.Text);
			} catch (Exception) {
				hasError = true;
				throw;
			}
		}
		
		static UIElement DescribeError(Exception ex)
		{
			return new TextBlock(new Run(ex.Message));
		}
		
		static void ShowError(ServiceContainer serviceContainer, FrameworkElement control, UIElement errorDescription)
		{
			IErrorService errorService = null;
			if (serviceContainer != null)
				errorService = serviceContainer.GetService<IErrorService>();
			
			if (errorService != null) {
				errorService.ShowErrorTooltip(control, errorDescription);
			}
		}
		
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			isDirty = true;
			this.Background = SystemColors.WindowBrush;
			this.Foreground = SystemColors.WindowTextBrush;
			base.OnTextChanged(e);
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (isDirty) {
				try {
					SaveValueToSource();
				} catch (Exception ex) {
					Dispatcher.BeginInvoke(
						DispatcherPriority.Normal, new Action<Exception>(
							delegate (Exception exception) {
								if (Focus()) {
									ShowError(property.OwnerDataSource.Services, this, DescribeError(exception));
								}
							}), ex);
				}
			} else if (hasError) {
				UpdateFromSource();
			}
			base.OnLostFocus(e);
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Handled) return;
			if (e.Key == Key.Enter) {
				string oldText = this.Text;
				try {
					SaveValueToSource();
				} catch (Exception ex) {
					ShowError(property.OwnerDataSource.Services, this, DescribeError(ex));
				}
				if (this.Text != oldText) {
					this.SelectAll();
				}
				e.Handled = true;
			} else if (e.Key == Key.Escape) {
				string oldText = this.Text;
				UpdateFromSource();
				if (this.Text != oldText) {
					this.SelectAll();
				}
				e.Handled = true;
			}
		}
	}
}
