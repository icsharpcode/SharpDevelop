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
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	List<Token> errors = new List<Token>();
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token la)
	{
		if (la.kind != expectedKind) {
			Error(la);
			output.AppendLine("expected: " + expectedKind);
			Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
		readXmlIdentifier = false;
		nextTokenIsStartOfImportsOrAccessExpression = false;
		const int endOfStatementTerminatorAndBlock = 157;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 438;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 435;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 299;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[0, la.kind]) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (la == null) { currentState = 5; break; }
				if (la.kind == 160) {
					goto case 431;
				} else {
					if (set[1, la.kind]) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (la == null) { currentState = 7; break; }
				if (la.kind == 40) {
					stateStack.Push(7);
					goto case 299;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[2, la.kind]) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						currentState = 336;
						break;
					} else {
						if (la.kind == 103) {
							currentState = 9;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 9: {
				if (la == null) { currentState = 9; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 10;
					break;
				} else {
					Error(la);
					goto case 10;
				}
			}
			case 10: {
				PushContext(Context.IdentifierExpected, t);
				goto case 11;
			}
			case 11: {
				if (la == null) { currentState = 11; break; }
				currentState = 12;
				break;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 334;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (la.kind == 63) {
					currentState = 17;
					break;
				} else {
					goto case 15;
				}
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				stateStack.Push(15);
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[3, la.kind]) {
					goto case 333;
				} else {
					Error(la);
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 37) {
					stateStack.Push(19);
					goto case 23;
				} else {
					goto case 20;
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.kind == 26) {
					currentState = 21;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 21: {
				stateStack.Push(22);
				goto case 50;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 37) {
					stateStack.Push(22);
					goto case 23;
				} else {
					goto case 20;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				Expect(37, la); // "("
				currentState = 24;
				break;
			}
			case 24: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 25;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 169) {
					goto case 330;
				} else {
					if (set[4, la.kind]) {
						goto case 27;
					} else {
						Error(la);
						goto case 26;
					}
				}
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 27: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 28;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[5, la.kind]) {
					goto case 29;
				} else {
					goto case 26;
				}
			}
			case 29: {
				stateStack.Push(26);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 30;
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (set[6, la.kind]) {
					goto case 326;
				} else {
					if (la.kind == 22) {
						goto case 31;
					} else {
						goto case 6;
					}
				}
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				currentState = 32;
				break;
			}
			case 32: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 33;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (set[6, la.kind]) {
					stateStack.Push(34);
					goto case 35;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 22) {
					goto case 31;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 36;
			}
			case 36: {
				stateStack.Push(37);
				goto case 38;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (set[7, la.kind]) {
					currentState = 36;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 38: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (set[8, la.kind]) {
					currentState = 38;
					break;
				} else {
					if (set[9, la.kind]) {
						stateStack.Push(60);
						goto case 71;
					} else {
						if (la.kind == 220) {
							currentState = 58;
							break;
						} else {
							if (la.kind == 162) {
								currentState = 40;
								break;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (set[3, la.kind]) {
					stateStack.Push(51);
					goto case 18;
				} else {
					goto case 41;
				}
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 233) {
					currentState = 42;
					break;
				} else {
					goto case 6;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(35, la); // "{"
				currentState = 43;
				break;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 147) {
					currentState = 44;
					break;
				} else {
					goto case 44;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				Expect(26, la); // "."
				currentState = 45;
				break;
			}
			case 45: {
				stateStack.Push(46);
				goto case 50;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(20, la); // "="
				currentState = 47;
				break;
			}
			case 47: {
				stateStack.Push(48);
				goto case 35;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (la.kind == 22) {
					currentState = 43;
					break;
				} else {
					goto case 49;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (set[10, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 52;
						break;
					} else {
						goto case 41;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.kind == 35) {
					goto case 53;
				} else {
					if (set[11, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 6;
					}
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				currentState = 54;
				break;
			}
			case 54: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 55;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (set[6, la.kind]) {
					stateStack.Push(56);
					goto case 35;
				} else {
					if (la.kind == 35) {
						stateStack.Push(56);
						goto case 57;
					} else {
						Error(la);
						goto case 56;
					}
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (la.kind == 22) {
					goto case 53;
				} else {
					goto case 49;
				}
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				Expect(35, la); // "{"
				currentState = 54;
				break;
			}
			case 58: {
				stateStack.Push(59);
				goto case 38;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				Expect(144, la); // "Is"
				currentState = 18;
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (set[12, la.kind]) {
					stateStack.Push(60);
					goto case 61;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.kind == 37) {
					currentState = 66;
					break;
				} else {
					if (set[13, la.kind]) {
						currentState = 62;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 62: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 63;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (la.kind == 10) {
					currentState = 64;
					break;
				} else {
					goto case 64;
				}
			}
			case 64: {
				stateStack.Push(65);
				goto case 50;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 11) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 66: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 67;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				if (la.kind == 169) {
					goto case 68;
				} else {
					if (set[5, la.kind]) {
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				currentState = 69;
				break;
			}
			case 69: {
				stateStack.Push(70);
				goto case 18;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 22) {
					goto case 68;
				} else {
					goto case 26;
				}
			}
			case 71: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 72;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				if (set[14, la.kind]) {
					goto case 16;
				} else {
					if (la.kind == 37) {
						goto case 78;
					} else {
						if (set[15, la.kind]) {
							goto case 16;
						} else {
							if (la.kind == 26 || la.kind == 29) {
								currentState = 50;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 325;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 323;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 311;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												if (la.kind == 210) {
													currentState = 304;
													break;
												} else {
													if (la.kind == 127) {
														currentState = 145;
														break;
													} else {
														goto case 6;
													}
												}
											} else {
												if (la.kind == 58 || la.kind == 126) {
													if (la.kind == 126) {
														stateStack.Push(85);
														goto case 144;
													} else {
														if (la.kind == 58) {
															stateStack.Push(85);
															goto case 143;
														} else {
															Error(la);
															goto case 85;
														}
													}
												} else {
													if (set[16, la.kind]) {
														if (set[17, la.kind]) {
															currentState = 84;
															break;
														} else {
															if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
																currentState = 80;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 135) {
															currentState = 73;
															break;
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
			case 73: {
				if (la == null) { currentState = 73; break; }
				Expect(37, la); // "("
				currentState = 74;
				break;
			}
			case 74: {
				stateStack.Push(75);
				goto case 35;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				Expect(22, la); // ","
				currentState = 76;
				break;
			}
			case 76: {
				stateStack.Push(77);
				goto case 35;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				if (la.kind == 22) {
					goto case 78;
				} else {
					goto case 26;
				}
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				currentState = 79;
				break;
			}
			case 79: {
				stateStack.Push(26);
				goto case 35;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				Expect(37, la); // "("
				currentState = 81;
				break;
			}
			case 81: {
				stateStack.Push(82);
				goto case 35;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				Expect(22, la); // ","
				currentState = 83;
				break;
			}
			case 83: {
				stateStack.Push(26);
				goto case 18;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				Expect(37, la); // "("
				currentState = 79;
				break;
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				if (set[18, la.kind]) {
					stateStack.Push(85);
					goto case 86;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				if (la.kind == 126) {
					goto case 140;
				} else {
					if (la.kind == 58) {
						currentState = 136;
						break;
					} else {
						if (la.kind == 197) {
							goto case 133;
						} else {
							if (la.kind == 107) {
								goto case 16;
							} else {
								if (la.kind == 230) {
									goto case 110;
								} else {
									if (la.kind == 176) {
										currentState = 129;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 127;
											break;
										} else {
											if (la.kind == 148) {
												goto case 124;
											} else {
												if (la.kind == 133) {
													currentState = 99;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 87;
														break;
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
			case 87: {
				stateStack.Push(88);
				goto case 93;
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				Expect(171, la); // "On"
				currentState = 89;
				break;
			}
			case 89: {
				stateStack.Push(90);
				goto case 35;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				Expect(116, la); // "Equals"
				currentState = 91;
				break;
			}
			case 91: {
				stateStack.Push(92);
				goto case 35;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				if (la.kind == 22) {
					currentState = 89;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 93: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(94);
				goto case 98;
			}
			case 94: {
				PopContext();
				goto case 95;
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				if (la.kind == 63) {
					currentState = 97;
					break;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				Expect(138, la); // "In"
				currentState = 35;
				break;
			}
			case 97: {
				stateStack.Push(96);
				goto case 18;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				if (set[19, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 99: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 100;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.kind == 146) {
					goto case 116;
				} else {
					if (set[20, la.kind]) {
						if (la.kind == 70) {
							goto case 113;
						} else {
							if (set[20, la.kind]) {
								goto case 114;
							} else {
								Error(la);
								goto case 101;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				Expect(70, la); // "By"
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(103);
				goto case 106;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				if (la.kind == 22) {
					goto case 113;
				} else {
					Expect(143, la); // "Into"
					currentState = 104;
					break;
				}
			}
			case 104: {
				stateStack.Push(105);
				goto case 106;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				if (la.kind == 22) {
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
				if (la == null) { currentState = 107; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(108);
					goto case 98;
				} else {
					goto case 35;
				}
			}
			case 108: {
				PopContext();
				goto case 109;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (la.kind == 63) {
					currentState = 111;
					break;
				} else {
					if (la.kind == 20) {
						goto case 110;
					} else {
						if (set[21, la.kind]) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 35;
						}
					}
				}
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				currentState = 35;
				break;
			}
			case 111: {
				stateStack.Push(112);
				goto case 18;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				Expect(20, la); // "="
				currentState = 35;
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				currentState = 102;
				break;
			}
			case 114: {
				stateStack.Push(115);
				goto case 106;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.kind == 22) {
					currentState = 114;
					break;
				} else {
					goto case 101;
				}
			}
			case 116: {
				stateStack.Push(117);
				goto case 123;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 121;
						break;
					} else {
						if (la.kind == 146) {
							goto case 116;
						} else {
							Error(la);
							goto case 117;
						}
					}
				} else {
					goto case 118;
				}
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				Expect(143, la); // "Into"
				currentState = 119;
				break;
			}
			case 119: {
				stateStack.Push(120);
				goto case 106;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				if (la.kind == 22) {
					currentState = 119;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 121: {
				stateStack.Push(122);
				goto case 123;
			}
			case 122: {
				stateStack.Push(117);
				goto case 118;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				Expect(146, la); // "Join"
				currentState = 87;
				break;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				currentState = 125;
				break;
			}
			case 125: {
				stateStack.Push(126);
				goto case 106;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 22) {
					goto case 124;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 127: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 128;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (la.kind == 231) {
					goto case 110;
				} else {
					goto case 35;
				}
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				Expect(70, la); // "By"
				currentState = 130;
				break;
			}
			case 130: {
				stateStack.Push(131);
				goto case 35;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				if (la.kind == 64 || la.kind == 104) {
					currentState = 132;
					break;
				} else {
					Error(la);
					goto case 132;
				}
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 22) {
					currentState = 130;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				currentState = 134;
				break;
			}
			case 134: {
				stateStack.Push(135);
				goto case 106;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.kind == 22) {
					goto case 133;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 136: {
				stateStack.Push(137);
				goto case 93;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (set[18, la.kind]) {
					stateStack.Push(137);
					goto case 86;
				} else {
					Expect(143, la); // "Into"
					currentState = 138;
					break;
				}
			}
			case 138: {
				stateStack.Push(139);
				goto case 106;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (la.kind == 22) {
					currentState = 138;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				currentState = 141;
				break;
			}
			case 141: {
				stateStack.Push(142);
				goto case 93;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				if (la.kind == 22) {
					goto case 140;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				Expect(58, la); // "Aggregate"
				currentState = 136;
				break;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(126, la); // "From"
				currentState = 141;
				break;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(37, la); // "("
				currentState = 146;
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (set[22, la.kind]) {
					stateStack.Push(147);
					goto case 293;
				} else {
					goto case 147;
				}
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(38, la); // ")"
				currentState = 148;
				break;
			}
			case 148: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 149;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				if (set[6, la.kind]) {
					goto case 35;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 292;
							break;
						} else {
							goto case 150;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 150: {
				stateStack.Push(151);
				goto case 153;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(113, la); // "End"
				currentState = 152;
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 153: {
				PushContext(Context.Body, t);
				goto case 154;
			}
			case 154: {
				stateStack.Push(155);
				goto case 15;
			}
			case 155: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 156;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (set[23, la.kind]) {
					if (set[24, la.kind]) {
						if (set[25, la.kind]) {
							stateStack.Push(154);
							goto case 161;
						} else {
							goto case 154;
						}
					} else {
						if (la.kind == 113) {
							currentState = 159;
							break;
						} else {
							goto case 158;
						}
					}
				} else {
					goto case 157;
				}
			}
			case 157: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 158: {
				Error(la);
				goto case 155;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 160;
				} else {
					if (set[26, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 158;
					}
				}
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				currentState = 155;
				break;
			}
			case 161: {
				if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 162;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 274;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 270;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 268;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 266;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 245;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 230;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 226;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 220;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 195;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 190;
																break;
															} else {
																goto case 190;
															}
														} else {
															if (la.kind == 194) {
																currentState = 189;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															goto case 172;
														} else {
															if (la.kind == 218) {
																currentState = 178;
																break;
															} else {
																if (set[27, la.kind]) {
																	if (la.kind == 132) {
																		currentState = 177;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 176;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 175;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 16;
																				} else {
																					if (la.kind == 195) {
																						goto case 172;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 170;
																		break;
																	} else {
																		if (la.kind == 117) {
																			goto case 167;
																		} else {
																			if (la.kind == 226) {
																				currentState = 163;
																				break;
																			} else {
																				if (set[28, la.kind]) {
																					if (la.kind == 73) {
																						goto case 110;
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
			case 163: {
				stateStack.Push(164);
				goto case 35;
			}
			case 164: {
				stateStack.Push(165);
				goto case 153;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				Expect(113, la); // "End"
				currentState = 166;
				break;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				currentState = 168;
				break;
			}
			case 168: {
				stateStack.Push(169);
				goto case 35;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (la.kind == 22) {
					goto case 167;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 170: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 171;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.kind == 184) {
					goto case 110;
				} else {
					goto case 35;
				}
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				currentState = 173;
				break;
			}
			case 173: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 174;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (set[6, la.kind]) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 108 || la.kind == 124 || la.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (set[29, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 178: {
				stateStack.Push(179);
				goto case 153;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				if (la.kind == 75) {
					currentState = 183;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 182;
						break;
					} else {
						goto case 180;
					}
				}
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(113, la); // "End"
				currentState = 181;
				break;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 182: {
				stateStack.Push(180);
				goto case 153;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(186);
					goto case 98;
				} else {
					goto case 184;
				}
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (la.kind == 229) {
					currentState = 185;
					break;
				} else {
					goto case 178;
				}
			}
			case 185: {
				stateStack.Push(178);
				goto case 35;
			}
			case 186: {
				PopContext();
				goto case 187;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 63) {
					currentState = 188;
					break;
				} else {
					goto case 184;
				}
			}
			case 188: {
				stateStack.Push(184);
				goto case 18;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (set[31, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				Expect(118, la); // "Error"
				currentState = 191;
				break;
			}
			case 191: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 192;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (set[6, la.kind]) {
					goto case 35;
				} else {
					if (la.kind == 132) {
						currentState = 194;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 193;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 195: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 196;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (set[9, la.kind]) {
					stateStack.Push(210);
					goto case 206;
				} else {
					if (la.kind == 110) {
						currentState = 197;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 197: {
				stateStack.Push(198);
				goto case 206;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(138, la); // "In"
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(200);
				goto case 35;
			}
			case 200: {
				stateStack.Push(201);
				goto case 153;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(163, la); // "Next"
				currentState = 202;
				break;
			}
			case 202: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 203;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (set[6, la.kind]) {
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
				if (la == null) { currentState = 205; break; }
				if (la.kind == 22) {
					currentState = 204;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 206: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(207);
				goto case 71;
			}
			case 207: {
				PopContext();
				goto case 208;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 33) {
					currentState = 209;
					break;
				} else {
					goto case 209;
				}
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (set[12, la.kind]) {
					stateStack.Push(209);
					goto case 61;
				} else {
					if (la.kind == 63) {
						currentState = 18;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				Expect(20, la); // "="
				currentState = 211;
				break;
			}
			case 211: {
				stateStack.Push(212);
				goto case 35;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 205) {
					currentState = 219;
					break;
				} else {
					goto case 213;
				}
			}
			case 213: {
				stateStack.Push(214);
				goto case 153;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				Expect(163, la); // "Next"
				currentState = 215;
				break;
			}
			case 215: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 216;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (set[6, la.kind]) {
					goto case 217;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 217: {
				stateStack.Push(218);
				goto case 35;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (la.kind == 22) {
					currentState = 217;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 219: {
				stateStack.Push(213);
				goto case 35;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 223;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(221);
						goto case 153;
					} else {
						goto case 6;
					}
				}
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(152, la); // "Loop"
				currentState = 222;
				break;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				if (la.kind == 224 || la.kind == 231) {
					goto case 110;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 223: {
				stateStack.Push(224);
				goto case 35;
			}
			case 224: {
				stateStack.Push(225);
				goto case 153;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 226: {
				stateStack.Push(227);
				goto case 35;
			}
			case 227: {
				stateStack.Push(228);
				goto case 153;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(113, la); // "End"
				currentState = 229;
				break;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 230: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 231;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 74) {
					currentState = 232;
					break;
				} else {
					goto case 232;
				}
			}
			case 232: {
				stateStack.Push(233);
				goto case 35;
			}
			case 233: {
				stateStack.Push(234);
				goto case 15;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (la.kind == 74) {
					currentState = 236;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 235;
					break;
				}
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 111) {
					currentState = 238;
					break;
				} else {
					if (set[32, la.kind]) {
						goto case 239;
					} else {
						Error(la);
						goto case 238;
					}
				}
			}
			case 238: {
				stateStack.Push(234);
				goto case 153;
			}
			case 239: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 240;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (set[33, la.kind]) {
					if (la.kind == 144) {
						currentState = 242;
						break;
					} else {
						goto case 242;
					}
				} else {
					if (set[6, la.kind]) {
						stateStack.Push(241);
						goto case 35;
					} else {
						Error(la);
						goto case 241;
					}
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (la.kind == 22) {
					currentState = 239;
					break;
				} else {
					goto case 238;
				}
			}
			case 242: {
				stateStack.Push(243);
				goto case 244;
			}
			case 243: {
				stateStack.Push(241);
				goto case 38;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (set[34, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 245: {
				stateStack.Push(246);
				goto case 35;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.kind == 214) {
					currentState = 255;
					break;
				} else {
					goto case 247;
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 248;
				} else {
					goto case 6;
				}
			}
			case 248: {
				stateStack.Push(249);
				goto case 153;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 254;
						break;
					} else {
						if (la.kind == 112) {
							goto case 251;
						} else {
							Error(la);
							goto case 248;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 250;
					break;
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				currentState = 252;
				break;
			}
			case 252: {
				stateStack.Push(253);
				goto case 35;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (la.kind == 214) {
					currentState = 248;
					break;
				} else {
					goto case 248;
				}
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 135) {
					goto case 251;
				} else {
					goto case 248;
				}
			}
			case 255: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 256;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[25, la.kind]) {
					goto case 257;
				} else {
					goto case 247;
				}
			}
			case 257: {
				stateStack.Push(258);
				goto case 161;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 21) {
					currentState = 264;
					break;
				} else {
					if (la.kind == 111) {
						goto case 260;
					} else {
						goto case 259;
					}
				}
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				currentState = 261;
				break;
			}
			case 261: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 262;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (set[25, la.kind]) {
					stateStack.Push(263);
					goto case 161;
				} else {
					goto case 263;
				}
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.kind == 21) {
					goto case 260;
				} else {
					goto case 259;
				}
			}
			case 264: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 265;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (set[25, la.kind]) {
					goto case 257;
				} else {
					goto case 258;
				}
			}
			case 266: {
				stateStack.Push(267);
				goto case 50;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (la.kind == 37) {
					currentState = 27;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 268: {
				stateStack.Push(269);
				goto case 35;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				Expect(22, la); // ","
				currentState = 35;
				break;
			}
			case 270: {
				stateStack.Push(271);
				goto case 35;
			}
			case 271: {
				stateStack.Push(272);
				goto case 153;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				Expect(113, la); // "End"
				currentState = 273;
				break;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 211 || la.kind == 233) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 274: {
				PushContext(Context.IdentifierExpected, t);
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				stateStack.Push(275);
				goto case 98;
			}
			case 275: {
				PopContext();
				goto case 276;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (la.kind == 33) {
					currentState = 277;
					break;
				} else {
					goto case 277;
				}
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (la.kind == 37) {
					goto case 290;
				} else {
					goto case 278;
				}
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (la.kind == 22) {
					currentState = 283;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 280;
						break;
					} else {
						goto case 279;
					}
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (la.kind == 20) {
					goto case 110;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 162) {
					goto case 282;
				} else {
					goto case 281;
				}
			}
			case 281: {
				stateStack.Push(279);
				goto case 18;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				currentState = 281;
				break;
			}
			case 283: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(284);
				goto case 98;
			}
			case 284: {
				PopContext();
				goto case 285;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (la.kind == 33) {
					currentState = 286;
					break;
				} else {
					goto case 286;
				}
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 37) {
					goto case 287;
				} else {
					goto case 278;
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				currentState = 288;
				break;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.kind == 22) {
					goto case 287;
				} else {
					goto case 289;
				}
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				Expect(38, la); // ")"
				currentState = 278;
				break;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				currentState = 291;
				break;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.kind == 22) {
					goto case 290;
				} else {
					goto case 289;
				}
			}
			case 292: {
				stateStack.Push(150);
				goto case 18;
			}
			case 293: {
				stateStack.Push(294);
				goto case 295;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 22) {
					currentState = 293;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 40) {
					stateStack.Push(295);
					goto case 299;
				} else {
					goto case 296;
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (set[35, la.kind]) {
					currentState = 296;
					break;
				} else {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(297);
					goto case 98;
				}
			}
			case 297: {
				PopContext();
				goto case 298;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 63) {
					goto case 282;
				} else {
					goto case 279;
				}
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				Expect(40, la); // "<"
				currentState = 300;
				break;
			}
			case 300: {
				PushContext(Context.Attribute, t);
				goto case 301;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (set[36, la.kind]) {
					currentState = 301;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 302;
					break;
				}
			}
			case 302: {
				PopContext();
				goto case 303;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(37, la); // "("
				currentState = 305;
				break;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (set[22, la.kind]) {
					stateStack.Push(306);
					goto case 293;
				} else {
					goto case 306;
				}
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				Expect(38, la); // ")"
				currentState = 307;
				break;
			}
			case 307: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 308;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (set[25, la.kind]) {
					goto case 161;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(309);
						goto case 153;
					} else {
						goto case 6;
					}
				}
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				Expect(113, la); // "End"
				currentState = 310;
				break;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 322;
					break;
				} else {
					stateStack.Push(312);
					goto case 314;
				}
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 17) {
					currentState = 313;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 16) {
					currentState = 312;
					break;
				} else {
					goto case 312;
				}
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 315;
				break;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (set[37, la.kind]) {
					if (set[38, la.kind]) {
						currentState = 315;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(315);
							goto case 319;
						} else {
							Error(la);
							goto case 315;
						}
					}
				} else {
					if (la.kind == 14) {
						goto case 16;
					} else {
						if (la.kind == 11) {
							goto case 316;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				currentState = 317;
				break;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (set[39, la.kind]) {
					if (set[40, la.kind]) {
						goto case 316;
					} else {
						if (la.kind == 12) {
							stateStack.Push(317);
							goto case 319;
						} else {
							if (la.kind == 10) {
								stateStack.Push(317);
								goto case 314;
							} else {
								Error(la);
								goto case 317;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 318;
					break;
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (set[41, la.kind]) {
					if (set[42, la.kind]) {
						currentState = 318;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(318);
							goto case 319;
						} else {
							Error(la);
							goto case 318;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 320;
				break;
			}
			case 320: {
				stateStack.Push(321);
				goto case 35;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 16) {
					currentState = 311;
					break;
				} else {
					goto case 311;
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				Expect(37, la); // "("
				currentState = 324;
				break;
			}
			case 324: {
				readXmlIdentifier = true;
				stateStack.Push(26);
				goto case 98;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				Expect(37, la); // "("
				currentState = 83;
				break;
			}
			case 326: {
				stateStack.Push(327);
				goto case 35;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 22) {
					currentState = 328;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 328: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 329;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (set[6, la.kind]) {
					goto case 326;
				} else {
					goto case 327;
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				currentState = 331;
				break;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (set[3, la.kind]) {
					stateStack.Push(332);
					goto case 18;
				} else {
					goto case 332;
				}
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 22) {
					goto case 330;
				} else {
					goto case 26;
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				currentState = 19;
				break;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (set[22, la.kind]) {
					stateStack.Push(335);
					goto case 293;
				} else {
					goto case 335;
				}
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 336: {
				PushContext(Context.IdentifierExpected, t);
				goto case 337;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				currentState = 338;
				break;
			}
			case 338: {
				PopContext();
				goto case 339;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 37) {
					currentState = 419;
					break;
				} else {
					goto case 340;
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (set[43, la.kind]) {
					currentState = 340;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						currentState = 341;
						break;
					} else {
						goto case 341;
					}
				}
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 140) {
					goto case 417;
				} else {
					goto case 342;
				}
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 136) {
					goto case 415;
				} else {
					goto case 343;
				}
			}
			case 343: {
				PushContext(Context.Type, t);
				goto case 344;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (set[44, la.kind]) {
					stateStack.Push(344);
					PushContext(Context.Member, t);
					goto case 348;
				} else {
					Expect(113, la); // "End"
					currentState = 345;
					break;
				}
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					currentState = 346;
					break;
				} else {
					Error(la);
					goto case 346;
				}
			}
			case 346: {
				stateStack.Push(347);
				goto case 15;
			}
			case 347: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 40) {
					stateStack.Push(348);
					goto case 299;
				} else {
					goto case 349;
				}
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (set[45, la.kind]) {
					currentState = 349;
					break;
				} else {
					if (set[46, la.kind]) {
						stateStack.Push(350);
						goto case 407;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(350);
							goto case 395;
						} else {
							if (la.kind == 101) {
								stateStack.Push(350);
								goto case 386;
							} else {
								if (la.kind == 119) {
									stateStack.Push(350);
									goto case 375;
								} else {
									if (la.kind == 98) {
										stateStack.Push(350);
										goto case 363;
									} else {
										if (la.kind == 172) {
											stateStack.Push(350);
											goto case 351;
										} else {
											Error(la);
											goto case 350;
										}
									}
								}
							}
						}
					}
				}
			}
			case 350: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				Expect(172, la); // "Operator"
				currentState = 352;
				break;
			}
			case 352: {
				PushContext(Context.IdentifierExpected, t);
				goto case 353;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				currentState = 354;
				break;
			}
			case 354: {
				PopContext();
				goto case 355;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(37, la); // "("
				currentState = 356;
				break;
			}
			case 356: {
				stateStack.Push(357);
				goto case 293;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(38, la); // ")"
				currentState = 358;
				break;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 63) {
					currentState = 362;
					break;
				} else {
					goto case 359;
				}
			}
			case 359: {
				stateStack.Push(360);
				goto case 153;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				Expect(113, la); // "End"
				currentState = 361;
				break;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 40) {
					stateStack.Push(362);
					goto case 299;
				} else {
					stateStack.Push(359);
					goto case 18;
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(98, la); // "Custom"
				currentState = 364;
				break;
			}
			case 364: {
				stateStack.Push(365);
				goto case 375;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (set[47, la.kind]) {
					goto case 367;
				} else {
					Expect(113, la); // "End"
					currentState = 366;
					break;
				}
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (la.kind == 40) {
					stateStack.Push(367);
					goto case 299;
				} else {
					if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
						currentState = 368;
						break;
					} else {
						Error(la);
						goto case 368;
					}
				}
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(37, la); // "("
				currentState = 369;
				break;
			}
			case 369: {
				stateStack.Push(370);
				goto case 293;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				Expect(38, la); // ")"
				currentState = 371;
				break;
			}
			case 371: {
				stateStack.Push(372);
				goto case 153;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(113, la); // "End"
				currentState = 373;
				break;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
					currentState = 374;
					break;
				} else {
					Error(la);
					goto case 374;
				}
			}
			case 374: {
				stateStack.Push(365);
				goto case 15;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				Expect(119, la); // "Event"
				currentState = 376;
				break;
			}
			case 376: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(377);
				goto case 98;
			}
			case 377: {
				PopContext();
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 63) {
					currentState = 385;
					break;
				} else {
					if (set[48, la.kind]) {
						if (la.kind == 37) {
							currentState = 383;
							break;
						} else {
							goto case 379;
						}
					} else {
						Error(la);
						goto case 379;
					}
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 136) {
					goto case 380;
				} else {
					goto case 15;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				currentState = 381;
				break;
			}
			case 381: {
				stateStack.Push(382);
				goto case 18;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (la.kind == 22) {
					goto case 380;
				} else {
					goto case 15;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (set[22, la.kind]) {
					stateStack.Push(384);
					goto case 293;
				} else {
					goto case 384;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(38, la); // ")"
				currentState = 379;
				break;
			}
			case 385: {
				stateStack.Push(379);
				goto case 18;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(101, la); // "Declare"
				currentState = 387;
				break;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 388;
					break;
				} else {
					goto case 388;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 389;
					break;
				} else {
					Error(la);
					goto case 389;
				}
			}
			case 389: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(390);
				goto case 98;
			}
			case 390: {
				PopContext();
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				Expect(149, la); // "Lib"
				currentState = 392;
				break;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				Expect(3, la); // LiteralString
				currentState = 393;
				break;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 59) {
					currentState = 394;
					break;
				} else {
					goto case 13;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				Expect(3, la); // LiteralString
				currentState = 13;
				break;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 396;
					break;
				} else {
					Error(la);
					goto case 396;
				}
			}
			case 396: {
				PushContext(Context.IdentifierExpected, t);
				goto case 397;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				currentState = 398;
				break;
			}
			case 398: {
				PopContext();
				goto case 399;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (la.kind == 37) {
					currentState = 405;
					break;
				} else {
					goto case 400;
				}
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (la.kind == 63) {
					currentState = 404;
					break;
				} else {
					goto case 401;
				}
			}
			case 401: {
				stateStack.Push(402);
				goto case 153;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				Expect(113, la); // "End"
				currentState = 403;
				break;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 15;
					break;
				} else {
					Error(la);
					goto case 15;
				}
			}
			case 404: {
				stateStack.Push(401);
				goto case 18;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (set[22, la.kind]) {
					stateStack.Push(406);
					goto case 293;
				} else {
					goto case 406;
				}
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				Expect(38, la); // ")"
				currentState = 400;
				break;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (la.kind == 88) {
					currentState = 408;
					break;
				} else {
					goto case 408;
				}
			}
			case 408: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(409);
				goto case 414;
			}
			case 409: {
				PopContext();
				goto case 410;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (la.kind == 63) {
					currentState = 413;
					break;
				} else {
					goto case 411;
				}
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 20) {
					currentState = 412;
					break;
				} else {
					goto case 15;
				}
			}
			case 412: {
				stateStack.Push(15);
				goto case 35;
			}
			case 413: {
				stateStack.Push(411);
				goto case 18;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (set[49, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				currentState = 416;
				break;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (set[26, la.kind]) {
					goto case 415;
				} else {
					stateStack.Push(343);
					goto case 15;
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				currentState = 418;
				break;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (set[26, la.kind]) {
					goto case 417;
				} else {
					stateStack.Push(342);
					goto case 15;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				Expect(169, la); // "Of"
				currentState = 420;
				break;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 421;
					break;
				} else {
					goto case 421;
				}
			}
			case 421: {
				stateStack.Push(422);
				goto case 430;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (la.kind == 63) {
					currentState = 424;
					break;
				} else {
					goto case 423;
				}
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 22) {
					currentState = 420;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 340;
					break;
				}
			}
			case 424: {
				stateStack.Push(423);
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (set[50, la.kind]) {
					goto case 429;
				} else {
					if (la.kind == 35) {
						goto case 426;
					} else {
						goto case 6;
					}
				}
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				currentState = 427;
				break;
			}
			case 427: {
				stateStack.Push(428);
				goto case 429;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 22) {
					goto case 426;
				} else {
					goto case 49;
				}
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (set[3, la.kind]) {
					goto case 333;
				} else {
					if (la.kind == 84 || la.kind == 162 || la.kind == 209) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (set[51, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				currentState = 432;
				break;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (set[26, la.kind]) {
					goto case 431;
				} else {
					stateStack.Push(433);
					goto case 15;
				}
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (set[52, la.kind]) {
					stateStack.Push(433);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 434;
					break;
				}
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				Expect(137, la); // "Imports"
				currentState = 436;
				break;
			}
			case 436: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (set[26, la.kind]) {
					currentState = 437;
					break;
				} else {
					goto case 15;
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				Expect(173, la); // "Option"
				currentState = 439;
				break;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (set[26, la.kind]) {
					currentState = 439;
					break;
				} else {
					goto case 15;
				}
			}
		}

		ApplyToken(la);
		if (la != null) t = la;
	}
	
	public void Advance()
	{
		Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,T,x,x, x,x,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,x, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,x, x,x,T,T, T,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,T,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,T,x, x,T,T,T, x,x,x,x, x,T,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, T,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,x,T, x,x,x,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,T, T,x,T,T, x,x,T,x, T,T,T,T, T,x,T,T, x,T,T,x, T,x,T,T, T,x,x,x, T,T,T,T, T,x,T,T, x,T,T,x, x,T,T,x, T,T,x,x, T,T,T,T, T,T,T,T, T,x,T,T, T,x,T,T, T,T,T,x, T,T,x,T, x,T,T,T, x,T,x,x, x,x,T,x, x,x,T,x, x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,x,T, x,x,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,x,T, x,T,x,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, T,x,T,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,T,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};

} // end Parser


}