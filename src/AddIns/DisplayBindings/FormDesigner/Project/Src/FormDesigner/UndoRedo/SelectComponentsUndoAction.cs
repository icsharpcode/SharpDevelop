//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//using System.Collections;
//using System.Drawing;
//using System.Drawing.Design;
//using System.Reflection;
//using System.Windows.Forms;
//using System.Drawing.Printing;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Xml;
//using System.ComponentModel.Design.Serialization;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.SharpDevelop.Internal.Undo;
//using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
//
//
//using ICSharpCode.FormDesigner.Services;
//using ICSharpCode.Core;
//using ICSharpCode.TextEditor;
//
//using System.CodeDom;
//using System.CodeDom.Compiler;
//
//using Microsoft.CSharp;
//using Microsoft.VisualBasic;
//
//using ICSharpCode.SharpDevelop.Gui.ErrorDialogs;
//using ICSharpCode.FormDesigner.Gui;
//
//using ICSharpCode.SharpDevelop.Gui.OptionPanels;
//
//namespace ICSharpCode.FormDesigner {
//	
//	public class SelectComponentsUndoAction : ICSharpCode.SharpDevelop.Internal.Undo.IUndoableOperation
//	{
//		IDesignerHost host;
//		
//		ArrayList oldComponentNames;
//		ArrayList newComponentNames;
//		
//		public SelectComponentsUndoAction(IDesignerHost host, ArrayList oldComponentNames)
//		{
//			this.host     = host;
//			this.oldComponentNames = oldComponentNames;
//		}
//		
//		public void SetNewSelection(ArrayList newComponentNames)
//		{
//			this.newComponentNames = newComponentNames;
//		}
//		
//		public void Undo()
//		{
//			UndoHandler.SetSelectedComponentsPerName(host, oldComponentNames);
//		}
//		
//		public void Redo()
//		{
//			UndoHandler.SetSelectedComponentsPerName(host, newComponentNames);
//		}
//	}
//}
