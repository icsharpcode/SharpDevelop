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
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.SharpDevelop.Internal.Undo;
//using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
//
//using ICSharpCode.Core;
//
//using ICSharpCode.FormDesigner.Services;
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
//	public class ComponentAddedUndoAction : ICSharpCode.SharpDevelop.Internal.Undo.IUndoableOperation
//	{
//		IDesignerHost host;
//		
//		Type   componentType;
//		string componentName;
//		
//		public ComponentAddedUndoAction(IDesignerHost host, ComponentEventArgs cea)
//		{
//			this.host     = host;
//			componentName = cea.Component.Site.Name;
//			componentType = cea.Component.GetType();
//		}
//		
//		public void Undo()
//		{
//			host.DestroyComponent(host.Container.Components[componentName]);
//		}
//		
//		public void Redo()
//		{
//			host.CreateComponent(componentType, componentName);
//		}
//	}
//}
