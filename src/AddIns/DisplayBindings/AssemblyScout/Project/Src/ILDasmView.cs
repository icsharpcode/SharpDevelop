// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;
using ICSharpCode.Core;
using Microsoft.Win32;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class ILDasmView : UserControl
	{
		CheckBox chk   = new CheckBox();
		RichTextBox tb = new RichTextBox();
		AssemblyTree tree;
		
		public ILDasmView(AssemblyTree tree)
		{
			this.tree = tree;
			
			Dock = DockStyle.Fill;
			
			tb.Location = new Point(0, 24);
			tb.Size = new Size(Width, Height - 24);
			tb.Font = new System.Drawing.Font("Courier New", 10);
			tb.ScrollBars = RichTextBoxScrollBars.Both;
			tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			
			tb.WordWrap  = false;
			tb.ReadOnly  = true;
			
			chk.Location = new Point(0, 0);
			chk.Size = new Size(250, 16);
			chk.Text = tree.ress.GetString("ObjectBrowser.ILDasm.Enable");
			chk.FlatStyle = FlatStyle.System;
			
			chk.CheckedChanged += new EventHandler(Check);
			Check(null, null);
			
			Controls.Add(tb);
			Controls.Add(chk);
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
		}
		
		void Check(object sender, EventArgs e)
		{
			if(chk.Checked) {
				tb.BackColor = SystemColors.Window;
			} else {
				tb.BackColor = SystemColors.Control;
				tb.Text = "";
			}
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			if (!chk.Checked) return;
			
			AssemblyTreeNode node = (AssemblyTreeNode)e.Node;
			
			SA.SharpAssembly assembly = null;
			string item = " /item=";
			
			if (node.Attribute is SA.SharpAssembly) {
				assembly = (SA.SharpAssembly)node.Attribute;
			} else if (node.Attribute is IClass) {
				IClass type = (IClass)node.Attribute;
				item += type.FullyQualifiedName;
				assembly = (SA.SharpAssembly)type.DeclaredIn;
			} else if (node.Attribute is IMethod) {
				IMethod method = (IMethod)node.Attribute;
				item += method.DeclaringType.FullyQualifiedName + "::" + method.Name;
				assembly = (SA.SharpAssembly)method.DeclaringType.DeclaredIn;
			} else {
				tb.Text = tree.ress.GetString("ObjectBrowser.ILDasm.NoView");
				return;
			}
			tb.Text = GetILDASMOutput(assembly, item).Replace("\n", "\r\n");
		}

		

		private string GetILDASMOutput(SA.SharpAssembly assembly, string item)
		{
			try {
				string args = '"' + assembly.Location + '"' + item + " /NOBAR /TEXT";
                RegistryKey regKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\.NETFramework");
                string cmd = (string)regKey.GetValue("sdkInstallRoot");
				if (cmd == null) cmd = (string)regKey.GetValue("sdkInstallRootv1.1");
                ProcessStartInfo psi = new ProcessStartInfo(FileUtility.GetDirectoryNameWithSeparator(cmd) +
                                                            "bin\\ildasm.exe", args);
				
				psi.RedirectStandardError  = true;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardInput  = true;
				psi.UseShellExecute        = false;
				psi.CreateNoWindow         = true;
				
				Process process = Process.Start(psi);
				string output   = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
								
				int cutpos = output.IndexOf(".namespace");
				
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				cutpos = output.IndexOf(".class");
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				cutpos = output.IndexOf(".method");
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				return output;
			} catch (Exception) {
				return tree.ress.GetString("ObjectBrowser.ILDasm.NotInstalled");
			}
		}
		
	}
}
