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
					goto case 322;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 319;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 252;
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
					goto case 315;
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
					goto case 252;
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
						goto case 314;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 314;
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
					goto case 252;
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
						goto case 307;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(20);
							goto case 295;
						} else {
							if (t.kind == 100) {
								stateStack.Push(20);
								goto case 284;
							} else {
								if (t.kind == 118) {
									stateStack.Push(20);
									goto case 274;
								} else {
									if (t.kind == 97) {
										stateStack.Push(20);
										goto case 262;
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
				goto case 257;
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
					currentState = 251;
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
					goto case 234;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						goto case 229;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							goto case 226;
						} else {
							if (t.kind == 187) {
								goto case 223;
							} else {
								if (t.kind == 134) {
									goto case 204;
								} else {
									if (t.kind == 195) {
										goto case 189;
									} else {
										if (t.kind == 229) {
											goto case 184;
										} else {
											if (t.kind == 107) {
												goto case 175;
											} else {
												if (t.kind == 123) {
													goto case 148;
												} else {
													if (t.kind == 117 || t.kind == 170 || t.kind == 192) {
														goto case 140;
													} else {
														if (t.kind == 213) {
															goto case 137;
														} else {
															if (t.kind == 216) {
																goto case 126;
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
				}
			}
			case 35: {
				if (t == null) { currentState = 35; break; }
				if (t.kind == 72) {
					goto case 125;
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
						stateStack.Push(95);
						goto case 104;
					} else {
						if (t.kind == 218) {
							currentState = 92;
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
					goto case 88;
				} else {
					if (set[16, t.kind]) {
						stateStack.Push(70);
						goto case 89;
					} else {
						if (set[17, t.kind]) {
							goto case 88;
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
					goto case 85;
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
				if (set[19, t.kind]) {
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
				if (t == null) { currentState = 78; break; }
				if (set[15, t.kind]) {
					goto case 82;
				} else {
					if (t.kind == 23) {
						goto case 79;
					} else {
						goto case 6;
					}
				}
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				currentState = 80;
				break;
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				if (set[15, t.kind]) {
					stateStack.Push(81);
					goto case 37;
				} else {
					goto case 81;
				}
			}
			case 81: {
				if (t == null) { currentState = 81; break; }
				if (t.kind == 23) {
					goto case 79;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 82: {
				stateStack.Push(83);
				goto case 37;
			}
			case 83: {
				if (t == null) { currentState = 83; break; }
				if (t.kind == 23) {
					currentState = 84;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 84: {
				if (t == null) { currentState = 84; break; }
				if (set[15, t.kind]) {
					goto case 82;
				} else {
					goto case 83;
				}
			}
			case 85: {
				if (t == null) { currentState = 85; break; }
				currentState = 86;
				break;
			}
			case 86: {
				if (t == null) { currentState = 86; break; }
				if (set[14, t.kind]) {
					stateStack.Push(87);
					goto case 69;
				} else {
					goto case 87;
				}
			}
			case 87: {
				if (t == null) { currentState = 87; break; }
				if (t.kind == 23) {
					goto case 85;
				} else {
					goto case 45;
				}
			}
			case 88: {
				if (t == null) { currentState = 88; break; }
				currentState = 70;
				break;
			}
			case 89: {
				PushContext(Context.IdentifierExpected, t);
				goto case 90;
			}
			case 90: {
				if (t == null) { currentState = 90; break; }
				if (set[16, t.kind]) {
					currentState = 91;
					break;
				} else {
					Error(t);
					goto case 91;
				}
			}
			case 91: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 92: {
				stateStack.Push(93);
				goto case 50;
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				Expect(143, t); // "Is"
				currentState = 94;
				break;
			}
			case 94: {
				goto case 69;
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(95);
					goto case 96;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				if (t.kind == 36) {
					currentState = 99;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 97;
					} else {
						goto case 6;
					}
				}
			}
			case 97: {
				if (t == null) { currentState = 97; break; }
				currentState = 98;
				break;
			}
			case 98: {
				goto case 16;
			}
			case 99: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 100;
			}
			case 100: {
				if (t == null) { currentState = 100; break; }
				if (t.kind == 168) {
					goto case 101;
				} else {
					if (set[19, t.kind]) {
						goto case 77;
					} else {
						goto case 6;
					}
				}
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(103);
				goto case 69;
			}
			case 103: {
				if (t == null) { currentState = 103; break; }
				if (t.kind == 23) {
					goto case 101;
				} else {
					goto case 45;
				}
			}
			case 104: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 105;
			}
			case 105: {
				if (t == null) { currentState = 105; break; }
				if (set[20, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						goto case 46;
					} else {
						if (set[16, t.kind]) {
							goto case 124;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 97;
							} else {
								if (t.kind == 128) {
									currentState = 123;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 121;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											goto case 113;
										} else {
											if (set[21, t.kind]) {
												goto case 106;
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
			case 106: {
				if (t == null) { currentState = 106; break; }
				if (set[22, t.kind]) {
					goto case 111;
				} else {
					if (t.kind == 93 || t.kind == 105 || t.kind == 217) {
						currentState = 107;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				Expect(36, t); // "("
				currentState = 108;
				break;
			}
			case 108: {
				stateStack.Push(109);
				goto case 37;
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				Expect(23, t); // ","
				currentState = 110;
				break;
			}
			case 110: {
				stateStack.Push(45);
				goto case 69;
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				if (set[22, t.kind]) {
					currentState = 112;
					break;
				} else {
					Error(t);
					goto case 112;
				}
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				Expect(36, t); // "("
				currentState = 47;
				break;
			}
			case 113: {
				PushContext(Context.Xml, t);
				goto case 114;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				if (t.kind == 17) {
					currentState = 114;
					break;
				} else {
					stateStack.Push(115);
					goto case 116;
				}
			}
			case 115: {
				if (t == null) { currentState = 115; break; }
				if (t.kind == 17) {
					currentState = 115;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 117;
				break;
			}
			case 117: {
				if (t == null) { currentState = 117; break; }
				if (set[23, t.kind]) {
					currentState = 117;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 118;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 118: {
				if (t == null) { currentState = 118; break; }
				currentState = 119;
				break;
			}
			case 119: {
				if (t == null) { currentState = 119; break; }
				if (set[24, t.kind]) {
					if (set[25, t.kind]) {
						goto case 118;
					} else {
						if (t.kind == 10) {
							stateStack.Push(119);
							goto case 116;
						} else {
							Error(t);
							goto case 119;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 120;
					break;
				}
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				if (set[26, t.kind]) {
					currentState = 120;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 121: {
				if (t == null) { currentState = 121; break; }
				Expect(36, t); // "("
				currentState = 122;
				break;
			}
			case 122: {
				readXmlIdentifier = true;
				stateStack.Push(45);
				goto case 89;
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				Expect(36, t); // "("
				currentState = 110;
				break;
			}
			case 124: {
				goto case 89;
			}
			case 125: {
				if (t == null) { currentState = 125; break; }
				currentState = 36;
				break;
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				Expect(216, t); // "Try"
				currentState = 127;
				break;
			}
			case 127: {
				stateStack.Push(128);
				goto case 31;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				if (t.kind == 74) {
					currentState = 132;
					break;
				} else {
					if (t.kind == 122) {
						currentState = 131;
						break;
					} else {
						goto case 129;
					}
				}
			}
			case 129: {
				if (t == null) { currentState = 129; break; }
				Expect(112, t); // "End"
				currentState = 130;
				break;
			}
			case 130: {
				if (t == null) { currentState = 130; break; }
				Expect(216, t); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 131: {
				stateStack.Push(129);
				goto case 31;
			}
			case 132: {
				if (t == null) { currentState = 132; break; }
				if (set[16, t.kind]) {
					stateStack.Push(135);
					goto case 89;
				} else {
					goto case 133;
				}
			}
			case 133: {
				if (t == null) { currentState = 133; break; }
				if (t.kind == 227) {
					currentState = 134;
					break;
				} else {
					goto case 127;
				}
			}
			case 134: {
				stateStack.Push(127);
				goto case 37;
			}
			case 135: {
				if (t == null) { currentState = 135; break; }
				if (t.kind == 62) {
					currentState = 136;
					break;
				} else {
					goto case 133;
				}
			}
			case 136: {
				stateStack.Push(133);
				goto case 69;
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				Expect(213, t); // "Throw"
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
					goto case 36;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				if (t.kind == 117 || t.kind == 170) {
					if (t.kind == 170) {
						currentState = 143;
						break;
					} else {
						goto case 143;
					}
				} else {
					if (t.kind == 192) {
						currentState = 141;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				if (t.kind == 5 || t.kind == 162) {
					goto case 16;
				} else {
					goto case 142;
				}
			}
			case 142: {
				if (t == null) { currentState = 142; break; }
				if (set[16, t.kind]) {
					goto case 124;
				} else {
					goto case 6;
				}
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				Expect(117, t); // "Error"
				currentState = 144;
				break;
			}
			case 144: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 145;
			}
			case 145: {
				if (t == null) { currentState = 145; break; }
				if (set[15, t.kind]) {
					goto case 36;
				} else {
					if (t.kind == 131) {
						currentState = 147;
						break;
					} else {
						if (t.kind == 192) {
							currentState = 146;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				Expect(162, t); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 147: {
				if (t == null) { currentState = 147; break; }
				if (t.kind == 5) {
					goto case 16;
				} else {
					goto case 142;
				}
			}
			case 148: {
				if (t == null) { currentState = 148; break; }
				Expect(123, t); // "For"
				currentState = 149;
				break;
			}
			case 149: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 150;
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				if (set[13, t.kind]) {
					goto case 164;
				} else {
					if (t.kind == 109) {
						goto case 151;
					} else {
						goto case 6;
					}
				}
			}
			case 151: {
				if (t == null) { currentState = 151; break; }
				Expect(109, t); // "Each"
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(153);
				goto case 161;
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				Expect(137, t); // "In"
				currentState = 154;
				break;
			}
			case 154: {
				stateStack.Push(155);
				goto case 37;
			}
			case 155: {
				stateStack.Push(156);
				goto case 31;
			}
			case 156: {
				if (t == null) { currentState = 156; break; }
				Expect(162, t); // "Next"
				currentState = 157;
				break;
			}
			case 157: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 158;
			}
			case 158: {
				if (t == null) { currentState = 158; break; }
				if (set[15, t.kind]) {
					goto case 159;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 159: {
				stateStack.Push(160);
				goto case 37;
			}
			case 160: {
				if (t == null) { currentState = 160; break; }
				if (t.kind == 23) {
					currentState = 159;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				stateStack.Push(162);
				goto case 104;
			}
			case 162: {
				if (t == null) { currentState = 162; break; }
				if (t.kind == 32) {
					currentState = 163;
					break;
				} else {
					goto case 163;
				}
			}
			case 163: {
				if (t == null) { currentState = 163; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(163);
					goto case 96;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 164: {
				stateStack.Push(165);
				goto case 161;
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				Expect(21, t); // "="
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(167);
				goto case 37;
			}
			case 167: {
				if (t == null) { currentState = 167; break; }
				if (t.kind == 203) {
					currentState = 174;
					break;
				} else {
					goto case 168;
				}
			}
			case 168: {
				stateStack.Push(169);
				goto case 31;
			}
			case 169: {
				if (t == null) { currentState = 169; break; }
				Expect(162, t); // "Next"
				currentState = 170;
				break;
			}
			case 170: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 171;
			}
			case 171: {
				if (t == null) { currentState = 171; break; }
				if (set[15, t.kind]) {
					goto case 172;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 172: {
				stateStack.Push(173);
				goto case 37;
			}
			case 173: {
				if (t == null) { currentState = 173; break; }
				if (t.kind == 23) {
					currentState = 172;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 174: {
				stateStack.Push(168);
				goto case 37;
			}
			case 175: {
				if (t == null) { currentState = 175; break; }
				Expect(107, t); // "Do"
				currentState = 176;
				break;
			}
			case 176: {
				if (t == null) { currentState = 176; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 180;
				} else {
					if (t.kind == 1 || t.kind == 22) {
						goto case 177;
					} else {
						goto case 6;
					}
				}
			}
			case 177: {
				stateStack.Push(178);
				goto case 31;
			}
			case 178: {
				if (t == null) { currentState = 178; break; }
				Expect(151, t); // "Loop"
				currentState = 179;
				break;
			}
			case 179: {
				if (t == null) { currentState = 179; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 125;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 180: {
				if (t == null) { currentState = 180; break; }
				if (t.kind == 222 || t.kind == 229) {
					currentState = 181;
					break;
				} else {
					Error(t);
					goto case 181;
				}
			}
			case 181: {
				stateStack.Push(182);
				goto case 37;
			}
			case 182: {
				stateStack.Push(183);
				goto case 31;
			}
			case 183: {
				if (t == null) { currentState = 183; break; }
				Expect(151, t); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 184: {
				if (t == null) { currentState = 184; break; }
				Expect(229, t); // "While"
				currentState = 185;
				break;
			}
			case 185: {
				stateStack.Push(186);
				goto case 37;
			}
			case 186: {
				stateStack.Push(187);
				goto case 31;
			}
			case 187: {
				if (t == null) { currentState = 187; break; }
				Expect(112, t); // "End"
				currentState = 188;
				break;
			}
			case 188: {
				if (t == null) { currentState = 188; break; }
				Expect(229, t); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 189: {
				if (t == null) { currentState = 189; break; }
				Expect(195, t); // "Select"
				currentState = 190;
				break;
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				if (t.kind == 73) {
					currentState = 191;
					break;
				} else {
					goto case 191;
				}
			}
			case 191: {
				stateStack.Push(192);
				goto case 37;
			}
			case 192: {
				stateStack.Push(193);
				goto case 15;
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				if (t.kind == 73) {
					currentState = 195;
					break;
				} else {
					Expect(112, t); // "End"
					currentState = 194;
					break;
				}
			}
			case 194: {
				if (t == null) { currentState = 194; break; }
				Expect(195, t); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 195: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 196;
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				if (t.kind == 110) {
					currentState = 197;
					break;
				} else {
					if (set[27, t.kind]) {
						goto case 198;
					} else {
						Error(t);
						goto case 197;
					}
				}
			}
			case 197: {
				stateStack.Push(193);
				goto case 31;
			}
			case 198: {
				if (t == null) { currentState = 198; break; }
				if (set[28, t.kind]) {
					if (t.kind == 143) {
						currentState = 201;
						break;
					} else {
						goto case 201;
					}
				} else {
					if (set[15, t.kind]) {
						stateStack.Push(199);
						goto case 37;
					} else {
						Error(t);
						goto case 199;
					}
				}
			}
			case 199: {
				if (t == null) { currentState = 199; break; }
				if (t.kind == 23) {
					currentState = 200;
					break;
				} else {
					goto case 197;
				}
			}
			case 200: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 198;
			}
			case 201: {
				stateStack.Push(202);
				goto case 203;
			}
			case 202: {
				stateStack.Push(199);
				goto case 50;
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (set[29, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 204: {
				if (t == null) { currentState = 204; break; }
				Expect(134, t); // "If"
				currentState = 205;
				break;
			}
			case 205: {
				stateStack.Push(206);
				goto case 37;
			}
			case 206: {
				if (t == null) { currentState = 206; break; }
				if (t.kind == 212) {
					currentState = 215;
					break;
				} else {
					goto case 207;
				}
			}
			case 207: {
				if (t == null) { currentState = 207; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 208;
				} else {
					goto case 6;
				}
			}
			case 208: {
				stateStack.Push(209);
				goto case 31;
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				if (t.kind == 110 || t.kind == 111) {
					if (t.kind == 110) {
						currentState = 214;
						break;
					} else {
						if (t.kind == 111) {
							goto case 211;
						} else {
							Error(t);
							goto case 208;
						}
					}
				} else {
					Expect(112, t); // "End"
					currentState = 210;
					break;
				}
			}
			case 210: {
				if (t == null) { currentState = 210; break; }
				Expect(134, t); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 211: {
				if (t == null) { currentState = 211; break; }
				currentState = 212;
				break;
			}
			case 212: {
				stateStack.Push(213);
				goto case 37;
			}
			case 213: {
				if (t == null) { currentState = 213; break; }
				if (t.kind == 212) {
					currentState = 208;
					break;
				} else {
					goto case 208;
				}
			}
			case 214: {
				if (t == null) { currentState = 214; break; }
				if (t.kind == 134) {
					goto case 211;
				} else {
					goto case 208;
				}
			}
			case 215: {
				if (t == null) { currentState = 215; break; }
				if (set[8, t.kind]) {
					goto case 216;
				} else {
					goto case 207;
				}
			}
			case 216: {
				stateStack.Push(217);
				goto case 34;
			}
			case 217: {
				if (t == null) { currentState = 217; break; }
				if (t.kind == 22) {
					currentState = 222;
					break;
				} else {
					if (t.kind == 110) {
						goto case 219;
					} else {
						goto case 218;
					}
				}
			}
			case 218: {
				if (t == null) { currentState = 218; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 219: {
				if (t == null) { currentState = 219; break; }
				currentState = 220;
				break;
			}
			case 220: {
				if (t == null) { currentState = 220; break; }
				if (set[8, t.kind]) {
					stateStack.Push(221);
					goto case 34;
				} else {
					goto case 221;
				}
			}
			case 221: {
				if (t == null) { currentState = 221; break; }
				if (t.kind == 22) {
					goto case 219;
				} else {
					goto case 218;
				}
			}
			case 222: {
				if (t == null) { currentState = 222; break; }
				if (set[8, t.kind]) {
					goto case 216;
				} else {
					goto case 217;
				}
			}
			case 223: {
				if (t == null) { currentState = 223; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 224;
				break;
			}
			case 224: {
				stateStack.Push(225);
				goto case 16;
			}
			case 225: {
				if (t == null) { currentState = 225; break; }
				if (t.kind == 36) {
					currentState = 76;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				if (t == null) { currentState = 226; break; }
				if (t.kind == 55 || t.kind == 191) {
					currentState = 227;
					break;
				} else {
					Error(t);
					goto case 227;
				}
			}
			case 227: {
				stateStack.Push(228);
				goto case 37;
			}
			case 228: {
				if (t == null) { currentState = 228; break; }
				Expect(23, t); // ","
				currentState = 36;
				break;
			}
			case 229: {
				if (t == null) { currentState = 229; break; }
				if (t.kind == 209 || t.kind == 231) {
					currentState = 230;
					break;
				} else {
					Error(t);
					goto case 230;
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 37;
			}
			case 231: {
				stateStack.Push(232);
				goto case 31;
			}
			case 232: {
				if (t == null) { currentState = 232; break; }
				Expect(112, t); // "End"
				currentState = 233;
				break;
			}
			case 233: {
				if (t == null) { currentState = 233; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 234: {
				if (t == null) { currentState = 234; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 235;
					break;
				} else {
					Error(t);
					goto case 235;
				}
			}
			case 235: {
				stateStack.Push(236);
				goto case 89;
			}
			case 236: {
				if (t == null) { currentState = 236; break; }
				if (t.kind == 32) {
					currentState = 237;
					break;
				} else {
					goto case 237;
				}
			}
			case 237: {
				if (t == null) { currentState = 237; break; }
				if (t.kind == 36) {
					goto case 249;
				} else {
					goto case 238;
				}
			}
			case 238: {
				if (t == null) { currentState = 238; break; }
				if (t.kind == 23) {
					currentState = 243;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 240;
						break;
					} else {
						goto case 239;
					}
				}
			}
			case 239: {
				if (t == null) { currentState = 239; break; }
				if (t.kind == 21) {
					goto case 125;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 240: {
				if (t == null) { currentState = 240; break; }
				if (t.kind == 161) {
					goto case 242;
				} else {
					goto case 241;
				}
			}
			case 241: {
				stateStack.Push(239);
				goto case 69;
			}
			case 242: {
				if (t == null) { currentState = 242; break; }
				currentState = 241;
				break;
			}
			case 243: {
				stateStack.Push(244);
				goto case 89;
			}
			case 244: {
				if (t == null) { currentState = 244; break; }
				if (t.kind == 32) {
					currentState = 245;
					break;
				} else {
					goto case 245;
				}
			}
			case 245: {
				if (t == null) { currentState = 245; break; }
				if (t.kind == 36) {
					goto case 246;
				} else {
					goto case 238;
				}
			}
			case 246: {
				if (t == null) { currentState = 246; break; }
				currentState = 247;
				break;
			}
			case 247: {
				if (t == null) { currentState = 247; break; }
				if (t.kind == 23) {
					goto case 246;
				} else {
					goto case 248;
				}
			}
			case 248: {
				if (t == null) { currentState = 248; break; }
				Expect(37, t); // ")"
				currentState = 238;
				break;
			}
			case 249: {
				if (t == null) { currentState = 249; break; }
				currentState = 250;
				break;
			}
			case 250: {
				if (t == null) { currentState = 250; break; }
				if (t.kind == 23) {
					goto case 249;
				} else {
					goto case 248;
				}
			}
			case 251: {
				if (t == null) { currentState = 251; break; }
				if (t.kind == 39) {
					stateStack.Push(251);
					goto case 252;
				} else {
					stateStack.Push(27);
					goto case 69;
				}
			}
			case 252: {
				if (t == null) { currentState = 252; break; }
				Expect(39, t); // "<"
				currentState = 253;
				break;
			}
			case 253: {
				PushContext(Context.Attribute, t);
				goto case 254;
			}
			case 254: {
				if (t == null) { currentState = 254; break; }
				if (set[30, t.kind]) {
					currentState = 254;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 255;
					break;
				}
			}
			case 255: {
				PopContext();
				goto case 256;
			}
			case 256: {
				if (t == null) { currentState = 256; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 257: {
				stateStack.Push(258);
				goto case 259;
			}
			case 258: {
				if (t == null) { currentState = 258; break; }
				if (t.kind == 23) {
					currentState = 257;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 259: {
				if (t == null) { currentState = 259; break; }
				if (t.kind == 39) {
					stateStack.Push(259);
					goto case 252;
				} else {
					goto case 260;
				}
			}
			case 260: {
				if (t == null) { currentState = 260; break; }
				if (set[31, t.kind]) {
					currentState = 260;
					break;
				} else {
					stateStack.Push(261);
					goto case 89;
				}
			}
			case 261: {
				if (t == null) { currentState = 261; break; }
				if (t.kind == 62) {
					goto case 242;
				} else {
					goto case 239;
				}
			}
			case 262: {
				if (t == null) { currentState = 262; break; }
				Expect(97, t); // "Custom"
				currentState = 263;
				break;
			}
			case 263: {
				stateStack.Push(264);
				goto case 274;
			}
			case 264: {
				if (t == null) { currentState = 264; break; }
				if (set[32, t.kind]) {
					goto case 266;
				} else {
					Expect(112, t); // "End"
					currentState = 265;
					break;
				}
			}
			case 265: {
				if (t == null) { currentState = 265; break; }
				Expect(118, t); // "Event"
				currentState = 30;
				break;
			}
			case 266: {
				if (t == null) { currentState = 266; break; }
				if (t.kind == 39) {
					stateStack.Push(266);
					goto case 252;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 267;
						break;
					} else {
						Error(t);
						goto case 267;
					}
				}
			}
			case 267: {
				if (t == null) { currentState = 267; break; }
				Expect(36, t); // "("
				currentState = 268;
				break;
			}
			case 268: {
				stateStack.Push(269);
				goto case 257;
			}
			case 269: {
				if (t == null) { currentState = 269; break; }
				Expect(37, t); // ")"
				currentState = 270;
				break;
			}
			case 270: {
				stateStack.Push(271);
				goto case 31;
			}
			case 271: {
				if (t == null) { currentState = 271; break; }
				Expect(112, t); // "End"
				currentState = 272;
				break;
			}
			case 272: {
				if (t == null) { currentState = 272; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 273;
					break;
				} else {
					Error(t);
					goto case 273;
				}
			}
			case 273: {
				stateStack.Push(264);
				goto case 15;
			}
			case 274: {
				if (t == null) { currentState = 274; break; }
				Expect(118, t); // "Event"
				currentState = 275;
				break;
			}
			case 275: {
				stateStack.Push(276);
				goto case 89;
			}
			case 276: {
				if (t == null) { currentState = 276; break; }
				if (t.kind == 62) {
					currentState = 283;
					break;
				} else {
					if (set[33, t.kind]) {
						if (t.kind == 36) {
							currentState = 281;
							break;
						} else {
							goto case 277;
						}
					} else {
						Error(t);
						goto case 277;
					}
				}
			}
			case 277: {
				if (t == null) { currentState = 277; break; }
				if (t.kind == 135) {
					goto case 278;
				} else {
					goto case 30;
				}
			}
			case 278: {
				if (t == null) { currentState = 278; break; }
				currentState = 279;
				break;
			}
			case 279: {
				stateStack.Push(280);
				goto case 69;
			}
			case 280: {
				if (t == null) { currentState = 280; break; }
				if (t.kind == 23) {
					goto case 278;
				} else {
					goto case 30;
				}
			}
			case 281: {
				if (t == null) { currentState = 281; break; }
				if (set[34, t.kind]) {
					stateStack.Push(282);
					goto case 257;
				} else {
					goto case 282;
				}
			}
			case 282: {
				if (t == null) { currentState = 282; break; }
				Expect(37, t); // ")"
				currentState = 277;
				break;
			}
			case 283: {
				stateStack.Push(277);
				goto case 69;
			}
			case 284: {
				if (t == null) { currentState = 284; break; }
				Expect(100, t); // "Declare"
				currentState = 285;
				break;
			}
			case 285: {
				if (t == null) { currentState = 285; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 286;
					break;
				} else {
					goto case 286;
				}
			}
			case 286: {
				if (t == null) { currentState = 286; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 287;
					break;
				} else {
					Error(t);
					goto case 287;
				}
			}
			case 287: {
				stateStack.Push(288);
				goto case 89;
			}
			case 288: {
				if (t == null) { currentState = 288; break; }
				Expect(148, t); // "Lib"
				currentState = 289;
				break;
			}
			case 289: {
				if (t == null) { currentState = 289; break; }
				Expect(3, t); // LiteralString
				currentState = 290;
				break;
			}
			case 290: {
				if (t == null) { currentState = 290; break; }
				if (t.kind == 58) {
					currentState = 294;
					break;
				} else {
					goto case 291;
				}
			}
			case 291: {
				if (t == null) { currentState = 291; break; }
				if (t.kind == 36) {
					currentState = 292;
					break;
				} else {
					goto case 30;
				}
			}
			case 292: {
				if (t == null) { currentState = 292; break; }
				if (set[34, t.kind]) {
					stateStack.Push(293);
					goto case 257;
				} else {
					goto case 293;
				}
			}
			case 293: {
				if (t == null) { currentState = 293; break; }
				Expect(37, t); // ")"
				currentState = 30;
				break;
			}
			case 294: {
				if (t == null) { currentState = 294; break; }
				Expect(3, t); // LiteralString
				currentState = 291;
				break;
			}
			case 295: {
				if (t == null) { currentState = 295; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 296;
					break;
				} else {
					Error(t);
					goto case 296;
				}
			}
			case 296: {
				PushContext(Context.IdentifierExpected, t);
				goto case 297;
			}
			case 297: {
				if (t == null) { currentState = 297; break; }
				currentState = 298;
				break;
			}
			case 298: {
				PopContext();
				goto case 299;
			}
			case 299: {
				if (t == null) { currentState = 299; break; }
				if (t.kind == 36) {
					currentState = 305;
					break;
				} else {
					goto case 300;
				}
			}
			case 300: {
				if (t == null) { currentState = 300; break; }
				if (t.kind == 62) {
					currentState = 304;
					break;
				} else {
					goto case 301;
				}
			}
			case 301: {
				stateStack.Push(302);
				goto case 31;
			}
			case 302: {
				if (t == null) { currentState = 302; break; }
				Expect(112, t); // "End"
				currentState = 303;
				break;
			}
			case 303: {
				if (t == null) { currentState = 303; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 30;
					break;
				} else {
					Error(t);
					goto case 30;
				}
			}
			case 304: {
				stateStack.Push(301);
				goto case 69;
			}
			case 305: {
				if (t == null) { currentState = 305; break; }
				if (set[34, t.kind]) {
					stateStack.Push(306);
					goto case 257;
				} else {
					goto case 306;
				}
			}
			case 306: {
				if (t == null) { currentState = 306; break; }
				Expect(37, t); // ")"
				currentState = 300;
				break;
			}
			case 307: {
				if (t == null) { currentState = 307; break; }
				if (t.kind == 87) {
					currentState = 308;
					break;
				} else {
					goto case 308;
				}
			}
			case 308: {
				stateStack.Push(309);
				goto case 313;
			}
			case 309: {
				if (t == null) { currentState = 309; break; }
				if (t.kind == 62) {
					currentState = 312;
					break;
				} else {
					goto case 310;
				}
			}
			case 310: {
				if (t == null) { currentState = 310; break; }
				if (t.kind == 21) {
					currentState = 311;
					break;
				} else {
					goto case 30;
				}
			}
			case 311: {
				stateStack.Push(30);
				goto case 37;
			}
			case 312: {
				stateStack.Push(310);
				goto case 69;
			}
			case 313: {
				if (t == null) { currentState = 313; break; }
				if (set[35, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 314: {
				if (t == null) { currentState = 314; break; }
				currentState = 9;
				break;
			}
			case 315: {
				if (t == null) { currentState = 315; break; }
				Expect(159, t); // "Namespace"
				currentState = 316;
				break;
			}
			case 316: {
				if (t == null) { currentState = 316; break; }
				if (set[3, t.kind]) {
					currentState = 316;
					break;
				} else {
					stateStack.Push(317);
					goto case 15;
				}
			}
			case 317: {
				if (t == null) { currentState = 317; break; }
				if (set[36, t.kind]) {
					stateStack.Push(317);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 318;
					break;
				}
			}
			case 318: {
				if (t == null) { currentState = 318; break; }
				Expect(159, t); // "Namespace"
				currentState = 30;
				break;
			}
			case 319: {
				if (t == null) { currentState = 319; break; }
				Expect(136, t); // "Imports"
				currentState = 320;
				break;
			}
			case 320: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 321;
			}
			case 321: {
				if (t == null) { currentState = 321; break; }
				if (set[3, t.kind]) {
					currentState = 321;
					break;
				} else {
					goto case 30;
				}
			}
			case 322: {
				if (t == null) { currentState = 322; break; }
				Expect(172, t); // "Option"
				currentState = 323;
				break;
			}
			case 323: {
				if (t == null) { currentState = 323; break; }
				if (set[3, t.kind]) {
					currentState = 323;
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
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, x,T,x,x, T,T,x,T, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, x,T,x,x, T,T,x,T, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, T,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
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