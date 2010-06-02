using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



namespace ICSharpCode.NRefactory.Parser.VBNet.Experimental {



partial class ExpressionFinder {

	const bool T = true;
	const bool x = false;

int currentState = 1;

	readonly Stack<int> stateStack = new Stack<int>();
	bool nextTokenIsPotentialStartOfXmlMode = false;
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token t)
	{
		if (t.kind != expectedKind)
			Error(t);
	}
	
	void Error(Token t) 
	{
	}

	public void InformToken(Token t) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
		switchlbl: switch (currentState) {
			case 0: {
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 1: { // start of ExpressionFinder
				PushContext(Context.Global);
				goto case 3;
			}
			case 2: {
				stateStack.Push(3);
				goto case 15; // OptionStatement
			}
			case 3: {
				if (t.kind == 166) {
					goto case 2;
				} else {
					goto case 5;
				}
			}
			case 4: {
				stateStack.Push(5);
				goto case 19; // ImportsStatement
			}
			case 5: {
				if (t.kind == 130) {
					goto case 4;
				} else {
					goto case 7;
				}
			}
			case 6: {
				stateStack.Push(7);
				goto case 24; // AttributeBlock
			}
			case 7: {
				if (t.kind == 34) {
					goto case 6;
				} else {
					goto case 9;
				}
			}
			case 8: {
				stateStack.Push(9);
				goto case 34; // NamespaceMemberDeclaration
			}
			case 9: {
				if (set[0, t.kind]) {
					goto case 8;
				} else {
					goto case 10;
				}
			}
			case 10: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 11: {
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 12: {
				Expect(17, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 13: { // start of StatementTerminator
				if (t.kind == 1) {
					goto case 11;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (t.kind == 17) {
					goto case 12;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 15: { // start of OptionStatement
				Expect(166, t); // "Option"
				currentState = 17;
				break;
			}
			case 16: {
				currentState = 17;
				break;
			}
			case 17: {
				if (set[1, t.kind]) {
					goto case 16;
				} else {
					goto case 18;
				}
			}
			case 18: {
				goto case 13; // StatementTerminator
			}
			case 19: { // start of ImportsStatement
				Expect(130, t); // "Imports"
				currentState = 20;
				break;
			}
			case 20: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 22;
			}
			case 21: {
				currentState = 22;
				break;
			}
			case 22: {
				if (set[1, t.kind]) {
					goto case 21;
				} else {
					goto case 23;
				}
			}
			case 23: {
				goto case 13; // StatementTerminator
			}
			case 24: { // start of AttributeBlock
				Expect(34, t); // "<"
				currentState = 25;
				break;
			}
			case 25: {
				PushContext(Context.Attribute);
				goto case 27;
			}
			case 26: {
				currentState = 27;
				break;
			}
			case 27: {
				if (set[2, t.kind]) {
					goto case 26;
				} else {
					goto case 28;
				}
			}
			case 28: {
				Expect(33, t); // ">"
				currentState = 29;
				break;
			}
			case 29: {
				PopContext();
				goto case 31;
			}
			case 30: {
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 31: {
				if (t.kind == 1) {
					goto case 30;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 32: {
				goto case 36; // NamespaceDeclaration
			}
			case 33: {
				goto case 46; // TypeDeclaration
			}
			case 34: { // start of NamespaceMemberDeclaration
				if (t.kind == 153) {
					goto case 32;
				} else {
					goto case 35;
				}
			}
			case 35: {
				if (set[3, t.kind]) {
					goto case 33;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 36: { // start of NamespaceDeclaration
				Expect(153, t); // "Namespace"
				currentState = 38;
				break;
			}
			case 37: {
				currentState = 38;
				break;
			}
			case 38: {
				if (set[1, t.kind]) {
					goto case 37;
				} else {
					goto case 39;
				}
			}
			case 39: {
				stateStack.Push(41);
				goto case 13; // StatementTerminator
			}
			case 40: {
				stateStack.Push(41);
				goto case 34; // NamespaceMemberDeclaration
			}
			case 41: {
				if (set[4, t.kind]) {
					goto case 40;
				} else {
					goto case 42;
				}
			}
			case 42: {
				Expect(106, t); // "End"
				currentState = 43;
				break;
			}
			case 43: {
				Expect(153, t); // "Namespace"
				currentState = 44;
				break;
			}
			case 44: {
				goto case 13; // StatementTerminator
			}
			case 45: {
				stateStack.Push(46);
				goto case 24; // AttributeBlock
			}
			case 46: { // start of TypeDeclaration
				if (t.kind == 34) {
					goto case 45;
				} else {
					goto case 48;
				}
			}
			case 47: {
				stateStack.Push(48);
				goto case 353; // TypeModifier
			}
			case 48: {
				if (set[5, t.kind]) {
					goto case 47;
				} else {
					goto case 51;
				}
			}
			case 49: {
				Expect(148, t); // "Module"
				currentState = 54;
				break;
			}
			case 50: {
				Expect(77, t); // "Class"
				currentState = 54;
				break;
			}
			case 51: {
				if (t.kind == 148) {
					goto case 49;
				} else {
					goto case 52;
				}
			}
			case 52: {
				if (t.kind == 77) {
					goto case 50;
				} else {
					Error(t);
					goto case 54;
				}
			}
			case 53: {
				currentState = 54;
				break;
			}
			case 54: {
				if (set[1, t.kind]) {
					goto case 53;
				} else {
					goto case 55;
				}
			}
			case 55: {
				stateStack.Push(56);
				goto case 13; // StatementTerminator
			}
			case 56: {
				PushContext(Context.Type);
				goto case 58;
			}
			case 57: {
				stateStack.Push(58);
				goto case 66; // MemberDeclaration
			}
			case 58: {
				if (set[6, t.kind]) {
					goto case 57;
				} else {
					goto case 59;
				}
			}
			case 59: {
				Expect(106, t); // "End"
				currentState = 62;
				break;
			}
			case 60: {
				Expect(148, t); // "Module"
				currentState = 64;
				break;
			}
			case 61: {
				Expect(77, t); // "Class"
				currentState = 64;
				break;
			}
			case 62: {
				if (t.kind == 148) {
					goto case 60;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t.kind == 77) {
					goto case 61;
				} else {
					Error(t);
					goto case 64;
				}
			}
			case 64: {
				stateStack.Push(65);
				goto case 13; // StatementTerminator
			}
			case 65: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 66: { // start of MemberDeclaration
				PushContext(Context.Member);
				goto case 68;
			}
			case 67: {
				stateStack.Push(68);
				goto case 24; // AttributeBlock
			}
			case 68: {
				if (t.kind == 34) {
					goto case 67;
				} else {
					goto case 70;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 357; // MemberModifier
			}
			case 70: {
				if (set[7, t.kind]) {
					goto case 69;
				} else {
					goto case 73;
				}
			}
			case 71: {
				stateStack.Push(75);
				goto case 99; // MemberVariableOrConstantDeclaration
			}
			case 72: {
				stateStack.Push(75);
				goto case 78; // SubOrFunctionDeclaration
			}
			case 73: {
				if (set[8, t.kind]) {
					goto case 71;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (t.kind == 120 || t.kind == 202) {
					goto case 72;
				} else {
					Error(t);
					goto case 75;
				}
			}
			case 75: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 76: {
				Expect(202, t); // "Sub"
				currentState = 80;
				break;
			}
			case 77: {
				Expect(120, t); // "Function"
				currentState = 80;
				break;
			}
			case 78: { // start of SubOrFunctionDeclaration
				if (t.kind == 202) {
					goto case 76;
				} else {
					goto case 79;
				}
			}
			case 79: {
				if (t.kind == 120) {
					goto case 77;
				} else {
					Error(t);
					goto case 80;
				}
			}
			case 80: {
				PushContext(Context.IdentifierExpected);
				goto case 81;
			}
			case 81: {
				currentState = 82;
				break;
			}
			case 82: {
				PopContext();
				goto case 87;
			}
			case 83: {
				Expect(31, t); // "("
				currentState = 85;
				break;
			}
			case 84: {
				stateStack.Push(86);
				goto case 108; // ParameterList
			}
			case 85: {
				if (set[9, t.kind]) {
					goto case 84;
				} else {
					goto case 86;
				}
			}
			case 86: {
				Expect(32, t); // ")"
				currentState = 90;
				break;
			}
			case 87: {
				if (t.kind == 31) {
					goto case 83;
				} else {
					goto case 90;
				}
			}
			case 88: {
				Expect(56, t); // "As"
				currentState = 89;
				break;
			}
			case 89: {
				stateStack.Push(91);
				goto case 192; // TypeName
			}
			case 90: {
				if (t.kind == 56) {
					goto case 88;
				} else {
					goto case 91;
				}
			}
			case 91: {
				stateStack.Push(92);
				goto case 123; // Block
			}
			case 92: {
				Expect(106, t); // "End"
				currentState = 95;
				break;
			}
			case 93: {
				Expect(202, t); // "Sub"
				currentState = 97;
				break;
			}
			case 94: {
				Expect(120, t); // "Function"
				currentState = 97;
				break;
			}
			case 95: {
				if (t.kind == 202) {
					goto case 93;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (t.kind == 120) {
					goto case 94;
				} else {
					Error(t);
					goto case 97;
				}
			}
			case 97: {
				goto case 13; // StatementTerminator
			}
			case 98: {
				Expect(81, t); // "Const"
				currentState = 100;
				break;
			}
			case 99: { // start of MemberVariableOrConstantDeclaration
				if (t.kind == 81) {
					goto case 98;
				} else {
					goto case 100;
				}
			}
			case 100: {
				stateStack.Push(103);
				goto case 283; // Identifier
			}
			case 101: {
				Expect(56, t); // "As"
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(106);
				goto case 192; // TypeName
			}
			case 103: {
				if (t.kind == 56) {
					goto case 101;
				} else {
					goto case 106;
				}
			}
			case 104: {
				Expect(16, t); // "="
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(107);
				goto case 132; // Expression
			}
			case 106: {
				if (t.kind == 16) {
					goto case 104;
				} else {
					goto case 107;
				}
			}
			case 107: {
				goto case 13; // StatementTerminator
			}
			case 108: { // start of ParameterList
				stateStack.Push(111);
				goto case 113; // Parameter
			}
			case 109: {
				Expect(18, t); // ","
				currentState = 110;
				break;
			}
			case 110: {
				stateStack.Push(111);
				goto case 113; // Parameter
			}
			case 111: {
				if (t.kind == 18) {
					goto case 109;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 112: {
				stateStack.Push(113);
				goto case 24; // AttributeBlock
			}
			case 113: { // start of Parameter
				if (t.kind == 34) {
					goto case 112;
				} else {
					goto case 115;
				}
			}
			case 114: {
				stateStack.Push(115);
				goto case 377; // ParameterModifier
			}
			case 115: {
				if (set[10, t.kind]) {
					goto case 114;
				} else {
					goto case 116;
				}
			}
			case 116: {
				stateStack.Push(119);
				goto case 283; // Identifier
			}
			case 117: {
				Expect(56, t); // "As"
				currentState = 118;
				break;
			}
			case 118: {
				stateStack.Push(122);
				goto case 192; // TypeName
			}
			case 119: {
				if (t.kind == 56) {
					goto case 117;
				} else {
					goto case 122;
				}
			}
			case 120: {
				Expect(16, t); // "="
				currentState = 121;
				break;
			}
			case 121: {
				goto case 132; // Expression
			}
			case 122: {
				if (t.kind == 16) {
					goto case 120;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 123: { // start of Block
				PushContext(Context.Body);
				goto case 124;
			}
			case 124: {
				stateStack.Push(126);
				goto case 13; // StatementTerminator
			}
			case 125: {
				stateStack.Push(126);
				goto case 13; // StatementTerminator
			}
			case 126: {
				if (t.kind == 1 || t.kind == 17) {
					goto case 125;
				} else {
					goto case 128;
				}
			}
			case 127: {
				stateStack.Push(128);
				goto case 281; // Statement
			}
			case 128: {
				if (set[11, t.kind]) {
					goto case 127;
				} else {
					goto case 130;
				}
			}
			case 129: {
				stateStack.Push(130);
				goto case 13; // StatementTerminator
			}
			case 130: {
				if (t.kind == 1 || t.kind == 17) {
					goto case 129;
				} else {
					goto case 131;
				}
			}
			case 131: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 132: { // start of Expression
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 137;
			}
			case 133: {
				goto case 217; // Literal
			}
			case 134: {
				Expect(31, t); // "("
				currentState = 135;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 132; // Expression
			}
			case 136: {
				Expect(32, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 137: {
				if (set[12, t.kind]) {
					goto case 133;
				} else {
					goto case 138;
				}
			}
			case 138: {
				if (t.kind == 31) {
					goto case 134;
				} else {
					goto case 148;
				}
			}
			case 139: {
				stateStack.Push(147);
				goto case 283; // Identifier
			}
			case 140: {
				Expect(31, t); // "("
				currentState = 141;
				break;
			}
			case 141: {
				Expect(162, t); // "Of"
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(145);
				goto case 192; // TypeName
			}
			case 143: {
				Expect(18, t); // ","
				currentState = 144;
				break;
			}
			case 144: {
				stateStack.Push(145);
				goto case 192; // TypeName
			}
			case 145: {
				if (t.kind == 18) {
					goto case 143;
				} else {
					goto case 146;
				}
			}
			case 146: {
				Expect(32, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 147: {
				if (t.kind == 31) {
					goto case 140;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 148: {
				if (set[13, t.kind]) {
					goto case 139;
				} else {
					goto case 151;
				}
			}
			case 149: {
				Expect(50, t); // "AddressOf"
				currentState = 150;
				break;
			}
			case 150: {
				goto case 132; // Expression
			}
			case 151: {
				if (t.kind == 50) {
					goto case 149;
				} else {
					goto case 157;
				}
			}
			case 152: {
				Expect(34, t); // "<"
				currentState = 153;
				break;
			}
			case 153: {
				PushContext(Context.Xml);
				goto case 154;
			}
			case 154: {
				currentState = 155;
				break;
			}
			case 155: {
				Expect(33, t); // ">"
				currentState = 156;
				break;
			}
			case 156: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 157: {
				if (t.kind == 34) {
					goto case 152;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 158: {
				Expect(64, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 159: {
				Expect(188, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 160: { // start of PrimitiveTypeName
				if (t.kind == 64) {
					goto case 158;
				} else {
					goto case 161;
				}
			}
			case 161: {
				if (t.kind == 188) {
					goto case 159;
				} else {
					goto case 163;
				}
			}
			case 162: {
				Expect(217, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 163: {
				if (t.kind == 217) {
					goto case 162;
				} else {
					goto case 165;
				}
			}
			case 164: {
				Expect(193, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 165: {
				if (t.kind == 193) {
					goto case 164;
				} else {
					goto case 167;
				}
			}
			case 166: {
				Expect(213, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 167: {
				if (t.kind == 213) {
					goto case 166;
				} else {
					goto case 169;
				}
			}
			case 168: {
				Expect(134, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 169: {
				if (t.kind == 134) {
					goto case 168;
				} else {
					goto case 171;
				}
			}
			case 170: {
				Expect(214, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 171: {
				if (t.kind == 214) {
					goto case 170;
				} else {
					goto case 173;
				}
			}
			case 172: {
				Expect(144, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 173: {
				if (t.kind == 144) {
					goto case 172;
				} else {
					goto case 175;
				}
			}
			case 174: {
				Expect(194, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 175: {
				if (t.kind == 194) {
					goto case 174;
				} else {
					goto case 177;
				}
			}
			case 176: {
				Expect(102, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 177: {
				if (t.kind == 102) {
					goto case 176;
				} else {
					goto case 179;
				}
			}
			case 178: {
				Expect(93, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 179: {
				if (t.kind == 93) {
					goto case 178;
				} else {
					goto case 181;
				}
			}
			case 180: {
				Expect(61, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 181: {
				if (t.kind == 61) {
					goto case 180;
				} else {
					goto case 183;
				}
			}
			case 182: {
				Expect(92, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 183: {
				if (t.kind == 92) {
					goto case 182;
				} else {
					goto case 185;
				}
			}
			case 184: {
				Expect(75, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 185: {
				if (t.kind == 75) {
					goto case 184;
				} else {
					goto case 187;
				}
			}
			case 186: {
				Expect(200, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 187: {
				if (t.kind == 200) {
					goto case 186;
				} else {
					goto case 189;
				}
			}
			case 188: {
				Expect(161, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 189: {
				if (t.kind == 161) {
					goto case 188;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 190: {
				Expect(123, t); // "Global"
				currentState = 197;
				break;
			}
			case 191: {
				stateStack.Push(197);
				goto case 283; // Identifier
			}
			case 192: { // start of TypeName
				if (t.kind == 123) {
					goto case 190;
				} else {
					goto case 193;
				}
			}
			case 193: {
				if (set[13, t.kind]) {
					goto case 191;
				} else {
					goto case 195;
				}
			}
			case 194: {
				stateStack.Push(197);
				goto case 160; // PrimitiveTypeName
			}
			case 195: {
				if (set[14, t.kind]) {
					goto case 194;
				} else {
					Error(t);
					goto case 197;
				}
			}
			case 196: {
				stateStack.Push(197);
				goto case 203; // TypeSuffix
			}
			case 197: {
				if (t.kind == 31) {
					goto case 196;
				} else {
					goto case 202;
				}
			}
			case 198: {
				Expect(22, t); // "."
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(201);
				goto case 214; // IdentifierOrKeyword
			}
			case 200: {
				stateStack.Push(201);
				goto case 203; // TypeSuffix
			}
			case 201: {
				if (t.kind == 31) {
					goto case 200;
				} else {
					goto case 202;
				}
			}
			case 202: {
				if (t.kind == 22) {
					goto case 198;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: { // start of TypeSuffix
				Expect(31, t); // "("
				currentState = 211;
				break;
			}
			case 204: {
				Expect(162, t); // "Of"
				currentState = 205;
				break;
			}
			case 205: {
				stateStack.Push(208);
				goto case 192; // TypeName
			}
			case 206: {
				Expect(18, t); // ","
				currentState = 207;
				break;
			}
			case 207: {
				stateStack.Push(208);
				goto case 192; // TypeName
			}
			case 208: {
				if (t.kind == 18) {
					goto case 206;
				} else {
					goto case 213;
				}
			}
			case 209: {
				Expect(18, t); // ","
				currentState = 210;
				break;
			}
			case 210: {
				if (t.kind == 18) {
					goto case 209;
				} else {
					goto case 213;
				}
			}
			case 211: {
				if (t.kind == 162) {
					goto case 204;
				} else {
					goto case 212;
				}
			}
			case 212: {
				if (t.kind == 18 || t.kind == 32) {
					goto case 210;
				} else {
					Error(t);
					goto case 213;
				}
			}
			case 213: {
				Expect(32, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 214: { // start of IdentifierOrKeyword
				currentState = stateStack.Pop();
				break;
			}
			case 215: {
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 216: {
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 217: { // start of Literal
				if (t.kind == 3) {
					goto case 215;
				} else {
					goto case 218;
				}
			}
			case 218: {
				if (t.kind == 4) {
					goto case 216;
				} else {
					goto case 220;
				}
			}
			case 219: {
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 220: {
				if (t.kind == 5) {
					goto case 219;
				} else {
					goto case 222;
				}
			}
			case 221: {
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 222: {
				if (t.kind == 6) {
					goto case 221;
				} else {
					goto case 224;
				}
			}
			case 223: {
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				if (t.kind == 7) {
					goto case 223;
				} else {
					goto case 226;
				}
			}
			case 225: {
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 226: {
				if (t.kind == 8) {
					goto case 225;
				} else {
					goto case 228;
				}
			}
			case 227: {
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 228: {
				if (t.kind == 9) {
					goto case 227;
				} else {
					goto case 230;
				}
			}
			case 229: {
				Expect(209, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 230: {
				if (t.kind == 209) {
					goto case 229;
				} else {
					goto case 232;
				}
			}
			case 231: {
				Expect(115, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				if (t.kind == 115) {
					goto case 231;
				} else {
					goto case 234;
				}
			}
			case 233: {
				Expect(158, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				if (t.kind == 158) {
					goto case 233;
				} else {
					goto case 236;
				}
			}
			case 235: {
				Expect(146, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				if (t.kind == 146) {
					goto case 235;
				} else {
					goto case 238;
				}
			}
			case 237: {
				Expect(151, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				if (t.kind == 151) {
					goto case 237;
				} else {
					goto case 240;
				}
			}
			case 239: {
				Expect(152, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 240: {
				if (t.kind == 152) {
					goto case 239;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 241: {
				stateStack.Push(245);
				goto case 283; // Identifier
			}
			case 242: {
				Expect(5, t); // LiteralInteger
				currentState = 245;
				break;
			}
			case 243: {
				if (set[13, t.kind]) {
					goto case 241;
				} else {
					goto case 244;
				}
			}
			case 244: {
				if (t.kind == 5) {
					goto case 242;
				} else {
					Error(t);
					goto case 245;
				}
			}
			case 245: {
				Expect(17, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 246: {
				Expect(98, t); // "Dim"
				currentState = 252;
				break;
			}
			case 247: {
				Expect(196, t); // "Static"
				currentState = 252;
				break;
			}
			case 248: {
				if (t.kind == 98) {
					goto case 246;
				} else {
					goto case 249;
				}
			}
			case 249: {
				if (t.kind == 196) {
					goto case 247;
				} else {
					goto case 251;
				}
			}
			case 250: {
				Expect(81, t); // "Const"
				currentState = 252;
				break;
			}
			case 251: {
				if (t.kind == 81) {
					goto case 250;
				} else {
					Error(t);
					goto case 252;
				}
			}
			case 252: {
				stateStack.Push(254);
				goto case 283; // Identifier
			}
			case 253: {
				Expect(27, t); // "?"
				currentState = 259;
				break;
			}
			case 254: {
				if (t.kind == 27) {
					goto case 253;
				} else {
					goto case 259;
				}
			}
			case 255: {
				Expect(31, t); // "("
				currentState = 257;
				break;
			}
			case 256: {
				Expect(18, t); // ","
				currentState = 257;
				break;
			}
			case 257: {
				if (t.kind == 18) {
					goto case 256;
				} else {
					goto case 258;
				}
			}
			case 258: {
				Expect(32, t); // ")"
				currentState = 269;
				break;
			}
			case 259: {
				if (t.kind == 31) {
					goto case 255;
				} else {
					goto case 269;
				}
			}
			case 260: {
				Expect(18, t); // ","
				currentState = 261;
				break;
			}
			case 261: {
				stateStack.Push(263);
				goto case 283; // Identifier
			}
			case 262: {
				Expect(27, t); // "?"
				currentState = 268;
				break;
			}
			case 263: {
				if (t.kind == 27) {
					goto case 262;
				} else {
					goto case 268;
				}
			}
			case 264: {
				Expect(31, t); // "("
				currentState = 266;
				break;
			}
			case 265: {
				Expect(18, t); // ","
				currentState = 266;
				break;
			}
			case 266: {
				if (t.kind == 18) {
					goto case 265;
				} else {
					goto case 267;
				}
			}
			case 267: {
				Expect(32, t); // ")"
				currentState = 269;
				break;
			}
			case 268: {
				if (t.kind == 31) {
					goto case 264;
				} else {
					goto case 269;
				}
			}
			case 269: {
				if (t.kind == 18) {
					goto case 260;
				} else {
					goto case 277;
				}
			}
			case 270: {
				Expect(56, t); // "As"
				currentState = 272;
				break;
			}
			case 271: {
				Expect(155, t); // "New"
				currentState = 273;
				break;
			}
			case 272: {
				if (t.kind == 155) {
					goto case 271;
				} else {
					goto case 273;
				}
			}
			case 273: {
				stateStack.Push(276);
				goto case 192; // TypeName
			}
			case 274: {
				Expect(31, t); // "("
				currentState = 275;
				break;
			}
			case 275: {
				Expect(32, t); // ")"
				currentState = 280;
				break;
			}
			case 276: {
				if (t.kind == 31) {
					goto case 274;
				} else {
					goto case 280;
				}
			}
			case 277: {
				if (t.kind == 56) {
					goto case 270;
				} else {
					goto case 280;
				}
			}
			case 278: {
				Expect(16, t); // "="
				currentState = 279;
				break;
			}
			case 279: {
				goto case 132; // Expression
			}
			case 280: {
				if (t.kind == 16) {
					goto case 278;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 281: { // start of Statement
				if (set[15, t.kind]) {
					goto case 243;
				} else {
					goto case 282;
				}
			}
			case 282: {
				if (t.kind == 81 || t.kind == 98 || t.kind == 196) {
					goto case 248;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 283: { // start of Identifier
				PushContext(Context.IdentifierExpected);
				goto case 286;
			}
			case 284: {
				stateStack.Push(288);
				goto case 291; // IdentifierForFieldDeclaration
			}
			case 285: {
				Expect(91, t); // "Custom"
				currentState = 288;
				break;
			}
			case 286: {
				if (set[16, t.kind]) {
					goto case 284;
				} else {
					goto case 287;
				}
			}
			case 287: {
				if (t.kind == 91) {
					goto case 285;
				} else {
					Error(t);
					goto case 288;
				}
			}
			case 288: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 289: {
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 290: {
				Expect(51, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 291: { // start of IdentifierForFieldDeclaration
				if (t.kind == 2) {
					goto case 289;
				} else {
					goto case 292;
				}
			}
			case 292: {
				if (t.kind == 51) {
					goto case 290;
				} else {
					goto case 294;
				}
			}
			case 293: {
				Expect(55, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 294: {
				if (t.kind == 55) {
					goto case 293;
				} else {
					goto case 296;
				}
			}
			case 295: {
				Expect(57, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 296: {
				if (t.kind == 57) {
					goto case 295;
				} else {
					goto case 298;
				}
			}
			case 297: {
				Expect(58, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				if (t.kind == 58) {
					goto case 297;
				} else {
					goto case 300;
				}
			}
			case 299: {
				Expect(59, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 300: {
				if (t.kind == 59) {
					goto case 299;
				} else {
					goto case 302;
				}
			}
			case 301: {
				Expect(60, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				if (t.kind == 60) {
					goto case 301;
				} else {
					goto case 304;
				}
			}
			case 303: {
				Expect(63, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 304: {
				if (t.kind == 63) {
					goto case 303;
				} else {
					goto case 306;
				}
			}
			case 305: {
				Expect(80, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 306: {
				if (t.kind == 80) {
					goto case 305;
				} else {
					goto case 308;
				}
			}
			case 307: {
				Expect(97, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 308: {
				if (t.kind == 97) {
					goto case 307;
				} else {
					goto case 310;
				}
			}
			case 309: {
				Expect(100, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 310: {
				if (t.kind == 100) {
					goto case 309;
				} else {
					goto case 312;
				}
			}
			case 311: {
				Expect(109, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 312: {
				if (t.kind == 109) {
					goto case 311;
				} else {
					goto case 314;
				}
			}
			case 313: {
				Expect(114, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 314: {
				if (t.kind == 114) {
					goto case 313;
				} else {
					goto case 316;
				}
			}
			case 315: {
				Expect(119, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 316: {
				if (t.kind == 119) {
					goto case 315;
				} else {
					goto case 318;
				}
			}
			case 317: {
				Expect(126, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 318: {
				if (t.kind == 126) {
					goto case 317;
				} else {
					goto case 320;
				}
			}
			case 319: {
				Expect(132, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 320: {
				if (t.kind == 132) {
					goto case 319;
				} else {
					goto case 322;
				}
			}
			case 321: {
				Expect(136, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 322: {
				if (t.kind == 136) {
					goto case 321;
				} else {
					goto case 324;
				}
			}
			case 323: {
				Expect(139, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 324: {
				if (t.kind == 139) {
					goto case 323;
				} else {
					goto case 326;
				}
			}
			case 325: {
				Expect(163, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 326: {
				if (t.kind == 163) {
					goto case 325;
				} else {
					goto case 328;
				}
			}
			case 327: {
				Expect(169, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 328: {
				if (t.kind == 169) {
					goto case 327;
				} else {
					goto case 330;
				}
			}
			case 329: {
				Expect(176, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 330: {
				if (t.kind == 176) {
					goto case 329;
				} else {
					goto case 332;
				}
			}
			case 331: {
				Expect(195, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 332: {
				if (t.kind == 195) {
					goto case 331;
				} else {
					goto case 334;
				}
			}
			case 333: {
				Expect(204, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 334: {
				if (t.kind == 204) {
					goto case 333;
				} else {
					goto case 336;
				}
			}
			case 335: {
				Expect(205, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 336: {
				if (t.kind == 205) {
					goto case 335;
				} else {
					goto case 338;
				}
			}
			case 337: {
				Expect(215, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 338: {
				if (t.kind == 215) {
					goto case 337;
				} else {
					goto case 340;
				}
			}
			case 339: {
				Expect(216, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 340: {
				if (t.kind == 216) {
					goto case 339;
				} else {
					goto case 342;
				}
			}
			case 341: {
				Expect(222, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 342: {
				if (t.kind == 222) {
					goto case 341;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 343: {
				Expect(180, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 344: {
				Expect(118, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 345: { // start of AccessModifier
				if (t.kind == 180) {
					goto case 343;
				} else {
					goto case 346;
				}
			}
			case 346: {
				if (t.kind == 118) {
					goto case 344;
				} else {
					goto case 348;
				}
			}
			case 347: {
				Expect(179, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 348: {
				if (t.kind == 179) {
					goto case 347;
				} else {
					goto case 350;
				}
			}
			case 349: {
				Expect(177, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 350: {
				if (t.kind == 177) {
					goto case 349;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 351: {
				goto case 345; // AccessModifier
			}
			case 352: {
				Expect(191, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 353: { // start of TypeModifier
				if (set[17, t.kind]) {
					goto case 351;
				} else {
					goto case 354;
				}
			}
			case 354: {
				if (t.kind == 191) {
					goto case 352;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 355: {
				goto case 345; // AccessModifier
			}
			case 356: {
				Expect(191, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 357: { // start of MemberModifier
				if (set[17, t.kind]) {
					goto case 355;
				} else {
					goto case 358;
				}
			}
			case 358: {
				if (t.kind == 191) {
					goto case 356;
				} else {
					goto case 360;
				}
			}
			case 359: {
				Expect(192, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 360: {
				if (t.kind == 192) {
					goto case 359;
				} else {
					goto case 362;
				}
			}
			case 361: {
				Expect(172, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 362: {
				if (t.kind == 172) {
					goto case 361;
				} else {
					goto case 364;
				}
			}
			case 363: {
				Expect(160, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 364: {
				if (t.kind == 160) {
					goto case 363;
				} else {
					goto case 366;
				}
			}
			case 365: {
				Expect(173, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 366: {
				if (t.kind == 173) {
					goto case 365;
				} else {
					goto case 368;
				}
			}
			case 367: {
				Expect(171, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 368: {
				if (t.kind == 171) {
					goto case 367;
				} else {
					goto case 370;
				}
			}
			case 369: {
				Expect(175, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 370: {
				if (t.kind == 175) {
					goto case 369;
				} else {
					goto case 372;
				}
			}
			case 371: {
				Expect(226, t); // "WithEvents"
				currentState = stateStack.Pop();
				break;
			}
			case 372: {
				if (t.kind == 226) {
					goto case 371;
				} else {
					goto case 374;
				}
			}
			case 373: {
				Expect(98, t); // "Dim"
				currentState = stateStack.Pop();
				break;
			}
			case 374: {
				if (t.kind == 98) {
					goto case 373;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 375: {
				Expect(65, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 376: {
				Expect(62, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 377: { // start of ParameterModifier
				if (t.kind == 65) {
					goto case 375;
				} else {
					goto case 378;
				}
			}
			case 378: {
				if (t.kind == 62) {
					goto case 376;
				} else {
					goto case 380;
				}
			}
			case 379: {
				Expect(167, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 380: {
				if (t.kind == 167) {
					goto case 379;
				} else {
					goto case 382;
				}
			}
			case 381: {
				Expect(174, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 382: {
				if (t.kind == 174) {
					goto case 381;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
		}

	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,T,x,T, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,T,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};

} // end Parser


}