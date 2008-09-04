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
using AvalonDock;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	sealed class AvalonPadContent : DockableContent, IDisposable
	{
		PadDescriptor descriptor;
		IPadContent content;
		AvalonDockLayout layout;
		TextBlock placeholder;
		
		public IPadContent PadContent {
			get { return content; }
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
			if (placeholder != null && placeholder.IsVisible && !layout.Busy) {
				placeholder.IsVisibleChanged -= AvalonPadContent_IsVisibleChanged;
				content = descriptor.PadContent;
				if (content != null) {
					this.SetContent(content.Content);
					placeholder = null;
				}
			}
		}

		public void Dispose()
		{
			if (content != null) {
				content.Dispose();
			}
		}
	}
}
