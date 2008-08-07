// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using AvalonDock;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// .
	/// </summary>
	sealed class AvalonPadContent : DockableContent
	{
		PadDescriptor descriptor;
		
		public AvalonPadContent(PadDescriptor descriptor)
		{
			this.descriptor = descriptor;
			
			this.Title = StringParser.Parse(descriptor.Title);
			this.Content = descriptor.Class;
		}
	}
}
