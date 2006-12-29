// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer
{
	sealed class DesignPanel : SingleVisualChildElement, IDesignPanel
	{
		sealed class InnerDesignPanel : SingleVisualChildElement
		{
			internal void SetElement(UIElement element)
			{
				this.VisualChild = element;
			}
		}
		
		DefaultServiceProvider _services;
		InnerDesignPanel _innerDesignPanel;
		UIElement _designedElement;
		
		public DesignPanel(DefaultServiceProvider services)
		{
			this._services = services;
			
			this.Focusable = true;
			
			_innerDesignPanel = new InnerDesignPanel();
			this.VisualChild = _innerDesignPanel;
		}
		
		public UIElement DesignedElement {
			get {
				return _designedElement;
			}
			set {
				_designedElement = value;
				_innerDesignPanel.SetElement(value);
			}
		}
		
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		
		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return new GeometryHitTestResult(this, IntersectionDetail.NotCalculated);
		}
		
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			if (!_isInInputAction) {
				Debug.WriteLine("DesignPanel.PreviewMouseDown Source=" + e.Source.GetType().Name + " OriginalSource=" + e.OriginalSource.GetType().Name);
				DesignItem site = FindDesignedElementForOriginalSource(e.OriginalSource);
				if (site != null) {
					Debug.WriteLine(" Found designed element: " + site.Component.GetType().Name);
				}
				_services.Tool.CurrentTool.OnMouseDown(this, e);
			}
		}
		
		public DesignItem FindDesignedElementForOriginalSource(object originalSource)
		{
			if (originalSource == null)
				return null;
			DesignItem site = _services.Component.GetDesignItem(originalSource);
			if (site != null)
				return site;
			if (originalSource == _innerDesignPanel)
				return null;
			DependencyObject dObj = originalSource as DependencyObject;
			if (dObj == null)
				return null;
			return FindDesignedElementForOriginalSource(VisualTreeHelper.GetParent(dObj));
		}
		
		/// <summary>
		/// prevent designed controls from getting the keyboard focus
		/// </summary>
		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != this) {
				if (e.NewFocus is TabItem) {
					Dispatcher.BeginInvoke(DispatcherPriority.Normal,
					                       new Action(delegate { Focus(); }));
				} else {
					e.Handled = true;
					Focus();
				}
			}
		}
		
		#region IDesignPanel implementation
		public UIElement DesignPanelUI {
			get { return this; }
		}
		
		public DefaultServiceProvider Services {
			get { return _services; }
		}
		#endregion
		
		bool _isInInputAction;
		
		void IDesignPanel.StartInputAction()
		{
			if (_isInInputAction) throw new InvalidOperationException();
			_isInInputAction = true;
			_innerDesignPanel.IsHitTestVisible = false;
		}
		
		void IDesignPanel.StopInputAction()
		{
			if (!_isInInputAction) throw new InvalidOperationException();
			_isInInputAction = false;
			_innerDesignPanel.IsHitTestVisible = true;
		}
	}
	
	internal delegate void Action();
}
