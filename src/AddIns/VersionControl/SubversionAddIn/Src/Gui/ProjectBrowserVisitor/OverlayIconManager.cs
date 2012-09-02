// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

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
					statusImages = WinFormsResourceService.GetBitmap("Icons.Svn.StatusImages");
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
		
		public static Image GetImage(StatusKind status)
		{
			switch (status) {
				case StatusKind.Added:
					return GetImage(StatusIcon.Added);
				case StatusKind.Deleted:
					return GetImage(StatusIcon.Deleted);
				case StatusKind.Modified:
				case StatusKind.Replaced:
					return GetImage(StatusIcon.Modified);
				case StatusKind.Normal:
					return GetImage(StatusIcon.OK);
				case StatusKind.Conflicted:
				case StatusKind.Obstructed:
					return GetImage(StatusIcon.Exclamation);
				default:
					return null;
			}
		}
		
		static Queue<AbstractProjectBrowserTreeNode> queue = new Queue<AbstractProjectBrowserTreeNode>();
		static bool threadRunning;
		
		public static void Enqueue(AbstractProjectBrowserTreeNode node)
		{
			if (subversionDisabled)
				return;
			lock (queue) {
				queue.Enqueue(node);
				if (!threadRunning) {
					threadRunning = true;
					ThreadPool.QueueUserWorkItem(Run);
				}
			}
		}
		
		public static void EnqueueRecursive(AbstractProjectBrowserTreeNode node)
		{
			if (subversionDisabled)
				return;
			lock (queue) {
				queue.Enqueue(node);
				// use breadth-first search
				Queue<AbstractProjectBrowserTreeNode> q = new Queue<AbstractProjectBrowserTreeNode>();
				q.Enqueue(node);
				while (q.Count > 0) {
					node = q.Dequeue();
					foreach (TreeNode n in node.Nodes) {
						node = n as AbstractProjectBrowserTreeNode;
						if (node != null) {
							q.Enqueue(node);
							queue.Enqueue(node);
						}
					}
				}
				
				if (!threadRunning) {
					threadRunning = true;
					ThreadPool.QueueUserWorkItem(Run);
				}
			}
		}
		
		static readonly object clientLock = new object();
		static SvnClientWrapper client;
		static bool subversionDisabled;
		
		public static bool SubversionDisabled {
			get { return subversionDisabled; }
		}
		
		static void Run(object state)
		{
			LoggingService.Debug("SVN: OverlayIconManager Thread started");
			// sleep a tiny bit to give main thread time to add more jobs to the queue
			Thread.Sleep(2);
			while (true) {
				if (ICSharpCode.SharpDevelop.ParserService.LoadSolutionProjectsThreadRunning) {
					// Run OverlayIconManager much more slowly while solution is being loaded.
					// This prevents the disk from seeking too much
					Thread.Sleep(100);
				}
				AbstractProjectBrowserTreeNode node;
				lock (queue) {
					if (queue.Count == 0) {
						threadRunning = false;
						ClearStatusCache();
						LoggingService.Debug("SVN: OverlayIconManager Thread finished");
						return;
					}
					node = queue.Dequeue();
				}
				try {
					RunStep(node);
				} catch (Exception ex) {
					MessageService.ShowException(ex);
				}
			}
		}
		
		public static void ClearStatusCache()
		{
			lock (clientLock) {
				if (client != null) {
					client.ClearStatusCache();
				}
			}
		}
		
		public static StatusKind GetStatus(string fileName)
		{
			lock (clientLock) {
				if (subversionDisabled)
					return StatusKind.None;
				
				//Console.WriteLine(fileName);
				
				if (client == null) {
					try {
						client = new SvnClientWrapper();
					} catch (Exception ex) {
						subversionDisabled = true;
						SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall(
							MessageService.ShowWarning,
							"Error initializing Subversion library:\n" + ex.ToString()
						);
						return StatusKind.None;
					}
				}
				
				try {
					return client.SingleStatus(fileName).TextStatus;
				} catch (SvnClientException ex) {
					LoggingService.Warn(ex);
					return StatusKind.None;
				}
			}
		}
		
		static void RunStep(AbstractProjectBrowserTreeNode node)
		{
			if (node.IsDisposed) return;
			
			FileNode fileNode = node as FileNode;
			StatusKind status;
			if (fileNode != null) {
				status = GetStatus(fileNode.FileName);
			} else {
				DirectoryNode directoryNode = node as DirectoryNode;
				if (directoryNode != null) {
					status = GetStatus(directoryNode.Directory);
				} else {
					SolutionNode solNode = node as SolutionNode;
					if (solNode != null) {
						status = GetStatus(solNode.Solution.Directory);
					} else {
						return;
					}
				}
			}
			
			SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					node.Overlay = GetImage(status);
				});
		}
	}
}
