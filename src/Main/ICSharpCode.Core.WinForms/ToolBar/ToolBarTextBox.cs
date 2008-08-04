// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="John Simons" email="johnsimons007@yahoo.com.au"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarTextBox : ToolStripTextBox, IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ITextBoxCommand menuCommand = null;
		
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
		
		public ITextBoxCommand MenuCommand {
			get {
				return menuCommand;
			}
		}

		public ToolBarTextBox(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;			
			this.caller  = caller;
			this.codon   = codon;

			TextBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);

			menuCommand = (ITextBoxCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.Owner = this;
			if (menuCommand == null) {
				throw new NullReferenceException("Can't create textbox toolbox command");
			}
			UpdateText();
			UpdateStatus();
		}

		void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				MenuCommand.Run();
			}
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
