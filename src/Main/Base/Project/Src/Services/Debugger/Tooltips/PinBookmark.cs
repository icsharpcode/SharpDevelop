// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;

namespace Services.Debugger.Tooltips
{
	public class PinBookmark : SDBookmark
	{
		string tooltip;
		
		public PinDebuggerControl Popup {	get; set; }
		
		public static readonly IImage PinImage = new ResourceServiceImage("Bookmarks.Pin");
		
		public PinBookmark(FileName fileName, Location location) : base(fileName, location)
		{
			Nodes = new ObservableCollection<ITreeNode>();
			IsVisibleInBookmarkPad = false;
		}
		
		public Point PopupPosition { get; set; }		
		
		public ObservableCollection<ITreeNode> Nodes { get; set; }
		
		/// <summary>
		/// Image, Name, Text
		/// </summary>
		public List<Tuple<string, string, string>> SavedNodes { get; set; }
		
		public string Comment { get; set; }
		
		public override IImage Image {
			get {
				return PinImage;
			}
		}
		
		public string Tooltip {
			get { return tooltip; }
			set { tooltip = value; }
		}		   
	}
	
	public static class PinBookmarkExtensions
	{
		public static bool ContainsNode(this PinBookmark mark, ITreeNode node)
		{
			if (mark == null)
				throw new ArgumentNullException("mark is null");
			if (node == null)
				throw new ArgumentNullException("Node is null");
			
			foreach (var currentNode in mark.Nodes) {
				if (node.Name.Trim() == currentNode.Name.Trim())
					return true;
			}
			
			return false;
		}
		
		public static void RemoveNode(this PinBookmark mark, ITreeNode node)
		{
			if (mark == null)
				throw new ArgumentNullException("mark is null");
			if (node == null)
				throw new ArgumentNullException("Node is null");
			
			foreach (var currentNode in mark.Nodes) {
				if (node.Name.Trim() == currentNode.Name.Trim()) {
					mark.Nodes.Remove(currentNode);
					return;
				}
			}
		}
	}
}
