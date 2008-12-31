using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	public class PlacementInfo
	{
		public DesignItem Item;
		public Rect OriginalBounds;
		public Rect NewBoundsInContainer;
	}
}
