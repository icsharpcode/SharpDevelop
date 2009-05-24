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
					
					CommandsRegistry.LoadContext(contextName, (UIElement)Content);
					
					CommandsRegistry.RegisterCommandBindingsUpdateHandler(contextName, delegate {
															var bindings = CommandsRegistry.GetCommandBindings(contextName);
															CommandsRegistry.RemoveManagedCommandBindings(CommandBindings);
															CommandBindings.AddRange(bindings);
														});
					
					CommandsRegistry.RegisterInputBindingUpdateHandler(contextName, delegate {
															var bindings = CommandsRegistry.GetInputBindings(contextName);
															CommandsRegistry.RemoveManagedInputBindings(InputBindings);
															InputBindings.AddRange(bindings);
														});
					CommandsRegistry.InvokeCommandBindingUpdateHandlers(contextName);
					CommandsRegistry.InvokeInputBindingUpdateHandlers(contextName);
				}
			}
		}

		public void Dispose()
		{
			if (padInstance != null) {
				padInstance.Dispose();
			}
		}
	}
}
