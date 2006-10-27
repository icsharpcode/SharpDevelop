// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	public static class LanguageService
	{
		static string languagePath = FileUtility.Combine(PropertyService.DataDirectory, "resources", "languages");
		
		static ImageList languageImageList = null;
		static ArrayList languages         = null;
		
		public static ImageList LanguageImageList {
			get {
				return languageImageList;
			}
		}
		
		public static ArrayList Languages {
			get {
				return languages;
			}
		}
		
		static LanguageService()
		{
			languageImageList = new ImageList();
			languageImageList.ColorDepth = ColorDepth.Depth32Bit;
			languages         = new ArrayList();
			LanguageImageList.ImageSize = new Size(46, 38);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(languagePath, "LanguageDefinition.xml"));
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			
			foreach (XmlNode node in nodes) {
				XmlElement el = node as XmlElement;
				if (el != null) {
					languages.Add(new Language(el.Attributes["name"].InnerText,
					                           el.Attributes["code"].InnerText,
					                           LanguageImageList.Images.Count));
					LanguageImageList.Images.Add(new Bitmap(Path.Combine(languagePath, el.Attributes["icon"].InnerText)));
				}
			}
		}
		
	}
}
