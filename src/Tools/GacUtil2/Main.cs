//	GacUtil2
//	Copyright (c) 2004, Christoph Wille
//	All rights reserved.
//	
//	Redistribution and use in source and binary forms, with or without modification, are 
//	permitted provided that the following conditions are met:
//	
//	- Redistributions of source code must retain the above copyright notice, this list 
//	  of conditions and the following disclaimer.
//	
//	- Redistributions in binary form must reproduce the above copyright notice, this list
//	  of conditions and the following disclaimer in the documentation and/or other materials 
//	  provided with the distribution.
//	
//	- Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to 
//	  endorse or promote products derived from this software without specific prior written 
//	  permission.
//	
//	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS 
//	OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
//	AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//	CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//	DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
//	DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
//	IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
//	OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Text;
using RJH.CommandLineHelper;  // http://www.codeproject.com/csharp/commandlineparser.asp

namespace GacUtil2
{
	public class GacUtil2Switches
	{
	    private string _Install = "";
		private string _Remove = "";
		private bool _ShowHelp = false;
	
	    [CommandLineSwitch("i","/i <assembly name>")]
	    public string Install
	    {
	        get { return _Install; }
	        set { _Install = value; }
	    }
	    
	    
	    [CommandLineSwitch("u","/u <assembly name>")]
	    public string Remove
	    {
	        get { return _Remove; }
	        set { _Remove = value; }
	    }
	    
	    [CommandLineSwitch("?","/?")]
	    public bool ShowHelp
	    {
	        get { return _ShowHelp; }
	        set { _ShowHelp = value; }
	    }
	    
	    public void PrintHelp()
	    {
	    	StringBuilder stb = new StringBuilder();
	    	stb.Append(Environment.NewLine);
	    	
	    	// TODO: implement the help text for this application
	    }
	}
	
	class MainClass
	{
		// NOTE: Only switches /i and /u are supported
		public static void Main(string[] args)
		{
			GacUtil2Switches switches = new GacUtil2Switches();
			Parser parser = new Parser( System.Environment.CommandLine, switches);
        	parser.Parse();
			
			if (switches.ShowHelp)
			{
				switches.PrintHelp();
				return;
			}
			
			if (switches.Install != "")
			{
				try
				{
					AssemblyCache.Install(switches.Install);
					Console.WriteLine("Assembly installed successfully");
				}
				catch(Exception e)
				{
					Console.WriteLine("Error during install: " + e.Message);
				}
			}
			else if (switches.Remove != "")
			{
				try
				{
					AssemblyCache.Uninstall(switches.Remove);
					Console.WriteLine("Assembly removed successfully");
				}
				catch(Exception e)
				{
					Console.WriteLine("Error during removal of assembly from GAC: " + e.Message);
				}			
			}
			else
			{
				Console.WriteLine("No valid parameters specified");
			}	
		}
	}
}
