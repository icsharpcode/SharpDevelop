// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferenceNodeBuilder
	{
		WebReferenceNodeBuilder()
		{
		}
		
		public static TreeNode AddWebReferencesFolderNode(ProjectNode projectNode, WebReference webReference)
		{
			if (webReference.WebReferencesProjectItem != null) {
				WebReferencesFolderNode webReferencesFolderNode = new WebReferencesFolderNode(webReference.WebReferencesProjectItem);
				webReferencesFolderNode.FileNodeStatus = FileNodeStatus.InProject;
				webReferencesFolderNode.InsertSorted(projectNode);
				return webReferencesFolderNode;
			}	
			return null;
		}
		
		public static TreeNode AddWebReference(WebReferencesFolderNode webReferencesFolderNode, WebReference webReference)
		{
			WebReferenceNode node = new WebReferenceNode(webReference);
			node.FileNodeStatus = FileNodeStatus.InProject;
			node.InsertSorted(webReferencesFolderNode);
			return node;
		}
	}
}
