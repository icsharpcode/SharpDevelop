// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ICSharpCode.NAnt.Gui
{
	/// <summary>
	/// Represents a NAnt build file in the <see cref="NAntPadTreeView"/>.
	/// </summary>
	public class NAntBuildFileTreeNode : TreeNode
	{
		NAntBuildFile buildFile;
		string projectName = String.Empty;
		
		/// <summary>
		/// Creates a new instance of the <see cref="NAntBuildFileTreeNode"/>
		/// class.
		/// </summary>
		/// <param name="buildFile">The <see cref="NAntBuildFile"/>
		/// associated with this tree node.</param>
		public NAntBuildFileTreeNode(string projectName, NAntBuildFile buildFile)
		{
			this.projectName = projectName;
			this.buildFile = buildFile;
			
			UpdateNode();
		}
		
		/// <summary>
		/// Gets the <see cref="NAntBuildFile"/> associated with
		/// this node.
		/// </summary>
		public NAntBuildFile BuildFile {
			get {
				return buildFile;
			}
			
			set {
				SetBuildFile(value);
			}
		}
		
		/// <summary>
		/// Gets or sets the build file's filename.
		/// </summary>
		/// <remarks>
		/// Called when the build file has been renamed.</remarks>
		public string FileName {
			get {
				return buildFile.FileName;
			}
			
			set {
				buildFile.FileName = value;
				SetNodeText();
			}
		}
		
		/// <summary>
		/// Sets the node text.
		/// </summary>
		void SetNodeText()
		{
			StringBuilder nodeText = new StringBuilder();
			
			if (projectName.Length > 0) {
				nodeText.Append(projectName);
				nodeText.Append(Path.DirectorySeparatorChar);
			}
			nodeText.Append(buildFile.FileName);
			
			if (buildFile.DefaultTarget != null) {
				nodeText.Append(" [");
				nodeText.Append(buildFile.DefaultTarget.Name);
				nodeText.Append("]");
			} 
			
			this.Text = nodeText.ToString();
		}
	
		/// <summary>
		/// Adds the targets to the node.
		/// </summary>
		/// <param name="targets">A collection of NAntBuildTargets.</param>
		void AddTargets(NAntBuildTargetCollection targets)
		{
			foreach (NAntBuildTarget target in targets) {
				AddTarget(this, target);
			}
		}
		
		/// <summary>
		/// Adds a NAnt build target to the tree.
		/// </summary>
		/// <param name="node">The parent tree node.</param>
		/// <param name="target">The NAnt build target.</param>
		void AddTarget(TreeNode node, NAntBuildTarget target)
		{
			NAntBuildTargetTreeNode targetNode = new NAntBuildTargetTreeNode(target);
			node.Nodes.Add(targetNode);
		}	
		
		/// <summary>
		/// Adds an error node to the tree.
		/// </summary>
		/// <param name="node">The parent tree node.</param>
		void AddBuildFileError(TreeNode node, NAntBuildFileError buildFileError)
		{
			NAntBuildFileErrorTreeNode errorNode = new NAntBuildFileErrorTreeNode(buildFileError);
			node.Nodes.Add(errorNode);
		}
		
		/// <summary>
		/// Updates the display since the build file has changed.
		/// </summary>
		void SetBuildFile(NAntBuildFile buildFile)
		{			
			Nodes.Clear();
			this.buildFile = buildFile;
			UpdateNode();
		}
		
		/// <summary>
		/// Sets the node's image, text and adds target nodes.
		/// </summary>
		void UpdateNode()
		{
			this.ImageIndex = NAntPadTreeViewImageList.BuildFileImage;
			this.SelectedImageIndex = NAntPadTreeViewImageList.BuildFileImage;			
			
			if (buildFile.HasError) {
				this.ImageIndex = NAntPadTreeViewImageList.BuildFileErrorImage;
				this.SelectedImageIndex = NAntPadTreeViewImageList.BuildFileErrorImage;
				AddBuildFileError(this, buildFile.Error);
			} else {
				AddTargets(buildFile.Targets);
			}
			
			SetNodeText();			
		}
	}
}
