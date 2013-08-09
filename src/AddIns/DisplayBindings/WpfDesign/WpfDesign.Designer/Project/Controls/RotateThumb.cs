// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class RotateThumb : ResizeThumb
	{
		private double initialAngle;
		private RotateTransform rotateTransform;
		private Vector startVector;
		private Point centerPoint;
		private Control designerItem;
		private Panel canvas;
		private AdornerPanel parent;

		static RotateThumb()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RotateThumb), new FrameworkPropertyMetadata(typeof(RotateThumb)));
		}

		public RotateThumb()
		{
			this.ResizeThumbVisible = true;
		}
	}
}
