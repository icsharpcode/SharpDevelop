// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
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
				FileService.JumpToFilePosition(c.CompilationUnit.FileName,
				                               c.Region != null ? c.Region.BeginLine - 1 : 0,
				                               c.Region != null ? c.Region.BeginColumn - 1 : 0);
			}
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Nodes.Clear();
			
			IAmbience languageConversion= AmbienceService.CurrentAmbience;
			languageConversion.ConversionFlags = ConversionFlags.None;
			
			// don't insert delegate 'members'
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			
			new BaseTypesNode(project, c).AddTo(this);
			new DerivedTypesNode(project, c).AddTo(this);
			
			foreach (IClass innerClass in c.InnerClasses) {
				new ClassNode(project, innerClass).AddTo(this);
			}
			
			foreach (IMethod method in c.Methods) {
				new MemberNode(c, method).AddTo(this);
			}
			
			foreach (IProperty property in c.Properties) {
				new MemberNode(c, property).AddTo(this);
			}
			
			foreach (IIndexer indexer in c.Indexer) {
				new MemberNode(c, indexer).AddTo(this);
			}
			
			foreach (IField field in c.Fields) {
				new MemberNode(c, field).AddTo(this);
			}
			
			foreach (IEvent e in c.Events) {
				new MemberNode(c, e).AddTo(this);
			}
			UpdateVisibility();
//	TODO: TreeNode comparer
//			SortUtility.QuickSort(classNode.Nodes, TreeNodeComparer.Default);		}
		}
	}
}
