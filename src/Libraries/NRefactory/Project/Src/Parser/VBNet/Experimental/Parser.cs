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

int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool nextTokenIsPotentialStartOfXmlMode = false;
	bool readXmlIdentifier = false;
	
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
		ApplyToken(t);
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (t == null) { currentState = 1; break; }
				if (t.kind == 172) {
					stateStack.Push(1);
					goto case 300;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 297;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 230;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (t == null) { currentState = 4; break; }
				if (set[0, t.kind]) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (t == null) { currentState = 5; break; }
				if (t.kind == 159) {
					goto case 293;
				} else {
					if (set[1, t.kind]) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(t);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (t == null) { currentState = 7; break; }
				if (t.kind == 39) {
					stateStack.Push(7);
					goto case 230;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (t == null) { currentState = 8; break; }
				if (set[2, t.kind]) {
					currentState = 8;
					break;
				} else {
					if (t.kind == 83 || t.kind == 154) {
						goto case 292;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 292;
				} else {
					stateStack.Push(10);
					goto case 15;
				}
			}
			case 10: {
				PushContext(Context.Type, t);
				goto case 11;
			}
			case 11: {
				if (t == null) { currentState = 11; break; }
				if (set[4, t.kind]) {
					stateStack.Push(11);
					goto case 17;
				} else {
					Expect(112, t); // "End"
					currentState = 12;
					break;
				}
			}
			case 12: {
				if (t == null) { currentState = 12; break; }
				if (t.kind == 83 || t.kind == 154) {
					currentState = 13;
					break;
				} else {
					Error(t);
					goto case 13;
				}
			}
			case 13: {
				stateStack.Push(14);
				goto case 15;
			}
			case 14: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 15: {
				if (t == null) { currentState = 15; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 16: {
				if (t == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				PushContext(Context.Member, t);
				goto case 18;
			}
			case 18: {
				if (t == null) { currentState = 18; break; }
				if (t.kind == 39) {
					stateStack.Push(18);
					goto case 230;
				} else {
					goto case 19;
				}
			}
			case 19: {
				if (t == null) { currentState = 19; break; }
				if (set[5, t.kind]) {
					currentState = 19;
					break;
				} else {
					if (set[6, t.kind]) {
						stateStack.Push(20);
						goto case 285;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(20);
							goto case 273;
						} else {
							if (t.kind == 100) {
								stateStack.Push(20);
								goto case 262;
							} else {
								if (t.kind == 118) {
									stateStack.Push(20);
									goto case 252;
								} else {
									if (t.kind == 97) {
										stateStack.Push(20);
										goto case 240;
									} else {
										if (t.kind == 171) {
											stateStack.Push(20);
											goto case 21;
										} else {
											Error(t);
											goto case 20;
										}
									}
								}
							}
						}
					}
				}
			}
			case 20: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 21: {
				if (t == null) { currentState = 21; break; }
				Expect(171, t); // "Operator"
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) { currentState = 22; break; }
				currentState = 23;
				break;
			}
			case 23: {
				if (t == null) { currentState = 23; break; }
				Expect(36, t); // "("
				currentState = 24;
				break;
			}
			case 24: {
				stateStack.Push(25);
				goto case 235;
			}
			case 25: {
				if (t == null) { currentState = 25; break; }
				Expect(37, t); // ")"
				currentState = 26;
				break;
			}
			case 26: {
				if (t == null) { currentState = 26; break; }
				if (t.kind == 62) {
					currentState = 229;
					break;
				} else {
					goto case 27;
				}
			}
			case 27: {
				stateStack.Push(28);
				goto case 31;
			}
			case 28: {
				if (t == null) { currentState = 28; break; }
				Expect(112, t); // "End"
				currentState = 29;
				break;
			}
			case 29: {
				if (t == null) { currentState = 29; break; }
				Expect(171, t); // "Operator"
				currentState = 30;
				break;
			}
			case 30: {
				goto case 15;
			}
			case 31: {
				PushContext(Context.Body, t);
				goto case 32;
			}
			case 32: {
				stateStack.Push(33);
				goto case 15;
			}
			case 33: {
				if (t == null) { currentState = 33; break; }
				if (set[7, t.kind]) {
					if (set[8, t.kind]) {
						stateStack.Push(32);
						goto case 34;
					} else {
						goto case 32;
					}
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 34: {
				if (t == null) { currentState = 34; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					goto case 212;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						goto case 207;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							goto case 204;
						} else {
							if (t.kind == 187) {
								goto case 201;
							} else {
								if (t.kind == 134) {
									goto case 183;
								} else {
									if (t.kind == 195) {
										goto case 170;
									} else {
										if (t.kind == 229) {
											goto case 165;
										} else {
											if (t.kind == 107) {
												goto case 156;
											} else {
												if (t.kind == 123) {
													goto case 129;
												} else {
													if (t.kind == 117 || t.kind == 170 || t.kind == 192) {
														goto case 121;
													} else {
														if (set[9, t.kind]) {
															goto case 35;
														} else {
															goto case 6;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 35: {
				if (t == null) { currentState = 35; break; }
				if (t.kind == 72) {
					goto case 120;
				} else {
					goto case 36;
				}
			}
			case 36: {
				goto case 37;
			}
			case 37: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 38;
			}
			case 38: {
				if (t == null) { currentState = 38; break; }
				if (set[10, t.kind]) {
					goto case 48;
				} else {
					if (t.kind == 134) {
						goto case 39;
					} else {
						goto case 6;
					}
				}
			}
			case 39: {
				if (t == null) { currentState = 39; break; }
				Expect(134, t); // "If"
				currentState = 40;
				break;
			}
			case 40: {
				if (t == null) { currentState = 40; break; }
				Expect(36, t); // "("
				currentState = 41;
				break;
			}
			case 41: {
				stateStack.Push(42);
				goto case 37;
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				Expect(23, t); // ","
				currentState = 43;
				break;
			}
			case 43: {
				stateStack.Push(44);
				goto case 37;
			}
			case 44: {
				if (t == null) { currentState = 44; break; }
				if (t.kind == 23) {
					goto case 46;
				} else {
					goto case 45;
				}
			}
			case 45: {
				if (t == null) { currentState = 45; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				if (t == null) { currentState = 46; break; }
				currentState = 47;
				break;
			}
			case 47: {
				stateStack.Push(45);
				goto case 37;
			}
			case 48: {
				stateStack.Push(49);
				goto case 50;
			}
			case 49: {
				if (t == null) { currentState = 49; break; }
				if (set[11, t.kind]) {
					currentState = 48;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 50: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 51;
			}
			case 51: {
				if (t == null) { currentState = 51; break; }
				if (set[12, t.kind]) {
					currentState = 51;
					break;
				} else {
					if (set[13, t.kind]) {
						stateStack.Push(90);
						goto case 99;
					} else {
						if (t.kind == 218) {
							currentState = 87;
							break;
						} else {
							if (t.kind == 161) {
								goto case 52;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				Expect(161, t); // "New"
				currentState = 53;
				break;
			}
			case 53: {
				if (t == null) { currentState = 53; break; }
				if (set[14, t.kind]) {
					stateStack.Push(64);
					goto case 69;
				} else {
					goto case 54;
				}
			}
			case 54: {
				if (t == null) { currentState = 54; break; }
				if (t.kind == 231) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 55: {
				goto case 56;
			}
			case 56: {
				if (t == null) { currentState = 56; break; }
				Expect(34, t); // "{"
				currentState = 57;
				break;
			}
			case 57: {
				if (t == null) { currentState = 57; break; }
				if (t.kind == 146) {
					currentState = 58;
					break;
				} else {
					goto case 58;
				}
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				Expect(27, t); // "."
				currentState = 59;
				break;
			}
			case 59: {
				stateStack.Push(60);
				goto case 16;
			}
			case 60: {
				if (t == null) { currentState = 60; break; }
				Expect(21, t); // "="
				currentState = 61;
				break;
			}
			case 61: {
				stateStack.Push(62);
				goto case 37;
			}
			case 62: {
				if (t == null) { currentState = 62; break; }
				if (t.kind == 23) {
					currentState = 57;
					break;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				Expect(35, t); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 64: {
				if (t == null) { currentState = 64; break; }
				if (t.kind == 125) {
					currentState = 65;
					break;
				} else {
					goto case 54;
				}
			}
			case 65: {
				stateStack.Push(54);
				goto case 66;
			}
			case 66: {
				if (t == null) { currentState = 66; break; }
				Expect(34, t); // "{"
				currentState = 67;
				break;
			}
			case 67: {
				if (t == null) { currentState = 67; break; }
				if (set[15, t.kind]) {
					stateStack.Push(68);
					goto case 37;
				} else {
					if (t.kind == 34) {
						stateStack.Push(68);
						goto case 66;
					} else {
						Error(t);
						goto case 68;
					}
				}
			}
			case 68: {
				if (t == null) { currentState = 68; break; }
				if (t.kind == 23) {
					currentState = 67;
					break;
				} else {
					goto case 63;
				}
			}
			case 69: {
				if (t == null) { currentState = 69; break; }
				if (t.kind == 129) {
					goto case 83;
				} else {
					if (set[16, t.kind]) {
						stateStack.Push(70);
						goto case 84;
					} else {
						if (set[17, t.kind]) {
							goto case 83;
						} else {
							Error(t);
							goto case 70;
						}
					}
				}
			}
			case 70: {
				if (t == null) { currentState = 70; break; }
				if (t.kind == 36) {
					stateStack.Push(70);
					goto case 74;
				} else {
					goto case 71;
				}
			}
			case 71: {
				if (t == null) { currentState = 71; break; }
				if (t.kind == 27) {
					currentState = 72;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 72: {
				stateStack.Push(73);
				goto case 16;
			}
			case 73: {
				if (t == null) { currentState = 73; break; }
				if (t.kind == 36) {
					stateStack.Push(73);
					goto case 74;
				} else {
					goto case 71;
				}
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				Expect(36, t); // "("
				currentState = 75;
				break;
			}
			case 75: {
				if (t == null) { currentState = 75; break; }
				if (t.kind == 168) {
					goto case 80;
				} else {
					if (set[18, t.kind]) {
						goto case 76;
					} else {
						Error(t);
						goto case 45;
					}
				}
			}
			case 76: {
				if (t == null) { currentState = 76; break; }
				if (set[15, t.kind]) {
					goto case 77;
				} else {
					goto case 45;
				}
			}
			case 77: {
				stateStack.Push(45);
				goto case 78;
			}
			case 78: {
				stateStack.Push(79);
				goto case 37;
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				if (t.kind == 23) {
					currentState = 78;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				currentState = 81;
				break;
			}
			case 81: {
				if (t == null) { currentState = 81; break; }
				if (set[14, t.kind]) {
					stateStack.Push(82);
					goto case 69;
				} else {
					goto case 82;
				}
			}
			case 82: {
				if (t == null) { currentState = 82; break; }
				if (t.kind == 23) {
					goto case 80;
				} else {
					goto case 45;
				}
			}
			case 83: {
				if (t == null) { currentState = 83; break; }
				currentState = 70;
				break;
			}
			case 84: {
				PushContext(Context.IdentifierExpected, t);
				goto case 85;
			}
			case 85: {
				if (t == null) { currentState = 85; break; }
				if (set[16, t.kind]) {
					currentState = 86;
					break;
				} else {
					Error(t);
					goto case 86;
				}
			}
			case 86: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 87: {
				stateStack.Push(88);
				goto case 50;
			}
			case 88: {
				if (t == null) { currentState = 88; break; }
				Expect(143, t); // "Is"
				currentState = 89;
				break;
			}
			case 89: {
				goto case 69;
			}
			case 90: {
				if (t == null) { currentState = 90; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(90);
					goto case 91;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 91: {
				if (t == null) { currentState = 91; break; }
				if (t.kind == 36) {
					currentState = 94;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 92;
					} else {
						goto case 6;
					}
				}
			}
			case 92: {
				if (t == null) { currentState = 92; break; }
				currentState = 93;
				break;
			}
			case 93: {
				goto case 16;
			}
			case 94: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 95;
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (t.kind == 168) {
					goto case 96;
				} else {
					if (set[15, t.kind]) {
						goto case 77;
					} else {
						goto case 6;
					}
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				currentState = 97;
				break;
			}
			case 97: {
				stateStack.Push(98);
				goto case 69;
			}
			case 98: {
				if (t == null) { currentState = 98; break; }
				if (t.kind == 23) {
					goto case 96;
				} else {
					goto case 45;
				}
			}
			case 99: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 100;
			}
			case 100: {
				if (t == null) { currentState = 100; break; }
				if (set[19, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						goto case 46;
					} else {
						if (set[16, t.kind]) {
							goto case 119;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 92;
							} else {
								if (t.kind == 128) {
									currentState = 118;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 116;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											goto case 108;
										} else {
											if (set[20, t.kind]) {
												goto case 101;
											} else {
												goto case 6;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				if (set[21, t.kind]) {
					goto case 106;
				} else {
					if (t.kind == 93 || t.kind == 105 || t.kind == 217) {
						currentState = 102;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 102: {
				if (t == null) { currentState = 102; break; }
				Expect(36, t); // "("
				currentState = 103;
				break;
			}
			case 103: {
				stateStack.Push(104);
				goto case 37;
			}
			case 104: {
				if (t == null) { currentState = 104; break; }
				Expect(23, t); // ","
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(45);
				goto case 69;
			}
			case 106: {
				if (t == null) { currentState = 106; break; }
				if (set[21, t.kind]) {
					currentState = 107;
					break;
				} else {
					Error(t);
					goto case 107;
				}
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				Expect(36, t); // "("
				currentState = 47;
				break;
			}
			case 108: {
				PushContext(Context.Xml, t);
				goto case 109;
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				if (t.kind == 17) {
					currentState = 109;
					break;
				} else {
					stateStack.Push(110);
					goto case 111;
				}
			}
			case 110: {
				if (t == null) { currentState = 110; break; }
				if (t.kind == 17) {
					currentState = 110;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 112;
				break;
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				if (set[22, t.kind]) {
					currentState = 112;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 113;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 113: {
				if (t == null) { currentState = 113; break; }
				currentState = 114;
				break;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				if (set[23, t.kind]) {
					if (set[24, t.kind]) {
						goto case 113;
					} else {
						if (t.kind == 10) {
							stateStack.Push(114);
							goto case 111;
						} else {
							Error(t);
							goto case 114;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 115;
					break;
				}
			}
			case 115: {
				if (t == null) { currentState = 115; break; }
				if (set[25, t.kind]) {
					currentState = 115;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				Expect(36, t); // "("
				currentState = 117;
				break;
			}
			case 117: {
				readXmlIdentifier = true;
				stateStack.Push(45);
				goto case 84;
			}
			case 118: {
				if (t == null) { currentState = 118; break; }
				Expect(36, t); // "("
				currentState = 105;
				break;
			}
			case 119: {
				goto case 84;
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				currentState = 36;
				break;
			}
			case 121: {
				if (t == null) { currentState = 121; break; }
				if (t.kind == 117 || t.kind == 170) {
					if (t.kind == 170) {
						currentState = 124;
						break;
					} else {
						goto case 124;
					}
				} else {
					if (t.kind == 192) {
						currentState = 122;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 122: {
				if (t == null) { currentState = 122; break; }
				if (t.kind == 5 || t.kind == 162) {
					goto case 16;
				} else {
					goto case 123;
				}
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				if (set[16, t.kind]) {
					goto case 119;
				} else {
					goto case 6;
				}
			}
			case 124: {
				if (t == null) { currentState = 124; break; }
				Expect(117, t); // "Error"
				currentState = 125;
				break;
			}
			case 125: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 126;
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				if (set[15, t.kind]) {
					goto case 36;
				} else {
					if (t.kind == 131) {
						currentState = 128;
						break;
					} else {
						if (t.kind == 192) {
							currentState = 127;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 127: {
				if (t == null) { currentState = 127; break; }
				Expect(162, t); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				if (t.kind == 5) {
					goto case 16;
				} else {
					goto case 123;
				}
			}
			case 129: {
				if (t == null) { currentState = 129; break; }
				Expect(123, t); // "For"
				currentState = 130;
				break;
			}
			case 130: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 131;
			}
			case 131: {
				if (t == null) { currentState = 131; break; }
				if (set[13, t.kind]) {
					goto case 145;
				} else {
					if (t.kind == 109) {
						goto case 132;
					} else {
						goto case 6;
					}
				}
			}
			case 132: {
				if (t == null) { currentState = 132; break; }
				Expect(109, t); // "Each"
				currentState = 133;
				break;
			}
			case 133: {
				stateStack.Push(134);
				goto case 142;
			}
			case 134: {
				if (t == null) { currentState = 134; break; }
				Expect(137, t); // "In"
				currentState = 135;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 37;
			}
			case 136: {
				stateStack.Push(137);
				goto case 31;
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				Expect(162, t); // "Next"
				currentState = 138;
				break;
			}
			case 138: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 139;
			}
			case 139: {
				if (t == null) { currentState = 139; break; }
				if (set[15, t.kind]) {
					goto case 140;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				stateStack.Push(141);
				goto case 37;
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				if (t.kind == 23) {
					currentState = 140;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 142: {
				stateStack.Push(143);
				goto case 99;
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				if (t.kind == 32) {
					currentState = 144;
					break;
				} else {
					goto case 144;
				}
			}
			case 144: {
				if (t == null) { currentState = 144; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(144);
					goto case 91;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 145: {
				stateStack.Push(146);
				goto case 142;
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				Expect(21, t); // "="
				currentState = 147;
				break;
			}
			case 147: {
				stateStack.Push(148);
				goto case 37;
			}
			case 148: {
				if (t == null) { currentState = 148; break; }
				if (t.kind == 203) {
					currentState = 155;
					break;
				} else {
					goto case 149;
				}
			}
			case 149: {
				stateStack.Push(150);
				goto case 31;
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				Expect(162, t); // "Next"
				currentState = 151;
				break;
			}
			case 151: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 152;
			}
			case 152: {
				if (t == null) { currentState = 152; break; }
				if (set[15, t.kind]) {
					goto case 153;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 153: {
				stateStack.Push(154);
				goto case 37;
			}
			case 154: {
				if (t == null) { currentState = 154; break; }
				if (t.kind == 23) {
					currentState = 153;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 155: {
				stateStack.Push(149);
				goto case 37;
			}
			case 156: {
				if (t == null) { currentState = 156; break; }
				Expect(107, t); // "Do"
				currentState = 157;
				break;
			}
			case 157: {
				if (t == null) { currentState = 157; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 161;
				} else {
					if (t.kind == 1 || t.kind == 22) {
						goto case 158;
					} else {
						goto case 6;
					}
				}
			}
			case 158: {
				stateStack.Push(159);
				goto case 31;
			}
			case 159: {
				if (t == null) { currentState = 159; break; }
				Expect(151, t); // "Loop"
				currentState = 160;
				break;
			}
			case 160: {
				if (t == null) { currentState = 160; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 120;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				if (t == null) { currentState = 161; break; }
				if (t.kind == 222 || t.kind == 229) {
					currentState = 162;
					break;
				} else {
					Error(t);
					goto case 162;
				}
			}
			case 162: {
				stateStack.Push(163);
				goto case 37;
			}
			case 163: {
				stateStack.Push(164);
				goto case 31;
			}
			case 164: {
				if (t == null) { currentState = 164; break; }
				Expect(151, t); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				Expect(229, t); // "While"
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(167);
				goto case 37;
			}
			case 167: {
				stateStack.Push(168);
				goto case 31;
			}
			case 168: {
				if (t == null) { currentState = 168; break; }
				Expect(112, t); // "End"
				currentState = 169;
				break;
			}
			case 169: {
				if (t == null) { currentState = 169; break; }
				Expect(229, t); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 170: {
				if (t == null) { currentState = 170; break; }
				Expect(195, t); // "Select"
				currentState = 171;
				break;
			}
			case 171: {
				if (t == null) { currentState = 171; break; }
				if (t.kind == 73) {
					currentState = 172;
					break;
				} else {
					goto case 172;
				}
			}
			case 172: {
				stateStack.Push(173);
				goto case 37;
			}
			case 173: {
				stateStack.Push(174);
				goto case 15;
			}
			case 174: {
				if (t == null) { currentState = 174; break; }
				if (t.kind == 73) {
					currentState = 176;
					break;
				} else {
					Expect(112, t); // "End"
					currentState = 175;
					break;
				}
			}
			case 175: {
				if (t == null) { currentState = 175; break; }
				Expect(195, t); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 176: {
				if (t == null) { currentState = 176; break; }
				if (t.kind == 110) {
					currentState = 177;
					break;
				} else {
					if (set[26, t.kind]) {
						goto case 178;
					} else {
						Error(t);
						goto case 177;
					}
				}
			}
			case 177: {
				stateStack.Push(174);
				goto case 31;
			}
			case 178: {
				if (t == null) { currentState = 178; break; }
				if (set[27, t.kind]) {
					if (t.kind == 143) {
						currentState = 180;
						break;
					} else {
						goto case 180;
					}
				} else {
					if (set[15, t.kind]) {
						stateStack.Push(179);
						goto case 37;
					} else {
						Error(t);
						goto case 179;
					}
				}
			}
			case 179: {
				if (t == null) { currentState = 179; break; }
				if (t.kind == 23) {
					currentState = 178;
					break;
				} else {
					goto case 177;
				}
			}
			case 180: {
				stateStack.Push(181);
				goto case 182;
			}
			case 181: {
				stateStack.Push(179);
				goto case 50;
			}
			case 182: {
				if (t == null) { currentState = 182; break; }
				if (set[28, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 183: {
				if (t == null) { currentState = 183; break; }
				Expect(134, t); // "If"
				currentState = 184;
				break;
			}
			case 184: {
				stateStack.Push(185);
				goto case 37;
			}
			case 185: {
				if (t == null) { currentState = 185; break; }
				if (t.kind == 212) {
					currentState = 194;
					break;
				} else {
					goto case 186;
				}
			}
			case 186: {
				if (t == null) { currentState = 186; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 187;
				} else {
					goto case 6;
				}
			}
			case 187: {
				stateStack.Push(188);
				goto case 31;
			}
			case 188: {
				if (t == null) { currentState = 188; break; }
				if (t.kind == 110 || t.kind == 111) {
					if (t.kind == 110) {
						currentState = 193;
						break;
					} else {
						if (t.kind == 111) {
							goto case 190;
						} else {
							Error(t);
							goto case 187;
						}
					}
				} else {
					Expect(112, t); // "End"
					currentState = 189;
					break;
				}
			}
			case 189: {
				if (t == null) { currentState = 189; break; }
				Expect(134, t); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				currentState = 191;
				break;
			}
			case 191: {
				stateStack.Push(192);
				goto case 37;
			}
			case 192: {
				if (t == null) { currentState = 192; break; }
				if (t.kind == 212) {
					currentState = 187;
					break;
				} else {
					goto case 187;
				}
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				if (t.kind == 134) {
					goto case 190;
				} else {
					goto case 187;
				}
			}
			case 194: {
				if (t == null) { currentState = 194; break; }
				if (set[8, t.kind]) {
					goto case 195;
				} else {
					goto case 186;
				}
			}
			case 195: {
				stateStack.Push(196);
				goto case 34;
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				if (t.kind == 22) {
					currentState = 195;
					break;
				} else {
					if (t.kind == 110) {
						goto case 198;
					} else {
						goto case 197;
					}
				}
			}
			case 197: {
				if (t == null) { currentState = 197; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 198: {
				if (t == null) { currentState = 198; break; }
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(200);
				goto case 34;
			}
			case 200: {
				if (t == null) { currentState = 200; break; }
				if (t.kind == 22) {
					goto case 198;
				} else {
					goto case 197;
				}
			}
			case 201: {
				if (t == null) { currentState = 201; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 202;
				break;
			}
			case 202: {
				stateStack.Push(203);
				goto case 16;
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (t.kind == 36) {
					currentState = 76;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				if (t == null) { currentState = 204; break; }
				if (t.kind == 55 || t.kind == 191) {
					currentState = 205;
					break;
				} else {
					Error(t);
					goto case 205;
				}
			}
			case 205: {
				stateStack.Push(206);
				goto case 37;
			}
			case 206: {
				if (t == null) { currentState = 206; break; }
				Expect(23, t); // ","
				currentState = 36;
				break;
			}
			case 207: {
				if (t == null) { currentState = 207; break; }
				if (t.kind == 209 || t.kind == 231) {
					currentState = 208;
					break;
				} else {
					Error(t);
					goto case 208;
				}
			}
			case 208: {
				stateStack.Push(209);
				goto case 37;
			}
			case 209: {
				stateStack.Push(210);
				goto case 31;
			}
			case 210: {
				if (t == null) { currentState = 210; break; }
				Expect(112, t); // "End"
				currentState = 211;
				break;
			}
			case 211: {
				if (t == null) { currentState = 211; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 212: {
				if (t == null) { currentState = 212; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 213;
					break;
				} else {
					Error(t);
					goto case 213;
				}
			}
			case 213: {
				stateStack.Push(214);
				goto case 84;
			}
			case 214: {
				if (t == null) { currentState = 214; break; }
				if (t.kind == 32) {
					currentState = 215;
					break;
				} else {
					goto case 215;
				}
			}
			case 215: {
				if (t == null) { currentState = 215; break; }
				if (t.kind == 36) {
					goto case 227;
				} else {
					goto case 216;
				}
			}
			case 216: {
				if (t == null) { currentState = 216; break; }
				if (t.kind == 23) {
					currentState = 221;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 218;
						break;
					} else {
						goto case 217;
					}
				}
			}
			case 217: {
				if (t == null) { currentState = 217; break; }
				if (t.kind == 21) {
					goto case 120;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 218: {
				if (t == null) { currentState = 218; break; }
				if (t.kind == 161) {
					goto case 220;
				} else {
					goto case 219;
				}
			}
			case 219: {
				stateStack.Push(217);
				goto case 69;
			}
			case 220: {
				if (t == null) { currentState = 220; break; }
				currentState = 219;
				break;
			}
			case 221: {
				stateStack.Push(222);
				goto case 84;
			}
			case 222: {
				if (t == null) { currentState = 222; break; }
				if (t.kind == 32) {
					currentState = 223;
					break;
				} else {
					goto case 223;
				}
			}
			case 223: {
				if (t == null) { currentState = 223; break; }
				if (t.kind == 36) {
					goto case 224;
				} else {
					goto case 216;
				}
			}
			case 224: {
				if (t == null) { currentState = 224; break; }
				currentState = 225;
				break;
			}
			case 225: {
				if (t == null) { currentState = 225; break; }
				if (t.kind == 23) {
					goto case 224;
				} else {
					goto case 226;
				}
			}
			case 226: {
				if (t == null) { currentState = 226; break; }
				Expect(37, t); // ")"
				currentState = 216;
				break;
			}
			case 227: {
				if (t == null) { currentState = 227; break; }
				currentState = 228;
				break;
			}
			case 228: {
				if (t == null) { currentState = 228; break; }
				if (t.kind == 23) {
					goto case 227;
				} else {
					goto case 226;
				}
			}
			case 229: {
				if (t == null) { currentState = 229; break; }
				if (t.kind == 39) {
					stateStack.Push(229);
					goto case 230;
				} else {
					stateStack.Push(27);
					goto case 69;
				}
			}
			case 230: {
				if (t == null) { currentState = 230; break; }
				Expect(39, t); // "<"
				currentState = 231;
				break;
			}
			case 231: {
				PushContext(Context.Attribute, t);
				goto case 232;
			}
			case 232: {
				if (t == null) { currentState = 232; break; }
				if (set[29, t.kind]) {
					currentState = 232;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 233;
					break;
				}
			}
			case 233: {
				PopContext();
				goto case 234;
			}
			case 234: {
				if (t == null) { currentState = 234; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 235: {
				stateStack.Push(236);
				goto case 237;
			}
			case 236: {
				if (t == null) { currentState = 236; break; }
				if (t.kind == 23) {
					currentState = 235;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 237: {
				if (t == null) { currentState = 237; break; }
				if (t.kind == 39) {
					stateStack.Push(237);
					goto case 230;
				} else {
					goto case 238;
				}
			}
			case 238: {
				if (t == null) { currentState = 238; break; }
				if (set[30, t.kind]) {
					currentState = 238;
					break;
				} else {
					stateStack.Push(239);
					goto case 84;
				}
			}
			case 239: {
				if (t == null) { currentState = 239; break; }
				if (t.kind == 62) {
					goto case 220;
				} else {
					goto case 217;
				}
			}
			case 240: {
				if (t == null) { currentState = 240; break; }
				Expect(97, t); // "Custom"
				currentState = 241;
				break;
			}
			case 241: {
				stateStack.Push(242);
				goto case 252;
			}
			case 242: {
				if (t == null) { currentState = 242; break; }
				if (set[31, t.kind]) {
					goto case 244;
				} else {
					Expect(112, t); // "End"
					currentState = 243;
					break;
				}
			}
			case 243: {
				if (t == null) { currentState = 243; break; }
				Expect(118, t); // "Event"
				currentState = 30;
				break;
			}
			case 244: {
				if (t == null) { currentState = 244; break; }
				if (t.kind == 39) {
					stateStack.Push(244);
					goto case 230;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 245;
						break;
					} else {
						Error(t);
						goto case 245;
					}
				}
			}
			case 245: {
				if (t == null) { currentState = 245; break; }
				Expect(36, t); // "("
				currentState = 246;
				break;
			}
			case 246: {
				stateStack.Push(247);
				goto case 235;
			}
			case 247: {
				if (t == null) { currentState = 247; break; }
				Expect(37, t); // ")"
				currentState = 248;
				break;
			}
			case 248: {
				stateStack.Push(249);
				goto case 31;
			}
			case 249: {
				if (t == null) { currentState = 249; break; }
				Expect(112, t); // "End"
				currentState = 250;
				break;
			}
			case 250: {
				if (t == null) { currentState = 250; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 251;
					break;
				} else {
					Error(t);
					goto case 251;
				}
			}
			case 251: {
				stateStack.Push(242);
				goto case 15;
			}
			case 252: {
				if (t == null) { currentState = 252; break; }
				Expect(118, t); // "Event"
				currentState = 253;
				break;
			}
			case 253: {
				stateStack.Push(254);
				goto case 84;
			}
			case 254: {
				if (t == null) { currentState = 254; break; }
				if (t.kind == 62) {
					currentState = 261;
					break;
				} else {
					if (set[32, t.kind]) {
						if (t.kind == 36) {
							currentState = 259;
							break;
						} else {
							goto case 255;
						}
					} else {
						Error(t);
						goto case 255;
					}
				}
			}
			case 255: {
				if (t == null) { currentState = 255; break; }
				if (t.kind == 135) {
					goto case 256;
				} else {
					goto case 30;
				}
			}
			case 256: {
				if (t == null) { currentState = 256; break; }
				currentState = 257;
				break;
			}
			case 257: {
				stateStack.Push(258);
				goto case 69;
			}
			case 258: {
				if (t == null) { currentState = 258; break; }
				if (t.kind == 23) {
					goto case 256;
				} else {
					goto case 30;
				}
			}
			case 259: {
				if (t == null) { currentState = 259; break; }
				if (set[33, t.kind]) {
					stateStack.Push(260);
					goto case 235;
				} else {
					goto case 260;
				}
			}
			case 260: {
				if (t == null) { currentState = 260; break; }
				Expect(37, t); // ")"
				currentState = 255;
				break;
			}
			case 261: {
				stateStack.Push(255);
				goto case 69;
			}
			case 262: {
				if (t == null) { currentState = 262; break; }
				Expect(100, t); // "Declare"
				currentState = 263;
				break;
			}
			case 263: {
				if (t == null) { currentState = 263; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 264;
					break;
				} else {
					goto case 264;
				}
			}
			case 264: {
				if (t == null) { currentState = 264; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 265;
					break;
				} else {
					Error(t);
					goto case 265;
				}
			}
			case 265: {
				stateStack.Push(266);
				goto case 84;
			}
			case 266: {
				if (t == null) { currentState = 266; break; }
				Expect(148, t); // "Lib"
				currentState = 267;
				break;
			}
			case 267: {
				if (t == null) { currentState = 267; break; }
				Expect(3, t); // LiteralString
				currentState = 268;
				break;
			}
			case 268: {
				if (t == null) { currentState = 268; break; }
				if (t.kind == 58) {
					currentState = 272;
					break;
				} else {
					goto case 269;
				}
			}
			case 269: {
				if (t == null) { currentState = 269; break; }
				if (t.kind == 36) {
					currentState = 270;
					break;
				} else {
					goto case 30;
				}
			}
			case 270: {
				if (t == null) { currentState = 270; break; }
				if (set[33, t.kind]) {
					stateStack.Push(271);
					goto case 235;
				} else {
					goto case 271;
				}
			}
			case 271: {
				if (t == null) { currentState = 271; break; }
				Expect(37, t); // ")"
				currentState = 30;
				break;
			}
			case 272: {
				if (t == null) { currentState = 272; break; }
				Expect(3, t); // LiteralString
				currentState = 269;
				break;
			}
			case 273: {
				if (t == null) { currentState = 273; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 274;
					break;
				} else {
					Error(t);
					goto case 274;
				}
			}
			case 274: {
				PushContext(Context.IdentifierExpected, t);
				goto case 275;
			}
			case 275: {
				if (t == null) { currentState = 275; break; }
				currentState = 276;
				break;
			}
			case 276: {
				PopContext();
				goto case 277;
			}
			case 277: {
				if (t == null) { currentState = 277; break; }
				if (t.kind == 36) {
					currentState = 283;
					break;
				} else {
					goto case 278;
				}
			}
			case 278: {
				if (t == null) { currentState = 278; break; }
				if (t.kind == 62) {
					currentState = 282;
					break;
				} else {
					goto case 279;
				}
			}
			case 279: {
				stateStack.Push(280);
				goto case 31;
			}
			case 280: {
				if (t == null) { currentState = 280; break; }
				Expect(112, t); // "End"
				currentState = 281;
				break;
			}
			case 281: {
				if (t == null) { currentState = 281; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 30;
					break;
				} else {
					Error(t);
					goto case 30;
				}
			}
			case 282: {
				stateStack.Push(279);
				goto case 69;
			}
			case 283: {
				if (t == null) { currentState = 283; break; }
				if (set[33, t.kind]) {
					stateStack.Push(284);
					goto case 235;
				} else {
					goto case 284;
				}
			}
			case 284: {
				if (t == null) { currentState = 284; break; }
				Expect(37, t); // ")"
				currentState = 278;
				break;
			}
			case 285: {
				if (t == null) { currentState = 285; break; }
				if (t.kind == 87) {
					currentState = 286;
					break;
				} else {
					goto case 286;
				}
			}
			case 286: {
				stateStack.Push(287);
				goto case 291;
			}
			case 287: {
				if (t == null) { currentState = 287; break; }
				if (t.kind == 62) {
					currentState = 290;
					break;
				} else {
					goto case 288;
				}
			}
			case 288: {
				if (t == null) { currentState = 288; break; }
				if (t.kind == 21) {
					currentState = 289;
					break;
				} else {
					goto case 30;
				}
			}
			case 289: {
				stateStack.Push(30);
				goto case 37;
			}
			case 290: {
				stateStack.Push(288);
				goto case 69;
			}
			case 291: {
				if (t == null) { currentState = 291; break; }
				if (set[34, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 292: {
				if (t == null) { currentState = 292; break; }
				currentState = 9;
				break;
			}
			case 293: {
				if (t == null) { currentState = 293; break; }
				Expect(159, t); // "Namespace"
				currentState = 294;
				break;
			}
			case 294: {
				if (t == null) { currentState = 294; break; }
				if (set[3, t.kind]) {
					currentState = 294;
					break;
				} else {
					stateStack.Push(295);
					goto case 15;
				}
			}
			case 295: {
				if (t == null) { currentState = 295; break; }
				if (set[35, t.kind]) {
					stateStack.Push(295);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 296;
					break;
				}
			}
			case 296: {
				if (t == null) { currentState = 296; break; }
				Expect(159, t); // "Namespace"
				currentState = 30;
				break;
			}
			case 297: {
				if (t == null) { currentState = 297; break; }
				Expect(136, t); // "Imports"
				currentState = 298;
				break;
			}
			case 298: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 299;
			}
			case 299: {
				if (t == null) { currentState = 299; break; }
				if (set[3, t.kind]) {
					currentState = 299;
					break;
				} else {
					goto case 30;
				}
			}
			case 300: {
				if (t == null) { currentState = 300; break; }
				Expect(172, t); // "Option"
				currentState = 301;
				break;
			}
			case 301: {
				if (t == null) { currentState = 301; break; }
				if (set[3, t.kind]) {
					currentState = 301;
					break;
				} else {
					goto case 30;
				}
			}
		}

	}
	
	public void Advance()
	{
		Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, x,T,x,x, T,T,x,T, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, x,T,x,x, T,T,x,T, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,T, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};

} // end Parser


}