// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//	   <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
//using ICSharpCode.Core.Services;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

//using ICSharpCode.Core.Properties;

using DebuggerLibrary;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class LocalVarPad : AbstractPadContent
	{
		TreeListView localVarList;

		//ClassBrowserIconsService iconsService;
		
		ColumnHeader name = new ColumnHeader();
		ColumnHeader val  = new ColumnHeader();
		ColumnHeader type = new ColumnHeader();
		
		public override Control Control {
			get {
				return localVarList;
			}
		}
		
		public LocalVarPad() //: base("${res:MainWindow.Windows.Debug.Local}", null)
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			ImageList imageList = new ImageList();
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Class"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Field"));
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.Property"));

			//iconsService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			localVarList = new TreeListView();
			localVarList.SmallImageList = imageList;
			localVarList.ShowPlusMinus = true;
			localVarList.FullRowSelect = true;
			localVarList.Dock = DockStyle.Fill;
			//localVarList.GridLines  = false;
			//localVarList.Activation = ItemActivation.OneClick;
			localVarList.Columns.AddRange(new ColumnHeader[] {name, val, type} );
			name.Width = 250;
			val.Width = 300;
			type.Width = 250;

			localVarList.BeforeExpand += new TreeListViewCancelEventHandler(localVarList_BeforeExpand);
			
			NDebugger.DebuggingPaused += new DebuggingPausedEventHandler(debuggerService_OnDebuggingPaused);

			RedrawContent();
		}
		
		public override void RedrawContent()
		{
			name.Text = "Name";
			val.Text  = "Value";
			type.Text = "Type";

            if (NDebugger.IsDebugging && NDebugger.IsProcessRunning == false) {
                debuggerService_OnDebuggingPaused(this, new DebuggingPausedEventArgs(PausedReason.StepComplete));
            }
		}

		private void debuggerService_OnDebuggingPaused(object sender, DebuggingPausedEventArgs e)
		{
			localVarList.BeginUpdate();
			localVarList.Items.Clear();

			AddVariables(localVarList.Items, NDebugger.LocalVariables);

			localVarList.EndUpdate();
		}

		private void localVarList_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
		{
			localVarList.BeginUpdate();
			e.Item.Items.Clear();

			ObjectVariable var = e.Item.Tag as ObjectVariable;
			if (var != null && var.HasBaseClass && var.BaseClass.Type != "System.Object")
			{
				TreeListViewItem newItem = new TreeListViewItem();
				newItem.Text = "<Base class>";
				newItem.SubItems.Add(var.BaseClass.Value.ToString());
				newItem.SubItems.Add(var.BaseClass.Type);
				newItem.Tag = var.BaseClass;
				newItem.ImageIndex = 0; // Class
				newItem.Items.Add(""); // Show plus icon
				e.Item.Items.Add(newItem);
			}
			AddVariables(e.Item.Items, ((Variable)e.Item.Tag).SubVariables);

			localVarList.EndUpdate();
		}

		void AddVariables (TreeListViewItemCollection items, VariableCollection vars)
		{
 			foreach (Variable var in vars) {
				TreeListViewItem newItem = new TreeListViewItem();
				newItem.Tag = var;
 				newItem.Text = var.Name;
 				newItem.SubItems.Add(var.Value.ToString());
				newItem.SubItems.Add(var.Type);
				items.Add(newItem);
 				RefreshVariable(var);
 				
 				if (var is PropertyVariable) {
 					((PropertyVariable)var).ValueEvaluated += new EventHandler(PropertyEvaluated);
 				}
			}           
		}
		
		void PropertyEvaluated (object sender, EventArgs args)
		{
			RefreshVariable((Variable)sender);
		}		

		void RefreshVariable (Variable var)
		{
			RefreshVariableInItemConnection(var, localVarList.Items);
		}
		
		void RefreshVariableInItemConnection (Variable var, TreeListViewItemCollection items)
		{
			foreach (TreeListViewItem item in items) {
				// Refresh in sub trees
				RefreshVariableInItemConnection(var, item.Items);

				if (item.Tag == var) {
					if (item.SubItems[1].Text == null) {
						item.SubItems[1].Text = var.Value.ToString();
					}
					item.SubItems[2].Text = var.Type;
					item.Items.Clear();
					if (var is ObjectVariable && ((ObjectVariable)var).HasBaseClass) {
						// It is a class
						item.ImageIndex = 0; // Class
						item.Items.Add(""); // Show plus icon

						//object devNull = (var as ObjectVariable).SubVariables; // Cache variables TODO: LAME

					} else if (var is PropertyVariable){
						// It is a property
						item.ImageIndex = 2; // Property
						if ((var as PropertyVariable).IsEvaluated && (var as PropertyVariable).Value is ObjectVariable) {
							item.Items.Add(""); // Show plus icon
						}
					} else {
						item.ImageIndex = 1; // Field
					}
				}
			}
		}
	}
}
