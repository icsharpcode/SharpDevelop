// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;

namespace Debugger.AddIn.Tooltips
{
	public class PinningBinding : DefaultLanguageBinding
	{
		ITextEditor _editor;
		PinLayer pinLayer;
		
		public PinningBinding()
		{}
		
		public override void Attach(ITextEditor editor)
		{
			if (editor == null)
				return;
			
			var textEditor = editor.GetService(typeof(TextEditor)) as TextEditor;
			if (textEditor != null) {
				pinLayer = new PinLayer(textEditor.TextArea);
				textEditor.TextArea.TextView.InsertLayer(
					pinLayer,
					KnownLayer.Caret,
					LayerInsertionPosition.Above);
			}
			
			_editor = editor;
			CreatePins(_editor);
			
			base.Attach(editor);
		}
		
		public override void Detach()
		{
			ClosePins(_editor);
			pinLayer = null;
			base.Detach();
		}
		
		public void CreatePins(ITextEditor editor)
		{
			// load pins
			var pins = BookmarkManager.Bookmarks.FindAll(
				b => b is PinBookmark && b.FileName == editor.FileName);
			
			foreach (var bookmark in pins) {
				var pin = (PinBookmark)bookmark;
				pin.Popup = new PinDebuggerControl();
				pin.Popup.Mark = pin;
				
				var nodes = new ObservableCollection<ITreeNode>();
				foreach (var tuple in pin.SavedNodes) {
					string imageName = !string.IsNullOrEmpty(tuple.Item1) ? tuple.Item1 : "Icons.16x16.Field";
					var node = new Debugger.AddIn.TreeModel.SavedTreeNode(
						new ResourceServiceImage(imageName),
						tuple.Item2,
						tuple.Item3);
					node.ImageName = imageName;
					nodes.Add(node);
				}
				
				pin.SavedNodes.Clear();
				pin.Popup.ItemsSource = nodes;
				pin.Nodes = nodes;
				
				pinLayer.Pin((PinDebuggerControl)pin.Popup);
			}
		}
		
		public void ClosePins(ITextEditor editor)
		{
			// save pins
			var pins = BookmarkManager.Bookmarks.FindAll(
				b => b is PinBookmark && b.FileName == editor.FileName);
			
			foreach (var bookmark in pins) {
				var pin = (PinBookmark)bookmark;
				if (!pin.PinPosition.HasValue)
					pin.PinPosition = pin.Popup.Location;
				
				// nodes
				if (pin.SavedNodes == null)
					pin.SavedNodes = new System.Collections.Generic.List<Tuple<string, string, string>>();
				
				foreach (var node in pin.Nodes) {
					pin.SavedNodes.Add(
						new Tuple<string, string, string>(
							"Icons.16x16.Field",
							node.FullName,
							node.Text));
				}
				
				pinLayer.Unpin((PinDebuggerControl)pin.Popup);
				pin.Popup = null;
			}
		}
		
		public static PinLayer GetPinlayer(ITextEditor editor) {
			var textEditor = editor.GetService(typeof(TextEditor)) as TextEditor;
			if (textEditor != null) {
				return textEditor.TextArea.TextView.Layers[3] as PinLayer;
			}
			
			return null;
		}
	}
}
