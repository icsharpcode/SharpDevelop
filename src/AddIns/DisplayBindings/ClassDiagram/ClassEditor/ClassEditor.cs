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
			
			if (addMemberItems.ContainsValue(membersList.SelectedItems[0]))
			{
				
			}
			else if (addParameterItems.ContainsValue(membersList.SelectedItems[0]))
			{
				
			}
			else
			{
				IMember itemMember = membersList.SelectedItems[0].Tag as IMember;
				IParameter itemParameter = membersList.SelectedItems[0].Tag as IParameter;
				if (itemMember != null)
				{
					MemberActivated(this, new IMemberEventArgs(itemMember));
				}
				else if (itemParameter != null)
				{
					IMethod method = membersList.SelectedItems[0].Parent.Tag as IMethod;
					if (method != null)
						ParameterActivated(this, new IParameterEventArgs(method, itemParameter));
				}
			}
		}
		
		private void BeforeEdit(object sender, TreeListViewBeforeLabelEditEventArgs e)
		{
			editedItem = membersList.SelectedItems[0];
			if (addMemberItems.ContainsValue(editedItem))
			{
				e.Cancel = true;
			}
			else if (addParameterItems.ContainsValue(editedItem))
			{
				e.Cancel = true;
			}
			else
			{
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
				else
				{
					e.Cancel = true;
				}
			}
		}
		
		private void AfterEdit (object sender, TreeListViewLabelEditEventArgs e)
		{
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
			
			if (!e.Cancel)
			{
				IMember member = editedItem.Tag as IMember;
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
		public IMemberEventArgs (IMember member)
		{
			this.member = member;
		}
		
		public IMember Member
		{
			get { return member; }
		}
	}
	
	
	public class IParameterEventArgs : EventArgs
	{
		IParameter parameter;
		IMethod method;
		public IParameterEventArgs (IMethod method, IParameter parameter)
		{
			this.method = method;
			this.parameter = parameter;
		}
		
		public IParameter Parameter
		{
			get { return parameter; }
		}
		
		public IMethod Method
		{
			get { return method; }
		}
	}
	
	public enum Modification { None, Name, Type, Modifier, Summary }

	public class IMemberModificationEventArgs : IMemberEventArgs
	{
		Modification modification;
		string newValue;
		
		bool cancel = false;
		
		public IMemberModificationEventArgs(IMember member, Modification modification, string newValue)
			: base (member)
		{
			this.modification = modification;
			this.newValue = newValue;
		}
		
		public Modification Modification
		{
			get { return modification; }
		}
		
		public string NewValue
		{
			get { return newValue; }
		}
		
		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}
	}
	
	public class IParameterModificationEventArgs : IParameterEventArgs
	{
		Modification modification;
		string newValue;
		
		bool cancel = false;
		
		public IParameterModificationEventArgs(IMethod method, IParameter parameter, Modification modification, string newValue)
			: base (method, parameter)
		{
			this.modification = modification;
			this.newValue = newValue;
		}
		
		public Modification Modification
		{
			get { return modification; }
		}
		
		public string NewValue
		{
			get { return newValue; }
		}
		
		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}
	}
	
	internal class VisibilityModifiersEditor : ComboBox
	{
		public VisibilityModifiersEditor()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Public);
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Private);
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Protected);
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Internal);
		}
	}
	
	internal class ParameterModifiersEditor : ComboBox
	{
		public ParameterModifiersEditor()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			Items.Add(ParameterModifiers.In);
			Items.Add(ParameterModifiers.Out);
			Items.Add(ParameterModifiers.Ref);
			Items.Add(ParameterModifiers.Params);
			Items.Add(ParameterModifiers.Optional);
		}
	}
}
