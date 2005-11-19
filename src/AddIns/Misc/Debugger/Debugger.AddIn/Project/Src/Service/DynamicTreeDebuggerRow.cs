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
			
			this[1].Paint += OnIconPaint;
			this[2].Text = variable.Name;
			this[3].Text = variable.Value.AsString;
			this[3].FinishLabelEdit += OnLabelEdited;
			if (variable.Value is PrimitiveValue && variable.Value.ManagedType != typeof(string)) {
				this[3].AllowLabelEdit = true;
			}
			
			if (!variable.Value.MayHaveSubVariables)
				this.ShowPlus = false;
			this.ShowMinusWhileExpanded = true;
		}
		
		void OnIconPaint(object sender, ItemPaintEventArgs e)
		{
			e.Graphics.DrawImageUnscaled(DebuggerIcons.GetImage(variable), e.ClipRectangle);
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
	}
}
