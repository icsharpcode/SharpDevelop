using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using PumaCode.SvnDotNet.AprSharp;
using PumaCode.SvnDotNet.SubversionSharp;

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
		
		SvnClient client = new SvnClient();
		string filename = Path.GetFullPath(".");
		int rev = 0;
		client.Status2(
			filename, Svn.Revision.Working, 
			delegate (IntPtr baton, SvnPath path, SvnWcStatus2 status) {
				if (StringComparer.InvariantCultureIgnoreCase.Equals(filename, path.Value)) {
					rev = status.Entry.Revision;
				}
			},
			IntPtr.Zero,
			false, true, false, false, false
		);
		client.GlobalPool.Destroy();
		
		Console.WriteLine(rev);
		using (StreamWriter writer = new StreamWriter("../REVISION")) {
			writer.Write(rev.ToString());
		}
	}
	
	static void ConvertChangeLog(int startRevision)
	{
		Console.WriteLine("Reading SVN changelog, this might take a while...");
		
		SvnClient client = new SvnClient();
		client.AddUsernameProvider();
		client.AddSimpleProvider();
		client.OpenAuth();
		
		StringWriter writer = new StringWriter();
		XmlTextWriter xmlWriter = new XmlTextWriter(writer);
		xmlWriter.Formatting = Formatting.Indented;
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("log");
		int progressCount = 0;
		client.Log2(
			new SvnPath[] { new SvnPath("..", client.Pool)},
			Svn.Revision.Base, new SvnRevision(startRevision),
			int.MaxValue, false, false,
			delegate(IntPtr baton, AprHash changed_paths, int revision, AprString author, AprString date, AprString message, AprPool pool) {
				if (++progressCount == 10) {
					Console.Write(".");
					progressCount = 0;
				}
				xmlWriter.WriteStartElement("logentry");
				xmlWriter.WriteAttributeString("revision", revision.ToString(System.Globalization.CultureInfo.InvariantCulture));
				xmlWriter.WriteElementString("author", author.Value);
				xmlWriter.WriteElementString("date", DateTime.Parse(date.Value).ToUniversalTime().ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture));
				xmlWriter.WriteElementString("msg", message.Value);
				xmlWriter.WriteEndElement();
				return SvnError.NoError;
			},
			IntPtr.Zero);
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
		
		client.GlobalPool.Destroy();
		
		Console.WriteLine("Finished");
	}
}
