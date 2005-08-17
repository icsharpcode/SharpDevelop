// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.NRefactory
{
	/// <summary>
	/// Description of Main.
	/// </summary>
	public class MainClass
	{
		public static ArrayList SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true);
		}
		
		public static ArrayList SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			ArrayList collection = new ArrayList();
			SearchDirectory(directory, filemask, collection, searchSubdirectories);
			return collection;
		}
		
		/// <summary>
		/// Finds all files which are valid to the mask <code>filemask</code> in the path
		/// <code>directory</code> and all subdirectories (if searchSubdirectories
		/// is true. The found files are added to the ArrayList
		/// <code>collection</code>.
		/// </summary>
		static void SearchDirectory(string directory, string filemask, ArrayList collection, bool searchSubdirectories)
		{
			try {
				string[] file = Directory.GetFiles(directory, filemask);
				foreach (string f in file) {
					collection.Add(f);
				}
				
				if (searchSubdirectories) {
					string[] dir = Directory.GetDirectories(directory);
					foreach (string d in dir) {
						try {
							SearchDirectory(d, filemask, collection, searchSubdirectories);
						} catch (Exception) {}
					}
				}
			} catch (Exception) {
			}
		}
		
		public static void Main(string[] args)
		{
		
//			string program = @"
//public class SimpleHandler {
//
//    public void ProcessRequest(HttpContext context) {
//      labelDate.Text = 123456789.ToString();      
//    }
//    
//}
//";
			IParser parser = ParserFactory.CreateParser(@"c:\Dokumente und Einstellungen\Markus\Desktop\CustomEvents.vb");
			parser.Parse();
			if (parser.Errors.ErrorOutput.Length > 0) {
				Console.WriteLine(parser.Errors.ErrorOutput);
				Console.ReadLine();
			}
			/*
			string searchPath = @"C:\Programme\Microsoft.NET\SDK\v1.1"; //Path.GetFullPath(Application.StartupPath + @"\..\..\..\..");
			ArrayList files = SearchDirectory(searchPath, "*.cs", true);
			ArrayList defs = new ArrayList();
			long oldSet = Environment.WorkingSet;
			
			DateTime start = DateTime.Now;
			foreach (string str in files) {
				IParser parser = ParserFactory.CreateParser(str);
				parser.Parse();
			}
			*/
		}	
	}
}

/* Supported .NET 2.0 features
 
 
 CONTINUE statement
 USING statement
 UInteger, ULong, UShort, SByte data type
 IsNot operator
 Partial classes
 
 TODO:
 
  Explicit Zero Lower Bound on an Array (Don't know if this requires grammar change, I've no documentation about it.)
  Properties with Mixed Access Levels
  Operator Overloading 
  Generic Types 

 */
