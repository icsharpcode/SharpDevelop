// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// class browser.
	/// </summary>
	public class ClassNode : ExtTreeNode
	{
		IClass   c;
		IProject project;
		public IClass Class {
			get {
				return c;
			}
			set {
				c = value;
				Initialize();
			}
		}
		
		public ClassNode(IProject project, IClass c)
		{
			sortOrder = 3;
			
			this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ClassBrowser/ClassContextMenu";
			this.project = project;
			this.c       = c;
			Text = c.Name;
			SelectedImageIndex = ImageIndex = ClassBrowserIconService.GetIcon(c);
			
			if (c.ClassType != ClassType.Delegate) {
				Nodes.Add(new TreeNode());
			}
		}
		
		public override void ActivateItem()
		{
			if (c.CompilationUnit != null) {
				FileService.JumpToFilePosition(c.CompilationUnit.FileName, c.Region.BeginLine - 1, c.Region.BeginColumn - 1);
			}
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Nodes.Clear();
			
			// don't insert delegate 'members'
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			
			if (c.BaseTypes.Count > 0) {
				new BaseTypesNode(project, c).AddTo(this);
			}
			if ((c.Modifiers & ModifierEnum.Sealed) != ModifierEnum.Sealed) {
				new DerivedTypesNode(project, c).AddTo(this);
			}
			
			foreach (IClass innerClass in c.InnerClasses) {
				new ClassNode(project, innerClass).AddTo(this);
			}
			
			foreach (IMethod method in c.Methods) {
				new MemberNode(method).AddTo(this);
			}
			
			foreach (IProperty property in c.Properties) {
				new MemberNode(property).AddTo(this);
			}
			
			foreach (IField field in c.Fields) {
				new MemberNode(field).AddTo(this);
			}
			
			foreach (IEvent e in c.Events) {
				new MemberNode(e).AddTo(this);
			}
			UpdateVisibility();
		}
	}
}
