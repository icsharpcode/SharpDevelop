// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data;
using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Views;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;

namespace ICSharpCode.Profiler.AddIn.Commands
{
    using ICSharpCode.Profiler.Controller;
    using ICSharpCode.Profiler.AddIn.Dialogs;

    /// <summary>
    /// Description of RunExecutable
    /// </summary>
    public class ProfileExecutable : AbstractMenuCommand
    {
        /// <summary>
        /// Starts the command
        /// </summary>
        public override void Run()
        {
            ProfileExecutableForm form = new ProfileExecutableForm();

            form.Show();
        }
    }
}
