// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.FormDesigner
{
	public class XmlDesignerGenerator : IDesignerGenerator
	{
		FormDesignerViewContent viewContent;
		
		public void Attach(FormDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
			IComponentChangeService componentChangeService = (IComponentChangeService)viewContent.DesignSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged    += new ComponentChangedEventHandler(ComponentChanged);
		
		}
		void ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			viewContent.IsDirty = true;
		}
		public void MergeFormChanges()
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
			viewContent.Document.TextContent = writer.ToString();
		}
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out int position)
		{
			position = 0;
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
		
		public XmlElement GetElementFor(XmlDocument doc, object o)
		{
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			
			if (o == null) {
				throw new ArgumentNullException("o");
			}
			
			try {
				XmlElement el = doc.CreateElement(o.GetType().FullName);
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
					if (pd.Name == "DataBindings" || 
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
					childEl = doc.CreateElement(pd.Name);
					
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
							XmlElement newEl = GetElementFor(doc, listObject);
							if (newEl != null) {
								childEl.AppendChild(newEl);
							}
						}
						
						// only insert lists that contain elements (no empty lists!)
						if (childEl.ChildNodes.Count > 0) {
							childNodes.Add(childEl);
						}
					} else if (pd.ShouldSerializeValue(o) && pd.IsBrowsable) {
						XmlAttribute valueAttribute = doc.CreateAttribute("value");
						if (propertyValue is Font) {
							Font f = (Font)propertyValue;
							propertyValue = new Font(f.FontFamily, (float)Math.Round(f.Size));
						}
						
						valueAttribute.InnerText = propertyValue == null ? null : propertyValue.ToString();
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
			
			// insert root element
			componentsElement.AppendChild(GetElementFor(doc, host.RootComponent));
			
			// insert any non gui (=tray components)
			foreach (IComponent component in host.Container.Components) {
				if (!(component is Control)) {
					componentsElement.AppendChild(GetElementFor(doc, component));
				}
			}
			
			return componentsElement;
		}
	}
}
