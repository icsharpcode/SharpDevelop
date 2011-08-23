// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using AvalonDock;
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
			this.Icon = PresentationResourceService.GetBitmapSource(descriptor.Icon);
			
			placeholder.IsVisibleChanged += AvalonPadContent_IsVisibleChanged;
		}
		
		protected override void FocusContent()
		{
			if (!(IsActiveContent && !IsKeyboardFocusWithin))
				return;
			IInputElement activeChild = CustomFocusManager.GetFocusedChild(this);
			AvalonWorkbenchWindow.SetFocus(this, () => activeChild ?? (padInstance != null ? padInstance.InitiallyFocusedControl as IInputElement : null));
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
			
			this.Show(layout.DockingManager, style);
			if ((descriptor.DefaultPosition & DefaultPadPositions.Hidden) != 0)
				Hide();
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
