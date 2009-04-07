using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ICSharpCode.TreeView
{
	public class InsertMarker : Control
	{
		static InsertMarker()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(InsertMarker),
				new FrameworkPropertyMetadata(typeof(InsertMarker)));
		}
	}
}
