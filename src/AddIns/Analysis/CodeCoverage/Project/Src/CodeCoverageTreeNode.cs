// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.IO;
using ICSharpCode.SharpDevelop;
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
		/// Code coverage is 100% but branch coverage is not 0%(no branches present) or 100%(all branches covered)
		/// </summary>
		public static readonly Color PartialBranchesTextColor = Color.DarkGreen;

		/// <summary>
		/// Code coverage is zero.
		/// </summary>
		public static readonly Color ZeroCoverageTextColor = Color.Gray;
		
		int visitedCodeLength;
		int unvisitedCodeLength;
		decimal visitedBranchCoverage;
		int baseImageIndex;
		
		public CodeCoverageTreeNode(string name, CodeCoverageImageListIndex index) 
			: this(name, index, 0, 0, 0)
		{
		}
		
		public CodeCoverageTreeNode(ICodeCoverageWithVisits codeCoverageWithVisits, CodeCoverageImageListIndex index)
			: this(codeCoverageWithVisits.Name,
				index,
				codeCoverageWithVisits.GetVisitedCodeLength(),
				codeCoverageWithVisits.GetUnvisitedCodeLength(),
				codeCoverageWithVisits.GetVisitedBranchCoverage()
			)
		{
		}
		
		public CodeCoverageTreeNode(string name, CodeCoverageImageListIndex index, int visitedCodeLength, int unvisitedCodeLength, decimal visitedBranchCoverage)
		{
			sortOrder = 10;
			this.visitedCodeLength = visitedCodeLength;
			this.unvisitedCodeLength = unvisitedCodeLength;
			this.visitedBranchCoverage = visitedBranchCoverage;
			
			Name = name;
			SetText();
			
			baseImageIndex = (int)index;
			SetImageIndex();
		}
		
		void SetText()
		{
			UpdateTextForeColorBasedOnPercentageCodeCoverage();			
			UpdateTextBasedOnPercentageCodeCoverage();
		}
		
		void UpdateTextForeColorBasedOnPercentageCodeCoverage()
		{
			if (visitedCodeLength == 0) {
				ForeColor = ZeroCoverageTextColor; 
			} else if(TotalCodeLength != visitedCodeLength) {
				ForeColor = PartialCoverageTextColor;
			} else if(TotalCodeLength == visitedCodeLength && VisitedBranchCoverage != 0 && VisitedBranchCoverage != 100 ) {
				ForeColor = PartialBranchesTextColor;
			} else {
				ForeColor = Color.Empty;
			}
		}
		
		void UpdateTextBasedOnPercentageCodeCoverage()
		{
			Text = GetNodeText();			
		}
		
		string GetNodeText()
		{
			if (TotalCodeLength > 0) {
				if ( visitedCodeLength == TotalCodeLength && visitedBranchCoverage != 0 && visitedBranchCoverage != 100 ) {
					return String.Format("{0} (100%/{1}%)", Name, visitedBranchCoverage);
				}
				int percentage = GetPercentage();
				return String.Format("{0} ({1}%)", Name, percentage);
			}
			return Name;
		}
		
		int GetPercentage()
		{
			int percentage = (visitedCodeLength * 100) / TotalCodeLength;
			return percentage;
		}
		
		void SetImageIndex()
		{
			ImageIndex = baseImageIndex;
			if (visitedCodeLength == 0) {
				ImageIndex++;
			}
			SelectedImageIndex = ImageIndex;	
		}
		
		public int VisitedCodeLength {
			get { return visitedCodeLength; }
			set {
				visitedCodeLength = value;
				SetText();
				SetImageIndex();
			}
		}
		
		public int UnvisitedCodeLength {
			get { return unvisitedCodeLength; }
			set { 
				unvisitedCodeLength = value;
				SetText();
			}
		}
		
		public int TotalCodeLength {
			get { return visitedCodeLength + unvisitedCodeLength; }
		}
		
		public decimal VisitedBranchCoverage {
			get { return visitedBranchCoverage; }
			set { 
				visitedBranchCoverage = value;
				SetText();
			}
		}
		
		/// <summary>
		/// Gets the string to use when sorting the code coverage tree node. 
		/// </summary>
		public override string CompareString {
			get { return Name; }
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
		
		protected void OpenFile(string fileName)
		{
			if (FileExists(fileName)) {
				FileService.OpenFile(fileName);
			}
		}
		
		bool FileExists(string fileName)
		{
			return !String.IsNullOrEmpty(fileName) && File.Exists(fileName);
		}
		
		protected void JumpToFilePosition(string fileName, int line, int column)
		{
			if (FileExists(fileName)) {
				FileService.JumpToFilePosition(fileName, line, column);
			}
		}
	}
}
