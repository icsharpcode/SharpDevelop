// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageTreeNode : ExtTreeNode
	{
		/// <summary>
		/// Code coverage is less than one hundred percent.
		/// </summary>
		public static readonly Color PartialCoverageTextColor = Color.Red;
		
		/// <summary>
		/// Code coverage is zero.
		/// </summary>
		public static readonly Color ZeroCoverageTextColor = Color.Gray;
		
		int visitedCount;
		int notVisitedCount;
		int baseImageIndex;
		
		public CodeCoverageTreeNode(string name, CodeCoverageImageListIndex index) : this(name, index, 0, 0)
		{
		}
		
		public CodeCoverageTreeNode(string name, CodeCoverageImageListIndex index, int visitedCount, int notVisitedCount)
		{
			sortOrder = 10;
			this.visitedCount = visitedCount;
			this.notVisitedCount = notVisitedCount;
			
			Name = name;
			SetText();
			
			baseImageIndex = (int)index;
			SetImageIndex();
		}
		
		public int VisitedCount {
			get {
				return visitedCount;
			}
			set {
				visitedCount = value;
				SetText();
				SetImageIndex();
			}
		}
		
		public int NotVisitedCount {
			get {
				return notVisitedCount;
			}
			set {
				notVisitedCount = value;
				SetText();
			}
		}
		
		/// <summary>
		/// Gets the string to use when sorting the code coverage tree node. 
		/// </summary>
		public override string CompareString {
			get { 
				return Name;
			}
		}
		
		/// <summary>
		/// Sorts the child nodes of this node. This sort is not
		/// recursive so it only sorts the immediate children.
		/// </summary>
		protected void SortChildNodes()
		{
			ExtTreeView treeView = (ExtTreeView)TreeView;
			treeView.SortNodes(Nodes, false);
		}
		
		static string GetPercentage(int visitedCount, int totalCount)
		{
			int percentage = (visitedCount * 100) / totalCount;
			return percentage.ToString();
		}
		
		static string GetNodeText(string name, int visitedCount, int totalCount)
		{
			if (totalCount > 0) {
				return String.Concat(name, " (", GetPercentage(visitedCount, totalCount), "%)");
			}
			return name;
		}
		
		void SetText()
		{
			int total = visitedCount + notVisitedCount;
			
			// Change the text color for partial coverage.
			if (visitedCount == 0) {
				ForeColor = ZeroCoverageTextColor; 
			} else if(total != visitedCount) {
				ForeColor = PartialCoverageTextColor;
			} else {
				ForeColor = Color.Empty;
			}
			
			// Get the text for the node.
			Text = GetNodeText(Name, visitedCount, total);
		}
		
		void SetImageIndex()
		{
			ImageIndex = baseImageIndex;
			if (visitedCount == 0) {
				ImageIndex++;
			}
			SelectedImageIndex = ImageIndex;	
		}
	}
}
