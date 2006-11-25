// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
{
    using System;

    public enum CorDebugRegister
    {
        // Fields
        REGISTER_AMD64_R10 = 11,
        REGISTER_AMD64_R11 = 12,
        REGISTER_AMD64_R12 = 13,
        REGISTER_AMD64_R13 = 14,
        REGISTER_AMD64_R14 = 15,
        REGISTER_AMD64_R15 = 0x10,
        REGISTER_AMD64_R8 = 9,
        REGISTER_AMD64_R9 = 10,
        REGISTER_AMD64_RAX = 3,
        REGISTER_AMD64_RBP = 2,
        REGISTER_AMD64_RBX = 6,
        REGISTER_AMD64_RCX = 4,
        REGISTER_AMD64_RDI = 8,
        REGISTER_AMD64_RDX = 5,
        REGISTER_AMD64_RIP = 0,
        REGISTER_AMD64_RSI = 7,
        REGISTER_AMD64_RSP = 1,
        REGISTER_AMD64_XMM0 = 0x11,
        REGISTER_AMD64_XMM1 = 0x12,
        REGISTER_AMD64_XMM10 = 0x1b,
        REGISTER_AMD64_XMM11 = 0x1c,
        REGISTER_AMD64_XMM12 = 0x1d,
        REGISTER_AMD64_XMM13 = 30,
        REGISTER_AMD64_XMM14 = 0x1f,
        REGISTER_AMD64_XMM15 = 0x20,
        REGISTER_AMD64_XMM2 = 0x13,
        REGISTER_AMD64_XMM3 = 20,
        REGISTER_AMD64_XMM4 = 0x15,
        REGISTER_AMD64_XMM5 = 0x16,
        REGISTER_AMD64_XMM6 = 0x17,
        REGISTER_AMD64_XMM7 = 0x18,
        REGISTER_AMD64_XMM8 = 0x19,
        REGISTER_AMD64_XMM9 = 0x1a,
        REGISTER_FRAME_POINTER = 2,
        REGISTER_IA64_BSP = 2,
        REGISTER_IA64_F0 = 0x83,
        REGISTER_IA64_R0 = 3,
        REGISTER_INSTRUCTION_POINTER = 0,
        REGISTER_STACK_POINTER = 1,
        REGISTER_X86_EAX = 3,
        REGISTER_X86_EBP = 2,
        REGISTER_X86_EBX = 6,
        REGISTER_X86_ECX = 4,
        REGISTER_X86_EDI = 8,
        REGISTER_X86_EDX = 5,
        REGISTER_X86_EIP = 0,
        REGISTER_X86_ESI = 7,
        REGISTER_X86_ESP = 1,
        REGISTER_X86_FPSTACK_0 = 9,
        REGISTER_X86_FPSTACK_1 = 10,
        REGISTER_X86_FPSTACK_2 = 11,
        REGISTER_X86_FPSTACK_3 = 12,
        REGISTER_X86_FPSTACK_4 = 13,
        REGISTER_X86_FPSTACK_5 = 14,
        REGISTER_X86_FPSTACK_6 = 15,
        REGISTER_X86_FPSTACK_7 = 0x10
    }
}

#pragma warning restore 108, 1591