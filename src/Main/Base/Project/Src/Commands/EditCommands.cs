// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class Undo : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IUndoHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler;
				if (editable != null) {
					return editable.EnableUndo;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			IUndoHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler;
			if (editable != null) {
				editable.Undo();
			}
		}
	}
	
	public class Redo : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IUndoHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler;
				if (editable != null) {
					return editable.EnableRedo;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			IUndoHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler;
			if (editable != null) {
				editable.Redo();
			}
		}
	}

	public class Cut : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					return editable.EnableCut;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			if (IsEnabled) {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					editable.Cut();
				}
			}
		}
	}
	
	public class Copy : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					return editable.EnableCopy;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			if (IsEnabled) {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					editable.Copy();
				}
			}
		}
	}
	
	public class Paste : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					return editable.EnablePaste;
				}
				return false;
			}
		}
		public override void Run()
		{
			if (IsEnabled) {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					editable.Paste();
				}
			}
		}
	}
	
	public class Delete : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					return editable.EnableDelete;
				}
				return false;
			}
		}
		public override void Run()
		{
			IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
			if (editable != null) {
				editable.Delete();
			}
		}
	}
	
	public class SelectAll : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					return editable.EnableSelectAll;
				}
				return false;
			}
		}
		public override void Run()
		{
			if (IsEnabled) {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable != null) {
					editable.SelectAll();
				}
			}
		}
	}

	public class WordCount : AbstractMenuCommand
	{
		public override void Run()
		{
			using (WordCountDialog wcd = new WordCountDialog()) {
				wcd.Owner = (Form)WorkbenchSingleton.Workbench;
				wcd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
}
