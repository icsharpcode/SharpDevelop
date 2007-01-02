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
				new SelectionGesture().Start((IDesignPanel)sender, e);
			}
		}
	}
	
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
		bool isStarted;
		
		public void Start(IDesignPanel designPanel, MouseButtonEventArgs e)
		{
			this.designPanel = designPanel;
			this.services = designPanel.Context.Services;
			isStarted = true;
			designPanel.IsAdornerLayerHitTestVisible = false;
			RegisterEvents();
			if (designPanel.CaptureMouse()) {
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
		}
		
		void UnRegisterEvents()
		{
			designPanel.LostMouseCapture -= OnLostMouseCapture;
			designPanel.MouseDown -= OnMouseDown;
			designPanel.MouseMove -= OnMouseMove;
			designPanel.MouseUp -= OnMouseUp;
		}
		
		protected virtual void OnLostMouseCapture(object sender, MouseEventArgs e)
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
	
	sealed class SelectionGesture : MouseGestureBase
	{
		protected override void OnStarted(MouseButtonEventArgs e)
		{
			base.OnStarted(e);
			DesignPanelHitTestResult result = designPanel.HitTest(e, false, true);
			if (result.ModelHit != null) {
				services.Selection.SetSelectedComponents(new DesignItem[] { result.ModelHit }, SelectionTypes.Auto);
			}
		}
		
		protected override void OnStopped()
		{
			//designPanel.cur
			base.OnStopped();
		}
	}
}
