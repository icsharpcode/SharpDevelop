// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner
{
	public class XmlDesignerGenerator : IDesignerGenerator
	{
		FormsDesignerViewContent viewContent;
		
		public CodeDomProvider CodeDomProvider {
			get {
				return new Microsoft.CSharp.CSharpCodeProvider();
			}
		}
		
		public FormsDesignerViewContent ViewContent {
			get { return this.viewContent; }
		}
		
		public void Attach(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public void Detach()
		{
			this.viewContent = null;
		}
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			designerCodeFile = this.viewContent.PrimaryFile;
			return new [] {designerCodeFile};
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
			StringWriter writer = new StringWriter();
			XmlTextWriter xml = new XmlTextWriter(writer);
			xml.Formatting = Formatting.Indented;
			
			//xml.WriteStartDocument();
			XmlElement el = GetElementFor(new XmlDocument(), viewContent.Host);
			xml.WriteStartElement(el.Name);
			xml.WriteAttributeString("version", "1.0");
			
			foreach (XmlNode node in el.ChildNodes) {
				node.WriteTo(xml);
			}
			xml.WriteEndElement();
			//xml.WriteEndDocument();
			viewContent.DesignerCodeFileContent = writer.ToString();
		}
		
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			position = 0;
			file = this.viewContent.PrimaryFileName;
			return false;
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			return new object[] {};
		}
		public ICollection GetCompatibleMethods(EventInfo edesc)
		{
			return new object[] {};
		}
		
		public XmlElement GetElementFor(XmlDocument doc, object o, Hashtable visitedControls)
		{
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			
			if (o == null) {
				throw new ArgumentNullException("o");
			}
			
			visitedControls[o] = null;
			
			try {
				XmlElement el = doc.CreateElement(XmlConvert.EncodeName(o.GetType().FullName));
				
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(o);
				
				Control ctrl = o as Control;
				bool nameInserted = false;
				if (ctrl != null) {
					XmlElement childEl = doc.CreateElement("Name");
					XmlAttribute valueAttribute = doc.CreateAttribute("value");
					valueAttribute.InnerText = ctrl.Name;
					childEl.Attributes.Append(valueAttribute);
					el.AppendChild(childEl);
					nameInserted = true;
				}
				
				// add collections as last child elements in the xml (because it is better
				// to set the properties first and then add items to controls (looks nicer
				// in XML and CODE))
				ArrayList childNodes = new ArrayList();
				
				// the Controls collection should be generated as the last
				// element because properties like 'anchor' in the child elements
				// must be applied after the size has set.
				foreach (PropertyDescriptor pd in properties) {
//					if (!pd.ShouldSerializeValue(o)) {
//						continue;
//					}
					if (pd.Name == "Name" && nameInserted) {
						continue;
					}
					if (pd.Name == "DataBindings" || pd.Name == "FlatAppearance" ||
					    // TabControl duplicate TabPages Workaround (TabPages got inserted twice : In Controls and in TabPages)
					    (o.GetType().FullName == "System.Windows.Forms.TabControl" && pd.Name == "Controls")) {
						continue;
					}
					
					XmlElement childEl   = null;
					if (pd.Name == "Size" && ctrl != null && (ctrl is UserControl || ctrl is Form)) {
						childEl = doc.CreateElement("ClientSize");
						childEl.SetAttribute("value", ctrl.ClientSize.ToString());
						childNodes.Insert(0, childEl);
						continue;
					}
					childEl = doc.CreateElement(XmlConvert.EncodeName(pd.Name));
					
					object propertyValue = null;
					try {
						propertyValue = pd.GetValue(o);
					} catch (Exception e) {
						ICSharpCode.Core.LoggingService.Warn(e);
						continue;
					}
					
					// lists are other than normal properties
					if (propertyValue is IList && !(ctrl is PropertyGrid)) {
						foreach (object listObject in (IList)propertyValue) {
							XmlElement newEl = GetElementFor(doc, listObject, visitedControls);
							if (newEl != null && !newEl.Name.StartsWith("System.Windows.Forms.Design.")) {
								childEl.AppendChild(newEl);
							}
						}
						
						// only insert lists that contain elements (no empty lists!)
						if (childEl.ChildNodes.Count > 0) {
							childNodes.Add(childEl);
						}
					} else if (pd.ShouldSerializeValue(o) && pd.IsBrowsable) {
						XmlAttribute valueAttribute = doc.CreateAttribute("value");
						
						if (propertyValue == null) {
							valueAttribute.InnerText = null;
						} else if (propertyValue is Color) {
							valueAttribute.InnerText = propertyValue.ToString();
						} else {
							TypeConverter typeConv = TypeDescriptor.GetConverter(pd.PropertyType);
							valueAttribute.InnerText = typeConv.ConvertToInvariantString(propertyValue);
						}
						childEl.Attributes.Append(valueAttribute);
						childNodes.Insert(0, childEl);
					}
				}
				
				foreach (XmlElement childEl in childNodes) {
					el.AppendChild(childEl);
				}
				
				// fallback to ToString, if no members can be generated (for example
				// handling System.String)
				if (el.ChildNodes.Count == 0) {
					XmlAttribute valueAttribute = doc.CreateAttribute("value");
					valueAttribute.InnerText = o.ToString();
					el.Attributes.Append(valueAttribute);
				}
				
				return el;
			} catch (Exception e) {
				ICSharpCode.Core.MessageService.ShowError(e);
			}
			return null;
		}
		
		public XmlElement GetElementFor(XmlDocument doc, IDesignerHost host)
		{
			XmlElement componentsElement   = doc.CreateElement("Components");
			
			XmlAttribute versionAttribute = doc.CreateAttribute("version");
			versionAttribute.InnerText = "1.0";
			componentsElement.Attributes.Append(versionAttribute);
			
			Hashtable visitedControls = new Hashtable();
			// insert root element
			componentsElement.AppendChild(GetElementFor(doc, host.RootComponent, visitedControls));
			
			// insert any non gui (=tray components)
			foreach (IComponent component in host.Container.Components) {
				if (!(component is Control) && !visitedControls.ContainsKey(component)) {
					componentsElement.AppendChild(GetElementFor(doc, component, visitedControls));
				}
			}
			
			return componentsElement;
		}
	}
}
