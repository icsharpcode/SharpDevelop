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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Simple implementation of IOptionPanel with support for OptionBinding markup extensions.
	/// </summary>
	public class OptionPanel : UserControl, IOptionPanel, IOptionBindingContainer,INotifyPropertyChanged
	{
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
			
		static OptionPanel()
		{
			MarginProperty.OverrideMetadata(typeof(OptionPanel),
			                                new FrameworkPropertyMetadata(new Thickness(2, 0, 4, 0)));
		}
		
		public OptionPanel()
		{
			this.Resources.Add(
				typeof(GroupBox),
				new Style(typeof(GroupBox)) { Setters = {
						new Setter(GroupBox.PaddingProperty, new Thickness(3, 3, 3, 7))
					}});
			this.Resources.Add(typeof(CheckBox), GlobalStyles.WordWrapCheckBoxStyle);
			this.Resources.Add(typeof(RadioButton), GlobalStyles.WordWrapCheckBoxStyle);
		}
		
		public virtual object Owner { get; set; }
		
		readonly List<OptionBinding> bindings = new List<OptionBinding>();
		
		void IOptionBindingContainer.AddBinding(OptionBinding binding)
		{
			this.bindings.Add(binding);
		}
		
		public virtual object Control {
			get {
				return this;
			}
		}
		
		public virtual void LoadOptions()
		{
		}
		
		public virtual bool SaveOptions()
		{
			foreach (OptionBinding b in bindings) {
				if (!b.Save())
					return false;
			}
			
			return true;
		}
		
		
		#region INotifyPropertyChanged implementation
		
		protected void RaisePropertyChanged(string propertyName)
		{
			RaiseInternal(propertyName);
		}
		
		
		protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
		{
			var propertyName = ExtractPropertyName(propertyExpresssion);
			RaiseInternal(propertyName);
		}
		
		
		private void RaiseInternal (string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		private static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
		{
			if (propertyExpresssion == null)
			{
				throw new ArgumentNullException("propertyExpresssion");
			}

			var memberExpression = propertyExpresssion.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
			}

			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
			{
				throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
			}

			var getMethod = property.GetGetMethod(true);
			if (getMethod.IsStatic)
			{
				throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
			}

			return memberExpression.Member.Name;
		}
		
		#endregion
	}
}
