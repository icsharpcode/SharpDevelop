using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

public class HelperFunctions
{
	public static string ExecuteCmdLineApp(string strCmd)
	{
		string output = "";
		string error  = "";
		
		TempFileCollection tf = new TempFileCollection();
		Executor.ExecWaitWithCapture(strCmd, tf, ref output, ref error);
		
		StreamReader sr = File.OpenText(output);
		StringBuilder strBuilder = new StringBuilder();
		string strLine = null;
		
		while (null != (strLine = sr.ReadLine())) {
			if ("" != strLine) {
				strBuilder.Append(strLine);
				strBuilder.Append("\r\n");
			}
		}
		sr.Close();
		
		File.Delete(output);
		File.Delete(error);
		
		return strBuilder.ToString();
	}
	
	public static bool GetHelpUpToDate()
	{
		DateTime sourceDate, targetDate;
		string basePath = Application.StartupPath + Path.DirectorySeparatorChar +
		                  ".." + Path.DirectorySeparatorChar +
		                  ".." + Path.DirectorySeparatorChar +
		                  "doc" + Path.DirectorySeparatorChar +
		                  "help";
		string filename = basePath + Path.DirectorySeparatorChar + "HelpDescription.xml";
		string targetname = basePath + Path.DirectorySeparatorChar + "SharpDevelopHelp.zip";
		if (!File.Exists(targetname)) return false;
		if (!File.Exists(filename)) {
			Console.WriteLine("HelpDescription.xml not found!");
			return false;
		}
		try {
			sourceDate = File.GetLastWriteTime(filename);
			targetDate = File.GetLastWriteTime(targetname);
			if (sourceDate > targetDate) return false;
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			return ParseTree(doc.DocumentElement, targetDate, basePath);
		} catch (Exception ex) {
			Console.WriteLine("Error checking if the help is up to date:");
			Console.WriteLine(ex.ToString());
			return false;
		}
	}
	
	/// <remarks>
	/// Parses the xml tree and returns false if the target is out of date.
	/// </remarks>
	static bool ParseTree(XmlNode parentNode, DateTime targetDate, string basePath)
	{
		try {
			foreach (XmlNode node in parentNode.ChildNodes) {
				switch (node.Name) {
					case "Condition": // condition is always true...
					case "HelpFolder":
						if (!ParseTree(node, targetDate, basePath))
							return false;
						break;
					case "HelpFile":
						string filename = basePath + Path.DirectorySeparatorChar + node.Attributes["hhc"].InnerText;
						if (File.Exists(filename)) {
							if (File.GetLastWriteTime(filename) > targetDate)
								return false;
						}
						break;
					case "HelpAssemblies":
						// ignore assemblies... they should be up to date
						break;
				}
			}
		} catch (Exception e) {
			Console.Error.WriteLine("unexpected exception : " + e.ToString());
		}
		return true;
	}
}
