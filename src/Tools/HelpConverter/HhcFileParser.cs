// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christoph Wille" email="christophw@alphasierrapapa.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.HelpConverter
{
	public class HhcFileParser
	{
		string hhcFileContents;
		string chmName;
		string basePath;
		
		ArrayList rootTreeNodes = new ArrayList();
		XmlNode   currentNode   = null;
		
		public HhcFileParser(string basePath)
		{
			this.basePath = basePath;
		}
		
		void LoadHhcFile(string fileName)
		{
			FileInfo fi = new FileInfo(basePath + Path.DirectorySeparatorChar + fileName);
			StreamReader sr = fi.OpenText();
			hhcFileContents = sr.ReadToEnd();
			sr.Close();
		}
		
		void MakeXmlCompliant()
		{
			StringBuilder strFixup =
			new StringBuilder(Regex.Replace(hhcFileContents,
			                                "(?'start'<param\\s[^>]*)(?'end'\"/?>)",
			                                "${start}\"/>"));
			
			strFixup.Replace("</OBJECT></UL>", "</OBJECT></LI></UL>");
			strFixup.Replace("</OBJECT><LI>", "</OBJECT></LI><LI>");
			strFixup.Replace("</OBJECT><UL><LI>", "</OBJECT></LI><UL><LI>");
			hhcFileContents = strFixup.ToString();
		}
		
		void Load(string fileName)
		{
			LoadHhcFile(fileName);
			MakeXmlCompliant();
		}
		
		public void Parse(XmlDocument helpFileDocument, XmlNode currentNode, string hhcFileName, string chmName)
		{
			this.chmName = chmName;
			
			Load(hhcFileName);
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(hhcFileContents);
			
			XmlNode root = doc.ChildNodes[0];
			XmlNode outermostList = null;
			
			switch (root.Name.ToLower()) {
				case "ul":
					outermostList = root;
					break;
				case "html": // this would be well-formed HTML
					outermostList = root["BODY"]["UL"];
					break;
				case "#comment":  // this is NDoc malformed HTML
					outermostList = doc.ChildNodes[1];
					break;
				default:
					// this is nothing we can read; please send us the .hhc file
					break;
			}
			
			if (null == outermostList) {
				Console.WriteLine("Format of file not valid, cannot find root <UL> node");
				return;
			}
			
			this.currentNode = currentNode;
			EvaluateLevel(helpFileDocument, outermostList);
			
			// quick & dirty overview of the structure
			// Console.WriteLine("---------------------------");
			// DumpElement(root, 0);
		}
		
		void EvaluateLevel(XmlDocument helpFileDocument, XmlNode currentLevel)
		{
			for (int i = 0; i < currentLevel.ChildNodes.Count; i++) {
				XmlNode currentElement = currentLevel.ChildNodes[i];
				
				// we need a lookup on the next node - is it a UL, then the current node
				// node needs to be displayed as a folder, otherwise it is a non-expandable
				// leaf node in our tree
				bool bIsFolderNode = false;
				if (i < (currentLevel.ChildNodes.Count - 1)) {
					XmlNode nextElement = currentLevel.ChildNodes[i + 1];
					if ("UL" == nextElement.Name) bIsFolderNode = true;
				}
				
				string strNodeName, strNodeUrl;
				if (bIsFolderNode) {
					GetNodeInformation(currentElement["OBJECT"], out strNodeName, out strNodeUrl);
					
					// we need to create a folder and continue traversing
					XmlElement folderElement = helpFileDocument.CreateElement("HelpFolder");
					
					XmlAttribute  attrib = helpFileDocument.CreateAttribute("name");
					attrib.InnerText = strNodeName;
					folderElement.Attributes.Append(attrib);
					currentNode.AppendChild(folderElement);
					
					
					XmlNode savedNode = currentNode;
					currentNode = folderElement;
					EvaluateLevel(helpFileDocument, currentLevel.ChildNodes[i + 1]);
					currentNode = savedNode;
				} else {
					// we have a leaf node here, but we need to ignore UL's, those are handled by
					// their parents above
					if ("UL" == currentElement.Name) {
						// we do nothing with this element
					} else {
						GetNodeInformation(currentElement["OBJECT"], out strNodeName, out strNodeUrl);
						
						XmlElement helpTopic = helpFileDocument.CreateElement("HelpTopic");
						
						XmlAttribute  attrib = helpFileDocument.CreateAttribute("name");
						attrib.InnerText = strNodeName;
						helpTopic.Attributes.Append(attrib);
						attrib = helpFileDocument.CreateAttribute("link");
						attrib.InnerText = chmName + "::/" + strNodeUrl;
						helpTopic.Attributes.Append(attrib);
						currentNode.AppendChild(helpTopic);
						
					}
				}
			}
		}
		
		void GetNodeInformation(XmlNode theNode, out string NodeName, out string NodeUrl)
		{
			// <param name="Name" value="name of node"/> and url are read from OBJECT node
			string strParamType, strParamValue;
			strParamType = strParamValue = NodeName = NodeUrl = "";
			
			for (int i = 0; i < theNode.ChildNodes.Count; i++) {
				XmlNode currentParam = theNode.ChildNodes[i];
				// we work with param nodes only, anything else cannot be handled here
				if ("param" != currentParam.Name) {
					return;
				}
				
				// if any of the below two XmlAttribute objects is not available,
				// we will get a null ref exception
				try {
					strParamType = currentParam.Attributes["name"].Value;
					strParamValue = currentParam.Attributes["value"].Value;
				} catch {
					strParamType = "";	// because of the switch statement, we need a default value when it fails
				}
				
				switch (strParamType) {
					case "Name":
						NodeName = strParamValue;
						break;
					case "Local":
						NodeUrl = strParamValue;
						break;
				}
			}
		}
		
		// for internal use only - dump entire structure to the console.
		void DumpElement(XmlNode el, int nDepth)
		{
			string strTree = new String(' ', nDepth);
			
			foreach(XmlNode subEl in el.ChildNodes) {
				Console.WriteLine(strTree + subEl.Name);
				DumpElement(subEl, nDepth+1);
			}
		}
	}
}
