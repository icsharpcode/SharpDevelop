//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration
{
	using System;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using HtmlHelp2Registration.HelperClasses;
	using HtmlHelp2Registration.ItemClasses;

	class HtmlHelp2RegistrationTool
	{
		#region [STAThread]
		[STAThread]
		public static int Main(string[] args)
		{
			HtmlHelp2RegistrationTool register = new HtmlHelp2RegistrationTool();
			return register.Run(args);
		}
		#endregion

		private bool showLogo = true;
		private bool showHelp;
		private bool beQuiet;
		private bool errorCodes;
		private string actionParam = string.Empty;
		private string xmlFilename = string.Empty;
		private string xpathSequence = string.Empty;
		private string[] invalidProcesses = new string[] { "dexplore.exe", "sharpdevelop.exe" };

		public int Run(string[] args)
		{
			this.ParseCommandLine(args);

			if (this.showLogo)
			{
				Assembly me = Assembly.GetExecutingAssembly();
				Console.WriteLine("Help 2.0 Registration Utility v{0}", me.GetName().Version.ToString());
				Console.WriteLine("Copyright (c) 2005 Mathias Simmack. All rights reserved.");
				Console.WriteLine();
			}

			if (this.showHelp || args.Length == 0)
			{
				Assembly me = Assembly.GetExecutingAssembly();
				Console.WriteLine(ResourcesHelper.GetString("RegisterToolCommandLineOptions"),
				                  me.GetName().Name);

				return (this.errorCodes)?1:0;
			}

			// Error code 2 is obsolete. It was set in my Delphi version if XML was not installed

			if (!ApplicationHelpers.IsClassRegistered("{31411198-A502-11D2-BBCA-00C04F8EC294}"))
			{
				if(!this.beQuiet) Console.WriteLine(ResourcesHelper.GetString("ErrorNoHelp2Environment"));
				return (this.errorCodes)?3:0;
			}

			if (!ApplicationHelpers.IsThisUserPrivileged())
			{
				if(!this.beQuiet) Console.WriteLine(ResourcesHelper.GetString("ErrorInvalidPrivileges"));
				return (this.errorCodes)?4:0;
			}

			if (this.actionParam != "/r" && this.actionParam != "/u" && this.actionParam != "+r" &&
			    this.actionParam != "-r" && this.actionParam != "+p" && this.actionParam != "-p")
			{
				if(!this.beQuiet) Console.WriteLine(ResourcesHelper.GetString("ErrorInvalidCommandLine"),
				                                    this.actionParam);
				return (this.errorCodes)?5:0;
			}

			if (string.IsNullOrEmpty(this.xmlFilename) || !File.Exists(this.xmlFilename))
			{
				if(!this.beQuiet) Console.WriteLine(ResourcesHelper.GetString("ErrorInvalidXmlFile"),
				                                    this.xmlFilename);
				return (this.errorCodes)?6:0;
			}

			if (!XmlValidator.XsdFileDoesExist)
			{
				if(!this.beQuiet) Console.WriteLine(ResourcesHelper.GetString("ErrorCannotValidateXmlFile"),
				                                    this.xmlFilename);
				return (this.errorCodes)?7:0;
			}
			if (!XmlValidator.Validate(this.xmlFilename, this.beQuiet))
			{
				// get a message from the validator class
				return (this.errorCodes)?7:0;
			}

			ApplicationHelpers.KillIllegalProcesses(this.invalidProcesses);

			if (this.actionParam == "/r" || this.actionParam == "+r") this.DoHelpStuff(true);
			if (this.actionParam == "/r" || this.actionParam == "+p") this.DoPluginStuff(true);
			if (this.actionParam == "/u" || this.actionParam == "-p") this.DoPluginStuff(false);
			if (this.actionParam == "/u" || this.actionParam == "-r") this.DoHelpStuff(false);

			return 0;
		}

		private void ParseCommandLine(string[] args)
		{
			if (args.Length == 0)
			{
				return;
			}
			
			this.actionParam = args[0].ToLower(CultureInfo.InvariantCulture);
			this.xmlFilename = args[args.Length - 1];

			for (int i = 1; i < args.Length - 1; i++)
			{
				string arg = args[i];

				if (string.IsNullOrEmpty(arg))
				{
					continue;
				}

				if ('-' == arg[0] || '/' == arg[0])
				{
					string parameter = arg.Substring(1);

					if ("nologo" == parameter)
					{
						this.showLogo = false;
					}
					if ("?" == parameter || "help" == parameter)
					{
						this.showHelp = true;
					}
					if ("quiet" == parameter || "q" == parameter)
					{
						this.beQuiet = true;
					}
					if (parameter.StartsWith("xpath:"))
					{
						this.xpathSequence = parameter.Substring(6);
					}
					if ("useerrorcodes" == parameter)
					{
						this.errorCodes = true;
					}
				}
			}
		}

		#region Action
		private void DoHelpStuff(bool register)
		{
			NamespacesItemClass namespaces =
				new NamespacesItemClass(this.xmlFilename, this.xpathSequence);

			if (!this.beQuiet)
			{
				namespaces.RegisterOrRemoveNamespace += this.RegisterOrRemoveNamespace;
				namespaces.RegisterOrRemoveHelpDocument += this.RegisterOrRemoveHelpDocument;
				namespaces.RegisterOrRemoveFilter += this.RegisterOrRemoveFilter;
				namespaces.RegisterOrRemovePlugin += this.RegisterOrRemovePlugin;
				namespaces.NamespaceMerge += this.NamespaceMerge;
			}
			if (register)
			{
				namespaces.Register();
			}
			else
			{
				namespaces.Unregister();
			}
		}

		private void DoPluginStuff(bool register)
		{
			PluginsItemClass plugins =
				new PluginsItemClass(this.xmlFilename, this.xpathSequence);

			if (!this.beQuiet)
			{
				plugins.RegisterOrRemovePlugin += this.RegisterOrRemovePlugin;
				plugins.NamespaceMerge += this.NamespaceMerge;
			}
			if (register)
			{
				plugins.Register();
			}
			else
			{
				plugins.Unregister();
			}
		}
		#endregion

		#region Events
		private void RegisterOrRemoveNamespace(object sender, CommonRegisterEventArgs e)
		{
			if (e.Register)
			{
				Console.WriteLine(ResourcesHelper.GetString("RegisterHelpNamespace"), e.Name);
			}
			else
			{
				Console.WriteLine(ResourcesHelper.GetString("RemoveHelpNamespace"), e.Name);
			}
		}

		private void RegisterOrRemoveHelpDocument(object sender, CommonRegisterEventArgs e)
		{
			if (e.Register)
			{
				Console.WriteLine(ResourcesHelper.GetString("RegisterHelpDocument"), e.Name);
			}
			else
			{
				Console.WriteLine(ResourcesHelper.GetString("RemoveHelpDocument"), e.Name);
			}
		}

		private void RegisterOrRemoveFilter(object sender, CommonRegisterEventArgs e)
		{
			if (e.Register)
			{
				Console.WriteLine(ResourcesHelper.GetString("RegisterHelpFilter"), e.Name);
			}
			else
			{
				Console.WriteLine(ResourcesHelper.GetString("RemoveHelpFilter"), e.Name);
			}
		}

		private void RegisterOrRemovePlugin(object sender, PluginRegisterEventArgs e)
		{
			if (e.Register)
			{
				Console.WriteLine(ResourcesHelper.GetString("RegisterHelpPlugin"), e.Child, e.Parent);
			}
			else
			{
				Console.WriteLine(ResourcesHelper.GetString("RemoveHelpPlugin"), e.Child, e.Parent);
			}
		}

		private void NamespaceMerge(object sender, MergeNamespaceEventArgs e)
		{
			Console.WriteLine(ResourcesHelper.GetString("MergeHelpNamespace"), e.Name);
		}
		#endregion
	}
}
