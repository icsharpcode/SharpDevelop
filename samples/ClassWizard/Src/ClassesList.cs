/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 12/23/2006
 * Time: 8:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Dom;

namespace ClassWizard
{
	public class ClassesListDialog
	{
		private class ClassesList : BaseSharpDevelopForm
		{
			IClass baseClass;
			public ClassesList() : this (null) {}
			
			public ClassesList(IClass baseClass)
			{
				this.baseClass = baseClass;
				SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ClassWizard.Resources.ClassesList.xfrm"));
				treeView = Get<TreeView>("classes");
				treeView.BeforeCheck += ClassesTreeViewBeforeCheck; 
				treeView.AfterCheck += ClassesTreeViewAfterCheck;
				treeView.AfterSelect += ClassesTreeViewAfterCheck;
			}
			
			private TreeView treeView;
			
			public void InitializeClasses(IProject project, ClassType ct)
			{
				IProjectContent pc = ParserService.GetProjectContent(project);
				
				treeView.Nodes.Clear();
				
				AddClasses (pc, ct, false);
				
				foreach (IProjectContent rpc in pc.ReferencedContents)
					AddClasses (rpc, ct, false);
			}
			
			void AddClasses (IProjectContent pc, ClassType ct, bool allowSealed)
			{
				if (pc.Classes == null) return;
				foreach (IClass c in pc.Classes)
				{
					if (c.ClassType == ct && (!c.IsSealed || (c.IsSealed && allowSealed)))
					{
						if (baseClass == null || (baseClass != null && c.IsTypeInInheritanceTree(baseClass)))
						{
							TreeNode node = AddItemToTreeView(c.FullyQualifiedName);
							if (node != null)
								node.Tag = c;
						}
					}
				}
			}
			
			TreeNode AddItemToTreeView (string item)
			{
				string[] path = item.Split(treeView.PathSeparator[0]);
				TreeNodeCollection tnc = treeView.Nodes;
				TreeNode ret = null;
				foreach (string pathitem in path)
				{
					if (tnc != null)
					{
						TreeNode tn;
						if (!tnc.ContainsKey(pathitem))
						{
							tn = tnc.Add(pathitem, pathitem);
							ret = tn;
						}
						else
							tn = tnc[pathitem];
						if (tn == null)
						{
							MessageBox.Show("Treenode is null: " + pathitem + "\n" + item);
						}
						tnc = tn.Nodes;
					}
				}
				return ret;
			}
			
			void ClassesTreeViewBeforeCheck (object sender, TreeViewCancelEventArgs e)
			{
				e.Cancel = (e.Node.Nodes != null && e.Node.Nodes.Count > 0);
			}

			void ClassesTreeViewAfterCheck (object sender, TreeViewEventArgs e)
			{
				IClass c = (IClass) e.Node.Tag;
				if (SelectMultiple)
				{
					if (e.Node.Checked && !selectedItems.Contains(c))
						selectedItems.Add(c);
					else if (!e.Node.Checked && selectedItems.Contains(c))
						selectedItems.Remove(c);
				}
				else
				{
					selectedItems.Clear();
					if (treeView.SelectedNode.Nodes == null || treeView.SelectedNode.Nodes.Count == 0)
						selectedItems.Add((IClass)treeView.SelectedNode.Tag);
				}
				Get<Button>("ok").Enabled = (selectedItems.Count > 0);
			}
			
			public bool SelectMultiple
			{
				get { return treeView.CheckBoxes; }
				set { treeView.CheckBoxes = value; }
			}
			
			List<IClass> selectedItems = new List<IClass>();
			
			public IList<IClass> SelectedClasses
			{
				get { return selectedItems; }
			}
		}
		
		ClassesList classesList = new ClassesList();
		ClassType classType;
		IProject project;
		
		bool initialized = false;
		
		public ClassesListDialog() {}
		
		public bool ShowDialog()
		{
			if (!initialized)
			{
				classesList.InitializeClasses(project, classType);
				initialized = true;
			}
			
			classesList.ShowDialog();
			return classesList.DialogResult == DialogResult.OK;
		}

		public ClassType ClassType
		{
			get { return classType; }
			set
			{
				initialized &= classType == value;
				classType = value;
			}
		}
		
		public IProject Project
		{
			get { return project; }
			set
			{
				initialized &= project == value;
				project = value;
			}
		}
		
		public bool SelectMultiple
		{
			get { return classesList.SelectMultiple; }
			set { classesList.SelectMultiple = value; }
		}
		
		public IList<IClass> SelectedClasses
		{
			get { return classesList.SelectedClasses; }
		}
	}
}
