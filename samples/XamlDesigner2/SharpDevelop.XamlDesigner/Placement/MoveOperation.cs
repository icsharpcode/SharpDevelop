using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Extensibility;
using SharpDevelop.XamlDesigner.Extensibility.Attributes;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.Placement
{
	class MoveOperation
	{
		public PlacementContainer PrimaryContainer;
		public PreviewContainer SecondaryContainer;
		public PlacementInfo[] PlacementInfos;
		public bool Copy;
		public Point StartPoint;
		public Point EndPoint;
		public DesignView DesignView;

		public MoveOperation(DesignItem newItem, Point startPoint)
		{
			DesignView = newItem.Context.DesignView;
			StartPoint = DesignView.TransformScrolledToZoomed.Transform(startPoint);

			newItem.View.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			var size = newItem.View.DesiredSize;
			var p = new Point(StartPoint.X - size.Width / 2, StartPoint.Y - size.Height / 2);

			this.PlacementInfos = new PlacementInfo() {
				Item = newItem,
				OriginalBounds = new Rect(p, size)
			}.AsArray();

			PrimaryContainer = DesignView.SeparatedContainer;
			PrimaryContainer.Enter(this);

			Begin();
		}

		public MoveOperation(IEnumerable<DesignItem> items, Point startPoint)
		{
			this.PlacementInfos = items.Select(t => new PlacementInfo() {
				Item = t,
				OriginalBounds = t.GetLayoutSlot()
			}).ToArray();

			this.StartPoint = startPoint;
			this.DesignView = items.First().Context.DesignView;

			Begin();
		}

		public IEnumerable<DesignItem> Items
		{
			get { return PlacementInfos.Select(f => f.Item); }
		}

		public void MoveTo(Point endPoint)
		{
			this.EndPoint = DesignView.TransformScrolledToZoomed.Transform(endPoint);

			DesignItem newParent = null;
			DesignItem currentParent = PrimaryContainer == null ? null : PrimaryContainer.ContainerItem;

			foreach (var hit in DesignView.HitTest(endPoint).Except(Items)) {
				if (hit == currentParent || hit.CanAdd(Items, Copy)) {
					newParent = hit;
					break;
				}
			}

			PlacementContainer newContainer = null;
			if (newParent == null) {
				newContainer = DesignView.SeparatedContainer;
			}
			else {
				newContainer = PlacementContainer.GetContainer(newParent);
				if (newContainer == null) {
					var containerType = MetadataStore.GetAttributes<ContainerTypeAttribute>(newParent.Type).First().ContainerType;
					newContainer = Activator.CreateInstance(containerType) as PlacementContainer;
					PlacementContainer.SetContainer(newParent, newContainer);
				}
			}

			var newPositionalContainer = newContainer as PositionalContainer;

			if (newPositionalContainer != null) {
				if (newPositionalContainer != PrimaryContainer) {
					newPositionalContainer.BeforeLeavePreviousContainer(this);
					PrimaryContainer.Leave(this);
					PrimaryContainer = newPositionalContainer;
					PrimaryContainer.Enter(this);
				}
			}

			if (newContainer != SecondaryContainer) {
				if (SecondaryContainer != null) {
					SecondaryContainer.Leave(this);
				}
				SecondaryContainer = newContainer as PreviewContainer;
				if (SecondaryContainer != null) {
					SecondaryContainer.Enter(this);
				}
			}

			var delta = EndPoint - StartPoint;
			var transformToContainer = PrimaryContainer.TransformToContainer();

			foreach (var info in PlacementInfos) {
				var newBounds = info.OriginalBounds;
				newBounds.Offset(delta);
				info.NewBoundsInContainer = transformToContainer.TransformBounds(newBounds);
			}				

			PrimaryContainer.OnMove(this);

			if (SecondaryContainer != null) {
				SecondaryContainer.OnMove(this);
			}
		}

		public void Abort()
		{
			End();
		}

		public void Commit()
		{
			End();
		}

		void Begin()
		{
			this.DesignView.Context.AdornerManager.OnBeginMove(Items);
			this.Copy = Keyboard.IsKeyDown(Key.LeftCtrl);
		}

		void End()
		{
			PrimaryContainer.Leave(this);
			if (SecondaryContainer != null) {
				SecondaryContainer.Leave(this);
			}
			DesignView.Context.AdornerManager.OnEndMove(Items);
		}
	}
}
