// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class SourceView : UserControl
	{
		RichTextBox    rtb;
		CheckBox       chk;
		
		AssemblyTree tree;
		IAmbience ambience;
				
		void CopyEvent(object sender, EventArgs e)
		{
			Clipboard.SetDataObject(new DataObject(DataFormats.Text, rtb.Text));
		}
		
		public SourceView(AssemblyTree tree)
		{
			rtb = new RichTextBox();
			rtb.ReadOnly = true;
			
			
			
			
			ambience = AmbienceService.CurrentAmbience;

			rtb.Font = ResourceService.LoadFont("Courier New", 10);
			
			this.tree = tree;
			
			Dock = DockStyle.Fill;
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
			
			rtb.Location = new Point(0, 24);
			rtb.Size = new Size(Width, Height - 24);
			rtb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			
			rtb.ContextMenu = new ContextMenu(new MenuItem[] {
					new MenuItem("Copy", new EventHandler(CopyEvent))
				});
			
			chk = new CheckBox();
			chk.Location = new Point(0, 0);
			chk.Size = new Size(250, 16);
			chk.Text = tree.ress.GetString("ObjectBrowser.SourceView.Enable");
			chk.FlatStyle = FlatStyle.System;
			chk.CheckedChanged += new EventHandler(Check);
			Check(null, null);
			
			Controls.Add(rtb);
			Controls.Add(chk);
		}
		
		
		void Check(object sender, EventArgs e)
		{
			if(chk.Checked) {
				rtb.BackColor = SystemColors.Window;
			} else {
				rtb.BackColor = SystemColors.Control;
				rtb.Text = "";
			}
		}

		string GetAttributes(int indent, IMember member)
		{
			if (member.Attributes.Count == 0) {
				return String.Empty;
			}
			return GetAttributes(indent, member.Attributes[0].Attributes);
		}
		
		string GetAttributes(int indent, IClass type)
		{
			if (type.Attributes.Count == 0) {
				return String.Empty;
			}
			return GetAttributes(indent, type.Attributes[0].Attributes);
		}
		
		string GetAttributes(int indent, SA.SharpAssembly assembly)
		{
			return GetAttributes(indent, SharpAssemblyAttribute.GetAssemblyAttributes(assembly));
		}
		
		string GetAttributes(int indent, AttributeCollection ca)
		{
			StringBuilder text = new StringBuilder();
			try {
				foreach(SharpAssemblyAttribute obj in ca) {
					string attrString = obj.ToString();
					text.Append(ambience.WrapAttribute(attrString));
					text.Append('\n');
				}
			} catch {
			}
			return text.ToString();
		}

		string GetTypeInfo(IClass type)
		{
			StringBuilder rt = new StringBuilder();
			{
				string attr2 = GetAttributes(0, (SA.SharpAssembly)type.DeclaredIn);
				rt.Append(ambience.WrapComment("assembly attributes\n") + attr2 + "\n" + ambience.WrapComment("declaration\n"));
			}
			string attr = GetAttributes(0, type);
			rt.Append(attr);
			rt.Append(ambience.Convert(type));
			rt.Append("\n");
			
			if (type.ClassType != ClassType.Enum) {
				
				rt.Append("\t" + ambience.WrapComment("events\n"));
				
				foreach (IField fieldinfo in type.Fields) {
					rt.Append(GetAttributes(1, fieldinfo));
					rt.Append("\t" + ambience.Convert(fieldinfo) + "\n");
				}
				
				rt.Append("\t" + ambience.WrapComment("methods\n"));
				
				foreach (IMethod methodinfo in type.Methods) {
					if (methodinfo.IsSpecialName) {
						continue;
					}
					ambience.ConversionFlags |= ConversionFlags.ShowReturnType;
					rt.Append(GetAttributes(1, methodinfo));
					rt.Append("\t" + ambience.Convert(methodinfo));
					if (type.ClassType == ClassType.Interface)
						rt.Append("\n\n");
					else {
						rt.Append("\n\t\t" + ambience.WrapComment("TODO") + "\n\t" + ambience.ConvertEnd(methodinfo) + "\n\n");
					}
				}
				
				rt.Append("\t" + ambience.WrapComment("properties\n"));
				
				foreach (IProperty propertyinfo in type.Properties) {
					rt.Append(GetAttributes(1, propertyinfo));
					rt.Append("\t" + ambience.Convert(propertyinfo) + "\n");
				}
				
				rt.Append("\t" + ambience.WrapComment("events\n"));
				
				foreach (IEvent eventinfo in type.Events) {
					rt.Append(GetAttributes(1, eventinfo));
					rt.Append("\t" + ambience.Convert(eventinfo) + "\n");
				}
			} else { // Enum
				foreach (IField fieldinfo in type.Fields) {					
					if (fieldinfo.IsLiteral) {
						attr = GetAttributes(1, fieldinfo);
						rt.Append(attr);
						rt.Append("\t" + fieldinfo.Name);
												
						if (fieldinfo is SharpAssemblyField) {
							SharpAssemblyField saField = fieldinfo as SharpAssemblyField;
							if (saField.InitialValue != null) {
								rt.Append(" = " + saField.InitialValue.ToString());
							}
						}
						
						rt.Append(",\n");
					}
				}
			}
			
			rt.Append(ambience.ConvertEnd(type));
			
			return rt.ToString();
		}
		
		string GenNodeSource(AssemblyTreeNode node)
		{
			if (node.Attribute is IClass)  {
				ambience.ConversionFlags = ConversionFlags.All | ConversionFlags.QualifiedNamesOnlyForReturnTypes | ConversionFlags.IncludeBodies;
				/*
				if (node.Attribute is SharpAssemblyClass) {
					if (!(node.Attribute as SharpAssemblyClass).MembersLoaded) (node.Attribute as SharpAssemblyClass).LoadMembers();
				}
				*/
				return GetTypeInfo((IClass)node.Attribute);
			} else {
				switch (node.Type) {
					case NodeType.Namespace:
						StringBuilder nsContents = new StringBuilder("namespace " + node.Text + "\n{\n");
						foreach (AssemblyTreeNode childNode in node.Nodes) {
							nsContents.Append(GenNodeSource(childNode));
							nsContents.Append("\n");
						}
						
						nsContents.Append("\n}");
						return nsContents.ToString();
					default:
						return tree.ress.GetString("ObjectBrowser.SourceView.NoView");
				}
			}
//			return String.Empty;
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			if(!chk.Checked) return;
			
			AssemblyTreeNode node = (AssemblyTreeNode)e.Node;
			
			rtb.Text = GenNodeSource(node);;
		}
		
	}
	
}
