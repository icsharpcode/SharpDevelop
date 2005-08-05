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
		
		static Hashtable oldTypes = new Hashtable();
		
		public static bool BaseClassIsFormOrControl(IClass c)
		{
			if (c == null || oldTypes.Contains(c.FullyQualifiedName)) {
				oldTypes.Clear();
				return false;
			}
			oldTypes.Add(c.FullyQualifiedName, null);
			
			foreach (string baseType in c.BaseTypes) {
				IClass type = ParserService.CurrentProjectContent.SearchType(baseType, c, c.Region != null ? c.Region.BeginLine : 0, c.Region != null ? c.Region.BeginColumn : 0);
				string typeName = type != null ? type.FullyQualifiedName : baseType;
				if (typeName == "System.Windows.Forms.Form" ||
				    typeName == "System.Windows.Forms.UserControl" ||
				    BaseClassIsFormOrControl(type)) {
					oldTypes.Clear();
					return true;
				}
			}
			oldTypes.Clear();
			return false;
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
			DesignerLoader     loader    = new NRefactoryDesignerLoader(SupportedLanguages.CSharp, ((ITextEditorControlProvider)viewContent).TextEditorControl);
			IDesignerGenerator generator = new CSharpDesignerGenerator();
			
			switch (fileExtension) {
				case ".cs":
					loader    = new NRefactoryDesignerLoader(SupportedLanguages.CSharp, ((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new CSharpDesignerGenerator();
					break;
				case ".vb":
					loader    = new NRefactoryDesignerLoader(SupportedLanguages.VBNet, ((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new VBNetDesignerGenerator();
					break;
				case ".xfrm":
					loader    = new XmlDesignerLoader(((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new XmlDesignerGenerator();
					break;
			}
			return new ISecondaryViewContent[] { new FormDesignerViewContent(viewContent, loader, generator) };
		}
	}
}
