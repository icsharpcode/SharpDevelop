// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui {
	
	public class EditHighlightingColorDialog : BaseSharpDevelopForm
	{
		private System.Windows.Forms.RadioButton foreNo;
		private System.Windows.Forms.RadioButton foreUser;
		private ICSharpCode.SharpDevelop.Gui.ColorButton backBtn;
		private System.Windows.Forms.RadioButton backSys;
		private System.Windows.Forms.Button acceptBtn;
		private ICSharpCode.SharpDevelop.Gui.ColorButton foreBtn;
		private System.Windows.Forms.RadioButton backNo;
		private System.Windows.Forms.ComboBox foreList;
		private System.Windows.Forms.CheckBox italicBox;
		private System.Windows.Forms.RadioButton foreSys;
		private System.Windows.Forms.RadioButton backUser;
		private System.Windows.Forms.CheckBox boldBox;
		private System.Windows.Forms.ComboBox backList;

		public EditorHighlightColor Color;
		
		public EditHighlightingColorDialog(EditorHighlightColor color)
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ColorDialog.xfrm"));
			
			if (color == null) {
				color = new EditorHighlightColor(true);
			}
			Color = color;
			
			boldBox   = (CheckBox)ControlDictionary["boldBox"];
			italicBox = (CheckBox)ControlDictionary["italicBox"];
			
			foreNo   = (RadioButton)ControlDictionary["foreNo"];
			foreUser = (RadioButton)ControlDictionary["foreUser"];
			foreSys  = (RadioButton)ControlDictionary["foreSys"];
			foreList = (ComboBox)ControlDictionary["foreList"];
			
			backNo   = (RadioButton)ControlDictionary["backNo"];
			backUser = (RadioButton)ControlDictionary["backUser"];
			backSys  = (RadioButton)ControlDictionary["backSys"];
			backList = (ComboBox)ControlDictionary["backList"];
			
			acceptBtn = (Button)ControlDictionary["acceptBtn"];
			
			this.foreBtn = new ColorButton();
			this.foreBtn.CenterColor = System.Drawing.Color.Empty;
			this.foreBtn.Enabled = false;
			this.foreBtn.Location = new System.Drawing.Point(30, 78);
			this.foreBtn.Name = "foreBtn";
			this.foreBtn.Size = new System.Drawing.Size(98, 24);
			
			this.ControlDictionary["foreBox"].Controls.Add(foreBtn);
			
			this.backBtn = new ColorButton();
			this.backBtn.CenterColor = System.Drawing.Color.Empty;
			this.backBtn.Enabled = false;
			this.backBtn.Location = new System.Drawing.Point(30, 78);
			this.backBtn.Name = "backBtn";
			this.backBtn.Size = new System.Drawing.Size(98, 24);
			
			this.ControlDictionary["backBox"].Controls.Add(backBtn);

			this.acceptBtn.Click += new EventHandler(AcceptClick);
			this.foreNo.CheckedChanged   += new EventHandler(foreCheck);
			this.foreSys.CheckedChanged  += new EventHandler(foreCheck);
			this.foreUser.CheckedChanged += new EventHandler(foreCheck);
			this.backNo.CheckedChanged   += new EventHandler(backCheck);
			this.backSys.CheckedChanged  += new EventHandler(backCheck);
			this.backUser.CheckedChanged += new EventHandler(backCheck);
			
			PropertyInfo[] names = typeof(System.Drawing.SystemColors).GetProperties(BindingFlags.Static | BindingFlags.Public);
			
			foreach(PropertyInfo info in names) {
				foreList.Items.Add(info.Name);
				backList.Items.Add(info.Name);
			}
			foreList.SelectedIndex = backList.SelectedIndex = 0;
			
			if (color.SysForeColor) {
				foreSys.Checked = true;
				for (int i = 0; i < foreList.Items.Count; ++i) {
					if ((string)foreList.Items[i] == color.SysForeColorName) foreList.SelectedIndex = i;
				}
			} else if (color.HasForeColor) {
				foreUser.Checked = true;
				foreBtn.CenterColor = color.ForeColor;
			} else {
				foreNo.Checked = true;
			}
			
			if (color.SysBackColor) {
				backSys.Checked = true;
				for (int i = 0; i < backList.Items.Count; ++i) {
					if ((string)backList.Items[i] == color.SysForeColorName) backList.SelectedIndex = i;
				}
			} else if (color.HasBackColor) {
				backUser.Checked = true;
				backBtn.CenterColor = color.BackColor;
			} else {
				backNo.Checked = true;
			}
			
			boldBox.Checked = color.Bold;
			italicBox.Checked = color.Italic;
		}
		
		void foreCheck(object sender, EventArgs e)
		{
			if (foreNo.Checked) {
				foreBtn.Enabled = false;
				foreList.Enabled = false;
			} else if (foreUser.Checked) {
				foreBtn.Enabled = true;
				foreList.Enabled = false;
			} else if (foreSys.Checked) {
				foreBtn.Enabled = false;
				foreList.Enabled = true;
			}
		}
		
		void backCheck(object sender, EventArgs e)
		{
			if (backNo.Checked) {
				backBtn.Enabled = false;
				backList.Enabled = false;
			} else if (backUser.Checked) {
				backBtn.Enabled = true;
				backList.Enabled = false;
			} else if (backSys.Checked) {
				backBtn.Enabled = false;
				backList.Enabled = true;
			}
		}
		
		void AcceptClick(object sender, EventArgs e)
		{
			object foreColor = null;
			object backColor = null;
			
			if (foreUser.Checked) {
				foreColor = (System.Drawing.Color)foreBtn.CenterColor;
			} else if (foreSys.Checked) {
				foreColor = (string)foreList.SelectedItem;
			}
			
			if (backUser.Checked) {
				backColor = (System.Drawing.Color)backBtn.CenterColor;
			} else if (backSys.Checked) {
				backColor = (string)backList.SelectedItem;
			}
			
			Color = new EditorHighlightColor(foreColor, backColor, boldBox.Checked, italicBox.Checked);
			
			DialogResult = DialogResult.OK;
		}
		
	}
}
