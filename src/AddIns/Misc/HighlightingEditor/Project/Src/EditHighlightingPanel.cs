// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;

using ICSharpCode.Core;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	class EditHighlightingPanel : AbstractOptionPanel {
		Label   builtinLabel;
		ListBox builtinList;
		Button  copyButton;
		Label   userLabel;
		ListBox userList;
		Button  deleteButton;
		Button  modifyButton;
				
		ResourceSyntaxModeProvider modeProvider;
		
		public override bool StorePanelContents()
		{
			ICSharpCode.TextEditor.Document.HighlightingManager.Manager.ReloadSyntaxModes();
			return true;
		}
		
		public EditHighlightingPanel() {
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.OptionPanel.xfrm"));
			InitializeComponent();
		}
		
		public override void LoadPanelContents()
		{
			FillLists();
		}

		void InitializeComponent() {
			builtinLabel = (Label)ControlDictionary["builtinLabel"];
			builtinList  = (ListBox)ControlDictionary["builtinList"];
			builtinList.SelectedIndexChanged += new EventHandler(BuiltinListSelectedIndexChanged);
			
			copyButton   = (Button)ControlDictionary["copyButton"];
			copyButton.Click     += new EventHandler(CopyButtonClick);
			
			userLabel    = (Label)ControlDictionary["userLabel"];
			userList     = (ListBox)ControlDictionary["userList"];
			
			deleteButton = (Button)ControlDictionary["deleteButton"];
			deleteButton.Click   += new EventHandler(DeleteButtonClick);
			
			modifyButton = (Button)ControlDictionary["modifyButton"];
			modifyButton.Click   += new EventHandler(ModifyButtonClick);
			
		}
		
		void BuiltinListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (builtinList.SelectedIndex == -1) {
				copyButton.Enabled = false;
				return;
			}
			HighlightItem item = (HighlightItem)builtinList.SelectedItem;
			string filename = item.FileName;
			if (filename == null) {
				filename = item.SyntaxMode.FileName;
			}
			
			string newname  = Path.GetFileName(filename);
			foreach (HighlightItem item2 in userList.Items) {
				if (Path.GetFileName(item2.FileName) == newname) {
					copyButton.Enabled = false;
					return;
				}
			}
			copyButton.Enabled = true;
		}
		
		protected override void OnResize(System.EventArgs ev)
		{
			int halfWidth  = (Width - 24) / 2;
			
			builtinLabel.Width = userLabel.Width = halfWidth;
			builtinList.Width  = userList.Width  = halfWidth;
			
			userLabel.Left     = userList.Left = halfWidth + 16;
			deleteButton.Left  = userList.Left;
			modifyButton.Left  = deleteButton.Left + 90;
			
			base.OnResize(ev);
		}
		
		void FillLists()
		{
			builtinList.Items.Clear();
			userList.Items.Clear();
			
			string uPath = Path.Combine(PropertyService.ConfigDirectory, "modes");
			
			List<string> uCol;
			
			if (Directory.Exists(uPath)) {
				uCol = FileUtility.SearchDirectory(uPath, "*.xshd", true);
			} else {
				Directory.CreateDirectory(uPath);
				uCol = new List<string>();
			}
			
			foreach(string str in uCol) {
				SchemeNode node = LoadFile(new XmlTextReader(str), true);
				if (node == null) continue;
				userList.Items.Add(new HighlightItem(null, str, node));
			}
			
			modeProvider = new ResourceSyntaxModeProvider();
			
			foreach(SyntaxMode mode in modeProvider.SyntaxModes){
				SchemeNode node = LoadFile(modeProvider.GetSyntaxModeFile(mode), false);
				if (node == null) continue;
				builtinList.Items.Add(new HighlightItem(mode, null, node));
			}
			
			if (builtinList.Items.Count > 0)
				builtinList.SelectedIndex = 0;
			
			if (userList.Items.Count > 0)
				userList.SelectedIndex = 0;
		}
			
		void CopyButtonClick(object sender, EventArgs ev)
		{
			if (builtinList.SelectedIndex == -1) return;
			
			try {
				HighlightItem item = (HighlightItem)builtinList.SelectedItem;
				
				string filename = item.FileName;
				if (filename == null) filename = item.SyntaxMode.FileName;
				
				string newname  = Path.GetFileName(filename);
				newname = Path.Combine(PropertyService.ConfigDirectory, "modes" + 
							Path.DirectorySeparatorChar + newname);
				
				using (StreamWriter fs = File.CreateText(newname)) {
					fs.Write(item.Node.Content);
				}
				
				SchemeNode newNode = LoadFile(new XmlTextReader(newname), true);
				if (newNode == null) throw new Exception();
				
				userList.Items.Add(new HighlightItem(null, newname, newNode));
			} catch (Exception e) {
				MessageService.ShowError(e, "${res:Dialog.Options.TextEditorOptions.EditHighlighting.CopyError}");
			}
			BuiltinListSelectedIndexChanged(this, EventArgs.Empty);
		}
		
		void ModifyButtonClick(object sender, EventArgs ev)
		{
			if (userList.SelectedIndex == -1) {
				return;
			}
			
			HighlightItem item = (HighlightItem)userList.SelectedItem;
			
			using (EditHighlightingDialog dlg = new EditHighlightingDialog(item.Node)) {
				DialogResult res = dlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				
				if (res == DialogResult.OK) {
					using (StreamWriter sw = new StreamWriter(item.FileName, false)) {
						sw.WriteLine(item.Node.ToXml().Replace("\n", "\r\n"));
					}
					// refresh item text
					userList.Items.RemoveAt(userList.SelectedIndex);
					userList.Items.Add(item);
				}
				
				try {
					item.Node.Remove();
				} catch {}
				
			}
		}

		void DeleteButtonClick(object sender, EventArgs ev)
		{
			if (userList.SelectedIndex == -1) return;
			HighlightItem item = (HighlightItem)userList.SelectedItem;
			
			if (!MessageService.AskQuestion("${res:Dialog.Options.TextEditorOptions.EditHighlighting.DeleteConfirm}")) return;
			
			try {
				File.Delete(item.FileName);
			} catch (Exception e) {
				MessageService.ShowError(e, "${res:Dialog.Options.TextEditorOptions.EditHighlighting.DeleteError}");
			}
			userList.Items.RemoveAt(userList.SelectedIndex);
			BuiltinListSelectedIndexChanged(this, EventArgs.Empty);
		}
		
		ArrayList errors = new ArrayList();
		
		private SchemeNode LoadFile(XmlTextReader reader, bool userList)
		{
			try {
				XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
				Stream schemaStream = typeof(SyntaxMode).Assembly.GetManifestResourceStream("ICSharpCode.TextEditor.Resources.Mode.xsd");
				validatingReader.Schemas.Add("", new XmlTextReader(schemaStream));
				validatingReader.ValidationType = ValidationType.Schema;
				validatingReader.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
				
				XmlDocument doc = new XmlDocument();
				doc.Load(validatingReader);
				
				if (errors.Count != 0) {
					ReportErrors();
					return null;
				} else {
					return new SchemeNode(doc.DocumentElement, userList);
				}
			} catch (Exception e) {

				MessageService.ShowError(e, "${res:Dialog.Options.TextEditorOptions.EditHighlighting.LoadError}");
				return null;
			} finally {
				reader.Close();
			}
			
		}
		
		private void ValidationHandler(object sender, ValidationEventArgs args)
		{
			errors.Add(args);
		}

		private void ReportErrors()
		{
			StringBuilder msg = new StringBuilder();
			msg.Append(ResourceService.GetString("Dialog.Options.TextEditorOptions.EditHighlighting.LoadError") + "\n\n");
			foreach(ValidationEventArgs args in errors) {
				msg.Append(args.Message);
				msg.Append(Console.Out.NewLine);
			}
			
			MessageService.ShowWarning(msg.ToString());
		}
			
		internal class HighlightItem
		{
			string fileName;
			SyntaxMode syntaxMode;
			SchemeNode node;
			
			public HighlightItem(SyntaxMode mode, string filename, SchemeNode Node)
			{
				syntaxMode = mode;
				fileName = filename;
				node = Node;
			}
			
			public string FileName {
				get {
					return fileName;
				}
			}
			
			public SyntaxMode SyntaxMode {
				get {
					return syntaxMode;
				}
			}
			
			public SchemeNode Node {
				get {
					return node;
				}
			}
			
			public override string ToString()
			{
				try {
					return syntaxMode.Name + " (" + String.Join(", ", node.Extensions) + ")";
				} catch {
					return fileName;
				}
			}
		}
	}
}
