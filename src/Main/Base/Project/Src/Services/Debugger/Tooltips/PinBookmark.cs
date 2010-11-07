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

namespace Services.Debugger.Tooltips
{
	public class PinBookmark : SDBookmark
	{
		string tooltip;
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		public static readonly IImage PinImage = new ResourceServiceImage("Bookmarks.Pin");
		
		public PinBookmark(FileName fileName, Location location) : base(fileName, location)
		{
			Nodes = new ObservableCollection<ITreeNode>();
			SavedNodes = new List<Tuple<string, string>>();
			Nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
			IsVisibleInBookmarkPad = false;
		}
		
		//TODO this should not be here but onto pinning surface of the code editor
		public DebuggerPopup Popup { get; set; }

		void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var handler = CollectionChanged;
			if (handler != null)
				handler.Invoke(this, e);
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
