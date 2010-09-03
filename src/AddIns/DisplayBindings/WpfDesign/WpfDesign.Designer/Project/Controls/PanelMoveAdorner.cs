// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class PanelMoveAdorner : Control
	{
		static PanelMoveAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelMoveAdorner),
				new FrameworkPropertyMetadata(typeof(PanelMoveAdorner)));
		}

		public PanelMoveAdorner(DesignItem item)
		{
			this.item = item;
		}

		DesignItem item;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			e.Handled = true;
            item.Services.Selection.SetSelectedComponents(new DesignItem [] { item }, SelectionTypes.Auto);
			new DragMoveMouseGesture(item, false).Start(item.Services.DesignPanel, e);
		}
	}
}
