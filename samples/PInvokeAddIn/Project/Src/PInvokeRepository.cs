// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.PInvokeAddIn
{
	/// <summary>
	/// Represents the repository of pinvoke information.
	/// </summary>
	/// <remarks>
	/// All the main data is stored at http://www.pinvoke.net.  
	/// This class contains function names and module names for the drop 
	/// down lists read from the "signatures.xml" config file.
	/// </remarks>
	public sealed class PInvokeRepository
	{
		static string[] functionNames;
		static string[] moduleNames;
		static PInvokeRepository repository;
		
		PInvokeRepository()
		{
		}
		
		public static PInvokeRepository Instance
		{
			get {
				if (repository == null) {
					repository = new PInvokeRepository();
				}
				return repository;
			}
		}
		
		public string[] GetSupportedLanguages()
		{
			return new string[] {"C#", "VB"};
		}
		
		public string[] GetFunctionNames()
		{
			if (functionNames == null) {
				ReadConfig();
			}
			
			return functionNames;
		}
		
		public string[] GetModuleNames()
		{
			if (moduleNames == null) {
				ReadConfig();
			}
			
			return moduleNames;
		}
		
		/// <summary>
        /// Gets the folder where this assembly was loaded from.
        /// </summary>
        /// <returns>The folder where this assembly was loaded.</returns>
        string GetAssemblyFolder()
        {
        	Assembly assembly = GetType().Assembly;
        	
        	string assemblyFilename = assembly.CodeBase.Replace("file:///", "");
        	string folder = Path.GetDirectoryName(assemblyFilename);
        
        	return folder;
        }  
        
        void ReadConfig()
        {
        	string configFile = Path.Combine(GetAssemblyFolder(), "signatures.xml");
        	
        	XmlDocument doc = new XmlDocument();
        	doc.Load(configFile);
        	
        	List<string> moduleList = new List<string>();
        	List<string> functionList = new List<string>();
        	
        	foreach(XmlElement moduleElement in doc.DocumentElement.SelectNodes("//module"))
        	{
        		XmlAttribute moduleName = (XmlAttribute)moduleElement.SelectSingleNode("@name");
        		moduleList.Add(moduleName.Value);
        		
        		foreach(XmlAttribute functionName in moduleElement.SelectNodes("function/@name"))
        		{
        			functionList.Add(functionName.Value);
        		}
        	}

        	moduleNames = new string[moduleList.Count];
        	moduleList.Sort();
        	moduleList.CopyTo(moduleNames);
        	
        	functionNames = new string[functionList.Count];
        	functionList.Sort();
        	functionList.CopyTo(functionNames);
        }
	}
}
