// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.Presentation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using AvalonDock;
using ICSharpCode.Core;
using CommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.SharpDevelop.Gui
{
	sealed class AvalonPadContent : DockableContent, IDisposable
	{
		PadDescriptor descriptor;
		IPadContent padInstance;
		AvalonDockLayout layout;
		TextBlock placeholder;
		
		public IPadContent PadContent {
			get { return padInstance; }
		}
		
		public AvalonPadContent(AvalonDockLayout layout, PadDescriptor descriptor)
		{
			this.descriptor = descriptor;
			this.layout = layout;
			
			this.Name = descriptor.Class.Replace('.', '_');
			this.Title = StringParser.Parse(descriptor.Title);
			placeholder = new TextBlock { Text = this.Title };
			this.Content = placeholder;
			this.Icon = PresentationResourceService.GetPixelSnappedImage(descriptor.Icon);
			
			placeholder.IsVisibleChanged += AvalonPadContent_IsVisibleChanged;
		}
		
		public void ShowInDefaultPosition()
		{
			AnchorStyle style;
			if ((descriptor.DefaultPosition & DefaultPadPositions.Top) != 0)
				style = AnchorStyle.Top;
			else if ((descriptor.DefaultPosition & DefaultPadPositions.Left) != 0)
				style = AnchorStyle.Left;
			else if ((descriptor.DefaultPosition & DefaultPadPositions.Bottom) != 0)
				style = AnchorStyle.Bottom;
			else
				style = AnchorStyle.Right;
			layout.DockingManager.Show(this, DockableContentState.Docked, style);
			SetPaneSizeWorkaround(this.ContainerPane);
			if ((descriptor.DefaultPosition & DefaultPadPositions.Hidden) != 0)
				layout.DockingManager.Hide(this);
		}
		
		static void SetPaneSizeWorkaround(Pane pane)
		{
			ResizingPanel panel = pane.Parent as ResizingPanel;
			if (panel != null) {
				if (panel.Orientation == Orientation.Horizontal) {
					if (ResizingPanel.GetResizeWidth(pane).Value == 0)
						ResizingPanel.SetResizeWidth(pane, new GridLength(200));
				} else if (panel.Orientation == Orientation.Vertical) {
					if (ResizingPanel.GetResizeHeight(pane).Value == 0)
						ResizingPanel.SetResizeHeight(pane, new GridLength(150));
				}
			}
		}
		
		void AvalonPadContent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(LoadPadContentIfRequired));
		}
		
		internal void LoadPadContentIfRequired()
		{
			bool dockingManagerIsInitializing = layout.Busy || !layout.DockingManager.IsLoaded;
			if (placeholder != null && placeholder.IsVisible && !dockingManagerIsInitializing) {
				placeholder.IsVisibleChanged -= AvalonPadContent_IsVisibleChanged;
				padInstance = descriptor.PadContent;
				if (padInstance != null) {
					this.SetContent(padInstance.Control, padInstance);
					placeholder = null;
					
					var contextName = padInstance.GetType().FullName;
					CommandManager.RegisterNamedUIElementInstance(contextName, (UIElement)Content);
				}
			}
		}

		public void Dispose()
		{
			if (padInstance != null) {
				padInstance.Dispose();
			}
		}
		
		public override string ToString()
		{
			return "[AvalonPadContent " + this.Name + "]";
		}
	}
}
