/*
 * User: dickon
 * Date: 17/09/2006
 * Time: 09:33
 * 
 */

using System;
using ICSharpCode.Core;


namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of FormsArtefactFactories.
	/// </summary>
	public class FormsArtefactFactories
	{
		public FormsArtefactFactories()
		{
		}
		
		public static FormsArtefactFactory GetFactory(string invariantName)
		{
			LoggingService.Debug("Looking for FormsArtefactFactory for: " + invariantName);
			
			// to test this base it on hardcoded strings for the type of the factory
			
			// TODO: drive this from the AddIn tree
			switch (invariantName)
			{
				case "System.Data.OracleClient":
					Type type = Type.GetType("SharpDbTools.Oracle.Forms.OracleFormsArtefactFactory, OracleDbToolsProvider");
					FormsArtefactFactory factory = (FormsArtefactFactory)Activator.CreateInstance(type);
					LoggingService.Debug("Found FormsArtefactFactory for: " + invariantName);
					return factory;
				default:
					LoggingService.Debug("Failed to find FormsArtefactFactory for: " + invariantName);
					throw new ArgumentException("There is no FormsArtefactFactory for invariant name: " + 
					                           invariantName);
			}
			
			
			// TODO: retrieve the relevant factory from file-base config
			// TODO: >>>>>>>>>>>>>>>>>>> NEXT: retrieve an XML element with mapping
			// from invariant name to class name of FormsArtefactProvider
			// options include specific config file or use of .net process config.
			
			// 1. load the config file for DbTools FormsArtefacts
			// 2. find the string name of the type of the FormsArtefactsFactory implementation
			// corresponding to invariatName
			// 3. use Type.GetType to create an instance of it and return it to the caller
		}
	}
}
