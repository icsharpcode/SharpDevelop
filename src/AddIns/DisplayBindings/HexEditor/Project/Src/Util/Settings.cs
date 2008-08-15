// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2984 $</version>
// </file>

using System;
using System.Drawing;
using System.Xml;

namespace HexEditor.Util
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	public class Settings
	{
		Color offsetForeColor, dataForeColor;
		bool offsetBold, offsetItalic, offsetUnderline, dataBold, dataItalic, dataUnderline;
		Font offsetFont, dataFont;
		bool fitToWidth;
		int bytesPerLine;
		ViewMode viewMode;
		string[] fileTypes;
		
		public static Settings FromXML(XmlDocument document)
		{
			Settings settings = new Settings();
			Font tmpFont = new Font("Courier New", 9.75f);
			
			foreach (XmlNode node in document.FirstChild.ChildNodes)
			{
				switch (node.Attributes["Name"].Value)
				{
					case "OffsetFore":
						settings.offsetForeColor = CreateColor(node);
						break;
					case "DataFore":
						settings.dataForeColor = CreateColor(node);
						break;
					case "OffsetStyle":
						settings.offsetBold = bool.Parse(node.Attributes["Bold"].Value);
						settings.offsetItalic = bool.Parse(node.Attributes["Italic"].Value);
						settings.offsetUnderline = bool.Parse(node.Attributes["Underline"].Value);
						break;
					case "DataStyle":
						settings.dataBold = bool.Parse(node.Attributes["Bold"].Value);
						settings.dataItalic = bool.Parse(node.Attributes["Italic"].Value);
						settings.dataUnderline = bool.Parse(node.Attributes["Underline"].Value);
						break;
					case "Font":
						tmpFont = new Font(node.Attributes["FontName"].Value,
							float.Parse(node.Attributes["FontSize"].Value));
						break;
					case "TextDisplay":
						settings.bytesPerLine = int.Parse(node.Attributes["BytesPerLine"].Value);
						settings.viewMode = (ViewMode)ViewMode.Parse(typeof(ViewMode),node.Attributes["ViewMode"].Value);
						settings.fitToWidth = bool.Parse(node.Attributes["FitToWidth"].Value);
						break;
					case "FileTypes":
						settings.fileTypes = node.Attributes["FileTypes"].Value.Split(new char[] {';'});
						break;
				}
				
				FontStyle offsetStyle = FontStyle.Regular;
				if (settings.offsetBold) offsetStyle |= FontStyle.Bold;
				if (settings.offsetItalic) offsetStyle |= FontStyle.Italic;
				if (settings.offsetUnderline) offsetStyle |= FontStyle.Underline;
				
				settings.offsetFont = new Font(tmpFont, offsetStyle);
				
				FontStyle dataStyle = FontStyle.Regular;
				if (settings.dataBold) dataStyle |= FontStyle.Bold;
				if (settings.dataItalic) dataStyle |= FontStyle.Italic;
				if (settings.dataUnderline) dataStyle |= FontStyle.Underline;
				
				settings.dataFont = new Font(tmpFont, dataStyle);
			}
			
			return settings;
		}
		
		private static Color CreateColor(XmlNode node)
		{
			return Color.FromArgb(int.Parse(node.Attributes["R"].Value),
			                      int.Parse(node.Attributes["G"].Value),
			                      int.Parse(node.Attributes["B"].Value));
		}
		
		public static Settings CreateDefault()
		{
			Settings settings = new Settings();
			
			settings.bytesPerLine = 16;
			settings.fitToWidth = false;
			settings.fileTypes = new string[] {".exe", ".dll"};
			settings.viewMode = ViewMode.Hexadecimal;
			
			settings.dataBold = false;
			settings.dataItalic = false;
			settings.dataUnderline = false;
			
			settings.offsetBold = false;
			settings.offsetItalic = false;
			settings.offsetUnderline = false;
			
			settings.dataForeColor = Color.Black;
			settings.offsetForeColor = Color.Blue;
			
			settings.offsetFont = settings.dataFont = new Font("Courier New", 9.5f, FontStyle.Regular);
			
			return settings;
		}
		
		public Color OffsetForeColor {
			get { return offsetForeColor; }
			set { offsetForeColor = value; }
		}
		
		public Color DataForeColor {
			get { return dataForeColor; }
			set { dataForeColor = value; }
		}
		
		public bool OffsetBold {
			get { return offsetBold; }
			set { offsetBold = value; }
		}
		
		public bool OffsetItalic {
			get { return offsetItalic; }
			set { offsetItalic = value; }
		}
		
		public bool OffsetUnderline {
			get { return offsetUnderline; }
			set { offsetUnderline = value; }
		}
		
		public bool DataBold {
			get { return dataBold; }
			set { dataBold = value; }
		}
		
		public bool DataItalic {
			get { return dataItalic; }
			set { dataItalic = value; }
		}
		
		public bool DataUnderline {
			get { return dataUnderline; }
			set { dataUnderline = value; }
		}
		
		public Font OffsetFont {
			get { return offsetFont; }
			set { offsetFont = value; }
		}
		
		public Font DataFont {
			get { return dataFont; }
			set { dataFont = value; }
		}
		
		public bool FitToWidth {
			get { return fitToWidth; }
			set { fitToWidth = value; }
		}
		
		public int BytesPerLine {
			get { return bytesPerLine; }
			set { bytesPerLine = value; }
		}
		
		public ViewMode ViewMode {
			get { return viewMode; }
			set { viewMode = value; }
		}
				
		public string[] FileTypes {
			get { return fileTypes; }
			set { fileTypes = value; }
		}
	}
}
