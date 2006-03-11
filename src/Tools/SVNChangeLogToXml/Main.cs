using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using NSvn.Common;
using NSvn.Core;

class MainClass
{
	public static int Main(string[] args)
	{
		Console.WriteLine("Initializing changelog application...");
		try {
			if (!File.Exists("SharpDevelop.sln")) {
				if (File.Exists(@"..\..\..\..\SharpDevelop.sln")) {
					Directory.SetCurrentDirectory(@"..\..\..\..");
				}
				if (File.Exists("..\\src\\SharpDevelop.sln")) {
					Directory.SetCurrentDirectory("..\\src");
				}
			}
			if (!File.Exists("SharpDevelop.sln")) {
				Console.WriteLine("Working directory must be SharpDevelop\\src or SharpDevelop\\bin!");
				return 2;
			}
			if (args.Length == 1 && args[0] == "--REVISION") {
				CreateRevisionFile();
			}
			ConvertChangeLog();
			return 0;
		} catch (Exception ex) {
			Console.WriteLine(ex);
			return 1;
		}
	}
	
	static void CreateRevisionFile()
	{
		Console.Write("Writing revision to file: ");
		int rev = new Client().SingleStatus(".").Entry.Revision;
		Console.WriteLine(rev);
		using (StreamWriter writer = new StreamWriter("../REVISION")) {
			writer.Write(rev.ToString());
		}
	}
	
	static void ConvertChangeLog()
	{
		Console.WriteLine("Reading SVN changelog, this might take a while...");
		
		Client client = new Client();
		client.AuthBaton.Add(AuthenticationProvider.GetUsernameProvider());
		client.AuthBaton.Add(AuthenticationProvider.GetSimpleProvider());
		client.AuthBaton.Add(AuthenticationProvider.GetSimplePromptProvider(PasswordPrompt, 3));
		
		StringWriter writer = new StringWriter();
		XmlTextWriter xmlWriter = new XmlTextWriter(writer);
		xmlWriter.Formatting = Formatting.Indented;
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("log");
		client.Log(new string[] {".."}, Revision.Base, Revision.FromNumber(2), false, false,
		           delegate(LogMessage message) {
		           	xmlWriter.WriteStartElement("logentry");
		           	xmlWriter.WriteAttributeString("revision", message.Revision.ToString(System.Globalization.CultureInfo.InvariantCulture));
		           	xmlWriter.WriteElementString("author", message.Author);
		           	xmlWriter.WriteElementString("date", message.Date.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture));
		           	xmlWriter.WriteElementString("msg", message.Message);
		           	xmlWriter.WriteEndElement();
		           });
		xmlWriter.WriteEndDocument();
		
		//Console.WriteLine(writer);
		
		XmlTextReader input = new XmlTextReader(new StringReader(writer.ToString()));
		
		XslCompiledTransform xsl = new XslCompiledTransform();
		xsl.Load(@"..\data\ConversionStyleSheets\SVNChangelogToXml.xsl");
		
		StreamWriter tw = new StreamWriter(@"..\doc\ChangeLog.xml", false, Encoding.UTF8);
		xmlWriter = new XmlTextWriter(tw);
		xmlWriter.Formatting = Formatting.Indented;
		xsl.Transform(input, xmlWriter);
		xmlWriter.Close();
		tw.Close();
		Console.WriteLine("Finished");
	}
	
	static SimpleCredential PasswordPrompt(string realm, string userName, bool maySave)
	{
		Console.WriteLine();
		Console.WriteLine("SUBVERSION: Authentication for realm: " + realm);
		Console.Write("Username: ");
		userName = Console.ReadLine();
		Console.Write("Password: ");
		string pwd = Console.ReadLine();
		return new SimpleCredential(userName, pwd, maySave);
	}
}
