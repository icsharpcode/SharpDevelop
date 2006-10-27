// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Dialogs;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComTypeLibRootTreeNode : BrowserTreeNode
	{


        protected static ComTypeLibRootTreeNode        _node;


        internal static ComTypeLibRootTreeNode ComTLRootNode
        {
            get
                {
                    return _node;
                }
        }


        internal ComTypeLibRootTreeNode() : base()
		{
            _node = this;
            SetPresInfo(PresentationMap.COM_FOLDER_TYPELIB);
            PostConstructor();
		}

        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            ProgressDialog progress = new ProgressDialog();
            progress.Setup(String.Format(StringParser.Parse("${res:ComponentInspector.ProgressDialog.GettingInformationDialogTitle}"), "Type Library"),
						   String.Format(StringParser.Parse("${res:ComponentInspector.ProgressDialog.GettingInformationMessage}"), "Type Library"),
                           TypeLibrary.GetRegisteredTypeLibsCount(),
                           !ProgressDialog.HAS_PROGRESS_TEXT,
                           ProgressDialog.FINAL);
            progress.ShowIfNotDone();

            return TypeLibrary.GetRegisteredTypeLibs(progress);
        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            ComTypeLibTreeNode node = new ComTypeLibTreeNode((TypeLibrary)obj);
            return node;
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            return TypeLibrary.GetRegisteredTypeLibsCount() > 0;
        }

        public override String GetName()
		{
            return StringParser.Parse("${res:ComponentInspector.ComTypeLibraryRootTreeNode.Text}");
		}

        public override void GetDetailText()
		{
            base.GetDetailText();
		}


	}

}
