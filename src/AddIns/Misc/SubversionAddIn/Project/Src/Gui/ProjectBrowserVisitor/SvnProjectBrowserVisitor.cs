/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 23.11.2004
 * Time: 10:11
 */

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of SvnProjectBrowserVisitor.
	/// </summary>
	public class SvnProjectBrowserVisitor : ProjectBrowserTreeNodeVisitor
	{
		public override object Visit(SolutionNode node, object data)
		{
			return node.AcceptChildren(this, data);
			/*
			string fileName = node.FileName;
			NodeStatus nodeStatus = NodeStatus.None;
			if (fileName != null && fileName.Length > 0) {
				Status status = client.SingleStatus(Path.GetFullPath(fileName));
				nodeStatus = GetNodeStatus(status.TextStatus);
			}
			NodeStatusInformer nsi = new NodeStatusInformer(nodeStatus);
			object back = node.AcceptChildren(this, nsi);
			
			node.NodeStatus = nsi.NodeStatus;
			return back;*/
		}
		
		public override object Visit(ProjectNode node, object data)
		{
			return Visit((DirectoryNode)node, data);
		}
		
		public override object Visit(DirectoryNode node, object data)
		{
			if (Directory.Exists(Path.Combine(node.Directory, ".svn"))) {
				OverlayIconManager.Enqueue(node);
				return node.AcceptChildren(this, data);
			}
			return data;
		}
		
		public override object Visit(FileNode node, object data)
		{
			OverlayIconManager.Enqueue(node);
			return node.AcceptChildren(this, data);
		}
	}
}
