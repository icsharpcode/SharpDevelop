/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 20/10/2006
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ClassDiagram
{
	public class IMemberEventArgs : EventArgs
	{
		IMember member;
		public IMemberEventArgs (IMember member)
		{
			this.member = member;
		}
		
		public IMember Member
		{
			get { return member; }
		}
	}
	
	public class IMemberModificationEventArgs : IMemberEventArgs
	{
		Modification modification;
		string newValue;
		
		bool cancel = false;
		
		public IMemberModificationEventArgs(IMember member, Modification modification, string newValue)
			: base (member)
		{
			this.modification = modification;
			this.newValue = newValue;
		}
		
		public Modification Modification
		{
			get { return modification; }
		}
		
		public string NewValue
		{
			get { return newValue; }
		}
		
		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}
	}

}
