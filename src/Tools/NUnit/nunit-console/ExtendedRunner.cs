// This version of NUnit-console is modified to support:
// 1) Writing all tests results to a file as the test results are known.

using System;
using System.IO;
using System.Reflection;
using NUnit.Core;
using NUnit.Util;


namespace NUnit.ConsoleRunner
{
	/// <summary>
	/// Modified version of NUnit's Runner class to support the ExtendedConsoleOptions
	/// and the ExtendedConsoleUi class.
	/// </summary>
	public class Runner
	{
		[STAThread]
		public static int Main(string[] args)
		{
			NTrace.Info( "NUnit-console.exe starting" );

			ExtendedConsoleOptions options = new ExtendedConsoleOptions(args);
			
			if(!options.nologo)
				WriteCopyright();

			if(options.help)
			{
				options.Help();
				return ConsoleUi.OK;
			}
			
			if(options.NoArgs) 
			{
				Console.Error.WriteLine("fatal error: no inputs specified");
				options.Help();
				return ConsoleUi.OK;
			}
			
			if(!options.Validate())
			{
				foreach( string arg in options.InvalidArguments )
					Console.Error.WriteLine("fatal error: invalid argument: {0}", arg );
				options.Help();
				return ConsoleUi.INVALID_ARG;
			}

			// Add Standard Services to ServiceManager
			ServiceManager.Services.AddService( new SettingsService() );
			ServiceManager.Services.AddService( new DomainManager() );
			//ServiceManager.Services.AddService( new RecentFilesService() );
			//ServiceManager.Services.AddService( new TestLoader() );
			ServiceManager.Services.AddService( new AddinRegistry() );
			ServiceManager.Services.AddService( new AddinManager() );
			// TODO: Resolve conflict with gui testagency when running
			// console tests under the gui.
			//ServiceManager.Services.AddService( new TestAgency() );

			// Initialize Services
			ServiceManager.Services.InitializeServices();

			try
			{
				ExtendedConsoleUi consoleUi = new ExtendedConsoleUi();
				return consoleUi.Execute( options );
			}
			catch( FileNotFoundException ex )
			{
				Console.WriteLine( ex.Message );
				return ConsoleUi.FILE_NOT_FOUND;
			}
			catch( Exception ex )
			{
				Console.WriteLine( "Unhandled Exception:\n{0}", ex.ToString() );
				return ConsoleUi.UNEXPECTED_ERROR;
			}
			finally
			{
				if(options.wait)
				{
					Console.Out.WriteLine("\nHit <enter> key to continue");
					Console.ReadLine();
				}

				NTrace.Info( "NUnit-console.exe terminating" );
			}

		}

		private static void WriteCopyright()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			System.Version version = executingAssembly.GetName().Version;

			string productName = "NUnit";
			string copyrightText = "Copyright (C) 2002-2007 Charlie Poole.\r\nCopyright (C) 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov.\r\nCopyright (C) 2000-2002 Philip Craig.\r\nAll Rights Reserved.";

			object[] objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			if ( objectAttrs.Length > 0 )
				productName = ((AssemblyProductAttribute)objectAttrs[0]).Product;

			objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			if ( objectAttrs.Length > 0 )
				copyrightText = ((AssemblyCopyrightAttribute)objectAttrs[0]).Copyright;

			Console.WriteLine(String.Format("{0} version {1}", productName, version.ToString(3)));
			Console.WriteLine(copyrightText);
			Console.WriteLine();

			Console.WriteLine( "Runtime Environment - " );
			RuntimeFramework framework = RuntimeFramework.CurrentFramework;
			Console.WriteLine( string.Format("   OS Version: {0}", Environment.OSVersion ) );
			Console.WriteLine( string.Format("  CLR Version: {0} ( {1} )",
				Environment.Version,  framework.GetDisplayName() ) );

			Console.WriteLine();
		}
	}
}
