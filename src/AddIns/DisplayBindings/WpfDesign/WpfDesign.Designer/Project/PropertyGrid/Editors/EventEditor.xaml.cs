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

		public string ValueString {
			get { return (string)PropertyNode.Value ?? ""; }
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
			if (PropertyNode != null && Text != ValueString) {
				Commit();
			}
		}

		public void Commit()
		{		
			if (Text != ValueString) {
				if (string.IsNullOrEmpty(Text)) {
					PropertyNode.Reset();
					return;
				}
				PropertyNode.Value = Text;
			}
			IEventHandlerService s = PropertyNode.Services.GetService<IEventHandlerService>();
			if (s != null) {
				s.CreateEventHandler(PropertyNode.FirstProperty);
			}
		}
	}
}
