// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;

using SA = ICSharpCode.SharpAssembly.Assembly;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class XmlView : UserControl
	{
		private Button      saveButton = new Button();
		private CheckBox    exportEvents = new CheckBox();
		private CheckBox    exportFields = new CheckBox();
		private CheckBox    exportMethods = new CheckBox();
		private CheckBox    exportProperties = new CheckBox();
		private Label       captionLabel = new Label();
		
		private AssemblyTreeNode SelectedNode;
		private XmlTextWriter writer;
		
		AssemblyTree tree;
		
		public XmlView(AssemblyTree _tree)
		{
			tree = _tree;
			
			captionLabel.Location = new Point(5, 0);
			captionLabel.Text = StringParser.Parse("${res:ObjectBrowser.XML.Desc}");
			captionLabel.Size = new Size(300, 25);
			captionLabel.FlatStyle = FlatStyle.System;
			
			exportEvents.Location = new Point(15, 40);
			exportEvents.Text = StringParser.Parse("${res:ObjectBrowser.XML.ExpEvt}");
			exportEvents.Checked = true;
			exportEvents.Width = 300;
			exportEvents.FlatStyle = FlatStyle.System;
			
			exportFields.Location = new Point(15, 65);
			exportFields.Text = StringParser.Parse("${res:ObjectBrowser.XML.ExpFld}");
			exportFields.Checked = true;
			exportFields.Width = 300;
			exportFields.FlatStyle = FlatStyle.System;
			
			exportMethods.Location = new Point(15, 90);
			exportMethods.Text = StringParser.Parse("${res:ObjectBrowser.XML.ExpMeth}");
			exportMethods.Checked = true;
			exportMethods.Width = 300;
			exportMethods.FlatStyle = FlatStyle.System;
			
			exportProperties.Location = new Point(15, 115);
			exportProperties.Width = 300;
			exportProperties.Text = StringParser.Parse("${res:ObjectBrowser.XML.ExpProp}");
			exportProperties.Checked = true;
			exportProperties.FlatStyle = FlatStyle.System;
			
			saveButton.Text = StringParser.Parse("${res:ObjectBrowser.XML.Save}");
			saveButton.Location = new Point(5, 160);
			saveButton.Enabled = false;
			saveButton.Click += new EventHandler(saveButton_Clicked);
			saveButton.FlatStyle = FlatStyle.System;
			
			Dock = DockStyle.Fill;
			Controls.AddRange(new Control[] { 
				              captionLabel,
							  saveButton,
			                  exportEvents,
			                  exportFields,
			                  exportMethods,
			                  exportProperties});
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
		}
		
		void saveButton_Clicked(object sender, System.EventArgs e) {
			
			SaveFileDialog fdialog = new SaveFileDialog();
			fdialog.Filter = StringParser.Parse("${res:ObjectBrowser.Filters.XML}") + "|*.xml";
			DialogResult result = fdialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			
			if(result != DialogResult.Cancel) {
				
				if (SelectedNode.Attribute is IClass) {
					writeStart(fdialog.FileName, ((SelectedNode.Attribute as IClass).DeclaringType as SA.SharpAssembly).FullName);
					exportClass((IClass)SelectedNode.Attribute);
				} else if (SelectedNode.Attribute is SA.SharpAssembly) {
					writeStart(fdialog.FileName, ((SA.SharpAssembly)SelectedNode.Attribute).FullName);
					foreach (SharpAssemblyClass type in SharpAssemblyClass.GetAssemblyTypes((SA.SharpAssembly)SelectedNode.Attribute)) {
						if(type.ToString().IndexOf("PrivateImplementationDetails") == -1) {					
							exportClass(type);
						}
					}
				}
				writeEnd();
			}
		}
		
		string GetScope(IDecoration type)
		{
			string retval;
			
			if (type.IsPublic)
				retval = "Public";
			else if (type.IsPrivate)
				retval = "Private";
			else if (type.IsProtectedOrInternal)
				retval = "ProtectedOrInternal";
			else if (type.IsProtectedAndInternal)
				retval = "Protected Internal";
			else if (type.IsProtected)
				retval = "Protected";
			else
				retval = "Internal";
			return retval;
		}
		
		void writeStart(string filename, string assemblyname) {
			try {
				writer = new XmlTextWriter(filename ,new System.Text.ASCIIEncoding());
			} catch (Exception e) {
				System.Windows.Forms.MessageBox.Show(e.Message);
				return;
			}
			
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 3;
			
			writer.WriteRaw("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
			writer.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"xml2html.xsl\" ?>");
			
			writer.WriteStartElement("assembly");
			writer.WriteAttributeString("name", assemblyname);
		}
		
		void writeEnd() {
			writer.WriteEndElement();
			writer.Flush();
			writer.Close();
			writer = null;
		}
		
		void exportClass(IClass type) {
			writer.WriteStartElement("class");
			writer.WriteAttributeString("name", type.Name);
			writer.WriteAttributeString("scope", GetScope(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			
			// events
			if(exportEvents.Checked) {
				writer.WriteStartElement("events");
				foreach(IEvent event_ in type.Events) {
					if(event_.DeclaringType == type) {
						writer.WriteStartElement("event");
						writer.WriteAttributeString("name", event_.Name);
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			
			// fields
			if(exportFields.Checked) {
				writer.WriteStartElement("fields");
				foreach(IField field in type.Fields) {
					if(field.DeclaringType == type) {
						writer.WriteStartElement("field");
						writer.WriteAttributeString("name", field.Name);
						writer.WriteAttributeString("type", field.ReturnType.ToString());
						writer.WriteAttributeString("scope", GetScope(field));
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			
			// methods
			if(exportMethods.Checked) {
				writer.WriteStartElement("methods");
				foreach(IMethod method in type.Methods) {
					if(!SharpAssemblyMethod.IsSpecial(method)) {
						if(method.DeclaringType == type) {
							
							writer.WriteStartElement("method");
							writer.WriteAttributeString("name", method.Name);
							writer.WriteAttributeString("scope", GetScope(method));
							writer.WriteAttributeString("type", method.ReturnType.ToString());
							
							WriteParameters(writer, method);
							writer.WriteEndElement();
						}
					}
				}
				writer.WriteEndElement();
			}
			
			// properties
			if(exportProperties.Checked) {
				writer.WriteStartElement("properties");
				foreach(IProperty property in type.Properties) {
					if(property.DeclaringType == type) {
						writer.WriteStartElement("property");
						writer.WriteAttributeString("name", property.Name);
						writer.WriteAttributeString("type", property.ReturnType.ToString());
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		
		void WriteParameters(XmlTextWriter writer, IMethod member) {
			foreach(IParameter param in member.Parameters) {
				writer.WriteStartElement("param");
				writer.WriteAttributeString("name", param.Name);
				writer.WriteAttributeString("type", param.ReturnType.ToString());
				writer.WriteEndElement();
			}
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			SelectedNode = (AssemblyTreeNode)e.Node;
			saveButton.Enabled = (SelectedNode.Attribute is IClass || SelectedNode.Attribute is SA.SharpAssembly);
		}
	}
	
}
