// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.IO;
using System.Text;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Encapsulates Neato.exe.
	/// </summary>
	public class NeatoProcess
	{
		private System.Diagnostics.Process neatoProcess;
		
		/// <summary>
		/// Starts neato.exe
		/// </summary>
		public static NeatoProcess Start()
		{
			System.Diagnostics.Process neatoProcess = new System.Diagnostics.Process();
			neatoProcess.StartInfo.FileName =  getNeatoExePath();
			neatoProcess.StartInfo.RedirectStandardInput = true;
			neatoProcess.StartInfo.RedirectStandardError = true;
			neatoProcess.StartInfo.RedirectStandardOutput = true;
			neatoProcess.StartInfo.UseShellExecute = false;
			neatoProcess.StartInfo.CreateNoWindow = true;
			// tell neato to use splines instead of straigt lines
			// output type = Graphviz's plain format
			neatoProcess.StartInfo.Arguments = " -Gsplines=true -Tplain";
			//p.EnableRaisingEvents = true;
			neatoProcess.Exited += delegate {
				neatoProcess.Dispose();
			};
			/*p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
			};
			p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
			};*/
			
			neatoProcess.Start();
			
			return new NeatoProcess(neatoProcess);
		}
		
		/// <summary>Creates new NeatoProcess.</summary>
		/// <param name="neatoProcess">Underlying neato.exe process</param>
		private NeatoProcess(System.Diagnostics.Process neatoProcess)
		{
			this.neatoProcess = neatoProcess;
		}
		
		/// <summary>Gets directory where Debugger.AddIn.dll resides</summary>
		private static string getCurrentAssemblyPath()
		{
			return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}
		
		private static string getNeatoExePath()
		{
			return Path.Combine(getCurrentAssemblyPath(), "neato.exe");
		}
		
		/// <summary>
		/// Passes given graph to neato and reads output.
		/// </summary>
		/// <param name="dotGraph">Graph in Graphviz dot format.</param>
		/// <returns>Same graph in Graphviz plain format with position information added.</returns>
		public string CalculatePositions(string dotGraph)
		{
			// write the input to neato.exe to a file for testing purposes
  			/*using (var writer = new StreamWriter(Path.Combine(getCurrentAssemblyPath(), "logIn.gv")))
			{
				writer.Write(dotGraph);
			}*/
			
			neatoProcess.StandardInput.Write(dotGraph);
			neatoProcess.StandardInput.Flush();
			neatoProcess.StandardInput.Close();
			
			StringBuilder output = new StringBuilder();
			while(true)
			{
				string line = neatoProcess.StandardOutput.ReadLine();
				if (line == null)
				{
					// happens if neato.exe is killed
					throw new DebuggerVisualizerException("Problem getting layout information from neato.exe");
				}
				if (line == "stop")
				{
					break;
				}
				output.AppendLine(line);
			}
			
			return output.ToString();
		}
	}
}
