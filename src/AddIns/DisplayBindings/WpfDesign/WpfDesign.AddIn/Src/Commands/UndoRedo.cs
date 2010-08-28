// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.Core;
using ICSharpCode.WpfDesign.Designer;
namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Invokes the Undo command if available on the Design Surface.
	/// </summary>
	class Undo : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
                surface.Undo();
        }
    }
	
	/// <summary>
	/// Invokes the Redo command if available on the Design surface.
	/// </summary>
	class Redo : AbstractMenuCommand
    {
        public override void Run()
        {
            var surface = Owner as DesignSurface;
            if (surface != null)
                surface.Redo();
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Undo"/>.
	/// </summary>
	class IsUndoEnabled : IConditionEvaluator
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if (surface != null)
                return surface.CanUndo();
            return false;
        }
    }
	
	/// <summary>
	/// Provides implementation of <see cref="IConditionEvaluator"/> for <see cref="Redo"/>.
	/// </summary>
	class IsRedoEnabled : IConditionEvaluator 
    {
        public bool IsValid(object owner, Condition condition)
        {
            var surface = owner as DesignSurface;
            if (surface != null)
                return surface.CanRedo();
            return false;
        }
    }
}
