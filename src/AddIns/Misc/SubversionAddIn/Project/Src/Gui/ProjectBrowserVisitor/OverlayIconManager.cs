/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 24.06.2005
 * Time: 21:40
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Static class managing the retrieval of the Subversion icons on a worker thread.
	/// </summary>
	public static class OverlayIconManager
	{
		static Bitmap statusImages;
		
		public static Bitmap StatusImages {
			get {
				if (statusImages == null) {
					statusImages = ResourceService.GetBitmap("Icons.Svn.StatusImages");
				}
				return statusImages;
			}
		}
		
		enum StatusIcon {
			Empty = 0,
			OK,
			Added,
			Deleted,
			Info,
			Empty2,
			Exclamation,
			PropertiesModified,
			Unknown,
			Modified
		}
		
		static Image[] statusIcons = new Image[10];
		
		static Image GetImage(StatusIcon status)
		{
			int index = (int)status;
			if (statusIcons[index] == null) {
				Bitmap statusImages = StatusImages;
				Bitmap smallImage = new Bitmap(7, 10);
				using (Graphics g = Graphics.FromImage(smallImage)) {
					//g.DrawImageUnscaled(statusImages, -index * 7, -3);
					Rectangle srcRect = new Rectangle(index * 7, 3, 7, 10);
					Rectangle destRect = new Rectangle(0, 0, 7, 10);
					g.DrawImage(statusImages, destRect, srcRect, GraphicsUnit.Pixel);
					//g.DrawLine(Pens.Black, 0, 0, 7, 10);
				}
				statusIcons[index] = smallImage;
			}
			return statusIcons[index];
		}
		
		public static Image GetImage(Status status)
		{
			switch (status.TextStatus) {
				case StatusKind.Added:
					return GetImage(StatusIcon.Added);
				case StatusKind.Deleted:
					return GetImage(StatusIcon.Deleted);
				case StatusKind.Modified:
					return GetImage(StatusIcon.Modified);
				case StatusKind.Normal:
					return GetImage(StatusIcon.OK);
				default:
					return null;
			}
		}
		
		static Queue<AbstractProjectBrowserTreeNode> queue = new Queue<AbstractProjectBrowserTreeNode>();
		
		public static void Enqueue(AbstractProjectBrowserTreeNode node)
		{
			lock (queue) {
				queue.Enqueue(node);
				if (queue.Count == 1) {
					new ThreadStart(Run).BeginInvoke(null, null);
				}
			}
		}
		
		static Client client;
		
		static void Run()
		{
			while (true) {
				AbstractProjectBrowserTreeNode node;
				lock (queue) {
					if (queue.Count == 0) return;
					node = queue.Dequeue();
				}
				try {
					RunStep(node);
				} catch (Exception ex) {
					MessageService.ShowError(ex);
				}
			}
		}
		
		static void RunStep(AbstractProjectBrowserTreeNode node)
		{
			if (node.IsDisposed) return;
			if (client == null) {
				client = new Client();
			}
			FileNode fileNode = node as FileNode;
			Status status;
			if (fileNode != null) {
				status = client.SingleStatus(fileNode.FileName);
			} else {
				DirectoryNode directoryNode = node as DirectoryNode;
				if (directoryNode != null) {
					status = client.SingleStatus(directoryNode.Directory);
				} else {
					return;
				}
			}
			if (node.TreeView != null) {
				node.TreeView.BeginInvoke(new ThreadStart(delegate() {
				                                          	node.Overlay = GetImage(status);
				                                          }));
			}
		}
	}
}
