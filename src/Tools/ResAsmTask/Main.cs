using System;
using System.Drawing;
using System.IO;
using System.Resources;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace ResAsmTask
{
	[TaskName("resasm")]
	public class ResAsmTask : Task
	{
		string  output;
		string  input = null;
		FileSet files = new FileSet();
		
		[TaskAttribute("output", Required=true)]
		public string Output {
			get {
				return output;
			}
			set {
				output = value;
			}
		}
		
		[TaskAttribute("input")]
		public string Input {
			get {
				return input;
			}
			set {
				input = value;
			}
		}
		
		[BuildElement("files")]
		public FileSet Files {
			get {
				return files;
			}
			set {
				files = value;
			}
		}
		
		protected override void ExecuteTask()
		{
			if (Files.BaseDirectory == null) {   
				Files.BaseDirectory = new DirectoryInfo(Project.BaseDirectory);
			}
			
			if (Files.FileNames.Count > 0) {
				string outputDirectory;
				if (Path.IsPathRooted(Output)) {
					outputDirectory = Output;
				} else {
					outputDirectory = Path.Combine(Project.BaseDirectory, Output);
				}
				
				if (!Directory.Exists(Path.GetDirectoryName(outputDirectory))) {
					Directory.CreateDirectory(Path.GetDirectoryName(outputDirectory));
				}
				
				ResourceWriter rw = new ResourceWriter(outputDirectory);
				foreach (string fileName in Files.FileNames) {
					Bitmap bitmap = new Bitmap(Path.Combine(Files.BaseDirectory.FullName, fileName));
					rw.AddResource(Path.GetFileNameWithoutExtension(fileName), bitmap);
				}
				rw.Generate();
				rw.Close();
			} else if (Input != null) {
				ResAsm.Assemble(Input, Output);
			}
		}
	}
}
