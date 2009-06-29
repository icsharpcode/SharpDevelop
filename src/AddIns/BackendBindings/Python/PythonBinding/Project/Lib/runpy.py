#####################################################################################
#
#  Copyright (c) Microsoft Corporation. All rights reserved.
#
# This source code is subject to terms and conditions of the Microsoft Public License. A 
# copy of the license can be found in the License.html file at the root of this distribution. If 
# you cannot locate the  Microsoft Public License, please send an email to 
# ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
# by the terms of the Microsoft Public License.
#
# You must not remove this notice, or any other, from this software.
#
#
#####################################################################################

"""
Fake runpy.py which emulates what CPython does to properly support the '-m' flag.  
If you have access to the CPython standard library, you most likely do not need this.
"""

import sys, nt

def run_module(modToRun, init_globals=None, run_name = '__main__', alter_sys = True):
    if alter_sys:
        for o in sys.path:
            libpath = o + '\\' + modToRun + '.py'
            if nt.access(libpath, nt.F_OK):
                sys.argv[0] = libpath
                break
    
    __import__(modToRun)
