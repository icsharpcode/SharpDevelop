// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Xml;

namespace ICSharpCode.NAnt
{
	/// <summary>
	/// Represents a NAnt Build File.
	/// </summary>
	public class NAntBuildFile
	{
		/// <summary>
		/// Standard NAnt build file's extension.
		/// </summary>
		public static readonly string BuildFileNameExtension = ".build";
		
		/// <summary>
		/// Standard NAnt include file's extension.
		/// </summary>
		public static readonly string IncludeFileNameExtension = ".include";
		
		static readonly string TargetElementName = "target";
		static readonly string ProjectElementName = "project";
		
		static readonly string NameAttributeName = "name";
		static readonly string DefaultAttributeName = "default";
		
		string directory = String.Empty;
		string fileName = String.Empty;
		string name = String.Empty;
		string defaultTargetName = String.Empty;
		NAntBuildFileError buildFileError;
		
		NAntBuildTargetCollection targets = new NAntBuildTargetCollection();
		NAntBuildTarget defaultTarget;
		
		enum ParseState
		{
			Nothing = 0,
			WaitingForProjectName = 1,
			WaitingForTargetName = 2
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="NAntBuildFile"/>
		/// class.
		/// </summary>
		/// <param name="fileName">The build filename.</param>
		public NAntBuildFile(string fileName)
		{
			this.directory = Path.GetDirectoryName(fileName);
			this.fileName = Path.GetFileName(fileName);
			
			ReadBuildFile(fileName);
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="NAntBuildFile"/>
		/// class.
		/// </summary>
		/// <param name="reader">The <see cref="TextReader"/> used to 
		/// feed the XML data into the <see cref="NAntBuildFile"/> 
		/// object.</param>
		public NAntBuildFile(TextReader reader)
		{
			ParseBuildFile(reader);
		}
		
		/// <summary>
		/// Gets or sets the filename without the path information.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
			
			set {
				fileName = value;
			}
		}
		
		/// <summary>
		/// Gets the build file's path information.
		/// </summary>
		public string Directory {
			get {
				return directory;
			}
		}
		
		/// <summary>
		/// Gets the project name.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// Checks the build file is a NAnt build file.
		/// </summary>
		public static bool IsBuildFile(string fileName)
		{
			bool isBuildFile = false;
			
			string extension = Path.GetExtension(fileName);
			
			if (String.Compare(extension, BuildFileNameExtension, true) == 0) {
				isBuildFile = true;
			} else if (String.Compare(extension, IncludeFileNameExtension, true) == 0) {
				isBuildFile = true;
			}
			
			return isBuildFile;
		}
		
		/// <summary>
		/// Gets the NAnt build targets.
		/// </summary>
		public NAntBuildTargetCollection Targets {
			get {
				return targets;
			}
		}
		
		/// <summary>
		/// Gets the default NAnt target.
		/// </summary>
		public NAntBuildTarget DefaultTarget {
			get {
				return defaultTarget;
			}
		}
		
		/// <summary>
		/// Gets whether there is an error with this build file.
		/// </summary>
		public bool HasError {
			get {
				return (buildFileError != null);
			}
		}
		
		/// <summary>
		/// Gets the error associated with the build file.
		/// </summary>
		public NAntBuildFileError Error {
			get {
				return buildFileError;
			}
		}
		
		/// <summary>
		/// Reads the NAnt build file and extracts target names.
		/// </summary>
		/// <param name="fileName">The name of the build file.</param>
		void ReadBuildFile(string fileName)
		{
			StreamReader reader = new StreamReader(fileName, true);
			ParseBuildFile(reader);
		}
		
		/// <summary>
		/// Gets the default target's name or returns an empty string.
		/// </summary>
		/// <param name="root">The root node of the build file.</param>
		/// <returns>The default target's name or an empty string if
		/// it does not exist.</returns>
		string GetDefaultTargetName(XmlElement root)
		{
			string defaultTargetName = String.Empty;
			
			XmlAttribute nameAttribute = root.Attributes["default"];
			
			if (nameAttribute != null) {
				defaultTargetName = nameAttribute.Value;
			}
			
			return defaultTargetName;
		}
		
		/// <summary>
		/// Gets the build file's project name or returns an empty string.
		/// </summary>
		/// <param name="root">The root node of the build file.</param>
		/// <returns>The project name or an empty string if
		/// it does not exist.</returns>
		string GetProjectName(XmlElement root)
		{
			string projectName = String.Empty;
			
			XmlAttribute nameAttribute = root.Attributes["name"];
			
			if (nameAttribute != null) {
				projectName = nameAttribute.Value;
			}
			
			return projectName;
		}	
		
		/// <summary>
		/// Tests whether <paramref name="name"/> matches the
		/// default target name.
		/// </summary>
		/// <param name="defaultTargetName">The default target
		/// name.</param>
		/// <param name="targetName">A target's name.</param>
		/// <returns><see langword="true"/> if the target name matches
		/// the default; otherwise <see langword="false"/>.</returns>
		bool IsDefaultTargetName(string defaultTargetName, string targetName)
		{
			bool isDefault = false;
			
			if (defaultTargetName.Length > 0) {
				if (String.Compare(defaultTargetName, targetName, true) == 0) {
					isDefault = true;
				}
			}
			
			return isDefault;
		}
		
		/// <summary>
		/// Parse the NAnt build file.
		/// </summary>
		/// <param name="textReader">A TextReader from which to read
		/// the build file.</param>
		void ParseBuildFile(TextReader textReader)
		{
			XmlTextReader xmlReader = new XmlTextReader(textReader);
			
			try
			{
				ParseState state = ParseState.WaitingForProjectName;
				
				while(xmlReader.Read())
				{
					if (state == ParseState.WaitingForProjectName) {
						if (IsProjectElement(xmlReader)) {
						    ParseProjectElement(xmlReader);
							state = ParseState.WaitingForTargetName;						    	
						}
					} else {
						if (IsTargetElement(xmlReader)) {
							ParseTargetElement(xmlReader);
						}
					}
				}
			} catch(XmlException ex) {
				buildFileError = new NAntBuildFileError(ex.Message, ex.LineNumber - 1, ex.LinePosition - 1);
			} finally {
				xmlReader.Close();
			}
		}
		
		/// <summary>
		/// Parses the current XmlTextReader node if it
		/// is the NAnt Project element.
		/// </summary>
		/// <param name="xmlReader">An XmlTextReader currently being
		/// read.</param>
		void ParseProjectElement(XmlTextReader xmlReader)
		{
			name = GetAttribute(xmlReader, NameAttributeName);
			defaultTargetName = GetAttribute(xmlReader, DefaultAttributeName);
		}
		
		/// <summary>
		/// Tests whether the current element is the project element.
		/// </summary>
		/// <param name="xmlReader"></param>
		/// <returns><see langword="true"/> if the current 
		/// element is the project element; 
		/// <see langword="false"/> otherwise</returns>
		bool IsProjectElement(XmlTextReader xmlReader)
		{
			bool isProjectElement = false;
			
			if (xmlReader.NodeType == XmlNodeType.Element) {
				if (xmlReader.Name == ProjectElementName) {
					isProjectElement = true;
				}
			}
			
			return isProjectElement;
		}
		
		/// <summary>
		/// Tests whether the current element is a target element.
		/// </summary>
		/// <param name="xmlReader">An xml text reader currently
		/// reading the build file xml.</param>
		/// <returns><see langword="true"/> if the current 
		/// element is a target element; 
		/// <see langword="false"/> otherwise</returns>
		bool IsTargetElement(XmlTextReader xmlReader)
		{
			bool isTargetElement = false;
			
			if (xmlReader.NodeType == XmlNodeType.Element) {
				if (xmlReader.Name == TargetElementName) {
					isTargetElement = true;
				}
			}
			
			return isTargetElement;
		}		
		
		/// <summary>
		/// Parses the current XmlTextReader node if it
		/// is the NAnt Target element.
		/// </summary>
		void ParseTargetElement(XmlTextReader xmlReader)
		{
			// Take off one for line/col since SharpDevelop is zero based.
			int line = xmlReader.LineNumber - 1;
			int col = xmlReader.LinePosition - 1;
			
			string targetName = GetAttribute(xmlReader, NameAttributeName);
			
			bool isDefaultTarget = IsDefaultTargetName(defaultTargetName, targetName);
			
			NAntBuildTarget target = 
				new NAntBuildTarget(targetName, isDefaultTarget, line, col);
			    
			targets.Add(target);
			
			if (isDefaultTarget) {
				defaultTarget = target;
			}
		}
		
		/// <summary>
		/// Gets the named attribute's value.
		/// </summary>
		/// <returns>The attribute's value or an empty string if
		/// it was not found.
		/// </returns>
		string GetAttribute(XmlTextReader xmlReader, string name)
		{
			string attributeValue = String.Empty;
			
			if (xmlReader.MoveToAttribute(name)) {
				if (xmlReader.Value != null) {
					attributeValue = xmlReader.Value;
				}
			}
			
			return attributeValue;
		}
	}
}
