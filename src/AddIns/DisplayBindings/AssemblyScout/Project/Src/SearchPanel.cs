// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Threading;
using System.Resources;

using ICSharpCode.Core;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Dom;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class SearchPanel : UserControl
	{
		System.Windows.Forms.Label searchfor  = new System.Windows.Forms.Label();
		System.Windows.Forms.Label foundcount = new System.Windows.Forms.Label();
		TextBox   searchstringbox = new TextBox();
		ListView  itemsfound      = new ListView();
		Button    button          = new Button();
		ComboBox  searchtypes     = new ComboBox();
		
		AssemblyTree tree;
		AssemblyScoutViewContent _parent;
		
		public SearchPanel(AssemblyTree tree)
		{
			Dock = DockStyle.Fill;
			
			this.tree = tree;
			
			searchfor.Text     = tree.ress.GetString("ObjectBrowser.Search.SearchFor");
			searchfor.Location = new Point(0, 0);
			searchfor.Size     = new Size(70, 12);
			searchfor.Anchor   = AnchorStyles.Top | AnchorStyles.Left;
			searchfor.FlatStyle = FlatStyle.System;
			
			foundcount.Text      = "0 " + tree.ress.GetString("ObjectBrowser.Search.ItemsFound");
			foundcount.Location  = new Point(searchfor.Width + 5, 0);
			foundcount.Size      = new Size(Width - searchfor.Width - 5, searchfor.Height);
			foundcount.TextAlign = ContentAlignment.TopRight;
			foundcount.Anchor    = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
			foundcount.FlatStyle = FlatStyle.System;
			
			searchstringbox.Location = new Point(0, 17);
			searchstringbox.Width    = Width;
			searchstringbox.Height   = 30;
			searchstringbox.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			searchstringbox.KeyUp    += new KeyEventHandler(searchbox_keyup);
			
			button.Location = new Point(Width - 52, 44);
			button.Size     = new Size(52, 21);
			button.Text     = tree.ress.GetString("ObjectBrowser.Search.Search");
			button.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
			button.Click    += new EventHandler(DoSearch);
			button.FlatStyle = FlatStyle.System;
			
			searchtypes.Location      = new Point(0, 44);
			searchtypes.Width         = Width - 60;
			searchtypes.Height        = 30;
			searchtypes.Anchor        = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			searchtypes.DropDownStyle = ComboBoxStyle.DropDownList;
			searchtypes.Items.Add(tree.ress.GetString("ObjectBrowser.Search.TypesAndMembers"));
			searchtypes.Items.Add(tree.ress.GetString("ObjectBrowser.Search.TypesOnly"));
			searchtypes.SelectedIndex = 0;
			
			itemsfound.Location       = new Point(0, 71);
			itemsfound.Width          = Width;
			itemsfound.FullRowSelect  = true;
			itemsfound.MultiSelect    = false;
			itemsfound.Height         = Height - 71;
			itemsfound.Anchor         = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			itemsfound.View           = View.Details;
			itemsfound.SmallImageList = tree.ImageList;
			
			itemsfound.Columns.Add(tree.ress.GetString("ObjectBrowser.Search.Name"), 160, HorizontalAlignment.Left);
			itemsfound.Columns.Add(tree.ress.GetString("ObjectBrowser.Search.Type"),  70, HorizontalAlignment.Left);
			itemsfound.Columns.Add("Namespace", 125, HorizontalAlignment.Left);
			itemsfound.Columns.Add("Assembly",  75, HorizontalAlignment.Left);
			itemsfound.DoubleClick += new EventHandler(SelectItem);
			
			Controls.Add(button);
			Controls.Add(searchfor);
			Controls.Add(foundcount);
			Controls.Add(searchstringbox);
			Controls.Add(itemsfound);
			Controls.Add(searchtypes);
		}
		
		public AssemblyScoutViewContent ParentDisplayInfo {
			get {
				return _parent;
			}
			set {
				_parent = value;
			}
		}
		
		void SelectItem(object sender, EventArgs e)
		{
			if (itemsfound.SelectedItems.Count != 1)
				return;
			
			if(itemsfound.SelectedItems[0] is TypeItem) {
				TypeItem item = (TypeItem)itemsfound.SelectedItems[0];
				tree.GoToType(item.type);
				
			} else if (itemsfound.SelectedItems[0] is MemberItem) {
				MemberItem member = (MemberItem)itemsfound.SelectedItems[0];
				tree.GoToMember(member.member);
			}
						
			//ParentDisplayInfo.leftTabs.ActiveDocument = 
			ParentDisplayInfo.leftTabs.Documents[0].Activate();
		}
		
		class TypeItem : ListViewItem {
			public IClass type;
			public TypeItem(string Namespace, IClass type, Color forecolor) : 
				base (new string[] {type.Name, GetType(type), Namespace, ((SA.SharpAssembly)type.DeclaredIn).Name})
			{
				this.type = type;
				this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(type);
				this.ForeColor = forecolor;
			}

			private static string GetType(IClass type) {
				if(type.ClassType == ClassType.Enum) {
					return "Enum";
				} else if(type.ClassType == ClassType.Interface) {
					return "Interface";
				} else if(type.ClassType == ClassType.Struct) {
					return "Structure";
				} else {
					return "Class";
				}
			}
		}

		
		class MemberItem : ListViewItem {
			public IMember member;
			
			public MemberItem(string Namespace, IMember member, Color forecolor) : 
				base (new string[] {member.DeclaringType.Name + "." + member.Name, GetType(member), Namespace, ((SA.SharpAssembly)member.DeclaringType.DeclaredIn).Name})
			{
				this.ForeColor = forecolor;
				this.member = member;
				if(member is IMethod) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as IMethod);
				} else if(member is IField) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as IField);
				} else if(member is IProperty) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as IProperty);
				} else if(member is IEvent) {
					this.ImageIndex = ((ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService))).GetIcon(member as IEvent);
				}
			}
			
			private static string GetType(IMember member) {
				if(member is IMethod) {
					if ((member as IMethod).IsConstructor) return "Constructor";
					return "Method";
				} else if(member is IField) {
					return "Field";
				} else if(member is IProperty) {
					return "Property";
				} else if(member is IEvent) {
					return "Event";
				} else {
					return "unknown";
				}
			}
		}
		
		void searchbox_keyup(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				DoSearch(sender, new EventArgs());
		}
		
		void DoSearch(object sender, EventArgs e)
		{
			bool searchMembers = (searchtypes.SelectedIndex == 0);
			
			if(searchstringbox.Text == "") return;
			string searchfor = searchstringbox.Text.ToLower();
			
			itemsfound.Items.Clear();
			itemsfound.BeginUpdate();
			
			foreach (AssemblyTreeNode tn in tree.Nodes) {  // assembly nodes
				SA.SharpAssembly assembly = (SA.SharpAssembly)tn.Attribute;
				foreach (AssemblyTreeNode libnode in tn.Nodes) {
					if (libnode.Type != NodeType.Library) continue;
					foreach (AssemblyTreeNode nsnode in libnode.Nodes) {
						if (nsnode.Type == NodeType.Type) {
							ProcessType("", searchfor, searchMembers, (IClass)nsnode.Attribute, nsnode.ForeColor);
							continue;
						}
						foreach (AssemblyTreeNode typenode in nsnode.Nodes) {
							ProcessType(nsnode.Text, searchfor, searchMembers, (IClass)typenode.Attribute, nsnode.ForeColor);
						}
					}
				}
			}
			
			itemsfound.EndUpdate();
			foundcount.Text = itemsfound.Items.Count.ToString() + " " + tree.ress.GetString("ObjectBrowser.Search.ItemsFound");
		}
		
		void ProcessType(string Namespace, string searchfor, bool searchMembers, IClass type, Color nodecolor)
		{
			if (type.Name.ToLower().IndexOf(searchfor) >= 0) {
				itemsfound.Items.Add(new TypeItem(Namespace, type, nodecolor));
			}
			
			if (!searchMembers) return;
			
			//if (!type.MembersLoaded) type.LoadMembers();
			
			foreach (IMethod method in type.Methods)
				ProcessMember(Namespace, method, searchfor);
			foreach (IProperty prop in type.Properties)
				ProcessMember(Namespace, prop, searchfor);
			foreach (IField field in type.Fields)
				ProcessMember(Namespace, field, searchfor);
			foreach (IEvent evt in type.Events)
				ProcessMember(Namespace, evt, searchfor);
		}
		
		private void ProcessMember(string Namespace, IMember member, string searchfor)
		{
			if(member is IMethod) {
				if (member.IsSpecialName) return;
			}
			
			if(member.IsPrivate && (tree.showPrivateMembers == ShowOptions.Hide)) return;
			if(member.IsInternal && (tree.showInternalMembers == ShowOptions.Hide)) return;
			
			if(member.Name.ToLower().IndexOf(searchfor) >= 0) {
				Color color = SystemColors.WindowText;
				if ((member.IsInternal && tree.showInternalMembers == ShowOptions.GreyOut) || 
				    (member.IsPrivate  && tree.showPrivateMembers  == ShowOptions.GreyOut)) {
					color = SystemColors.GrayText;
				}
				itemsfound.Items.Add(new MemberItem(Namespace, member, color));
			}
		}
		
	}
}
