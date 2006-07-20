//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Reflection;
	using System.Resources;

	public sealed class ResourcesHelper
	{
		static ResourcesHelper instance = new ResourcesHelper();

		public static string GetString(string keyName)
		{
			return instance.StringResourceHelper.GetString(keyName);
		}

		ResourceManager StringResourceHelper;

		ResourcesHelper()
		{
			StringResourceHelper = new ResourceManager("Help2Register.String", GetType().Assembly);
		}
	}
}
