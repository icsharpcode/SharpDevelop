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
		
		WixComponentElement parentComponent;

		public WixFileElement(WixDocument document)
			: base(document.WixNamespacePrefix, FileElementName, WixNamespaceManager.Namespace, document)
		{
		}
		
		public WixFileElement(WixDocument document, string fileName) : this(document)
		{
			Init(fileName);
		}
		
		public WixFileElement(WixComponentElement component, string fileName) : this((WixDocument)component.OwnerDocument)
		{
			parentComponent = component;
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
		
		/// <summary>
		/// Gets the filename where the resource being added to the setup 
		/// package can be found. Typically this is relative to the Wix document the
		/// File element is a part of.
		/// </summary>
		public string Source {
			get {
				return GetAttribute("Source");
			}
			set {
				SetAttribute("Source", value);
			}
		}
		
		/// <summary>
		/// Gets the short name (8.3) of the file.
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
		
		/// <summary>
		/// The is the filename that will be used when installing the file.
		/// </summary>
		public string LongName {
			get {
				return GetAttribute("LongName");
			}
			set {
				SetAttribute("LongName", value);
			}
		}
		
		/// <summary>
		/// Gets the full path to the file. If the parent WixDocument
		/// has no filename then the relative path as stored in the 
		/// wix document is returned.
		/// </summary>
		public string FileName {
			get {
				WixDocument document = OwnerDocument as WixDocument;
				if (document != null && !String.IsNullOrEmpty(document.FileName)) {
					string directory = Path.GetDirectoryName(document.FileName);
					return Path.GetFullPath(Path.Combine(directory, Source));
				}
				return Source;
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
				shortFileName = ShortFileName.Convert(shortFileName, ShortFileNameExists);
				idFileName = longFileName;
			}
			
			ShortName = shortFileName;
			Id = GenerateUniqueId(Path.GetDirectoryName(fileName), idFileName);
			
			if (longFileName != null) {
				LongName = longFileName;				
			} 
		}
		
		/// <summary>
		/// Generates a unique id for the entire document that this file element
		/// belongs to.
		/// </summary>
		/// <param name="parentDirectory">The full path of the parent directory
		/// for the filename.</param>
		/// <param name="fileName">The name of the file to generate a unique
		/// id for. This does not include any path.</param>
		string GenerateUniqueId(string parentDirectory, string fileName)
		{
			string id = GenerateId(fileName);
			WixDocument document = (WixDocument)OwnerDocument;
			if (!document.FileIdExists(id)) {
				return id;
			}
			
			// Add the file's parent directory to the id.
			string parentDirectoryName = WixDirectoryElement.GetLastDirectoryName(parentDirectory);
			if (parentDirectoryName.Length > 0) {
				id = String.Concat(parentDirectoryName, ".", id);
				if (!document.FileIdExists(id)) {
					return id;
				}
				fileName = id;
			}
			
			// Add a number to the file name until we get a unique id.
			int count = 0;
			string idStart = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);
			do {
				++count;
				id = String.Concat(idStart, count, extension);
			} while (document.FileIdExists(id));
			
			return id;
		}
		
		/// <summary>
		/// Determines if the short filename already exists in the document.
		/// </summary>
		bool ShortFileNameExists(string fileName)
		{
			if (parentComponent == null) {
				return false;
			}
			WixDirectoryElement directoryElement = parentComponent.ParentNode as WixDirectoryElement;
			return directoryElement.ShortFileNameExists(fileName);
		}
	}
}
