// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Xml;
	
	using HtmlHelp2.Environment;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;

	public class ShowFavoritesMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor favorites = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2FavoritesPad));
			if (favorites != null) favorites.BringPadToFront();
		}
	}

	public class HtmlHelp2FavoritesPad : AbstractPadContent
	{
		const string help2FavoritesFile = "help2favorites.xml";
		string[] toolbarButtons         = new string[] {
			"${res:AddIns.HtmlHelp2.MoveUp}",
			"${res:AddIns.HtmlHelp2.MoveDown}",
			"${res:AddIns.HtmlHelp2.Rename}",
			"${res:AddIns.HtmlHelp2.Delete}"
		};
	
		bool Help2EnvIsReady            = false;
		Panel mainPanel                 = new Panel();
		TreeView tv                     = new TreeView();
		ToolStrip toolStrip             = new ToolStrip();

		public override Control Control
		{
			get { return mainPanel;	}
		}

		public override void RedrawContent()
		{
			for (int i = 0; i < toolbarButtons.Length; i++)
			{
				toolStrip.Items[i].ToolTipText = StringParser.Parse(toolbarButtons[i]);
			}
		}

		public HtmlHelp2FavoritesPad()
		{
			this.InitializeComponents();
		}

		private void InitializeComponents()
		{
			Help2EnvIsReady      = HtmlHelp2Environment.IsReady;
			
			mainPanel.Controls.Add(tv);
			tv.Dock              = DockStyle.Fill;
			tv.Enabled           = Help2EnvIsReady;
			tv.ShowLines         = false;
			tv.ShowRootLines     = false;
			tv.LabelEdit         = true;
			tv.HideSelection     = false;
			tv.AfterSelect      += new TreeViewEventHandler(this.TreeNodeAfterSelect);
			tv.BeforeLabelEdit  += new NodeLabelEditEventHandler(this.BeforeLabelEdit);
			tv.AfterLabelEdit   += new NodeLabelEditEventHandler(this.AfterLabelEdit);
			tv.KeyDown          += new KeyEventHandler(this.TreeViewKeyDown);
			tv.DoubleClick      += new EventHandler(this.TreeNodeDoubleClick);

			mainPanel.Controls.Add(toolStrip);
			toolStrip.Dock             = DockStyle.Top;
			toolStrip.Enabled          = Help2EnvIsReady;
			toolStrip.AllowItemReorder = false;
			for (int i = 0; i < toolbarButtons.Length; i++)
			{
				ToolStripButton button = new ToolStripButton();
				button.ToolTipText     = StringParser.Parse(toolbarButtons[i]);
				button.ImageIndex      = i;
				button.Enabled         = false;
				button.Click          += new EventHandler(this.ToolStripButtonClicked);

				toolStrip.Items.Add(button);
			}

			toolStrip.ImageList                  = new ImageList();
			toolStrip.ImageList.ColorDepth       = ColorDepth.Depth4Bit;
			toolStrip.ImageList.TransparentColor = Color.Red;
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("Favorites.16x16.MoveUp.bmp"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("Favorites.16x16.MoveDown.bmp"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("Favorites.16x16.Rename.bmp"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("Favorites.16x16.Delete.bmp"));

			if (Help2EnvIsReady) this.LoadFavorites();
		}

		#region TreeView
		private void TreeNodeAfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNode tn           = tv.SelectedNode;

			toolStrip.Items[0].Enabled = (tn != null && tn.PrevNode != null);
			toolStrip.Items[1].Enabled = (tn != null && tn.NextNode != null);
			toolStrip.Items[2].Enabled = (tn != null);
			toolStrip.Items[3].Enabled = (tn != null);
		}
		
		private void BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			e.CancelEdit = (e.Node == null);
		}
		
		private void AfterLabelEdit(object Sender, NodeLabelEditEventArgs e)
		{
			if (e.Label != null && e.Label.Length > 0)
			{
				if (e.Node.Tag != null && e.Node.Tag is string && (string)e.Node.Tag != "")
				{
					this.PatchFavoriteName(e.Label.ToString(), (string)e.Node.Tag);
				}
			}
		}
		
		private void TreeViewKeyDown(object sender, KeyEventArgs e)
		{
			if (tv.SelectedNode != null)
			{
				switch (e.KeyCode)
				{
					case Keys.F2:
						tv.SelectedNode.BeginEdit();
						break;
					case Keys.Delete:
						tv.Nodes.Remove(tv.SelectedNode);
						this.SaveFavorites();
						break;
				}
			}
		}
		
		private void TreeNodeDoubleClick(object sender, EventArgs e)
		{
			TreeNode tn = tv.SelectedNode;
			
			if (tn != null && tn.Tag != null && tn.Tag is string && (string)tn.Tag != "")
			{
				ShowHelpBrowser.OpenHelpView((string)tn.Tag);
			}
		}
		#endregion

		#region ToolStrip
		private void ToolStripButtonClicked(object sender, EventArgs e)
		{
			if (tv.SelectedNode == null) return;

			ToolStripItem item = (ToolStripItem)sender;
			TreeNode tempNode  = null;

			switch (item.ImageIndex)
			{
				case 0:
					tempNode              = (TreeNode)tv.SelectedNode.Clone();
					tv.Nodes.Insert(tv.SelectedNode.PrevNode.Index, tempNode);
					tv.Nodes.Remove(tv.SelectedNode);
					tv.SelectedNode       = tempNode;
					this.SaveFavorites();
					break;
				case 1:
					tempNode              = (TreeNode)tv.SelectedNode.Clone();
					TreeNode nextNextNode = tv.SelectedNode.NextNode.NextNode;
					if (nextNextNode == null)
						tv.Nodes.Add(tempNode);
					else
						tv.Nodes.Insert(nextNextNode.Index, tempNode);
					tv.Nodes.Remove(tv.SelectedNode);
					tv.SelectedNode       = tempNode;
					this.SaveFavorites();
					break;
				case 2:
					tv.SelectedNode.BeginEdit();
					break;
				case 3:
					string text = StringParser.Parse("${res:AddIns.HtmlHelp2.RemoveFavorite}", new string[,] {{"0", tv.SelectedNode.Text}});
					DialogResult result = MessageBox.Show(text,
					                                      StringParser.Parse("${res:MainWindow.Windows.HelpScoutLabel}"),
					                                      MessageBoxButtons.YesNo,
					                                      MessageBoxIcon.Question,
					                                      MessageBoxDefaultButton.Button2);
					if (result == DialogResult.Yes)
					{
						tv.Nodes.Remove(tv.SelectedNode);
						this.SaveFavorites();
					}
					break;
			}

			this.TreeNodeAfterSelect(null, null);
		}
		#endregion

		#region Favorites
		private void LoadFavorites()
		{
			tv.Nodes.Clear();
			tv.BeginUpdate();
			
			try
			{
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(PropertyService.ConfigDirectory + help2FavoritesFile);
				
				XmlNodeList nl = xmldoc.SelectNodes("favorites/favorite");
				for (int i = 0; i < nl.Count; i++)
				{
					XmlNode title = nl.Item(i).SelectSingleNode("title");
					XmlNode url   = nl.Item(i).SelectSingleNode("url");
					
					if (title != null && url != null && title.InnerText != "" && url.InnerText != "")
					{
						TreeNode node = new TreeNode();
						node.Text     = title.InnerText;
						node.Tag      = url.InnerText;
						tv.Nodes.Add(node);
					}
				}
			}
			catch {}
			
			tv.EndUpdate();
		}
		
		private void SaveFavorites()
		{
			try
			{
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.LoadXml("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><favorites/>");
				
				foreach (TreeNode node in tv.Nodes)
				{
					if (node.Text != "" && node.Tag != null && node.Tag is string && (string)node.Tag != "")
					{
						XmlNode favorite = xmldoc.CreateElement("favorite");
						
						XmlNode title    = xmldoc.CreateElement("title");
						title.InnerText  = node.Text;
						favorite.AppendChild(title);
						
						XmlCDataSection cdata = xmldoc.CreateCDataSection((string)node.Tag);
						XmlNode url           = xmldoc.CreateElement("url");
						url.AppendChild(cdata);
						favorite.AppendChild(url);
						xmldoc.DocumentElement.AppendChild(favorite);
					}
				}
				
				xmldoc.Save(PropertyService.ConfigDirectory + help2FavoritesFile);
			}
			catch {}
		}
		
		private void PatchFavoriteName(string newName, string topicUrl)
		{
			try
			{
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(PropertyService.ConfigDirectory + help2FavoritesFile);
				
				XmlNode node = xmldoc.SelectSingleNode(String.Format("/favorites/favorite[url=\"{0}\"]/title", topicUrl));
				
				if (node != null)
				{
					node.InnerText = newName;
					xmldoc.Save(PropertyService.ConfigDirectory + help2FavoritesFile);
				}
			}
			catch {}
		}
		
		public void AddToFavorites(string topicName, string topicUrl)
		{
			if (Help2EnvIsReady && topicName != "" && topicUrl != "")
			{
				bool urlFound = false;
				
				foreach (TreeNode node in tv.Nodes)
				{
					if (node.Tag != null &&
					    node.Tag is string &&
					    String.Compare(topicUrl, (string)node.Tag) == 0)
					{
						urlFound = true;
						break;
					}
				}
				
				if (!urlFound) 
				{
					TreeNode node = new TreeNode();
					node.Text     = topicName;
					node.Tag      = topicUrl;
					
					tv.Nodes.Add(node);
					this.SaveFavorites();
				}
			}
		}
		#endregion
	}
}
