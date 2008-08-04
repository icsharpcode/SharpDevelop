// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarComboBox : ToolStripComboBox, IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		IComboBoxCommand menuCommand = null;
		
		public object Caller {
			get {
				return caller;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public IComboBoxCommand MenuCommand {
			get {
				return menuCommand;
			}
		}
		
		public ToolBarComboBox(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBox.SelectionChangeCommitted += new EventHandler(selectionChanged);
			ComboBox.KeyDown += new KeyEventHandler(ComboBoxKeyDown);
			
			this.caller  = caller;
			this.codon   = codon;
			
			menuCommand = (IComboBoxCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.Owner = this;
			if (menuCommand == null) {
				throw new NullReferenceException("Can't create combobox menu command");
			}
			UpdateText();
			UpdateStatus();
		}
		
		void ComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Control) {
				MenuCommand.Run();
			}
		}
		
		void selectionChanged(object sender, EventArgs e)
		{
			MenuCommand.Run();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null) {
					isEnabled &= menuCommand.IsEnabled;
				}
				
				return isEnabled;
			}
		}
		
		public virtual void UpdateStatus()
		{
			bool isVisible = base.Visible;
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				isVisible &= failedAction != ConditionFailedAction.Exclude;
			}
			if (base.Visible != isVisible) {
				Visible = isVisible;
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon.Properties.Contains("label")) {
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (codon.Properties.Contains("tooltip")) {
				ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
	}
}
