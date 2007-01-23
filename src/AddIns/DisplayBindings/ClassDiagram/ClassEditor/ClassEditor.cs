/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 20/10/2006
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
using ICSharpCode.SharpDevelop.Project;
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
		
		VisibilityModifiersEditor visibilityModifierEditor = new VisibilityModifiersEditor();
		ParameterModifiersEditor parameterModifierEditor = new ParameterModifiersEditor();
		
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ClassEditor));
		
		Dictionary<Type, TreeListViewItem> addMemberItems = new Dictionary<Type, TreeListViewItem>();
		Dictionary<IMethod, TreeListViewItem> addParameterItems = new Dictionary<IMethod, TreeListViewItem>();
		
		Dictionary<ClassType, Action<IClass>> classTypeGroupCreators = new Dictionary<ClassType, Action<IClass>>();
		IClass currClass;
		
		public event EventHandler<IMemberEventArgs> MemberActivated = delegate {};
		public event EventHandler<IParameterEventArgs> ParameterActivated = delegate {};
		
		public event EventHandler<IMemberModificationEventArgs> MemberModified = delegate {};
		public event EventHandler<IParameterModificationEventArgs> ParameterModified = delegate {};
		
		public event EventHandler<IMemberEventArgs> ClassMemberAdded = delegate {};
		
		TreeListViewItem editedItem = null;
		
		ColumnHeader nameCol;
		ColumnHeader typeCol;
		ColumnHeader modifierCol;
		ColumnHeader summaryCol;
		
		public ClassEditor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			nameCol = membersList.Columns.Add("Name");
			typeCol = membersList.Columns.Add("Type");
			modifierCol = membersList.Columns.Add("Modifier");
			summaryCol = membersList.Columns.Add("Summary");
			
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
			
			membersList.LabelEdit = true;
			membersList.Sorting = SortOrder.None;
			membersList.ShowPlusMinus = true;
			membersList.FullRowSelect = true;

			membersList.Items.Sortable = false;
			membersList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			
			membersList.DoubleClick += HandleDoubleClick;
			membersList.BeforeLabelEdit += BeforeEdit;
			membersList.AfterLabelEdit += AfterEdit;
			
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
			
			TreeListViewItem item = membersList.SelectedItems[0];
			
			if (addMemberItems.ContainsValue(item))
			{
				/*				IAmbience ambience = GetAmbience();
				item.SubItems.Add(ambience.Convert(VoidReturnType.Instance));
				item.SubItems.Add(ModifierEnum.Public.ToString());
				item.SubItems.Add("");
				item.Text = "[method name]";
				item.BeginEdit(0);*/
			}
			else if (addParameterItems.ContainsValue(item))
			{
				
			}
			else
			{
				IMember itemMember = item.Tag as IMember;
				IParameter itemParameter = item.Tag as IParameter;
				if (itemMember != null)
				{
					MemberActivated(this, new IMemberEventArgs(itemMember));
				}
				else if (itemParameter != null)
				{
					IMethod method = item.Parent.Tag as IMethod;
					if (method != null)
						ParameterActivated(this, new IParameterEventArgs(method, itemParameter));
				}
			}
		}
		
		private void BeforeEdit(object sender, TreeListViewBeforeLabelEditEventArgs e)
		{
			editedItem = membersList.SelectedItems[0];

			IMember itemMember = editedItem.Tag as IMember;
			IParameter itemParameter = editedItem.Tag as IParameter;
			if (itemMember != null)
			{
				if (e.ColumnIndex == nameCol.Index)
				{
				}
				else if (e.ColumnIndex == typeCol.Index)
				{
				}
				else if (e.ColumnIndex == modifierCol.Index)
				{
					e.Editor = visibilityModifierEditor;
				}
				else if (e.ColumnIndex == summaryCol.Index)
				{
				}
				else
				{
					e.Cancel = true;
				}
			}
			else if (itemParameter != null)
			{
				if (e.ColumnIndex == nameCol.Index)
				{
				}
				else if (e.ColumnIndex == typeCol.Index)
				{
				}
				else if (e.ColumnIndex == modifierCol.Index)
				{
					e.Editor = parameterModifierEditor;
				}
				else if (e.ColumnIndex == summaryCol.Index)
				{
				}
				else
				{
					e.Cancel = true;
				}
			}

			if (addParameterItems.ContainsValue(editedItem))
			{
				e.Cancel = true;
			}
		}
		
		private void AfterEdit (object sender, TreeListViewLabelEditEventArgs e)
		{
			IProjectContent pc = ProjectService.CurrentProject.CreateProjectContent();
			Modification modification = Modification.None;
			if (e.ColumnIndex == nameCol.Index)
			{
				modification = Modification.Name;
			}
			else if (e.ColumnIndex == typeCol.Index)
			{
				modification = Modification.Type;
			}
			else if (e.ColumnIndex == modifierCol.Index)
			{
				modification = Modification.Modifier;
			}
			else if (e.ColumnIndex == summaryCol.Index)
			{
				modification = Modification.Summary;
			}
			else
			{
				e.Cancel = true;
			}
			
			if (e.Cancel) return;

			IMember member = null;
			
			if (addMemberItems.ContainsValue(editedItem))
			{
				Type parentGroupType = editedItem.Parent.Tag as Type;
				if (parentGroupType == typeof(IMethod))
				{
					member = new DefaultMethod(currClass, e.Label);
					member.ReturnType = pc.SystemTypes.Void;
				}
				else if (parentGroupType == typeof(IField))
				{
					member = new DefaultField(currClass, e.Label);
					if (currClass.ClassType == ClassType.Enum)
						member.ReturnType = pc.SystemTypes.Int32;
					else
						member.ReturnType = pc.SystemTypes.Object;
				}
				else if (parentGroupType == typeof(IProperty))
				{
					member = new DefaultProperty(currClass, e.Label);
					member.ReturnType = pc.SystemTypes.Object;
				}
				else if (parentGroupType == typeof(IEvent))
				{
					member = new DefaultEvent(currClass, e.Label);
					member.ReturnType = pc.SystemTypes.CreatePrimitive(typeof(EventHandler));
				}

				ConvertAddItemToMemberItem(editedItem, member, GetAmbience());
				AddAddItem(parentGroupType, editedItem.Parent);
				
				IReturnType memberType = VoidReturnType.Instance;
				
				IMemberEventArgs memberargs = new IMemberEventArgs(member);
				ClassMemberAdded(this, memberargs);
				return;
			}
			
			member = editedItem.Tag as IMember;
			IParameter parameter = editedItem.Tag as IParameter;
			if (member != null)
			{
				IMemberModificationEventArgs mmea = new IMemberModificationEventArgs(member, modification, e.Label);
				MemberModified(this, mmea);
				e.Cancel = mmea.Cancel;
			}
			else if (parameter != null)
			{
				IMethod method = editedItem.Parent.Tag as IMethod;
				if (method == null)
				{
					e.Cancel = true;
				}
				else
				{
					IParameterModificationEventArgs pmea = new IParameterModificationEventArgs(method, parameter, modification, e.Label);
					ParameterModified(this, pmea);
					e.Cancel = pmea.Cancel;
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		
		private void SetClassGroups (IClass classType)
		{
			AddGroup<IMethod>("Methods", classType.Methods);
			AddGroup<IProperty>("Properties", classType.Properties);
			AddGroup<IField>("Fields", classType.Fields);
			AddGroup<IEvent>("Events", classType.Events);
		}

		private void SetEnumGroups (IClass classType)
		{
			AddGroup<IField>("Fields", classType.Fields);
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
		
		private TreeListViewItem AddGroup<MT>(string title, ICollection<MT> members) where MT : IMember
		{
			if (members == null) return null;
			
			TreeListViewItem group = new TreeListViewItem(title);
			group.ForeColor = Color.Gray;
			group.Font = new Font(group.Font, FontStyle.Bold);
			group.Items.Sortable = false;
			group.Tag = typeof(MT);
			
			IAmbience ambience = GetAmbience();
			
			if (members.Count != 0)
			{
				foreach (IMember member in members)
				{
					TreeListViewItem memberItem = CreateMemberItem(member, ambience);

					IMethod methodMember = memberItem.Tag as IMethod;
					
					group.Items.Add(memberItem);
					
					if (methodMember != null)
						FillParams (memberItem, methodMember);
					
					
					memberItem.SubItems.Add(ambience.Convert(member.ReturnType));
					memberItem.SubItems.Add(member.Modifiers.ToString());
					memberItem.SubItems.Add(GetSummary(member));
				}
			}
			
			AddAddItem(typeof(MT), group);
			
			membersList.Items.Add(group);
			return group;
		}
		
		private TreeListViewItem CreateMemberItem(IMember member, IAmbience ambience)
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
			return memberItem;
		}
		
		private void AddAddItem(Type memberType, TreeListViewItem group)
		{
			string str = "";
			
			if (memberType == typeof(IMethod)) str = "<add method>";
			else if (memberType == typeof(IEvent)) str = "<add event>";
			else if (memberType == typeof(IProperty)) str = "<add property>";
			else if (memberType == typeof(IField)) str = "<add field>";
			
			TreeListViewItem addNewMember = new TreeListViewItem(str);
			addNewMember.ForeColor = SystemColors.GrayText;
			group.Items.Add(addNewMember);
			addMemberItems[memberType] = addNewMember;
		}
		
		private void ConvertAddItemToMemberItem(TreeListViewItem addItem, IMember member, IAmbience ambience)
		{
			addItem.ForeColor = SystemColors.ControlText;
			addItem.SubItems.Add(ambience.Convert(member.ReturnType));
			addItem.SubItems.Add(member.Modifiers.ToString());
			addItem.SubItems.Add(GetSummary(member));
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
				parameter.SubItems.Add (param.Modifiers.ToString());
				parameter.SubItems.Add (param.Documentation);
				item.Items.Add(parameter);
				parameter.Tag = param;
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
			ITextEditorControlProvider textEditorProvider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorControlProvider;
			if (textEditorProvider != null)
				return textEditorProvider.TextEditorControl;
			else
				return null;
		}
	}

	public enum Modification { None, Name, Type, Modifier, Summary }
}
