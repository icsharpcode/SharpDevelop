// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System.Windows;
using System;
using System.Diagnostics;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// A tool that creates a component.
	/// </summary>
	public class CreateComponentTool : ITool
	{
		readonly Type componentType;
		
		/// <summary>
		/// Creates a new CreateComponentTool instance.
		/// </summary>
		public CreateComponentTool(Type componentType)
		{
			if (componentType == null)
				throw new ArgumentNullException("componentType");
			this.componentType = componentType;
		}
		
		/// <summary>
		/// Gets the type of the component to be created.
		/// </summary>
		public Type ComponentType {
			get { return componentType; }
		}
		
		public Cursor Cursor {
			get { return null; }
		}
		
		public void Activate(IDesignPanel designPanel)
		{
			designPanel.MouseDown += OnMouseDown;
			designPanel.DragOver += OnDragOver;
			designPanel.Drop += OnDrop;
		}
		
		public void Deactivate(IDesignPanel designPanel)
		{
			designPanel.MouseDown -= OnMouseDown;
			designPanel.DragOver -= OnDragOver;
			designPanel.Drop -= OnDrop;
		}
		
		/// <summary>
		/// Is called to create the item used by the CreateComponentTool.
		/// </summary>
		protected virtual DesignItem CreateItem(DesignContext context)
		{
			object newInstance = context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(componentType, null);
			DesignItem item = context.Services.Component.RegisterComponentForDesigner(newInstance);
			context.Services.ExtensionManager.ApplyDefaultInitializers(item);
			return item;
		}
		
		void OnDragOver(object sender, DragEventArgs e)
		{
			try {
				if (e.Data.GetData(typeof(CreateComponentTool)) == this) {
					e.Effects = DragDropEffects.Copy;
					e.Handled = true;
				} else {
					e.Effects = DragDropEffects.None;
				}
			} catch (Exception ex) {
				DragDropExceptionHandler.HandleException(ex);
			}
		}
		
		void OnDrop(object sender, DragEventArgs e)
		{
			try {
				if (e.Data.GetData(typeof(CreateComponentTool)) != this)
					return;
				e.Handled = true;
				
				IDesignPanel designPanel = (IDesignPanel)sender;
				DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true);
				if (result.ModelHit != null) {
					designPanel.Focus();
					
					DesignItem createdItem = CreateItem(designPanel.Context);
					AddItemWithDefaultSize(result.ModelHit, createdItem, e.GetPosition(result.ModelHit.View));
				}
				
				if (designPanel.Context.Services.Tool.CurrentTool is CreateComponentTool) {
					designPanel.Context.Services.Tool.CurrentTool = designPanel.Context.Services.Tool.PointerTool;
				}
			} catch (Exception ex) {
				DragDropExceptionHandler.HandleException(ex);
			}
		}
		
		internal static bool AddItemWithDefaultSize(DesignItem container, DesignItem createdItem, Point position)
		{
			PlacementOperation operation = PlacementOperation.TryStartInsertNewComponents(
				container,
				new DesignItem[] { createdItem },
				new Rect[] { new Rect(position, ModelTools.GetDefaultSize(createdItem)) },
				PlacementType.AddItem
			);
			if (operation != null) {
				container.Services.Selection.SetSelectedComponents(new DesignItem[] { createdItem });
				operation.Commit();
				return true;
			} else {
				return false;
			}
		}
		
		void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left)) {
				e.Handled = true;
				IDesignPanel designPanel = (IDesignPanel)sender;
				DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true);
				if (result.ModelHit != null) {
					IPlacementBehavior behavior = result.ModelHit.GetBehavior<IPlacementBehavior>();
					if (behavior != null) {
						DesignItem createdItem = CreateItem(designPanel.Context);
						
						new CreateComponentMouseGesture(result.ModelHit, createdItem).Start(designPanel, e);
					}
				}
			}
		}
	}
	
	sealed class CreateComponentMouseGesture : ClickOrDragMouseGesture
	{
		DesignItem createdItem;
		PlacementOperation operation;
		DesignItem container;
		
		public CreateComponentMouseGesture(DesignItem clickedOn, DesignItem createdItem)
		{
			this.container = clickedOn;
			this.createdItem = createdItem;
			this.positionRelativeTo = clickedOn.View;
		}
		
//		GrayOutDesignerExceptActiveArea grayOut;
//		SelectionFrame frame;
//		AdornerPanel adornerPanel;
		
		Rect GetStartToEndRect(MouseEventArgs e)
		{
			Point endPoint = e.GetPosition(positionRelativeTo);
			return new Rect(
				Math.Min(startPoint.X, endPoint.X),
				Math.Min(startPoint.Y, endPoint.Y),
				Math.Abs(startPoint.X - endPoint.X),
				Math.Abs(startPoint.Y - endPoint.Y)
			);
		}
		
		protected override void OnDragStarted(MouseEventArgs e)
		{
			operation = PlacementOperation.TryStartInsertNewComponents(container,
			                                                           new DesignItem[] { createdItem },
			                                                           new Rect[] { GetStartToEndRect(e) },
			                                                           PlacementType.Resize);
			if (operation != null) {
				services.Selection.SetSelectedComponents(new DesignItem[] { createdItem });
			}
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e);
			if (operation != null) {
				foreach (PlacementInformation info in operation.PlacedItems) {
					info.Bounds = GetStartToEndRect(e);
					operation.CurrentContainerBehavior.SetPosition(info);
				}
			}
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (hasDragStarted) {
				if (operation != null) {
					operation.Commit();
					operation = null;
				}
			} else {
				CreateComponentTool.AddItemWithDefaultSize(container, createdItem, e.GetPosition(positionRelativeTo));
			}
			base.OnMouseUp(sender, e);
		}
		
		protected override void OnStopped()
		{
			if (operation != null) {
				operation.Abort();
				operation = null;
			}
			if (services.Tool.CurrentTool is CreateComponentTool) {
				services.Tool.CurrentTool = services.Tool.PointerTool;
			}
			base.OnStopped();
		}
	}
}
