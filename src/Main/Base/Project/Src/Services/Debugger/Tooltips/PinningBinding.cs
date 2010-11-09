// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Services.Debugger.Tooltips
{
	public class PinningBinding : DefaultLanguageBinding
	{
		ITextEditor _editor;
		
		public PinningBinding()
		{}
		
		public override void Attach(ITextEditor editor) 
		{
			if (editor == null)
				return;
			
			_editor = editor;
			
			// load pins
			var pins = BookmarkManager.Bookmarks.FindAll(
				b => b is PinBookmark && b.FileName == _editor.FileName);
			
			foreach (var bookmark in pins) {
				var pin = (PinBookmark)bookmark;
				pin.Popup = new DebuggerPopup(null, true);
				pin.Popup.HorizontalOffset = pin.SavedPopupPosition.X;
				pin.Popup.VerticalOffset = pin.SavedPopupPosition.Y;
				pin.Popup.contentControl.pinCloseControl.Mark = pin;
				
				var nodes = new ObservableCollection<ITreeNode>();
				
				foreach (var tuple in pin.SavedNodes) {
					var node = new TreeNode();
					node.IconImage = 
						new ResourceServiceImage(
							!string.IsNullOrEmpty(tuple.Item1) ? tuple.Item1 : "Icons.16x16.Field");
					node.Name = tuple.Item2;
					node.Text = tuple.Item3;
					
					nodes.Add(node);
				}
				
				pin.SavedNodes.Clear();
				
				pin.Popup.SetItemsSource(nodes);
				pin.Nodes = nodes;
				pin.Popup.Open();
			}
			
			base.Attach(editor);
		}
		
		public override void Detach()
		{
			// save pins
			var pins = BookmarkManager.Bookmarks.FindAll(
				b => b is PinBookmark && b.FileName == _editor.FileName);
			
			foreach (var bookmark in pins) {
				var pin = (PinBookmark)bookmark;
				pin.SavedPopupPosition = new Point 
				{ 
					X = pin.Popup.HorizontalOffset,
				 	Y = pin.Popup.VerticalOffset
				};
				
				pin.Popup.CloseSelfAndChildren();
			}
			
			base.Detach();
		}
	}
}
