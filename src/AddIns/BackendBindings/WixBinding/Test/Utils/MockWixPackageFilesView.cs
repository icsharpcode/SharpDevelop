// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.WixBinding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;

namespace WixBinding.Tests.Utils
{
	public class MockWixPackageFilesView : IWixPackageFilesView
	{
		bool noSourceFileFoundMessageDisplayed;
		bool sourceFilesContainErrorsMessageDisplayed;
		bool noDifferencesFoundMessageDisplayed;
		bool noRootDirectoryFoundMessageDisplayed;
		string projectName;
		WixDirectoryElement rootDirectory;
		bool selectedItemAccessed;
		XmlElement selectedItem;
		XmlElement elementRemoved;
		bool isDirty;
		WixXmlAttributeCollection attributes = new WixXmlAttributeCollection();
		StringCollection allowedElements = new StringCollection();
		List<XmlElement> elementsAdded = new List<XmlElement>();
		bool attributesChangedCalled;
		List<XmlElement> directoriesAdded = new List<XmlElement>();
		bool clearDirectoriesCalled;
		WixPackageFilesDiffResult[] diffResults;
		bool contextMenuEnabled;
		
		public MockWixPackageFilesView()
		{
		}
		
		public void ShowNoRootDirectoryFoundMessage()
		{
			noRootDirectoryFoundMessageDisplayed = true;
		}
		
		public void ShowNoSourceFileFoundMessage(string projectName)
		{
			noSourceFileFoundMessageDisplayed = true;
			this.projectName = projectName;
		}
		
		public void ShowSourceFilesContainErrorsMessage()
		{
			sourceFilesContainErrorsMessageDisplayed = true;
		}
		
		public void ShowNoDifferenceFoundMessage()
		{
			noDifferencesFoundMessageDisplayed = true;
		}
		
		public WixDirectoryElement RootDirectory {
			get { return rootDirectory; }
			set { rootDirectory = value; }
		}
		
		public void AddDirectories(WixDirectoryElement[] directories)
		{
			foreach (WixDirectoryElement directory in directories) {
				directoriesAdded.Add(directory);
			}
		}
		
		public List<XmlElement> DirectoriesAdded {
			get { return directoriesAdded; }
		}
		
		public XmlElement SelectedElement {
			get {
				selectedItemAccessed = true;
				return selectedItem;
			}
			set { selectedItem = value; }
		}
		
		public void RemoveElement(XmlElement element)
		{
			elementRemoved = element;
		}
		
		public WixXmlAttributeCollection Attributes {
			get { return attributes; }
		}
		
		public bool IsDirty {
			get { return isDirty; }
			set { isDirty = value; }
		}
		
		public StringCollection AllowedChildElements {
			get { return allowedElements; }
		}
		
		public void AddElement(XmlElement element)
		{
			elementsAdded.Add(element);
		}
		
		public void AttributesChanged()
		{
			attributesChangedCalled = true;
		}
		
		public void ClearDirectories()
		{
			clearDirectoriesCalled = true;
		}
		
		public void ShowDiffResults(WixPackageFilesDiffResult[] diffResults)
		{
			this.diffResults = diffResults;
		}
		
		/// <summary>
		/// Gets whether the No source file found error message is displayed in the
		/// view.
		/// </summary>
		public bool IsNoSourceFileFoundMessageDisplayed {
			get { return noSourceFileFoundMessageDisplayed; }
		}
		
		/// <summary>
		/// Gets whether the "All WiX files contains errors" message is displayed in the
		/// view.
		/// </summary>
		public bool IsSourceFilesContainErrorsMessageDisplayed {
			get { return sourceFilesContainErrorsMessageDisplayed; }
		}
		
		/// <summary>
		/// Gets the project name that was passed to ShowNoSourceFileFoundMessage.
		/// </summary>
		public string NoSourceFileFoundProjectName {
			get { return projectName; }
		}
		
		/// <summary>
		/// Returns whether the view's SelectedItem was accessed.
		/// </summary>
		public bool SelectedElementAccessed {
			get { return selectedItemAccessed; }
		}
		
		public XmlElement ElementRemoved {
			get { return elementRemoved; }
		}
		
		public XmlElement[] ElementsAdded {
			get { return elementsAdded.ToArray(); }
		}
		
		public bool IsAttributesChangedCalled {
			get { return attributesChangedCalled; }
		}
		
		public bool IsClearDirectoriesCalled {
			get { return clearDirectoriesCalled; }
		}
		
		public bool IsNoDifferencesFoundMessageDisplayed {
			get { return noDifferencesFoundMessageDisplayed; }
		}

		public bool IsNoRootDirectoryFoundMessageDisplayed {
			get { return noRootDirectoryFoundMessageDisplayed; }
		}
		
		public WixPackageFilesDiffResult[] DiffResults {
			get { return diffResults; }
		}
		
		public bool ContextMenuEnabled {
			get { return contextMenuEnabled; }
			set { contextMenuEnabled = value; }
		}
	}
}
