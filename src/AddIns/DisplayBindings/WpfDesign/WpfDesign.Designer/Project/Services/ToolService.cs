// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	// See IToolService for description.
	sealed class DefaultToolService : IToolService
	{
		PointerTool _pointerTool;
		ITool _currentTool;
		IDesignPanel _designPanel;
		
		public DefaultToolService(DesignContext context)
		{
			_currentTool = _pointerTool = new PointerTool();
			context.Services.RunWhenAvailable<IDesignPanel>(
				delegate(IDesignPanel designPanel) {
					_designPanel = designPanel;
					_currentTool.Activate(designPanel);
				});
		}
		
		public ITool PointerTool {
			get { return _pointerTool; }
		}
		
		public ITool CurrentTool {
			get { return _currentTool; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (_currentTool == value) return;
				_currentTool.Deactivate(_designPanel);
				_currentTool = value;
				_currentTool.Activate(_designPanel);
			}
		}
	}
	
	sealed class PointerTool : ITool
	{
		public Cursor Cursor {
			get { return null; }
		}
		
		public void Activate(IDesignPanel designPanel)
		{
			designPanel.MouseDown += OnMouseDown;
		}
		
		public void Deactivate(IDesignPanel designPanel)
		{
			designPanel.MouseDown -= OnMouseDown;
		}
		
		void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left)) {
				e.Handled = true;
				IDesignPanel designPanel = (IDesignPanel)sender;
				DesignPanelHitTestResult result = designPanel.HitTest(e, false, true);
				if (result.ModelHit != null) {
					IHandlePointerToolMouseDown b = result.ModelHit.GetBehavior<IHandlePointerToolMouseDown>();
					if (b != null) {
						b.HandleSelectionMouseDown(designPanel, e, result);
					} else {
						ISelectionService selectionService = designPanel.Context.Services.Selection;
						selectionService.SetSelectedComponents(new DesignItem[] { result.ModelHit }, SelectionTypes.Auto);
						if (selectionService.IsComponentSelected(result.ModelHit)) {
							new DragMoveMouseGesture(result.ModelHit).Start(designPanel, e);
						}
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Base class for classes handling mouse gestures on the design surface.
	/// </summary>
	abstract class MouseGestureBase
	{
		/// <summary>
		/// Checks if <paramref name="button"/> is the only button that is currently pressed.
		/// </summary>
		internal static bool IsOnlyButtonPressed(MouseEventArgs e, MouseButton button)
		{
			return e.LeftButton == (button == MouseButton.Left ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.MiddleButton == (button == MouseButton.Middle ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.RightButton == (button == MouseButton.Right ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.XButton1 == (button == MouseButton.XButton1 ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.XButton2 == (button == MouseButton.XButton2 ? MouseButtonState.Pressed : MouseButtonState.Released);
		}
		
		protected IDesignPanel designPanel;
		protected ServiceContainer services;
		protected bool canAbortWithEscape = true;
		bool isStarted;
		
		public void Start(IDesignPanel designPanel, MouseButtonEventArgs e)
		{
			if (designPanel == null)
				throw new ArgumentNullException("designPanel");
			if (e == null)
				throw new ArgumentNullException("e");
			if (isStarted)
				throw new InvalidOperationException("Gesture already was started");
			
			isStarted = true;
			this.designPanel = designPanel;
			this.services = designPanel.Context.Services;
			designPanel.IsAdornerLayerHitTestVisible = false;
			if (designPanel.CaptureMouse()) {
				RegisterEvents();
				OnStarted(e);
			} else {
				Stop();
			}
		}
		
		void RegisterEvents()
		{
			designPanel.LostMouseCapture += OnLostMouseCapture;
			designPanel.MouseDown += OnMouseDown;
			designPanel.MouseMove += OnMouseMove;
			designPanel.MouseUp += OnMouseUp;
			designPanel.KeyDown += OnKeyDown;
		}
		
		void UnRegisterEvents()
		{
			designPanel.LostMouseCapture -= OnLostMouseCapture;
			designPanel.MouseDown -= OnMouseDown;
			designPanel.MouseMove -= OnMouseMove;
			designPanel.MouseUp -= OnMouseUp;
			designPanel.KeyDown -= OnKeyDown;
		}
		
		void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (canAbortWithEscape && e.Key == Key.Escape) {
				e.Handled = true;
				Stop();
			}
		}
		
		void OnLostMouseCapture(object sender, MouseEventArgs e)
		{
			Stop();
		}
		
		protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
		}
		
		protected virtual void OnMouseMove(object sender, MouseEventArgs e)
		{
		}
		
		protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			Stop();
		}
		
		protected void Stop()
		{
			if (!isStarted) return;
			isStarted = false;
			designPanel.ReleaseMouseCapture();
			UnRegisterEvents();
			designPanel.IsAdornerLayerHitTestVisible = true;
			OnStopped();
		}
		
		protected virtual void OnStarted(MouseButtonEventArgs e) {}
		protected virtual void OnStopped() {}
	}
}
