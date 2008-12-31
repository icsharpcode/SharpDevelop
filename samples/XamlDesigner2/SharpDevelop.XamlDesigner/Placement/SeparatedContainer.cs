using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpDevelop.XamlDesigner.Placement
{
	class SeparatedContainer : CanvasContainer
	{
		public SeparatedContainer(DesignView DesignView, Canvas canvas)
		{
			this.DesignView = DesignView;
			this.canvas = canvas;
		}

		DesignView DesignView;
		Canvas canvas;

		public override void Enter(MoveOperation op)
		{
			foreach (var item in op.Items) {
				canvas.Children.Add(item.View);
			}
		}

		public override void Leave(MoveOperation op)
		{
			canvas.Children.Clear();
		}

		public override GeneralTransform TransformToContainer()
		{
			return DesignView.ZoomedLayer.TransformToDescendant(canvas);
		}
	}
}
