//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.Diagnostics;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Windows.Forms;
//using System.Windows.Forms.Design;
//
//using System.Drawing.Design;
//using ICSharpCode.FormDesigner;
//
//namespace Microsoft.VisualStudio.Windows.Forms {
//	
//	public class MenuDesigner : ComponentDesigner
//	{
//		DesignerVerbCollection designerVerbCollection = new DesignerVerbCollection();
//		
//		public override void DoDefaultAction()
//		{
//			Console.WriteLine("MenuDesigner.DoDefaultAction");
//		}
//		
//		public override void Initialize(IComponent component)
//		{
//			base.Initialize(component);
//		}
//		
//		public override ICollection AssociatedComponents { 
//			get {
//				Menu menu = base.Component as Menu;
//				if (menu != null) {
//					return menu.MenuItems;
//				}
//				return null;
//			}
//		}
//		
//		public override DesignerVerbCollection Verbs {
//			get {
//				return designerVerbCollection;
//			}
//		}
//	}
//}
