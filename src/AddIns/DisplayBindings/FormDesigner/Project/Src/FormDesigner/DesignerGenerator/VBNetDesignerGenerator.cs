// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 230 $</version>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core;
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
	public class VBNetDesignerGenerator : IDesignerGenerator
	{
		IClass  c;
		IMethod initializeComponents;
		
		FormDesignerViewContent viewContent;
		bool failedDesignerInitialize = false;
		
		public void Attach(FormDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
			IComponentChangeService componentChangeService = (IComponentChangeService)viewContent.DesignSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded    += new ComponentEventHandler(ComponentAdded);
			componentChangeService.ComponentRename   += new ComponentRenameEventHandler(ComponentRenamed);
			componentChangeService.ComponentRemoving += new ComponentEventHandler(ComponentRemoved);
		}
		
		public void Detach()
		{
			IComponentChangeService componentChangeService = (IComponentChangeService)viewContent.DesignSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded    -= new ComponentEventHandler(ComponentAdded);
			componentChangeService.ComponentRename   -= new ComponentRenameEventHandler(ComponentRenamed);
			componentChangeService.ComponentRemoving -= new ComponentEventHandler(ComponentRemoved);
			this.viewContent = null;
		}
		
		void ComponentRemoved(object sender, ComponentEventArgs e)
		{
			try {
				Reparse(viewContent.Document.TextContent);
				foreach (IField field in c.Fields) {
					if (field.Name == e.Component.Site.Name) {
						int startOffset = viewContent.Document.PositionToOffset(new Point(0, field.Region.BeginLine - 1));
						int endOffset   = viewContent.Document.PositionToOffset(new Point(0, field.Region.EndLine));
						viewContent.Document.Remove(startOffset, endOffset - startOffset);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void ComponentAdded(object sender, ComponentEventArgs e)
		{
			try {
				Reparse(viewContent.Document.TextContent);
				int endOffset = viewContent.Document.PositionToOffset(new Point(0, initializeComponents.BodyRegion.EndLine));
				viewContent.Document.Insert(endOffset, "\tPrivate " + e.Component.Site.Name + " As " + e.Component.GetType() + Environment.NewLine);
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void ComponentRenamed(object sender, ComponentRenameEventArgs e)
		{
			Reparse(viewContent.Document.TextContent);
			foreach (IField field in c.Fields) {
				if (field.Name == e.OldName) {
					int startOffset = viewContent.Document.PositionToOffset(new Point(0, field.Region.BeginLine - 1));
					int endOffset   = viewContent.Document.PositionToOffset(new Point(0, field.Region.EndLine));
					viewContent.Document.Replace(startOffset, endOffset - startOffset, "\tPrivate " + e.NewName + " As " + e.Component.GetType() + Environment.NewLine);
				}
			}
		}
		
		public void MergeFormChanges()
		{
			// generate file and get initialize components string
			StringWriter writer = new StringWriter();
			new CodeDOMGenerator(viewContent.Host, new Microsoft.VisualBasic.VBCodeProvider()).ConvertContentDefinition(writer);
			string statements = writer.ToString();
			
			Reparse(viewContent.Document.TextContent);
			
			int startOffset = viewContent.Document.PositionToOffset(new Point(0, initializeComponents.BodyRegion.BeginLine + 1));
			int endOffset   = viewContent.Document.PositionToOffset(new Point(0, initializeComponents.BodyRegion.EndLine - 1));
			
			viewContent.Document.Replace(startOffset, endOffset - startOffset, statements);
		}
		
		protected void Reparse(string content)
		{
			// get new initialize components
			ParseInformation info = ParserService.ParseFile(viewContent.TextEditorControl.FileName, content, false, true);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (FormDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					initializeComponents = GetInitializeComponents(c);
					if (initializeComponents != null) {
						this.c = c;
						break;
					}
				}
			}
		}
		
		IMethod GetInitializeComponents(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if ((method.Name == "InitializeComponents" || method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		
		protected static string GenerateParams(EventDescriptor edesc, bool paramNames)
		{
			System.Type type =  edesc.EventType;
			MethodInfo mInfo = type.GetMethod("Invoke");
			string param = "";
			IAmbience csa = null;
			try {
				csa = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/VBNet", null);
			} catch (TreePathNotFoundException) {
				LoggingService.Warn("VB ambience not found");
			}
			
			for (int i = 0; i < mInfo.GetParameters().Length; ++i)  {
				ParameterInfo pInfo  = mInfo.GetParameters()[i];
				
				string typeStr = pInfo.ParameterType.ToString();
				if (csa != null) {
					typeStr = csa.GetIntrinsicTypeName(typeStr);
				}
				param += typeStr;
				if (paramNames == true) {
					param += " ";
					param += pInfo.Name;
				}
				if (i + 1 < mInfo.GetParameters().Length) {
					param += ", ";
				}
			}
			return param;
		}
		
		/// <summary>
		/// If found return true and int as position
		/// </summary>
		/// <param name="component"></param>
		/// <param name="edesc"></param>
		/// <returns></returns>
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out int position)
		{
			if (this.failedDesignerInitialize) {
				position = 0;
				return false;
			}

			Reparse(viewContent.Document.TextContent);
			
			foreach (IMethod method in c.Methods) {
				if (method.Name == eventMethodName) {
					position = method.Region.BeginLine + 1;
					return true;
				}
			}
			MergeFormChanges();
			Reparse(viewContent.Document.TextContent);
			
			position = c.Region.EndLine + 1;
			
			int offset = viewContent.Document.GetLineSegment(c.Region.EndLine - 1).Offset;
			
			string param = GenerateParams(edesc, true);
			
			string text = "Sub " + eventMethodName + "(" + param + ")\n" +
				body +
				"\nEnd Sub\n\n";
			viewContent.Document.Insert(offset, text);
			viewContent.Document.FormattingStrategy.IndentLines(viewContent.TextEditorControl.ActiveTextAreaControl.TextArea, c.Region.EndLine - 1, c.Region.EndLine + 3);
			
			return false;
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			Reparse(viewContent.Document.TextContent);
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == methodInfo.GetParameters().Length) {
					bool found = true;
					for (int i = 0; i < methodInfo.GetParameters().Length; ++i) {
						ParameterInfo pInfo = methodInfo.GetParameters()[i];
						IParameter p = method.Parameters[i];
						if (p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString()) {
							found = false;
							break;
						}
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			
			return compatibleMethods;
		}
		
		public ICollection GetCompatibleMethods(EventInfo edesc)
		{
			Reparse(viewContent.Document.TextContent);
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.GetAddMethod();
			ParameterInfo pInfo = methodInfo.GetParameters()[0];
			string eventName = pInfo.ParameterType.ToString().Replace("EventHandler", "EventArgs");
			
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == 2) {
					bool found = true;
					
					IParameter p = method.Parameters[1];
					if (p.ReturnType.FullyQualifiedName != eventName) {
						found = false;
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			return compatibleMethods;
		}
	}
}
