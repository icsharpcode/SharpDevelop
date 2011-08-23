// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	sealed class DriveObject
	{
		DriveInfo driveInfo;
		string text  = null;
		
		public string Drive {
			get {
				return driveInfo.Name;
			}
		}
		
		public static Image GetImageForFile(string fileName)
		{
			return IconService.GetBitmap(IconService.GetImageForFile(fileName));
		}
		
		public DriveObject(DriveInfo driveInfo)
		{
			this.driveInfo = driveInfo;
			
			text = this.Drive.Substring(0, 2);
			
			switch (driveInfo.DriveType) {
				case DriveType.Removable:
					text += " (${res:MainWindow.Windows.FileScout.DriveType.Removeable})";
					break;
				case DriveType.Fixed:
					text += " (${res:MainWindow.Windows.FileScout.DriveType.Fixed})";
					break;
				case DriveType.CDRom:
					text += " (${res:MainWindow.Windows.FileScout.DriveType.CD})";
					break;
				case DriveType.Network:
					text += " (${res:MainWindow.Windows.FileScout.DriveType.Remote})";
					break;
			}
			text = StringParser.Parse(text);
		}
		
		public override string ToString()
		{
			return text;
		}
	}
	
	sealed class IconManager
	{
		private static ImageList icons = new ImageList();
		private static Hashtable iconIndecies = new Hashtable();
		
		static IconManager()
		{
			icons.ColorDepth = ColorDepth.Depth32Bit;
		}
		
		
		public static ImageList List
		{
			get {
				return icons;
			}
		}
		public static int GetIndexForFile(string file)
		{
			string key;
			
			// icon files and exe files can have their custom icons
			if(Path.GetExtension(file).Equals(".ico", StringComparison.OrdinalIgnoreCase) ||
			   Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase)) {
				key = file;
			} else {
				key = Path.GetExtension(file).ToLower();
			}
			
			// clear the icon cache
			if(icons.Images.Count > 100) {
				icons.Images.Clear();
				iconIndecies.Clear();
			}
			
			if(iconIndecies.Contains(key)) {
				return (int)iconIndecies[key];
			} else {
				icons.Images.Add(DriveObject.GetImageForFile(file));
				int index = icons.Images.Count - 1;
				iconIndecies.Add(key, index);
				return index;
			}
		}
	}
	
	sealed class FileList : ListView
	{
		private FileSystemWatcher watcher;
		
//		private MagicMenus.PopupMenu menu = null;
		
		public FileList()
		{
			ResourceManager resources = new ResourceManager("ProjectComponentResources", this.GetType().Module.Assembly);
			
			
			Columns.Add(ResourceService.GetString("CompilerResultView.FileText"), 100, HorizontalAlignment.Left);
			Columns.Add(ResourceService.GetString("MainWindow.Windows.FileScout.Size"), -2, HorizontalAlignment.Right);
			Columns.Add(ResourceService.GetString("MainWindow.Windows.FileScout.LastModified"), -2, HorizontalAlignment.Left);
			
//			menu = new MagicMenus.PopupMenu();
//			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Delete file", new EventHandler(deleteFiles)));
//			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Rename", new EventHandler(renameFile)));
			
			try {
				watcher = new FileSystemWatcher();
			} catch {}
			
			if(watcher != null) {
				watcher.NotifyFilter = NotifyFilters.FileName;
				watcher.EnableRaisingEvents = false;
				
				watcher.Renamed += new RenamedEventHandler(fileRenamed);
				watcher.Deleted += new FileSystemEventHandler(fileDeleted);
				watcher.Created += new FileSystemEventHandler(fileCreated);
				watcher.Changed += new FileSystemEventHandler(fileChanged);
			}
			
			HideSelection 	= false;
			GridLines		= true;
			LabelEdit		= true;
			SmallImageList = IconManager.List;
			HeaderStyle 	= ColumnHeaderStyle.Nonclickable;
			View 				= View.Details;
			Alignment		= ListViewAlignment.Left;
		}
		
		void fileDeleted(object sender, FileSystemEventArgs e)
		{
			Action method = delegate {
				foreach(FileListItem fileItem in Items)
				{
					if(fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase)) {
						Items.Remove(fileItem);
						break;
					}
				}
			};
			WorkbenchSingleton.SafeThreadAsyncCall(method);
		}
		
		void fileChanged(object sender, FileSystemEventArgs e)
		{
			Action method = delegate {
				foreach(FileListItem fileItem in Items)
				{
					if(fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase)) {
						
						FileInfo info = new FileInfo(e.FullPath);
						
						try {
							fileItem.SubItems[1].Text = Math.Round((double)info.Length / 1024).ToString() + " KB";
							fileItem.SubItems[2].Text = info.LastWriteTime.ToString();
						} catch (IOException) {
							// ignore IO errors
						}
						break;
					}
				}
			};
			WorkbenchSingleton.SafeThreadAsyncCall(method);
		}
		
		void fileCreated(object sender, FileSystemEventArgs e)
		{
			Action method = delegate {
				FileInfo info = new FileInfo(e.FullPath);
				
				ListViewItem fileItem = Items.Add(new FileListItem(e.FullPath));
				try {
					fileItem.SubItems.Add(Math.Round((double)info.Length / 1024).ToString() + " KB");
					fileItem.SubItems.Add(info.LastWriteTime.ToString());
				} catch (IOException) {
					// ignore IO errors
				}
			};
			WorkbenchSingleton.SafeThreadAsyncCall(method);
		}
		
		void fileRenamed(object sender, RenamedEventArgs e)
		{
			Action method = delegate {
				foreach(FileListItem fileItem in Items)
				{
					if(fileItem.FullName.Equals(e.OldFullPath, StringComparison.OrdinalIgnoreCase)) {
						fileItem.FullName = e.FullPath;
						fileItem.Text = e.Name;
						break;
					}
				}
			};
			WorkbenchSingleton.SafeThreadAsyncCall(method);
		}
		
		void renameFile(object sender, EventArgs e)
		{
			if(SelectedItems.Count == 1) {
				SelectedItems[0].BeginEdit();
			}
		}
		
		void deleteFiles(object sender, EventArgs e)
		{
			string fileName = "";
			foreach(FileListItem fileItem in SelectedItems) {
				fileName = fileItem.FullName;
				break;
			}
			if (MessageService.AskQuestion(
				StringParser.Parse("${res:ProjectComponent.ContextMenu.Delete.Question}",
				                   new StringTagPair("FileName", fileName)),
				"${Global.Delete}")) {
				foreach(FileListItem fileItem in SelectedItems)
				{
					try {
						File.Delete(fileItem.FullName);
					} catch(Exception ex) {
						MessageService.ShowException(ex, "Couldn't delete file '" + Path.GetFileName(fileItem.FullName) + "'");
						break;
					}
				}
			}
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			
			ListViewItem itemUnderMouse = GetItemAt(PointToScreen(new Point(e.X, e.Y)).X, PointToScreen(new Point(e.X, e.Y)).Y);
			
			if(e.Button == MouseButtons.Right && this.SelectedItems.Count > 0) {
//				menu.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
			}
		}
		
		protected override void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			
			if (e.Label == null || !FileService.CheckFileName(e.Label)) {
				e.CancelEdit = true;
				return;
			}
			
			string oldFileName = ((FileListItem)Items[e.Item]).FullName;
			string newFileName = Path.Combine(Path.GetDirectoryName(oldFileName), e.Label);
			
			if (FileService.RenameFile(oldFileName, newFileName, false)) {
				((FileListItem)Items[e.Item]).FullName = newFileName;
			} else {
				e.CancelEdit = true;
			}
		}
		
		public void ShowFilesInPath(string path)
		{
			string[] files;
			Items.Clear();
			
			try {
				if (Directory.Exists(path)) {
					files = Directory.GetFiles(path);
				} else {
					return;
				}
			} catch (Exception) {
				return;
			}
			
			watcher.Path = path;
			watcher.EnableRaisingEvents = true;
			
			foreach (string file in files) {
				FileInfo info = new FileInfo(file);
				ListViewItem fileItem = Items.Add(new FileListItem(file));
				fileItem.SubItems.Add(Math.Round((double)info.Length / 1024).ToString() + " KB");
				fileItem.SubItems.Add(info.LastWriteTime.ToString());
			}
			
			EndUpdate();
		}
		
		internal class FileListItem : ListViewItem
		{
			string fullname;
			public string FullName {
				get {
					return fullname;
				} set {
					fullname = value;
				}
			}
			
			public FileListItem(string fullname) : base(Path.GetFileName(fullname))
			{
				this.fullname = fullname;
				ImageIndex = IconManager.GetIndexForFile(fullname);
			}
		}
	}
	
	public class FileScout : UserControl, IPadContent
	{
		public object Control {
			get {
				return this;
			}
		}
		
		public object InitiallyFocusedControl {
			get {
				return null;
			}
		}
		
		Splitter      splitter1     = new Splitter();
		
		FileList   filelister = new FileList();
		ShellTree  filetree   = new ShellTree();
		
		public FileScout()
		{
			filetree.Dock = DockStyle.Top;
			filetree.BorderStyle = BorderStyle.Fixed3D;
			filetree.Location = new System.Drawing.Point(0, 22);
			filetree.Size = new System.Drawing.Size(184, 157);
			filetree.TabIndex = 1;
			filetree.AfterSelect += new TreeViewEventHandler(DirectorySelected);
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.FLOPPY"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.DRIVE"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.CDROM"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.NETWORK"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Desktop"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PersonalFiles"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.MyComputer"));
			
			filetree.ImageList = imglist;
			
			filelister.Dock = DockStyle.Fill;
			filelister.BorderStyle = BorderStyle.Fixed3D;
			filelister.Location = new System.Drawing.Point(0, 184);
			
			filelister.Sorting = SortOrder.Ascending;
			filelister.Size = new System.Drawing.Size(184, 450);
			filelister.TabIndex = 3;
			filelister.ItemActivate += new EventHandler(FileSelected);
			
			splitter1.Dock = DockStyle.Top;
			splitter1.Location = new System.Drawing.Point(0, 179);
			splitter1.Size = new System.Drawing.Size(184, 5);
			splitter1.TabIndex = 2;
			splitter1.TabStop = false;
			splitter1.MinSize = 50;
			splitter1.MinExtra = 50;
			
			this.Controls.Add(filelister);
			this.Controls.Add(splitter1);
			this.Controls.Add(filetree);
		}
		
		void DirectorySelected(object sender, TreeViewEventArgs e)
		{
			filelister.ShowFilesInPath(filetree.NodePath + Path.DirectorySeparatorChar);
		}
		
		void FileSelected(object sender, EventArgs e)
		{
			foreach (FileList.FileListItem item in filelister.SelectedItems) {
				IProjectLoader loader = ProjectService.GetProjectLoader(item.FullName);
				if (loader != null) {
					loader.Load(item.FullName);
				} else {
					FileService.OpenFile(item.FullName);
				}
			}
		}
	}
	
	sealed class ShellTree : TreeView
	{
		public string NodePath {
			get {
				return (string)SelectedNode.Tag;
			}
			set {
				PopulateShellTree(value);
			}
		}
		
		public ShellTree()
		{
			Sorted = true;
			TreeNode rootNode = Nodes.Add(Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
			rootNode.ImageIndex = 6;
			rootNode.SelectedImageIndex = 6;
			rootNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			
			TreeNode myFilesNode = rootNode.Nodes.Add(ResourceService.GetString("MainWindow.Windows.FileScout.MyDocuments"));
			myFilesNode.ImageIndex = 7;
			myFilesNode.SelectedImageIndex = 7;
			try {
				myFilesNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			} catch (Exception) {
				myFilesNode.Tag = "C:\\";
			}
			
			myFilesNode.Nodes.Add("");
			
			TreeNode computerNode = rootNode.Nodes.Add(ResourceService.GetString("MainWindow.Windows.FileScout.MyComputer"));
			computerNode.ImageIndex = 8;
			computerNode.SelectedImageIndex = 8;
			try {
				computerNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			} catch (Exception) {
				computerNode.Tag = "C:\\";
			}
			
			foreach (DriveInfo info in DriveInfo.GetDrives()) {
				DriveObject drive = new DriveObject(info);
				
				TreeNode node = new TreeNode(drive.ToString());
				node.Nodes.Add(new TreeNode(""));
				node.Tag = drive.Drive.Substring(0, 2);
				computerNode.Nodes.Add(node);
				
				switch (info.DriveType) {
					case DriveType.Removable:
						node.ImageIndex = node.SelectedImageIndex = 2;
						break;
					case DriveType.Fixed:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
					case DriveType.CDRom:
						node.ImageIndex = node.SelectedImageIndex = 4;
						break;
					case DriveType.Network:
						node.ImageIndex = node.SelectedImageIndex = 5;
						break;
					default:
						node.ImageIndex = node.SelectedImageIndex = 3;
						break;
				}
			}
			
			foreach (string directory in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))) {
				TreeNode node = rootNode.Nodes.Add(Path.GetFileName(directory));
				node.Tag = directory;
				node.ImageIndex = node.SelectedImageIndex = 0;
				node.Nodes.Add(new TreeNode(""));
			}
			
			rootNode.Expand();
			computerNode.Expand();
			
			InitializeComponent();
		}
		
		int getNodeLevel(TreeNode node)
		{
			TreeNode parent = node;
			int depth = 0;
			
			while(true)
			{
				parent = parent.Parent;
				if(parent == null) {
					return depth;
				}
				depth++;
			}
		}
		
		void InitializeComponent ()
		{
			BeforeSelect   += new TreeViewCancelEventHandler(SetClosedIcon);
			AfterSelect    += new TreeViewEventHandler(SetOpenedIcon);
		}
		
		void SetClosedIcon(object sender, TreeViewCancelEventArgs e) // Set icon as closed
		{
			if (SelectedNode != null) {
				if(getNodeLevel(SelectedNode) > 2) {
					SelectedNode.ImageIndex = SelectedNode.SelectedImageIndex = 0;
				}
			}
		}
		
		void SetOpenedIcon(object sender, TreeViewEventArgs e) // Set icon as opened
		{
			if(getNodeLevel(e.Node) > 2) {
				if (e.Node.Parent != null && e.Node.Parent.Parent != null) {
					e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
				}
			}
		}
		
		void PopulateShellTree(string path)
		{
			string[]  pathlist = path.Split(new char[] { Path.DirectorySeparatorChar });
			TreeNodeCollection  curnode = Nodes;
			
			foreach(string dir in pathlist) {
				
				foreach(TreeNode childnode in curnode) {
					if (((string)childnode.Tag).Equals(dir, StringComparison.OrdinalIgnoreCase)) {
						SelectedNode = childnode;
						
						PopulateSubDirectory(childnode, 2);
						childnode.Expand();
						
						curnode = childnode.Nodes;
						break;
					}
				}
			}
		}
		
		void PopulateSubDirectory(TreeNode curNode, int depth)
		{
			if (--depth < 0) {
				return;
			}
			
			if (curNode.Nodes.Count == 1 && curNode.Nodes[0].Text.Equals("")) {
				
				string[] directories = null;
				curNode.Nodes.Clear();
				try {
					directories  = Directory.GetDirectories(curNode.Tag.ToString() + Path.DirectorySeparatorChar);
				} catch (Exception) {
					return;
				}
				
				
				foreach (string fulldir in directories) {
					try {
						string dir = System.IO.Path.GetFileName(fulldir);
						
						FileAttributes attr = File.GetAttributes(fulldir);
						if ((attr & FileAttributes.Hidden) == 0) {
							TreeNode node   = curNode.Nodes.Add(dir);
							node.Tag = curNode.Tag.ToString() + Path.DirectorySeparatorChar + dir;
							node.ImageIndex = node.SelectedImageIndex = 0;
							
							node.Nodes.Add(""); // Add dummy child node to make node expandable
							
							PopulateSubDirectory(node, depth);
						}
					} catch (Exception) {
					}
				}
			} else {
				foreach (TreeNode node in curNode.Nodes) {
					PopulateSubDirectory(node, depth); // Populate sub directory
				}
			}
		}
		
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			
			try {
				// do not populate if the "My Cpmputer" node is expaned
				if(e.Node.Parent != null && e.Node.Parent.Parent != null) {
					PopulateSubDirectory(e.Node, 2);
					Cursor.Current = Cursors.Default;
				} else {
					PopulateSubDirectory(e.Node, 1);
					Cursor.Current = Cursors.Default;
				}
			} catch (Exception excpt) {
				
				MessageService.ShowException(excpt, "Device error");
				e.Cancel = true;
			}
			
			Cursor.Current = Cursors.Default;
		}
	}
}
