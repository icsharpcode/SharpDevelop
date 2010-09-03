// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Reflection;

namespace ICSharpCode.WpfDesign.Designer
{
	public class CallExtension : MarkupExtension
	{
		public CallExtension(string methodName)
		{
			this.methodName = methodName;
		}

		string methodName;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var t = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			return new CallCommand(t.TargetObject as FrameworkElement, methodName);
		}
	}

	public class CallCommand : DependencyObject, ICommand
	{
		public CallCommand(FrameworkElement element, string methodName)
		{
			this.element = element;
			this.methodName = methodName;
			element.DataContextChanged += target_DataContextChanged;

			BindingOperations.SetBinding(this, CanCallProperty, new Binding("DataContext.Can" + methodName) {
				Source = element
			});

			GetMethod();
		}

		FrameworkElement element;
		string methodName;
		MethodInfo method;

		public static readonly DependencyProperty CanCallProperty =
			DependencyProperty.Register("CanCall", typeof(bool), typeof(CallCommand),
			                            new PropertyMetadata(true));

		public bool CanCall {
			get { return (bool)GetValue(CanCallProperty); }
			set { SetValue(CanCallProperty, value); }
		}

		public object DataContext {
			get { return element.DataContext; }
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == CanCallProperty) {
				RaiseCanExecuteChanged();
			}
		}

		void GetMethod()
		{
			if (DataContext == null) {
				method = null;
			}
			else {
				method = DataContext.GetType().GetMethod(methodName, Type.EmptyTypes);
			}
		}

		void target_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			GetMethod();
			RaiseCanExecuteChanged();
		}

		void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null) {
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}

		#region ICommand Members

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return method != null && CanCall;
		}

		public void Execute(object parameter)
		{
			method.Invoke(DataContext, null);
		}

		#endregion
	}
}
