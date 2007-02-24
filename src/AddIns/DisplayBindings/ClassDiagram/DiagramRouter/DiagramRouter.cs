/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 03/11/2006
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Tools.Diagrams
{
	public class DiagramRouter
	{
		private IList<IRectangle> rects = new List<IRectangle>();
		private List<Route> routes = new List<Route>();

		private const float XSpacing = 40;
		private const float YSpacing = 40;
		
		private class IRectangleSizeDecorator : IEquatable<IRectangleSizeDecorator>
		{
			private IRectangle rect;
			
			private float size;
			private float childrenSize;
			private float summedSize;
			private float x;
			private float y;
			private float width;
			private float height;
			
			public IRectangleSizeDecorator (IRectangle rect)
			{
				this.rect = rect;
			}

			public float Size
			{
				get { return size; }
				set { size = value; }
			}
			
			public float SummedSize
			{
				get { return summedSize; }
				set { summedSize = value; }
			}
			
			public float ChildrenSize
			{
				get { return childrenSize; }
				set { childrenSize = value; }
			}

			public float X
			{
				get { return x; }
				set { x = value; }
			}

			public float Y
			{
				get { return y; }
				set { y = value; }
			}
			
			public float Width
			{
				get { return width; }
				set { width = value; }
			}
			
			public float Height
			{
				get { return height; }
				set { height = value; }
			}
			
			public IRectangle Rectangle
			{
				get { return rect; }
			}
			
			public bool Equals(IRectangleSizeDecorator other)
			{
				if (other == null) return false;
				return rect.Equals(other.Rectangle);
			}
		}
		
		public IList<IRectangle> Rectangles
		{
			get { return rects; }
		}
		
		public void AddItem (IRectangle item)
		{
			rects.Add(item);
		}

		public void RemoveItem (IRectangle item)
		{
			rects.Remove(item);
		}
		
		public Route AddRoute (IRectangle from, IRectangle to)
		{
			Route route = new Route (from, to);
			routes.Add(route);
			return route;
		}
		
		public void RemoveRoute (Route route)
		{
			routes.Remove(route);
		}
		
		public void Clear()
		{
			routes.Clear();
			rects.Clear();
		}
		
		public void ClearRoutes()
		{
			routes.Clear();
		}
		
		private DependencyTree<IRectangleSizeDecorator> BuildDependenciesTree ()
		{
			DependencyTree<IRectangleSizeDecorator> deps = new DependencyTree<IRectangleSizeDecorator>();
			foreach (Route r in routes)
			{
				deps.AddDependency(new IRectangleSizeDecorator(r.From),
				                   new IRectangleSizeDecorator(r.To));
			}
			
			foreach (IRectangle r in rects)
			{
				deps.AddDependency(new IRectangleSizeDecorator(r), deps.Root.Item);
			}
			
			return deps;
		}
		
		public void RecalcPositions ()
		{
			DependencyTree<IRectangleSizeDecorator> deps = BuildDependenciesTree();

			// 1. Make sure each node's initial size is the same as its rectangle size.
			deps.WalkTreeRootFirst(CopyPhysicalSize);
			
			// 2. Arrange the nodes in groups with known width and height.
			deps.WalkTreeChildrenFirst(ArrangeNodes);
			
			// 3. Calculate the physical size of all the nodes.
			//    Treat the nodes decendants as part of the node itself.
			//deps.WalkTreeChildrenFirst(CalcPhysicalSizeRecursive);
			
			// 4. Sort everything by its physical size.
			//deps.WalkTreeChildrenFirst(SortByPhysicalSize);
			
			// 5. Set the positions of the nodes.
			deps.WalkTreeRootFirst(SetupPositions);

			deps.WalkTreeRootFirst(MoveSubTreeByParentPosition);
			float y = 0;
			foreach (DependencyTreeNode<IRectangleSizeDecorator> group in deps.Root.Dependants)
			{
				if (group.Item == null) continue;
				MoveSubTreeBy(group, 0, y);
				y += group.Item.Height + YSpacing;
			}
		}

		private void CopyPhysicalSize(IRectangleSizeDecorator rect)
		{
			if (rect == null) return;
			if (rect.Rectangle == null) return;
			rect.Width = rect.Rectangle.ActualWidth;
			rect.Height = rect.Rectangle.ActualHeight;
		}
		
		private void ArrangeNodes(DependencyTreeNode<IRectangleSizeDecorator> rectNode)
		{
			// Don't handle leafs directly. Let their parent arrange them.
			if (rectNode.IsLeaf) return;
			if (rectNode.Item == null) return;
			
			float x = XSpacing;
			rectNode.Item.X = XSpacing;
			rectNode.Item.Y = YSpacing;
			float h = rectNode.Item.Height;
			foreach (DependencyTreeNode<IRectangleSizeDecorator> node in rectNode.Dependants)
			{
				node.Item.X = x;
				node.Item.Y = YSpacing;
				x += node.Item.Width + XSpacing;
				h = Math.Max (h, rectNode.Item.Height + YSpacing + node.Item.Height);
			}
			rectNode.Item.Height = h;
			rectNode.Item.Width = Math.Max (x - XSpacing, rectNode.Item.Width);
		}

		private void MoveSubTreeByParentPosition (DependencyTreeNode<IRectangleSizeDecorator> rectNode)
		{
			if (rectNode.Item == null) return;
			foreach (DependencyTreeNode<IRectangleSizeDecorator> node in rectNode.Dependants)
				MoveSubTreeBy(node, rectNode.Item.X - XSpacing, rectNode.Item.Y - YSpacing);
		}
		
		private void MoveSubTreeBy (DependencyTreeNode<IRectangleSizeDecorator> root, float x, float y)
		{
			DependencyTree<IRectangleSizeDecorator>.WalkTreeRootFirst(
				root,
				delegate (IRectangleSizeDecorator rect)
				{
					if (rect == null) return;
					rect.Rectangle.X += x;
					rect.Rectangle.Y += y;
				});
		}
		
		private void CalcPhysicalSizeRecursive(DependencyTreeNode<IRectangleSizeDecorator> rectNode)
		{
			rectNode.Item.Size = (rectNode.Item.Rectangle.ActualWidth + XSpacing) * (rectNode.Item.Rectangle.ActualHeight + YSpacing);
			rectNode.Item.ChildrenSize = 0;
			rectNode.Item.SummedSize = rectNode.Item.Size;
			foreach(DependencyTreeNode<IRectangleSizeDecorator> dn in rectNode.Dependants)
			{
				rectNode.Item.ChildrenSize += dn.Item.Size;
				rectNode.Item.SummedSize += dn.Item.SummedSize;
			}
		}
		
		private void SetupPositions(DependencyTreeNode<IRectangleSizeDecorator> rectNode)
		{
			if (rectNode.Item == null) return;
			if (rectNode.Item.Rectangle == null) return;
			
			if (rectNode.ParentNode != null && rectNode.ParentNode.Item != null)
			{
				IRectangle parent = rectNode.ParentNode.Item.Rectangle;
				rectNode.Item.Rectangle.Y = parent.Y + parent.ActualHeight + YSpacing;
				rectNode.Item.Rectangle.X = rectNode.Item.X;
			}
			else
			{
				rectNode.Item.Rectangle.X = XSpacing;
				rectNode.Item.Rectangle.Y = YSpacing;
			}
		}
		
		private void SortByPhysicalSize(DependencyTreeNode<IRectangleSizeDecorator> rectNode)
		{
			rectNode.Dependants.Sort(ComparePairValue);
		}
		
		private int ComparePairValue (
			DependencyTreeNode<IRectangleSizeDecorator> a,
			DependencyTreeNode<IRectangleSizeDecorator> b)
		{
			return a.Item.SummedSize.CompareTo(b.Item.SummedSize);
		}
		
		public void RecalcRoutes ()
		{
			
		}
		
		public Route[] Routes
		{
			get
			{
				return routes.ToArray();
			}
		}
	}
}
