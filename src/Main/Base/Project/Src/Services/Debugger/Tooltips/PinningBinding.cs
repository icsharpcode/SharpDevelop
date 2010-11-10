// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows;

using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;

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
			CreatePins(_editor);
			
			base.Attach(editor);
		}
		
		public override void Detach()
		{
			ClosePins(_editor);
			
			base.Detach();
		}
		
		public static void CreatePins(ITextEditor editor)
		{
			// load pins
			var pins = BookmarkManager.Bookmarks.FindAll(
				b => b is PinBookmark && b.FileName == editor.FileName);
			
			foreach (var bookmark in pins) {
				var pin = (PinBookmark)bookmark;
				pin.Popup = new PinDebuggerControl();
				pin.Popup.Tag = new Point { X = pin.SavedPopupPosition.X, Y = pin.SavedPopupPosition.Y };
				pin.Popup.Mark = pin;
				
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
				
				pin.Popup.ItemsSource = nodes;
				pin.Nodes = nodes;
				pin.Popup.Open();
				
				var textEditor = editor.GetService(typeof(TextEditor)) as TextEditor;
				if (textEditor != null)
					textEditor.TextArea.PinningLayer.Pin(pin.Popup);
			}
		}
		
		public static void ClosePins(ITextEditor editor)
		{
			// save pins
			var pins = BookmarkManager.Bookmarks.FindAll(
				b => b is PinBookmark && b.FileName == editor.FileName);
			
			foreach (var bookmark in pins) {
				var pin = (PinBookmark)bookmark;
				pin.SavedPopupPosition = (Point)pin.Popup.Tag;
				
				// nodes
				foreach (var node in pin.Nodes) {
					pin.SavedNodes.Add(
						new Tuple<string, string, string>(
							"Icons.16x16.Field",
							node.Name,
							node.Text));
				}
				
				pin.Popup.Close();
				var textEditor = editor.GetService(typeof(TextEditor)) as TextEditor;
				if (textEditor != null)
					textEditor.TextArea.PinningLayer.Unpin(pin.Popup);
				pin.Popup = null;
			}
		}
	}
}
