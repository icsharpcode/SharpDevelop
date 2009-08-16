// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor;

namespace ICSharpCode.FormsDesigner
{
	public class XmlDesignerLoader : BasicDesignerLoader, IObjectCreator
	{
		IDesignerGenerator generator;
		
		public XmlDesignerLoader(IDesignerGenerator generator)
		{
			this.generator = generator;
		}
		
		IDesignerLoaderHost host;
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			Debug.Assert(host != null);
			this.host = host;
			host.AddService(typeof(INameCreationService), new NameCreationService(host));
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService((IServiceProvider)host));
			base.BeginLoad(host);
		}
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			XmlLoader loader = new XmlLoader();
			loader.ObjectCreator = this;
			loader.CreateObjectFromXmlDefinition(this.generator.ViewContent.DesignerCodeFileContent);
		}
		
		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			// the XML designer is not based on CodeDom, so we pass null for the CodeCompileUnit
			generator.MergeFormChanges(null);
		}
		
		Type IObjectCreator.GetType(string name)
		{
			return host.GetType(name);
		}
		
		object IObjectCreator.CreateObject(string name, XmlElement el)
		{
			string componentName = null;
			
			if (el != null) {
				foreach (XmlNode childNode in el) {
					if (childNode.Name == "Name") {
						componentName = ((XmlElement)childNode).GetAttribute("value");
						break;
					}
				}
			}
			Debug.Assert(componentName != null);
			
			Type componentType = host.GetType(name);
			Debug.Assert(componentType != null);
			
			object newObject = host.CreateComponent(componentType, componentName);
			
			if (newObject is Control) {
				((Control)newObject).SuspendLayout();
			}
			
			return newObject;
		}
		
		public class NameCreationService : INameCreationService
		{
			IDesignerHost host;
			
			public NameCreationService(IDesignerHost host)
			{
				this.host = host;
			}
			
			public string CreateName(Type dataType)
			{
				return CreateName(host.Container, dataType);
			}
			
			public string CreateName(IContainer container, Type dataType)
			{
				string name = Char.ToLower(dataType.Name[0]) + dataType.Name.Substring(1);
				int number = 1;
				if (container != null) {
					while (container.Components[name + number.ToString()] != null) {
						++number;
					}
				}
				return name + number.ToString();
			}
			
			public bool IsValidName(string name)
			{
				if (name == null || name.Length == 0 || !(Char.IsLetter(name[0]) || name[0] == '_')) {
					return false;
				}
				
				foreach (char ch in name) {
					if (!Char.IsLetterOrDigit(ch) && ch != '_') {
						return false;
					}
				}
				
				return true;
			}
			
			public void ValidateName(string name)
			{
				if (!IsValidName(name)) {
					throw new System.Exception("Invalid name " + name);
				}
			}
		}	
	}
}
