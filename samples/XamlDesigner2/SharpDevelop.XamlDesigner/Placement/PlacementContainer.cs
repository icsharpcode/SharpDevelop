using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	class PlacementContainer
	{
		public DesignItem ContainerItem;

		public static PlacementContainer GetContainer(DependencyObject obj)
		{
			return (PlacementContainer)obj.GetValue(ContainerProperty);
		}

		public static void SetContainer(DependencyObject obj, PlacementContainer value)
		{
			obj.SetValue(ContainerProperty, value);
		}

		public static readonly DependencyProperty ContainerProperty =
			DependencyProperty.RegisterAttached("Container", typeof(PlacementContainer), typeof(PlacementContainer),
			new PropertyMetadata(ContainerChanged));

		static void ContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(e.NewValue as PlacementContainer).ContainerItem = d as DesignItem;
		}

		public virtual GeneralTransform TransformToContainer()
		{
			return ContainerItem.Context.DesignView.ZoomedLayer.TransformToDescendant(ContainerItem.View);
		}

		public virtual void Enter(MoveOperation op)
		{
		}

		public virtual void Leave(MoveOperation op)
		{
		}

		public virtual void OnResize(ResizeOperation op)
		{
		}

		public virtual void OnMove(MoveOperation op)
		{
		}
	}
}
