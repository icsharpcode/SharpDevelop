using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDevelop.XamlDesigner.Controls;

namespace SharpDevelop.XamlDesigner
{
	public abstract class Tool
	{
		public DesignView DesignView;		

		public virtual void OnDragStarted(AdvancedDragEventArgs e)
		{
		}

		public virtual void OnDragDelta(AdvancedDragEventArgs e)
		{
		}

		public virtual void OnDragCompleted(AdvancedDragEventArgs e)
		{
		}

		public virtual void OnMouseClick(AdvancedDragEventArgs e)
		{
		}

		public virtual void OnMouseEnter()
		{
		}

		public virtual void OnMouseMove()
		{
		}

		public virtual void OnMouseLeave()
		{
		}
	}
}
