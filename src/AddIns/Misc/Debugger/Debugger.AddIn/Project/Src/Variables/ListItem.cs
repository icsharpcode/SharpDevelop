// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Debugger
{
	public abstract class ListItem
	{
		public event EventHandler<ListItemEventArgs> Changed;
		
		public abstract int ImageIndex { get; }
		public abstract string Name { get; }
		public abstract string Text { get; }
		public abstract bool CanEditText { get; }
		public abstract string Type  { get; }
		public abstract bool HasSubItems  { get; }
		public abstract IList<ListItem> SubItems { get; }
		
		public System.Drawing.Image Image {
			get {
				if (ImageIndex == -1) {
					return null;
				} else {
					return DebuggerIcons.ImageList.Images[ImageIndex];
				}
			}
		}
		
		protected virtual void OnChanged(ListItemEventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public virtual bool SetText(string newValue)
		{
			throw new NotImplementedException();
		}
		
		public virtual ContextMenuStrip GetContextMenu()
		{
			return null;
		}
	}
}
