using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 37;
	const int endOfStatementTerminatorAndBlock = 213;
	static BitArray GetExpectedSet(int state)
	{
		switch (state) {
			case 0:
			case 1:
				return set[0];
			case 2:
				return set[1];
			case 3:
			case 4:
				return set[2];
			case 5:
				return set[3];
			case 6:
			case 51:
			case 214:
			case 483:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 200:
			case 460:
			case 474:
			case 482:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 10:
			case 11:
			case 107:
			case 164:
			case 165:
			case 215:
			case 366:
			case 367:
			case 381:
			case 382:
			case 383:
			case 406:
			case 407:
			case 408:
			case 409:
			case 422:
			case 423:
			case 475:
			case 476:
			case 499:
			case 500:
			case 520:
			case 521:
				return set[6];
			case 12:
			case 13:
			case 466:
			case 477:
			case 478:
				return set[7];
			case 14:
			case 428:
			case 467:
			case 479:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 15:
			case 16:
			case 20:
			case 206:
			case 209:
			case 210:
			case 220:
			case 234:
			case 238:
			case 258:
			case 274:
			case 285:
			case 288:
			case 294:
			case 299:
			case 308:
			case 309:
			case 322:
			case 330:
			case 354:
			case 416:
			case 429:
			case 433:
			case 442:
			case 445:
			case 470:
			case 480:
			case 486:
			case 527:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 17:
			case 319:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 18:
			case 151:
			case 168:
			case 244:
			case 268:
			case 339:
			case 352:
			case 362:
			case 455:
			case 468:
			case 484:
			case 495:
			case 509:
			case 517:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					return a;
				}
			case 19:
			case 21:
			case 111:
			case 120:
			case 136:
			case 152:
			case 169:
			case 245:
			case 269:
			case 340:
			case 342:
			case 344:
			case 353:
			case 363:
			case 392:
			case 451:
			case 456:
			case 469:
			case 485:
			case 496:
			case 533:
				return set[8];
			case 22:
			case 25:
				return set[9];
			case 23:
				return set[10];
			case 24:
			case 57:
			case 61:
			case 116:
			case 325:
			case 395:
				return set[11];
			case 26:
			case 126:
			case 133:
			case 137:
			case 201:
			case 370:
			case 388:
			case 391:
			case 424:
			case 425:
			case 439:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 27:
			case 28:
			case 118:
			case 119:
				return set[12];
			case 29:
			case 203:
			case 350:
			case 372:
			case 390:
			case 405:
			case 427:
			case 441:
			case 454:
			case 472:
			case 488:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 30:
			case 31:
			case 34:
			case 35:
			case 400:
			case 401:
				return set[13];
			case 32:
				return set[14];
			case 33:
			case 128:
			case 135:
			case 328:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 36:
			case 121:
			case 130:
			case 349:
			case 351:
			case 356:
			case 399:
			case 403:
			case 505:
			case 511:
			case 519:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 37:
			case 38:
			case 40:
			case 41:
			case 42:
			case 45:
			case 59:
			case 109:
			case 127:
			case 129:
			case 131:
			case 134:
			case 143:
			case 145:
			case 187:
			case 219:
			case 223:
			case 225:
			case 226:
			case 241:
			case 257:
			case 262:
			case 272:
			case 278:
			case 280:
			case 284:
			case 287:
			case 293:
			case 304:
			case 306:
			case 312:
			case 327:
			case 329:
			case 385:
			case 397:
			case 398:
			case 494:
				return set[15];
			case 39:
			case 43:
			case 52:
				return set[16];
			case 44:
			case 53:
			case 54:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 46:
			case 60:
			case 514:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 47:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 48:
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 49:
				return set[17];
			case 50:
			case 62:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 55:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 56:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 58:
			case 167:
			case 170:
			case 171:
			case 271:
			case 529:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 63:
			case 290:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 64:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 65:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 67:
			case 237:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 70:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 71:
			case 376:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 73:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 75:
			case 296:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 80:
			case 252:
			case 259:
			case 275:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 83:
			case 174:
			case 179:
			case 181:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 84:
			case 176:
			case 180:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 87:
			case 208:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 88:
			case 199:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 91:
			case 144:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 95:
			case 434:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 98:
			case 157:
			case 186:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 100:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 104:
			case 198:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 106:
				return set[18];
			case 108:
				return set[19];
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 112:
				return set[20];
			case 113:
				return set[21];
			case 114:
			case 115:
			case 393:
			case 394:
				return set[22];
			case 117:
				return set[23];
			case 122:
			case 123:
			case 255:
			case 264:
				return set[24];
			case 124:
				return set[25];
			case 125:
			case 311:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 132:
				return set[26];
			case 138:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 139:
			case 140:
				return set[27];
			case 141:
			case 147:
			case 154:
			case 192:
			case 196:
			case 233:
			case 333:
			case 345:
			case 389:
			case 447:
			case 461:
			case 528:
				return set[28];
			case 142:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 146:
			case 161:
			case 178:
			case 183:
			case 189:
			case 191:
			case 195:
			case 197:
				return set[29];
			case 148:
			case 149:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 150:
			case 153:
			case 256:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 155:
			case 156:
			case 158:
			case 160:
			case 162:
			case 163:
			case 172:
			case 177:
			case 182:
			case 190:
			case 194:
				return set[30];
			case 159:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 166:
				return set[31];
			case 173:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 175:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 184:
			case 185:
				return set[32];
			case 188:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 193:
				return set[33];
			case 202:
			case 371:
			case 404:
			case 453:
			case 471:
			case 487:
				return set[34];
			case 204:
			case 205:
				return set[35];
			case 207:
			case 221:
			case 236:
			case 289:
			case 331:
			case 375:
			case 430:
			case 443:
			case 481:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 211:
			case 212:
				return set[36];
			case 213:
				return set[37];
			case 216:
				return set[38];
			case 217:
			case 218:
			case 317:
				return set[39];
			case 222:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 224:
			case 263:
			case 279:
				return set[40];
			case 227:
			case 228:
			case 260:
			case 261:
			case 276:
			case 277:
				return set[41];
			case 229:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 230:
				return set[42];
			case 231:
			case 248:
				return set[43];
			case 232:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 235:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 239:
				return set[44];
			case 240:
			case 246:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 242:
			case 243:
				return set[45];
			case 247:
				return set[46];
			case 249:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 250:
			case 251:
				return set[47];
			case 253:
			case 254:
				return set[48];
			case 265:
			case 266:
				return set[49];
			case 267:
				return set[50];
			case 270:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 273:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 281:
				return set[51];
			case 282:
			case 286:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 283:
				return set[52];
			case 291:
			case 292:
				return set[53];
			case 295:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 297:
			case 298:
				return set[54];
			case 300:
			case 301:
				return set[55];
			case 302:
			case 452:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 303:
			case 305:
				return set[56];
			case 307:
			case 313:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 314:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 315:
			case 316:
			case 320:
			case 321:
			case 373:
			case 374:
				return set[57];
			case 318:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 323:
			case 324:
				return set[58];
			case 326:
				return set[59];
			case 332:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 334:
			case 335:
			case 346:
			case 347:
				return set[60];
			case 336:
			case 348:
				return set[61];
			case 337:
				return set[62];
			case 338:
			case 343:
				return set[63];
			case 341:
				return set[64];
			case 355:
			case 357:
			case 426:
			case 440:
				return set[65];
			case 358:
				return set[66];
			case 359:
			case 360:
				return set[67];
			case 361:
			case 364:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 365:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 368:
			case 369:
				return set[68];
			case 377:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 378:
				return set[69];
			case 379:
				return set[70];
			case 380:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 384:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 386:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 387:
				return set[71];
			case 396:
				return set[72];
			case 402:
				return set[73];
			case 410:
				return set[74];
			case 411:
				return set[75];
			case 412:
				return set[76];
			case 413:
			case 414:
			case 420:
				return set[77];
			case 415:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 417:
				return set[78];
			case 418:
				return set[79];
			case 419:
				return set[80];
			case 421:
			case 431:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 432:
				return set[81];
			case 435:
			case 437:
			case 446:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 436:
				return set[82];
			case 438:
				return set[83];
			case 444:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 448:
			case 449:
				return set[84];
			case 450:
			case 457:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 458:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 459:
				return set[85];
			case 462:
			case 463:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 464:
			case 473:
			case 530:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 465:
				return set[86];
			case 489:
				return set[87];
			case 490:
			case 498:
				return set[88];
			case 491:
			case 492:
				return set[89];
			case 493:
			case 497:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 501:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 502:
			case 506:
				return set[90];
			case 503:
			case 507:
			case 516:
				return set[91];
			case 504:
			case 508:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 510:
			case 512:
			case 518:
				return set[92];
			case 513:
			case 515:
				return set[93];
			case 522:
				return set[94];
			case 523:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 524:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 525:
			case 526:
				return set[95];
			case 531:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 532:
				return set[96];
			case 534:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 535:
				return set[97];
			case 536:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 537:
				return set[98];
			default: throw new InvalidOperationException();
		}
	}

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool wasQualifierTokenAtStart = false;
	bool nextTokenIsPotentialStartOfExpression = false;
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
			//Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		//Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfExpression = false;
		readXmlIdentifier = false;
		nextTokenIsStartOfImportsOrAccessExpression = false;
		wasQualifierTokenAtStart = false;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 534;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					PushContext(Context.Importable, la, t);
					goto case 524;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 365;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[3].Get(la.kind)) {
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
					currentState = 520;
					break;
				} else {
					if (set[4].Get(la.kind)) {
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
					goto case 365;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[99].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						currentState = 406;
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
				if (la.kind == 210) {
					currentState = 10;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 10;
						break;
					} else {
						Error(la);
						goto case 10;
					}
				}
			}
			case 10: {
				PushContext(Context.IdentifierExpected, la, t);
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
					currentState = 404;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 18;
				} else {
					goto case 15;
				}
			}
			case 15: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 16;
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.kind == 1) {
					goto case 17;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				Expect(63, la); // "As"
				currentState = 19;
				break;
			}
			case 19: {
				stateStack.Push(20);
				goto case 21;
			}
			case 20: {
				PopContext();
				goto case 15;
			}
			case 21: {
				if (la == null) { currentState = 21; break; }
				if (la.kind == 130) {
					currentState = 22;
					break;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = 22;
						break;
					} else {
						if (set[100].Get(la.kind)) {
							currentState = 22;
							break;
						} else {
							Error(la);
							goto case 22;
						}
					}
				}
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 37) {
					stateStack.Push(22);
					goto case 26;
				} else {
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (la.kind == 26) {
					currentState = 24;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 24: {
				stateStack.Push(25);
				goto case 61;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 37) {
					stateStack.Push(25);
					goto case 26;
				} else {
					goto case 23;
				}
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				Expect(37, la); // "("
				currentState = 27;
				break;
			}
			case 27: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 28;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (la.kind == 169) {
					currentState = 402;
					break;
				} else {
					if (set[13].Get(la.kind)) {
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 31;
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				if (set[14].Get(la.kind)) {
					stateStack.Push(29);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 32;
				} else {
					goto case 29;
				}
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (set[15].Get(la.kind)) {
					goto case 398;
				} else {
					if (la.kind == 22) {
						goto case 33;
					} else {
						goto case 6;
					}
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				currentState = 34;
				break;
			}
			case 34: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 35;
			}
			case 35: {
				if (la == null) { currentState = 35; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(36);
					goto case 37;
				} else {
					goto case 36;
				}
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (la.kind == 22) {
					goto case 33;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 37: {
				PushContext(Context.Expression, la, t);
				goto case 38;
			}
			case 38: {
				stateStack.Push(39);
				goto case 40;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (set[101].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 40: {
				PushContext(Context.Expression, la, t);
				goto case 41;
			}
			case 41: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 42;
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (set[102].Get(la.kind)) {
					currentState = 41;
					break;
				} else {
					if (set[24].Get(la.kind)) {
						stateStack.Push(112);
						goto case 122;
					} else {
						if (la.kind == 220) {
							currentState = 109;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(43);
								PushContext(Context.ObjectCreation, la, t);
								goto case 48;
							} else {
								if (la.kind == 35) {
									stateStack.Push(43);
									goto case 44;
								} else {
									Error(la);
									goto case 43;
								}
							}
						}
					}
				}
			}
			case 43: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				Expect(35, la); // "{"
				currentState = 45;
				break;
			}
			case 45: {
				stateStack.Push(46);
				goto case 37;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				if (la.kind == 22) {
					currentState = 45;
					break;
				} else {
					goto case 47;
				}
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				Expect(162, la); // "New"
				currentState = 49;
				break;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(106);
					goto case 21;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (la.kind == 233) {
					currentState = 53;
					break;
				} else {
					goto case 51;
				}
			}
			case 51: {
				Error(la);
				goto case 52;
			}
			case 52: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 53: {
				stateStack.Push(52);
				goto case 54;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				Expect(35, la); // "{"
				currentState = 55;
				break;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (la.kind == 147) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				Expect(26, la); // "."
				currentState = 57;
				break;
			}
			case 57: {
				stateStack.Push(58);
				goto case 61;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				Expect(20, la); // "="
				currentState = 59;
				break;
			}
			case 59: {
				stateStack.Push(60);
				goto case 37;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.kind == 22) {
					currentState = 55;
					break;
				} else {
					goto case 47;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.kind == 2) {
					goto case 105;
				} else {
					if (la.kind == 56) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 57) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 58) {
								goto case 104;
							} else {
								if (la.kind == 59) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 60) {
										currentState = stateStack.Pop();
										break;
									} else {
										if (la.kind == 61) {
											currentState = stateStack.Pop();
											break;
										} else {
											if (la.kind == 62) {
												goto case 103;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 102;
													} else {
														if (la.kind == 65) {
															goto case 101;
														} else {
															if (la.kind == 66) {
																goto case 100;
															} else {
																if (la.kind == 67) {
																	goto case 99;
																} else {
																	if (la.kind == 68) {
																		currentState = stateStack.Pop();
																		break;
																	} else {
																		if (la.kind == 69) {
																			currentState = stateStack.Pop();
																			break;
																		} else {
																			if (la.kind == 70) {
																				goto case 98;
																			} else {
																				if (la.kind == 71) {
																					currentState = stateStack.Pop();
																					break;
																				} else {
																					if (la.kind == 72) {
																						currentState = stateStack.Pop();
																						break;
																					} else {
																						if (la.kind == 73) {
																							currentState = stateStack.Pop();
																							break;
																						} else {
																							if (la.kind == 74) {
																								currentState = stateStack.Pop();
																								break;
																							} else {
																								if (la.kind == 75) {
																									currentState = stateStack.Pop();
																									break;
																								} else {
																									if (la.kind == 76) {
																										currentState = stateStack.Pop();
																										break;
																									} else {
																										if (la.kind == 77) {
																											currentState = stateStack.Pop();
																											break;
																										} else {
																											if (la.kind == 78) {
																												currentState = stateStack.Pop();
																												break;
																											} else {
																												if (la.kind == 79) {
																													currentState = stateStack.Pop();
																													break;
																												} else {
																													if (la.kind == 80) {
																														currentState = stateStack.Pop();
																														break;
																													} else {
																														if (la.kind == 81) {
																															currentState = stateStack.Pop();
																															break;
																														} else {
																															if (la.kind == 82) {
																																currentState = stateStack.Pop();
																																break;
																															} else {
																																if (la.kind == 83) {
																																	currentState = stateStack.Pop();
																																	break;
																																} else {
																																	if (la.kind == 84) {
																																		goto case 97;
																																	} else {
																																		if (la.kind == 85) {
																																			currentState = stateStack.Pop();
																																			break;
																																		} else {
																																			if (la.kind == 86) {
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 87) {
																																					goto case 96;
																																				} else {
																																					if (la.kind == 88) {
																																						currentState = stateStack.Pop();
																																						break;
																																					} else {
																																						if (la.kind == 89) {
																																							currentState = stateStack.Pop();
																																							break;
																																						} else {
																																							if (la.kind == 90) {
																																								currentState = stateStack.Pop();
																																								break;
																																							} else {
																																								if (la.kind == 91) {
																																									currentState = stateStack.Pop();
																																									break;
																																								} else {
																																									if (la.kind == 92) {
																																										currentState = stateStack.Pop();
																																										break;
																																									} else {
																																										if (la.kind == 93) {
																																											currentState = stateStack.Pop();
																																											break;
																																										} else {
																																											if (la.kind == 94) {
																																												currentState = stateStack.Pop();
																																												break;
																																											} else {
																																												if (la.kind == 95) {
																																													currentState = stateStack.Pop();
																																													break;
																																												} else {
																																													if (la.kind == 96) {
																																														currentState = stateStack.Pop();
																																														break;
																																													} else {
																																														if (la.kind == 97) {
																																															currentState = stateStack.Pop();
																																															break;
																																														} else {
																																															if (la.kind == 98) {
																																																goto case 95;
																																															} else {
																																																if (la.kind == 99) {
																																																	currentState = stateStack.Pop();
																																																	break;
																																																} else {
																																																	if (la.kind == 100) {
																																																		currentState = stateStack.Pop();
																																																		break;
																																																	} else {
																																																		if (la.kind == 101) {
																																																			currentState = stateStack.Pop();
																																																			break;
																																																		} else {
																																																			if (la.kind == 102) {
																																																				currentState = stateStack.Pop();
																																																				break;
																																																			} else {
																																																				if (la.kind == 103) {
																																																					currentState = stateStack.Pop();
																																																					break;
																																																				} else {
																																																					if (la.kind == 104) {
																																																						goto case 94;
																																																					} else {
																																																						if (la.kind == 105) {
																																																							currentState = stateStack.Pop();
																																																							break;
																																																						} else {
																																																							if (la.kind == 106) {
																																																								currentState = stateStack.Pop();
																																																								break;
																																																							} else {
																																																								if (la.kind == 107) {
																																																									goto case 93;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 92;
																																																									} else {
																																																										if (la.kind == 109) {
																																																											currentState = stateStack.Pop();
																																																											break;
																																																										} else {
																																																											if (la.kind == 110) {
																																																												currentState = stateStack.Pop();
																																																												break;
																																																											} else {
																																																												if (la.kind == 111) {
																																																													currentState = stateStack.Pop();
																																																													break;
																																																												} else {
																																																													if (la.kind == 112) {
																																																														currentState = stateStack.Pop();
																																																														break;
																																																													} else {
																																																														if (la.kind == 113) {
																																																															currentState = stateStack.Pop();
																																																															break;
																																																														} else {
																																																															if (la.kind == 114) {
																																																																currentState = stateStack.Pop();
																																																																break;
																																																															} else {
																																																																if (la.kind == 115) {
																																																																	currentState = stateStack.Pop();
																																																																	break;
																																																																} else {
																																																																	if (la.kind == 116) {
																																																																		goto case 91;
																																																																	} else {
																																																																		if (la.kind == 117) {
																																																																			currentState = stateStack.Pop();
																																																																			break;
																																																																		} else {
																																																																			if (la.kind == 118) {
																																																																				currentState = stateStack.Pop();
																																																																				break;
																																																																			} else {
																																																																				if (la.kind == 119) {
																																																																					currentState = stateStack.Pop();
																																																																					break;
																																																																				} else {
																																																																					if (la.kind == 120) {
																																																																						currentState = stateStack.Pop();
																																																																						break;
																																																																					} else {
																																																																						if (la.kind == 121) {
																																																																							goto case 90;
																																																																						} else {
																																																																							if (la.kind == 122) {
																																																																								currentState = stateStack.Pop();
																																																																								break;
																																																																							} else {
																																																																								if (la.kind == 123) {
																																																																									currentState = stateStack.Pop();
																																																																									break;
																																																																								} else {
																																																																									if (la.kind == 124) {
																																																																										goto case 89;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 88;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 87;
																																																																												} else {
																																																																													if (la.kind == 128) {
																																																																														currentState = stateStack.Pop();
																																																																														break;
																																																																													} else {
																																																																														if (la.kind == 129) {
																																																																															currentState = stateStack.Pop();
																																																																															break;
																																																																														} else {
																																																																															if (la.kind == 130) {
																																																																																currentState = stateStack.Pop();
																																																																																break;
																																																																															} else {
																																																																																if (la.kind == 131) {
																																																																																	currentState = stateStack.Pop();
																																																																																	break;
																																																																																} else {
																																																																																	if (la.kind == 132) {
																																																																																		currentState = stateStack.Pop();
																																																																																		break;
																																																																																	} else {
																																																																																		if (la.kind == 133) {
																																																																																			goto case 86;
																																																																																		} else {
																																																																																			if (la.kind == 134) {
																																																																																				currentState = stateStack.Pop();
																																																																																				break;
																																																																																			} else {
																																																																																				if (la.kind == 135) {
																																																																																					currentState = stateStack.Pop();
																																																																																					break;
																																																																																				} else {
																																																																																					if (la.kind == 136) {
																																																																																						currentState = stateStack.Pop();
																																																																																						break;
																																																																																					} else {
																																																																																						if (la.kind == 137) {
																																																																																							currentState = stateStack.Pop();
																																																																																							break;
																																																																																						} else {
																																																																																							if (la.kind == 138) {
																																																																																								currentState = stateStack.Pop();
																																																																																								break;
																																																																																							} else {
																																																																																								if (la.kind == 139) {
																																																																																									goto case 85;
																																																																																								} else {
																																																																																									if (la.kind == 140) {
																																																																																										currentState = stateStack.Pop();
																																																																																										break;
																																																																																									} else {
																																																																																										if (la.kind == 141) {
																																																																																											currentState = stateStack.Pop();
																																																																																											break;
																																																																																										} else {
																																																																																											if (la.kind == 142) {
																																																																																												currentState = stateStack.Pop();
																																																																																												break;
																																																																																											} else {
																																																																																												if (la.kind == 143) {
																																																																																													goto case 84;
																																																																																												} else {
																																																																																													if (la.kind == 144) {
																																																																																														currentState = stateStack.Pop();
																																																																																														break;
																																																																																													} else {
																																																																																														if (la.kind == 145) {
																																																																																															currentState = stateStack.Pop();
																																																																																															break;
																																																																																														} else {
																																																																																															if (la.kind == 146) {
																																																																																																goto case 83;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 82;
																																																																																																} else {
																																																																																																	if (la.kind == 148) {
																																																																																																		currentState = stateStack.Pop();
																																																																																																		break;
																																																																																																	} else {
																																																																																																		if (la.kind == 149) {
																																																																																																			currentState = stateStack.Pop();
																																																																																																			break;
																																																																																																		} else {
																																																																																																			if (la.kind == 150) {
																																																																																																				currentState = stateStack.Pop();
																																																																																																				break;
																																																																																																			} else {
																																																																																																				if (la.kind == 151) {
																																																																																																					currentState = stateStack.Pop();
																																																																																																					break;
																																																																																																				} else {
																																																																																																					if (la.kind == 152) {
																																																																																																						currentState = stateStack.Pop();
																																																																																																						break;
																																																																																																					} else {
																																																																																																						if (la.kind == 153) {
																																																																																																							currentState = stateStack.Pop();
																																																																																																							break;
																																																																																																						} else {
																																																																																																							if (la.kind == 154) {
																																																																																																								currentState = stateStack.Pop();
																																																																																																								break;
																																																																																																							} else {
																																																																																																								if (la.kind == 155) {
																																																																																																									currentState = stateStack.Pop();
																																																																																																									break;
																																																																																																								} else {
																																																																																																									if (la.kind == 156) {
																																																																																																										currentState = stateStack.Pop();
																																																																																																										break;
																																																																																																									} else {
																																																																																																										if (la.kind == 157) {
																																																																																																											currentState = stateStack.Pop();
																																																																																																											break;
																																																																																																										} else {
																																																																																																											if (la.kind == 158) {
																																																																																																												currentState = stateStack.Pop();
																																																																																																												break;
																																																																																																											} else {
																																																																																																												if (la.kind == 159) {
																																																																																																													currentState = stateStack.Pop();
																																																																																																													break;
																																																																																																												} else {
																																																																																																													if (la.kind == 160) {
																																																																																																														currentState = stateStack.Pop();
																																																																																																														break;
																																																																																																													} else {
																																																																																																														if (la.kind == 161) {
																																																																																																															currentState = stateStack.Pop();
																																																																																																															break;
																																																																																																														} else {
																																																																																																															if (la.kind == 162) {
																																																																																																																goto case 81;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 80;
																																																																																																																} else {
																																																																																																																	if (la.kind == 164) {
																																																																																																																		currentState = stateStack.Pop();
																																																																																																																		break;
																																																																																																																	} else {
																																																																																																																		if (la.kind == 165) {
																																																																																																																			currentState = stateStack.Pop();
																																																																																																																			break;
																																																																																																																		} else {
																																																																																																																			if (la.kind == 166) {
																																																																																																																				currentState = stateStack.Pop();
																																																																																																																				break;
																																																																																																																			} else {
																																																																																																																				if (la.kind == 167) {
																																																																																																																					currentState = stateStack.Pop();
																																																																																																																					break;
																																																																																																																				} else {
																																																																																																																					if (la.kind == 168) {
																																																																																																																						currentState = stateStack.Pop();
																																																																																																																						break;
																																																																																																																					} else {
																																																																																																																						if (la.kind == 169) {
																																																																																																																							currentState = stateStack.Pop();
																																																																																																																							break;
																																																																																																																						} else {
																																																																																																																							if (la.kind == 170) {
																																																																																																																								goto case 79;
																																																																																																																							} else {
																																																																																																																								if (la.kind == 171) {
																																																																																																																									currentState = stateStack.Pop();
																																																																																																																									break;
																																																																																																																								} else {
																																																																																																																									if (la.kind == 172) {
																																																																																																																										currentState = stateStack.Pop();
																																																																																																																										break;
																																																																																																																									} else {
																																																																																																																										if (la.kind == 173) {
																																																																																																																											currentState = stateStack.Pop();
																																																																																																																											break;
																																																																																																																										} else {
																																																																																																																											if (la.kind == 174) {
																																																																																																																												currentState = stateStack.Pop();
																																																																																																																												break;
																																																																																																																											} else {
																																																																																																																												if (la.kind == 175) {
																																																																																																																													currentState = stateStack.Pop();
																																																																																																																													break;
																																																																																																																												} else {
																																																																																																																													if (la.kind == 176) {
																																																																																																																														goto case 78;
																																																																																																																													} else {
																																																																																																																														if (la.kind == 177) {
																																																																																																																															currentState = stateStack.Pop();
																																																																																																																															break;
																																																																																																																														} else {
																																																																																																																															if (la.kind == 178) {
																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																break;
																																																																																																																															} else {
																																																																																																																																if (la.kind == 179) {
																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																	break;
																																																																																																																																} else {
																																																																																																																																	if (la.kind == 180) {
																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																		break;
																																																																																																																																	} else {
																																																																																																																																		if (la.kind == 181) {
																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																			break;
																																																																																																																																		} else {
																																																																																																																																			if (la.kind == 182) {
																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																				break;
																																																																																																																																			} else {
																																																																																																																																				if (la.kind == 183) {
																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																					break;
																																																																																																																																				} else {
																																																																																																																																					if (la.kind == 184) {
																																																																																																																																						goto case 77;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 76;
																																																																																																																																							} else {
																																																																																																																																								if (la.kind == 187) {
																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																									break;
																																																																																																																																								} else {
																																																																																																																																									if (la.kind == 188) {
																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																										break;
																																																																																																																																									} else {
																																																																																																																																										if (la.kind == 189) {
																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																											break;
																																																																																																																																										} else {
																																																																																																																																											if (la.kind == 190) {
																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																												break;
																																																																																																																																											} else {
																																																																																																																																												if (la.kind == 191) {
																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																													break;
																																																																																																																																												} else {
																																																																																																																																													if (la.kind == 192) {
																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																														break;
																																																																																																																																													} else {
																																																																																																																																														if (la.kind == 193) {
																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																															break;
																																																																																																																																														} else {
																																																																																																																																															if (la.kind == 194) {
																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																break;
																																																																																																																																															} else {
																																																																																																																																																if (la.kind == 195) {
																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																	break;
																																																																																																																																																} else {
																																																																																																																																																	if (la.kind == 196) {
																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																		break;
																																																																																																																																																	} else {
																																																																																																																																																		if (la.kind == 197) {
																																																																																																																																																			goto case 75;
																																																																																																																																																		} else {
																																																																																																																																																			if (la.kind == 198) {
																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																				break;
																																																																																																																																																			} else {
																																																																																																																																																				if (la.kind == 199) {
																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																					break;
																																																																																																																																																				} else {
																																																																																																																																																					if (la.kind == 200) {
																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																						break;
																																																																																																																																																					} else {
																																																																																																																																																						if (la.kind == 201) {
																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																							break;
																																																																																																																																																						} else {
																																																																																																																																																							if (la.kind == 202) {
																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																								break;
																																																																																																																																																							} else {
																																																																																																																																																								if (la.kind == 203) {
																																																																																																																																																									goto case 74;
																																																																																																																																																								} else {
																																																																																																																																																									if (la.kind == 204) {
																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																										break;
																																																																																																																																																									} else {
																																																																																																																																																										if (la.kind == 205) {
																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																											break;
																																																																																																																																																										} else {
																																																																																																																																																											if (la.kind == 206) {
																																																																																																																																																												goto case 73;
																																																																																																																																																											} else {
																																																																																																																																																												if (la.kind == 207) {
																																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																																													break;
																																																																																																																																																												} else {
																																																																																																																																																													if (la.kind == 208) {
																																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																																														break;
																																																																																																																																																													} else {
																																																																																																																																																														if (la.kind == 209) {
																																																																																																																																																															goto case 72;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 71;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 70;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 69;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 68;
																																																																																																																																																																		} else {
																																																																																																																																																																			if (la.kind == 214) {
																																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																																				break;
																																																																																																																																																																			} else {
																																																																																																																																																																				if (la.kind == 215) {
																																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																																					break;
																																																																																																																																																																				} else {
																																																																																																																																																																					if (la.kind == 216) {
																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																						break;
																																																																																																																																																																					} else {
																																																																																																																																																																						if (la.kind == 217) {
																																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																																							break;
																																																																																																																																																																						} else {
																																																																																																																																																																							if (la.kind == 218) {
																																																																																																																																																																								goto case 67;
																																																																																																																																																																							} else {
																																																																																																																																																																								if (la.kind == 219) {
																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																									break;
																																																																																																																																																																								} else {
																																																																																																																																																																									if (la.kind == 220) {
																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																										break;
																																																																																																																																																																									} else {
																																																																																																																																																																										if (la.kind == 221) {
																																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																																											break;
																																																																																																																																																																										} else {
																																																																																																																																																																											if (la.kind == 222) {
																																																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																																																												break;
																																																																																																																																																																											} else {
																																																																																																																																																																												if (la.kind == 223) {
																																																																																																																																																																													goto case 66;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 65;
																																																																																																																																																																													} else {
																																																																																																																																																																														if (la.kind == 225) {
																																																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																																																															break;
																																																																																																																																																																														} else {
																																																																																																																																																																															if (la.kind == 226) {
																																																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																																																break;
																																																																																																																																																																															} else {
																																																																																																																																																																																if (la.kind == 227) {
																																																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																																																	break;
																																																																																																																																																																																} else {
																																																																																																																																																																																	if (la.kind == 228) {
																																																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																																																		break;
																																																																																																																																																																																	} else {
																																																																																																																																																																																		if (la.kind == 229) {
																																																																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																																																																			break;
																																																																																																																																																																																		} else {
																																																																																																																																																																																			if (la.kind == 230) {
																																																																																																																																																																																				goto case 64;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 63;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 62;
																																																																																																																																																																																						} else {
																																																																																																																																																																																							if (la.kind == 234) {
																																																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																																																								break;
																																																																																																																																																																																							} else {
																																																																																																																																																																																								if (la.kind == 235) {
																																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																																									break;
																																																																																																																																																																																								} else {
																																																																																																																																																																																									if (la.kind == 236) {
																																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																																										break;
																																																																																																																																																																																									} else {
																																																																																																																																																																																										if (la.kind == 237) {
																																																																																																																																																																																											currentState = stateStack.Pop();
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
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 83: {
				if (la == null) { currentState = 83; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 107;
						break;
					} else {
						goto case 50;
					}
				} else {
					goto case 52;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (la.kind == 35) {
					stateStack.Push(52);
					goto case 44;
				} else {
					if (set[19].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 51;
					}
				}
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				currentState = 52;
				break;
			}
			case 109: {
				stateStack.Push(110);
				goto case 40;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				Expect(144, la); // "Is"
				currentState = 111;
				break;
			}
			case 111: {
				stateStack.Push(43);
				goto case 21;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(112);
					goto case 113;
				} else {
					goto case 43;
				}
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (la.kind == 37) {
					currentState = 118;
					break;
				} else {
					if (set[103].Get(la.kind)) {
						currentState = 114;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 114: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 115;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.kind == 10) {
					currentState = 116;
					break;
				} else {
					goto case 116;
				}
			}
			case 116: {
				stateStack.Push(117);
				goto case 61;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 118: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 119;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				if (la.kind == 169) {
					currentState = 120;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 30;
					} else {
						goto case 6;
					}
				}
			}
			case 120: {
				stateStack.Push(121);
				goto case 21;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 22) {
					currentState = 120;
					break;
				} else {
					goto case 29;
				}
			}
			case 122: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 123;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				if (set[104].Get(la.kind)) {
					currentState = 124;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 397;
						break;
					} else {
						if (set[105].Get(la.kind)) {
							currentState = 124;
							break;
						} else {
							if (set[103].Get(la.kind)) {
								currentState = 393;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 391;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 388;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(124);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 377;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(124);
												goto case 200;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(124);
													PushContext(Context.Query, la, t);
													goto case 138;
												} else {
													if (set[26].Get(la.kind)) {
														stateStack.Push(124);
														goto case 132;
													} else {
														if (la.kind == 135) {
															stateStack.Push(124);
															goto case 125;
														} else {
															Error(la);
															goto case 124;
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
			case 124: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				Expect(135, la); // "If"
				currentState = 126;
				break;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				Expect(37, la); // "("
				currentState = 127;
				break;
			}
			case 127: {
				stateStack.Push(128);
				goto case 37;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(22, la); // ","
				currentState = 129;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 37;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 22) {
					currentState = 131;
					break;
				} else {
					goto case 29;
				}
			}
			case 131: {
				stateStack.Push(29);
				goto case 37;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (set[106].Get(la.kind)) {
					currentState = 137;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 133;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				Expect(37, la); // "("
				currentState = 134;
				break;
			}
			case 134: {
				stateStack.Push(135);
				goto case 37;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				Expect(22, la); // ","
				currentState = 136;
				break;
			}
			case 136: {
				stateStack.Push(29);
				goto case 21;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				Expect(37, la); // "("
				currentState = 131;
				break;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 126) {
					stateStack.Push(139);
					goto case 199;
				} else {
					if (la.kind == 58) {
						stateStack.Push(139);
						goto case 198;
					} else {
						Error(la);
						goto case 139;
					}
				}
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(139);
					goto case 140;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 126) {
					currentState = 196;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 192;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 190;
							break;
						} else {
							if (la.kind == 107) {
								goto case 93;
							} else {
								if (la.kind == 230) {
									currentState = 37;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 186;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 184;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 182;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 155;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 141;
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
			case 141: {
				stateStack.Push(142);
				goto case 147;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(171, la); // "On"
				currentState = 143;
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 37;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(116, la); // "Equals"
				currentState = 145;
				break;
			}
			case 145: {
				stateStack.Push(146);
				goto case 37;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 22) {
					currentState = 143;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 147: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(148);
				goto case 154;
			}
			case 148: {
				PopContext();
				goto case 149;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 151;
				} else {
					goto case 150;
				}
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(138, la); // "In"
				currentState = 37;
				break;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(63, la); // "As"
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(153);
				goto case 21;
			}
			case 153: {
				PopContext();
				goto case 150;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (set[88].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 95;
					} else {
						goto case 6;
					}
				}
			}
			case 155: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 156;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (la.kind == 146) {
					goto case 174;
				} else {
					if (set[30].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 158;
							break;
						} else {
							if (set[30].Get(la.kind)) {
								goto case 172;
							} else {
								Error(la);
								goto case 157;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(70, la); // "By"
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 162;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 22) {
					currentState = 158;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 160;
					break;
				}
			}
			case 160: {
				stateStack.Push(161);
				goto case 162;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 22) {
					currentState = 160;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 162: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 163;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(164);
					goto case 154;
				} else {
					goto case 37;
				}
			}
			case 164: {
				PopContext();
				goto case 165;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 168;
				} else {
					if (la.kind == 20) {
						goto case 167;
					} else {
						if (set[31].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 37;
						}
					}
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				currentState = 37;
				break;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				currentState = 37;
				break;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				Expect(63, la); // "As"
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 21;
			}
			case 170: {
				PopContext();
				goto case 171;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				Expect(20, la); // "="
				currentState = 37;
				break;
			}
			case 172: {
				stateStack.Push(173);
				goto case 162;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				if (la.kind == 22) {
					currentState = 172;
					break;
				} else {
					goto case 157;
				}
			}
			case 174: {
				stateStack.Push(175);
				goto case 181;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 179;
						break;
					} else {
						if (la.kind == 146) {
							goto case 174;
						} else {
							Error(la);
							goto case 175;
						}
					}
				} else {
					goto case 176;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(143, la); // "Into"
				currentState = 177;
				break;
			}
			case 177: {
				stateStack.Push(178);
				goto case 162;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 22) {
					currentState = 177;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 179: {
				stateStack.Push(180);
				goto case 181;
			}
			case 180: {
				stateStack.Push(175);
				goto case 176;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(146, la); // "Join"
				currentState = 141;
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 162;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (la.kind == 22) {
					currentState = 182;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 184: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 231) {
					currentState = 37;
					break;
				} else {
					goto case 37;
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(70, la); // "By"
				currentState = 187;
				break;
			}
			case 187: {
				stateStack.Push(188);
				goto case 37;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 64) {
					currentState = 189;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 189;
						break;
					} else {
						Error(la);
						goto case 189;
					}
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.kind == 22) {
					currentState = 187;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 190: {
				stateStack.Push(191);
				goto case 162;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.kind == 22) {
					currentState = 190;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 192: {
				stateStack.Push(193);
				goto case 147;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(193);
					goto case 140;
				} else {
					Expect(143, la); // "Into"
					currentState = 194;
					break;
				}
			}
			case 194: {
				stateStack.Push(195);
				goto case 162;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (la.kind == 22) {
					currentState = 194;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 196: {
				stateStack.Push(197);
				goto case 147;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.kind == 22) {
					currentState = 196;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(58, la); // "Aggregate"
				currentState = 192;
				break;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(126, la); // "From"
				currentState = 196;
				break;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 210) {
					currentState = 370;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 201;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(37, la); // "("
				currentState = 202;
				break;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(203);
					goto case 355;
				} else {
					goto case 203;
				}
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				Expect(38, la); // ")"
				currentState = 204;
				break;
			}
			case 204: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 205;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[15].Get(la.kind)) {
					goto case 37;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							PushContext(Context.Type, la, t);
							goto case 352;
						} else {
							goto case 206;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 206: {
				stateStack.Push(207);
				goto case 209;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(113, la); // "End"
				currentState = 208;
				break;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 209: {
				PushContext(Context.Body, la, t);
				goto case 210;
			}
			case 210: {
				stateStack.Push(211);
				goto case 15;
			}
			case 211: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 212;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (set[107].Get(la.kind)) {
					if (set[57].Get(la.kind)) {
						if (set[39].Get(la.kind)) {
							stateStack.Push(210);
							goto case 217;
						} else {
							goto case 210;
						}
					} else {
						if (la.kind == 113) {
							currentState = 215;
							break;
						} else {
							goto case 214;
						}
					}
				} else {
					goto case 213;
				}
			}
			case 213: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 214: {
				Error(la);
				goto case 211;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 210;
				} else {
					if (set[38].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 214;
					}
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				currentState = 211;
				break;
			}
			case 217: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 333;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 329;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 327;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 325;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 306;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 291;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 287;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 281;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 253;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 249;
																break;
															} else {
																goto case 249;
															}
														} else {
															if (la.kind == 194) {
																currentState = 247;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 227;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 234;
																break;
															} else {
																if (set[108].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 231;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 230;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 229;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 73;
																				} else {
																					if (la.kind == 195) {
																						currentState = 227;
																						break;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 225;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 223;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 219;
																				break;
																			} else {
																				if (set[109].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 37;
																						break;
																					} else {
																						goto case 37;
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
			case 219: {
				stateStack.Push(220);
				goto case 37;
			}
			case 220: {
				stateStack.Push(221);
				goto case 209;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(113, la); // "End"
				currentState = 222;
				break;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 223: {
				stateStack.Push(224);
				goto case 37;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 22) {
					currentState = 223;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 225: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 226;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.kind == 184) {
					currentState = 37;
					break;
				} else {
					goto case 37;
				}
			}
			case 227: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 228;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (set[15].Get(la.kind)) {
					goto case 37;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (la.kind == 108) {
					goto case 92;
				} else {
					if (la.kind == 124) {
						goto case 89;
					} else {
						if (la.kind == 231) {
							goto case 63;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 108) {
					goto case 92;
				} else {
					if (la.kind == 124) {
						goto case 89;
					} else {
						if (la.kind == 231) {
							goto case 63;
						} else {
							if (la.kind == 197) {
								goto case 75;
							} else {
								if (la.kind == 210) {
									goto case 71;
								} else {
									if (la.kind == 127) {
										goto case 87;
									} else {
										if (la.kind == 186) {
											goto case 76;
										} else {
											if (la.kind == 218) {
												goto case 67;
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
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (set[28].Get(la.kind)) {
					goto case 233;
				} else {
					if (la.kind == 5) {
						goto case 232;
					} else {
						goto case 6;
					}
				}
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				stateStack.Push(235);
				goto case 209;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 75) {
					currentState = 239;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 238;
						break;
					} else {
						goto case 236;
					}
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(113, la); // "End"
				currentState = 237;
				break;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				stateStack.Push(236);
				goto case 209;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(242);
					goto case 154;
				} else {
					goto case 240;
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 229) {
					currentState = 241;
					break;
				} else {
					goto case 234;
				}
			}
			case 241: {
				stateStack.Push(234);
				goto case 37;
			}
			case 242: {
				PopContext();
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 244;
				} else {
					goto case 240;
				}
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				Expect(63, la); // "As"
				currentState = 245;
				break;
			}
			case 245: {
				stateStack.Push(246);
				goto case 21;
			}
			case 246: {
				PopContext();
				goto case 240;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 163) {
					goto case 80;
				} else {
					goto case 248;
				}
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (la.kind == 5) {
					goto case 232;
				} else {
					if (set[28].Get(la.kind)) {
						goto case 233;
					} else {
						goto case 6;
					}
				}
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				Expect(118, la); // "Error"
				currentState = 250;
				break;
			}
			case 250: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 251;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (set[15].Get(la.kind)) {
					goto case 37;
				} else {
					if (la.kind == 132) {
						currentState = 248;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 252;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 253: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 254;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (set[24].Get(la.kind)) {
					stateStack.Push(271);
					goto case 264;
				} else {
					if (la.kind == 110) {
						currentState = 255;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 255: {
				stateStack.Push(256);
				goto case 264;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				Expect(138, la); // "In"
				currentState = 257;
				break;
			}
			case 257: {
				stateStack.Push(258);
				goto case 37;
			}
			case 258: {
				stateStack.Push(259);
				goto case 209;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(163, la); // "Next"
				currentState = 260;
				break;
			}
			case 260: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[15].Get(la.kind)) {
					goto case 262;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 262: {
				stateStack.Push(263);
				goto case 37;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.kind == 22) {
					currentState = 262;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 264: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(265);
				goto case 122;
			}
			case 265: {
				PopContext();
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.kind == 33) {
					currentState = 267;
					break;
				} else {
					goto case 267;
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(267);
					goto case 113;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 268;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(63, la); // "As"
				currentState = 269;
				break;
			}
			case 269: {
				stateStack.Push(270);
				goto case 21;
			}
			case 270: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				Expect(20, la); // "="
				currentState = 272;
				break;
			}
			case 272: {
				stateStack.Push(273);
				goto case 37;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 205) {
					currentState = 280;
					break;
				} else {
					goto case 274;
				}
			}
			case 274: {
				stateStack.Push(275);
				goto case 209;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(163, la); // "Next"
				currentState = 276;
				break;
			}
			case 276: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 277;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (set[15].Get(la.kind)) {
					goto case 278;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 278: {
				stateStack.Push(279);
				goto case 37;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (la.kind == 22) {
					currentState = 278;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 280: {
				stateStack.Push(274);
				goto case 37;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 284;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(282);
						goto case 209;
					} else {
						goto case 6;
					}
				}
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(152, la); // "Loop"
				currentState = 283;
				break;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 37;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 284: {
				stateStack.Push(285);
				goto case 37;
			}
			case 285: {
				stateStack.Push(286);
				goto case 209;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 287: {
				stateStack.Push(288);
				goto case 37;
			}
			case 288: {
				stateStack.Push(289);
				goto case 209;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				Expect(113, la); // "End"
				currentState = 290;
				break;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 291: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 292;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 74) {
					currentState = 293;
					break;
				} else {
					goto case 293;
				}
			}
			case 293: {
				stateStack.Push(294);
				goto case 37;
			}
			case 294: {
				stateStack.Push(295);
				goto case 15;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 74) {
					currentState = 297;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 296;
					break;
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 297: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 298;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 111) {
					currentState = 299;
					break;
				} else {
					if (set[55].Get(la.kind)) {
						goto case 300;
					} else {
						Error(la);
						goto case 299;
					}
				}
			}
			case 299: {
				stateStack.Push(295);
				goto case 209;
			}
			case 300: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 301;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (set[110].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 303;
						break;
					} else {
						goto case 303;
					}
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(302);
						goto case 37;
					} else {
						Error(la);
						goto case 302;
					}
				}
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (la.kind == 22) {
					currentState = 300;
					break;
				} else {
					goto case 299;
				}
			}
			case 303: {
				stateStack.Push(304);
				goto case 305;
			}
			case 304: {
				stateStack.Push(302);
				goto case 40;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 20) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 41) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 40) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 39) {
								currentState = stateStack.Pop();
								break;
							} else {
								if (la.kind == 42) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 43) {
										currentState = stateStack.Pop();
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
			case 306: {
				stateStack.Push(307);
				goto case 37;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (la.kind == 214) {
					currentState = 315;
					break;
				} else {
					goto case 308;
				}
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 309;
				} else {
					goto case 6;
				}
			}
			case 309: {
				stateStack.Push(310);
				goto case 209;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 314;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 312;
							break;
						} else {
							Error(la);
							goto case 309;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 311;
					break;
				}
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 312: {
				stateStack.Push(313);
				goto case 37;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 214) {
					currentState = 309;
					break;
				} else {
					goto case 309;
				}
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 135) {
					currentState = 312;
					break;
				} else {
					goto case 309;
				}
			}
			case 315: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 316;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (set[39].Get(la.kind)) {
					goto case 317;
				} else {
					goto case 308;
				}
			}
			case 317: {
				stateStack.Push(318);
				goto case 217;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 21) {
					currentState = 323;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 320;
						break;
					} else {
						goto case 319;
					}
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 320: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 321;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (set[39].Get(la.kind)) {
					stateStack.Push(322);
					goto case 217;
				} else {
					goto case 322;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 21) {
					currentState = 320;
					break;
				} else {
					goto case 319;
				}
			}
			case 323: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 324;
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (set[39].Get(la.kind)) {
					goto case 317;
				} else {
					goto case 318;
				}
			}
			case 325: {
				stateStack.Push(326);
				goto case 61;
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				if (la.kind == 37) {
					currentState = 30;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 327: {
				stateStack.Push(328);
				goto case 37;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				Expect(22, la); // ","
				currentState = 37;
				break;
			}
			case 329: {
				stateStack.Push(330);
				goto case 37;
			}
			case 330: {
				stateStack.Push(331);
				goto case 209;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(113, la); // "End"
				currentState = 332;
				break;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 233) {
					goto case 62;
				} else {
					if (la.kind == 211) {
						goto case 70;
					} else {
						goto case 6;
					}
				}
			}
			case 333: {
				PushContext(Context.IdentifierExpected, la, t);	
				stateStack.Push(334);
				goto case 154;
			}
			case 334: {
				PopContext();
				goto case 335;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 33) {
					currentState = 336;
					break;
				} else {
					goto case 336;
				}
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 37) {
					currentState = 351;
					break;
				} else {
					goto case 337;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 22) {
					currentState = 345;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 339;
					} else {
						goto case 338;
					}
				}
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 20) {
					goto case 167;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				Expect(63, la); // "As"
				currentState = 340;
				break;
			}
			case 340: {
				stateStack.Push(341);
				goto case 21;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 162) {
					currentState = 344;
					break;
				} else {
					goto case 342;
				}
			}
			case 342: {
				stateStack.Push(343);
				goto case 21;
			}
			case 343: {
				if (CurrentBlock.context == Context.ObjectCreation)
					PopContext();
				PopContext();

				goto case 338;
			}
			case 344: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 342;
			}
			case 345: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(346);
				goto case 154;
			}
			case 346: {
				PopContext();
				goto case 347;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 33) {
					currentState = 348;
					break;
				} else {
					goto case 348;
				}
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 37) {
					currentState = 349;
					break;
				} else {
					goto case 337;
				}
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (la.kind == 22) {
					currentState = 349;
					break;
				} else {
					goto case 350;
				}
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				Expect(38, la); // ")"
				currentState = 337;
				break;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 22) {
					currentState = 351;
					break;
				} else {
					goto case 350;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				Expect(63, la); // "As"
				currentState = 353;
				break;
			}
			case 353: {
				stateStack.Push(354);
				goto case 21;
			}
			case 354: {
				PopContext();
				goto case 206;
			}
			case 355: {
				stateStack.Push(356);
				goto case 357;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 22) {
					currentState = 355;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 40) {
					stateStack.Push(357);
					goto case 365;
				} else {
					goto case 358;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (set[111].Get(la.kind)) {
					currentState = 358;
					break;
				} else {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(359);
					goto case 154;
				}
			}
			case 359: {
				PopContext();
				goto case 360;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 362;
				} else {
					goto case 361;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 20) {
					goto case 167;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(63, la); // "As"
				currentState = 363;
				break;
			}
			case 363: {
				stateStack.Push(364);
				goto case 21;
			}
			case 364: {
				PopContext();
				goto case 361;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				Expect(40, la); // "<"
				currentState = 366;
				break;
			}
			case 366: {
				PushContext(Context.Attribute, la, t);
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (set[112].Get(la.kind)) {
					currentState = 367;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 368;
					break;
				}
			}
			case 368: {
				PopContext();
				goto case 369;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 1) {
					goto case 17;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				Expect(37, la); // "("
				currentState = 371;
				break;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(372);
					goto case 355;
				} else {
					goto case 372;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(38, la); // ")"
				currentState = 373;
				break;
			}
			case 373: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 374;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (set[39].Get(la.kind)) {
					goto case 217;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(375);
						goto case 209;
					} else {
						goto case 6;
					}
				}
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				Expect(113, la); // "End"
				currentState = 376;
				break;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 387;
					break;
				} else {
					stateStack.Push(378);
					goto case 380;
				}
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 17) {
					currentState = 379;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 16) {
					currentState = 378;
					break;
				} else {
					goto case 378;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 381;
				break;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (set[113].Get(la.kind)) {
					if (set[114].Get(la.kind)) {
						currentState = 381;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(381);
							goto case 384;
						} else {
							Error(la);
							goto case 381;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 382;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (set[115].Get(la.kind)) {
					if (set[116].Get(la.kind)) {
						currentState = 382;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(382);
							goto case 384;
						} else {
							if (la.kind == 10) {
								stateStack.Push(382);
								goto case 380;
							} else {
								Error(la);
								goto case 382;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 383;
					break;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (set[117].Get(la.kind)) {
					if (set[118].Get(la.kind)) {
						currentState = 383;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(383);
							goto case 384;
						} else {
							Error(la);
							goto case 383;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 385;
				break;
			}
			case 385: {
				stateStack.Push(386);
				goto case 37;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 16) {
					currentState = 377;
					break;
				} else {
					goto case 377;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(37, la); // "("
				currentState = 389;
				break;
			}
			case 389: {
				readXmlIdentifier = true;
				stateStack.Push(390);
				goto case 154;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(38, la); // ")"
				currentState = 124;
				break;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				Expect(37, la); // "("
				currentState = 392;
				break;
			}
			case 392: {
				stateStack.Push(390);
				goto case 21;
			}
			case 393: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 394;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 10) {
					currentState = 395;
					break;
				} else {
					goto case 395;
				}
			}
			case 395: {
				stateStack.Push(396);
				goto case 61;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (la.kind == 11) {
					currentState = 124;
					break;
				} else {
					goto case 124;
				}
			}
			case 397: {
				stateStack.Push(390);
				goto case 37;
			}
			case 398: {
				stateStack.Push(399);
				goto case 37;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (la.kind == 22) {
					currentState = 400;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 400: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 401;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (set[15].Get(la.kind)) {
					goto case 398;
				} else {
					goto case 399;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(403);
					goto case 21;
				} else {
					goto case 403;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 22) {
					currentState = 402;
					break;
				} else {
					goto case 29;
				}
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(405);
					goto case 355;
				} else {
					goto case 405;
				}
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 406: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 407;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				currentState = 408;
				break;
			}
			case 408: {
				PopContext();
				goto case 409;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 37) {
					currentState = 501;
					break;
				} else {
					goto case 410;
				}
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (set[119].Get(la.kind)) {
					currentState = 410;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(411);
						goto case 15;
					} else {
						goto case 411;
					}
				}
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 140) {
					currentState = 500;
					break;
				} else {
					goto case 412;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 136) {
					currentState = 499;
					break;
				} else {
					goto case 413;
				}
			}
			case 413: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 414;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (set[79].Get(la.kind)) {
					stateStack.Push(414);
					PushContext(Context.Member, la, t);
					goto case 418;
				} else {
					Expect(113, la); // "End"
					currentState = 415;
					break;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 155) {
					currentState = 416;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 416;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 416;
							break;
						} else {
							Error(la);
							goto case 416;
						}
					}
				}
			}
			case 416: {
				stateStack.Push(417);
				goto case 15;
			}
			case 417: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 40) {
					stateStack.Push(418);
					goto case 365;
				} else {
					goto case 419;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (set[120].Get(la.kind)) {
					currentState = 419;
					break;
				} else {
					if (set[87].Get(la.kind)) {
						stateStack.Push(420);
						goto case 489;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(420);
							goto case 474;
						} else {
							if (la.kind == 101) {
								stateStack.Push(420);
								goto case 458;
							} else {
								if (la.kind == 119) {
									stateStack.Push(420);
									goto case 446;
								} else {
									if (la.kind == 98) {
										stateStack.Push(420);
										goto case 434;
									} else {
										if (la.kind == 172) {
											stateStack.Push(420);
											goto case 421;
										} else {
											Error(la);
											goto case 420;
										}
									}
								}
							}
						}
					}
				}
			}
			case 420: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(172, la); // "Operator"
				currentState = 422;
				break;
			}
			case 422: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 423;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				currentState = 424;
				break;
			}
			case 424: {
				PopContext();
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				Expect(37, la); // "("
				currentState = 426;
				break;
			}
			case 426: {
				stateStack.Push(427);
				goto case 355;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				Expect(38, la); // ")"
				currentState = 428;
				break;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 63) {
					currentState = 432;
					break;
				} else {
					goto case 429;
				}
			}
			case 429: {
				stateStack.Push(430);
				goto case 209;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				Expect(113, la); // "End"
				currentState = 431;
				break;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 40) {
					stateStack.Push(432);
					goto case 365;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(433);
					goto case 21;
				}
			}
			case 433: {
				PopContext();
				goto case 429;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				Expect(98, la); // "Custom"
				currentState = 435;
				break;
			}
			case 435: {
				stateStack.Push(436);
				goto case 446;
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (set[83].Get(la.kind)) {
					goto case 438;
				} else {
					Expect(113, la); // "End"
					currentState = 437;
					break;
				}
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 40) {
					stateStack.Push(438);
					goto case 365;
				} else {
					if (la.kind == 56) {
						currentState = 439;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 439;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 439;
								break;
							} else {
								Error(la);
								goto case 439;
							}
						}
					}
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				Expect(37, la); // "("
				currentState = 440;
				break;
			}
			case 440: {
				stateStack.Push(441);
				goto case 355;
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				Expect(38, la); // ")"
				currentState = 442;
				break;
			}
			case 442: {
				stateStack.Push(443);
				goto case 209;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				Expect(113, la); // "End"
				currentState = 444;
				break;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 56) {
					currentState = 445;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 445;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 445;
							break;
						} else {
							Error(la);
							goto case 445;
						}
					}
				}
			}
			case 445: {
				stateStack.Push(436);
				goto case 15;
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				Expect(119, la); // "Event"
				currentState = 447;
				break;
			}
			case 447: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(448);
				goto case 154;
			}
			case 448: {
				PopContext();
				goto case 449;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 455;
				} else {
					if (set[121].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 453;
							break;
						} else {
							goto case 450;
						}
					} else {
						Error(la);
						goto case 450;
					}
				}
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 136) {
					currentState = 451;
					break;
				} else {
					goto case 15;
				}
			}
			case 451: {
				stateStack.Push(452);
				goto case 21;
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 22) {
					currentState = 451;
					break;
				} else {
					goto case 15;
				}
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(454);
					goto case 355;
				} else {
					goto case 454;
				}
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				Expect(38, la); // ")"
				currentState = 450;
				break;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				Expect(63, la); // "As"
				currentState = 456;
				break;
			}
			case 456: {
				stateStack.Push(457);
				goto case 21;
			}
			case 457: {
				PopContext();
				goto case 450;
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				Expect(101, la); // "Declare"
				currentState = 459;
				break;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 460;
					break;
				} else {
					goto case 460;
				}
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (la.kind == 210) {
					currentState = 461;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 461;
						break;
					} else {
						Error(la);
						goto case 461;
					}
				}
			}
			case 461: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(462);
				goto case 154;
			}
			case 462: {
				PopContext();
				goto case 463;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(149, la); // "Lib"
				currentState = 464;
				break;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				Expect(3, la); // LiteralString
				currentState = 465;
				break;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				if (la.kind == 59) {
					currentState = 473;
					break;
				} else {
					goto case 466;
				}
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 37) {
					currentState = 471;
					break;
				} else {
					goto case 467;
				}
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 468;
				} else {
					goto case 15;
				}
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				Expect(63, la); // "As"
				currentState = 469;
				break;
			}
			case 469: {
				stateStack.Push(470);
				goto case 21;
			}
			case 470: {
				PopContext();
				goto case 15;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(472);
					goto case 355;
				} else {
					goto case 472;
				}
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				Expect(38, la); // ")"
				currentState = 467;
				break;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				Expect(3, la); // LiteralString
				currentState = 466;
				break;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (la.kind == 210) {
					currentState = 475;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 475;
						break;
					} else {
						Error(la);
						goto case 475;
					}
				}
			}
			case 475: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 476;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				currentState = 477;
				break;
			}
			case 477: {
				PopContext();
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (la.kind == 37) {
					currentState = 487;
					break;
				} else {
					goto case 479;
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 484;
				} else {
					goto case 480;
				}
			}
			case 480: {
				stateStack.Push(481);
				goto case 209;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				Expect(113, la); // "End"
				currentState = 482;
				break;
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				if (la.kind == 210) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 15;
						break;
					} else {
						goto case 483;
					}
				}
			}
			case 483: {
				Error(la);
				goto case 15;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				Expect(63, la); // "As"
				currentState = 485;
				break;
			}
			case 485: {
				stateStack.Push(486);
				goto case 21;
			}
			case 486: {
				PopContext();
				goto case 480;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(488);
					goto case 355;
				} else {
					goto case 488;
				}
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				Expect(38, la); // ")"
				currentState = 479;
				break;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				if (la.kind == 88) {
					currentState = 490;
					break;
				} else {
					goto case 490;
				}
			}
			case 490: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(491);
				goto case 498;
			}
			case 491: {
				PopContext();
				goto case 492;
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 495;
				} else {
					goto case 493;
				}
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 20) {
					currentState = 494;
					break;
				} else {
					goto case 15;
				}
			}
			case 494: {
				stateStack.Push(15);
				goto case 37;
			}
			case 495: {
				if (la == null) { currentState = 495; break; }
				Expect(63, la); // "As"
				currentState = 496;
				break;
			}
			case 496: {
				stateStack.Push(497);
				goto case 21;
			}
			case 497: {
				PopContext();
				goto case 493;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (set[105].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 104;
					} else {
						if (la.kind == 126) {
							goto case 88;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				if (set[38].Get(la.kind)) {
					currentState = 499;
					break;
				} else {
					stateStack.Push(413);
					goto case 15;
				}
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (set[38].Get(la.kind)) {
					currentState = 500;
					break;
				} else {
					stateStack.Push(412);
					goto case 15;
				}
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				Expect(169, la); // "Of"
				currentState = 502;
				break;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 503;
					break;
				} else {
					goto case 503;
				}
			}
			case 503: {
				stateStack.Push(504);
				goto case 516;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 517;
				} else {
					goto case 505;
				}
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 22) {
					currentState = 506;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 410;
					break;
				}
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 507;
					break;
				} else {
					goto case 507;
				}
			}
			case 507: {
				stateStack.Push(508);
				goto case 516;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 509;
				} else {
					goto case 505;
				}
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				Expect(63, la); // "As"
				currentState = 510;
				break;
			}
			case 510: {
				stateStack.Push(511);
				goto case 512;
			}
			case 511: {
				PopContext();
				goto case 505;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				if (set[93].Get(la.kind)) {
					goto case 515;
				} else {
					if (la.kind == 35) {
						currentState = 513;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 513: {
				stateStack.Push(514);
				goto case 515;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 22) {
					currentState = 513;
					break;
				} else {
					goto case 47;
				}
			}
			case 515: {
				if (la == null) { currentState = 515; break; }
				if (set[8].Get(la.kind)) {
					currentState = 22;
					break;
				} else {
					if (la.kind == 162) {
						goto case 81;
					} else {
						if (la.kind == 84) {
							goto case 97;
						} else {
							if (la.kind == 209) {
								goto case 72;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				if (la.kind == 2) {
					goto case 105;
				} else {
					if (la.kind == 62) {
						goto case 103;
					} else {
						if (la.kind == 64) {
							goto case 102;
						} else {
							if (la.kind == 65) {
								goto case 101;
							} else {
								if (la.kind == 66) {
									goto case 100;
								} else {
									if (la.kind == 67) {
										goto case 99;
									} else {
										if (la.kind == 70) {
											goto case 98;
										} else {
											if (la.kind == 87) {
												goto case 96;
											} else {
												if (la.kind == 104) {
													goto case 94;
												} else {
													if (la.kind == 107) {
														goto case 93;
													} else {
														if (la.kind == 116) {
															goto case 91;
														} else {
															if (la.kind == 121) {
																goto case 90;
															} else {
																if (la.kind == 133) {
																	goto case 86;
																} else {
																	if (la.kind == 139) {
																		goto case 85;
																	} else {
																		if (la.kind == 143) {
																			goto case 84;
																		} else {
																			if (la.kind == 146) {
																				goto case 83;
																			} else {
																				if (la.kind == 147) {
																					goto case 82;
																				} else {
																					if (la.kind == 170) {
																						goto case 79;
																					} else {
																						if (la.kind == 176) {
																							goto case 78;
																						} else {
																							if (la.kind == 184) {
																								goto case 77;
																							} else {
																								if (la.kind == 203) {
																									goto case 74;
																								} else {
																									if (la.kind == 212) {
																										goto case 69;
																									} else {
																										if (la.kind == 213) {
																											goto case 68;
																										} else {
																											if (la.kind == 223) {
																												goto case 66;
																											} else {
																												if (la.kind == 224) {
																													goto case 65;
																												} else {
																													if (la.kind == 230) {
																														goto case 64;
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
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				Expect(63, la); // "As"
				currentState = 518;
				break;
			}
			case 518: {
				stateStack.Push(519);
				goto case 512;
			}
			case 519: {
				PopContext();
				goto case 505;
			}
			case 520: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 521;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (set[38].Get(la.kind)) {
					currentState = 521;
					break;
				} else {
					PopContext();
					stateStack.Push(522);
					goto case 15;
				}
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(522);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 523;
					break;
				}
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				Expect(137, la); // "Imports"
				currentState = 525;
				break;
			}
			case 525: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 526;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (set[8].Get(la.kind)) {
					currentState = 532;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 528;
						break;
					} else {
						Error(la);
						goto case 527;
					}
				}
			}
			case 527: {
				PopContext();
				goto case 15;
			}
			case 528: {
				stateStack.Push(529);
				goto case 154;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				Expect(20, la); // "="
				currentState = 530;
				break;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				Expect(3, la); // LiteralString
				currentState = 531;
				break;
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 527;
				break;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				if (la.kind == 37) {
					stateStack.Push(532);
					goto case 26;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 533;
						break;
					} else {
						goto case 527;
					}
				}
			}
			case 533: {
				stateStack.Push(527);
				goto case 21;
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				Expect(173, la); // "Option"
				currentState = 535;
				break;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 537;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 536;
						break;
					} else {
						goto case 483;
					}
				}
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				if (la.kind == 213) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 15;
						break;
					} else {
						goto case 483;
					}
				}
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 15;
					break;
				} else {
					goto case 15;
				}
			}
		}

		if (la != null) t = la;
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	public BitArray GetExpectedSet() { return GetExpectedSet(currentState); }
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {1, 256, 1048576, 536871040, 134218240, 436215809, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 536871040, 134218240, 436207617, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 536871040, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 536871040, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 536871040, 134217728, 436207616, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 536871040, 134217728, 436207616, 131200, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564474, 1962934261, 8650975, 1108388124, 81767716, 17272068, -512676304, 4707}),
		new BitArray(new int[] {-940564474, 1962934229, 8650975, 1108388124, 81767716, 17272068, -512676304, 4707}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-62257156, 1174405224, -51646385, -972026621, -1039365982, 17106484, -1707866112, 8257}),
		new BitArray(new int[] {-62257156, 1174405224, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-62257156, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007290364, 1140850720, -51646385, -972026621, -1039365982, 17105952, -1976301568, 8257}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026617, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8385}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64354306, -973078488, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-64354306, 1191182376, -1311153, -546070781, -1022588750, -1593504708, -1631823826, 8901}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66451460, 1191182376, -1314225, -680517885, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1174405160, -51646385, -971993853, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365966, 17105972, -1707866108, 8257}),
		new BitArray(new int[] {1007290364, 1140850720, -51646385, -972010237, -1039365982, 17105952, -1976301568, 8257}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66451460, 1174405160, -51645361, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-65402884, 1174409128, -51646385, -971993853, -1039300446, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-65402884, 1174409128, -51646385, -972026621, -1039300446, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1191182376, -1314225, -680517885, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {-64354306, 1191182376, -1314225, -680485117, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {2097154, 32, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483648, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {5242880, -2147483584, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7, 1157628160, 26477055, -493868100, 680306724, 1006458243, -533262446, 1347}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {721920, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850752, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-2, -33, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537696544, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692448, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {1, 256, 1048576, 537002112, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850688, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850696, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850688, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {0, 256, 1048576, 537002112, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 128, 0}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 67108864, -1040187392, 32, 33554432, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 0, -1133776896, 3, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1191182376, -1314225, -680386813, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66451460, 1174405160, -51645873, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 65536, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 288, 0, 0, 4210688, 0, 0}),
		new BitArray(new int[] {-2, -129, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2097160, -1140850977, -25165904, 493745371, -537696545, -465376387, 2144073343, -1346}),
		new BitArray(new int[] {0, 0, 0, 536871424, 536870912, 448266370, 384, 1280}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0})

	};

} // end Parser


}