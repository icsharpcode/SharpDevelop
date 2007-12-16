// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Mono.Build.Tasks
{
	/// <summary>
	/// Gets the path to the Mono Framework assemblies.
	/// </summary>
	public class GetMonoFrameworkPath : Task
	{
		public const string TargetMonoFrameworkVersion11 = "v1.1";
		public const string TargetMonoFrameworkVersion20 = "v2.0";
		
		string path = String.Empty;
		TargetMonoFrameworkVersion targetFrameworkVersion = TargetMonoFrameworkVersion.VersionLatest;
		
		public GetMonoFrameworkPath()
		{
		}
		
		[Output]
		public string Path { 
			get { return path; }
			set { path = value; }
		}
		
		public string TargetFrameworkVersion {
			get { return ConvertToString(targetFrameworkVersion); }
			set { targetFrameworkVersion = ConvertToEnum(value); }
		}

		public override bool Execute()
		{
			if (MonoToolLocationHelper.IsMonoInstalled) {
				System.Diagnostics.Debug.WriteLine("TargetFrameworkVersion: " + targetFrameworkVersion.ToString());
				path = MonoToolLocationHelper.GetPathToMonoFramework(targetFrameworkVersion);
				System.Diagnostics.Debug.WriteLine("MonoFrameworkPath: " + path);
				return true;
			}
			Log.LogError("Mono is not installed.");
			return false;
		}
		
		static string ConvertToString(TargetMonoFrameworkVersion frameworkVersion)
		{
			switch (frameworkVersion) {
				case TargetMonoFrameworkVersion.Version11:
					return TargetMonoFrameworkVersion11;
				case TargetMonoFrameworkVersion.Version20:
					return TargetMonoFrameworkVersion20;
			}
			return null;
		}
		
		static TargetMonoFrameworkVersion ConvertToEnum(string frameworkVersion)
		{
			if (frameworkVersion == TargetMonoFrameworkVersion11) {
				return TargetMonoFrameworkVersion.Version11;
			} else if (frameworkVersion == TargetMonoFrameworkVersion20) {
				return TargetMonoFrameworkVersion.Version20;
			}
			throw new ArgumentException("Unknown Mono target framework version: " + frameworkVersion);
		}
	}
}
