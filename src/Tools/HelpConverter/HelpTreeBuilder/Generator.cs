using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	public class Generator
	{
		public Generator(XmlDocument doc, XmlNode rootNode, string[] args)
		{
			AssemblyLoader loader = new AssemblyLoader();
			Assembly[] assemblies;
			
			try {
				assemblies = loader.LoadAssemblies(args);
			} catch(Exception e) {
				throw e;
			}
					
			Hashtable typesByNamespace = loadTypesByNamespace(assemblies);
			
			string[] namespacesSorted = new string[typesByNamespace.Keys.Count];
			int count = 0;
			
			foreach(string nspace in typesByNamespace.Keys) {
				namespacesSorted[count] = nspace;
				count++;
			}
			
			Array.Sort(namespacesSorted);
			
			foreach(string nspace in namespacesSorted) {
				// create folder node for namespace
				XmlNode namespaceNode = doc.CreateElement("HelpFolder");
				XmlAttribute attrib = doc.CreateAttribute("name");
				attrib.Value = nspace;
				
				
				namespaceNode.Attributes.Append(attrib);
				
				string[] typesSorted = new string[((Hashtable)typesByNamespace[nspace]).Count];
				count = 0;
				
				foreach(string typeName in ((Hashtable)typesByNamespace[nspace]).Keys) {
					typesSorted[count] = typeName;
					count++;
				}
				
				Array.Sort(typesSorted);
				
				// put the namespace in a separate file
				XmlDocument newDoc = new XmlDocument();
				newDoc.LoadXml("<HelpCollection/>");
				bool setNamespaceLink = false;
				foreach(string typeName in typesSorted) {
					Type type = (Type)((Hashtable)typesByNamespace[nspace])[typeName];
					try {
						newDoc.DocumentElement.AppendChild(TypeNodeFactory.CreateNode(type, newDoc));
						// set link to namespace description
						if (!setNamespaceLink) {
							classNodeBuilder cbn = new classNodeBuilder();
							cbn.SetLink(doc, namespaceNode, cbn.ConvertLink(classNodeBuilder.helpFileFormat.NamespaceFormat,type ));	
							setNamespaceLink = true;
						}
					} catch(Exception e) {
						System.Console.WriteLine(e.Message);
					}
				}
				string helpFileName = Application.StartupPath + Path.DirectorySeparatorChar + nspace + "Help.xml";
				HelpBrowserApp.HelpFiles.Add(helpFileName);
				newDoc.Save(helpFileName);
				
				// create help reference
				XmlElement referenceNode = doc.CreateElement("HelpReference");
				attrib = doc.CreateAttribute("reference");
				attrib.Value = nspace + "Help.xml";
				referenceNode.Attributes.Append(attrib);
				namespaceNode.AppendChild(referenceNode);
				
				// add the namespace + reference node to the document
				rootNode.AppendChild(namespaceNode);
			}
		}
		
		Hashtable loadTypesByNamespace(Assembly[] assemblies)
		{
			Hashtable namespaces = new Hashtable();
			
			foreach(Assembly assembly in assemblies) {
				
				foreach(Type type in assembly.GetTypes()) {
					if(type.Namespace != null && type.IsPublic) {
						
						if(namespaces.Contains(type.Namespace) == false) {
							namespaces.Add(type.Namespace, new Hashtable());
						}
						
						((Hashtable)namespaces[type.Namespace]).Add(type.Name, type);
						
					} else {
						System.Console.WriteLine("no namespace, ignoring: " + type.Name);
					}
				}
			}
			return namespaces;
		}
	}
}
