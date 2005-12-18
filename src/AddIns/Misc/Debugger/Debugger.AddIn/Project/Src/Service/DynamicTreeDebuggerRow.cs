// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
		bool populated = false;
		
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
			this.variable.ValueChanged += Update;
			this.Hidden += delegate { this.variable.ValueChanged -= Update; };
			
			DebuggerGridControl.AddColumns(this.ChildColumns);
			
			this[1].Paint += OnIconPaint;
			this[3].FinishLabelEdit += OnLabelEdited;
			
			Update();
		}
		
		void Update(object sender, DebuggerEventArgs e)
		{
			Update();
		}
		
		void Update()
		{
			image = DebuggerIcons.GetImage(variable);
			this[1].Text = ""; // Icon
			this[2].Text = variable.Name;
			this[3].Text = variable.Value.AsString;
			if (variable.Value is PrimitiveValue && variable.Value.ManagedType != typeof(string)) {
				this[3].AllowLabelEdit = true;
			}
			
			if (!variable.Value.MayHaveSubVariables) {
				this.ShowPlus = false;
			}
			this.ShowMinusWhileExpanded = true;
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
			if (!populated) {
				Populate();
			}
		}
		
		void Populate()
		{
			List<Variable> publicStatic = new List<Variable>();
			List<Variable> publicInstance = new List<Variable>();
			List<Variable> privateStatic = new List<Variable>();
			List<Variable> privateInstance = new List<Variable>();
			
			foreach(Variable variable in this.Variable.SubVariables) {
				ClassVariable classVariable = variable as ClassVariable;
				if (classVariable == null) {
					publicInstance.Add(variable);
				} else {
					if (classVariable.IsPublic) {
						if (classVariable.IsStatic) {
							publicStatic.Add(variable);
						} else {
							publicInstance.Add(variable);
						}
					} else {
						if (classVariable.IsStatic) {
							privateStatic.Add(variable);
						} else {
							privateInstance.Add(variable);
						}
					}
				}
			}
			
			this.ChildRows.Clear();
			// Private Members
			if (privateInstance.Count > 0) {
				this.ChildRows.Add(MakeSubMenu("Private Members",
				                               RowsFromVariables(privateInstance)));
			}
			// Static Members
			if (publicStatic.Count > 0 || privateStatic.Count > 0) {
				DynamicTreeRow privateStaticSubMenu = MakeSubMenu("Private Static Members",
				                                                  RowsFromVariables(privateStatic));
				this.ChildRows.Add(MakeSubMenu("Static Members",
				                               privateStatic.Count > 0 ? new DynamicListRow[]{privateStaticSubMenu} : new DynamicListRow[]{},
				                               RowsFromVariables(publicStatic)));
			}
			// Public Members
			this.ChildRows.AddRange(RowsFromVariables(publicInstance));
			
			populated = true;
		}
		
		IEnumerable<DynamicListRow> RowsFromVariables(IEnumerable<Variable> variables)
		{
			foreach(Variable variable in variables) {
				yield return new DynamicTreeDebuggerRow(variable);
			}
		}
		
		DynamicTreeRow MakeSubMenu(string name, params IEnumerable<DynamicListRow>[] elements)
		{
			DynamicTreeRow rootRow = new DynamicTreeRow();
			DebuggerGridControl.AddColumns(rootRow.ChildColumns);
			rootRow[2].Text = name;
			foreach(IEnumerable<DynamicListRow> rows in elements) {
				rootRow.ChildRows.AddRange(rows);
			}
			return rootRow;
		}
	}
}
