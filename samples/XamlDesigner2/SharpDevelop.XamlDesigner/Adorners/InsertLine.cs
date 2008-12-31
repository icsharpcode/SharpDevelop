using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner
{
	class InsertLine : Control
	{
		static InsertLine()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(InsertLine),
				new FrameworkPropertyMetadata(typeof(InsertLine)));
		}
	}
}
