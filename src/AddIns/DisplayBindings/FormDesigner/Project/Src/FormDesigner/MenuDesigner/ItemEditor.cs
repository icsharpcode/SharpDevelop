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
//namespace ICSharpCode.FormDesigner
//{
//	public class ItemEditor : TextBox
//	{
//		MenuItem item = null;
//		AbstractMenuEditorControl motherEditor;
//		bool     isEscape = false;
//		
//		public ItemEditor(AbstractMenuEditorControl motherEditor)
//		{
//			this.motherEditor = motherEditor;
//			base.BorderStyle = BorderStyle.None;
//		}
//		
//		protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
//		{
//			switch (keyData) {
//				case Keys.Escape:
//					isEscape = true;
//					CloseItemEditor();
//					return true;
//				case Keys.Return:
//					try {
//						item.Text = Text;
//					} catch (Exception e) {
//						Console.WriteLine(e);
//					}
//					CloseItemEditor();
//					return true;
//			}
//			return base.ProcessDialogKey(keyData);
//		}
//		
//		protected override void OnLostFocus(System.EventArgs e)
//		{
//			if (!isEscape) {
//				try {
//					item.Text = Text;
//				} catch (Exception ex) {
//					Console.WriteLine(ex);
//				}
//				CloseItemEditor();
//			}
//		}
//		
//		void CloseItemEditor()
//		{
//			base.Dispose();
//			motherEditor.SetSize(null, null);
//			motherEditor.Invalidate();
//			motherEditor.Update();
//		}
//		
//		public void SetItem(MenuItem item)
//		{
//			this.item = item;
//			this.Text = item.Text;
//			AbstractMenuEditorControl.MenuEditorFocused = true;
//		}
//		
//		
//	}
//}
