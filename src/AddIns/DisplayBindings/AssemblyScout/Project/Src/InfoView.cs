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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class InfoView : UserControl
	{
		GradientLabel cap = new GradientLabel();
		Label typ = new Label();
		LinkLabel back = new LinkLabel();
		
		WebBrowser  ht = new WebBrowser();
		Panel pan = new Panel();
		
		AssemblyTree tree;
		IParserService ParserService;
		IAmbience ambience;
		PropertyService PropertyService;
		
		ArrayList references = new ArrayList();
		string cssPath;
		string imgPath;
		string resPath;
		
		public InfoView(AssemblyTree tree)
		{
			
			
			PropertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));			
			
			ambience = AmbienceService.CurrentAmbience;
			
			this.tree = tree;
			
			imgPath = Path.Combine(PropertyService.ConfigDirectory, "tempPicture.png");
			cssPath = Path.Combine(PropertyService.ConfigDirectory, "tempStylesheet.css");
			resPath = Path.Combine(PropertyService.ConfigDirectory, "tempResource");
			
			Color col = SystemColors.Control;
			string color = "#" + col.R.ToString("X") + col.G.ToString("X") + col.B.ToString("X");
			
			StreamWriter sw = new StreamWriter(cssPath, false);
			sw.Write(@"body { margin: 0px; border: 0px; overflow: hidden; padding: 0px;
							  background-color: " + color + @"; 
							  background-image: url(" + imgPath + @"); 
							  background-position: bottom right; 
							  background-repeat: no-repeat }
							  
					p   { font: 8pt Tahoma }
					div { margin: 0px; width: 100% }
					p.bottomline { font: 8pt Tahoma; border-bottom: 1px solid black; margin-bottom: 3px }
					p.docmargin  { font: 8pt Tahoma; margin-top: 0px; margin-bottom: 0px; padding-left: 8px; padding-right: 8px; padding-bottom: 3px; border-bottom: 1px solid black }

					a   { font: 8pt Tahoma; text-decoration: none; color: blue}
					a:visited { color: blue }
					a:active  { color: blue }
					a:hover   { color: red; text-decoration: underline }");
			sw.Close();
			
			cap.Location  = new Point(0, 0);
			cap.Size      = new Size(Width, 32);
			cap.Text      = tree.ress.GetString("ObjectBrowser.Welcome");
			cap.Font      = new Font("Tahoma", 14);
			cap.BackColor = SystemColors.ControlLight;
			cap.TextAlign = ContentAlignment.MiddleLeft;
			cap.Anchor    = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
						
			string backt  = tree.ress.GetString("ObjectBrowser.Back");
			back.Size     = new Size(40, 16);
			back.Location = new Point(Width - back.Width, 44);
			back.Text     = backt;
			back.TextAlign = ContentAlignment.TopRight;
			back.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
			back.Links.Add(0, backt.Length);
			back.LinkClicked += new LinkLabelLinkClickedEventHandler(back_click);
			
			typ.Location  = new Point(0, 44);
			typ.Size      = new Size(Width - back.Width, 16);
			typ.Font      = new Font(Font, FontStyle.Bold);
			typ.Text      = tree.ress.GetString("ObjectBrowser.WelcomeText");
			typ.TextAlign = ContentAlignment.TopLeft;
			typ.Anchor    = cap.Anchor;
			
			ht = new WebBrowser();
			//ht.Size = new Size(20, 20);
			//ht.Location = new Point(20, 20);
			ht.Navigating += new WebBrowserNavigatingEventHandler (HtmlControlBeforeNavigate);
			CreateImage(ResourceService.GetIcon("Icons.16x16.Class").ToBitmap());
//	TODO: StyleSheet		
//			ht.CascadingStyleSheet = cssPath;
			string html = RenderHead() + tree.ress.GetString("ObjectBrowser.Info.SelectNode") + RenderFoot();
			ht.DocumentText = html;
			
			pan.Location   = new Point(0, 72);
			pan.DockPadding.Left = 10;
			pan.DockPadding.Bottom = 75;
			pan.Size       = new Size(Width, Height - ht.Top);
			pan.Anchor     = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			pan.Controls.Add(ht);
			
			ht.Dock = DockStyle.Fill;
			
			Controls.AddRange(new Control[] {
				cap, typ, back, pan
			});
			
			Dock = DockStyle.Fill;
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
			
			ParserService = (IParserService)ServiceManager.Services.GetService(typeof(IParserService));
		}
		
		~InfoView() {
			System.IO.File.Delete(imgPath);
			System.IO.File.Delete(cssPath);
		}
		
		void HtmlControlBeforeNavigate(object sender, WebBrowserNavigatingEventArgs e)
		{
			e.Cancel = true;
			
			try {
				string url = e.Url;
				int refnr  = Int32.Parse(url.Substring(5, url.Length - 6));
				object obj = references[refnr];
				
				if (obj is IClass) {
					// Go To Type
					tree.GoToType((IClass)obj);
					// try {
					// 	tree.SelectedNode.Expand();
					// } catch {}
				} else if (obj is AssemblyTree.RefNodeAttribute) {
					// Open Assembly Reference
					tree.OpenAssemblyByName((AssemblyTree.RefNodeAttribute)obj);
				} else if (obj is SaveResLink) {
					SaveResLink link = (SaveResLink)obj;
					tree.SaveResource(link.Asm, link.Name);
				} else if (obj is NamespaceLink) {
					NamespaceLink ns = (NamespaceLink)obj;
					tree.GoToNamespace(ns.Asm, ns.Name);
				}
				
			} catch { 
				
				MessageService.ShowError("Something failed following this link.");
			}
		}
		
		string RenderHead()
		{
			return "<div><p>";
		}
		
		string RenderFoot()
		{
			return "</div>";
		}
				
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			AssemblyTreeNode node = (AssemblyTreeNode)e.Node;
			cap.Text = node.Text;
			typ.Text = node.Type.ToString();
			
			references.Clear();
			
			ambience.LinkArrayList = references;
			ambience.ConversionFlags = ConversionFlags.AssemblyScoutDefaults;
						
			CreateImage(tree.ImageList.Images[node.ImageIndex]);
			ht.Cursor = Cursors.Default;
			StringBuilder htmlSB = new StringBuilder(RenderHead());
			
			try {
			
				switch(node.Type) {
					case NodeType.Assembly:
						htmlSB.Append(GetAssemblyInfo((SA.SharpAssembly)node.Attribute));
						break;
					case NodeType.Library:
						htmlSB.Append(GetLibInfo((SA.SharpAssembly)node.Attribute));
						break;
					case NodeType.Reference:
						htmlSB.Append(GetRefInfo((AssemblyTree.RefNodeAttribute)node.Attribute));
						break;
					case NodeType.Resource:
						htmlSB.Append(GetResInfo((SA.SharpAssembly)node.Attribute, node.Text));
						break;
					case NodeType.SingleResource:
						htmlSB.Append(GetSingleResInfo(node.Attribute, node.Text));
						break;
					case NodeType.Folder:
						htmlSB.Append(GetFolderInfo(node.Text));
						break;
					case NodeType.Namespace:
						htmlSB.Append(GetNSInfo((SA.SharpAssembly)node.Attribute));
						break;
					case NodeType.SubTypes:
						htmlSB.Append(GetSubInfo());
						break;
					case NodeType.SuperTypes:
						htmlSB.Append(GetSuperInfo());
						break;
					case NodeType.Link:
						htmlSB.Append(GetLinkInfo((IClass)node.Attribute));
						break;
					case NodeType.Type:
						htmlSB.Append(GetTypeInfo((IClass)node.Attribute));
						break;
					case NodeType.Event:
						htmlSB.Append(GetEventInfo((IEvent)node.Attribute));
						break;
					case NodeType.Field:
						htmlSB.Append(GetFieldInfo((IField)node.Attribute));
						break;
					case NodeType.Constructor:
					case NodeType.Method:
						htmlSB.Append(GetMethodInfo((IMethod)node.Attribute));
						break;
					case NodeType.Property:
						htmlSB.Append(GetPropInfo((IProperty)node.Attribute));
						break;
					default:
						break;
				}
			} catch(Exception ex) {
				htmlSB.Append("<p class='bottomline'>" + tree.ress.GetString("ObjectBrowser.Info.CollectError") + "<p>" + ex.ToString().Replace("\n", "<br>"));
			}
			
			htmlSB.Append(RenderFoot());
			
			ht.DocumentText = htmlSB.ToString();
		}
		
		string GetLinkInfo(IClass type)
		{
			StringBuilder text = new StringBuilder(ln(references.Add(type), RT("GotoType")));
			text.Append("<p>");
			text.Append(RT("LinkedType"));
			text.Append(" ");
			text.Append(GetInAsm((SA.SharpAssembly)type.DeclaredIn));
			return text.ToString();			
		}
		
		string GetSubInfo()
		{
			return RT("SubInfo");
		}
		
		string GetSuperInfo()
		{
			return RT("SuperInfo");
		}
		
		string GetNSInfo(SA.SharpAssembly asm)
		{
			return GetInAsm(asm);
		}
		
		string GetFolderInfo(string folder)
		{
			if (folder == tree.ress.GetString("ObjectBrowser.Nodes.Resources")) 
				return RT("ResFInfo");
			else if (folder == tree.ress.GetString("ObjectBrowser.Nodes.References"))
				return RT("RefFInfo");
			else if (folder == tree.ress.GetString("ObjectBrowser.Nodes.Modules"))
				return RT("ModFInfo");
			else
				return RT("NoInfo");
		}
		
		string GetLibInfo(SA.SharpAssembly asm)
		{
			return String.Concat(RT("LibInfo"), "<p>", GetInAsm(asm));
		}
		
		string GetRefInfo(AssemblyTree.RefNodeAttribute asn)
		{
			string text = String.Format(RT("RefInfo"), asn.RefName.Name, asn.RefName.FullName, asn.RefName.Version.ToString(), "");
			return String.Concat(text, ln(references.Add(asn), RT("OpenRef")));
		}
		
		string GetResInfo(SA.SharpAssembly asm, string name)
		{
			long size = 0;
			try {
				size = asm.GetManifestResourceSize(name);
			} catch {}
			
			StringBuilder text = new StringBuilder(String.Format(RT("ResInfo"), name, size));
			text.Append(GetInAsm(asm));
			
			text.Append("<p>");
			text.Append(ln(references.Add(new SaveResLink(asm, name)), RT("SaveRes")));
			
			if (PropertyService.Get("AddIns.AssemblyScout.ShowResPreview", true) == false) {
				return text.ToString();
			}
			
			try {
				if (name.ToLower().EndsWith(".bmp") || name.ToLower().EndsWith(".gif") 
					|| name.ToLower().EndsWith(".png") || name.ToLower().EndsWith(".jpg")) {
					byte[] res = asm.GetManifestResource(name);
					FileStream fstr = new FileStream(resPath, FileMode.Create);
					BinaryWriter wr = new BinaryWriter(fstr);
					wr.Write(res);
					fstr.Close();
					
					text.Append("<p>Preview:<p>");
					text.Append("<img src=\"");
					text.Append(resPath);
					text.Append("\">");
				}
				if (name.ToLower().EndsWith(".tif")) {
					byte[] res = asm.GetManifestResource(name);
					Image tifImg = Image.FromStream(new MemoryStream(res));
					tifImg.Save(resPath, ImageFormat.Bmp);
					
					text.Append("<p>Preview:<p>");
					text.Append("<img src=\"");
					text.Append(resPath);
					text.Append("\">");
				}
				if (name.ToLower().EndsWith(".ico")) {
					byte[] res = asm.GetManifestResource(name);
					Icon icon = new Icon(new MemoryStream(res));
					using (Bitmap b = new Bitmap(icon.Width, icon.Height)) {
						Graphics g = Graphics.FromImage(b);
						g.FillRectangle(SystemBrushes.Control, 0, 0, b.Width, b.Height);
						g.DrawIcon(icon, 0, 0);
						            
						b.Save(resPath, System.Drawing.Imaging.ImageFormat.Png);
					}
					text.Append("<p>Preview:<p>");
					text.Append("<img src=\"");
					text.Append(resPath);
					text.Append("\">");
				}
				if (name.ToLower().EndsWith(".cur")) {
					byte[] res = asm.GetManifestResource(name);
					Cursor cursor = new Cursor(new MemoryStream(res));
					
					using (Bitmap b = new Bitmap(cursor.Size.Width, cursor.Size.Height)) {
						Graphics g = Graphics.FromImage(b);
						g.FillRectangle(SystemBrushes.Control, 0, 0, b.Width, b.Height);
						cursor.Draw(g, new Rectangle(0, 0, 32, 32));
						            
						b.Save(resPath, System.Drawing.Imaging.ImageFormat.Png);
					}
					
					text.Append("<p>Preview:<p>");
					text.Append("<img src=\"");
					text.Append(resPath);
					text.Append("\">");
				}
				if (name.ToLower().EndsWith(".txt") || name.ToLower().EndsWith(".xml") ||
				    name.ToLower().EndsWith(".xsd") || name.ToLower().EndsWith(".htm") ||
				    name.ToLower().EndsWith(".html") || name.ToLower().EndsWith(".xshd") ||
				    name.ToLower().EndsWith(".xsl") || name.ToLower().EndsWith(".txt")) {
					byte[] res = asm.GetManifestResource(name);
					string utf = System.Text.UTF8Encoding.UTF8.GetString(res);
					
					text.Append("<p>Preview:<br>");
					text.Append("<textarea style='border: 1px solid black; width: 300px; height: 400px; font: 8pt Tahoma'>");
					text.Append(utf);
					text.Append("</textarea>");
				}
			} catch {}
			
			return text.ToString();
		}
		
		string GetSingleResInfo(object singleRes, string name)
		{
			int len = name.Length;
			if (name.LastIndexOf(":") != -1) len = name.LastIndexOf(":");
			StringBuilder ret = new StringBuilder("Name: ");
			ret.Append(name.Substring(0, len));
			ret.Append("<p>");
			
			if (singleRes != null) {
				ret.Append("Type: ");
				ret.Append(singleRes.GetType().Name);
				ret.Append("<p>");
				ret.Append("Value:<br>");
				ret.Append(singleRes.ToString());
			}
			
			return ret.ToString();
		}
		
		string GetAssemblyInfo(SA.SharpAssembly asm)
		{
			string text = String.Format(RT("AsmInfo"),
			                        asm.Name, asm.FullName, asm.GetAssemblyName().Version.ToString(),
			                        asm.Location, asm.FromGAC);
			return text + GetCustomAttribs(asm);
		}
		
		string GetEventInfo(IEvent info)
		{
			StringBuilder ret = new StringBuilder(ambience.Convert(info));
			
			ret.Append("<p>");
			ret.Append(RT("Attributes"));
			ret.Append("<br>");
			ret.Append(GetCustomAttribs(info));
			
			IClass c = ParserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IEvent e in c.Events) {
				if(e.Name == info.Name) {
					if (e.Documentation == null || e.Documentation == "") continue;
					ret.Append("<p class='bottomline'>");
					ret.Append(RT("Documentation"));
					ret.Append("<p class='docmargin'>");
					ret.Append(GetDocumentation(e.Documentation));
					ret.Append("<p>");
					break;
				}
			}
						
			noDoc:
			
			ret.Append("<br>");
			ret.Append(GetInType(info.DeclaringType));
			ret.Append("<p>");
			ret.Append(GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn));
			
			return ret.ToString();
		}
		
		string GetFieldInfo(IField info)
		{
			StringBuilder ret = new StringBuilder(ambience.Convert(info));
			
			if (info is SharpAssemblyField) {
				SharpAssemblyField saField = info as SharpAssemblyField;
				if (saField.InitialValue != null) {
					ret.Append(" = ");
					ret.Append(saField.InitialValue.ToString());
				}
			}
			
			ret.Append("<p>");
			ret.Append(RT("Attributes"));
			ret.Append("<br>");
			ret.Append(GetCustomAttribs(info));
			
			IClass c = ParserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IField f in c.Fields) {
				if(f.Name == info.Name) {
					if (f.Documentation == null || f.Documentation == "") continue;
					ret.Append("<p class='bottomline'>");
					ret.Append(RT("Documentation"));
					ret.Append("<p class='docmargin'>");
					ret.Append(GetDocumentation(f.Documentation));
					ret.Append("<p>");
					break;
				}
			}
						
			noDoc:
			
			ret.Append("<br>");
			ret.Append(GetInType(info.DeclaringType));
			ret.Append("<p>");
			ret.Append(GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn));
			
			return ret.ToString();
		}
		
		string GetMethodInfo(IMethod info)
		{
			StringBuilder ret = new StringBuilder(ambience.Convert(info));
			
			ret.Append("<p>");
			ret.Append(RT("Attributes"));
			ret.Append("<br>");
			ret.Append(GetCustomAttribs(info));
			
			IClass c = ParserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IMethod cc in c.Methods) {
				if (cc.Name == info.Name) {
					if (cc.Documentation == null || cc.Documentation == "") continue;
					ret.Append("<p class='bottomline'>");
					ret.Append(RT("Documentation"));
					ret.Append("<p class='docmargin'>");
					ret.Append(GetDocumentation(cc.Documentation));
					ret.Append("<p>");
					break;
				}
			}
						
			noDoc:
			
			ret.Append("<br>");
			ret.Append(GetInType(info.DeclaringType));
			ret.Append("<p>");
			ret.Append(GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn));
			
			return ret.ToString();
		}
		
		string GetPropInfo(IProperty info)
		{
			StringBuilder ret = new StringBuilder(ambience.Convert(info));
			
			ret.Append("<p>");
			ret.Append(RT("Attributes"));
			ret.Append("<br>");
			ret.Append(GetCustomAttribs(info));
			
			IClass c = ParserService.GetClass(info.DeclaringType.FullyQualifiedName.Replace("+", "."));
			if(c == null) goto noDoc;
			foreach(IProperty p in c.Properties) {
				if(p.Name == info.Name) {
					if (p.Documentation == null || p.Documentation == "") continue;
					ret.Append("<p class='bottomline'>");
					ret.Append(RT("Documentation"));
					ret.Append("<p class='docmargin'>");
					ret.Append(GetDocumentation(p.Documentation));
					ret.Append("<p>");
					break;
				}
			}
						
			noDoc:
			
			ret.Append("<br>");
			ret.Append(GetInType(info.DeclaringType));
			ret.Append("<p>");
			ret.Append(GetInAsm((SA.SharpAssembly)info.DeclaringType.DeclaredIn));
			
			return ret.ToString();
		}
		
		string GetTypeInfo(IClass type)
		{
			StringBuilder t = new StringBuilder(ambience.Convert(type));
			
			t.Append("<p>");
			t.Append(RT("BaseTypes"));
			t.Append("<br>");
			t.Append(GetBaseTypes(type as SharpAssemblyClass));
			
			t.Append("<br>");
			t.Append(RT("Attributes"));
			t.Append("<br>");
			t.Append(GetCustomAttribs(type));
			
			IClass c = ParserService.GetClass(type.FullyQualifiedName.Replace("+", "."));
			if (c == null) goto noDoc;
			if (c.Documentation == null || c.Documentation == "") goto noDoc;
			t.Append("<p class='bottomline'>");
			t.Append(RT("Documentation"));
			t.Append("<p class='docmargin'>");
			t.Append(GetDocumentation(c.Documentation));
			t.Append("<p>");
			
			noDoc:
			
			if (type.Namespace == null || type.Namespace == "") goto inAsm;
			t.Append("<br>");
			t.Append(GetInNS((SA.SharpAssembly)type.DeclaredIn, type.Namespace));
			
			inAsm:
			t.Append("<p>");
			t.Append(GetInAsm((SA.SharpAssembly)type.DeclaredIn));
			
			return t.ToString();
		}
		
		string GetCustomAttribs(SA.SharpAssembly assembly)
		{
			return GetCustomAttribs(SharpAssemblyAttribute.GetAssemblyAttributes(assembly));
		}
		
		string GetCustomAttribs(IClass type)
		{
			if (type.Attributes.Count == 0) return "";
			return GetCustomAttribs(type.Attributes[0].Attributes);
		}
		
		string GetCustomAttribs(IMember member)
		{
			if (member.Attributes.Count == 0) return "";
			return GetCustomAttribs(member.Attributes[0].Attributes);
		}
		
		string GetCustomAttribs(AttributeCollection ca)
		{
			StringBuilder text = new StringBuilder();
			try {
				foreach(SharpAssemblyAttribute obj in ca) {
					text.Append(ln(references.Add(obj.AttributeType), obj.Name));
					text.Append(obj.ToString().Substring(obj.Name.Length));
					text.Append("<br>");
	
				}
			} catch {
				return "An error occured while looking for attributes.<br>";
			}
			return text.ToString();
		}
		
		string GetBaseTypes(SharpAssemblyClass type)
		{
			if (type == null || type.BaseTypeCollection.Count == 0) {
				return String.Empty;
			}
			
			StringBuilder text = new StringBuilder();
			
			foreach (SharpAssemblyClass basetype in type.BaseTypeCollection) {
				text.Append(ln(references.Add(basetype), basetype.FullyQualifiedName));
				text.Append("<br>");
			}

			return text.ToString();
		}

		string GetInAsm(SA.SharpAssembly asm)
		{
			StringBuilder text = new StringBuilder(RT("ContainedIn"));
			text.Append(" ");
			text.Append(ln(references.Add(new AssemblyTree.RefNodeAttribute(asm, asm.GetAssemblyName())), asm.Name));
			return text.ToString();
		}
		
		string GetInNS(SA.SharpAssembly asm, string ns)
		{
			return String.Concat(RT("Namespace"),
			                     " ",
			                     ln(references.Add(new NamespaceLink(asm, ns)), ns));
		}
		
		string GetInType(IClass type)
		{
			return String.Concat(RT("Type"),
			                     " ",
			                     ln(references.Add(type), type.FullyQualifiedName));
		}

		void back_click(object sender, LinkLabelLinkClickedEventArgs ev)
		{
			try {
				tree.GoBack();
			} catch {}
		}
		
		class SaveResLink
		{
			public SA.SharpAssembly Asm;
			public string Name;
			public SaveResLink(SA.SharpAssembly asm, string name)
			{
				Asm  = asm;
				Name = name;
			}
		}
		
		class NamespaceLink
		{
			public SA.SharpAssembly Asm;
			public string Name;
			public NamespaceLink(SA.SharpAssembly asm, string name)
			{
				Asm  = asm;
				Name = name;
			}
		}
		
		readonly static Regex whitespace = new Regex(@"\s+");
		public string GetDocumentation(string doc)
		{
			StringReader reader = new StringReader(String.Concat("<docroot>", doc, "</docroot>"));
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLower();
						if (elname == "remarks") {
							ret.Append(String.Concat("<b>", RTD("Remarks"), "</b><br>"));
						} else if (elname == "example") {
							ret.Append(String.Concat("<b>", RTD("Example"), "</b><br>"));
						} else if (elname == "exception") {
							ret.Append(String.Concat("<b>", RTD("Exception"), "</b> ", GetCref(xml["cref"]), ":<br>"));
						} else if (elname == "returns") {
							ret.Append(String.Concat("<b>", RTD("Returns"), "</b> "));
						} else if (elname == "see") {
							ret.Append(String.Concat(GetCref(xml["cref"]), xml["langword"]));
						} else if (elname == "seealso") {
							ret.Append(String.Concat("<b>", RTD("SeeAlso"), "</b> ", GetCref(xml["cref"]), xml["langword"]));
						} else if (elname == "paramref") {
							ret.Append(String.Concat("<i>", xml["name"], "</i>"));
						} else if (elname == "param") {
							ret.Append(String.Concat("<i>", xml["name"].Trim(), "</i>: "));
						} else if (elname == "value") {
							ret.Append(String.Concat("<b>", RTD("Value"), "</b> "));
						} else if (elname == "summary") {
							ret.Append(String.Concat("<b>", RTD("Summary"), "</b> "));
						}
					} else if (xml.NodeType == XmlNodeType.EndElement) {
						string elname = xml.Name.ToLower();
						if (elname == "para" || elname == "param") {
							ret.Append("<br>");
						}
						if (elname == "exception") {
							ret.Append("<br>");
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						ret.Append(whitespace.Replace(xml.Value, " "));
					}
				} while(xml.Read());
			} catch {
				return doc;
			}
			return ret.ToString();
		}
		
		string GetCref(string cref)
		{
			if (cref == null) {
				return String.Empty;
			}
			if (cref.Length < 2) {
				return cref;
			}
			if (cref.Substring(1, 1) == ":") {
				return String.Concat("<u>", cref.Substring(2, cref.Length - 2), "</u>");
			}
			return String.Concat("<u>", cref, "</u>");
		}

		string RT(string ResName)
		{
			return tree.ress.GetString(String.Concat("ObjectBrowser.Info.", ResName));
		}
		
		string RTD(string ResName)
		{
			return tree.ress.GetString(String.Concat("ObjectBrowser.Info.Doc.", ResName));
		}
		
		string ln(int rnr, string text)
		{
			return String.Concat("<a href='as://", rnr, "'>", text, "</a>");
		}
		
		bool CreateImage(Image pv)
		{
			try {
				using (Bitmap b = new Bitmap(170, 170, PixelFormat.Format24bppRgb)) {
					Graphics g = Graphics.FromImage(b);
					g.FillRectangle(SystemBrushes.Control, 0, 0, 170, 170);
					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					
					g.DrawImage(pv, 5, 5, 160, 160);
					using (Brush brush = new SolidBrush(Color.FromArgb(220, SystemColors.Control))) {
						g.FillRectangle(brush, 0, 0, 170, 170);
					}
					g.Dispose();
					
					b.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png);
				}
				return true;
			} catch { return false; }
		}
		
	}
	
	public class GradientLabel : Label
	{
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			g.FillRectangle(SystemBrushes.Control, pe.ClipRectangle);
			using (Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
			                                             SystemColors.ControlLightLight,
			                                             SystemColors.Control)) {
				g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
			}
		}
	}
		
}
