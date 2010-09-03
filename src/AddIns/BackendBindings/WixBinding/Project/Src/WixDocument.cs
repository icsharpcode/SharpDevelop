// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A Wix document (.wxs or .wxi).
	/// </summary>
	public class WixDocument : XmlDocument, IWixPropertyValueProvider
	{	
		WixNamespaceManager namespaceManager;
		IFileLoader fileLoader;
		WixProject project;
		string fileName = String.Empty;
		
		public WixDocument() 
			: this((WixProject)null)
		{
		}
		
		public WixDocument(WixProject project) 
			: this(project, new DefaultFileLoader())
		{
		}
		
		/// <summary>
		/// Creates a new instance of the WixDocument class but overrides the 
		/// default file loading functionality of the WixDocument. 
		/// </summary>
		public WixDocument(WixProject project, IFileLoader fileLoader)
		{
			this.project = project;
			this.fileLoader = fileLoader;
			
			if (fileLoader == null) {
				throw new ArgumentNullException("fileLoader");
			}
			
			namespaceManager = new WixNamespaceManager(NameTable);			
		}
		
		public WixProject Project {
			get { return project; }
		}
		
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		/// <summary>
		/// Gets a WixDialog object for the specified dialog id.
		/// </summary>
		public WixDialog CreateWixDialog(string id, ITextFileReader reader)
		{
			XmlElement dialogElement = GetDialogElement(id);
			if (dialogElement != null) {
				return new WixDialog(this, dialogElement, new WixBinaries(this, reader));
			}
			return null;
		}
		
		XmlElement GetDialogElement(string id)
		{
			string xpath = GetXPath("//w:Dialog[@Id='{0}']", id);
			return (XmlElement)SelectSingleElement(xpath);
		}
		
		string GetXPath(string xpathFormat, string arg)
		{
			return String.Format(xpathFormat, XmlEncode(arg));
		}
		
		string GetXPath(string xpathFormat, string arg1, string arg2)
		{
			return String.Format(xpathFormat, XmlEncode(arg1), XmlEncode(arg2));
		}
		
		string XmlEncode(string item)
		{
			char quoteChar = '\'';
			return XmlEncoder.Encode(item, quoteChar);
		}
		
		XmlElement SelectSingleElement(string xpath)
		{
			return (XmlElement)SelectSingleNode(xpath, namespaceManager);
		}
		
		public Bitmap LoadBitmapWithId(string id)
		{
			string bitmapFileName = GetBinaryFileName(id);
			return LoadBitmapWithFileName(bitmapFileName);
		}
		
		public string GetBinaryFileName(string id)
		{
			string xpath = GetXPath("//w:Binary[@Id='{0}']", id);
			WixBinaryElement binaryElement = (WixBinaryElement)SelectSingleElement(xpath);
			if (binaryElement != null) {
				return binaryElement.GetFileName();
			}
			return null;
		}
		
		public Bitmap LoadBitmapWithFileName(string fileName)
		{
			if (fileName != null) {
				return fileLoader.LoadBitmap(fileName);
			}
			return null;
		}
		
		/// <summary>
		/// Gets a property value defined in the Wix document.
		/// </summary>
		public string GetProperty(string name)
		{
			string xpath = GetXPath("//w:Property[@Id='{0}']", name);
			XmlElement textStyleElement = SelectSingleElement(xpath);
			if (textStyleElement != null) {
				return textStyleElement.InnerText;
			}
			return String.Empty;
		}
		
		string IWixPropertyValueProvider.GetValue(string name)
		{
			if (project != null) {
				return project.GetPreprocessorVariableValue(name);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the top SOURCEDIR directory.
		/// </summary>
		public WixDirectoryElement GetRootDirectory()
		{
			string xpath = GetXPath("//w:Product/w:Directory[@Id='{0}']", WixDirectoryElement.RootDirectoryId);
			return (WixDirectoryElement)SelectSingleElement(xpath);
		}
		
		/// <summary>
		/// Gets a reference to the root directory.
		/// </summary>
		public WixDirectoryRefElement GetRootDirectoryRef()
		{
			string xpath = GetXPath("//w:DirectoryRef[@Id='{0}']", WixDirectoryElement.RootDirectoryId);
			return (WixDirectoryRefElement)SelectSingleElement(xpath);
		}
		
		public bool HasProduct {
			get { return GetProduct() != null; }
		}
		
		public string GetWixNamespacePrefix()
		{
			XmlElement documentElement = DocumentElement;
			if (documentElement != null) {
				return documentElement.GetPrefixOfNamespace(WixNamespaceManager.Namespace);
			}
			return String.Empty;
		}
		
		public WixDirectoryElement AddRootDirectory()
		{
			XmlElement productElement = GetProduct();
			if (productElement == null) {
				productElement = AddProduct();
			}
			return AddRootDirectoryToProduct(productElement);
		}
		
		XmlElement AddProduct()
		{
			XmlElement productElement = CreateWixElement("Product");
			DocumentElement.AppendChild(productElement);
			return productElement;
		}
		
		WixDirectoryElement AddRootDirectoryToProduct(XmlElement parentProductElement)
		{
			WixDirectoryElement rootDirectory = WixDirectoryElement.CreateRootDirectory(this);
			return (WixDirectoryElement)parentProductElement.AppendChild(rootDirectory);
		}
		
		public XmlElement CreateWixElement(string name)
		{
			return CreateElement(GetWixNamespacePrefix(), name, WixNamespaceManager.Namespace);
		}
		
		/// <summary>
		/// Creates custom Wix elements for certain elements such as Directory and File.
		/// </summary>
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			if (namespaceURI == WixNamespaceManager.Namespace) {
				switch (localName) {
					case WixDirectoryElement.DirectoryElementName:
						return new WixDirectoryElement(this);
					case WixComponentElement.ComponentElementName:
						return new WixComponentElement(this);
					case WixFileElement.FileElementName:
						return new WixFileElement(this);
					case WixDirectoryRefElement.DirectoryRefElementName:
						return new WixDirectoryRefElement(this);
					case WixBinaryElement.BinaryElementName:
						return new WixBinaryElement(this);
					case WixDialogElement.DialogElementName:
						return new WixDialogElement(this);
				}
			}
			return base.CreateElement(prefix, localName, namespaceURI);
		}
		
		/// <summary>
		/// Checks to see if a File element exists with the specified id in this
		/// document.
		/// </summary>
		public bool FileIdExists(string id)
		{
			return ElementIdExists(WixFileElement.FileElementName, id);
		}
		
		bool ElementIdExists(string elementName, string id)
		{
			string xpath = GetXPath("//w:{0}[@Id='{1}']", elementName, id);
			XmlNodeList nodes = SelectNodes(xpath, new WixNamespaceManager(NameTable));
			return nodes.Count > 0;
		}
		
		/// <summary>
		/// Checks to see if a Component element exists with the specified id in this
		/// document.
		/// </summary>
		public bool ComponentIdExists(string id)
		{
			return ElementIdExists(WixComponentElement.ComponentElementName, id);
		}
		
		public bool DirectoryIdExists(string id)
		{
			return ElementIdExists(WixDirectoryElement.DirectoryElementName, id);
		}
		
		/// <summary>
		/// Returns the full path based on the location of 
		/// this WixDocument.
		/// </summary>
		public string GetFullPath(string relativePath)
		{
			if (!String.IsNullOrEmpty(fileName)) {
				string basePath = Path.GetDirectoryName(fileName);
				return FileUtility.GetAbsolutePath(basePath, relativePath);
			}
			return relativePath;
		}
		
		/// <summary>
		/// Returns the relative path based on the location of 
		/// this WixDocument.
		/// </summary>
		public string GetRelativePath(string fullPath)
		{
			if (!String.IsNullOrEmpty(fileName)) {
				string basePath = Path.GetDirectoryName(fileName);
				return FileUtility.GetRelativePath(basePath, fullPath);
			}
			return fullPath;
		}
		
		public XmlElement GetProduct()
		{
			return SelectSingleElement("w:Wix/w:Product");
		}
		
		public WixBinaryElement[] GetBinaries()
		{
			List<WixBinaryElement> binaries = new List<WixBinaryElement>();
			foreach (WixBinaryElement element in SelectNodes("//w:Binary", namespaceManager)) {
				binaries.Add(element);
			}
			return binaries.ToArray();
		}
	}
}
