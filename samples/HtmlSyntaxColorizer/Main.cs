// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.HtmlSyntaxColorizer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			HtmlWriter w = new HtmlWriter();
			w.ShowLineNumbers = true;
			w.AlternateLineBackground = true;
			string code = File.ReadAllText("../../Main.cs");
			string html = w.GenerateHtml(code, "C#");
			File.WriteAllText("output.html", "<html><body>" + html + "</body></html>");
			Process.Start("output.html"); // view in browser
		}
	}
}
