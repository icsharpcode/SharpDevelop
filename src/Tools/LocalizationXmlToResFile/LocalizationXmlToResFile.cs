// tbuilder.cs
// Copyright (c) 2001 Mike Krueger
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
