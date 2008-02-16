// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Pickering" email="robert@strangelights.com"/>
//     <version>$Revision$</version>
// </file>

#light
namespace FSharpBinding
open System
open System.Xml
open ICSharpCode.SharpDevelop.Internal.Templates
open ICSharpCode.SharpDevelop.Project
open FSharpBinding

type FSharpLanguageBinding() = class
    interface ILanguageBinding with
        member x.Language 
            with get () = "F#"
        member x.LoadProject(engineProvider : IMSBuildEngineProvider, fileName : string, projectName : string) =
            new FSharpProject(engineProvider, fileName, projectName ) :> IProject
        member c.CreateProject(info : ProjectCreateInformation) =
            new FSharpProject(info) :> IProject
    end
end