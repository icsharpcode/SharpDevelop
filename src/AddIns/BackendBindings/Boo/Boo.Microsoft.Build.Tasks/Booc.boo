#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Microsoft.Build.Tasks

import Microsoft.Build.Framework
import Microsoft.Build.Tasks
import Microsoft.Build.Utilities
import System
import System.Diagnostics
import System.IO
import System.Globalization
import System.Text.RegularExpressions
import System.Threading

class Booc(ManagedCompiler):
"""
Represents the Boo compiler MSBuild task.

Authors:
	Sorin Ionescu (sorin.ionescu@gmail.com)
"""
	Pipelines:
	"""
	Gets/sets the aditional pipelines to add to the compiler process.
	"""
		get:
			return Bag['Pipelines'] as (string)
		set:
			Bag['Pipelines'] = value
	
	Verbosity:
	"""
	Gets/sets the verbosity level.
	"""
		get:
			return Bag['Verbosity'] as string
		set:
			Bag['Verbosity'] = value
	
	Culture:
	"""
	Gets/sets the culture.
	"""
		get:
			return Bag['Culture'] as string
		set:
			Bag['Culture'] = value
	
	SourceDirectory:
	"""
	Gets/sets the source directory.
	"""
		get:
			return Bag['Source Directory'] as string
		set:
			Bag['Source Directory'] = value
	
	ToolName:
	"""
	Gets the tool name.
	"""
		get:
			return "Booc.exe"
	
	override def Execute():
	"""
	Executes the task.
	
	Returns:
		true if the task completed successfully; otherwise, false.
	"""
		boocCommandLine = CommandLineBuilderExtension()
		AddResponseFileCommands(boocCommandLine)
		
		warningPattern = regex(
			'^(?<file>.*?)\\((?<line>\\d+),(?<column>\\d+)\\):' +
				' (?<code>BCW\\d{4}): WARNING: (?<message>.*)$',
			RegexOptions.Compiled)
		# Captures the file, line, column, code, and message from a BOO warning
		# in the form of: Program.boo(1,1): BCW0000: WARNING: This is a warning.
		
		errorPattern = regex(
			'^(((?<file>.*?)\\((?<line>\\d+),(?<column>\\d+)\\): )?' +
				'(?<code>BCE\\d{4})|(?<errorType>Fatal) error):' +
				'( Boo.Lang.Compiler.CompilerError:)?' + 
				' (?<message>.*?)($| --->)',
			RegexOptions.Compiled |
				RegexOptions.ExplicitCapture |
				RegexOptions.Multiline)
		/* 
		 * Captures the file, line, column, code, error type, and message from a
		 * BOO error of the form of:
		 * 1. Program.boo(1,1): BCE0000: This is an error.
		 * 2. Program.boo(1,1): BCE0000: Boo.Lang.Compiler.CompilerError:
		 *    	This is an error. ---> Program.boo:4:19: This is an error
		 * 3. BCE0000: This is an error.
		 * 4. Fatal error: This is an error.
		 *
		 * The second line of the following error format is not cought because 
		 * .NET does not support if|then|else in regular expressions,
		 * and the regex will be horrible complicated.  
		 * The second line is as worthless as the first line.
		 * Therefore, it is not worth implementing it.
		 *
		 * 	Fatal error: This is an error.
		 * 	Parameter name: format.
		 */
		
		buildSuccess = true
		outputLine = String.Empty
		errorLine = String.Empty
		readingDoneEvents = (ManualResetEvent(false), ManualResetEvent(false))
		
		boocProcessStartInfo = ProcessStartInfo(
			FileName: GenerateFullPathToTool(),
			Arguments: boocCommandLine.ToString(),
			ErrorDialog: false,
			CreateNoWindow: true,
			RedirectStandardError: true,
			RedirectStandardInput: false,
			RedirectStandardOutput: true,
			UseShellExecute: false)
		
		boocProcess = Process(StartInfo: boocProcessStartInfo)
		
		parseOutput = def(line as string):
			warningPatternMatch = warningPattern.Match(line)
			errorPatternMatch = errorPattern.Match(line)
		
			if warningPatternMatch.Success:
				Log.LogWarning(
					null,
					warningPatternMatch.Groups['code'].Value,
					null,
					warningPatternMatch.Groups['file'].Value,
					int.Parse(warningPatternMatch.Groups['line'].Value),
					int.Parse(warningPatternMatch.Groups['column'].Value),
					0,
					0,
					warningPatternMatch.Groups['message'].Value)
		
			elif errorPatternMatch.Success:					
				code = errorPatternMatch.Groups['code'].Value
				code = 'BCE0000' if string.IsNullOrEmpty(code)
				file = errorPatternMatch.Groups['file'].Value
				file = 'BOOC' if string.IsNullOrEmpty(file)
				
				try:
					lineNumber = int.Parse(
						errorPatternMatch.Groups['line'].Value,
						NumberStyles.Integer)
						
				except FormatException:
					lineNumber = 0

				try:
					columnNumber = int.Parse(
						errorPatternMatch.Groups['column'].Value,
						NumberStyles.Integer)
						
				except FormatException:
					columnNumber = 0

				Log.LogError(
					errorPatternMatch.Groups['errorType'].Value.ToLower(),
					code,
					null,
					file,
					lineNumber,
					columnNumber,
					0,
					0,
					errorPatternMatch.Groups['message'].Value)
		
				buildSuccess = false
		
			else:
				Log.LogMessage(MessageImportance.Low, line)
				
		readStandardOutput = def():
			while true:
				outputLine = boocProcess.StandardOutput.ReadLine()
		
				if outputLine:
					parseOutput(outputLine)
					
				else:
					readingDoneEvents[0].Set()
					break

		readStandardError = def():
			while true:
				errorLine = boocProcess.StandardError.ReadLine()

				if errorLine:
					parseOutput(errorLine)
					
				else:
					readingDoneEvents[1].Set()
					break
		
		standardOutputReadingThread = Thread(readStandardOutput as ThreadStart)	
		standardErrorReadingThread = Thread(readStandardError as ThreadStart)
		# Two threads are required (MSDN); otherwise, a deadlock WILL occur.
		
		try:
			boocProcess.Start()
			
			Log.LogMessage(
				MessageImportance.High,
				"${ToolName} ${boocProcess.StartInfo.Arguments}",
				null)
				
			standardOutputReadingThread.Start()
			standardErrorReadingThread.Start()
			
			WaitHandle.WaitAny((readingDoneEvents[0],))
			WaitHandle.WaitAny((readingDoneEvents[1],))
			# MSBuild runs on an STA thread, and WaitHandle.WaitAll()
			# is not supported.
			
		except e as Exception:
			Log.LogErrorFromException(e)
			buildSuccess = false
			
		ensure:
			boocProcess.Close()

		return buildSuccess
	
	protected override def AddCommandLineCommands(
		commandLine as CommandLineBuilderExtension):
	"""
	Adds command line commands.
	
	Remarks:
		It prevents <ManagedCompiler> from adding the standard commands.
	"""
			pass
	
	protected override def AddResponseFileCommands(
		commandLine as CommandLineBuilderExtension):
	"""
	Generates the Boo compiler command line.
	
	Returns:
		The Boo compiler command line.
	"""	
		commandLine.AppendSwitchIfNotNull('-t:', TargetType)
		commandLine.AppendSwitchIfNotNull('-o:', OutputAssembly)
		commandLine.AppendSwitchIfNotNull('-c:', Culture)
		commandLine.AppendSwitchIfNotNull('-srcdir:', SourceDirectory)
		
		if Pipelines:
			for pipeline in Pipelines:
				commandLine.AppendSwitchIfNotNull('-p:', pipeline)
			
		if References:
			for reference in References:
				commandLine.AppendSwitchIfNotNull('-r:', reference)
				
		if Resources:
			for resource in Resources:
				commandLine.AppendSwitchIfNotNull('-resource:', resource)
		
		if Verbosity:
			if string.Compare(
					Verbosity,
					'Normal',
					StringComparison.InvariantCultureIgnoreCase) == 0:
				pass
				
			elif string.Compare(
					Verbosity,
					'Warning',
					StringComparison.InvariantCultureIgnoreCase) == 0:
					
				commandLine.AppendSwitch('-v')
				
			elif string.Compare(
					Verbosity,
					'Info',
					StringComparison.InvariantCultureIgnoreCase) == 0:
					
				commandLine.AppendSwitch('-vv')
				
			elif string.Compare(
					Verbosity,
					'Verbose',
					StringComparison.InvariantCultureIgnoreCase) == 0:
					
				commandLine.AppendSwitch('-vvv')
			
			else:
				Log.LogErrorWithCodeFromResources(
					'Vbc.EnumParameterHasInvalidValue',
					'Verbosity',
					Verbosity,
					'Normal, Warning, Info, Verbose')
					
		commandLine.AppendFileNamesIfNotNull(Sources, ' ')
		
	protected override def GenerateFullPathToTool():
	"""
	Generats the full path to booc.exe.
	"""
		toolPath as string = ToolPath
		if toolPath is not null:
			path = Path.Combine(toolPath, ToolName)
		
		if path is null or not File.Exists(path):
			path = Path.Combine(
				Path.GetDirectoryName(typeof(Booc).Assembly.Location),
				ToolName)
		
		unless File.Exists(path):
			path = ToolLocationHelper.GetPathToDotNetFrameworkFile(
				ToolName,
				TargetDotNetFrameworkVersion.Version20)
		
		unless File.Exists(path):
			Log.LogErrorWithCodeFromResources(
				"General.FrameworksFileNotFound",
				ToolName,
				ToolLocationHelper.GetDotNetFrameworkVersionFolderPrefix(
					TargetDotNetFrameworkVersion.Version20))
						
		return path
