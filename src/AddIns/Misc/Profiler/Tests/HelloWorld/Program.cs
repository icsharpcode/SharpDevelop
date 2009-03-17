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