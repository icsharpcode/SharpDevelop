// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Interface for the UI that displays all the files defined for a setup package.
	/// </summary>
	public interface IWixPackageFilesView
	{
		bool ContextMenuEnabled { get; set; }
		
		/// <summary>
		/// Displays the message in the view that no WiX source file is contained
		/// in the project.
		/// </summary>
		/// <param name="projectName">The name of the project that no WiX
		/// source file could be found.</param>
		void ShowNoSourceFileFoundMessage(string projectName);
		
		/// <summary>
		/// Displays the message that no files could be found and some of the WiX source 
		/// files could not be read because they contain errors.
		/// </summary>
		void ShowSourceFilesContainErrorsMessage();
		
		/// <summary>
		/// Displays a message indicating that no //w:Product/w:Directory[@Id="TARGETDIR"] or 
		/// //w:DirectoryRef[@Id="TARGETDIR"] element could be found.
		/// </summary>
		void ShowNoRootDirectoryFoundMessage();
				
		/// <summary>
		/// Adds the directories that will be displayed. Each directory may contain its
		/// own directories.
		/// </summary>
		void AddDirectories(WixDirectoryElement[] directories);
		
		/// <summary>
		/// Removes all directories currently being displayed.
		/// </summary>
		void ClearDirectories();
		
		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		XmlElement SelectedElement {get; set;}
		
		/// <summary>
		/// Gets the attributes for the selected item.
		/// </summary>
		WixXmlAttributeCollection Attributes {get;}
		
		/// <summary>
		/// Called when the attributes have been changed for the selected item.
		/// </summary>
		void AttributesChanged();
		
		/// <summary>
		/// Gets or sets whether the view needs saving.
		/// </summary>
		bool IsDirty {get; set;}
		
		/// <summary>
		/// Removes the element from the view.
		/// </summary>
		void RemoveElement(XmlElement element);
		
		/// <summary>
		/// Adds the element to the view. The element should be added as a child 
		/// of the currently selected element.
		/// </summary>
		void AddElement(XmlElement element);
		
		/// <summary>
		/// Gets the element names that can be added as children to the selected element.
		/// </summary>
		StringCollection AllowedChildElements {get;}
		
		/// <summary>
		/// Displays the message that no difference was found between
		/// the files in the Wix document and those on the file system.
		/// </summary>
		void ShowNoDifferenceFoundMessage();
		
		/// <summary>
		/// Displays the diff results.
		/// </summary>
		void ShowDiffResults(WixPackageFilesDiffResult[] diffResults);
	}
}
