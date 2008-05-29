using System;

// Conversion general
using ICSharpCode.NRefactory.PrettyPrinter;


namespace ICSharpCode.CodeConversion
{
    public class HelperFunctions
    {
        private HelperFunctions() { }

        public static string GetNRefactoryVersion()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(VBNetOutputVisitor));
            Version v = asm.GetName().Version;

            string s = string.Format("{0:0}.{1:0}.{2:0}.{3:0}",
                    v.Major, v.Minor, v.Build, v.Revision);

            return s;
        }
    }
}
