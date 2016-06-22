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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using Debugger.AddIn.Pads.Controls;

namespace Debugger.AddIn.Breakpoints
{
	/// <summary>
	/// Interaction logic for BreakpointEditorPopup.xaml
	/// </summary>
	public partial class BreakpointEditorPopup : Popup, ITooltip
	{
		public BreakpointEditorPopup(BreakpointBookmark target)
		{
			InitializeComponent();
			this.DataContext = target;
			condition.ContextFileName = target.FileName;
			condition.ContextTextLocation = target.Location;
			condition.FontFamily = new FontFamily(SD.EditorControlService.GlobalOptions.FontFamily);
			condition.FontSize = SD.EditorControlService.GlobalOptions.FontSize;
			if (target.Condition == null)
				breakAction.IsChecked = true;
			else
				conditionalAction.IsChecked = true;
		}

		public bool CloseWhenMouseMovesAway {
			get { return !IsKeyboardFocusWithin && !IsFocused; }
		}
		
		void TypeChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender == breakAction)
				((BreakpointBookmark)DataContext).Condition = null;
			if (sender == conditionalAction && ((BreakpointBookmark)DataContext).Condition == null)
				((BreakpointBookmark)DataContext).Condition = "";
		}
	}
}