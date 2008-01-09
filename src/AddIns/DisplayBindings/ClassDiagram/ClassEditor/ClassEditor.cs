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
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace ClassDiagram
{
	public partial class ClassEditor
	{
		VisibilityModifiersEditor visibilityModifierEditor = new VisibilityModifiersEditor();
		ParameterModifiersEditor parameterModifierEditor = new ParameterModifiersEditor();
		
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ClassEditor));
		
		Dictionary<Type, Node> addMemberItems = new Dictionary<Type, Node>();
		Dictionary<IMethod, Node> addParameterItems = new Dictionary<IMethod, Node>();
		
		Dictionary<ClassType, Action<IClass>> classTypeGroupCreators = new Dictionary<ClassType, Action<IClass>>();
		IClass currClass;
		
		public event EventHandler<IMemberEventArgs> MemberActivated = delegate {};
		public event EventHandler<IParameterEventArgs> ParameterActivated = delegate {};
		
		public event EventHandler<IMemberModificationEventArgs> MemberModified = delegate {};
		public event EventHandler<IParameterModificationEventArgs> ParameterModified = delegate {};
		
		public event EventHandler<IMemberEventArgs> ClassMemberAdded = delegate {};
		
		TreeModel model = new TreeModel();
			
		private class GroupNode : Node
		{
			public GroupNode(Type groupType, string title)
				: base (title)
			{
				this.groupType = groupType;
			}
			
			Type groupType;
			
			public Type GroupType {
				get { return groupType; }
			}
			
			public string MemberName
			{
				get { return base.Text; }
			}
		}
		
		private class MemberNode : Node
		{
			public MemberNode(IMember member, IAmbience ambience, ClassEditor editor)
			{
				this.member = member;
				this.ambience = ambience;
				this.editor = editor;
				UpdateValues();
			}
			
			void UpdateValues()
			{
				IMethod methodMember = member as IMethod;
				IEvent eventMember = member as IEvent;
				IProperty propertyMember = member as IProperty;
				IField fieldMember = member as IField;
				
				int iconIndex = -1;
				try
				{
					iconIndex = ClassBrowserIconService.GetIcon(member);
				}
				catch {}
				
				if (iconIndex > -1)
					icon = ClassBrowserIconService.ImageList.Images[iconIndex];
				
				if (methodMember != null) memberName = ambience.Convert(methodMember);
				if (eventMember != null) memberName = ambience.Convert(eventMember);
				if (propertyMember != null) memberName = ambience.Convert(propertyMember);
				if (fieldMember != null) memberName = ambience.Convert(fieldMember);
			}

			IAmbience ambience;
			IMember member;
			ClassEditor editor;
			
			public IMember Member
			{
				get { return member; }
				set { member = value; UpdateValues(); }
			}
			
			private string memberName;
			public string MemberName
			{
				get { return memberName; }
				set
				{
					IMemberModificationEventArgs mmea = new IMemberModificationEventArgs(member, Modification.Name, value);
					editor.EmitMemberModified(mmea);
				}
			}
			
			protected Image icon;
			public Image MemberIcon
			{
				get { return icon; }
			}
			
			public string MemberType
			{
				get { return ambience.Convert(member.ReturnType); }
				set
				{
					IMemberModificationEventArgs mmea = new IMemberModificationEventArgs(member, Modification.Type, value);
					editor.EmitMemberModified(mmea);
				}
			}
			
			public string MemberModifiers
			{
				get { return member.Modifiers.ToString(); }
				set
				{
					IMemberModificationEventArgs mmea = new IMemberModificationEventArgs(member, Modification.Modifier, value);
					editor.EmitMemberModified(mmea);
				}
			}
			
			public string MemberSummary
			{
				get { return ClassEditor.GetSummary(member); }
				set
				{
					IMemberModificationEventArgs mmea = new IMemberModificationEventArgs(member, Modification.Summary, value);
					editor.EmitMemberModified(mmea);
				}
			}
		}
		
		private class ParameterNode : Node
		{
			public ParameterNode(IMethod method, IParameter parameter, IAmbience ambience, Image icon, ClassEditor editor)
			{
				this.ambience = ambience;
				this.icon = icon;
				this.parameter = parameter;
				this.editor = editor;
				this.method = method;
			}

			IAmbience ambience;
			IParameter parameter;
			IMethod method;
			ClassEditor editor;
			Image icon;
			
			public IParameter Parameter {
				get { return parameter; }
				set { parameter = value; }
			}
			
			public IMethod Method {
				get { return method; }
				set { method = value; }
			}
			
			public string MemberName
			{
				get { return parameter.Name; }
				set
				{
					IParameterModificationEventArgs pmea = new IParameterModificationEventArgs(method, parameter, Modification.Name, value);
					editor.EmitParameterModified(pmea);
				}
			}
			
			public Image MemberIcon
			{
				get { return icon; }
				set { icon = value; }
			}
			
			public string MemberType
			{
				get { return ambience.Convert(parameter.ReturnType); }
				set
				{
					IParameterModificationEventArgs pmea = new IParameterModificationEventArgs(method, parameter, Modification.Type, value);
					editor.EmitParameterModified(pmea);
				}

			}
			
			public string MemberModifiers
			{
				get { return parameter.Modifiers.ToString(); }
				set
				{
					IParameterModificationEventArgs pmea = new IParameterModificationEventArgs(method, parameter, Modification.Modifier, value);
					editor.EmitParameterModified(pmea);
				}
			}
			
			public string MemberSummary
			{
				get { return ClassEditor.GetParameterSummary(method, parameter.Name); }
				set
				{
					IParameterModificationEventArgs pmea = new IParameterModificationEventArgs(method, parameter, Modification.Summary, value);
					editor.EmitParameterModified(pmea);
				}
			}
		}
		
		private class AddMemberNode : Node
		{
			public AddMemberNode (string str) : base (str) {}
			
			public string MemberName {
				get { return base.Text; }
			}
		}
		
		private class AddParameterNode : Node
		{
			public AddParameterNode (Image icon) : base ("<Add Parameter>")
			{
				this.icon = icon;
			}
			
			Image icon;
			public Image MemberIcon {
				get { return icon; }
			}
			
			public string MemberName {
				get { return base.Text; }
			}
		}
		
		public ClassEditor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			membersList.Model = model;
			
			classTypeGroupCreators.Add(ClassType.Class, SetClassGroups);
			classTypeGroupCreators.Add(ClassType.Interface, SetClassGroups);
			classTypeGroupCreators.Add(ClassType.Struct, SetClassGroups);
			classTypeGroupCreators.Add(ClassType.Enum, SetEnumGroups);
			classTypeGroupCreators.Add(ClassType.Delegate, SetDelegateGroups);
			//classTypeGroupCreators[ClassType.Module] = SetClassGroups; //???
		}
		
		private void InitTypeList()
		{
			_type.DropDownItems.Clear();
			
			IProjectContent pc = ProjectService.CurrentProject.CreateProjectContent();
			foreach (IClass c in pc.Classes)
				_type.DropDownItems.Add(c.Name);
			_type.DropDownItems.Sort();
		}
		
		private void HandleDoubleClick (object sender, EventArgs e)
		{

		}
		
		internal void EmitMemberModified(IMemberModificationEventArgs e)
		{
			MemberModified(this, e);
		}
		
		internal void EmitParameterModified(IParameterModificationEventArgs e)
		{
			ParameterModified(this, e);
		}
/*
		private void AfterEdit (object sender, TreeListViewLabelEditEventArgs e)
		{
			IProjectContent pc = ProjectService.CurrentProject.CreateProjectContent();
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
			
		}
		*/
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
			model.Nodes.Clear();
			
			currClass = classType;
			if (classType != null)
				classTypeGroupCreators[classType.ClassType](classType);

			membersList.EndUpdate();
		}
		
		private Node AddGroup<MT>(string title, ICollection<MT> members) where MT : IMember
		{
			if (members == null) return null;
			GroupNode group = new GroupNode(typeof(MT), title);
			//TreeListViewItem group = new TreeListViewItem(title);
//			group.ForeColor = Color.Gray;
//			group.Font = new Font(group.Font, FontStyle.Bold);
//			group.Items.Sortable = false;
//			group.Tag = typeof(MT);
			
			IAmbience ambience = GetAmbience();
			
			if (members.Count != 0)
			{
				foreach (IMember member in members)
				{
					MemberNode memberItem = CreateMemberItem(member, ambience);
					group.Nodes.Add(memberItem);
					
					IMethod methodMember = memberItem.Member as IMethod;
					if (methodMember != null)
						FillParams (memberItem, methodMember);
				}
			}
			
			AddAddItem(typeof(MT), group);
			
			model.Nodes.Add(group);
			return group;
		}
		
		private MemberNode CreateMemberItem(IMember member, IAmbience ambience)
		{
			MemberNode memberItem = new MemberNode(member, ambience, this);
			return memberItem;
		}
		
		private void AddAddItem(Type memberType, GroupNode group)
		{
			string str = "";
			
			if (memberType == typeof(IMethod)) str = "<Add Method>";
			else if (memberType == typeof(IEvent)) str = "<Add Event>";
			else if (memberType == typeof(IProperty)) str = "<Add Property>";
			else if (memberType == typeof(IField)) str = "<Add Field>";
			
			AddMemberNode addNewMember = new AddMemberNode(str); // TODO set color
			//addNewMember.ForeColor = SystemColors.GrayText;
			group.Nodes.Add(addNewMember);
			addMemberItems[memberType] = addNewMember;
		}
		
		private void ConvertAddItemToMemberItem(Node addItem, IMember member, IAmbience ambience)
		{
			//addItem.
			//	addItem.ForeColor = SystemColors.ControlText;
			//	addItem.SubItems.Add(ambience.Convert(member.ReturnType));
			//	addItem.SubItems.Add(member.Modifiers.ToString());
			//	addItem.SubItems.Add(GetSummary(member));
		}
		
		private static string GetSummary (IDecoration decoration)
		{
			if (decoration == null) return String.Empty;
			return GetSummary(decoration.Documentation, @"/docroot/summary");
		}

		private static string GetParameterSummary (IDecoration decoration, string parameterName)
		{
			if (decoration == null) return String.Empty;
			return GetSummary(decoration.Documentation, @"/docroot/param[@name='"+parameterName+"']");
		}
		
		private static string GetSummary (string documentation, string xpath)
		{
			StringReader strReader = new StringReader("<docroot>" + documentation + "</docroot>");
			XmlDocument doc = new XmlDocument();
			doc.Load(strReader);
			XPathNavigator nav = doc.CreateNavigator();
			XPathNodeIterator ni = nav.Select(xpath);
			if (ni.MoveNext())
				return ni.Current.InnerXml;
			else
				return String.Empty;
		}
		
		private void FillParams(MemberNode item, IMethod method)
		{
			Image comma = (Image)resources.GetObject("ClassEditor.Comma");
			Image openBrace = (Image)resources.GetObject("ClassEditor.OpenBrace");
			Image closeBrace = (Image)resources.GetObject("ClassEditor.CloseBrace");
			Image emptyBraces = (Image)resources.GetObject("ClassEditor.EmptyBraces");
			Image currentImage = openBrace;
			foreach (IParameter param in method.Parameters)
			{
				ParameterNode parameter = new ParameterNode(method, param, GetAmbience(), currentImage, this);
				currentImage = comma;
				item.Nodes.Add(parameter);
			}
			if (currentImage == openBrace)
				currentImage = emptyBraces;
			else
				currentImage = closeBrace;
			
			AddParameterNode addParam = new AddParameterNode(currentImage);
			item.Nodes.Add(addParam);
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
		
		void _modifiersEditorShowing(object sender, CancelEventArgs e)
		{
			MemberNode memberItem = membersList.SelectedNode.Tag as MemberNode;
			ParameterNode parameterItem = membersList.SelectedNode.Tag as ParameterNode;
			
			if (memberItem != null)
			{
				_modifiers.DropDownItems.Clear();
				
				_modifiers.DropDownItems.Add(ICSharpCode.NRefactory.Ast.Modifiers.Public);
				_modifiers.DropDownItems.Add(ICSharpCode.NRefactory.Ast.Modifiers.Private);
				_modifiers.DropDownItems.Add(ICSharpCode.NRefactory.Ast.Modifiers.Protected);
				_modifiers.DropDownItems.Add(ICSharpCode.NRefactory.Ast.Modifiers.Internal);
			}
			else if (parameterItem != null)
			{
				_modifiers.DropDownItems.Clear();

				_modifiers.DropDownItems.Add(ParameterModifiers.In);
				_modifiers.DropDownItems.Add(ParameterModifiers.Out);
				_modifiers.DropDownItems.Add(ParameterModifiers.Ref);
				_modifiers.DropDownItems.Add(ParameterModifiers.Params);
				_modifiers.DropDownItems.Add(ParameterModifiers.Optional);
			}
			else
				e.Cancel = true;
		}
		
		void MembersListNodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
		{
			Node item = e.Node.Tag as Node;
			
			if (item == null) return;
			
			MemberNode memberItem = item as MemberNode;
			ParameterNode paramItem = item as ParameterNode;
			AddMemberNode addMemberItem = item as AddMemberNode;
			AddParameterNode addParamItem = item as AddParameterNode;
			
			if (addMemberItem != null)// && addMemberItems.ContainsValue(memberItem))
			{
				/*				IAmbience ambience = GetAmbience();
				item.SubItems.Add(ambience.Convert(VoidReturnType.Instance));
				item.SubItems.Add(ModifierEnum.Public.ToString());
				item.SubItems.Add("");
				item.Text = "[method name]";
				item.BeginEdit(0);*/
			}
			else if (addParamItem != null) //addParameterItems.ContainsValue(item))
			{
				
			}
			else if (memberItem != null)
			{
					MemberActivated(this, new IMemberEventArgs(memberItem.Member));
			}
			else if (paramItem != null)
			{
				if (paramItem.Parameter != null && paramItem.Method != null)
					ParameterActivated(this, new IParameterEventArgs(paramItem.Method, paramItem.Parameter));
			}
		}
		
		void _nameDrawText(object sender, DrawEventArgs e)
		{
			if ((e.Node.Tag is GroupNode || e.Node.Tag is AddParameterNode || e.Node.Tag is AddMemberNode) && e.Node.IsSelected == false)
				e.TextColor = Color.Gray;
		}
	}

	public enum Modification { None, Name, Type, Modifier, Summary }
}
