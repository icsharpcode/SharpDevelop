// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Represents a Wix File element in a Wix XML document.
	/// </summary>
	public class WixFileElement : XmlElement
	{		
		public const string FileElementName = "File";

		public WixFileElement(WixDocument document)
			: base(document.WixNamespacePrefix, FileElementName, WixNamespaceManager.Namespace, document)
		{
		}
		
		public WixFileElement(WixDocument document, string fileName) : this(document)
		{
			Init(fileName);
		}
		
		/// <summary>
		/// Generates an id from the filename.
		/// </summary>
		/// <remarks>If the filename has full path information this is
		/// ignored when generating the id. The id generated just removes any
		/// invalid id characters. A valid id can must start with a letter or 
		/// an underscore and can contain the characters A-Z (a-z), digits, underscores, 
		/// or periods.</remarks>
		public static string GenerateId(string fileName)
		{
			if (fileName == null) {
				return String.Empty;
			}
			StringBuilder id = new StringBuilder();
			for (int i = 0; i < fileName.Length; ++i) {
				id.Append(GetIdCharacter(fileName[i], i));
			}
			return id.ToString();
		}
		
		public string Source {
			get {
				return GetAttribute("Source");
			}
			set {
				SetAttribute("Source", value);
			}
		}
		
		/// <summary>
		/// Gets the short name of the file.
		/// </summary>
		public string ShortName {
			get {
				return GetAttribute("Name");
			}
			set {
				SetAttribute("Name", value);
			}
		}
		
		public string Id {
			get {
				return GetAttribute("Id");
			}
			set {
				SetAttribute("Id", value);
			}
		}
		
		public string LongName {
			get {
				return GetAttribute("LongName");
			}
			set {
				SetAttribute("LongName", value);
			}
		}
		
		/// <summary>
		/// Valid Id letters are letters A-Z, a-z, underscores, digits and periods.
		/// </summary>
		static bool IsValidIdCharacter(char ch)
		{
			return IsValidIdLetter(ch) || Char.IsDigit(ch);
		}
		
		/// <summary>
		/// Valid Id letters are letters A-Z, a-z, underscores and periods.
		/// </summary>
		static bool IsValidIdLetter(char ch)
		{
			return IsLetterOrUnderscore(ch) || (ch == '.');
		}
		
		static bool IsLetterOrUnderscore(char ch)
		{
			return IsLetter(ch) || (ch == '_');
		}
		
		/// <summary>
		/// The characters A-Z, a-z are considered to be valid letters.
		/// </summary>
		static bool IsLetter(char ch)
		{
			return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
		}
		
		/// <summary>
		/// Returns a valid id character for the given position. If the
		/// character is an invalid character it is replaced with an underscore.
		/// </summary>
		static char GetIdCharacter(char ch, int index)
		{
			if (IsValidIdCharacter(ch)) {
				// First char must be a letter or underscore.
				if (index > 0 || IsLetterOrUnderscore(ch)) {
					return ch;
				}
			} 
			return '_';
		}
		
		/// <summary>
		/// Initialises a new Wix File element with the specified 
		/// <paramref name="fileName"/>.
		/// </summary>
		/// <remarks>
		/// The element generated will have an Id, LongName and Name 
		/// all set and derived from the <paramref name="fileName"/>
		/// </remarks>
		void Init(string fileName)
		{
			WixDocument document = (WixDocument)OwnerDocument;
			string baseDirectory = Path.GetDirectoryName(document.FileName);
			string sourceFileName = FileUtility.GetRelativePath(baseDirectory, fileName);

			Source = sourceFileName;
			
			string longFileName = null;
			string shortFileName = Path.GetFileName(sourceFileName);
			string idFileName = shortFileName;
			if (shortFileName.Length > ShortFileName.MaximumFileNameLength) {
				longFileName = shortFileName;
				shortFileName = ShortFileName.Convert(shortFileName);
				idFileName = longFileName;
			}
			
			
			ShortName = shortFileName;
			Id = GenerateId(idFileName);
			
			if (longFileName != null) {
				LongName = longFileName;				
			} 
		}
	}
}
