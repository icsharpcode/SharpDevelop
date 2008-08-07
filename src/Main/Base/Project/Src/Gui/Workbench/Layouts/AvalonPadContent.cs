// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using AvalonDock;
using ICSharpCode.Core;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// .
	/// </summary>
	sealed class AvalonPadContent : DockableContent
	{
		PadDescriptor descriptor;
		IPadContent content;
		
		public AvalonPadContent(PadDescriptor descriptor)
		{
			this.descriptor = descriptor;
			
			this.Name = descriptor.Class.Replace('.', '_');
			this.Title = StringParser.Parse(descriptor.Title);
			this.Content = "Placeholder for " + descriptor.Class;
			
			this.Loaded += AvalonPadContent_Loaded;
		}

		void AvalonPadContent_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= AvalonPadContent_Loaded;
			// the first time the pad is "loaded"
			content = descriptor.PadContent;
			if (content != null) {
				this.Content = AvalonWorkbenchWindow.WrapContent(content.Content);
			}
		}
	}
}
