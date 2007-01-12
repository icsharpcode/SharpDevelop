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
	public class IParameterEventArgs : EventArgs
	{
		IParameter parameter;
		IMethod method;
		public IParameterEventArgs (IMethod method, IParameter parameter)
		{
			this.method = method;
			this.parameter = parameter;
		}
		
		public IParameter Parameter
		{
			get { return parameter; }
		}
		
		public IMethod Method
		{
			get { return method; }
		}
	}
	
	public class IParameterModificationEventArgs : IParameterEventArgs
	{
		Modification modification;
		string newValue;
		
		bool cancel = false;
		
		public IParameterModificationEventArgs(IMethod method, IParameter parameter, Modification modification, string newValue)
			: base (method, parameter)
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
