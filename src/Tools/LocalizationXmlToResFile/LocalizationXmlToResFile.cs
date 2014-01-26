// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Drawing;
using System.Resources;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

public class TranslationBuilder
{
	static void Assemble(string pattern)
	{
		string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), pattern);
		
		foreach (string file in files) {
			if (Path.GetExtension(file).ToUpper() == ".XML") {
				try {
					XmlDocument  doc = new XmlDocument();
					doc.Load(file);
					string resfilename = "StringResources." + doc.DocumentElement.Attributes["language"].InnerText + ".resources";
					ResourceWriter  rw = new ResourceWriter(resfilename);
					
					foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
						rw.AddResource(el.Attributes["name"].InnerText, 
						               el.InnerText);
					}
					
					rw.Generate();
					rw.Close();
				} catch (Exception e) {
					Console.WriteLine("Error while processing " + file + " :");
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
	
	static void ShowHelp()
	{
		Console.WriteLine(".NET Translation Builder Version 0.1");
		Console.WriteLine("Copyright (C) Mike Krueger 2001. Released under GPL.\n");
		Console.WriteLine("                      Translation Builder Options Options\n");
		Console.WriteLine("                        - INPUT FILES -");
		Console.WriteLine("<wildcard>              translates the given xml files into resource files");
	}
	
	public static void Main(string[] args)
	{
		if (args.Length == 0) {
			ShowHelp();
		}
		foreach (string param in args) {
			string par = param.ToUpper();
			if (par == "/?" || par == "/H" || par== "-?" || par == "-H" || par == "?") {
				ShowHelp();
				return;
			} else {
				Assemble(param);
			}
		}
	}
}
