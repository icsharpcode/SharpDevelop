// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
namespace Plugins.Wizards.MessageBoxBuilder.Generator {
	
	public class MessageBoxGenerator
	{
		MessageBoxButtons       messageBoxButtons       = MessageBoxButtons.OK;
		MessageBoxIcon          messageBoxIcon          = MessageBoxIcon.None;
		MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button1;
		
		string text    = String.Empty;
		string caption = String.Empty;
		
		bool   generateReturnValue = false;
		string variableName        = String.Empty;
		bool   generateSwitchCase  = false;
		
		public MessageBoxButtons MessageBoxButtons {
			get {
				return messageBoxButtons;
			}
			set {
				messageBoxButtons = value;
				OnChanged(null);
			}
		}

		public MessageBoxIcon MessageBoxIcon {
			get {
				return messageBoxIcon;
			}
			set {
				messageBoxIcon = value;
				OnChanged(null);
			}
		}

		public MessageBoxDefaultButton MessageBoxDefaultButton {
			get {
				return messageBoxDefaultButton;
			}
			set {
				messageBoxDefaultButton = value;
				OnChanged(null);
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				OnChanged(null);
			}
		}
		
		public string Caption {
			get {
				return caption;
			}
			set {
				caption = value;
				OnChanged(null);
			}
		}

		public bool GenerateReturnValue {
			get {
				return generateReturnValue;
			}
			set {
				generateReturnValue = value;
				OnChanged(null);
			}
		}

		public string VariableName {
			get {
				return variableName;
			}
			set {
				variableName = value;
				OnChanged(null);
			}
		}

		public bool GenerateSwitchCase {
			get {
				return generateSwitchCase;
			}
			set {
				generateSwitchCase = value;
				OnChanged(null);
			}
		}
		
		public void PreviewMessageBox()
		{
			MessageBox.Show(text, caption, messageBoxButtons, messageBoxIcon, messageBoxDefaultButton);
		}
		
		public string GenerateCode(string language) 
		{
			if (language == "C#") {
				string code = "MessageBox.Show(\"" + text.Replace("\n", "\\n").Replace("\r","").Replace("\"", "\\\"") + "\", \"" + 
				                                     caption + "\", MessageBoxButtons." + 
				                                     messageBoxButtons + ", MessageBoxIcon." + 
				                                     messageBoxIcon + ", MessageBoxDefaultButton." + 
				                                     messageBoxDefaultButton +");";
				if (generateReturnValue) {
					code = "DialogResult " + variableName + " = " + code;
					if (generateSwitchCase) {
						code += "\nswitch(" + variableName +") {\n";
						switch(messageBoxButtons) {
							case MessageBoxButtons.AbortRetryIgnore:
								code += "\tcase DialogResult.Abort:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.Retry:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.Ignore:\n";
								code += "\t\tbreak;\n";
								break;
							case MessageBoxButtons.OK:
								code += "\tcase DialogResult.OK:\n";
								code += "\t\tbreak;\n";
								break;
							case MessageBoxButtons.OKCancel:
								code += "\tcase DialogResult.OK:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.Cancel:\n";
								code += "\t\tbreak;\n";
								break;
							case MessageBoxButtons.RetryCancel:
								code += "\tcase DialogResult.Retry:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.Cancel:\n";
								code += "\t\tbreak;\n";
								break;
							case MessageBoxButtons.YesNo:
								code += "\tcase DialogResult.Yes:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.No:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.Ignore:\n";
								code += "\t\tbreak;\n";
								break;
							case MessageBoxButtons.YesNoCancel:
								code += "\tcase DialogResult.Yes:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.No:\n";
								code += "\t\tbreak;\n";
								code += "\tcase DialogResult.Cancel:\n";
								code += "\t\tbreak;\n";
								break;
						}
						code += "}";
					}
				}
				return code;
			} else if (language == "VBNET") {
				string code = "MessageBox.Show(\"" + text.Replace("\n", "\\n").Replace("\r","") + "\", \"" + 
				                                     caption + "\", MessageBoxButtons." + 
				                                     messageBoxButtons + ", MessageBoxIcon." + 
				                                     messageBoxIcon + ", MessageBoxDefaultButton." + 
				                                     messageBoxDefaultButton +")";
				if (generateReturnValue) {
					code = "Dim " + variableName + " As DialogResult = " + code;
					if (generateSwitchCase) {
						code += "\nSelect " + variableName +"\n";
						switch(messageBoxButtons) {
							case MessageBoxButtons.AbortRetryIgnore:
								code += "\tCase DialogResult.Abort\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.Retry\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.Ignore\n";
								code += "\t\t\n";
								break;
							case MessageBoxButtons.OK:
								code += "\tCase DialogResult.OK\n";
								code += "\t\t\n";
								break;
							case MessageBoxButtons.OKCancel:
								code += "\tCase DialogResult.OK\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.Cancel\n";
								code += "\t\t\n";
								break;
							case MessageBoxButtons.RetryCancel:
								code += "\tCase DialogResult.Retry\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.Cancel\n";
								code += "\t\t\n";
								break;
							case MessageBoxButtons.YesNo:
								code += "\tCase DialogResult.Yes\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.No\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.Ignore\n";
								code += "\t\t\n";
								break;
							case MessageBoxButtons.YesNoCancel:
								code += "\tCase DialogResult.Yes\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.No\n";
								code += "\t\t\n";
								code += "\tCase DialogResult.Cancel\n";
								code += "\t\t\n";
								break;
						}
						code += "End Select";
					}
				}
				return code;
			} else {
				throw new System.ArgumentException("Unknown language: " + language);
			}
		}
		
		void OnChanged(EventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public event EventHandler Changed;
	}
}
