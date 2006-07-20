//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Globalization;
	using HtmlHelp2Registration.ItemClasses;
	using MSHelpServices;

	public sealed class MergeNamespace
	{
		MergeNamespace()
		{
		}

		public static void CallMerge(string namespaceName)
		{
			try
			{
				HxSessionClass session = new HxSessionClass();
				session.Initialize(string.Format(CultureInfo.InvariantCulture, "ms-help://{0}", namespaceName), 0);

				// Next lesson about the Help 2.0 API: You have to wait until
				// "MergeIndex" is ready. This is a console tool, so ... But
				// if you want to do it with a GUI tool, you have to use a
				// thread or something. I used a thread in my Delphi version.

				IHxCollection collection  = session.Collection;
				collection.MergeIndex();
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
		}
	}
}
