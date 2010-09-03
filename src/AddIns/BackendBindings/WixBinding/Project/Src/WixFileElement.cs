// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Represents a Wix File element in a Wix XML document.
	/// </summary>
	public class WixFileElement : WixElementBase
	{		
		public const string FileElementName = "File";
		
		WixComponentElement parentComponent;

		public WixFileElement(WixDocument document)
			: base(FileElementName, document)
		{
		}
		
		public WixFileElement(WixDocument document, string fileName) 
			: this(document)
		{
			Init(fileName);
		}
		
		public WixFileElement(WixComponentElement component, string fileName) 
			: this((WixDocument)component.OwnerDocument)
		{
			parentComponent = component;
			Init(fileName);
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
			Id = GenerateUniqueId(fileName);
			FileName = Path.GetFileName(fileName);
			Source = OwnerWixDocument.GetRelativePath(fileName);;
		}
		
		string GenerateUniqueId(string fileName)
		{
			string fileNameWithoutPath = Path.GetFileName(fileName);
			return GenerateUniqueId(Path.GetDirectoryName(fileName), fileNameWithoutPath);
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
			if (!OwnerWixDocument.FileIdExists(id)) {
				return id;
			}
			
			id = GenerateIdFromParentDirectory(parentDirectory, id);
			if (!OwnerWixDocument.FileIdExists(id)) {
				return id;
			}
			
			return GenerateUniqueIdByAppendingNumberToFileNameId(id);
		}
		
		string GenerateIdFromParentDirectory(string parentDirectory, string fileNameId)
		{
			string id = GenerateIdFromParentDirectory(parentDirectory);
			if (!String.IsNullOrEmpty(id)) {
				return String.Concat(id, ".", fileNameId);
			}
			return fileNameId;
		}
		
		string GenerateIdFromParentDirectory(string parentDirectory)
		{
			string parentDirectoryName = WixDirectoryElement.GetLastFolderInDirectoryName(parentDirectory);
			if (parentDirectoryName.Length > 0) {
				return WixFileElement.GenerateId(parentDirectoryName);
			}
			return String.Empty;
		}
		
		string GenerateUniqueIdByAppendingNumberToFileNameId(string fileNameId)
		{
			string id = String.Empty;
			int count = 0;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameId);
			string extension = Path.GetExtension(fileNameId);
			do {
				++count;
				id = String.Concat(fileNameWithoutExtension, count, extension);
			} while (OwnerWixDocument.FileIdExists(id));
			
			return id;
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
		/// Returns a valid id character for the given position. If the
		/// character is an invalid character it is replaced with an underscore.
		/// </summary>
		static char GetIdCharacter(char ch, int index)
		{
			if (IsValidIdCharacter(ch)) {
				// First char must be a letter or underscore.
				if (index == 0) {
					if (IsLetterOrUnderscore(ch)) {
						return ch;
					}
				} else {
					return ch;
				}
			} 
			return '_';
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
			return ((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z'));
		}
		
		/// <summary>
		/// Gets the filename where the resource being added to the setup 
		/// package can be found. Typically this is relative to the Wix document the
		/// File element is a part of.
		/// </summary>
		public string Source {
			get { return GetAttribute("Source"); }
			set { SetAttribute("Source", value); }
		}
		
		/// <summary>
		/// Gets whether the file is the KeyPath for its parent component.
		/// </summary>
		public string KeyPath {
			get { return GetAttribute("KeyPath"); }
			set { SetAttribute("KeyPath", value); }
		}
		
		/// <summary>
		/// Gets the name of the file without any path information.
		/// This is the name that will be used when installing the file.
		/// </summary>
		public string FileName {
			get { return GetAttribute("Name"); }
			set { SetAttribute("Name", value); }
		}
		
		/// <summary>
		/// Gets the full path to the file. If the parent WixDocument
		/// has no filename then the relative path as stored in the 
		/// wix document is returned.
		/// </summary>
		public string GetSourceFullPath()
		{
			if (!String.IsNullOrEmpty(OwnerWixDocument.FileName)) {
				string directory = Path.GetDirectoryName(OwnerWixDocument.FileName);
				return Path.GetFullPath(Path.Combine(directory, Source));
			}
			return Source;
		}
	}
}
