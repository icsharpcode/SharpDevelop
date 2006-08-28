// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace UpdateSetupInfo
{
	/// <summary>
	/// Creates the SharpDevelop.Setup.wixproj.user file based on the 
	/// SharpDevelop.Setup.wixproj.user.template.
	/// </summary>
	class UpdateApplication
	{
		/// <summary>
		/// Path to the setup project relative to the UpdateSetupInfo.exe file.
		/// </summary>
		const string SetupProjectFolderRelativePath = @"..\..\..\Setup";
		
		/// <summary>
		/// Name of the setup template file.
		/// </summary>
		const string SetupTemplateFileName = "SharpDevelop.Setup.wixproj.user.template";
		
		/// <summary>
		/// Name of the setup project user file that will be generated.
		/// </summary>
		const string SetupProjectUserFileName = "SharpDevelop.Setup.wixproj.user";
		
		const int SetupTemplateFileNotFoundReturnCode = 1;
		const int UpdateSetupInfoExceptionReturnCode = 2;
		
		/// <summary>
		/// The full filename including path to the setup template file.
		/// </summary>
		string setupTemplateFullFileName;
	
		/// <summary>
		/// The full filename including path to the setup project user file that
		/// will be generated.
		/// </summary>
		string setupProjectUserFullFileName;
		
		/// <summary>
		/// The folder containing the UpdateSetupInfo application.
		/// </summary>
		string applicationFolder;
		
		/// <summary>
		/// The file that contains the last revision number used to update the
		/// template.
		/// </summary>
		string previousRevisionFileName;
		
		public UpdateApplication()
		{
			// Work out filenames.
			applicationFolder = Path.GetDirectoryName(GetType().Assembly.Location);
			string setupProjectFolder = Path.Combine(applicationFolder, SetupProjectFolderRelativePath);
			setupProjectFolder = Path.GetFullPath(setupProjectFolder);
			
			setupTemplateFullFileName = Path.Combine(setupProjectFolder, SetupTemplateFileName);
			setupProjectUserFullFileName = Path.Combine(setupProjectFolder, SetupProjectUserFileName);
			previousRevisionFileName = Path.Combine(setupProjectFolder, @"bin\REVISION");
			
			// Set current directory to a folder that is in the repository.
			Environment.CurrentDirectory = setupProjectFolder;
		}
		
		public static int Main(string[] args)
		{
			try {
				UpdateApplication app = new UpdateApplication();
				return app.Run();
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				return UpdateApplication.UpdateSetupInfoExceptionReturnCode;
			}
		}
		
		public int Run()
		{			
			// Read setup template contents.
			if (!SetupTemplateFileExists) {
				Console.WriteLine(String.Concat(SetupTemplateFileName, " not found. Unable to update setup information."));
				return SetupTemplateFileNotFoundReturnCode;
			}
			string template = ReadSetupTemplate();
						
			// Get current revision.
			string currentRevision = GetCurrentRevision();
			
			if (RevisionExists(currentRevision)) {
				Console.WriteLine(String.Concat("Not updating setup guids for previously seen revision ", currentRevision));
				return 0;
			}
			
			// Populate setup template.
			template = PopulateSetupTemplate(template, currentRevision);
			
			// Create setup user file.
			SaveSetupUserFile(template);
			
			// Save revision.
			SaveRevision(currentRevision);
			
			return 0;
		}
		
		bool SetupUserFileExists {
			get {
				return File.Exists(setupProjectUserFullFileName);
			}
		}
		
		bool SetupTemplateFileExists {
			get {
				return File.Exists(setupTemplateFullFileName);
			}
		}
		
		string ReadSetupTemplate() {
			using (StreamReader reader = new StreamReader(setupTemplateFullFileName, true)) {
				return reader.ReadToEnd();
			}
		}
		
		string PopulateSetupTemplate(string template, string revision)
		{
			string updatedTemplate = template.Replace("$INSERTPRODUCTBUILDVERSION$", revision);
			updatedTemplate = updatedTemplate.Replace("$INSERTPRODUCTCODEGUID$", GetNewGuid());
			return updatedTemplate.Replace("$INSERTPACKAGECODEGUID$", GetNewGuid());
		}
		
		string GetNewGuid()
		{
			return Guid.NewGuid().ToString().ToUpperInvariant();
		}
		
		void SaveSetupUserFile(string contents)
		{
			using (StreamWriter writer = new StreamWriter(setupProjectUserFullFileName, false, Encoding.UTF8)) {
				writer.Write(contents);
			}
		}
		
		/// <summary>
		/// Writes the revision just used to generate the SharpDevelop.Setup.wixproj.user
		/// file so we do not regenerate the product guid for the same revision.
		/// </summary>
		void SaveRevision(string revision)
		{
			using (StreamWriter writer = new StreamWriter(previousRevisionFileName, false, Encoding.UTF8)) {
				writer.Write(revision);
			}
		}
		
		/// <summary>
		/// Code taken directly from UpdateAssemblyInfo and the paths slightly modified.
		/// </summary>
		/// <remarks>
		/// The product build version maps to the Subversion revision number.
		/// </remarks>
		string GetCurrentRevision()
		{
			string revisionNumber = null;
			string oldWorkingDir = Environment.CurrentDirectory;
			try {
				// Set working directory so msvcp70.dll and msvcr70.dll can be found
				Environment.CurrentDirectory = Path.Combine(applicationFolder, @"..\..\..\AddIns\Misc\SubversionAddIn\RequiredLibraries");
				Assembly asm = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "NSvn.Core.dll"));
				Type clientType = asm.GetType("NSvn.Core.Client");
				object clientInstance = Activator.CreateInstance(clientType);
				object statusInstance = clientType.InvokeMember("SingleStatus",
				                                                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				                                                null, clientInstance,
				                                                new object[] { oldWorkingDir });
				Type statusType = statusInstance.GetType();
				object entryInstance = statusType.InvokeMember("Entry",
				                                               BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public,
				                                               null, statusInstance, new object[0]);
				Type entryType = entryInstance.GetType();
				int revision = (int)entryType.InvokeMember("Revision",
				                                           BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public,
				                                           null, entryInstance, new object[0]);
				revisionNumber = revision.ToString();
			} catch (Exception e) {
				Console.WriteLine("Reading revision number with NSvn failed: " + e.ToString());
			} finally {
				Environment.CurrentDirectory = oldWorkingDir;
			}
			if (revisionNumber == null || revisionNumber.Length == 0 || revisionNumber == "0") {
				revisionNumber = ReadCurrentRevisionFromFile();
			}
			if (revisionNumber == null || revisionNumber.Length == 0 || revisionNumber == "0") {
				throw new ApplicationException("Error reading revision number");
			}
			return revisionNumber;
		}
		
		string ReadCurrentRevisionFromFile()
		{
			using (StreamReader reader = new StreamReader(Path.Combine(applicationFolder, @"..\..\..\..\REVISION"))) {
				return reader.ReadLine();
			}
		}
		
		/// <summary>
		/// Checks that the current revision matches the revision last used to
		/// update the SharpDevelop.Setup.wixproj.user file.
		/// </summary>
		bool RevisionExists(string currentRevision)
		{
			// Read previous revision.
			string previousRevision = ReadPreviousRevision();
			if (previousRevision != null) {
				return previousRevision == currentRevision;
			}
			return false;
		}
		
		/// <summary>
		/// Reads the previous revision number from the Setup\bin\REVISION file.
		/// </summary>
		string ReadPreviousRevision()
		{
			if (File.Exists(previousRevisionFileName)) {
				using (StreamReader reader = new StreamReader(previousRevisionFileName, true)) {
					return reader.ReadLine();
				}
			}
			return null;
		}
	}
}
