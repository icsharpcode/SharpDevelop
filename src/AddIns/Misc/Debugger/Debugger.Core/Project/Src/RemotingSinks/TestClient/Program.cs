using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Threading;

namespace CustomSinks
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Thread.CurrentThread.Name = "Client (STA)";

			Application.EnableVisualStyles();
			Application.Run(new FormClient());
		}
	}
}