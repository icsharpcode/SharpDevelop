// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Internal.ExternalTool
{
	/// <summary>
	/// This class describes an external tool, which is a external program
	/// that can be launched from the toolmenu inside Sharp Develop.
	/// </summary>
	public class ExternalTool
	{
		string menuCommand       = "New Tool";
		string command           = "";
		string arguments         = "";
		string initialDirectory  = "";
		bool   promptForArguments = false;
		bool   useOutputPad       = false;
		
		public string MenuCommand {
			get {
				return menuCommand;
			}
			set {
				menuCommand = value;
				System.Diagnostics.Debug.Assert(menuCommand != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string MenuCommand == null");
			}
		}
		
		public string Command {
			get {
				return command;
			}
			set {
				command = value;
				System.Diagnostics.Debug.Assert(command != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string Command == null");
			}
		}
		
		public string Arguments {
			get {
				return arguments;
			}
			set {
				arguments = value;
				System.Diagnostics.Debug.Assert(arguments != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string Arguments == null");
			}
		}
		
		public string InitialDirectory {
			get {
				return initialDirectory;
			}
			set {
				initialDirectory = value;
				System.Diagnostics.Debug.Assert(initialDirectory != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string InitialDirectory == null");
			}
		}
		
		public bool PromptForArguments {
			get {
				return promptForArguments;
			}
			set {
				promptForArguments = value;
			}
		}
		
		public bool UseOutputPad {
			get {
				return useOutputPad;
			}
			set {
				useOutputPad = value;
			}
		}
		
		public ExternalTool() 
		{
		}
		
		public ExternalTool(XmlElement el)
		{
			if (el == null) {
				throw new ArgumentNullException("ExternalTool(XmlElement el) : el can't be null");
			}
			
			if (el["INITIALDIRECTORY"] == null ||
				el["ARGUMENTS"] == null ||
				el["COMMAND"] == null ||
				el["MENUCOMMAND"] == null || 
				el["PROMPTFORARGUMENTS"] == null) {
				throw new Exception("ExternalTool(XmlElement el) : INITIALDIRECTORY and ARGUMENTS and COMMAND and MENUCOMMAND and PROMPTFORARGUMENTS attributes must exist.(check the ExternalTool XML)");
			}
			
			InitialDirectory  = el["INITIALDIRECTORY"].InnerText;
			Arguments         = el["ARGUMENTS"].InnerText;
			Command           = el["COMMAND"].InnerText;
			MenuCommand       = el["MENUCOMMAND"].InnerText;
			
			PromptForArguments = Boolean.Parse(el["PROMPTFORARGUMENTS"].InnerText);
			
			// option was introduced later
			if(el["USEOUTPUTPAD"] != null) {
				UseOutputPad = Boolean.Parse(el["USEOUTPUTPAD"].InnerText);
			}
		}
		
		public override string ToString()
		{
			return menuCommand;
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("ExternalTool.ToXmlElement(XmlDocument doc) : doc can't be null");
			}
			
			XmlElement el = doc.CreateElement("TOOL");
			
			XmlElement x = doc.CreateElement("INITIALDIRECTORY");
			x.InnerText = InitialDirectory;
			el.AppendChild(x);
			
			x = doc.CreateElement("ARGUMENTS");
			x.InnerText = Arguments;
			el.AppendChild(x);
			
			x = doc.CreateElement("COMMAND");
			x.InnerText = command;
			el.AppendChild(x);
			
			x = doc.CreateElement("MENUCOMMAND");
			x.InnerText = MenuCommand;
			el.AppendChild(x);
			
			x = doc.CreateElement("PROMPTFORARGUMENTS");
			x.InnerText = PromptForArguments.ToString();
			el.AppendChild(x);
			
			x = doc.CreateElement("USEOUTPUTPAD");
			x.InnerText = UseOutputPad.ToString();
			el.AppendChild(x);
			
			return el;
		}
	}
}
