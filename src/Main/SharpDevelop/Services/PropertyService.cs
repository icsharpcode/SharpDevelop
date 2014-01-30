// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Threading;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop
{
	class PropertyService : PropertyServiceImpl
	{
		DirectoryName dataDirectory;
		DirectoryName configDirectory;
		FileName propertiesFileName;

		public PropertyService(DirectoryName configDirectory, DirectoryName dataDirectory, string propertiesName)
			: base(LoadPropertiesFromStream(configDirectory.CombineFile(propertiesName + ".xml")))
		{
			this.dataDirectory = dataDirectory;
			this.configDirectory = configDirectory;
			propertiesFileName = configDirectory.CombineFile(propertiesName + ".xml");
		}
		
		public override DirectoryName ConfigDirectory {
			get {
				return configDirectory;
			}
		}
		
		public override DirectoryName DataDirectory {
			get {
				return dataDirectory;
			}
		}
		static Properties LoadPropertiesFromStream(FileName fileName)
		{
			if (!File.Exists(fileName)) {
				return new Properties();
			}
			try {
				using (LockPropertyFile()) {
					return Properties.Load(fileName);
				}
			} catch (XmlException ex) {
				SD.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			} catch (IOException ex) {
				SD.MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			return new Properties();
		}
		
		public override void Save()
		{
			using (LockPropertyFile()) {
				this.MainPropertiesContainer.Save(propertiesFileName);
			}
		}
		
		/// <summary>
		/// Acquires an exclusive lock on the properties file so that it can be opened safely.
		/// </summary>
		static IDisposable LockPropertyFile()
		{
			Mutex mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
			mutex.WaitOne();
			return new CallbackOnDispose(
				delegate {
					mutex.ReleaseMutex();
					mutex.Close();
				});
		}
		
		FileName GetExtraFileName(string key)
		{
			return configDirectory.CombineFile("preferences/" + key.GetStableHashCode().ToString("x8") + ".xml");
		}
		
		public override Properties LoadExtraProperties(string key)
		{
			var fileName = GetExtraFileName(key);
			using (LockPropertyFile()) {
				if (File.Exists(fileName))
					return Properties.Load(fileName);
				else
					return new Properties();
			}
		}
		
		public override void SaveExtraProperties(string key, Properties p)
		{
			var fileName = GetExtraFileName(key);
			using (LockPropertyFile()) {
				Directory.CreateDirectory(fileName.GetParentDirectory());
				p.Save(fileName);
			}
		}
	}
}
