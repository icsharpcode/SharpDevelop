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
		/// Code coverage is zero.
		/// </summary>
		public static readonly Color ZeroCoverageTextColor = Color.Gray;
		
		int visitedCodeLength;
		int unvisitedCodeLength;		
		int baseImageIndex;
		
		public CodeCoverageTreeNode(string name, CodeCoverageImageListIndex index) 
			: this(name, index, 0, 0)
		{
		}
		
		public CodeCoverageTreeNode(ICodeCoverageWithVisits codeCoverageWithVisits, CodeCoverageImageListIndex index)
			: this(codeCoverageWithVisits.Name,
				index,
				codeCoverageWithVisits.GetVisitedCodeLength(),
				codeCoverageWithVisits.GetUnvisitedCodeLength())
		{
		}
		
		public CodeCoverageTreeNode(string name, CodeCoverageImageListIndex index, int visitedCodeLength, int unvisitedCodeLength)
		{
			sortOrder = 10;
			this.visitedCodeLength = visitedCodeLength;
			this.unvisitedCodeLength = unvisitedCodeLength;
			
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
