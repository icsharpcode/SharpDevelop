// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

namespace HelloWorld
{
	class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main()
		{            
			Application.SetCompatibleTextRenderingDefault(true);
			Application.EnableVisualStyles();
			using (Form1 form = new Form1())
			{
				Application.Run(form);
			}
		}
	}
}
