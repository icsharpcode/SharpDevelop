/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 07/02/2008
 * Time: 16:30
 * 
 * 
 */

using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Collections.Generic;
using log=ICSharpCode.Core.LoggingService;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of HeaderIndexedTreeViewItem.
	/// </summary>
	public static class TreeViewItemExtender
	{
		public static TreeViewItem GetItemWithHeader(this TreeViewItem t, string header)
		{
			log.Debug("looking for item with name: " + header);
			foreach(Object o in t.Items) {
				TreeViewItem item = o as TreeViewItem;				
				if (item != null) {					
					string h = (string)item.Header;
					log.Debug("looking at item with name: " + h);
					if (h.Equals(header)) {
						log.Debug("found item with name: " + header);
						return item;
					}
				} else {
					MessageBox.Show("While navigating the db connection tree an object not of type TreeViewItem was found",
					                "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					log.Error("found and item in the DbConnectionsNode tree that is not a TreeViewItem");
				}
			}
			log.Debug("could not find item with name: " + header);
			return null;
		}
		
		public static void RemoveItemWithHeader(this TreeViewItem t, string header)
		{
			foreach(Object o in t.Items) {
				HeaderedContentControl h = o as HeaderedContentControl;
				if (h != null) {
					if (h.Header.Equals(header)) {
						log.Debug("removing item with name: " + header);
						t.Items.Remove(h);
					}
				}
			}
		}
		
		public static void RemoveItemsWithHeaderNotIn(this TreeViewItem t, IList<string> headers)
		{
			foreach(Object o in t.Items) {
				HeaderedContentControl h = o as HeaderedContentControl;
				if (h != null) {
					string header = h.Header as string;
					if (!(headers.Contains(header))) {
						log.Debug("item with name: " + header + " was not in the keep-list, so removing it");
						t.Items.Remove(h);
					}
				}
			}
		}
	}
}
