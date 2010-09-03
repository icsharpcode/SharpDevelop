// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;



namespace ICSharpCode.VBNetBinding {



partial class VBIndentationStrategy {
	const int startOfExpression = 35;
	const int endOfStatementTerminatorAndBlock = 82;

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	List<Token> errors = new List<Token>();
	
	VBIndentationStrategy()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token la)
	{
		if (la.Kind != expectedKind) {
			Error(la);
			Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		switchlbl: switch (currentState) {
			case 0: {
				if (la == null) { currentState = 0; break; }
				if (set[0].Get(la.Kind)) {
					currentState = 0;
					break;
				} else {
					goto case 1;
				}
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (set[1].Get(la.Kind)) {
					stateStack.Push(2);
					goto case 3;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (set[2].Get(la.Kind)) {
					currentState = 2;
					break;
				} else {
					goto case 1;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.Kind == 160) {
					goto case 357;
				} else {
					if (set[3].Get(la.Kind)) {
						if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
							goto case 350;
						} else {
							if (la.Kind == 103) {
								currentState = 263;
								break;
							} else {
								if (la.Kind == 115) {
									goto case 261;
								} else {
									if (la.Kind == 142) {
										currentState = 5;
										break;
									} else {
										goto case 4;
									}
								}
							}
						}
					} else {
						goto case 4;
					}
				}
			}
			case 4: {
				Error(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 5: {
				if (la == null) { currentState = 5; break; }
				currentState = 6;
				break;
			}
			case 6: {
				if (la == null) { currentState = 6; break; }
				if (la.Kind == 37) {
					currentState = 354;
					break;
				} else {
					goto case 7;
				}
			}
			case 7: {
				stateStack.Push(8);
				goto case 13;
			}
			case 8: {
				Indent(la);
				goto case 9;
			}
			case 9: {
				if (la == null) { currentState = 9; break; }
				if (la.Kind == 140) {
					goto case 351;
				} else {
					goto case 10;
				}
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (set[4].Get(la.Kind)) {
					if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
						stateStack.Push(15);
						goto case 268;
					} else {
						if (la.Kind == 103) {
							stateStack.Push(15);
							goto case 262;
						} else {
							if (la.Kind == 115) {
								stateStack.Push(15);
								goto case 252;
							} else {
								if (la.Kind == 142) {
									stateStack.Push(15);
									goto case 251;
								} else {
									if (set[5].Get(la.Kind)) {
										stateStack.Push(15);
										goto case 16;
									} else {
										Error(la);
										goto case 15;
									}
								}
							}
						}
					}
				} else {
					Unindent(la);
					goto case 11;
				}
			}
			case 11: {
				if (la == null) { currentState = 11; break; }
				Expect(113, la); // "End"
				currentState = 12;
				break;
			}
			case 12: {
				if (la == null) { currentState = 12; break; }
				Expect(142, la); // "Interface"
				currentState = 13;
				break;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.Kind == 1 || la.Kind == 21) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (set[6].Get(la.Kind)) {
					currentState = 15;
					break;
				} else {
					goto case 10;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.Kind == 119) {
					goto case 249;
				} else {
					if (la.Kind == 186) {
						goto case 247;
					} else {
						if (la.Kind == 127 || la.Kind == 210) {
							goto case 17;
						} else {
							goto case 4;
						}
					}
				}
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				currentState = 18;
				break;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[7].Get(la.Kind)) {
					goto case 17;
				} else {
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.Kind == 37) {
					currentState = 234;
					break;
				} else {
					goto case 20;
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.Kind == 63) {
					currentState = 21;
					break;
				} else {
					goto case 13;
				}
			}
			case 21: {
				stateStack.Push(13);
				goto case 22;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (set[8].Get(la.Kind)) {
					goto case 233;
				} else {
					Error(la);
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (la.Kind == 37) {
					stateStack.Push(23);
					goto case 27;
				} else {
					goto case 24;
				}
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.Kind == 26) {
					currentState = 25;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 25: {
				stateStack.Push(26);
				goto case 14;
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				if (la.Kind == 37) {
					stateStack.Push(26);
					goto case 27;
				} else {
					goto case 24;
				}
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				Expect(37, la); // "("
				currentState = 28;
				break;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (la.Kind == 169) {
					goto case 230;
				} else {
					if (set[9].Get(la.Kind)) {
						goto case 30;
					} else {
						Error(la);
						goto case 29;
					}
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(29);
					goto case 31;
				} else {
					goto case 29;
				}
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				if (set[9].Get(la.Kind)) {
					goto case 227;
				} else {
					if (la.Kind == 22) {
						goto case 32;
					} else {
						goto case 4;
					}
				}
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				currentState = 33;
				break;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(34);
					goto case 35;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.Kind == 22) {
					goto case 32;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				goto case 36;
			}
			case 36: {
				stateStack.Push(37);
				goto case 38;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (set[10].Get(la.Kind)) {
					currentState = 36;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (set[11].Get(la.Kind)) {
					currentState = 38;
					break;
				} else {
					if (set[9].Get(la.Kind)) {
						stateStack.Push(56);
						goto case 65;
					} else {
						if (la.Kind == 220) {
							currentState = 54;
							break;
						} else {
							if (la.Kind == 162) {
								currentState = 43;
								break;
							} else {
								if (la.Kind == 35) {
									goto case 39;
								} else {
									goto case 4;
								}
							}
						}
					}
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				currentState = 40;
				break;
			}
			case 40: {
				stateStack.Push(41);
				goto case 35;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.Kind == 22) {
					goto case 39;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (set[8].Get(la.Kind)) {
					stateStack.Push(52);
					goto case 22;
				} else {
					goto case 44;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.Kind == 233) {
					currentState = 45;
					break;
				} else {
					goto case 4;
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(35, la); // "{"
				currentState = 46;
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				if (la.Kind == 147) {
					currentState = 47;
					break;
				} else {
					goto case 47;
				}
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				Expect(26, la); // "."
				currentState = 48;
				break;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				currentState = 49;
				break;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(20, la); // "="
				currentState = 50;
				break;
			}
			case 50: {
				stateStack.Push(51);
				goto case 35;
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (la.Kind == 22) {
					currentState = 46;
					break;
				} else {
					goto case 42;
				}
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.Kind == 126 || la.Kind == 233) {
					if (la.Kind == 126) {
						currentState = 53;
						break;
					} else {
						goto case 44;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (la.Kind == 35) {
					goto case 39;
				} else {
					if (set[12].Get(la.Kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 4;
					}
				}
			}
			case 54: {
				stateStack.Push(55);
				goto case 38;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				Expect(144, la); // "Is"
				currentState = 22;
				break;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (set[13].Get(la.Kind)) {
					stateStack.Push(56);
					goto case 57;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (la.Kind == 37) {
					currentState = 61;
					break;
				} else {
					if (set[14].Get(la.Kind)) {
						currentState = 58;
						break;
					} else {
						goto case 4;
					}
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.Kind == 10) {
					currentState = 59;
					break;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				currentState = 60;
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.Kind == 11) {
					goto case 14;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.Kind == 169) {
					goto case 62;
				} else {
					if (set[9].Get(la.Kind)) {
						goto case 30;
					} else {
						goto case 4;
					}
				}
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = 63;
				break;
			}
			case 63: {
				stateStack.Push(64);
				goto case 22;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (la.Kind == 22) {
					goto case 62;
				} else {
					goto case 29;
				}
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (set[15].Get(la.Kind)) {
					goto case 14;
				} else {
					if (set[16].Get(la.Kind)) {
						if (la.Kind == 17 || la.Kind == 18 || la.Kind == 19) {
							goto case 222;
						} else {
							if (la.Kind == 10) {
								stateStack.Push(212);
								goto case 214;
							} else {
								goto case 4;
							}
						}
					} else {
						if (la.Kind == 127 || la.Kind == 210) {
							if (la.Kind == 210) {
								currentState = 206;
								break;
							} else {
								if (la.Kind == 127) {
									currentState = 72;
									break;
								} else {
									goto case 4;
								}
							}
						} else {
							if (la.Kind == 135) {
								currentState = 66;
								break;
							} else {
								goto case 4;
							}
						}
					}
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(37, la); // "("
				currentState = 67;
				break;
			}
			case 67: {
				stateStack.Push(68);
				goto case 35;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				Expect(22, la); // ","
				currentState = 69;
				break;
			}
			case 69: {
				stateStack.Push(70);
				goto case 35;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.Kind == 22) {
					currentState = 71;
					break;
				} else {
					goto case 29;
				}
			}
			case 71: {
				stateStack.Push(29);
				goto case 35;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				Expect(37, la); // "("
				currentState = 73;
				break;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(74);
					goto case 202;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(38, la); // ")"
				currentState = 75;
				break;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (set[9].Get(la.Kind)) {
					goto case 35;
				} else {
					if (la.Kind == 1 || la.Kind == 21 || la.Kind == 63) {
						if (la.Kind == 63) {
							currentState = 201;
							break;
						} else {
							goto case 76;
						}
					} else {
						goto case 4;
					}
				}
			}
			case 76: {
				stateStack.Push(77);
				goto case 79;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(113, la); // "End"
				currentState = 78;
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 79: {
				Indent(la);
				goto case 80;
			}
			case 80: {
				stateStack.Push(81);
				goto case 13;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (set[9].Get(la.Kind)) {
					if (set[9].Get(la.Kind)) {
						if (set[9].Get(la.Kind)) {
							stateStack.Push(80);
							goto case 86;
						} else {
							goto case 80;
						}
					} else {
						if (la.Kind == 113) {
							currentState = 84;
							break;
						} else {
							goto case 83;
						}
					}
				} else {
					goto case 82;
				}
			}
			case 82: {
				Unindent(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 83: {
				Error(la);
				goto case 81;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.Kind == 1 || la.Kind == 21) {
					goto case 85;
				} else {
					if (set[17].Get(la.Kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 83;
					}
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				currentState = 81;
				break;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				if (la.Kind == 88 || la.Kind == 105 || la.Kind == 204) {
					currentState = 184;
					break;
				} else {
					if (la.Kind == 211 || la.Kind == 233) {
						currentState = 180;
						break;
					} else {
						if (la.Kind == 56 || la.Kind == 193) {
							currentState = 178;
							break;
						} else {
							if (la.Kind == 189) {
								currentState = 176;
								break;
							} else {
								if (la.Kind == 135) {
									currentState = 159;
									break;
								} else {
									if (la.Kind == 197) {
										currentState = 147;
										break;
									} else {
										if (la.Kind == 231) {
											currentState = 143;
											break;
										} else {
											if (la.Kind == 108) {
												currentState = 137;
												break;
											} else {
												if (la.Kind == 124) {
													currentState = 116;
													break;
												} else {
													if (la.Kind == 118 || la.Kind == 171 || la.Kind == 194) {
														if (la.Kind == 118 || la.Kind == 171) {
															if (la.Kind == 171) {
																currentState = 112;
																break;
															} else {
																goto case 112;
															}
														} else {
															if (la.Kind == 194) {
																currentState = 111;
																break;
															} else {
																goto case 4;
															}
														}
													} else {
														if (la.Kind == 215) {
															goto case 96;
														} else {
															if (la.Kind == 218) {
																currentState = 101;
																break;
															} else {
																if (set[18].Get(la.Kind)) {
																	if (la.Kind == 132) {
																		currentState = 100;
																		break;
																	} else {
																		if (la.Kind == 120) {
																			currentState = 99;
																			break;
																		} else {
																			if (la.Kind == 89) {
																				currentState = 98;
																				break;
																			} else {
																				if (la.Kind == 206) {
																					goto case 14;
																				} else {
																					if (la.Kind == 195) {
																						goto case 96;
																					} else {
																						goto case 4;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.Kind == 191) {
																		currentState = 95;
																		break;
																	} else {
																		if (la.Kind == 117) {
																			goto case 92;
																		} else {
																			if (la.Kind == 226) {
																				currentState = 88;
																				break;
																			} else {
																				if (set[9].Get(la.Kind)) {
																					if (la.Kind == 73) {
																						goto case 87;
																					} else {
																						goto case 35;
																					}
																				} else {
																					goto case 4;
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
			case 87: {
				if (la == null) { currentState = 87; break; }
				currentState = 35;
				break;
			}
			case 88: {
				stateStack.Push(89);
				goto case 35;
			}
			case 89: {
				stateStack.Push(90);
				goto case 79;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				Expect(113, la); // "End"
				currentState = 91;
				break;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				currentState = 93;
				break;
			}
			case 93: {
				stateStack.Push(94);
				goto case 35;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.Kind == 22) {
					goto case 92;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				if (la.Kind == 184) {
					goto case 87;
				} else {
					goto case 35;
				}
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				currentState = 97;
				break;
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				if (set[9].Get(la.Kind)) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				if (la.Kind == 108 || la.Kind == 124 || la.Kind == 231) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				if (set[19].Get(la.Kind)) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.Kind == 2 || la.Kind == 5) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 101: {
				stateStack.Push(102);
				goto case 79;
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				if (la.Kind == 75) {
					currentState = 106;
					break;
				} else {
					if (la.Kind == 123) {
						currentState = 105;
						break;
					} else {
						goto case 103;
					}
				}
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				Expect(113, la); // "End"
				currentState = 104;
				break;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 105: {
				stateStack.Push(103);
				goto case 79;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (set[20].Get(la.Kind)) {
					currentState = 109;
					break;
				} else {
					goto case 107;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (la.Kind == 229) {
					currentState = 108;
					break;
				} else {
					goto case 101;
				}
			}
			case 108: {
				stateStack.Push(101);
				goto case 35;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (la.Kind == 63) {
					currentState = 110;
					break;
				} else {
					goto case 107;
				}
			}
			case 110: {
				stateStack.Push(107);
				goto case 22;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				if (la.Kind == 2 || la.Kind == 5 || la.Kind == 163) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				Expect(118, la); // "Error"
				currentState = 113;
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (set[9].Get(la.Kind)) {
					goto case 35;
				} else {
					if (la.Kind == 132) {
						currentState = 115;
						break;
					} else {
						if (la.Kind == 194) {
							currentState = 114;
							break;
						} else {
							goto case 4;
						}
					}
				}
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.Kind == 2 || la.Kind == 5) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(128);
					goto case 125;
				} else {
					if (la.Kind == 110) {
						currentState = 117;
						break;
					} else {
						goto case 4;
					}
				}
			}
			case 117: {
				stateStack.Push(118);
				goto case 125;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				Expect(138, la); // "In"
				currentState = 119;
				break;
			}
			case 119: {
				stateStack.Push(120);
				goto case 35;
			}
			case 120: {
				stateStack.Push(121);
				goto case 79;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				Expect(163, la); // "Next"
				currentState = 122;
				break;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				if (set[9].Get(la.Kind)) {
					goto case 123;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 123: {
				stateStack.Push(124);
				goto case 35;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				if (la.Kind == 22) {
					currentState = 123;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 125: {
				stateStack.Push(126);
				goto case 65;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.Kind == 33) {
					currentState = 127;
					break;
				} else {
					goto case 127;
				}
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (set[13].Get(la.Kind)) {
					stateStack.Push(127);
					goto case 57;
				} else {
					if (la.Kind == 63) {
						currentState = 22;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(20, la); // "="
				currentState = 129;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 35;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.Kind == 205) {
					currentState = 136;
					break;
				} else {
					goto case 131;
				}
			}
			case 131: {
				stateStack.Push(132);
				goto case 79;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				Expect(163, la); // "Next"
				currentState = 133;
				break;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (set[9].Get(la.Kind)) {
					goto case 134;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 134: {
				stateStack.Push(135);
				goto case 35;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.Kind == 22) {
					currentState = 134;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 136: {
				stateStack.Push(131);
				goto case 35;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (la.Kind == 224 || la.Kind == 231) {
					currentState = 140;
					break;
				} else {
					if (la.Kind == 1 || la.Kind == 21) {
						stateStack.Push(138);
						goto case 79;
					} else {
						goto case 4;
					}
				}
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				Expect(152, la); // "Loop"
				currentState = 139;
				break;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (la.Kind == 224 || la.Kind == 231) {
					goto case 87;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				stateStack.Push(141);
				goto case 35;
			}
			case 141: {
				stateStack.Push(142);
				goto case 79;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 35;
			}
			case 144: {
				stateStack.Push(145);
				goto case 79;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(113, la); // "End"
				currentState = 146;
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (la.Kind == 74) {
					currentState = 148;
					break;
				} else {
					goto case 148;
				}
			}
			case 148: {
				stateStack.Push(149);
				goto case 35;
			}
			case 149: {
				stateStack.Push(150);
				goto case 13;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.Kind == 74) {
					currentState = 152;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 151;
					break;
				}
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.Kind == 111) {
					currentState = 153;
					break;
				} else {
					if (set[9].Get(la.Kind)) {
						goto case 154;
					} else {
						Error(la);
						goto case 153;
					}
				}
			}
			case 153: {
				stateStack.Push(150);
				goto case 79;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (set[21].Get(la.Kind)) {
					if (la.Kind == 144) {
						currentState = 156;
						break;
					} else {
						goto case 156;
					}
				} else {
					if (set[9].Get(la.Kind)) {
						stateStack.Push(155);
						goto case 35;
					} else {
						Error(la);
						goto case 155;
					}
				}
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (la.Kind == 22) {
					currentState = 154;
					break;
				} else {
					goto case 153;
				}
			}
			case 156: {
				stateStack.Push(157);
				goto case 158;
			}
			case 157: {
				stateStack.Push(155);
				goto case 38;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (set[22].Get(la.Kind)) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 159: {
				stateStack.Push(160);
				goto case 35;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.Kind == 214) {
					currentState = 169;
					break;
				} else {
					goto case 161;
				}
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.Kind == 1 || la.Kind == 21) {
					goto case 162;
				} else {
					goto case 4;
				}
			}
			case 162: {
				stateStack.Push(163);
				goto case 79;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.Kind == 111 || la.Kind == 112) {
					if (la.Kind == 111) {
						currentState = 168;
						break;
					} else {
						if (la.Kind == 112) {
							goto case 165;
						} else {
							Error(la);
							goto case 162;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 164;
					break;
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(167);
				goto case 35;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (la.Kind == 214) {
					currentState = 162;
					break;
				} else {
					goto case 162;
				}
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.Kind == 135) {
					goto case 165;
				} else {
					goto case 162;
				}
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (set[9].Get(la.Kind)) {
					goto case 170;
				} else {
					goto case 161;
				}
			}
			case 170: {
				stateStack.Push(171);
				goto case 86;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.Kind == 21) {
					currentState = 175;
					break;
				} else {
					if (la.Kind == 111) {
						goto case 172;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				currentState = 173;
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(174);
					goto case 86;
				} else {
					goto case 174;
				}
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.Kind == 21) {
					goto case 172;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (set[9].Get(la.Kind)) {
					goto case 170;
				} else {
					goto case 171;
				}
			}
			case 176: {
				stateStack.Push(177);
				goto case 14;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (la.Kind == 37) {
					currentState = 30;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 178: {
				stateStack.Push(179);
				goto case 35;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				Expect(22, la); // ","
				currentState = 35;
				break;
			}
			case 180: {
				stateStack.Push(181);
				goto case 35;
			}
			case 181: {
				stateStack.Push(182);
				goto case 79;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				Expect(113, la); // "End"
				currentState = 183;
				break;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (la.Kind == 211 || la.Kind == 233) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				currentState = 185;
				break;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.Kind == 33) {
					currentState = 186;
					break;
				} else {
					goto case 186;
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				if (la.Kind == 37) {
					goto case 199;
				} else {
					goto case 187;
				}
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.Kind == 22) {
					currentState = 193;
					break;
				} else {
					goto case 188;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.Kind == 63) {
					currentState = 190;
					break;
				} else {
					goto case 189;
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.Kind == 20) {
					goto case 87;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.Kind == 162) {
					stateStack.Push(189);
					goto case 192;
				} else {
					if (set[8].Get(la.Kind)) {
						goto case 191;
					} else {
						Error(la);
						goto case 189;
					}
				}
			}
			case 191: {
				stateStack.Push(189);
				goto case 22;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				Expect(162, la); // "New"
				currentState = 43;
				break;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				currentState = 194;
				break;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.Kind == 33) {
					currentState = 195;
					break;
				} else {
					goto case 195;
				}
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (la.Kind == 37) {
					goto case 196;
				} else {
					goto case 187;
				}
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				currentState = 197;
				break;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.Kind == 22) {
					goto case 196;
				} else {
					goto case 198;
				}
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(38, la); // ")"
				currentState = 187;
				break;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				currentState = 200;
				break;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.Kind == 22) {
					goto case 199;
				} else {
					goto case 198;
				}
			}
			case 201: {
				stateStack.Push(76);
				goto case 22;
			}
			case 202: {
				stateStack.Push(203);
				goto case 204;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.Kind == 22) {
					currentState = 202;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				currentState = 205;
				break;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[23].Get(la.Kind)) {
					goto case 204;
				} else {
					if (la.Kind == 63) {
						currentState = 191;
						break;
					} else {
						goto case 189;
					}
				}
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				Expect(37, la); // "("
				currentState = 207;
				break;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(208);
					goto case 202;
				} else {
					goto case 208;
				}
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				Expect(38, la); // ")"
				currentState = 209;
				break;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (set[9].Get(la.Kind)) {
					goto case 86;
				} else {
					if (la.Kind == 1 || la.Kind == 21) {
						stateStack.Push(210);
						goto case 79;
					} else {
						goto case 4;
					}
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				Expect(113, la); // "End"
				currentState = 211;
				break;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.Kind == 17) {
					currentState = 213;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.Kind == 16) {
					currentState = 212;
					break;
				} else {
					goto case 212;
				}
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 215;
				break;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (set[24].Get(la.Kind)) {
					if (set[25].Get(la.Kind)) {
						currentState = 215;
						break;
					} else {
						if (la.Kind == 12) {
							stateStack.Push(215);
							goto case 219;
						} else {
							Error(la);
							goto case 215;
						}
					}
				} else {
					if (la.Kind == 14) {
						goto case 14;
					} else {
						if (la.Kind == 11) {
							goto case 216;
						} else {
							goto case 4;
						}
					}
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				currentState = 217;
				break;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				if (set[26].Get(la.Kind)) {
					if (set[27].Get(la.Kind)) {
						goto case 216;
					} else {
						if (la.Kind == 12) {
							stateStack.Push(217);
							goto case 219;
						} else {
							if (la.Kind == 10) {
								stateStack.Push(217);
								goto case 214;
							} else {
								Error(la);
								goto case 217;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 218;
					break;
				}
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[28].Get(la.Kind)) {
					if (set[29].Get(la.Kind)) {
						currentState = 218;
						break;
					} else {
						if (la.Kind == 12) {
							stateStack.Push(218);
							goto case 219;
						} else {
							Error(la);
							goto case 218;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 220;
				break;
			}
			case 220: {
				stateStack.Push(221);
				goto case 35;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				currentState = 223;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.Kind == 16) {
					currentState = 224;
					break;
				} else {
					goto case 224;
				}
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.Kind == 17 || la.Kind == 19) {
					goto case 222;
				} else {
					if (la.Kind == 10) {
						stateStack.Push(225);
						goto case 214;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.Kind == 17) {
					currentState = 226;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.Kind == 16) {
					currentState = 225;
					break;
				} else {
					goto case 225;
				}
			}
			case 227: {
				stateStack.Push(228);
				goto case 35;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (la.Kind == 22) {
					currentState = 229;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (set[9].Get(la.Kind)) {
					goto case 227;
				} else {
					goto case 228;
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				currentState = 231;
				break;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (set[8].Get(la.Kind)) {
					stateStack.Push(232);
					goto case 22;
				} else {
					goto case 232;
				}
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (la.Kind == 22) {
					goto case 230;
				} else {
					goto case 29;
				}
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				currentState = 23;
				break;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (set[9].Get(la.Kind)) {
					if (la.Kind == 169) {
						currentState = 236;
						break;
					} else {
						if (set[9].Get(la.Kind)) {
							stateStack.Push(235);
							goto case 202;
						} else {
							Error(la);
							goto case 235;
						}
					}
				} else {
					goto case 235;
				}
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(38, la); // ")"
				currentState = 19;
				break;
			}
			case 236: {
				stateStack.Push(235);
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.Kind == 138 || la.Kind == 178) {
					currentState = 238;
					break;
				} else {
					goto case 238;
				}
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				currentState = 239;
				break;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (la.Kind == 63) {
					currentState = 241;
					break;
				} else {
					goto case 240;
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.Kind == 22) {
					currentState = 237;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 241: {
				stateStack.Push(240);
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[30].Get(la.Kind)) {
					goto case 246;
				} else {
					if (la.Kind == 35) {
						goto case 243;
					} else {
						goto case 4;
					}
				}
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				currentState = 244;
				break;
			}
			case 244: {
				stateStack.Push(245);
				goto case 246;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (la.Kind == 22) {
					goto case 243;
				} else {
					goto case 42;
				}
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (set[8].Get(la.Kind)) {
					goto case 233;
				} else {
					if (la.Kind == 84 || la.Kind == 162 || la.Kind == 209) {
						goto case 14;
					} else {
						goto case 4;
					}
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				currentState = 248;
				break;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (set[17].Get(la.Kind)) {
					goto case 247;
				} else {
					goto case 13;
				}
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				currentState = 250;
				break;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (set[17].Get(la.Kind)) {
					goto case 249;
				} else {
					goto case 13;
				}
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(142, la); // "Interface"
				currentState = 5;
				break;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				Expect(115, la); // "Enum"
				currentState = 253;
				break;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (set[17].Get(la.Kind)) {
					goto case 261;
				} else {
					stateStack.Push(254);
					goto case 13;
				}
			}
			case 254: {
				Indent(la);
				goto case 255;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (set[31].Get(la.Kind)) {
					currentState = 258;
					break;
				} else {
					Unindent(la);
					goto case 256;
				}
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				Expect(113, la); // "End"
				currentState = 257;
				break;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				Expect(115, la); // "Enum"
				currentState = 13;
				break;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.Kind == 20) {
					currentState = 260;
					break;
				} else {
					goto case 259;
				}
			}
			case 259: {
				stateStack.Push(255);
				goto case 13;
			}
			case 260: {
				stateStack.Push(259);
				goto case 35;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				currentState = 253;
				break;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				Expect(103, la); // "Delegate"
				currentState = 263;
				break;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.Kind == 127 || la.Kind == 210) {
					currentState = 264;
					break;
				} else {
					Error(la);
					goto case 264;
				}
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				currentState = 265;
				break;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (la.Kind == 37) {
					currentState = 266;
					break;
				} else {
					goto case 20;
				}
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(267);
					goto case 202;
				} else {
					goto case 267;
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				Expect(38, la); // ")"
				currentState = 20;
				break;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
					goto case 350;
				} else {
					Error(la);
					goto case 269;
				}
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				currentState = 270;
				break;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.Kind == 37) {
					currentState = 347;
					break;
				} else {
					goto case 271;
				}
			}
			case 271: {
				stateStack.Push(272);
				goto case 13;
			}
			case 272: {
				Indent(la);
				goto case 273;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (set[9].Get(la.Kind)) {
					if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
						stateStack.Push(273);
						goto case 268;
					} else {
						if (la.Kind == 103) {
							stateStack.Push(273);
							goto case 262;
						} else {
							if (la.Kind == 115) {
								stateStack.Push(273);
								goto case 252;
							} else {
								if (la.Kind == 142) {
									stateStack.Push(273);
									goto case 251;
								} else {
									if (set[9].Get(la.Kind)) {
										stateStack.Push(273);
										goto case 278;
									} else {
										Error(la);
										goto case 273;
									}
								}
							}
						}
					}
				} else {
					Unindent(la);
					goto case 274;
				}
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				Expect(113, la); // "End"
				currentState = 275;
				break;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
					goto case 277;
				} else {
					goto case 276;
				}
			}
			case 276: {
				Error(la);
				goto case 13;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				currentState = 13;
				break;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (set[9].Get(la.Kind)) {
					goto case 343;
				} else {
					if (la.Kind == 127 || la.Kind == 210) {
						currentState = 334;
						break;
					} else {
						if (la.Kind == 101) {
							currentState = 327;
							break;
						} else {
							if (la.Kind == 119) {
								currentState = 318;
								break;
							} else {
								if (la.Kind == 98) {
									currentState = 307;
									break;
								} else {
									if (la.Kind == 186) {
										goto case 283;
									} else {
										if (la.Kind == 172) {
											goto case 279;
										} else {
											goto case 4;
										}
									}
								}
							}
						}
					}
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				currentState = 280;
				break;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (set[17].Get(la.Kind)) {
					goto case 279;
				} else {
					stateStack.Push(281);
					goto case 79;
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				Expect(113, la); // "End"
				currentState = 282;
				break;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(172, la); // "Operator"
				currentState = 13;
				break;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				currentState = 284;
				break;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (set[32].Get(la.Kind)) {
					goto case 283;
				} else {
					if (la.Kind == 20) {
						currentState = 306;
						break;
					} else {
						goto case 285;
					}
				}
			}
			case 285: {
				stateStack.Push(286);
				goto case 13;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (set[33].Get(la.Kind)) {
					currentState = 286;
					break;
				} else {
					if (la.Kind == 128 || la.Kind == 198) {
						Indent(la);
						goto case 287;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 288;
					break;
				} else {
					Error(la);
					goto case 288;
				}
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.Kind == 37) {
					currentState = 304;
					break;
				} else {
					goto case 289;
				}
			}
			case 289: {
				stateStack.Push(290);
				goto case 79;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(113, la); // "End"
				currentState = 291;
				break;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 292;
					break;
				} else {
					Error(la);
					goto case 292;
				}
			}
			case 292: {
				stateStack.Push(293);
				goto case 13;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 297;
					break;
				} else {
					goto case 294;
				}
			}
			case 294: {
				Unindent(la);
				goto case 295;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				Expect(113, la); // "End"
				currentState = 296;
				break;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(186, la); // "Property"
				currentState = 13;
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.Kind == 37) {
					currentState = 302;
					break;
				} else {
					goto case 298;
				}
			}
			case 298: {
				stateStack.Push(299);
				goto case 79;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				Expect(113, la); // "End"
				currentState = 300;
				break;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 301;
					break;
				} else {
					Error(la);
					goto case 301;
				}
			}
			case 301: {
				stateStack.Push(294);
				goto case 13;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(303);
					goto case 202;
				} else {
					goto case 303;
				}
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				Expect(38, la); // ")"
				currentState = 298;
				break;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(305);
					goto case 202;
				} else {
					goto case 305;
				}
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(38, la); // ")"
				currentState = 289;
				break;
			}
			case 306: {
				stateStack.Push(285);
				goto case 35;
			}
			case 307: {
				stateStack.Push(308);
				goto case 317;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.Kind == 56 || la.Kind == 189 || la.Kind == 193) {
					currentState = 310;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 309;
					break;
				}
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				Expect(119, la); // "Event"
				currentState = 13;
				break;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(37, la); // "("
				currentState = 311;
				break;
			}
			case 311: {
				stateStack.Push(312);
				goto case 202;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				Expect(38, la); // ")"
				currentState = 313;
				break;
			}
			case 313: {
				stateStack.Push(314);
				goto case 79;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				Expect(113, la); // "End"
				currentState = 315;
				break;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.Kind == 56 || la.Kind == 189 || la.Kind == 193) {
					currentState = 316;
					break;
				} else {
					Error(la);
					goto case 316;
				}
			}
			case 316: {
				stateStack.Push(308);
				goto case 13;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				Expect(119, la); // "Event"
				currentState = 318;
				break;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				currentState = 319;
				break;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.Kind == 63) {
					currentState = 326;
					break;
				} else {
					if (set[34].Get(la.Kind)) {
						if (la.Kind == 37) {
							currentState = 324;
							break;
						} else {
							goto case 320;
						}
					} else {
						Error(la);
						goto case 320;
					}
				}
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.Kind == 136) {
					goto case 321;
				} else {
					goto case 13;
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				currentState = 322;
				break;
			}
			case 322: {
				stateStack.Push(323);
				goto case 22;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.Kind == 22) {
					goto case 321;
				} else {
					goto case 13;
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(325);
					goto case 202;
				} else {
					goto case 325;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				Expect(38, la); // ")"
				currentState = 320;
				break;
			}
			case 326: {
				stateStack.Push(320);
				goto case 22;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.Kind == 62 || la.Kind == 66 || la.Kind == 223) {
					currentState = 328;
					break;
				} else {
					goto case 328;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.Kind == 127 || la.Kind == 210) {
					currentState = 329;
					break;
				} else {
					Error(la);
					goto case 329;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				currentState = 330;
				break;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				Expect(149, la); // "Lib"
				currentState = 331;
				break;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(3, la); // LiteralString
				currentState = 332;
				break;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.Kind == 59) {
					currentState = 333;
					break;
				} else {
					goto case 265;
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(3, la); // LiteralString
				currentState = 265;
				break;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				currentState = 335;
				break;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.Kind == 37) {
					currentState = 340;
					break;
				} else {
					if (la.Kind == 63) {
						currentState = 339;
						break;
					} else {
						goto case 336;
					}
				}
			}
			case 336: {
				stateStack.Push(337);
				goto case 79;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				Expect(113, la); // "End"
				currentState = 338;
				break;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.Kind == 127 || la.Kind == 210) {
					goto case 277;
				} else {
					goto case 276;
				}
			}
			case 339: {
				stateStack.Push(336);
				goto case 22;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (set[9].Get(la.Kind)) {
					if (la.Kind == 169) {
						currentState = 342;
						break;
					} else {
						if (set[9].Get(la.Kind)) {
							stateStack.Push(341);
							goto case 202;
						} else {
							Error(la);
							goto case 341;
						}
					}
				} else {
					goto case 341;
				}
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				Expect(38, la); // ")"
				currentState = 335;
				break;
			}
			case 342: {
				stateStack.Push(341);
				goto case 237;
			}
			case 343: {
				stateStack.Push(344);
				goto case 345;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.Kind == 22) {
					currentState = 343;
					break;
				} else {
					goto case 13;
				}
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (la.Kind == 88) {
					currentState = 346;
					break;
				} else {
					goto case 346;
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				currentState = 188;
				break;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				Expect(169, la); // "Of"
				currentState = 348;
				break;
			}
			case 348: {
				stateStack.Push(349);
				goto case 237;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				Expect(38, la); // ")"
				currentState = 271;
				break;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				currentState = 269;
				break;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				currentState = 352;
				break;
			}
			case 352: {
				stateStack.Push(353);
				goto case 22;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.Kind == 22) {
					goto case 351;
				} else {
					stateStack.Push(10);
					goto case 13;
				}
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				Expect(169, la); // "Of"
				currentState = 355;
				break;
			}
			case 355: {
				stateStack.Push(356);
				goto case 237;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				Expect(38, la); // ")"
				currentState = 7;
				break;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				currentState = 358;
				break;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (set[17].Get(la.Kind)) {
					goto case 357;
				} else {
					stateStack.Push(359);
					goto case 13;
				}
			}
			case 359: {
				Indent(la);
				goto case 360;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (set[1].Get(la.Kind)) {
					stateStack.Push(363);
					goto case 3;
				} else {
					Unindent(la);
					goto case 361;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(113, la); // "End"
				currentState = 362;
				break;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(160, la); // "Namespace"
				currentState = 13;
				break;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (set[35].Get(la.Kind)) {
					currentState = 363;
					break;
				} else {
					goto case 360;
				}
			}
		}

		if (la != null)
			t = la;
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {-2, -1, -1048577, -524417, -134234113, -2, -131073, -1}),
		new BitArray(new int[] {0, 0, 1048576, 524416, 134234112, 1, 131072, 0}),
		new BitArray(new int[] {-2, -1, -1048577, -524417, -134234113, -2, -131073, -1}),
		new BitArray(new int[] {0, 0, 1048576, 524416, 134234112, 0, 131072, 0}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1048577, 2138439551, -134234113, -67108865, -393217, -1}),
		new BitArray(new int[] {-2097156, 2147483615, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 2, 262288, 8216, 8396804, 256, 1610679824, 2}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-918530, -1, -1, 2147483647, -129, -1, -262145, -1}),
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -33}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 65536, 0, 0, 0}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 2, 1310864, 8216, 8396804, 260, 1610810896, 2}),
		new BitArray(new int[] {-2, -1, -1, -131073, -1, -1, -1, -1}),
		new BitArray(new int[] {-3145732, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2, -1, -1, -1, -2, -1, -65, -1}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {-2, -1, -1048577, -655489, -134234113, -2, -131073, -1})

	};

} // end Parser


}
