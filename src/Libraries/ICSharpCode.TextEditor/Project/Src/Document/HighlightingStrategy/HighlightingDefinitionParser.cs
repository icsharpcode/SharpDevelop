// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;

namespace ICSharpCode.TextEditor.Document
{
	internal class HighlightingDefinitionParser
	{
		private HighlightingDefinitionParser()
		{
			// This is a pure utility class with no instances.	
		}
		
		static ArrayList errors = null;
		
		public static DefaultHighlightingStrategy Parse(SyntaxMode syntaxMode, XmlTextReader xmlTextReader)
		{
			try {
				XmlReaderSettings settings = new XmlReaderSettings();
				Stream shemaStream = typeof(HighlightingDefinitionParser).Assembly.GetManifestResourceStream("ICSharpCode.TextEditor.Resources.Mode.xsd");
				settings.Schemas.Add("", new XmlTextReader(shemaStream));
				settings.Schemas.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
				settings.ValidationType = ValidationType.Schema;
				XmlReader validatingReader = XmlReader.Create(xmlTextReader, settings);

//				XmlValidatingReader validatingReader = new XmlValidatingReader(xmlTextReader);
				//				Stream shemaStream = Assembly.GetCallingAssembly().GetManifestResourceStream("Resources.Mode.xsd");
//				validatingReader.Schemas.Add("", new XmlTextReader(shemaStream));
//				validatingReader.ValidationType = ValidationType.Schema;
			    
				
				XmlDocument doc = new XmlDocument();
				doc.Load(validatingReader);
				
				DefaultHighlightingStrategy highlighter = new DefaultHighlightingStrategy(doc.DocumentElement.Attributes["name"].InnerText);
				
				if (doc.DocumentElement.Attributes["extensions"]!= null) {
					highlighter.Extensions = doc.DocumentElement.Attributes["extensions"].InnerText.Split(new char[] { ';', '|' });
				}
				
				XmlElement environment = doc.DocumentElement["Environment"];
				if (environment != null) {
					foreach (XmlNode node in environment.ChildNodes) {
						if (node is XmlElement) {
							XmlElement el = (XmlElement)node;
							highlighter.SetColorFor(el.Name, el.HasAttribute("bgcolor") ? new HighlightBackground(el) : new HighlightColor(el));
						}
					}
				}
				
				// parse properties
				if (doc.DocumentElement["Properties"]!= null) {
					foreach (XmlElement propertyElement in doc.DocumentElement["Properties"].ChildNodes) {
						highlighter.Properties[propertyElement.Attributes["name"].InnerText] =  propertyElement.Attributes["value"].InnerText;
					}
				}
				
				if (doc.DocumentElement["Digits"]!= null) {
					highlighter.DigitColor = new HighlightColor(doc.DocumentElement["Digits"]);
				}
				
				XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("RuleSet");
				foreach (XmlElement element in nodes) {
					highlighter.AddRuleSet(new HighlightRuleSet(element));
				}
				
				xmlTextReader.Close();
				
				if(errors!=null) {
					ReportErrors(syntaxMode.FileName);
					errors = null;
					return null;
				} else {
					return highlighter;		
				}
			} catch (Exception e) {
				MessageBox.Show("Could not load mode definition file.\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return null;
			}
		}	
		
		private static void ValidationHandler(object sender, ValidationEventArgs args)
		{
			if (errors==null) {
				errors=new ArrayList();
			}
			errors.Add(args);
		}

		private static void ReportErrors(string fileName)
		{
			StringBuilder msg = new StringBuilder();
			msg.Append("Could not load mode definition file. Reason:\n\n");
			foreach(ValidationEventArgs args in errors) {
				msg.Append(args.Message);
				msg.Append(Console.Out.NewLine);
			}
			MessageBox.Show(msg.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
		}

	}
}
