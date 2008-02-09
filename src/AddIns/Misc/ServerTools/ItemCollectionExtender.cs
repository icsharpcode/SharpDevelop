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
using System.Collections.Generic;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Description of HeaderIndexedTreeViewItem.
	/// </summary>
	public static class ItemCollectionExtender
	{
		public static Object GetItemWithHeader(this TreeViewItem t, string header)
		{
			foreach(Object o in t.Items) {
				HeaderedContentControl h = o as HeaderedContentControl;
				if (h != null) {
					if (h.Header.Equals(header)) {
						return h;
					}
				}
			}
			return null;
		}
		
		public static void RemoveItemWithHeader(this TreeViewItem t, string header)
		{
			foreach(Object o in t.Items) {
				HeaderedContentControl h = o as HeaderedContentControl;
				if (h != null) {
					if (h.Header.Equals(header)) {
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
						t.Items.Remove(h);
					}
				}
			}
		}
	}
}
