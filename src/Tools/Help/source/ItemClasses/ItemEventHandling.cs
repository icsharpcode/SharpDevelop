//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.ItemClasses
{
	using System;

	public class CommonRegisterEventArgs : EventArgs
	{
		private readonly string name;
		private readonly bool register = true;

		public CommonRegisterEventArgs(string name, bool register)
		{
			this.name     = name;
			this.register = register;
		}

		public string Name
		{
			get { return this.name; }
		}

		public bool Register
		{
			get { return this.register; }
		}
	}

	public class PluginRegisterEventArgs : EventArgs
	{
		private readonly string parent;
		private readonly string child;
		private readonly bool register = true;

		public PluginRegisterEventArgs(string parent, string child, bool register)
		{
			this.parent   = parent;
			this.child    = child;
			this.register = register;
		}

		public string Parent
		{
			get { return this.parent; }
		}

		public string Child
		{
			get { return this.child; }
		}

		public bool Register
		{
			get { return this.register; }
		}
	}

	public class MergeNamespaceEventArgs : EventArgs
	{
		private readonly string name;

		public MergeNamespaceEventArgs(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return this.name; }
		}
	}

	public class LoggingEventArgs : EventArgs
	{
		private readonly string message;

		public LoggingEventArgs(string message)
		{
			this.message = message;
		}

		public string Message
		{
			get { return this.message; }
		}
	}
}
