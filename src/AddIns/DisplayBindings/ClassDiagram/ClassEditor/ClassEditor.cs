// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ClassDiagram
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class ClassEditor
	{
		TreeListView membersList = new TreeListView();
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ClassEditor));
		
		Dictionary<Type, TreeListViewItem> addMemberItems = new Dictionary<Type, TreeListViewItem>();
		Dictionary<IMethod, TreeListViewItem> addParameterItems = new Dictionary<IMethod, TreeListViewItem>();
		
		Dictionary<ClassType, Action<IClass>> classTypeGroupCreators = new Dictionary<ClassType, Action<IClass>>();
		IClass currClass;
		
		public event EventHandler<IMemberEventArgs> MemberActivated = delegate {};
		
		public ClassEditor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			ColumnHeader nameCol = membersList.Columns.Add("Name");
			ColumnHeader typeCol = membersList.Columns.Add("Type");
			ColumnHeader modifierCol = membersList.Columns.Add("Modifier");
			ColumnHeader summaryCol = membersList.Columns.Add("Summary");
			
			nameCol.Width = 160;
			typeCol.Width = 100;
			modifierCol.Width = 100;
			summaryCol.Width = 200;
			
			try
			{
				membersList.SmallImageList = ClassBrowserIconService.ImageList;
			}
			catch
			{
				membersList.SmallImageList = new ImageList();
			}
			
			//TODO - check with the #D documentation how to add new icons correctly.
			membersList.SmallImageList.Images.Add("OpenBrace", (Bitmap)resources.GetObject("openbrace"));
			membersList.SmallImageList.Images.Add("Comma", (Bitmap)resources.GetObject("comma"));
			membersList.SmallImageList.Images.Add("CloseBrace", (Bitmap)resources.GetObject("closebrace"));
			membersList.SmallImageList.Images.Add("EmptyBraces", (Bitmap)resources.GetObject("emptybraces"));
			
			membersList.Sorting = SortOrder.None;
			membersList.ShowPlusMinus = true;
			membersList.FullRowSelect = true;

			membersList.Items.Sortable = false;
			membersList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			
			membersList.DoubleClick += HandleDoubleClick;
			
			Controls.Add(membersList);
			membersList.Dock = DockStyle.Fill;
			
			classTypeGroupCreators.Add(ClassType.Class, SetClassGroups);
			classTypeGroupCreators.Add(ClassType.Interface, SetClassGroups);
			classTypeGroupCreators.Add(ClassType.Struct, SetClassGroups);
			classTypeGroupCreators.Add(ClassType.Enum, SetEnumGroups);
			classTypeGroupCreators.Add(ClassType.Delegate, SetDelegateGroups);
			//classTypeGroupCreators[ClassType.Module] = SetClassGroups; //???
		}
		
		private void HandleDoubleClick (object sender, EventArgs e)
		{
			if (membersList.SelectedItems.Count == 0) return;
			
			if (addMemberItems.ContainsValue(membersList.SelectedItems[0]))
			{
				
			}
			else if (addParameterItems.ContainsValue(membersList.SelectedItems[0]))
			{
				
			}
			else
			{
				IMember itemMember = membersList.SelectedItems[0].Tag as IMember;
				if (itemMember != null)
					MemberActivated(this, new IMemberEventArgs(itemMember));
			}
		}
		
		private void SetClassGroups (IClass classType)
		{
			AddGroup<IMethod>("Methods", "method", classType.Methods);
			AddGroup<IProperty>("Properties", "property", classType.Properties);
			AddGroup<IField>("Fields", "field", classType.Fields);
			AddGroup<IEvent>("Events", "event", classType.Events);
		}

		private void SetEnumGroups (IClass classType)
		{
			AddGroup<IField>("Fields", "field", classType.Fields);
		}
		
		private void SetDelegateGroups (IClass classType)
		{
		}
		
		public void SetClass (IClass classType)
		{
			membersList.BeginUpdate();
			membersList.Items.Clear();
			
			currClass = classType;
			if (classType != null)
				classTypeGroupCreators[classType.ClassType](classType);

			membersList.EndUpdate();
		}
		
		private TreeListViewItem AddGroup<MT>(string title, string type, ICollection<MT> members) where MT : IMember
		{
			if (members == null) return null;
			
			TreeListViewItem group = new TreeListViewItem(title);
			group.ForeColor = Color.Gray;
			group.Font = new Font(group.Font, FontStyle.Bold);
			group.Items.Sortable = false;
			
			if (members.Count != 0)
			{
				foreach (IMember member in members)
				{
					
					IMethod methodMember = member as IMethod;
					IEvent eventMember = member as IEvent;
					IProperty propertyMember = member as IProperty;
					IField fieldMember = member as IField;
					
					int icon = -1;
					try
					{
						icon = ClassBrowserIconService.GetIcon(member);
					}
					catch {}
					
					IAmbience ambience = GetAmbience();
					string memberName = "";
					
					if (methodMember != null)
					{
						if (methodMember.IsConstructor)
						{
							if (methodMember.DeclaringType != null)
							{
								memberName = methodMember.DeclaringType.Name;
							}
							else
							{
								memberName = methodMember.Name;
							}
						}
						else
						{
							memberName = methodMember.Name;
						}
					}
					if (eventMember != null)
					{
						memberName = eventMember.Name;
					}
					if (propertyMember != null) memberName = ambience.Convert(propertyMember);
					if (fieldMember != null) memberName = ambience.Convert(fieldMember);
					
					TreeListViewItem memberItem = new TreeListViewItem(memberName, icon);
					memberItem.Items.Sortable = false;
					memberItem.Items.SortOrder = SortOrder.None;
					memberItem.Tag = member;
					group.Items.Add(memberItem);
					
					if (methodMember != null)
						FillParams (memberItem, methodMember);
					
					memberItem.SubItems.Add(ambience.Convert(member.ReturnType));
					memberItem.SubItems.Add(member.Modifiers.ToString());
					
					memberItem.SubItems.Add(GetSummary(member));
				}
			}
			
			TreeListViewItem addNewMember = new TreeListViewItem(String.Format("<add {0}>", type));
			addNewMember.ForeColor = Color.Gray;
			group.Items.Add(addNewMember);
			
			addMemberItems[typeof(MT)] = addNewMember;
			
			membersList.Items.Add(group);
			return group;
		}
		
		private string GetSummary (IDecoration decoration)
		{
			StringReader strReader = new StringReader("<docroot>" + decoration.Documentation + "</docroot>");
			XmlDocument doc = new XmlDocument();
			doc.Load(strReader);
			XPathNavigator nav = doc.CreateNavigator();
			XPathNodeIterator ni = nav.Select(@"/docroot/summary");
			if (ni.MoveNext())
				return ni.Current.InnerXml;
			else
				return String.Empty;
		}
		
		private void FillParams(TreeListViewItem item, IMethod method)
		{
			string imageKey = "OpenBrace";
			foreach (IParameter param in method.Parameters)
			{
				TreeListViewItem parameter = new TreeListViewItem(param.Name);
				parameter.ImageKey = imageKey;
				parameter.SubItems.Add (param.ReturnType.Name);
				item.Items.Add(parameter);
				imageKey = "Comma";
			}
			TreeListViewItem addParam = new TreeListViewItem("<add parameter>");
			if (imageKey == "OpenBrace")
				addParam.ImageKey = "EmptyBraces";
			else
				addParam.ImageKey = "CloseBrace";
			addParam.ForeColor = Color.Gray;
			item.Items.Add (addParam);
			addParameterItems[method] = addParam;
		}
		
		protected IAmbience GetAmbience()
		{
			IAmbience ambience = null;
			
			try
			{
				ambience = AmbienceService.CurrentAmbience;
			}
			catch (NullReferenceException)
			{
				ambience = ICSharpCode.SharpDevelop.Dom.CSharp.CSharpAmbience.Instance;
			}
			
			ambience.ConversionFlags = ConversionFlags.None;
			
			return ambience;
		}
		
		private static TextEditorControl GetTextEditorControl()
		{
			TextEditorControl control1 = null;
			IWorkbenchWindow window1 = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if ((window1 != null) && (window1.ViewContent is ITextEditorControlProvider))
			{
				control1 = ((ITextEditorControlProvider) window1.ViewContent).TextEditorControl;
			}
			return control1;
		}
	}
	
	public class IMemberEventArgs : EventArgs
	{
		IMember member;
		
		public IMemberEventArgs(IMember member)
		{
			this.member = member;
		}
		
		public IMember Member
		{
			get { return member; }
		}
	}
}
