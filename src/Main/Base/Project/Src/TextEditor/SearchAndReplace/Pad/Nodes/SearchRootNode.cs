// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchFolderNode.
	/// </summary>
	public class SearchRootNode : ExtTreeNode
	{
		List<SearchResult> results;
		string             pattern;
		
		public List<SearchResult> Results { 
			get {
				return results;
			}
		}
		
		public SearchRootNode(string pattern, List<SearchResult> results)
		{
			drawDefault = false;
			this.results = results;
			this.pattern = pattern;
			Text = GetText();
		}
		string GetText()
		{
			if (results.Count == 1) {
				if (Nodes.Count == 1) {
					return "Occurrences of '" + pattern + "' (1 occurence in 1 file)";
				} else {
					return "Occurrences of '" + pattern + "' (1 occurence in " + Nodes.Count + " files)";
				}
			} else {
				if (Nodes.Count == 1) {
					return "Occurrences of '" + pattern + "' (" + results.Count + " occurences in 1 file)";
				} else {
					return "Occurrences of '" + pattern + "' (" + results.Count + " occurences in " + Nodes.Count + " files)";
				}
			}
		}
		
		protected override int MeasureItemWidth(DrawTreeNodeEventArgs e)
		{
			return MeasureTextWidth(e.Graphics, GetText(), BoldFont);
		}
		protected override void DrawForeground(DrawTreeNodeEventArgs e)
		{
			Graphics g = e.Graphics;
			float x = e.Bounds.X;
			DrawText(g, GetText(), Brushes.Black, BoldFont, ref x, e.Bounds.Y);
		}
	}
}
