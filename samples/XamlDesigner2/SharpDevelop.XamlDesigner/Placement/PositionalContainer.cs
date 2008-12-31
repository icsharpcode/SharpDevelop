using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	abstract class PositionalContainer : PlacementContainer
	{
		SnapEngine snapEngine;

		public abstract void SetPosition(DesignItem item, Rect bounds);

		public override void OnMove(MoveOperation op)
		{
			if (snapEngine != null) {
				snapEngine.SnapMove(op);
			}
			foreach (var info in op.PlacementInfos) {
				SetPosition(info.Item, info.NewBoundsInContainer);
			}
		}

		public override void Enter(MoveOperation op)
		{
			ContainerItem.Add(op.Items, op.Copy);
			snapEngine = new SnapEngine(ContainerItem);
			snapEngine.BuildMap(op);
		}

		public override void Leave(MoveOperation op)
		{
			op.Items.Delete();
			snapEngine.HideSnaplines();
			snapEngine = null;
		}

		public void BeforeLeavePreviousContainer(MoveOperation op)
		{
			SnapEngine.UpdateBaseline(op.Items.First());
		}
	}
}
