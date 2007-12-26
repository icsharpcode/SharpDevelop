// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Text.RegularExpressions;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.NAnt
{
	/// <summary>
	/// Utility class that parses the console output from NAnt.
	/// </summary>
	public class NAntOutputParser
	{
		NAntOutputParser()
		{
		}
		
		/// <summary>
		/// Parses the NAnt console output and extracts a set of 
		/// Tasks.
		/// </summary>
		/// <param name="output">The NAnt console output to parse.</param>
		/// <returns>A <see cref="TaskCollection"/>.</returns>
		public static TaskCollection Parse(string output)
		{
			TaskCollection tasks = new TaskCollection();
			
			output = output.Replace("\r\n", "\n");
			
			// Look for errors on a per line basis.
			Task task = null;
			StringReader reader = new StringReader(output);
			while (reader.Peek() != -1) {
				
				string currentLine = reader.ReadLine();
				if (currentLine.StartsWith("BUILD FAILED")) {
				    break;
				}
				
				task = ParseLine(currentLine);
				if (task != null) {
					tasks.Add(task);
				}
			}
			reader.Close();
						
			// Look for multiline build errors.
			task = ParseMultilineBuildError(output);
			if (task != null) {
				tasks.Add(task);
			}
			
			// Look for NAnt build failed.
			task = ParseNAntBuildFailedError(output);
			if (task != null) {
				tasks.Add(task);
			}
				
			return tasks;
		}
		
		/// <summary>
		/// Parses a single line of text looking for errors.
		/// </summary>
		/// <param name="textLine">A NAnt output line.</param>
		/// <returns>A <see cref="Task"/> if the line contains an error or 
		/// a warning; otherwise <see langword="null"/>.</returns>
		static Task ParseLine(string textLine)
		{
			Task task = null;
			
			task = ParseCSharpError(textLine);
			
			if (task == null) {
				task = ParseVBError(textLine);
			}
			
			if (task == null) {
				task = ParseFatalError(textLine);
			}
			
			if (task == null) {
				task = ParseVBFatalError(textLine);
			}
			
			if (task == null) {
				task = ParseNAntWarning(textLine);
			}
			if (task == null) {
				task = ParseNAntError(textLine);
			}
			
			return task;
		}
		
		/// <summary>
		/// Looks for errors of the form 
		/// "C:/MyProject/MyProject.build(40,3): error CS1000: An error occurred."
		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseCSharpError(string textLine)
		{
			Task task = null;
			Match match = Regex.Match(textLine, @"^.*?(\w+:[/\\].*?)\(([\d]*),([\d]*)\): (.*?) (.*?): (.*?)$");
			if (match.Success) {
				try	{
					// Take off 1 for line/col since SharpDevelop is zero index based.
					int line = Convert.ToInt32(match.Groups[2].Value) - 1;
					int col = Convert.ToInt32(match.Groups[3].Value) - 1;                     
					string description = String.Concat(match.Groups[6].Value, " (", match.Groups[5], ")");
					
					TaskType taskType = TaskType.Error;
					if (String.Compare(match.Groups[4].Value, "warning", true) == 0) {
						taskType = TaskType.Warning;
					}
					task = new Task(match.Groups[1].Value, description, col, line, taskType);
				} catch (Exception) {
					// Ignore.
				}
			}
			
			return task;
		}
		
		/// <summary>
		/// Looks for errors of the form 
		/// "C:/MyProject/MyProject.build(40,3): ifnot is deprecated."
		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseNAntError(string textLine)
		{
			Task task = null;
			
			Match match = Regex.Match(textLine, @"^.*?(\w+:[/\\].*?)\(([\d]*),([\d]*)\): (.*?)$");
			if (match.Success) {
				try	{
					// Take off 1 for line/col since SharpDevelop is zero index based.
					int line = Convert.ToInt32(match.Groups[2].Value) - 1;
					int col = Convert.ToInt32(match.Groups[3].Value) - 1;                     
					
					task = new Task(match.Groups[1].Value, match.Groups[4].Value, col, line, TaskType.Warning);
				} catch (Exception) {
					// Ignore.
				}
			}

			return task;
		}
		
		/// <summary>
		/// Looks for errors of the form 
		/// "C:/MyProject/MyProject.build(40): error CS1000: An error occurred."
		/// </summary>
		/// <remarks>
		/// This should handle C++ errors too.</remarks>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseVBError(string textLine)
		{
			Task task = null;
			Match match = Regex.Match(textLine, @"^.*?(\w+:[/\\].*?)\(([\d]*)\) : (.*?) (.*?): (.*?)$");
			if (match.Success) {
				try	{
					// Take off 1 for line/col since SharpDevelop is zero index based.
					int line = Convert.ToInt32(match.Groups[2].Value) - 1;
					string description = String.Concat(match.Groups[5].Value, " (", match.Groups[4], ")");
					
					TaskType taskType = TaskType.Error;
					if (String.Compare(match.Groups[3].Value, "warning", true) == 0) {
						taskType = TaskType.Warning;
					}
					task = new Task(match.Groups[1].Value, description, 0, line, taskType);
				} catch (Exception) {
					// Ignore.
				}
			}
			
			return task;
		}
		
		/// <summary>
		/// Looks for errors of the form 
		/// "fatal error CS00042: An error occurred."
		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseFatalError(string textLine)
		{
			Task task = null;
			Match match = Regex.Match(textLine, @"^.*?(fatal error .*?: .*?)$");
			if (match.Success) {
				try	{
					task = new Task(String.Empty, match.Groups[1].Value, 0, 0, TaskType.Error);
				} catch (Exception) {
					// Ignore.
				}
			}
			
			return task;			
		}
		
		/// <summary>
		/// Looks for errors of the form 
		/// "vbc : error BC31019: Unable to write to output file."
		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseVBFatalError(string textLine)
		{
			Task task = null;
			Match match = Regex.Match(textLine, @"^.*?vbc : error (.*?): (.*?)$");
			if (match.Success) {
				try	{
					string description = String.Concat(match.Groups[2].Value, " (", match.Groups[1].Value, ")");
					task = new Task(String.Empty, description, 0, 0, TaskType.Error);
				} catch (Exception) {
					// Ignore.
				}
			}
			
			return task;			
		}	
		
		/// <summary>
		/// Looks for errors of the form 
		/// "fatal error CS00042: An error occurred."
		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if a warning was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseNAntWarning(string textLine)
		{
			Task task = null;
			Match match = Regex.Match(textLine, @"^.*?(Read-only property .*? cannot be overwritten.)$");
			if (match.Success) {
				try	{
					task = new Task(String.Empty, match.Groups[1].Value, 0, 0, TaskType.Warning);
				} catch (Exception) {
					// Ignore.
				}
			}
			
			return task;			
		}
		
		/// <summary>
		/// Looks for errors of the form 
		/// "BUILD FAILED"
		/// "[csc] C:/foo/foo.cs(5,5):"
		/// "Something bad happened."
		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns>
		static Task ParseNAntBuildFailedError(string output)
		{
			Task task = null;
			
			Match match = Regex.Match(output, @"^BUILD FAILED.*?$\n^$\n^(\w+:[/\\].*?)\(([\d]*),([\d]*)\):$\n^(.*?)$\n^(.*?)$", RegexOptions.Multiline);
			
			if (match.Success) {
				
				try	{
					// Take off 1 for line/col since SharpDevelop is zero index based.
					int line = Convert.ToInt32(match.Groups[2].Value) - 1;
					int col = Convert.ToInt32(match.Groups[3].Value) - 1;
					string description = String.Concat(match.Groups[4], Environment.NewLine, match.Groups[5]);
					task = new Task(match.Groups[1].Value, description, col, line, TaskType.Error);
				} catch(Exception) { };
			} else {
				
				match = Regex.Match(output, @"^BUILD FAILED$\n^$\n^(.*?)$", RegexOptions.Multiline);
				
				if (match.Success) {
					task = new Task(String.Empty, match.Groups[1].Value, 0, 0, TaskType.Error);
				}
			}	
			
			return task;
		}
		
		/// <summary>
		/// Parses errors of the form.
		/// "[delete] C:\foo\foo.build(94,5):"
        /// "[delete] Cannot delete directory 'C:\foo\bin'. The directory does not exist."
 		/// </summary>
		/// <param name="textLine">The line of text to parse.</param>
		/// <returns>A <see cref="Task"/> if an error was found; otherwise
		/// <see langword="null"/>.</returns> 
		static Task ParseMultilineBuildError(string output)
		{
			Task task = null;
			
			Match match = Regex.Match(output, @"^.*?\[delete\] (\w+:[/\\].*?)\(([\d]*),([\d]*)\):$\n^.*?\[delete\] (.*?)$", RegexOptions.Multiline);
			
			if (match.Success) {
				
				try	{
					// Take off 1 for line/col since SharpDevelop is zero index based.
					int line = Convert.ToInt32(match.Groups[2].Value) - 1;
					int col = Convert.ToInt32(match.Groups[3].Value) - 1;
					string description = String.Concat(match.Groups[4]);
					task = new Task(match.Groups[1].Value, description, col, line, TaskType.Error);
				} catch(Exception) { };
			}
			
			return task;
		}		
	}
}
