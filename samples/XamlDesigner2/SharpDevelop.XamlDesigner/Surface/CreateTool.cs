using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Placement;

namespace SharpDevelop.XamlDesigner
{
	public class CreateTool : Tool
	{
		public CreateTool(Type type)
		{
			Type = type;
		}

		public Type Type { get; private set; }

		MoveOperation op;

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is CreateTool && (obj as CreateTool).Type == Type;
		}

		public override void OnMouseEnter()
		{
			if (op == null) {
				var startPoint = Mouse.GetPosition(DesignView.ScrolledLayer);
				var item = DesignView.Context.CreateItem(Type);
				op = new MoveOperation(item, startPoint);
			}
		}

		public override void OnMouseMove()
		{
			if (op != null) {
				op.MoveTo(Mouse.GetPosition(DesignView.ScrolledLayer));
			}
		}

		public override void OnMouseLeave()
		{
			if (op != null) {
				op.Abort();
				op = null;
			}
		}
	}
}
