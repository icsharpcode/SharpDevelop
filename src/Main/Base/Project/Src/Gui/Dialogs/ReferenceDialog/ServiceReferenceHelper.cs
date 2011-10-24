/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.10.2011
 * Time: 20:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Web.Services.Discovery;

using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of ServiceReferenceHelper.
	/// </summary>
	internal class ServiceReferenceHelper
	{
		private ServiceReferenceHelper()
		{
		}
		
		public static List <string>  AddMruList()
		{
			var list = new List<string>();
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
				if (key != null) {
					foreach (string name in key.GetValueNames()) {
						list.Add ((string)key.GetValue(name));
					}
				}
			} catch (Exception)
			{
			};
			return list;
		}
		
		
		public static ServiceDescriptionCollection GetServiceDescriptions(DiscoveryClientProtocol protocol)
		{
			ServiceDescriptionCollection services = new ServiceDescriptionCollection();
			protocol.ResolveOneLevel();
		
			foreach (DictionaryEntry entry in protocol.References) {
				ContractReference contractRef = entry.Value as ContractReference;
				if (contractRef != null) {
					services.Add(contractRef.Contract);
				}
			}
			return services;
		}
	}
}
