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
using SA = ICSharpCode.SharpAssembly.Assembly;

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
			chk.Text = StringParser.Parse("${res:ObjectBrowser.ILDasm.Enable}");
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
			} else if (node.Attribute is SharpAssemblyClass) {
				// TODO Check this works - was node.Attribute is IClass if statement.
				SharpAssemblyClass type = (SharpAssemblyClass)node.Attribute;
				item += type.FullyQualifiedName;
				assembly = (SA.SharpAssembly)type.DeclaredIn;
			} else if (node.Attribute is SharpAssemblyMethod) {
				// TODO Check this works - was node.Attribute is IMethod if statement.
				SharpAssemblyMethod method = (SharpAssemblyMethod)node.Attribute;
				item += method.DeclaringType.FullyQualifiedName + "::" + method.Name;
				SharpAssemblyClass type = method.DeclaringType as SharpAssemblyClass;
				if (type != null) {
					assembly = (SA.SharpAssembly)type.DeclaredIn;
				} else {
					LoggingService.Error("ILDasmView.SelectedNode - Should not be getting here.");
					tb.Text = StringParser.Parse("${res:ObjectBrowser.ILDasm.NoView}");
					return;
				}
			} else {
				tb.Text = StringParser.Parse("${res:ObjectBrowser.ILDasm.NoView}");
				return;
			}
			tb.Text = GetILDASMOutput(assembly, item).Replace("\n", "\r\n");
		}

		

		private string GetILDASMOutput(SA.SharpAssembly assembly, string item)
		{
			try {
				string args = '"' + assembly.Location + '"' + item + " /NOBAR /TEXT";
                ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(FileUtility.NetSdkInstallRoot, "bin\\ildasm.exe"), args);
				
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
				return StringParser.Parse("${res:ObjectBrowser.ILDasm.NotInstalled}");
			}
		}
		
	}
}
