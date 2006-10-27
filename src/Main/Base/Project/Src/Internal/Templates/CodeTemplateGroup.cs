// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class reperesents a single Code Template
	/// </summary>
	public class CodeTemplateGroup
	{
		List<string> extensions = new List<string>();
		List<CodeTemplate> templates  = new List<CodeTemplate>();
		
		public List<string> Extensions {
			get {
				return extensions;
			}
		}
		
		public List<CodeTemplate> Templates {
			get {
				return templates;
			}
		}
		
		public string[] ExtensionStrings {
			get {
				string[] extensionStrings = new string[extensions.Count];
				extensions.CopyTo(extensionStrings, 0);
				return extensionStrings;
			}
			set {
				extensions.Clear();
				foreach (string str in value) {
					extensions.Add(str);
				}
			}
		}
		
		public CodeTemplateGroup(string extensions)
		{
			ExtensionStrings = extensions.Split(';');
		}
		
		public CodeTemplateGroup(XmlElement el)
		{
			if (el == null) {
				throw new ArgumentNullException("el");
			}
			string[] exts = el.GetAttribute("extensions").Split(';');
			foreach (string ext in exts) {
				extensions.Add(ext);
			}
			foreach (XmlNode childNode in el.ChildNodes) {
				XmlElement childElement = childNode as XmlElement;
				if (childElement != null) {
					templates.Add(new CodeTemplate(childElement));
				}
			}
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			XmlElement newElement = doc.CreateElement("CodeTemplateGroup");
			
			newElement.SetAttribute("extensions", String.Join(";", ExtensionStrings));
			
			foreach (CodeTemplate codeTemplate in templates) {
				newElement.AppendChild(codeTemplate.ToXmlElement(doc));
			}
			
			return newElement;
		}
		
	}
}
