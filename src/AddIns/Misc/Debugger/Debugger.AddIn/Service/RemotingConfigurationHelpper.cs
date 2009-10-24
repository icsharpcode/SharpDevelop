// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Policy;

namespace ICSharpCode.SharpDevelop.Services
{
	[Serializable]
	class RemotingConfigurationHelpper
	{
		public string path;

		public RemotingConfigurationHelpper(string path)
		{
			this.path = path;
		}

		public static string GetLoadedAssemblyPath(string assemblyName)
		{
			string path = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					string fullFilename = assembly.Location;
					if (Path.GetFileName(fullFilename).Equals(assemblyName, StringComparison.OrdinalIgnoreCase)) {
						path = Path.GetDirectoryName(fullFilename);
						break;
					}
				} catch (NotSupportedException) {
					// assembly.Location throws NotSupportedException for assemblies emitted using
					// Reflection.Emit by custom controls used in the forms designer
				}
			}
			if (path == null) {
				throw new Exception("Assembly " + assemblyName + " is not loaded");
			}
			return path;
		}

		public void Configure()
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			
			RemotingConfiguration.Configure(Path.Combine(path, "Client.config"), false);

			string baseDir = Directory.GetDirectoryRoot(AppDomain.CurrentDomain.BaseDirectory);
			string relDirs = AppDomain.CurrentDomain.BaseDirectory + ";" + path;
			AppDomain serverAppDomain = AppDomain.CreateDomain("Debugging server",
			                                                   new Evidence(AppDomain.CurrentDomain.Evidence),
			                                                   baseDir,
			                                                   relDirs,
			                                                   AppDomain.CurrentDomain.ShadowCopyFiles);
			serverAppDomain.DoCallBack(new CrossAppDomainDelegate(ConfigureServer));
		}

		private void ConfigureServer()
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			RemotingConfiguration.Configure(Path.Combine(path, "Server.config"), false);
		}

		Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					string fullFilename = assembly.Location;
					if (Path.GetFileNameWithoutExtension(fullFilename).Equals(args.Name, StringComparison.OrdinalIgnoreCase) ||
					    assembly.FullName == args.Name) {
						return assembly;
					}
				} catch (NotSupportedException) {
					// assembly.Location throws NotSupportedException for assemblies emitted using
					// Reflection.Emit by custom controls used in the forms designer
				}
			}
			return null;
		}
	}
}
