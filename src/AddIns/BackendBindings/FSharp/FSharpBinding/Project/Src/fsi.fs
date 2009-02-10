// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Pickering" email="robert@strangelights.com"/>
//     <version>$Revision$</version>
// </file>

#light
namespace FSharpBinding

open System
open System.IO
open System.Configuration
open System.Collections.Generic
open System.Diagnostics
open System.Windows.Forms
open System.Xml
open ICSharpCode.SharpDevelop.Dom
open ICSharpCode.SharpDevelop.Dom.CSharp
open ICSharpCode.SharpDevelop.Internal.Templates
open ICSharpCode.SharpDevelop.Project
open ICSharpCode.Core
open ICSharpCode.SharpDevelop.Gui
open ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor

module TheControl =
    let outputQueue = new Queue<string>()
    let errorQueue = new Queue<string>()
    let fsiProcess = new Process()
    let panel = new Panel()
    let input = new TextBox(Anchor = (AnchorStyles.Left ||| AnchorStyles.Top ||| AnchorStyles.Right),
                            Width = panel.Width)
    let output = new TextBox(Multiline = true,
                             Top = input.Height,
                             Height = panel.Height - input.Height,
                             Width = panel.Width,
                             ReadOnly = true,
                             ScrollBars = ScrollBars.Both,
                             WordWrap = false,
                             Anchor = (AnchorStyles.Left ||| AnchorStyles.Top ||| AnchorStyles.Right ||| AnchorStyles.Bottom))
    panel.Controls.Add(input)
    panel.Controls.Add(output)
    let foundCompiler = 
        if ConfigurationManager.AppSettings.AllKeys |> Array.exists (fun x -> x = "alt_fs_bin_path") then
            let path = Path.Combine(ConfigurationManager.AppSettings.get_Item("alt_fs_bin_path"), "fsi.exe")
            if File.Exists(path) then
                fsiProcess.StartInfo.FileName <- path
                true
            else
                output.Text <- 
                    "you are trying to use the app setting alt_fs_bin_path, but fsi.exe is not localed in the given directory"
                false
        else
            let path = Environment.GetEnvironmentVariable("PATH")
            let paths = path.Split([|';'|])
            let path =  paths |> Array.tryfind (fun x -> try File.Exists(Path.Combine(x, "fsi.exe")) with _ -> false)
            match path with
            | Some x -> 
                fsiProcess.StartInfo.FileName <- Path.Combine(x, "fsi.exe")
                true
            | None ->
                let programFiles = Environment.GetEnvironmentVariable("ProgramFiles")
                let fsdirs = Directory.GetDirectories(programFiles, "FSharp*")
                let possibleFiles =
                    fsdirs |> Array.choose 
                        (fun x -> 
                            let fileInfo = new FileInfo(Path.Combine(x, "bin\fsi.exe"))
                            if fileInfo.Exists then
                                Some fileInfo
                            else
                                None)
                possibleFiles |> Array.sort (fun x y -> DateTime.Compare(x.CreationTime, y.CreationTime))
                if possibleFiles.Length > 0 then
                    fsiProcess.StartInfo.FileName <- possibleFiles.[0].FullName
                    true
                else
                    output.Text <- 
                        "Can not find the fsi.exe, ensure a version of the F# compiler is installed." + Environment.NewLine +
                        "Please see http://research.microsoft.com/fsharp for details of how to install the compiler"
                    false
    if foundCompiler then
        input.KeyUp.Add(fun ea -> 
                            if ea.KeyData = Keys.Return then
                                fsiProcess.StandardInput.WriteLine(input.Text)
                                input.Text <- "" )
        //fsiProcess.StartInfo.Arguments <- "--fsi-server sharpdevelopfsi"
        fsiProcess.StartInfo.UseShellExecute <- false
        fsiProcess.StartInfo.CreateNoWindow <- true
        fsiProcess.StartInfo.RedirectStandardError <- true
        fsiProcess.StartInfo.RedirectStandardInput <- true
        fsiProcess.StartInfo.RedirectStandardOutput <- true
        fsiProcess.ErrorDataReceived.Add(fun ea -> lock errorQueue (fun () -> errorQueue.Enqueue(ea.Data)) )
        fsiProcess.OutputDataReceived.Add(fun ea -> lock outputQueue (fun () ->  outputQueue.Enqueue(ea.Data)) )
        fsiProcess.Exited.Add(fun ea -> 
                            output.AppendText("fsi.exe died" + Environment.NewLine)
                            output.AppendText("restarting ..." + Environment.NewLine)
                            fsiProcess.Start() |> ignore)
        fsiProcess.Start() |> ignore
        fsiProcess.BeginErrorReadLine()
        fsiProcess.BeginOutputReadLine()
        let timer = new Timer(Interval = 1000)
        let readAll (q : Queue<string>) =
            while q.Count > 0 do
                output.AppendText(q.Dequeue() + Environment.NewLine)
        timer.Tick.Add(fun _ -> 
                        lock errorQueue (fun () -> readAll errorQueue)
                        lock outputQueue (fun () -> readAll outputQueue))
        timer.Start()
    else
        input.KeyUp.Add(fun ea -> 
                            if ea.KeyData = Keys.Return then
                                output.Text <- 
                                    (output.Text + Environment.NewLine +
                                     "F# not installed - could not execute command")
                                input.Text <- "" )

type FSharpInteractive = class
    inherit AbstractPadContent
        new() = {}
        override x.Content
            with get() = TheControl.panel :> Object
end

type SentToFSharpInteractive = class
    inherit AbstractMenuCommand
    new () = {}
    override x.Run() =
        if TheControl.foundCompiler then
            let window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow
            if window <> null then
                match window.ActiveViewContent :> obj with
                | :? ITextEditorControlProvider as textArea ->
                    let textArea = textArea.TextEditorControl.ActiveTextAreaControl.TextArea
                    let pad = WorkbenchSingleton.Workbench.GetPad(typeof<FSharpInteractive>)
                    pad.BringPadToFront()
                    if textArea.SelectionManager.HasSomethingSelected then
                        for selection in textArea.SelectionManager.SelectionCollection do
                            TheControl.fsiProcess.StandardInput.WriteLine(selection.SelectedText)
                    else
                        let line = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset)
                        let lineSegment = textArea.Document.GetLineSegment(line)
                        let lineText = textArea.Document.GetText(lineSegment.Offset, lineSegment.TotalLength)
                        TheControl.fsiProcess.StandardInput.WriteLine(lineText)
                    TheControl.fsiProcess.StandardInput.WriteLine(";;")
                | _ -> ()
end
