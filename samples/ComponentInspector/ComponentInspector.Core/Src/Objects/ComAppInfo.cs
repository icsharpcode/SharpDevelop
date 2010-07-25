// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Microsoft.Win32;

namespace NoGoop.Obj
{

	internal class ComAppInfo : BasicInfo
	{

		public Type Type
		{
			get
				{
					return null;
				}
		}

		internal ComAppInfo(RegistryKey classKey,
							String guidStr) : 
				base(classKey)
		{
			_infoType = "ApplId";

			// The AppId entries either use the GUID as their key
			// and the value is the description, or the exe file
			// is the key, and the Guid is found in the AppId value
			String guidValueStr = (String)classKey.GetValue("AppID");
			if (guidValueStr != null)
			{
				Name = guidStr;
				InitGuid(guidValueStr, new Guid(guidValueStr));
			}
			else
			{
				String defValue = (String)classKey.GetValue(null);
				if (defValue == null || defValue.Equals(""))
					Name = guidStr;
				else
					DocString = defValue;
				InitGuid(guidStr, new Guid(guidStr));
			}
		}


		public override void GetDetailText()
		{
			base.GetDetailText();
		}

		public override String ToString()
		{
			return base.ToString();
		}
	}
}
