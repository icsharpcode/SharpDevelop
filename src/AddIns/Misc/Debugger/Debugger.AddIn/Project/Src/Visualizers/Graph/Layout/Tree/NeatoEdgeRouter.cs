// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Text;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Given <see cref="PositionedGraph" /> with positions of nodes, calculates positions of edges.
	/// </summary>
	public class NeatoEdgeRouter
	{
		public NeatoEdgeRouter()
		{
		}
		
		public PositionedGraph CalculateEdges(PositionedGraph graphWithNodesPositioned)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.FileName = @"D:\__prog__\Graphviz\Graphviz2.22\bin\neato.exe";
			//p.StartInfo.Arguments = arguments.ToString();
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;
			//p.EnableRaisingEvents = true;
			p.Exited += delegate {
					p.Dispose();
			};
     		/*p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					SvnClient.Instance.SvnCategory.AppendText(e.Data);
			};
			p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					SvnClient.Instance.SvnCategory.AppendText(e.Data);
			};*/
     		
     		// convert PosGraph to .dot string
     		// assign unique ids to edges, build map id->edge
			
     		p.Start();
			p.StandardInput.Write("digraph G {}");
			p.StandardInput.Flush();
			p.StandardInput.Close();
			
			StringBuilder output = new StringBuilder();
			while(true)
			{
				string line = p.StandardOutput.ReadLine();
				output.Append(line);
				if (line == "}")
				{
					break;
				}
			}
			
			string layoutedGraph = output.ToString();
			
			// parse edge positions
			// lookup edge in id->edge map, set position
			 
			PositionedGraph posGraph = graphWithNodesPositioned;
			//posGraph.Edges
			
			return posGraph;
		}
	}
}
