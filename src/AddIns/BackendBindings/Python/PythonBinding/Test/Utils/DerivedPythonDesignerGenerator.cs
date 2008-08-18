// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Gives access to various protected methods
	/// of the PythonDesignerGenerator class for testing.
	/// </summary>
	public class DerivedPythonDesignerGenerator : PythonDesignerGenerator
	{
		CodeTypeDeclaration generatedFormClass;
		CodeMemberMethod generatedInitializeComponent;
		IMethod methodToReturnFromInitializeComponents;
		string fileNamePassedToParseFile;
		string textContentPassedToParseFile;
		ParseInformation parseInfoToReturnFromParseFile;
		
		public DerivedPythonDesignerGenerator()
		{
		}
				
		/// <summary>
		/// Gets the form class found during the MergeFormChanges method.
		/// </summary>
		public CodeTypeDeclaration GeneratedFormClass {
			get { return generatedFormClass; }
		}
		
		/// <summary>
		/// Get the InitializeComponents Method found during the
		/// MergeFormChanges call.
		/// </summary>
		public CodeMemberMethod GenerateInitializeComponentsMethod {
			get { return generatedInitializeComponent; }
		}
		
		public DomRegion CallGetBodyRegionInDocument(IMethod method)
		{
			return base.GetBodyRegionInDocument(method);
		}
						
		/// <summary>
		/// Gets the filename passed to the ParseFile method. This is called
		/// at the start of the GetInitializeComponents method to get the
		/// latest parse information.
		/// </summary>
		public string FileNamePassedToParseFileMethod {
			get { return fileNamePassedToParseFile; }
		}
		
		/// <summary>
		/// Gets the text content passed to the ParseFile method when 
		/// the GetInitializeComponents method is called.
		/// </summary>
		public string TextContentPassedToParseFileMethod {
			get { return textContentPassedToParseFile; }
		}
		
		
		/// <summary>
		/// Gets or sets the parse information that will be returned from the
		/// ParseFile method.
		/// </summary>
		public ParseInformation ParseInfoToReturnFromParseFileMethod {
			get { return parseInfoToReturnFromParseFile; }
			set { parseInfoToReturnFromParseFile = value; }
		}
		
		/// <summary>
		/// Gets the view content attached to the Python Designer Generator.
		/// </summary>
		public FormsDesignerViewContent GetViewContent()
		{
			return base.ViewContent;
		}
		
		public IMethod MethodToReturnFromInitializeComponents {
			get { return methodToReturnFromInitializeComponents; }
			set { methodToReturnFromInitializeComponents = value; }
		}

		/// <summary>
		/// Calls the PythonDesignerGenerator's CreateEventHandler method.
		/// </summary>
		public string CallCreateEventHandler(string eventMethodName, string body, string indentation)
		{
			return base.CreateEventHandler(eventMethodName, body, indentation);
		}
		
		protected override void MergeFormChanges(GeneratedInitializeComponentMethod generatedInitializeComponent)
		{
			this.generatedFormClass = generatedInitializeComponent.Type;
			this.generatedInitializeComponent = generatedInitializeComponent.Method;
			base.MergeFormChanges(generatedInitializeComponent);
		}
				
		protected override ParseInformation ParseFile(string fileName, string textContent)
		{
			fileNamePassedToParseFile = fileName;
			textContentPassedToParseFile = textContent;
			return parseInfoToReturnFromParseFile;
		}
	}
}
