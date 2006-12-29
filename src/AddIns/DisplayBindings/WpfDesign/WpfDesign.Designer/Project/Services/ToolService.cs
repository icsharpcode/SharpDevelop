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
		
		public DefaultToolService()
		{
			_currentTool = _pointerTool = new PointerTool();
		}
		
		public ITool PointerTool {
			get { return _pointerTool; }
		}
		
		public ITool CurrentTool {
			get { return _currentTool; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				_currentTool = value;
			}
		}
	}
	
	sealed class PointerTool : ITool
	{
		public InputHandlingLayer InputLayer {
			get { return InputHandlingLayer.Tool; }
		}
		
		public Cursor Cursor {
			get { return Cursors.Arrow; }
		}
		
		public void OnMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e)
		{
			e.Handled = true;
			new SelectionGesture().Start(designPanel, e);
		}
	}
	
	abstract class MouseGestureBase
	{
		protected IDesignPanel designPanel;
		protected ServiceContainer services;
		bool isStarted;
		
		public void Start(IDesignPanel designPanel, MouseButtonEventArgs e)
		{
			this.designPanel = designPanel;
			this.services = designPanel.Context.Services;
			isStarted = true;
			designPanel.StartInputAction();
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
			designPanel.StopInputAction();
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
			DesignItem item = designPanel.FindDesignedElementForOriginalSource(e.OriginalSource);
			if (item != null) {
				services.Selection.SetSelectedComponents(new DesignItem[] { item }, SelectionTypes.Auto);
			} else {
				services.Selection.SetSelectedComponents(new DesignItem[] { }, SelectionTypes.Auto);
			}
		}
		
		protected override void OnStopped()
		{
			//designPanel.cur
			base.OnStopped();
		}
	}
}
