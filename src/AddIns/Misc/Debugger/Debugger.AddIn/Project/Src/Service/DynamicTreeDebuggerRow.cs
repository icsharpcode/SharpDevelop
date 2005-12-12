// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.TreeGrid;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DynamicTreeDebuggerRow:DynamicTreeRow
	{
		// Columns:
		// 0 = plus sign
		// 1 = icon
		// 2 = text
		// 3 = value
		
		Variable variable;
		Image image;
		
		public Variable Variable {
			get {
				return variable;
			}
			set {
				variable = value;
			}
		}
		
		public DynamicTreeDebuggerRow()
		{
		}
		
		public DynamicTreeDebuggerRow(Variable variable)
		{
			if (variable == null) throw new ArgumentNullException("variable");
			
			this.variable = variable;
			this.variable.ValueChanged += delegate { Update(); };
			
			this[1].Paint += OnIconPaint;
			this[3].FinishLabelEdit += OnLabelEdited;
			if (variable.Value is PrimitiveValue && variable.Value.ManagedType != typeof(string)) {
				this[3].AllowLabelEdit = true;
			}
			
			if (!variable.Value.MayHaveSubVariables) {
				this.ShowPlus = false;
			}
			this.ShowMinusWhileExpanded = true;
			
			Update();
		}
		
		void Update()
		{
			image = DebuggerIcons.GetImage(variable);
			this[1].Text = ""; // Icon
			this[2].Text = variable.Name;
			this[3].Text = variable.Value.AsString;
		}
		
		void OnIconPaint(object sender, ItemPaintEventArgs e)
		{
			if (image != null) {
				e.Graphics.DrawImageUnscaled(image, e.ClipRectangle);
			}
		}
		
		void OnLabelEdited(object sender, DynamicListEventArgs e)
		{
			PrimitiveValue val = (PrimitiveValue)variable.Value;
			string newValue = ((DynamicListItem)sender).Text;
			try {
				val.Primitive = newValue;
			} catch (NotSupportedException) {
				MessageBox.Show(WorkbenchSingleton.MainForm, "Can not covert " + newValue + " to " + val.ManagedType.ToString(), "Can not set value");
			}
		}
		
		/// <summary>
		/// Called when plus is pressed in debugger tooltip.
		/// Sets the data to be show in the next level.
		/// </summary>
		protected override void OnExpanding(DynamicListEventArgs e)
		{
			this.ChildRows.Clear();
			foreach(Variable variable in this.Variable.SubVariables) {
				DynamicTreeDebuggerRow newRow = new DynamicTreeDebuggerRow(variable);
				DebuggerGridControl.AddColumns(newRow.ChildColumns);
				this.ChildRows.Add(newRow);
			}
		}
	}
}
