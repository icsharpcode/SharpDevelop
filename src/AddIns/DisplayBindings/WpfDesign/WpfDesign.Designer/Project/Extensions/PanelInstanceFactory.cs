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
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Instance factory used to create Panel instances.
	/// Sets the panels Brush to a transparent brush, and modifies the panel's type descriptor so that
	/// the property value is reported as null when the transparent brush is used, and
	/// setting the Brush to null actually restores the transparent brush.
	/// </summary>
	[ExtensionFor(typeof(Panel))]
	public sealed class PanelInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);
		
		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Panel panel = instance as Panel;
			if (panel != null) {
				if (panel.Background == null) {
					panel.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, panel);
			}
			return instance;
		}
	}
	
	[ExtensionFor(typeof(HeaderedContentControl))]
	public sealed class HeaderedContentControlInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);
		
		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Control control = instance as Control;
			if (control != null) {
				if (control.Background == null) {
					control.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, control);
			}
			return instance;
		}
	}
	
	[ExtensionFor(typeof(ItemsControl))]
	public sealed class TransparentControlsInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);
		
		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Control control = instance as Control;
			if (control != null && (
				type == typeof(ItemsControl))) {
				if (control.Background == null) {
					control.Background = _transparentBrush;
				}
				
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, control);
			}
			return instance;
		}
	}
	
	[ExtensionFor(typeof(Border))]
	public sealed class BorderInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Border panel = instance as Border;
			if (panel != null)
			{
				if (panel.Background == null)
				{
					panel.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, panel);
			}
			return instance;
		}
	}
}
