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
//
//namespace ICSharpCode.FormDesigner {
//	
//	public class MainMenuEditorControl : AbstractMenuEditorControl
//	{
//		public MainMenuEditorControl(IDesignerHost host, Menu menu) : base(host, menu)
//		{
//		}
//		
//		protected override void OnLostFocus(EventArgs e)
//		{
//			MenuEditorFocused = base.subMenuEditor != null && !subMenuEditor.iveTheFocus;
//			base.OnLostFocus(e);
//		}
//	}
//}
