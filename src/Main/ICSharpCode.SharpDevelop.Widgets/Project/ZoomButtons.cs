// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

namespace ICSharpCode.SharpDevelop.Widgets
{
	public class ZoomButtons : RangeBase
	{
		static ZoomButtons()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomButtons),
			                                         new FrameworkPropertyMetadata(typeof(ZoomButtons)));
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			var uxPlus = (ButtonBase)Template.FindName("uxPlus", this);
			var uxMinus = (ButtonBase)Template.FindName("uxMinus", this);
			var uxReset = (ButtonBase)Template.FindName("uxReset", this);
			
			if (uxPlus != null)
				uxPlus.Click += OnZoomInClick;
			if (uxMinus != null)
				uxMinus.Click += OnZoomOutClick;
			if (uxReset != null)
				uxReset.Click += OnResetClick;
		}
		
		const double ZoomFactor = 1.1;
		
		void OnZoomInClick(object sender, EventArgs e)
		{
			SetCurrentValue(ValueProperty, ZoomScrollViewer.RoundToOneIfClose(this.Value * ZoomFactor));
		}
		
		void OnZoomOutClick(object sender, EventArgs e)
		{
			SetCurrentValue(ValueProperty, ZoomScrollViewer.RoundToOneIfClose(this.Value / ZoomFactor));
		}
		
		void OnResetClick(object sender, EventArgs e)
		{
			SetCurrentValue(ValueProperty, 1.0);
		}
	}
}
