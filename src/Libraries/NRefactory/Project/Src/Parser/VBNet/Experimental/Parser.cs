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
	
	Token consumedEndToken;
	
	public void InformToken(Token t) 
	{
		if (consumedEndToken != null) {
			if (t.kind == Tokens.EOL || t.kind == Tokens.Colon) {
				consumedEndToken = null;
				return; // ignore End statement
			}
			InformTokenInternal(consumedEndToken);
			consumedEndToken = null;
		}
		if (t.kind == Tokens.End) {
			consumedEndToken = t;
		} else {
			InformTokenInternal(t);
		}
	}

	void InformTokenInternal(Token t) 
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
					goto case 335;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 332;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 84;
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
					goto case 328;
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
					goto case 84;
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
						goto case 327;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 327;
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
					PushContext(Context.Member, t);
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
				if (t == null) { currentState = 17; break; }
				if (t.kind == 39) {
					stateStack.Push(17);
					goto case 84;
				} else {
					goto case 18;
				}
			}
			case 18: {
				if (t == null) { currentState = 18; break; }
				if (set[5, t.kind]) {
					currentState = 18;
					break;
				} else {
					if (set[6, t.kind]) {
						stateStack.Push(19);
						goto case 320;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(19);
							goto case 308;
						} else {
							if (t.kind == 100) {
								stateStack.Push(19);
								goto case 297;
							} else {
								if (t.kind == 118) {
									stateStack.Push(19);
									goto case 287;
								} else {
									if (t.kind == 97) {
										stateStack.Push(19);
										goto case 275;
									} else {
										if (t.kind == 171) {
											stateStack.Push(19);
											goto case 20;
										} else {
											Error(t);
											goto case 19;
										}
									}
								}
							}
						}
					}
				}
			}
			case 19: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 20: {
				if (t == null) { currentState = 20; break; }
				Expect(171, t); // "Operator"
				currentState = 21;
				break;
			}
			case 21: {
				if (t == null) { currentState = 21; break; }
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) { currentState = 22; break; }
				Expect(36, t); // "("
				currentState = 23;
				break;
			}
			case 23: {
				stateStack.Push(24);
				goto case 75;
			}
			case 24: {
				if (t == null) { currentState = 24; break; }
				Expect(37, t); // ")"
				currentState = 25;
				break;
			}
			case 25: {
				if (t == null) { currentState = 25; break; }
				if (t.kind == 62) {
					currentState = 274;
					break;
				} else {
					goto case 26;
				}
			}
			case 26: {
				stateStack.Push(27);
				goto case 29;
			}
			case 27: {
				if (t == null) { currentState = 27; break; }
				Expect(112, t); // "End"
				currentState = 28;
				break;
			}
			case 28: {
				if (t == null) { currentState = 28; break; }
				Expect(171, t); // "Operator"
				currentState = 15;
				break;
			}
			case 29: {
				PushContext(Context.Body, t);
				goto case 30;
			}
			case 30: {
				stateStack.Push(31);
				goto case 15;
			}
			case 31: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 32;
			}
			case 32: {
				if (t == null) { currentState = 32; break; }
				if (set[7, t.kind]) {
					if (set[8, t.kind]) {
						stateStack.Push(30);
						goto case 33;
					} else {
						goto case 30;
					}
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 33: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 34;
			}
			case 34: {
				if (t == null) { currentState = 34; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 261;
					break;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						currentState = 257;
						break;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							currentState = 255;
							break;
						} else {
							if (t.kind == 187) {
								currentState = 253;
								break;
							} else {
								if (t.kind == 134) {
									currentState = 232;
									break;
								} else {
									if (t.kind == 195) {
										currentState = 217;
										break;
									} else {
										if (t.kind == 229) {
											currentState = 213;
											break;
										} else {
											if (t.kind == 107) {
												currentState = 207;
												break;
											} else {
												if (t.kind == 123) {
													currentState = 183;
													break;
												} else {
													if (t.kind == 117 || t.kind == 170 || t.kind == 192) {
														if (t.kind == 117 || t.kind == 170) {
															if (t.kind == 170) {
																currentState = 178;
																break;
															} else {
																goto case 178;
															}
														} else {
															if (t.kind == 192) {
																currentState = 176;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (t.kind == 213) {
															goto case 160;
														} else {
															if (t.kind == 216) {
																currentState = 166;
																break;
															} else {
																if (set[9, t.kind]) {
																	if (t.kind == 131) {
																		currentState = 165;
																		break;
																	} else {
																		if (t.kind == 119) {
																			currentState = 164;
																			break;
																		} else {
																			if (t.kind == 88) {
																				currentState = 163;
																				break;
																			} else {
																				if (t.kind == 204) {
																					goto case 16;
																				} else {
																					if (t.kind == 193) {
																						goto case 160;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (t.kind == 189) {
																		currentState = 158;
																		break;
																	} else {
																		if (t.kind == 116) {
																			goto case 155;
																		} else {
																			if (t.kind == 224) {
																				currentState = 151;
																				break;
																			} else {
																				if (set[10, t.kind]) {
																					if (t.kind == 72) {
																						goto case 81;
																					} else {
																						goto case 35;
																					}
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
						}
					}
				}
			}
			case 35: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 36;
			}
			case 36: {
				if (t == null) { currentState = 36; break; }
				if (set[11, t.kind]) {
					goto case 104;
				} else {
					if (t.kind == 134) {
						currentState = 97;
						break;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							if (t.kind == 208) {
								currentState = 89;
								break;
							} else {
								if (t.kind == 126) {
									currentState = 37;
									break;
								} else {
									goto case 6;
								}
							}
						} else {
							goto case 6;
						}
					}
				}
			}
			case 37: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 38;
			}
			case 38: {
				if (t == null) { currentState = 38; break; }
				if (t.kind == 36) {
					currentState = 73;
					break;
				} else {
					goto case 39;
				}
			}
			case 39: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 40;
			}
			case 40: {
				if (t == null) { currentState = 40; break; }
				if (set[12, t.kind]) {
					goto case 35;
				} else {
					if (t.kind == 1 || t.kind == 22 || t.kind == 62) {
						if (t.kind == 62) {
							currentState = 44;
							break;
						} else {
							goto case 41;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 41: {
				stateStack.Push(42);
				goto case 29;
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				Expect(112, t); // "End"
				currentState = 43;
				break;
			}
			case 43: {
				if (t == null) { currentState = 43; break; }
				Expect(126, t); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 44: {
				stateStack.Push(41);
				goto case 45;
			}
			case 45: {
				if (t == null) { currentState = 45; break; }
				if (t.kind == 129) {
					goto case 69;
				} else {
					if (set[13, t.kind]) {
						stateStack.Push(46);
						goto case 70;
					} else {
						if (set[14, t.kind]) {
							goto case 69;
						} else {
							Error(t);
							goto case 46;
						}
					}
				}
			}
			case 46: {
				if (t == null) { currentState = 46; break; }
				if (t.kind == 36) {
					stateStack.Push(46);
					goto case 50;
				} else {
					goto case 47;
				}
			}
			case 47: {
				if (t == null) { currentState = 47; break; }
				if (t.kind == 27) {
					currentState = 48;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 48: {
				stateStack.Push(49);
				goto case 16;
			}
			case 49: {
				if (t == null) { currentState = 49; break; }
				if (t.kind == 36) {
					stateStack.Push(49);
					goto case 50;
				} else {
					goto case 47;
				}
			}
			case 50: {
				if (t == null) { currentState = 50; break; }
				Expect(36, t); // "("
				currentState = 51;
				break;
			}
			case 51: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 52;
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				if (t.kind == 168) {
					goto case 66;
				} else {
					if (set[15, t.kind]) {
						goto case 54;
					} else {
						Error(t);
						goto case 53;
					}
				}
			}
			case 53: {
				if (t == null) { currentState = 53; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 54: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 55;
			}
			case 55: {
				if (t == null) { currentState = 55; break; }
				if (set[16, t.kind]) {
					goto case 56;
				} else {
					goto case 53;
				}
			}
			case 56: {
				stateStack.Push(53);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 57;
			}
			case 57: {
				if (t == null) { currentState = 57; break; }
				if (set[12, t.kind]) {
					goto case 62;
				} else {
					if (t.kind == 23) {
						goto case 58;
					} else {
						goto case 6;
					}
				}
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				currentState = 59;
				break;
			}
			case 59: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 60;
			}
			case 60: {
				if (t == null) { currentState = 60; break; }
				if (set[12, t.kind]) {
					stateStack.Push(61);
					goto case 35;
				} else {
					goto case 61;
				}
			}
			case 61: {
				if (t == null) { currentState = 61; break; }
				if (t.kind == 23) {
					goto case 58;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 62: {
				stateStack.Push(63);
				goto case 35;
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				if (t.kind == 23) {
					currentState = 64;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 64: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 65;
			}
			case 65: {
				if (t == null) { currentState = 65; break; }
				if (set[12, t.kind]) {
					goto case 62;
				} else {
					goto case 63;
				}
			}
			case 66: {
				if (t == null) { currentState = 66; break; }
				currentState = 67;
				break;
			}
			case 67: {
				if (t == null) { currentState = 67; break; }
				if (set[17, t.kind]) {
					stateStack.Push(68);
					goto case 45;
				} else {
					goto case 68;
				}
			}
			case 68: {
				if (t == null) { currentState = 68; break; }
				if (t.kind == 23) {
					goto case 66;
				} else {
					goto case 53;
				}
			}
			case 69: {
				if (t == null) { currentState = 69; break; }
				currentState = 46;
				break;
			}
			case 70: {
				PushContext(Context.IdentifierExpected, t);
				goto case 71;
			}
			case 71: {
				if (t == null) { currentState = 71; break; }
				if (set[13, t.kind]) {
					currentState = 72;
					break;
				} else {
					Error(t);
					goto case 72;
				}
			}
			case 72: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 73: {
				if (t == null) { currentState = 73; break; }
				if (set[18, t.kind]) {
					stateStack.Push(74);
					goto case 75;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				Expect(37, t); // ")"
				currentState = 39;
				break;
			}
			case 75: {
				stateStack.Push(76);
				goto case 77;
			}
			case 76: {
				if (t == null) { currentState = 76; break; }
				if (t.kind == 23) {
					currentState = 75;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 77: {
				if (t == null) { currentState = 77; break; }
				if (t.kind == 39) {
					stateStack.Push(77);
					goto case 84;
				} else {
					goto case 78;
				}
			}
			case 78: {
				if (t == null) { currentState = 78; break; }
				if (set[19, t.kind]) {
					currentState = 78;
					break;
				} else {
					stateStack.Push(79);
					goto case 70;
				}
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				if (t.kind == 62) {
					goto case 82;
				} else {
					goto case 80;
				}
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				if (t.kind == 21) {
					goto case 81;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 81: {
				if (t == null) { currentState = 81; break; }
				currentState = 35;
				break;
			}
			case 82: {
				if (t == null) { currentState = 82; break; }
				currentState = 83;
				break;
			}
			case 83: {
				stateStack.Push(80);
				goto case 45;
			}
			case 84: {
				if (t == null) { currentState = 84; break; }
				Expect(39, t); // "<"
				currentState = 85;
				break;
			}
			case 85: {
				PushContext(Context.Attribute, t);
				goto case 86;
			}
			case 86: {
				if (t == null) { currentState = 86; break; }
				if (set[20, t.kind]) {
					currentState = 86;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 87;
					break;
				}
			}
			case 87: {
				PopContext();
				goto case 88;
			}
			case 88: {
				if (t == null) { currentState = 88; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 89: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 90;
			}
			case 90: {
				if (t == null) { currentState = 90; break; }
				if (t.kind == 36) {
					currentState = 95;
					break;
				} else {
					goto case 91;
				}
			}
			case 91: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 92;
			}
			case 92: {
				if (t == null) { currentState = 92; break; }
				if (set[8, t.kind]) {
					goto case 33;
				} else {
					if (t.kind == 1 || t.kind == 22) {
						stateStack.Push(93);
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				Expect(112, t); // "End"
				currentState = 94;
				break;
			}
			case 94: {
				if (t == null) { currentState = 94; break; }
				Expect(208, t); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (set[18, t.kind]) {
					stateStack.Push(96);
					goto case 75;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				Expect(37, t); // ")"
				currentState = 91;
				break;
			}
			case 97: {
				if (t == null) { currentState = 97; break; }
				Expect(36, t); // "("
				currentState = 98;
				break;
			}
			case 98: {
				stateStack.Push(99);
				goto case 35;
			}
			case 99: {
				if (t == null) { currentState = 99; break; }
				Expect(23, t); // ","
				currentState = 100;
				break;
			}
			case 100: {
				stateStack.Push(101);
				goto case 35;
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				if (t.kind == 23) {
					goto case 102;
				} else {
					goto case 53;
				}
			}
			case 102: {
				if (t == null) { currentState = 102; break; }
				currentState = 103;
				break;
			}
			case 103: {
				stateStack.Push(53);
				goto case 35;
			}
			case 104: {
				stateStack.Push(105);
				goto case 106;
			}
			case 105: {
				if (t == null) { currentState = 105; break; }
				if (set[21, t.kind]) {
					currentState = 104;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 106: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 107;
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				if (set[22, t.kind]) {
					currentState = 106;
					break;
				} else {
					if (set[23, t.kind]) {
						stateStack.Push(126);
						goto case 134;
					} else {
						if (t.kind == 218) {
							currentState = 124;
							break;
						} else {
							if (t.kind == 161) {
								currentState = 108;
								break;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 108: {
				if (t == null) { currentState = 108; break; }
				if (set[17, t.kind]) {
					stateStack.Push(118);
					goto case 45;
				} else {
					goto case 109;
				}
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				if (t.kind == 231) {
					currentState = 110;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 110: {
				if (t == null) { currentState = 110; break; }
				Expect(34, t); // "{"
				currentState = 111;
				break;
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				if (t.kind == 146) {
					currentState = 112;
					break;
				} else {
					goto case 112;
				}
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				Expect(27, t); // "."
				currentState = 113;
				break;
			}
			case 113: {
				stateStack.Push(114);
				goto case 16;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				Expect(21, t); // "="
				currentState = 115;
				break;
			}
			case 115: {
				stateStack.Push(116);
				goto case 35;
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				if (t.kind == 23) {
					currentState = 111;
					break;
				} else {
					goto case 117;
				}
			}
			case 117: {
				if (t == null) { currentState = 117; break; }
				Expect(35, t); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 118: {
				if (t == null) { currentState = 118; break; }
				if (t.kind == 125) {
					currentState = 119;
					break;
				} else {
					goto case 109;
				}
			}
			case 119: {
				stateStack.Push(109);
				goto case 120;
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				Expect(34, t); // "{"
				currentState = 121;
				break;
			}
			case 121: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 122;
			}
			case 122: {
				if (t == null) { currentState = 122; break; }
				if (set[12, t.kind]) {
					stateStack.Push(123);
					goto case 35;
				} else {
					if (t.kind == 34) {
						stateStack.Push(123);
						goto case 120;
					} else {
						Error(t);
						goto case 123;
					}
				}
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				if (t.kind == 23) {
					currentState = 121;
					break;
				} else {
					goto case 117;
				}
			}
			case 124: {
				stateStack.Push(125);
				goto case 106;
			}
			case 125: {
				if (t == null) { currentState = 125; break; }
				Expect(143, t); // "Is"
				currentState = 45;
				break;
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(126);
					goto case 127;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 127: {
				if (t == null) { currentState = 127; break; }
				if (t.kind == 36) {
					currentState = 129;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 128;
					} else {
						goto case 6;
					}
				}
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				currentState = 16;
				break;
			}
			case 129: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 130;
			}
			case 130: {
				if (t == null) { currentState = 130; break; }
				if (t.kind == 168) {
					goto case 131;
				} else {
					if (set[16, t.kind]) {
						goto case 56;
					} else {
						goto case 6;
					}
				}
			}
			case 131: {
				if (t == null) { currentState = 131; break; }
				currentState = 132;
				break;
			}
			case 132: {
				stateStack.Push(133);
				goto case 45;
			}
			case 133: {
				if (t == null) { currentState = 133; break; }
				if (t.kind == 23) {
					goto case 131;
				} else {
					goto case 53;
				}
			}
			case 134: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 135;
			}
			case 135: {
				if (t == null) { currentState = 135; break; }
				if (set[24, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						goto case 102;
					} else {
						if (set[13, t.kind]) {
							goto case 70;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 128;
							} else {
								if (t.kind == 128) {
									currentState = 150;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 148;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 141;
										} else {
											if (set[25, t.kind]) {
												if (set[26, t.kind]) {
													currentState = 140;
													break;
												} else {
													if (t.kind == 93 || t.kind == 105 || t.kind == 217) {
														currentState = 136;
														break;
													} else {
														goto case 6;
													}
												}
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
			case 136: {
				if (t == null) { currentState = 136; break; }
				Expect(36, t); // "("
				currentState = 137;
				break;
			}
			case 137: {
				stateStack.Push(138);
				goto case 35;
			}
			case 138: {
				if (t == null) { currentState = 138; break; }
				Expect(23, t); // ","
				currentState = 139;
				break;
			}
			case 139: {
				stateStack.Push(53);
				goto case 45;
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				Expect(36, t); // "("
				currentState = 103;
				break;
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				if (t.kind == 17) {
					currentState = 141;
					break;
				} else {
					stateStack.Push(142);
					goto case 143;
				}
			}
			case 142: {
				if (t == null) { currentState = 142; break; }
				if (t.kind == 17) {
					currentState = 142;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 144;
				break;
			}
			case 144: {
				if (t == null) { currentState = 144; break; }
				if (set[27, t.kind]) {
					currentState = 144;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 145;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 145: {
				if (t == null) { currentState = 145; break; }
				currentState = 146;
				break;
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				if (set[28, t.kind]) {
					if (set[29, t.kind]) {
						goto case 145;
					} else {
						if (t.kind == 10) {
							stateStack.Push(146);
							goto case 143;
						} else {
							Error(t);
							goto case 146;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 147;
					break;
				}
			}
			case 147: {
				if (t == null) { currentState = 147; break; }
				if (set[30, t.kind]) {
					currentState = 147;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 148: {
				if (t == null) { currentState = 148; break; }
				Expect(36, t); // "("
				currentState = 149;
				break;
			}
			case 149: {
				readXmlIdentifier = true;
				stateStack.Push(53);
				goto case 70;
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				Expect(36, t); // "("
				currentState = 139;
				break;
			}
			case 151: {
				stateStack.Push(152);
				goto case 35;
			}
			case 152: {
				stateStack.Push(153);
				goto case 29;
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				Expect(112, t); // "End"
				currentState = 154;
				break;
			}
			case 154: {
				if (t == null) { currentState = 154; break; }
				Expect(224, t); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 155: {
				if (t == null) { currentState = 155; break; }
				currentState = 156;
				break;
			}
			case 156: {
				stateStack.Push(157);
				goto case 35;
			}
			case 157: {
				if (t == null) { currentState = 157; break; }
				if (t.kind == 23) {
					goto case 155;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 158: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 159;
			}
			case 159: {
				if (t == null) { currentState = 159; break; }
				if (t.kind == 182) {
					goto case 81;
				} else {
					goto case 35;
				}
			}
			case 160: {
				if (t == null) { currentState = 160; break; }
				currentState = 161;
				break;
			}
			case 161: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 162;
			}
			case 162: {
				if (t == null) { currentState = 162; break; }
				if (set[12, t.kind]) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 163: {
				if (t == null) { currentState = 163; break; }
				if (t.kind == 107 || t.kind == 123 || t.kind == 229) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 164: {
				if (t == null) { currentState = 164; break; }
				if (set[31, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				if (set[13, t.kind]) {
					goto case 70;
				} else {
					if (t.kind == 5) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 166: {
				stateStack.Push(167);
				goto case 29;
			}
			case 167: {
				if (t == null) { currentState = 167; break; }
				if (t.kind == 74) {
					currentState = 171;
					break;
				} else {
					if (t.kind == 122) {
						currentState = 170;
						break;
					} else {
						goto case 168;
					}
				}
			}
			case 168: {
				if (t == null) { currentState = 168; break; }
				Expect(112, t); // "End"
				currentState = 169;
				break;
			}
			case 169: {
				if (t == null) { currentState = 169; break; }
				Expect(216, t); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 170: {
				stateStack.Push(168);
				goto case 29;
			}
			case 171: {
				if (t == null) { currentState = 171; break; }
				if (set[13, t.kind]) {
					stateStack.Push(174);
					goto case 70;
				} else {
					goto case 172;
				}
			}
			case 172: {
				if (t == null) { currentState = 172; break; }
				if (t.kind == 227) {
					currentState = 173;
					break;
				} else {
					goto case 166;
				}
			}
			case 173: {
				stateStack.Push(166);
				goto case 35;
			}
			case 174: {
				if (t == null) { currentState = 174; break; }
				if (t.kind == 62) {
					currentState = 175;
					break;
				} else {
					goto case 172;
				}
			}
			case 175: {
				stateStack.Push(172);
				goto case 45;
			}
			case 176: {
				if (t == null) { currentState = 176; break; }
				if (t.kind == 5 || t.kind == 162) {
					goto case 16;
				} else {
					goto case 177;
				}
			}
			case 177: {
				if (t == null) { currentState = 177; break; }
				if (set[13, t.kind]) {
					goto case 70;
				} else {
					goto case 6;
				}
			}
			case 178: {
				if (t == null) { currentState = 178; break; }
				Expect(117, t); // "Error"
				currentState = 179;
				break;
			}
			case 179: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 180;
			}
			case 180: {
				if (t == null) { currentState = 180; break; }
				if (set[12, t.kind]) {
					goto case 35;
				} else {
					if (t.kind == 131) {
						currentState = 182;
						break;
					} else {
						if (t.kind == 192) {
							currentState = 181;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 181: {
				if (t == null) { currentState = 181; break; }
				Expect(162, t); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 182: {
				if (t == null) { currentState = 182; break; }
				if (t.kind == 5) {
					goto case 16;
				} else {
					goto case 177;
				}
			}
			case 183: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 184;
			}
			case 184: {
				if (t == null) { currentState = 184; break; }
				if (set[23, t.kind]) {
					stateStack.Push(197);
					goto case 194;
				} else {
					if (t.kind == 109) {
						currentState = 185;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 185: {
				stateStack.Push(186);
				goto case 194;
			}
			case 186: {
				if (t == null) { currentState = 186; break; }
				Expect(137, t); // "In"
				currentState = 187;
				break;
			}
			case 187: {
				stateStack.Push(188);
				goto case 35;
			}
			case 188: {
				stateStack.Push(189);
				goto case 29;
			}
			case 189: {
				if (t == null) { currentState = 189; break; }
				Expect(162, t); // "Next"
				currentState = 190;
				break;
			}
			case 190: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 191;
			}
			case 191: {
				if (t == null) { currentState = 191; break; }
				if (set[12, t.kind]) {
					goto case 192;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 192: {
				stateStack.Push(193);
				goto case 35;
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				if (t.kind == 23) {
					currentState = 192;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 194: {
				stateStack.Push(195);
				goto case 134;
			}
			case 195: {
				if (t == null) { currentState = 195; break; }
				if (t.kind == 32) {
					currentState = 196;
					break;
				} else {
					goto case 196;
				}
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(196);
					goto case 127;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 197: {
				if (t == null) { currentState = 197; break; }
				Expect(21, t); // "="
				currentState = 198;
				break;
			}
			case 198: {
				stateStack.Push(199);
				goto case 35;
			}
			case 199: {
				if (t == null) { currentState = 199; break; }
				if (t.kind == 203) {
					currentState = 206;
					break;
				} else {
					goto case 200;
				}
			}
			case 200: {
				stateStack.Push(201);
				goto case 29;
			}
			case 201: {
				if (t == null) { currentState = 201; break; }
				Expect(162, t); // "Next"
				currentState = 202;
				break;
			}
			case 202: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 203;
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (set[12, t.kind]) {
					goto case 204;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 35;
			}
			case 205: {
				if (t == null) { currentState = 205; break; }
				if (t.kind == 23) {
					currentState = 204;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 206: {
				stateStack.Push(200);
				goto case 35;
			}
			case 207: {
				if (t == null) { currentState = 207; break; }
				if (t.kind == 222 || t.kind == 229) {
					currentState = 210;
					break;
				} else {
					if (t.kind == 1 || t.kind == 22) {
						stateStack.Push(208);
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 208: {
				if (t == null) { currentState = 208; break; }
				Expect(151, t); // "Loop"
				currentState = 209;
				break;
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 81;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 210: {
				stateStack.Push(211);
				goto case 35;
			}
			case 211: {
				stateStack.Push(212);
				goto case 29;
			}
			case 212: {
				if (t == null) { currentState = 212; break; }
				Expect(151, t); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 213: {
				stateStack.Push(214);
				goto case 35;
			}
			case 214: {
				stateStack.Push(215);
				goto case 29;
			}
			case 215: {
				if (t == null) { currentState = 215; break; }
				Expect(112, t); // "End"
				currentState = 216;
				break;
			}
			case 216: {
				if (t == null) { currentState = 216; break; }
				Expect(229, t); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 217: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 218;
			}
			case 218: {
				if (t == null) { currentState = 218; break; }
				if (t.kind == 73) {
					currentState = 219;
					break;
				} else {
					goto case 219;
				}
			}
			case 219: {
				stateStack.Push(220);
				goto case 35;
			}
			case 220: {
				stateStack.Push(221);
				goto case 15;
			}
			case 221: {
				if (t == null) { currentState = 221; break; }
				if (t.kind == 73) {
					currentState = 223;
					break;
				} else {
					Expect(112, t); // "End"
					currentState = 222;
					break;
				}
			}
			case 222: {
				if (t == null) { currentState = 222; break; }
				Expect(195, t); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 223: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 224;
			}
			case 224: {
				if (t == null) { currentState = 224; break; }
				if (t.kind == 110) {
					currentState = 225;
					break;
				} else {
					if (set[32, t.kind]) {
						goto case 226;
					} else {
						Error(t);
						goto case 225;
					}
				}
			}
			case 225: {
				stateStack.Push(221);
				goto case 29;
			}
			case 226: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 227;
			}
			case 227: {
				if (t == null) { currentState = 227; break; }
				if (set[33, t.kind]) {
					if (t.kind == 143) {
						currentState = 229;
						break;
					} else {
						goto case 229;
					}
				} else {
					if (set[12, t.kind]) {
						stateStack.Push(228);
						goto case 35;
					} else {
						Error(t);
						goto case 228;
					}
				}
			}
			case 228: {
				if (t == null) { currentState = 228; break; }
				if (t.kind == 23) {
					currentState = 226;
					break;
				} else {
					goto case 225;
				}
			}
			case 229: {
				stateStack.Push(230);
				goto case 231;
			}
			case 230: {
				stateStack.Push(228);
				goto case 106;
			}
			case 231: {
				if (t == null) { currentState = 231; break; }
				if (set[34, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 232: {
				stateStack.Push(233);
				goto case 35;
			}
			case 233: {
				if (t == null) { currentState = 233; break; }
				if (t.kind == 212) {
					currentState = 242;
					break;
				} else {
					goto case 234;
				}
			}
			case 234: {
				if (t == null) { currentState = 234; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 235;
				} else {
					goto case 6;
				}
			}
			case 235: {
				stateStack.Push(236);
				goto case 29;
			}
			case 236: {
				if (t == null) { currentState = 236; break; }
				if (t.kind == 110 || t.kind == 111) {
					if (t.kind == 110) {
						currentState = 241;
						break;
					} else {
						if (t.kind == 111) {
							goto case 238;
						} else {
							Error(t);
							goto case 235;
						}
					}
				} else {
					Expect(112, t); // "End"
					currentState = 237;
					break;
				}
			}
			case 237: {
				if (t == null) { currentState = 237; break; }
				Expect(134, t); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				if (t == null) { currentState = 238; break; }
				currentState = 239;
				break;
			}
			case 239: {
				stateStack.Push(240);
				goto case 35;
			}
			case 240: {
				if (t == null) { currentState = 240; break; }
				if (t.kind == 212) {
					currentState = 235;
					break;
				} else {
					goto case 235;
				}
			}
			case 241: {
				if (t == null) { currentState = 241; break; }
				if (t.kind == 134) {
					goto case 238;
				} else {
					goto case 235;
				}
			}
			case 242: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 243;
			}
			case 243: {
				if (t == null) { currentState = 243; break; }
				if (set[8, t.kind]) {
					goto case 244;
				} else {
					goto case 234;
				}
			}
			case 244: {
				stateStack.Push(245);
				goto case 33;
			}
			case 245: {
				if (t == null) { currentState = 245; break; }
				if (t.kind == 22) {
					currentState = 251;
					break;
				} else {
					if (t.kind == 110) {
						goto case 247;
					} else {
						goto case 246;
					}
				}
			}
			case 246: {
				if (t == null) { currentState = 246; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				if (t == null) { currentState = 247; break; }
				currentState = 248;
				break;
			}
			case 248: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 249;
			}
			case 249: {
				if (t == null) { currentState = 249; break; }
				if (set[8, t.kind]) {
					stateStack.Push(250);
					goto case 33;
				} else {
					goto case 250;
				}
			}
			case 250: {
				if (t == null) { currentState = 250; break; }
				if (t.kind == 22) {
					goto case 247;
				} else {
					goto case 246;
				}
			}
			case 251: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 252;
			}
			case 252: {
				if (t == null) { currentState = 252; break; }
				if (set[8, t.kind]) {
					goto case 244;
				} else {
					goto case 245;
				}
			}
			case 253: {
				stateStack.Push(254);
				goto case 16;
			}
			case 254: {
				if (t == null) { currentState = 254; break; }
				if (t.kind == 36) {
					currentState = 54;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 255: {
				stateStack.Push(256);
				goto case 35;
			}
			case 256: {
				if (t == null) { currentState = 256; break; }
				Expect(23, t); // ","
				currentState = 35;
				break;
			}
			case 257: {
				stateStack.Push(258);
				goto case 35;
			}
			case 258: {
				stateStack.Push(259);
				goto case 29;
			}
			case 259: {
				if (t == null) { currentState = 259; break; }
				Expect(112, t); // "End"
				currentState = 260;
				break;
			}
			case 260: {
				if (t == null) { currentState = 260; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 261: {
				stateStack.Push(262);
				goto case 70;
			}
			case 262: {
				if (t == null) { currentState = 262; break; }
				if (t.kind == 32) {
					currentState = 263;
					break;
				} else {
					goto case 263;
				}
			}
			case 263: {
				if (t == null) { currentState = 263; break; }
				if (t.kind == 36) {
					goto case 272;
				} else {
					goto case 264;
				}
			}
			case 264: {
				if (t == null) { currentState = 264; break; }
				if (t.kind == 23) {
					currentState = 266;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 265;
						break;
					} else {
						goto case 80;
					}
				}
			}
			case 265: {
				if (t == null) { currentState = 265; break; }
				if (t.kind == 161) {
					goto case 82;
				} else {
					goto case 83;
				}
			}
			case 266: {
				stateStack.Push(267);
				goto case 70;
			}
			case 267: {
				if (t == null) { currentState = 267; break; }
				if (t.kind == 32) {
					currentState = 268;
					break;
				} else {
					goto case 268;
				}
			}
			case 268: {
				if (t == null) { currentState = 268; break; }
				if (t.kind == 36) {
					goto case 269;
				} else {
					goto case 264;
				}
			}
			case 269: {
				if (t == null) { currentState = 269; break; }
				currentState = 270;
				break;
			}
			case 270: {
				if (t == null) { currentState = 270; break; }
				if (t.kind == 23) {
					goto case 269;
				} else {
					goto case 271;
				}
			}
			case 271: {
				if (t == null) { currentState = 271; break; }
				Expect(37, t); // ")"
				currentState = 264;
				break;
			}
			case 272: {
				if (t == null) { currentState = 272; break; }
				currentState = 273;
				break;
			}
			case 273: {
				if (t == null) { currentState = 273; break; }
				if (t.kind == 23) {
					goto case 272;
				} else {
					goto case 271;
				}
			}
			case 274: {
				if (t == null) { currentState = 274; break; }
				if (t.kind == 39) {
					stateStack.Push(274);
					goto case 84;
				} else {
					stateStack.Push(26);
					goto case 45;
				}
			}
			case 275: {
				if (t == null) { currentState = 275; break; }
				Expect(97, t); // "Custom"
				currentState = 276;
				break;
			}
			case 276: {
				stateStack.Push(277);
				goto case 287;
			}
			case 277: {
				if (t == null) { currentState = 277; break; }
				if (set[35, t.kind]) {
					goto case 279;
				} else {
					Expect(112, t); // "End"
					currentState = 278;
					break;
				}
			}
			case 278: {
				if (t == null) { currentState = 278; break; }
				Expect(118, t); // "Event"
				currentState = 15;
				break;
			}
			case 279: {
				if (t == null) { currentState = 279; break; }
				if (t.kind == 39) {
					stateStack.Push(279);
					goto case 84;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 280;
						break;
					} else {
						Error(t);
						goto case 280;
					}
				}
			}
			case 280: {
				if (t == null) { currentState = 280; break; }
				Expect(36, t); // "("
				currentState = 281;
				break;
			}
			case 281: {
				stateStack.Push(282);
				goto case 75;
			}
			case 282: {
				if (t == null) { currentState = 282; break; }
				Expect(37, t); // ")"
				currentState = 283;
				break;
			}
			case 283: {
				stateStack.Push(284);
				goto case 29;
			}
			case 284: {
				if (t == null) { currentState = 284; break; }
				Expect(112, t); // "End"
				currentState = 285;
				break;
			}
			case 285: {
				if (t == null) { currentState = 285; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 286;
					break;
				} else {
					Error(t);
					goto case 286;
				}
			}
			case 286: {
				stateStack.Push(277);
				goto case 15;
			}
			case 287: {
				if (t == null) { currentState = 287; break; }
				Expect(118, t); // "Event"
				currentState = 288;
				break;
			}
			case 288: {
				stateStack.Push(289);
				goto case 70;
			}
			case 289: {
				if (t == null) { currentState = 289; break; }
				if (t.kind == 62) {
					currentState = 296;
					break;
				} else {
					if (set[36, t.kind]) {
						if (t.kind == 36) {
							currentState = 294;
							break;
						} else {
							goto case 290;
						}
					} else {
						Error(t);
						goto case 290;
					}
				}
			}
			case 290: {
				if (t == null) { currentState = 290; break; }
				if (t.kind == 135) {
					goto case 291;
				} else {
					goto case 15;
				}
			}
			case 291: {
				if (t == null) { currentState = 291; break; }
				currentState = 292;
				break;
			}
			case 292: {
				stateStack.Push(293);
				goto case 45;
			}
			case 293: {
				if (t == null) { currentState = 293; break; }
				if (t.kind == 23) {
					goto case 291;
				} else {
					goto case 15;
				}
			}
			case 294: {
				if (t == null) { currentState = 294; break; }
				if (set[18, t.kind]) {
					stateStack.Push(295);
					goto case 75;
				} else {
					goto case 295;
				}
			}
			case 295: {
				if (t == null) { currentState = 295; break; }
				Expect(37, t); // ")"
				currentState = 290;
				break;
			}
			case 296: {
				stateStack.Push(290);
				goto case 45;
			}
			case 297: {
				if (t == null) { currentState = 297; break; }
				Expect(100, t); // "Declare"
				currentState = 298;
				break;
			}
			case 298: {
				if (t == null) { currentState = 298; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 299;
					break;
				} else {
					goto case 299;
				}
			}
			case 299: {
				if (t == null) { currentState = 299; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 300;
					break;
				} else {
					Error(t);
					goto case 300;
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 70;
			}
			case 301: {
				if (t == null) { currentState = 301; break; }
				Expect(148, t); // "Lib"
				currentState = 302;
				break;
			}
			case 302: {
				if (t == null) { currentState = 302; break; }
				Expect(3, t); // LiteralString
				currentState = 303;
				break;
			}
			case 303: {
				if (t == null) { currentState = 303; break; }
				if (t.kind == 58) {
					currentState = 307;
					break;
				} else {
					goto case 304;
				}
			}
			case 304: {
				if (t == null) { currentState = 304; break; }
				if (t.kind == 36) {
					currentState = 305;
					break;
				} else {
					goto case 15;
				}
			}
			case 305: {
				if (t == null) { currentState = 305; break; }
				if (set[18, t.kind]) {
					stateStack.Push(306);
					goto case 75;
				} else {
					goto case 306;
				}
			}
			case 306: {
				if (t == null) { currentState = 306; break; }
				Expect(37, t); // ")"
				currentState = 15;
				break;
			}
			case 307: {
				if (t == null) { currentState = 307; break; }
				Expect(3, t); // LiteralString
				currentState = 304;
				break;
			}
			case 308: {
				if (t == null) { currentState = 308; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 309;
					break;
				} else {
					Error(t);
					goto case 309;
				}
			}
			case 309: {
				PushContext(Context.IdentifierExpected, t);
				goto case 310;
			}
			case 310: {
				if (t == null) { currentState = 310; break; }
				currentState = 311;
				break;
			}
			case 311: {
				PopContext();
				goto case 312;
			}
			case 312: {
				if (t == null) { currentState = 312; break; }
				if (t.kind == 36) {
					currentState = 318;
					break;
				} else {
					goto case 313;
				}
			}
			case 313: {
				if (t == null) { currentState = 313; break; }
				if (t.kind == 62) {
					currentState = 317;
					break;
				} else {
					goto case 314;
				}
			}
			case 314: {
				stateStack.Push(315);
				goto case 29;
			}
			case 315: {
				if (t == null) { currentState = 315; break; }
				Expect(112, t); // "End"
				currentState = 316;
				break;
			}
			case 316: {
				if (t == null) { currentState = 316; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 15;
					break;
				} else {
					Error(t);
					goto case 15;
				}
			}
			case 317: {
				stateStack.Push(314);
				goto case 45;
			}
			case 318: {
				if (t == null) { currentState = 318; break; }
				if (set[18, t.kind]) {
					stateStack.Push(319);
					goto case 75;
				} else {
					goto case 319;
				}
			}
			case 319: {
				if (t == null) { currentState = 319; break; }
				Expect(37, t); // ")"
				currentState = 313;
				break;
			}
			case 320: {
				if (t == null) { currentState = 320; break; }
				if (t.kind == 87) {
					currentState = 321;
					break;
				} else {
					goto case 321;
				}
			}
			case 321: {
				stateStack.Push(322);
				goto case 326;
			}
			case 322: {
				if (t == null) { currentState = 322; break; }
				if (t.kind == 62) {
					currentState = 325;
					break;
				} else {
					goto case 323;
				}
			}
			case 323: {
				if (t == null) { currentState = 323; break; }
				if (t.kind == 21) {
					currentState = 324;
					break;
				} else {
					goto case 15;
				}
			}
			case 324: {
				stateStack.Push(15);
				goto case 35;
			}
			case 325: {
				stateStack.Push(323);
				goto case 45;
			}
			case 326: {
				if (t == null) { currentState = 326; break; }
				if (set[37, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 327: {
				if (t == null) { currentState = 327; break; }
				currentState = 9;
				break;
			}
			case 328: {
				if (t == null) { currentState = 328; break; }
				currentState = 329;
				break;
			}
			case 329: {
				if (t == null) { currentState = 329; break; }
				if (set[3, t.kind]) {
					goto case 328;
				} else {
					stateStack.Push(330);
					goto case 15;
				}
			}
			case 330: {
				if (t == null) { currentState = 330; break; }
				if (set[38, t.kind]) {
					stateStack.Push(330);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 331;
					break;
				}
			}
			case 331: {
				if (t == null) { currentState = 331; break; }
				Expect(159, t); // "Namespace"
				currentState = 15;
				break;
			}
			case 332: {
				if (t == null) { currentState = 332; break; }
				Expect(136, t); // "Imports"
				currentState = 333;
				break;
			}
			case 333: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 334;
			}
			case 334: {
				if (t == null) { currentState = 334; break; }
				if (set[3, t.kind]) {
					currentState = 334;
					break;
				} else {
					goto case 15;
				}
			}
			case 335: {
				if (t == null) { currentState = 335; break; }
				Expect(172, t); // "Option"
				currentState = 336;
				break;
			}
			case 336: {
				if (t == null) { currentState = 336; break; }
				if (set[3, t.kind]) {
					currentState = 336;
					break;
				} else {
					goto case 15;
				}
			}
		}

	}
	
	public void Advance()
	{
		Console.WriteLine("Advance");
		InformTokenInternal(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,T, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};

} // end Parser


}