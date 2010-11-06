// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;

namespace Services.Debugger.Tooltips
{
	public class PinBookmark : SDBookmark
	{
		string tooltip;
		
		public DebuggerPopup Popup {	get; set; }
		
		public static readonly IImage PinImage = new ResourceServiceImage("Bookmarks.Pin");
		
		public PinBookmark(FileName fileName, Location location) : base(fileName, location)
		{
			Nodes = new ObservableCollection<ITreeNode>();
			Nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
			IsVisibleInBookmarkPad = false;
		}

		void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add ||
			   e.Action == NotifyCollectionChangedAction.Remove)
				Popup.contentControl.ItemsSource = Nodes;
		}
		
		public ObservableCollection<ITreeNode> Nodes { get; set; }
		
		public List<Tuple<string, string>> SavedNodes { get; set; }
		
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
}
