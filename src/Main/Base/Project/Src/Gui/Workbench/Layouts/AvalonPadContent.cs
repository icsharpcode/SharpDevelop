// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using AvalonDock;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

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
			
			CustomFocusManager.SetRememberFocusedChild(this, true);
			this.Name = descriptor.Class.Replace('.', '_');
			this.SetValueToExtension(TitleProperty, new StringParseExtension(descriptor.Title));
			placeholder = new TextBlock { Text = this.Title };
			this.Content = placeholder;
			this.Icon = PresentationResourceService.GetImage(descriptor.Icon);
			
			placeholder.IsVisibleChanged += AvalonPadContent_IsVisibleChanged;
		}
		
		protected override void FocusContent()
		{
			IInputElement activeChild = CustomFocusManager.GetFocusedChild(this);
			if (activeChild == null && padInstance != null) {
				activeChild = padInstance.InitiallyFocusedControl as IInputElement;
			}
			if (activeChild != null) {
				LoggingService.Debug("Will move focus to: " + activeChild);
				Dispatcher.BeginInvoke(DispatcherPriority.Background,
				                       new Action(delegate { Keyboard.Focus(activeChild); }));
			}
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
			if ((descriptor.DefaultPosition & DefaultPadPositions.Hidden) != 0)
				layout.DockingManager.Hide(this);
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
					bool isFocused = this.IsKeyboardFocused;
					this.SetContent(padInstance.Control, padInstance);
					placeholder = null;
					
					if (isFocused) {
						IInputElement initialFocus = padInstance.InitiallyFocusedControl as IInputElement;
						if (initialFocus != null) {
							Dispatcher.BeginInvoke(DispatcherPriority.Background,
							                       new Action(delegate { Keyboard.Focus(initialFocus); }));
						}
					}
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
