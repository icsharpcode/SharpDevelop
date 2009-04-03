using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

using SharpSvn;

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

			int start = 2;
			for(int i = 0; i < args.Length; i++)
			{
				if(args[i] == "--REVISION")
				{
					CreateRevisionFile();
				}
				else if(args[i] == "--START") {
					Int32.TryParse(args[i + 1], out start);
				}
			}
			ConvertChangeLog(start);
			return 0;
		} catch (Exception ex) {
			Console.WriteLine(ex);
			return 1;
		}
	}
	
	static void CreateRevisionFile()
	{
		Console.Write("Writing revision to file: ");
		
		long rev = 0;
		string filename = Path.GetFullPath(".");
		SvnWorkingCopyClient client = new SvnWorkingCopyClient();
		SvnWorkingCopyVersion version;
		if (client.GetVersion(filename, out version)) {
			rev = version.Start;
		}
		Console.WriteLine(rev);
		using (StreamWriter writer = new StreamWriter("../REVISION")) {
			writer.Write(rev.ToString());
		}
	}
	
	static void ConvertChangeLog(int startRevision)
	{
		Console.WriteLine("Reading SVN changelog, this might take a while...");
		
		SvnClient client = new SvnClient();
		
		StringWriter writer = new StringWriter();
		XmlTextWriter xmlWriter = new XmlTextWriter(writer);
		xmlWriter.Formatting = Formatting.Indented;
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("log");
		int progressCount = 0;
		client.Log(
			"..",
			new SvnLogArgs {
				// retrieve log in reverse order
				Start = SvnRevision.Base,
				End = new SvnRevision(startRevision)
			},
			delegate(object sender, SvnLogEventArgs e) {
				if (++progressCount == 10) {
					Console.Write(".");
					progressCount = 0;
				}
				xmlWriter.WriteStartElement("logentry");
				xmlWriter.WriteAttributeString("revision", e.Revision.ToString(CultureInfo.InvariantCulture));
				xmlWriter.WriteElementString("author", e.Author);
				xmlWriter.WriteElementString("date", e.Time.ToUniversalTime().ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture));
				xmlWriter.WriteElementString("msg", e.LogMessage);
				xmlWriter.WriteEndElement();
			}
		);
		xmlWriter.WriteEndDocument();
		
		Console.WriteLine();
		
		XmlTextReader input = new XmlTextReader(new StringReader(writer.ToString()));
		
		XslCompiledTransform xsl = new XslCompiledTransform();
		xsl.Load(@"..\data\ConversionStyleSheets\SVNChangelogToXml.xsl");
		
		StreamWriter tw = new StreamWriter(@"..\doc\ChangeLog.xml", false, Encoding.UTF8);
		xmlWriter = new XmlTextWriter(tw);
		xmlWriter.Formatting = Formatting.Indented;
		xsl.Transform(input, xmlWriter);
		xmlWriter.Close();
		tw.Close();
		
		client.Dispose();
		
		Console.WriteLine("Finished");
	}
}
