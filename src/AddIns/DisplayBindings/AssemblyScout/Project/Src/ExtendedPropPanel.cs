// created on 11.12.2002 at 16:10
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout {
	
	public class ExtendedPropsPanel : System.Windows.Forms.Panel
	{
		PropertyViewer props;
		AssemblyTree _tree;
		
		public ExtendedPropsPanel(AssemblyTree tree)
		{
			_tree = tree;
			_tree.AfterSelect += new TreeViewEventHandler(SelectNode);
			
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			Dock         = DockStyle.Fill;
			
			props        = new PropertyViewer(_tree);
			props.Size   = new Size(Width, Height);
			props.Dock   = DockStyle.Fill;
			props.BackClick += new EventHandler(back_click);
			
			Controls.Add(props);
		}
		
		void back_click(object sender, EventArgs e)
		{
			try {
				_tree.GoBack();
			} catch {}
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			AssemblyTreeNode node = (AssemblyTreeNode)e.Node;
			object toShow = node.Attribute;
			
			switch(node.Type) {
				case NodeType.Assembly:
				case NodeType.Library:
				case NodeType.Reference:
				case NodeType.Module:
				case NodeType.Link:
				case NodeType.Type:
				case NodeType.Constructor:
				case NodeType.Event:
				case NodeType.Field:
				case NodeType.Method:
				case NodeType.Property:
					break;
				case NodeType.Resource:
				case NodeType.Folder:
				case NodeType.Namespace:
				case NodeType.SubTypes:
				case NodeType.SuperTypes:
					toShow = null;
					break;
				default:
					toShow = null;
					break;
			}
			props.ShowObject(toShow, node.Text, node.Type.ToString());
		}
	}

	public class PropertyViewer : System.Windows.Forms.UserControl
	{
		ListView list          = new ListView();
		GradientLabel cap      = new GradientLabel();
		Label typ              = new Label();
		LinkLabel back         = new LinkLabel();
		ColumnHeader namecol;
		ColumnHeader valuecol;
		
		AssemblyTree tree;
		
		public event EventHandler BackClick;
		
		private System.ComponentModel.Container components = null;

		public PropertyViewer(AssemblyTree _tree)
		{
			tree = _tree;
			InitializeComponent();

		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void InitializeComponent()
		{	
			cap.Location  = new Point(0, 0);
			cap.Size      = new Size(Width, 32);
			cap.Text      = tree.ress.GetString("ObjectBrowser.Welcome");
			cap.Font      = new Font("Tahoma", 14);
			cap.BackColor = SystemColors.ControlLight;
			cap.TextAlign = ContentAlignment.MiddleLeft;
			cap.Anchor    = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
						
			string backt  = tree.ress.GetString("ObjectBrowser.Back");
			back.Size     = new Size(40, 16);
			back.Location = new Point(Width - back.Width, 44);
			back.Text     = backt;
			back.TextAlign = ContentAlignment.TopRight;
			back.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
			back.Links.Add(0, backt.Length);
			back.LinkClicked += new LinkLabelLinkClickedEventHandler(back_click);
			
			typ.Location  = new Point(0, 44);
			typ.Size      = new Size(Width - back.Width, 16);
			typ.Font      = new Font(Font, FontStyle.Bold);
			typ.Text      = tree.ress.GetString("ObjectBrowser.WelcomeText");
			typ.TextAlign = ContentAlignment.TopLeft;
			typ.Anchor    = cap.Anchor;
						
			list.Location   = new Point(0, 72);
			list.Size       = new Size(Width, Height - list.Top);
			list.Anchor     = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			list.FullRowSelect = true;
			list.GridLines  = true;
			list.View       = View.Details;
			
			namecol = new ColumnHeader();
			namecol.Text = tree.ress.GetString("ObjectBrowser.Props.Property");
			namecol.Width = 120;
			
			valuecol = new ColumnHeader();
			valuecol.Text = tree.ress.GetString("ObjectBrowser.Props.Value");
			valuecol.Width = 300;
			
			list.Columns.Add(namecol);
			list.Columns.Add(valuecol);
			
			Controls.AddRange(new System.Windows.Forms.Control[] {cap, typ, list, back});
		}

		public void ShowObject(object obj, string title, string subtitle)
		{
			cap.Text = title;
			typ.Text = subtitle;
			list.Items.Clear();
			
			if (obj == null)
			{
				return;
			}
	
			SetProps(obj);

		}

		void SetProps(object o)
		{
			Type type = o.GetType();
			PropertyInfo[] pi = type.GetProperties();

			for(int i=0; i < pi.Length; ++i)
			{
				object ret = null;
				try
				{
					ret = type.InvokeMember(pi[i].Name, BindingFlags.GetProperty, null, o, new object [] {});

					AddItem(pi[i].Name, (ret == null) ? "<null>" : ret.ToString());
				}
				catch
				{
				}
			}
		}

		void AddItem(string name, string val)
		{
			list.Items.Add(new ListViewItem(new string[] {name, val}));
		}
		
		void back_click(object sender, LinkLabelLinkClickedEventArgs ev)
		{
			BackClick(this, EventArgs.Empty);
		}
		
	}
}
