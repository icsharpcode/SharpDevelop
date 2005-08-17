// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.FormDesigner.Services;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.FormDesigner
{
	public class FormDesignerSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		IMethod GetInitializeComponents(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if ((method.Name == "InitializeComponents" || method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		
		public static bool BaseClassIsFormOrControl(IClass c)
		{
			// Simple test for fully qualified name
			foreach (IReturnType baseType in c.BaseTypes) {
				if (baseType.FullyQualifiedName == "System.Windows.Forms.Form" || baseType.FullyQualifiedName == "System.Windows.Forms.UserControl") {
					return true;
				}
			}
			// Test for real base type (does not work while solution load thread is still running)
			IProjectContent pc = ProjectContentRegistry.WinForms;
			return c.IsTypeInInheritanceTree(pc.GetClass("System.Windows.Forms.Form")) || c.IsTypeInInheritanceTree(pc.GetClass("System.Windows.Forms.UserControl"));
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
				string fileExtension = String.Empty;
				string fileName      = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
				if (fileName == null)
					return false;
				
				fileExtension = Path.GetExtension(fileName).ToLower();
				
				switch (fileExtension) {
					case ".cs":
						ParseInformation info = ParserService.ParseFile(fileName, textAreaControlProvider.TextEditorControl.Document.TextContent, false, true);
						if (info != null) {
							ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
							foreach (IClass c in cu.Classes) {
								if (BaseClassIsFormOrControl(c)) {
									IMethod method = GetInitializeComponents(c);
									if (method == null) {
										continue;
									}
									Console.WriteLine("TRUE "+ info);
									
									return true;
								}
							}
						}
						break;
					case ".vb":
						info = ParserService.ParseFile(fileName, textAreaControlProvider.TextEditorControl.Document.TextContent, false, true);
						if (info != null) {
							ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
							foreach (IClass c in cu.Classes) {
								if (BaseClassIsFormOrControl(c)) {
									IMethod method = GetInitializeComponents(c);
									if (method == null) {
										continue;
									}
									return true;
								}
							}
						}
						break;
					case ".xfrm":
						return true;
				}
			}
			return false;
		}
		
		public ISecondaryViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			string fileExtension = String.Empty;
			string fileName      = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
			
			fileExtension = Path.GetExtension(fileName).ToLower();
			
			if (!FormKeyHandler.inserted) {
				FormKeyHandler.Insert();
			}
			IDesignerLoaderProvider loader;
			IDesignerGenerator generator;
			
			switch (fileExtension) {
				case ".cs":
					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguages.CSharp, ((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new CSharpDesignerGenerator();
					break;
				case ".vb":
					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguages.VBNet, ((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new VBNetDesignerGenerator();
					break;
				case ".xfrm":
					loader    = new XmlDesignerLoaderProvider(((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new XmlDesignerGenerator();
					break;
				default:
					throw new ApplicationException("Cannot create content for " + fileExtension);
			}
			return new ISecondaryViewContent[] { new FormDesignerViewContent(viewContent, loader, generator) };
		}
	}
}
