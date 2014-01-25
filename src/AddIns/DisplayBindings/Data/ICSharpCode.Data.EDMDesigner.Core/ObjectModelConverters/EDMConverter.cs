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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using System.Xml.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters
{
    public class EDMConverter
    {
        public static EDM CreateEDMFromIDatabase(IDatabase database, string modelNamespace)
        {
            EDM edm = new EDM();

            edm.SSDLContainer = SSDLConverter.CreateSSDLContainer(database, modelNamespace);
            
            return edm;
        }

        public static XDocument GetSSDLXML(IDatabase database, string modelNamespace)
        { 
            SSDLContainer ssdlContainer = SSDLConverter.CreateSSDLContainer(database, modelNamespace);
            return SSDLIO.WriteXDocument(ssdlContainer);
        }

        public static XDocument CreateEDMXFromIDatabase(IDatabase database, string modelNamespace, string objectContextNamespace, string objectContextName)
        {
            XDocument ssdlXDocument = GetSSDLXML(database, modelNamespace);

            string ssdlTempFilename = IO.IO.GetTempFilenameWithExtension("ssdl");
            ssdlXDocument.Save(ssdlTempFilename);

            FileInfo fileInfo = new FileInfo(ssdlTempFilename);
            string filenameRump = Path.GetTempPath() + fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

            string edmGenPath = RuntimeEnvironment.GetRuntimeDirectory() + "\\EdmGen.exe";

            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = Path.GetTempPath();
            processStartInfo.FileName = edmGenPath;
            processStartInfo.Arguments = string.Format(@"/mode:FromSSDLGeneration /inssdl:""{0}.ssdl"" /outcsdl:""{0}.csdl"" /outmsl:""{0}.msl"" /outobjectlayer:""{1}.Designer.cs"" /outviews:""{1}.Views.cs"" /Namespace:{2} /EntityContainer:{1}",
                filenameRump, objectContextName, objectContextNamespace);
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            process.StartInfo = processStartInfo;
            process.Start();
            string outputMessage = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
            	throw new ObjectModelConverterException(string.Format("An error occured during generating the EDMX file from \"{0}.ssdl\".", filenameRump), 
                    outputMessage, ObjectModelConverterExceptionEnum.EDM);
            }

            XDocument csdlXDocument = XDocument.Load(filenameRump + ".csdl");
            XDocument mslXDocument = XDocument.Load(filenameRump + ".msl");
            mslXDocument = MSLIO.GenerateTypeMapping(mslXDocument);

            return EDMXIO.WriteXDocument(ssdlXDocument, csdlXDocument, mslXDocument);
        }
    }
}
