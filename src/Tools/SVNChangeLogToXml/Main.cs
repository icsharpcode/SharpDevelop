using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

class MainClass
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Reading SVN changlog, this might take a while...");
		
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.FileName = "svn";
		process.StartInfo.Arguments = "log --xml";
		
		// this has to point to the root directory (/trunk/SharpDevelop)
		process.StartInfo.WorkingDirectory = Path.Combine(Application.StartupPath, @"..\..");
		try {
			process.Start();
		} catch(Win32Exception) {
			// subversion not installed
			return;
		}
		string output = process.StandardOutput.ReadToEnd();
		// convert date format
		output = Regex.Replace(output, @"(\d{4})-(\d{2})-(\d{2}).*?</date>", "$2/$3/$1</date>");
		
		MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(output));
		XslTransform xsl = new XslTransform();
		xsl.Load(Path.Combine(Application.StartupPath, @"..\..\data\ConversionStyleSheets\SVNChangelogToXml.xsl"));
		
		XmlDocument doc = new XmlDocument();
		doc.Load(ms);
		
		StreamWriter tw = new StreamWriter(Path.Combine(Path.Combine(Application.StartupPath, @"..\..\doc"), "ChangeLog.xml"));
		xsl.Transform(doc.CreateNavigator(), null, tw, null);
		tw.Close();
		ms.Close();
		Console.WriteLine("Finished");
	}
}
