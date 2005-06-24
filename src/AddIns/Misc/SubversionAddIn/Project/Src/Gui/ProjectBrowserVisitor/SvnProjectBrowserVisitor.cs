/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 23.11.2004
 * Time: 10:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn
{
	/*
	public class NodeStatusInformer 
	{
		NodeStatus nodeStatus;
		
		public NodeStatus NodeStatus {
			get {
				return nodeStatus;
			}
		}
		
		public NodeStatusInformer(NodeStatus nodeStatus)
		{
			this.nodeStatus = nodeStatus;
		}
		
		public void Inform(NodeStatus status)
		{
			switch (status) {
				case NodeStatus.Added:
				case NodeStatus.Deleted:
				case NodeStatus.Modified:
					this.nodeStatus = NodeStatus.Modified;
					break;
			}			
		}
	}
	
	/// <summary>
	/// Description of SvnProjectBrowserVisitor.
	/// </summary>
	public class SvnProjectBrowserVisitor : AbstractBrowserNodeVisitor
	{
		Client client = new Client();
		
		public override object Visit(AbstractBrowserNode node, object data)
		{
			return node.AcceptChildren(this, data);
		}
		
		public override object Visit(CombineBrowserNode node, object data)
		{
			string fileName = node.FileName;
			NodeStatus nodeStatus = NodeStatus.None;
			if (fileName != null && fileName.Length > 0) {
				Status status = client.SingleStatus(Path.GetFullPath(fileName));
				nodeStatus = GetNodeStatus(status.TextStatus);
			}
			NodeStatusInformer nsi = new NodeStatusInformer(nodeStatus);
			object back = node.AcceptChildren(this, nsi);
			
			node.NodeStatus = nsi.NodeStatus;
			return back;
		}
		
		public override object Visit(DirectoryNode node, object data)
		{
			Status status = client.SingleStatus(Path.GetFullPath(node.FileName));
			node.NodeStatus = GetNodeStatus(status.TextStatus);
			if (data != null) {
				((NodeStatusInformer)data).Inform(node.NodeStatus);
			}
			return node.AcceptChildren(this, data);
		}
		
		public override object Visit(FileNode node, object data)
		{
			Status status = client.SingleStatus(Path.GetFullPath(node.FileName));
			node.NodeStatus = GetNodeStatus(status.TextStatus);
			if (data != null) {
				((NodeStatusInformer)data).Inform(node.NodeStatus);
			}
			return node.AcceptChildren(this, data);
		}
		
		public override object Visit(FolderNode node, object data)
		{
			return node.AcceptChildren(this, data);
		}
		
		public override object Visit(NamedFolderNode node, object data)
		{
			return node.AcceptChildren(this, data);
		}
		
		public override object Visit(ProjectBrowserNode node, object data)
		{
			string fileName = node.FileName;
			NodeStatus nodeStatus = NodeStatus.None;
			if (fileName != null && fileName.Length > 0) {
				Status status = client.SingleStatus(Path.GetFullPath(fileName));
				nodeStatus = GetNodeStatus(status.TextStatus);
			}
			if (data != null) {
				((NodeStatusInformer)data).Inform(nodeStatus);
			}
			object back = node.AcceptChildren(this, data);
			
			if (data != null) {
				node.NodeStatus = ((NodeStatusInformer)data).NodeStatus;
			} else {
				node.NodeStatus = nodeStatus;
			}
			
			return back;
		}
		
		public override object Visit(ReferenceNode node, object data)
		{
			return node.AcceptChildren(this, data);
		}
		
		
		NodeStatus GetNodeStatus(StatusKind kind)
		{
			switch (kind) {
				case StatusKind.None:
					return NodeStatus.None;
				case StatusKind.Normal:
					return NodeStatus.Normal;
				case StatusKind.Added:
					return NodeStatus.Added;
				case StatusKind.Deleted:
					return NodeStatus.Deleted;
				case StatusKind.Conflicted:
					return NodeStatus.Conflicted;
				case StatusKind.Unversioned:
					return NodeStatus.Unversioned;
				case StatusKind.Modified:
					return NodeStatus.Modified;
				case StatusKind.Ignored:
					return NodeStatus.Ignored;
			}
			return NodeStatus.IndividualStatusesConflicting;
		}
	}
	*/
}
