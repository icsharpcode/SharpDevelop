// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows;

using Editor.AvalonEdit;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
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
			
			var textEditor = editor.GetService(typeof(TextEditor)) as TextEditor;
			if (textEditor != null) {
				textEditor.TextArea.TextView.InsertLayer(
					new PinLayer(textEditor.TextArea),
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
				pin.Popup.Mark = pin;
				
				var nodes = new ObservableCollection<ITreeNode>();
				foreach (var tuple in pin.SavedNodes) {
					var node = new TreeNode();
					node.IconImage =
						new ResourceServiceImage(
							!string.IsNullOrEmpty(tuple.Item1) ? tuple.Item1 : "Icons.16x16.Field");
					node.FullName = tuple.Item2;
					node.Text = tuple.Item3;
					nodes.Add(node);
				}
				
				pin.SavedNodes.Clear();
				pin.Popup.ItemsSource = nodes;
				pin.Nodes = nodes;
				pin.Popup.Open();
				
				GetPinlayer(editor).Pin(pin.Popup);
			}
		}
		
		public static void ClosePins(ITextEditor editor)
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
				
				GetPinlayer(editor).Unpin(pin.Popup);
				pin.Popup = null;
			}
		}
		
		public static PinLayer GetPinlayer(ITextEditor editor) {
			var textEditor = editor.GetService(typeof(TextEditor)) as TextEditor;
			if (textEditor != null) {
				foreach(var layer in textEditor.TextArea.TextView.Layers)
					if(((Layer)layer).LayerType == KnownLayer.DataPins)
						return (PinLayer)layer;
			}
			
			return null;
		}
	}
}
