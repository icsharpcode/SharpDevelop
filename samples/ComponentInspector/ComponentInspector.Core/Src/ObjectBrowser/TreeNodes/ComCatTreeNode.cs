// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NoGoop.Obj;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.TreeNodes
{

	internal class ComCatTreeNode : BrowserTreeNode
	{

        // Category guid
        protected BasicInfo                 _catInfo;

        protected  SortedList               _allClasses;


        internal ComCatTreeNode(BasicInfo category) : base()
		{
            _catInfo = category;
            _allClasses = new SortedList();
            SetPresInfo(PresentationMap.COM_FOLDER_CLASS);
            PostConstructor();
		}


        // Gets the objects to iterate over to make the child nodes
        protected override ICollection GetChildren()
        {
            lock (_allClasses)
            {
                if (_allClasses.Count > 0)
                    return _allClasses.Values;

                try
                {
                    IntPtr comObj;
                    int result = NoGoop.Win32.ActiveX.CoCreateInstance
                        (ref NoGoop.Win32.ActiveX.CategoriesMgrCLSID,
                         (IntPtr)0,
                         NoGoop.Win32.ActiveX.CLSCTX_INPROC_SERVER,
                         ref NoGoop.Win32.ActiveX.IUnknownIID,
                         out comObj);

                    TraceUtil.WriteLineInfo(this,
                                            "com create: 0x" 
                                            + result.ToString("X")
                                            + " " + comObj);
                    if (result == 0)
                    {
                        ICatInformation catInfo = (ICatInformation)
                            Marshal.GetObjectForIUnknown(comObj);

                        // Get the CLSIDs associated with each category
                        Guid[] cats = new Guid[1];
                        Guid[] cats1 = new Guid[0];
                        cats[0] = _catInfo._guid;
                        IEnumGUID enumClsIds;
                        catInfo.EnumClassesOfCategories(1,
                                                        cats,
                                                        0,
                                                        cats1,
                                                        out enumClsIds);

                        Guid clsId;
                        uint numRet;
                        while (true)
                        {
                            enumClsIds.Next(1, out clsId, out numRet);
                            if (numRet == 0)
                                break;

                            BasicInfo info = ComClassInfo.GetClassInfo(clsId);
                            if (info != null)
                                _allClasses.Add(info, info);
                        }

                        Marshal.ReleaseComObject(enumClsIds);
                        Marshal.ReleaseComObject(catInfo);

                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.WriteLineIf(null, TraceLevel.Info,
                                          "Categories - failure to read: " 
                                          + _catInfo._guid + " " + ex);
                }
                return _allClasses.Values;
            }    

        }

        // Allocates the correct type of node
        protected override BrowserTreeNode AllocateChildNode(Object obj)
        {
            ComTypeTreeNode node = new ComTypeTreeNode((BasicInfo)obj);
            node.IntermediateNodeTypes = null;
            return node;
        }


        // Determines is this node has children
        protected override bool HasChildren()
        {
            // Assume there are classes in the category
            // FIXME
            return true;
        }

        public override String GetName()
        {
            return _catInfo.GetName();
        }


        public override void GetDetailText()
		{
            _catInfo.GetDetailText();
            base.GetDetailText();
		}


	}

}
