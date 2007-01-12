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
	internal class VisibilityModifiersEditor : ComboBox
	{
		public VisibilityModifiersEditor()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Public);
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Private);
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Protected);
			Items.Add(ICSharpCode.NRefactory.Ast.Modifiers.Internal);
		}
	}
	
	internal class ParameterModifiersEditor : ComboBox
	{
		public ParameterModifiersEditor()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			Items.Add(ParameterModifiers.In);
			Items.Add(ParameterModifiers.Out);
			Items.Add(ParameterModifiers.Ref);
			Items.Add(ParameterModifiers.Params);
			Items.Add(ParameterModifiers.Optional);
		}
	}
}
