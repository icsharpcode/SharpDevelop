// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Pickering" email="robert@strangelights.com"/>
//     <version>$Revision$</version>
// </file>

#light
namespace FSharpBinding

//Global todos
//TODO add "compiling" dialog to output the correct directory
//TODO copy all config to the correct locations
//TODO add directory structure

open System
open System.ComponentModel
open System.Diagnostics
open System.Collections.Generic
open System.Windows.Forms
open System.IO
open System.Xml

open ICSharpCode.SharpDevelop.Dom
open ICSharpCode.SharpDevelop.Dom.CSharp
open ICSharpCode.SharpDevelop.Internal.Templates
open ICSharpCode.SharpDevelop.Project
open ICSharpCode.Core
open ICSharpCode.SharpDevelop.Gui
open ICSharpCode.SharpDevelop.Gui.OptionPanels

type FSharpProject = class
    inherit CompilableProject
    new (info : ProjectLoadInformation) as x = 
        { inherit CompilableProject(info ) }
    new (info : ProjectCreateInformation) as x = 
        { inherit CompilableProject(info) } then
        try
            base.AddImport(@"$(MSBuildExtensionsPath32)\..\Microsoft F#\v4.0\Microsoft.FSharp.Targets", null)
            base.ReevaluateIfNecessary() // provoke exception if import is invalid
        with
            | :? Microsoft.Build.Exceptions.InvalidProjectFileException as ex ->
                raise (ProjectLoadException("Please ensure that the F# compiler is installed on your computer.\n\n" + ex.Message, ex))
    override x.GetDefaultItemType(fileName : string) = 
        if String.Equals(".fs", Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase) then
            ItemType.Compile
        else if String.Equals(".fsi", Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase) then
            ItemType.Compile
        else
            base.GetDefaultItemType(fileName)
    override x.Language
        with get() = "F#"
    override x.LanguageProperties
        with get() = LanguageProperties.None
end



type FSharpProjectNode = class
    inherit ProjectNode
    new (project : IProject) = 
        { inherit ProjectNode(project) }
    member  x.AddParentFolder((virtualName : string), (relativeDirectoryPath : string), (directoryNodeList :Dictionary<string, DirectoryNode>)) =
        if (relativeDirectoryPath.Length = 0
            || String.Compare(virtualName, 0, relativeDirectoryPath, 0, relativeDirectoryPath.Length, StringComparison.InvariantCultureIgnoreCase) = 0) then
            let pos = virtualName.IndexOf('/', relativeDirectoryPath.Length + 1)
            if (pos > 0) then
                let subFolderName = virtualName.Substring(relativeDirectoryPath.Length, pos - relativeDirectoryPath.Length);
                let res,node  = directoryNodeList.TryGetValue(subFolderName)
                if (res) then
                    if (node.FileNodeStatus = FileNodeStatus.None) then
                        node.FileNodeStatus <- FileNodeStatus.InProject
                else
                    let node = new DirectoryNode(Path.Combine(x.Directory, subFolderName), FileNodeStatus.Missing);
                    node.AddTo(x)
                    directoryNodeList.[subFolderName] <- node
    override x.Initialize() =
        //Debugger.Break()
        let fileNodeDictionary
            = new Dictionary<string, FileNode>(StringComparer.InvariantCultureIgnoreCase)
        let directoryNodeList = new Dictionary<string, DirectoryNode>(StringComparer.InvariantCultureIgnoreCase)
        
        let relativeDirectoryPath = 
            if (x.RelativePath.Length > 0) then
                (x.RelativePath.Replace('\\', '/')) + "/"
            else
                String.Empty
                    
        for item in x.Project.Items do
            match item with
            | :? FileProjectItem as item ->
                let virtualName = item.VirtualName.Replace('\\', '/')
                let virtualName = 
                    if (virtualName.EndsWith("/")) then
                        virtualName.Substring(0, virtualName.Length - 1)
                    else
                        virtualName
                let fileName = Path.GetFileName(virtualName)
                if (not (String.Equals(virtualName, relativeDirectoryPath + fileName, StringComparison.InvariantCultureIgnoreCase))) then
                    x.AddParentFolder(virtualName, relativeDirectoryPath, directoryNodeList);
                    //continue;
                    
                if (item.ItemType = ItemType.Folder || item.ItemType = ItemType.WebReferences) then
                    let newDirectoryNode = DirectoryNodeFactory.CreateDirectoryNode(x, x.Project, fileName)
                    if not (Directory.Exists(item.FileName)) then
                        newDirectoryNode.FileNodeStatus <- FileNodeStatus.Missing
                    newDirectoryNode.ProjectItem <- item
                    newDirectoryNode.AddTo(x)
                    directoryNodeList.[fileName] <- newDirectoryNode
                else
                    let fileNode = new FileNode(item.FileName)
                    if not (File.Exists(item.FileName)) then
                        fileNode.FileNodeStatus <- FileNodeStatus.Missing
                    fileNode.ProjectItem <- item
                    fileNodeDictionary.[fileName] <- fileNode
                    fileNode.AddTo(x)
            | _ -> ()

        // Add files found in file system
        if (System.IO.Directory.Exists(x.Directory)) then
            for subDirectory in System.IO.Directory.GetDirectories(x.Directory) do
                let filename = Path.GetFileName(subDirectory)
                if (filename <> ".svn") then
                    let res, node  = directoryNodeList.TryGetValue(filename)
                    if res then
                        if (node.FileNodeStatus = FileNodeStatus.None) then
                            node.FileNodeStatus <- FileNodeStatus.InProject;
                    else
                        let node = DirectoryNodeFactory.CreateDirectoryNode(x, x.Project, subDirectory)
                        node.AddTo(x)
                    
            for fullpath in System.IO.Directory.GetFiles(x.Directory) do
                let file = Path.GetFileName(fullpath)
                let res, node = fileNodeDictionary.TryGetValue(file)
                if res then
                    if (node.FileNodeStatus = FileNodeStatus.None) then
                        node.FileNodeStatus <- FileNodeStatus.InProject
                else
                    let node = new FileNode(file)
                    node.AddTo(x)
                    
end

type FSharpProjectNodeBuilder() = class
    interface IProjectNodeBuilder with
        member x.CanBuildProjectTree(project : IProject) = 
            project :? FSharpProject
        member x.AddProjectNode(solution : TreeNode, project : IProject) =
            let prjNode = new FSharpProjectNode(project)
            prjNode.AddTo(solution)
            
            let referenceFolderNode = new ReferenceFolder(project)
            referenceFolderNode.AddTo(prjNode)
            
            prjNode :> TreeNode
    end
end

module ProjectHelpers = 
    let forEachFileNode f (nodes : TreeNodeCollection) =
        for node in nodes do
            match node with
            | :? FileNode as fileNode ->
                if fileNode.ProjectItem <> null then
                    f fileNode
            | _ -> ()
         
    let reorderItems (nodes : TreeNodeCollection) (project : IProject) =
        //ProjectService.MarkProjectDirty(project)
        project.Save()
        let doc = new XmlDocument()
        doc.Load(project.FileName)
        let nsmgr = new XmlNamespaceManager(doc.NameTable)
        nsmgr.AddNamespace("proj", "http://schemas.microsoft.com/developer/msbuild/2003")
        let d = new Dictionary<FileNode,XmlNode>()
        nodes |> forEachFileNode
         (fun node -> 
            let docNode = doc.SelectSingleNode(Printf.sprintf @"//proj:Compile[@Include=""%s""]" (Path.GetFileName(node.FileName)), nsmgr)
            if docNode <> null then
                d.[node] <- docNode
                docNode.ParentNode.RemoveChild(docNode) |> ignore)
        let itemNode = doc.SelectSingleNode("//proj:ItemGroup", nsmgr)
        nodes |> forEachFileNode
         (fun node -> 
            let found, xmlElem = d.TryGetValue(node)
            if found then
                itemNode.AppendChild(xmlElem) |> ignore)
        doc.Save(project.FileName)
        

type MoveUpFileEvent() =
    inherit AbstractMenuCommand()
    override x.Run() = 
        let node  = ProjectBrowserPad.Instance.SelectedNode
        match node with
        | :? FileNode as fileNode ->
            let parent = node.Parent
            let nodeIndex = parent.Nodes.IndexOf(node)
            if  nodeIndex > 1 then
                parent.Nodes.Remove(node)
                parent.Nodes.Insert(nodeIndex - 1, node)
            ProjectHelpers.reorderItems parent.Nodes fileNode.Project
        | _ -> ()


type MoveDownFileEvent() =
    inherit AbstractMenuCommand()
    override x.Run() =
        let node  = ProjectBrowserPad.Instance.SelectedNode
        match node with
        | :? FileNode as fileNode ->
            let parent = node.Parent
            let nodeIndex = parent.Nodes.IndexOf(node)
            if  nodeIndex < parent.Nodes.Count then
                parent.Nodes.Remove(node)
                parent.Nodes.Insert(nodeIndex + 1, node)
            ProjectHelpers.reorderItems parent.Nodes fileNode.Project
        | _ -> ()
    
 
type FsOptions() =
    inherit AbstractXmlFormsProjectOptionPanel()
    override x.LoadPanelContents() =
        let this = (typeof<FsOptions>)
        let caller = this.Assembly
        x.SetupFromXmlStream(caller.GetManifestResourceStream("FsOptions.xfrm"))
        x.InitializeHelper()
        x.helper.BindBoolean(x.Get<CheckBox>("standalone"), "Standalone", false) |> ignore
        x.helper.BindBoolean(x.Get<CheckBox>("nomllib"), "NoMLLib", false) |> ignore
