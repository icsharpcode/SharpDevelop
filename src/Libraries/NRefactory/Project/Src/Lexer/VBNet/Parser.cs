using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 49;
	const int endOfStatementTerminatorAndBlock = 226;
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
			case 63:
			case 227:
			case 459:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 153:
			case 159:
			case 166:
			case 204:
			case 208:
			case 246:
			case 347:
			case 359:
			case 408:
			case 449:
			case 457:
			case 465:
			case 489:
			case 536:
			case 551:
			case 621:
				return set[6];
			case 10:
			case 490:
			case 491:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 11:
			case 18:
			case 19:
			case 32:
			case 219:
			case 222:
			case 223:
			case 233:
			case 247:
			case 251:
			case 272:
			case 288:
			case 299:
			case 302:
			case 308:
			case 313:
			case 322:
			case 323:
			case 336:
			case 344:
			case 368:
			case 467:
			case 483:
			case 492:
			case 501:
			case 518:
			case 522:
			case 531:
			case 534:
			case 560:
			case 570:
			case 575:
			case 599:
			case 620:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 12:
			case 13:
				return set[7];
			case 14:
			case 15:
				return set[8];
			case 16:
			case 220:
			case 234:
			case 249:
			case 303:
			case 345:
			case 394:
			case 499:
			case 519:
			case 532:
			case 571:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 17:
			case 463:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 20:
			case 333:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 21:
			case 22:
				return set[9];
			case 23:
			case 603:
				return set[10];
			case 24:
				return set[11];
			case 25:
				return set[12];
			case 26:
			case 27:
			case 119:
			case 176:
			case 177:
			case 228:
			case 384:
			case 385:
			case 400:
			case 401:
			case 402:
			case 476:
			case 477:
			case 511:
			case 512:
			case 566:
			case 567:
			case 613:
			case 614:
				return set[13];
			case 28:
			case 29:
			case 450:
			case 458:
			case 478:
			case 479:
			case 556:
			case 568:
			case 569:
				return set[14];
			case 30:
			case 163:
			case 180:
			case 258:
			case 282:
			case 353:
			case 366:
			case 380:
			case 438:
			case 446:
			case 481:
			case 545:
			case 558:
			case 573:
			case 586:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					return a;
				}
			case 31:
			case 33:
			case 123:
			case 132:
			case 148:
			case 164:
			case 181:
			case 259:
			case 283:
			case 354:
			case 356:
			case 358:
			case 367:
			case 381:
			case 411:
			case 453:
			case 473:
			case 482:
			case 540:
			case 546:
			case 559:
			case 574:
			case 587:
			case 592:
			case 595:
			case 598:
			case 605:
			case 608:
			case 626:
				return set[15];
			case 34:
			case 37:
				return set[16];
			case 35:
				return set[17];
			case 36:
			case 69:
			case 73:
			case 128:
			case 339:
			case 414:
				return set[18];
			case 38:
			case 138:
			case 145:
			case 149:
			case 213:
			case 388:
			case 407:
			case 410:
			case 513:
			case 514:
			case 528:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 39:
			case 40:
			case 130:
			case 131:
				return set[19];
			case 41:
			case 216:
			case 364:
			case 391:
			case 409:
			case 425:
			case 456:
			case 462:
			case 486:
			case 516:
			case 530:
			case 544:
			case 563:
			case 578:
			case 602:
			case 612:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 42:
			case 43:
			case 46:
			case 47:
			case 419:
			case 420:
				return set[20];
			case 44:
				return set[21];
			case 45:
			case 140:
			case 147:
			case 342:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 48:
			case 133:
			case 142:
			case 363:
			case 365:
			case 370:
			case 378:
			case 418:
			case 422:
			case 432:
			case 440:
			case 448:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 49:
			case 50:
			case 52:
			case 53:
			case 54:
			case 57:
			case 71:
			case 121:
			case 139:
			case 141:
			case 143:
			case 146:
			case 155:
			case 157:
			case 199:
			case 232:
			case 236:
			case 238:
			case 239:
			case 255:
			case 271:
			case 276:
			case 286:
			case 292:
			case 294:
			case 298:
			case 301:
			case 307:
			case 318:
			case 320:
			case 326:
			case 341:
			case 343:
			case 379:
			case 404:
			case 416:
			case 417:
			case 472:
			case 585:
				return set[22];
			case 51:
			case 55:
			case 64:
				return set[23];
			case 56:
			case 65:
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 58:
			case 72:
			case 443:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 59:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 60:
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 61:
				return set[24];
			case 62:
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 67:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 70:
			case 179:
			case 182:
			case 183:
			case 285:
			case 622:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 75:
			case 304:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 79:
			case 250:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 80:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 83:
			case 395:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 87:
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 92:
			case 266:
			case 273:
			case 289:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 95:
			case 186:
			case 191:
			case 193:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 96:
			case 188:
			case 192:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 99:
			case 221:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 100:
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 103:
			case 156:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 104:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 107:
			case 523:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 108:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 110:
			case 169:
			case 198:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 111:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 116:
			case 210:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 118:
				return set[25];
			case 120:
				return set[26];
			case 122:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 124:
				return set[27];
			case 125:
				return set[28];
			case 126:
			case 127:
			case 412:
			case 413:
				return set[29];
			case 129:
				return set[30];
			case 134:
			case 135:
			case 269:
			case 278:
				return set[31];
			case 136:
				return set[32];
			case 137:
			case 325:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 144:
				return set[33];
			case 150:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 151:
			case 152:
				return set[34];
			case 154:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 158:
			case 173:
			case 190:
			case 195:
			case 201:
			case 203:
			case 207:
			case 209:
				return set[35];
			case 160:
			case 161:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 162:
			case 165:
			case 270:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 167:
			case 168:
			case 170:
			case 172:
			case 174:
			case 175:
			case 184:
			case 189:
			case 194:
			case 202:
			case 206:
				return set[36];
			case 171:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 178:
				return set[37];
			case 185:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 187:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 196:
			case 197:
				return set[38];
			case 200:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 205:
				return set[39];
			case 212:
			case 475:
			case 550:
			case 565:
			case 572:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 214:
			case 215:
			case 389:
			case 390:
			case 454:
			case 455:
			case 460:
			case 461:
			case 484:
			case 485:
			case 542:
			case 543:
			case 561:
			case 562:
				return set[40];
			case 217:
			case 218:
				return set[41];
			case 224:
			case 225:
				return set[42];
			case 226:
				return set[43];
			case 229:
				return set[44];
			case 230:
			case 231:
			case 331:
				return set[45];
			case 235:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 237:
			case 277:
			case 293:
				return set[46];
			case 240:
			case 241:
			case 274:
			case 275:
			case 290:
			case 291:
				return set[47];
			case 242:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 243:
				return set[48];
			case 244:
			case 262:
				return set[49];
			case 245:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 248:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 252:
			case 253:
				return set[50];
			case 254:
			case 260:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 256:
			case 257:
				return set[51];
			case 261:
				return set[52];
			case 263:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 264:
			case 265:
				return set[53];
			case 267:
			case 268:
				return set[54];
			case 279:
			case 280:
				return set[55];
			case 281:
				return set[56];
			case 284:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 287:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 295:
				return set[57];
			case 296:
			case 300:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 297:
				return set[58];
			case 305:
			case 306:
				return set[59];
			case 309:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 311:
			case 312:
				return set[60];
			case 314:
			case 315:
				return set[61];
			case 316:
			case 541:
			case 593:
			case 594:
			case 596:
			case 606:
			case 607:
			case 609:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 317:
			case 319:
				return set[62];
			case 321:
			case 327:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 324:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 328:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 329:
			case 330:
			case 334:
			case 335:
			case 392:
			case 393:
				return set[63];
			case 332:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 337:
			case 338:
				return set[64];
			case 340:
				return set[65];
			case 346:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 348:
			case 349:
			case 360:
			case 361:
				return set[66];
			case 350:
			case 362:
				return set[67];
			case 351:
				return set[68];
			case 352:
			case 357:
				return set[69];
			case 355:
				return set[70];
			case 369:
			case 371:
			case 372:
			case 515:
			case 529:
				return set[71];
			case 373:
			case 374:
				return set[72];
			case 375:
			case 376:
				return set[73];
			case 377:
			case 382:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 383:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 386:
			case 387:
				return set[74];
			case 396:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 397:
				return set[75];
			case 398:
				return set[76];
			case 399:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 403:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 405:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 406:
				return set[77];
			case 415:
				return set[78];
			case 421:
				return set[79];
			case 423:
			case 424:
			case 576:
			case 577:
				return set[80];
			case 426:
			case 427:
			case 428:
			case 433:
			case 434:
			case 579:
			case 601:
			case 611:
				return set[81];
			case 429:
			case 435:
			case 445:
				return set[82];
			case 430:
			case 431:
			case 436:
			case 437:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 439:
			case 441:
			case 447:
				return set[83];
			case 442:
			case 444:
				return set[84];
			case 451:
			case 466:
			case 480:
			case 517:
			case 557:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 452:
			case 521:
				return set[85];
			case 464:
			case 469:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 468:
				return set[86];
			case 470:
				return set[87];
			case 471:
			case 584:
			case 588:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 474:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 487:
			case 488:
			case 500:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 493:
			case 494:
				return set[88];
			case 495:
			case 496:
				return set[89];
			case 497:
			case 498:
			case 509:
				return set[90];
			case 502:
				return set[91];
			case 503:
			case 504:
				return set[92];
			case 505:
			case 506:
			case 590:
				return set[93];
			case 507:
				return set[94];
			case 508:
				return set[95];
			case 510:
			case 520:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 524:
			case 526:
			case 535:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 525:
				return set[96];
			case 527:
				return set[97];
			case 533:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 537:
			case 538:
				return set[98];
			case 539:
			case 547:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 548:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 549:
				return set[99];
			case 552:
			case 553:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 554:
			case 564:
			case 623:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 555:
				return set[100];
			case 580:
				return set[101];
			case 581:
			case 589:
				return set[102];
			case 582:
			case 583:
				return set[103];
			case 591:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 597:
			case 604:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 600:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 615:
				return set[104];
			case 616:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 617:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 618:
			case 619:
				return set[105];
			case 624:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 625:
				return set[106];
			case 627:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 628:
				return set[107];
			case 629:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 630:
				return set[108];
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
	bool identifierExpected = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	bool isMissingModifier = false;
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
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 627;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					PushContext(Context.Importable, la, t);
					goto case 617;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 383;
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
					currentState = 613;
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
					goto case 383;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[109].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 487;
					} else {
						if (la.kind == 103) {
							currentState = 475;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 465;
								break;
							} else {
								if (la.kind == 142) {
									currentState = 9;
									break;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				stateStack.Push(10);
				goto case 166;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (la.kind == 37) {
					currentState = 610;
					break;
				} else {
					goto case 11;
				}
			}
			case 11: {
				stateStack.Push(12);
				goto case 18;
			}
			case 12: {
				isMissingModifier = true;
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 140) {
					PushContext(Context.Type, la, t);
					goto case 604;
				} else {
					goto case 14;
				}
			}
			case 14: {
				isMissingModifier = true;
				goto case 15;
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (set[9].Get(la.kind)) {
					goto case 21;
				} else {
					isMissingModifier = false;
					goto case 16;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				Expect(113, la); // "End"
				currentState = 17;
				break;
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				Expect(142, la); // "Interface"
				currentState = 18;
				break;
			}
			case 18: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 19;
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 21: {
				isMissingModifier = true;
				goto case 22;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 40) {
					stateStack.Push(21);
					goto case 383;
				} else {
					isMissingModifier = true;
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (set[110].Get(la.kind)) {
					currentState = 603;
					break;
				} else {
					isMissingModifier = false;
					goto case 24;
				}
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(14);
					goto case 487;
				} else {
					if (la.kind == 103) {
						stateStack.Push(14);
						goto case 474;
					} else {
						if (la.kind == 115) {
							stateStack.Push(14);
							goto case 464;
						} else {
							if (la.kind == 142) {
								stateStack.Push(14);
								goto case 463;
							} else {
								if (set[12].Get(la.kind)) {
									stateStack.Push(14);
									goto case 25;
								} else {
									Error(la);
									goto case 14;
								}
							}
						}
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 119) {
					currentState = 457;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 449;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 26;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 26: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				currentState = 28;
				break;
			}
			case 28: {
				PopContext();
				goto case 29;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 37) {
					currentState = 423;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 30;
					} else {
						goto case 18;
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				Expect(63, la); // "As"
				currentState = 31;
				break;
			}
			case 31: {
				stateStack.Push(32);
				goto case 33;
			}
			case 32: {
				PopContext();
				goto case 18;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 130) {
					currentState = 34;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 34;
						break;
					} else {
						if (set[111].Get(la.kind)) {
							currentState = 34;
							break;
						} else {
							Error(la);
							goto case 34;
						}
					}
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 37) {
					stateStack.Push(34);
					goto case 38;
				} else {
					goto case 35;
				}
			}
			case 35: {
				if (la == null) { currentState = 35; break; }
				if (la.kind == 26) {
					currentState = 36;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 36: {
				stateStack.Push(37);
				goto case 73;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (la.kind == 37) {
					stateStack.Push(37);
					goto case 38;
				} else {
					goto case 35;
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				Expect(37, la); // "("
				currentState = 39;
				break;
			}
			case 39: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 40;
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (la.kind == 169) {
					currentState = 421;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 42;
					} else {
						Error(la);
						goto case 41;
					}
				}
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 42: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 43;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(41);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 44;
				} else {
					goto case 41;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (set[22].Get(la.kind)) {
					goto case 417;
				} else {
					if (la.kind == 22) {
						goto case 45;
					} else {
						goto case 6;
					}
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				currentState = 46;
				break;
			}
			case 46: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 47;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(48);
					goto case 49;
				} else {
					goto case 48;
				}
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (la.kind == 22) {
					goto case 45;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 49: {
				PushContext(Context.Expression, la, t);
				goto case 50;
			}
			case 50: {
				stateStack.Push(51);
				goto case 52;
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (set[112].Get(la.kind)) {
					currentState = 50;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 52: {
				PushContext(Context.Expression, la, t);
				goto case 53;
			}
			case 53: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 54;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (set[113].Get(la.kind)) {
					currentState = 53;
					break;
				} else {
					if (set[31].Get(la.kind)) {
						stateStack.Push(124);
						goto case 134;
					} else {
						if (la.kind == 220) {
							currentState = 121;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(55);
								PushContext(Context.ObjectCreation, la, t);
								goto case 60;
							} else {
								if (la.kind == 35) {
									stateStack.Push(55);
									goto case 56;
								} else {
									Error(la);
									goto case 55;
								}
							}
						}
					}
				}
			}
			case 55: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				Expect(35, la); // "{"
				currentState = 57;
				break;
			}
			case 57: {
				stateStack.Push(58);
				goto case 49;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.kind == 22) {
					currentState = 57;
					break;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				Expect(162, la); // "New"
				currentState = 61;
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(118);
					goto case 33;
				} else {
					goto case 62;
				}
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				if (la.kind == 233) {
					currentState = 65;
					break;
				} else {
					goto case 63;
				}
			}
			case 63: {
				Error(la);
				goto case 64;
			}
			case 64: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 65: {
				stateStack.Push(64);
				goto case 66;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(35, la); // "{"
				currentState = 67;
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				if (la.kind == 147) {
					currentState = 68;
					break;
				} else {
					goto case 68;
				}
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				Expect(26, la); // "."
				currentState = 69;
				break;
			}
			case 69: {
				stateStack.Push(70);
				goto case 73;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				Expect(20, la); // "="
				currentState = 71;
				break;
			}
			case 71: {
				stateStack.Push(72);
				goto case 49;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				if (la.kind == 22) {
					currentState = 67;
					break;
				} else {
					goto case 59;
				}
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (la.kind == 2) {
					goto case 117;
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
								goto case 116;
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
												goto case 115;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 114;
													} else {
														if (la.kind == 65) {
															goto case 113;
														} else {
															if (la.kind == 66) {
																goto case 112;
															} else {
																if (la.kind == 67) {
																	goto case 111;
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
																				goto case 110;
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
																																		goto case 109;
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
																																					goto case 108;
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
																																																goto case 107;
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
																																																						goto case 106;
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
																																																									goto case 105;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 104;
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
																																																																		goto case 103;
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
																																																																							goto case 102;
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
																																																																										goto case 101;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 100;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 99;
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
																																																																																			goto case 98;
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
																																																																																									goto case 97;
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
																																																																																													goto case 96;
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
																																																																																																goto case 95;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 94;
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
																																																																																																																goto case 93;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 92;
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
																																																																																																																								goto case 91;
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
																																																																																																																														goto case 90;
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
																																																																																																																																						goto case 89;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 88;
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
																																																																																																																																																			goto case 87;
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
																																																																																																																																																									goto case 86;
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
																																																																																																																																																												goto case 85;
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
																																																																																																																																																															goto case 84;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 83;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 82;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 81;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 80;
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
																																																																																																																																																																								goto case 79;
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
																																																																																																																																																																													goto case 78;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 77;
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
																																																																																																																																																																																				goto case 76;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 75;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 74;
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
				currentState = stateStack.Pop();
				break;
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 119;
						break;
					} else {
						goto case 62;
					}
				} else {
					goto case 64;
				}
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				if (la.kind == 35) {
					stateStack.Push(64);
					goto case 56;
				} else {
					if (set[26].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 63;
					}
				}
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				currentState = 64;
				break;
			}
			case 121: {
				stateStack.Push(122);
				goto case 52;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				Expect(144, la); // "Is"
				currentState = 123;
				break;
			}
			case 123: {
				stateStack.Push(55);
				goto case 33;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				if (set[28].Get(la.kind)) {
					stateStack.Push(124);
					goto case 125;
				} else {
					goto case 55;
				}
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (la.kind == 37) {
					currentState = 130;
					break;
				} else {
					if (set[114].Get(la.kind)) {
						currentState = 126;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 126: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 127;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (la.kind == 10) {
					currentState = 128;
					break;
				} else {
					goto case 128;
				}
			}
			case 128: {
				stateStack.Push(129);
				goto case 73;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 130: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 131;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				if (la.kind == 169) {
					currentState = 132;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 42;
					} else {
						goto case 6;
					}
				}
			}
			case 132: {
				stateStack.Push(133);
				goto case 33;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 22) {
					currentState = 132;
					break;
				} else {
					goto case 41;
				}
			}
			case 134: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 135;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (set[115].Get(la.kind)) {
					currentState = 136;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 416;
						break;
					} else {
						if (set[116].Get(la.kind)) {
							currentState = 136;
							break;
						} else {
							if (set[114].Get(la.kind)) {
								currentState = 412;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 410;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 407;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(136);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 396;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(136);
												goto case 212;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(136);
													PushContext(Context.Query, la, t);
													goto case 150;
												} else {
													if (set[33].Get(la.kind)) {
														stateStack.Push(136);
														goto case 144;
													} else {
														if (la.kind == 135) {
															stateStack.Push(136);
															goto case 137;
														} else {
															Error(la);
															goto case 136;
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
			case 136: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				Expect(135, la); // "If"
				currentState = 138;
				break;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				Expect(37, la); // "("
				currentState = 139;
				break;
			}
			case 139: {
				stateStack.Push(140);
				goto case 49;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(22, la); // ","
				currentState = 141;
				break;
			}
			case 141: {
				stateStack.Push(142);
				goto case 49;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				if (la.kind == 22) {
					currentState = 143;
					break;
				} else {
					goto case 41;
				}
			}
			case 143: {
				stateStack.Push(41);
				goto case 49;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (set[117].Get(la.kind)) {
					currentState = 149;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 145;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(37, la); // "("
				currentState = 146;
				break;
			}
			case 146: {
				stateStack.Push(147);
				goto case 49;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(22, la); // ","
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(41);
				goto case 33;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(37, la); // "("
				currentState = 143;
				break;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 126) {
					stateStack.Push(151);
					goto case 211;
				} else {
					if (la.kind == 58) {
						stateStack.Push(151);
						goto case 210;
					} else {
						Error(la);
						goto case 151;
					}
				}
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(151);
					goto case 152;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.kind == 126) {
					currentState = 208;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 204;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 202;
							break;
						} else {
							if (la.kind == 107) {
								goto case 105;
							} else {
								if (la.kind == 230) {
									currentState = 49;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 198;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 196;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 194;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 167;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 153;
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
			case 153: {
				stateStack.Push(154);
				goto case 159;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				Expect(171, la); // "On"
				currentState = 155;
				break;
			}
			case 155: {
				stateStack.Push(156);
				goto case 49;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(116, la); // "Equals"
				currentState = 157;
				break;
			}
			case 157: {
				stateStack.Push(158);
				goto case 49;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (la.kind == 22) {
					currentState = 155;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 159: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(160);
				goto case 166;
			}
			case 160: {
				PopContext();
				goto case 161;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 163;
				} else {
					goto case 162;
				}
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				Expect(138, la); // "In"
				currentState = 49;
				break;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				Expect(63, la); // "As"
				currentState = 164;
				break;
			}
			case 164: {
				stateStack.Push(165);
				goto case 33;
			}
			case 165: {
				PopContext();
				goto case 162;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (set[102].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 107;
					} else {
						goto case 6;
					}
				}
			}
			case 167: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 168;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.kind == 146) {
					goto case 186;
				} else {
					if (set[36].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 170;
							break;
						} else {
							if (set[36].Get(la.kind)) {
								goto case 184;
							} else {
								Error(la);
								goto case 169;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				Expect(70, la); // "By"
				currentState = 170;
				break;
			}
			case 170: {
				stateStack.Push(171);
				goto case 174;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.kind == 22) {
					currentState = 170;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 172;
					break;
				}
			}
			case 172: {
				stateStack.Push(173);
				goto case 174;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				if (la.kind == 22) {
					currentState = 172;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 174: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 175;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(176);
					goto case 166;
				} else {
					goto case 49;
				}
			}
			case 176: {
				PopContext();
				goto case 177;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 180;
				} else {
					if (la.kind == 20) {
						goto case 179;
					} else {
						if (set[37].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 49;
						}
					}
				}
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				currentState = 49;
				break;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				currentState = 49;
				break;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(63, la); // "As"
				currentState = 181;
				break;
			}
			case 181: {
				stateStack.Push(182);
				goto case 33;
			}
			case 182: {
				PopContext();
				goto case 183;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				Expect(20, la); // "="
				currentState = 49;
				break;
			}
			case 184: {
				stateStack.Push(185);
				goto case 174;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 22) {
					currentState = 184;
					break;
				} else {
					goto case 169;
				}
			}
			case 186: {
				stateStack.Push(187);
				goto case 193;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 191;
						break;
					} else {
						if (la.kind == 146) {
							goto case 186;
						} else {
							Error(la);
							goto case 187;
						}
					}
				} else {
					goto case 188;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(143, la); // "Into"
				currentState = 189;
				break;
			}
			case 189: {
				stateStack.Push(190);
				goto case 174;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 22) {
					currentState = 189;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 191: {
				stateStack.Push(192);
				goto case 193;
			}
			case 192: {
				stateStack.Push(187);
				goto case 188;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(146, la); // "Join"
				currentState = 153;
				break;
			}
			case 194: {
				stateStack.Push(195);
				goto case 174;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 197;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.kind == 231) {
					currentState = 49;
					break;
				} else {
					goto case 49;
				}
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(70, la); // "By"
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(200);
				goto case 49;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 64) {
					currentState = 201;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 201;
						break;
					} else {
						Error(la);
						goto case 201;
					}
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (la.kind == 22) {
					currentState = 199;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 202: {
				stateStack.Push(203);
				goto case 174;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 22) {
					currentState = 202;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 159;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(205);
					goto case 152;
				} else {
					Expect(143, la); // "Into"
					currentState = 206;
					break;
				}
			}
			case 206: {
				stateStack.Push(207);
				goto case 174;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (la.kind == 22) {
					currentState = 206;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 208: {
				stateStack.Push(209);
				goto case 159;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 22) {
					currentState = 208;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				Expect(58, la); // "Aggregate"
				currentState = 204;
				break;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(126, la); // "From"
				currentState = 208;
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 210) {
					currentState = 388;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 213;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				Expect(37, la); // "("
				currentState = 214;
				break;
			}
			case 214: {
				SetIdentifierExpected(la);
				goto case 215;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(216);
					goto case 369;
				} else {
					goto case 216;
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(38, la); // ")"
				currentState = 217;
				break;
			}
			case 217: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							PushContext(Context.Type, la, t);
							goto case 366;
						} else {
							goto case 219;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 219: {
				stateStack.Push(220);
				goto case 222;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				Expect(113, la); // "End"
				currentState = 221;
				break;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 222: {
				PushContext(Context.Body, la, t);
				goto case 223;
			}
			case 223: {
				stateStack.Push(224);
				goto case 18;
			}
			case 224: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 225;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (set[118].Get(la.kind)) {
					if (set[63].Get(la.kind)) {
						if (set[45].Get(la.kind)) {
							stateStack.Push(223);
							goto case 230;
						} else {
							goto case 223;
						}
					} else {
						if (la.kind == 113) {
							currentState = 228;
							break;
						} else {
							goto case 227;
						}
					}
				} else {
					goto case 226;
				}
			}
			case 226: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 227: {
				Error(la);
				goto case 224;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 223;
				} else {
					if (set[44].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 227;
					}
				}
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				currentState = 224;
				break;
			}
			case 230: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 231;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 347;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 343;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 341;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 339;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 320;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 305;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 301;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 295;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 267;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 263;
																break;
															} else {
																goto case 263;
															}
														} else {
															if (la.kind == 194) {
																currentState = 261;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 240;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 247;
																break;
															} else {
																if (set[119].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 244;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 243;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 242;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 85;
																				} else {
																					if (la.kind == 195) {
																						currentState = 240;
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
																		currentState = 238;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 236;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 232;
																				break;
																			} else {
																				if (set[120].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 49;
																						break;
																					} else {
																						goto case 49;
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
			case 232: {
				stateStack.Push(233);
				goto case 49;
			}
			case 233: {
				stateStack.Push(234);
				goto case 222;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(113, la); // "End"
				currentState = 235;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				stateStack.Push(237);
				goto case 49;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 22) {
					currentState = 236;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 238: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 239;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (la.kind == 184) {
					currentState = 49;
					break;
				} else {
					goto case 49;
				}
			}
			case 240: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (la.kind == 108) {
					goto case 104;
				} else {
					if (la.kind == 124) {
						goto case 101;
					} else {
						if (la.kind == 231) {
							goto case 75;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 108) {
					goto case 104;
				} else {
					if (la.kind == 124) {
						goto case 101;
					} else {
						if (la.kind == 231) {
							goto case 75;
						} else {
							if (la.kind == 197) {
								goto case 87;
							} else {
								if (la.kind == 210) {
									goto case 83;
								} else {
									if (la.kind == 127) {
										goto case 99;
									} else {
										if (la.kind == 186) {
											goto case 88;
										} else {
											if (la.kind == 218) {
												goto case 79;
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
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (set[6].Get(la.kind)) {
					goto case 246;
				} else {
					if (la.kind == 5) {
						goto case 245;
					} else {
						goto case 6;
					}
				}
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				stateStack.Push(248);
				goto case 222;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (la.kind == 75) {
					currentState = 252;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 251;
						break;
					} else {
						goto case 249;
					}
				}
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				Expect(113, la); // "End"
				currentState = 250;
				break;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 251: {
				stateStack.Push(249);
				goto case 222;
			}
			case 252: {
				SetIdentifierExpected(la);
				goto case 253;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(256);
					goto case 166;
				} else {
					goto case 254;
				}
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 229) {
					currentState = 255;
					break;
				} else {
					goto case 247;
				}
			}
			case 255: {
				stateStack.Push(247);
				goto case 49;
			}
			case 256: {
				PopContext();
				goto case 257;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 258;
				} else {
					goto case 254;
				}
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(63, la); // "As"
				currentState = 259;
				break;
			}
			case 259: {
				stateStack.Push(260);
				goto case 33;
			}
			case 260: {
				PopContext();
				goto case 254;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (la.kind == 163) {
					goto case 92;
				} else {
					goto case 262;
				}
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (la.kind == 5) {
					goto case 245;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				Expect(118, la); // "Error"
				currentState = 264;
				break;
			}
			case 264: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 265;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 132) {
						currentState = 262;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 266;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(285);
					goto case 278;
				} else {
					if (la.kind == 110) {
						currentState = 269;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 269: {
				stateStack.Push(270);
				goto case 278;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				Expect(138, la); // "In"
				currentState = 271;
				break;
			}
			case 271: {
				stateStack.Push(272);
				goto case 49;
			}
			case 272: {
				stateStack.Push(273);
				goto case 222;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				Expect(163, la); // "Next"
				currentState = 274;
				break;
			}
			case 274: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 275;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				if (set[22].Get(la.kind)) {
					goto case 276;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 276: {
				stateStack.Push(277);
				goto case 49;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (la.kind == 22) {
					currentState = 276;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 278: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(279);
				goto case 134;
			}
			case 279: {
				PopContext();
				goto case 280;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 33) {
					currentState = 281;
					break;
				} else {
					goto case 281;
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (set[28].Get(la.kind)) {
					stateStack.Push(281);
					goto case 125;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 282;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(63, la); // "As"
				currentState = 283;
				break;
			}
			case 283: {
				stateStack.Push(284);
				goto case 33;
			}
			case 284: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				Expect(20, la); // "="
				currentState = 286;
				break;
			}
			case 286: {
				stateStack.Push(287);
				goto case 49;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 205) {
					currentState = 294;
					break;
				} else {
					goto case 288;
				}
			}
			case 288: {
				stateStack.Push(289);
				goto case 222;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				Expect(163, la); // "Next"
				currentState = 290;
				break;
			}
			case 290: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 291;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (set[22].Get(la.kind)) {
					goto case 292;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 292: {
				stateStack.Push(293);
				goto case 49;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 22) {
					currentState = 292;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 294: {
				stateStack.Push(288);
				goto case 49;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 298;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(296);
						goto case 222;
					} else {
						goto case 6;
					}
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(152, la); // "Loop"
				currentState = 297;
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 49;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 298: {
				stateStack.Push(299);
				goto case 49;
			}
			case 299: {
				stateStack.Push(300);
				goto case 222;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 301: {
				stateStack.Push(302);
				goto case 49;
			}
			case 302: {
				stateStack.Push(303);
				goto case 222;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				Expect(113, la); // "End"
				currentState = 304;
				break;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 305: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 306;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (la.kind == 74) {
					currentState = 307;
					break;
				} else {
					goto case 307;
				}
			}
			case 307: {
				stateStack.Push(308);
				goto case 49;
			}
			case 308: {
				stateStack.Push(309);
				goto case 18;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.kind == 74) {
					currentState = 311;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 310;
					break;
				}
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 311: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 312;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 111) {
					currentState = 313;
					break;
				} else {
					if (set[61].Get(la.kind)) {
						goto case 314;
					} else {
						Error(la);
						goto case 313;
					}
				}
			}
			case 313: {
				stateStack.Push(309);
				goto case 222;
			}
			case 314: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 315;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (set[121].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 317;
						break;
					} else {
						goto case 317;
					}
				} else {
					if (set[22].Get(la.kind)) {
						stateStack.Push(316);
						goto case 49;
					} else {
						Error(la);
						goto case 316;
					}
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (la.kind == 22) {
					currentState = 314;
					break;
				} else {
					goto case 313;
				}
			}
			case 317: {
				stateStack.Push(318);
				goto case 319;
			}
			case 318: {
				stateStack.Push(316);
				goto case 52;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
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
			case 320: {
				stateStack.Push(321);
				goto case 49;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 214) {
					currentState = 329;
					break;
				} else {
					goto case 322;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 323;
				} else {
					goto case 6;
				}
			}
			case 323: {
				stateStack.Push(324);
				goto case 222;
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 328;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 326;
							break;
						} else {
							Error(la);
							goto case 323;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 325;
					break;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 326: {
				stateStack.Push(327);
				goto case 49;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 214) {
					currentState = 323;
					break;
				} else {
					goto case 323;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 135) {
					currentState = 326;
					break;
				} else {
					goto case 323;
				}
			}
			case 329: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 330;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (set[45].Get(la.kind)) {
					goto case 331;
				} else {
					goto case 322;
				}
			}
			case 331: {
				stateStack.Push(332);
				goto case 230;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 21) {
					currentState = 337;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 334;
						break;
					} else {
						goto case 333;
					}
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 334: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 335;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (set[45].Get(la.kind)) {
					stateStack.Push(336);
					goto case 230;
				} else {
					goto case 336;
				}
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 21) {
					currentState = 334;
					break;
				} else {
					goto case 333;
				}
			}
			case 337: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 338;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (set[45].Get(la.kind)) {
					goto case 331;
				} else {
					goto case 332;
				}
			}
			case 339: {
				stateStack.Push(340);
				goto case 73;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 37) {
					currentState = 42;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 341: {
				stateStack.Push(342);
				goto case 49;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				Expect(22, la); // ","
				currentState = 49;
				break;
			}
			case 343: {
				stateStack.Push(344);
				goto case 49;
			}
			case 344: {
				stateStack.Push(345);
				goto case 222;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				Expect(113, la); // "End"
				currentState = 346;
				break;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 233) {
					goto case 74;
				} else {
					if (la.kind == 211) {
						goto case 82;
					} else {
						goto case 6;
					}
				}
			}
			case 347: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(348);
				goto case 166;
			}
			case 348: {
				PopContext();
				goto case 349;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (la.kind == 33) {
					currentState = 350;
					break;
				} else {
					goto case 350;
				}
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 37) {
					currentState = 365;
					break;
				} else {
					goto case 351;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 22) {
					currentState = 359;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 353;
					} else {
						goto case 352;
					}
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 20) {
					goto case 179;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				Expect(63, la); // "As"
				currentState = 354;
				break;
			}
			case 354: {
				stateStack.Push(355);
				goto case 33;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 162) {
					currentState = 358;
					break;
				} else {
					goto case 356;
				}
			}
			case 356: {
				stateStack.Push(357);
				goto case 33;
			}
			case 357: {
				if (CurrentBlock.context == Context.ObjectCreation)
					PopContext();
				PopContext();

				goto case 352;
			}
			case 358: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 356;
			}
			case 359: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(360);
				goto case 166;
			}
			case 360: {
				PopContext();
				goto case 361;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 33) {
					currentState = 362;
					break;
				} else {
					goto case 362;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 37) {
					currentState = 363;
					break;
				} else {
					goto case 351;
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 22) {
					currentState = 363;
					break;
				} else {
					goto case 364;
				}
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				Expect(38, la); // ")"
				currentState = 351;
				break;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 22) {
					currentState = 365;
					break;
				} else {
					goto case 364;
				}
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				Expect(63, la); // "As"
				currentState = 367;
				break;
			}
			case 367: {
				stateStack.Push(368);
				goto case 33;
			}
			case 368: {
				PopContext();
				goto case 219;
			}
			case 369: {
				stateStack.Push(370);
				PushContext(Context.Parameter, la, t);
				goto case 371;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 22) {
					currentState = 369;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 371: {
				SetIdentifierExpected(la);
				goto case 372;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 40) {
					stateStack.Push(371);
					goto case 383;
				} else {
					goto case 373;
				}
			}
			case 373: {
				SetIdentifierExpected(la);
				goto case 374;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (set[122].Get(la.kind)) {
					currentState = 373;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(375);
					goto case 166;
				}
			}
			case 375: {
				PopContext();
				goto case 376;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 380;
				} else {
					goto case 377;
				}
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 20) {
					currentState = 379;
					break;
				} else {
					goto case 378;
				}
			}
			case 378: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 379: {
				stateStack.Push(378);
				goto case 49;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				Expect(63, la); // "As"
				currentState = 381;
				break;
			}
			case 381: {
				stateStack.Push(382);
				goto case 33;
			}
			case 382: {
				PopContext();
				goto case 377;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				Expect(40, la); // "<"
				currentState = 384;
				break;
			}
			case 384: {
				PushContext(Context.Attribute, la, t);
				goto case 385;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (set[123].Get(la.kind)) {
					currentState = 385;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 386;
					break;
				}
			}
			case 386: {
				PopContext();
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(37, la); // "("
				currentState = 389;
				break;
			}
			case 389: {
				SetIdentifierExpected(la);
				goto case 390;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(391);
					goto case 369;
				} else {
					goto case 391;
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				Expect(38, la); // ")"
				currentState = 392;
				break;
			}
			case 392: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (set[45].Get(la.kind)) {
					goto case 230;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(394);
						goto case 222;
					} else {
						goto case 6;
					}
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				Expect(113, la); // "End"
				currentState = 395;
				break;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 406;
					break;
				} else {
					stateStack.Push(397);
					goto case 399;
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (la.kind == 17) {
					currentState = 398;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 16) {
					currentState = 397;
					break;
				} else {
					goto case 397;
				}
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 400;
				break;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (set[124].Get(la.kind)) {
					if (set[125].Get(la.kind)) {
						currentState = 400;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(400);
							goto case 403;
						} else {
							Error(la);
							goto case 400;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 401;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (set[126].Get(la.kind)) {
					if (set[127].Get(la.kind)) {
						currentState = 401;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(401);
							goto case 403;
						} else {
							if (la.kind == 10) {
								stateStack.Push(401);
								goto case 399;
							} else {
								Error(la);
								goto case 401;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 402;
					break;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (set[128].Get(la.kind)) {
					if (set[129].Get(la.kind)) {
						currentState = 402;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(402);
							goto case 403;
						} else {
							Error(la);
							goto case 402;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 404;
				break;
			}
			case 404: {
				stateStack.Push(405);
				goto case 49;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 16) {
					currentState = 396;
					break;
				} else {
					goto case 396;
				}
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				Expect(37, la); // "("
				currentState = 408;
				break;
			}
			case 408: {
				readXmlIdentifier = true;
				stateStack.Push(409);
				goto case 166;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				Expect(38, la); // ")"
				currentState = 136;
				break;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				Expect(37, la); // "("
				currentState = 411;
				break;
			}
			case 411: {
				stateStack.Push(409);
				goto case 33;
			}
			case 412: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 413;
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				if (la.kind == 10) {
					currentState = 414;
					break;
				} else {
					goto case 414;
				}
			}
			case 414: {
				stateStack.Push(415);
				goto case 73;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 11) {
					currentState = 136;
					break;
				} else {
					goto case 136;
				}
			}
			case 416: {
				stateStack.Push(409);
				goto case 49;
			}
			case 417: {
				stateStack.Push(418);
				goto case 49;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 22) {
					currentState = 419;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 419: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 420;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (set[22].Get(la.kind)) {
					goto case 417;
				} else {
					goto case 418;
				}
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(422);
					goto case 33;
				} else {
					goto case 422;
				}
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (la.kind == 22) {
					currentState = 421;
					break;
				} else {
					goto case 41;
				}
			}
			case 423: {
				SetIdentifierExpected(la);
				goto case 424;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (set[130].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 426;
						break;
					} else {
						if (set[71].Get(la.kind)) {
							stateStack.Push(425);
							goto case 369;
						} else {
							Error(la);
							goto case 425;
						}
					}
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				Expect(38, la); // ")"
				currentState = 29;
				break;
			}
			case 426: {
				stateStack.Push(425);
				goto case 427;
			}
			case 427: {
				SetIdentifierExpected(la);
				goto case 428;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 429;
					break;
				} else {
					goto case 429;
				}
			}
			case 429: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(430);
				goto case 445;
			}
			case 430: {
				PopContext();
				goto case 431;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 446;
				} else {
					goto case 432;
				}
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 22) {
					currentState = 433;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 433: {
				SetIdentifierExpected(la);
				goto case 434;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 435;
					break;
				} else {
					goto case 435;
				}
			}
			case 435: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(436);
				goto case 445;
			}
			case 436: {
				PopContext();
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 438;
				} else {
					goto case 432;
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				Expect(63, la); // "As"
				currentState = 439;
				break;
			}
			case 439: {
				stateStack.Push(440);
				goto case 441;
			}
			case 440: {
				PopContext();
				goto case 432;
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (set[84].Get(la.kind)) {
					goto case 444;
				} else {
					if (la.kind == 35) {
						currentState = 442;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 442: {
				stateStack.Push(443);
				goto case 444;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 22) {
					currentState = 442;
					break;
				} else {
					goto case 59;
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (set[15].Get(la.kind)) {
					currentState = 34;
					break;
				} else {
					if (la.kind == 162) {
						goto case 93;
					} else {
						if (la.kind == 84) {
							goto case 109;
						} else {
							if (la.kind == 209) {
								goto case 84;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 2) {
					goto case 117;
				} else {
					if (la.kind == 62) {
						goto case 115;
					} else {
						if (la.kind == 64) {
							goto case 114;
						} else {
							if (la.kind == 65) {
								goto case 113;
							} else {
								if (la.kind == 66) {
									goto case 112;
								} else {
									if (la.kind == 67) {
										goto case 111;
									} else {
										if (la.kind == 70) {
											goto case 110;
										} else {
											if (la.kind == 87) {
												goto case 108;
											} else {
												if (la.kind == 104) {
													goto case 106;
												} else {
													if (la.kind == 107) {
														goto case 105;
													} else {
														if (la.kind == 116) {
															goto case 103;
														} else {
															if (la.kind == 121) {
																goto case 102;
															} else {
																if (la.kind == 133) {
																	goto case 98;
																} else {
																	if (la.kind == 139) {
																		goto case 97;
																	} else {
																		if (la.kind == 143) {
																			goto case 96;
																		} else {
																			if (la.kind == 146) {
																				goto case 95;
																			} else {
																				if (la.kind == 147) {
																					goto case 94;
																				} else {
																					if (la.kind == 170) {
																						goto case 91;
																					} else {
																						if (la.kind == 176) {
																							goto case 90;
																						} else {
																							if (la.kind == 184) {
																								goto case 89;
																							} else {
																								if (la.kind == 203) {
																									goto case 86;
																								} else {
																									if (la.kind == 212) {
																										goto case 81;
																									} else {
																										if (la.kind == 213) {
																											goto case 80;
																										} else {
																											if (la.kind == 223) {
																												goto case 78;
																											} else {
																												if (la.kind == 224) {
																													goto case 77;
																												} else {
																													if (la.kind == 230) {
																														goto case 76;
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
			case 446: {
				if (la == null) { currentState = 446; break; }
				Expect(63, la); // "As"
				currentState = 447;
				break;
			}
			case 447: {
				stateStack.Push(448);
				goto case 441;
			}
			case 448: {
				PopContext();
				goto case 432;
			}
			case 449: {
				stateStack.Push(450);
				goto case 166;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 37) {
					currentState = 454;
					break;
				} else {
					goto case 451;
				}
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 63) {
					currentState = 452;
					break;
				} else {
					goto case 18;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 40) {
					stateStack.Push(452);
					goto case 383;
				} else {
					goto case 453;
				}
			}
			case 453: {
				stateStack.Push(18);
				goto case 33;
			}
			case 454: {
				SetIdentifierExpected(la);
				goto case 455;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(456);
					goto case 369;
				} else {
					goto case 456;
				}
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				Expect(38, la); // ")"
				currentState = 451;
				break;
			}
			case 457: {
				stateStack.Push(458);
				goto case 166;
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 453;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 460;
							break;
						} else {
							goto case 459;
						}
					}
				} else {
					goto case 18;
				}
			}
			case 459: {
				Error(la);
				goto case 18;
			}
			case 460: {
				SetIdentifierExpected(la);
				goto case 461;
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(462);
					goto case 369;
				} else {
					goto case 462;
				}
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				Expect(38, la); // ")"
				currentState = 18;
				break;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(142, la); // "Interface"
				currentState = 9;
				break;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				Expect(115, la); // "Enum"
				currentState = 465;
				break;
			}
			case 465: {
				stateStack.Push(466);
				goto case 166;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 63) {
					currentState = 473;
					break;
				} else {
					goto case 467;
				}
			}
			case 467: {
				stateStack.Push(468);
				goto case 18;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (set[87].Get(la.kind)) {
					goto case 470;
				} else {
					Expect(113, la); // "End"
					currentState = 469;
					break;
				}
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				Expect(115, la); // "Enum"
				currentState = 18;
				break;
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				if (la.kind == 40) {
					stateStack.Push(470);
					goto case 383;
				} else {
					stateStack.Push(471);
					goto case 166;
				}
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (la.kind == 20) {
					currentState = 472;
					break;
				} else {
					goto case 467;
				}
			}
			case 472: {
				stateStack.Push(467);
				goto case 49;
			}
			case 473: {
				stateStack.Push(467);
				goto case 33;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				Expect(103, la); // "Delegate"
				currentState = 475;
				break;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (la.kind == 210) {
					currentState = 476;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 476;
						break;
					} else {
						Error(la);
						goto case 476;
					}
				}
			}
			case 476: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 477;
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				currentState = 478;
				break;
			}
			case 478: {
				PopContext();
				goto case 479;
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				if (la.kind == 37) {
					currentState = 484;
					break;
				} else {
					goto case 480;
				}
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 481;
				} else {
					goto case 18;
				}
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				Expect(63, la); // "As"
				currentState = 482;
				break;
			}
			case 482: {
				stateStack.Push(483);
				goto case 33;
			}
			case 483: {
				PopContext();
				goto case 18;
			}
			case 484: {
				SetIdentifierExpected(la);
				goto case 485;
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(486);
					goto case 369;
				} else {
					goto case 486;
				}
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				Expect(38, la); // ")"
				currentState = 480;
				break;
			}
			case 487: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 488;
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				if (la.kind == 155) {
					currentState = 489;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 489;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 489;
							break;
						} else {
							Error(la);
							goto case 489;
						}
					}
				}
			}
			case 489: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(490);
				goto case 166;
			}
			case 490: {
				PopContext();
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 37) {
					currentState = 600;
					break;
				} else {
					goto case 492;
				}
			}
			case 492: {
				stateStack.Push(493);
				goto case 18;
			}
			case 493: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 597;
				} else {
					goto case 495;
				}
			}
			case 495: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 496;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 591;
				} else {
					goto case 497;
				}
			}
			case 497: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 498;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (set[92].Get(la.kind)) {
					goto case 503;
				} else {
					isMissingModifier = false;
					goto case 499;
				}
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				Expect(113, la); // "End"
				currentState = 500;
				break;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 155) {
					currentState = 501;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 501;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 501;
							break;
						} else {
							Error(la);
							goto case 501;
						}
					}
				}
			}
			case 501: {
				stateStack.Push(502);
				goto case 18;
			}
			case 502: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 503: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 504;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (la.kind == 40) {
					stateStack.Push(503);
					goto case 383;
				} else {
					isMissingModifier = true;
					goto case 505;
				}
			}
			case 505: {
				SetIdentifierExpected(la);
				goto case 506;
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (set[110].Get(la.kind)) {
					currentState = 590;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 507;
				}
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(497);
					goto case 487;
				} else {
					if (la.kind == 103) {
						stateStack.Push(497);
						goto case 474;
					} else {
						if (la.kind == 115) {
							stateStack.Push(497);
							goto case 464;
						} else {
							if (la.kind == 142) {
								stateStack.Push(497);
								goto case 463;
							} else {
								if (set[95].Get(la.kind)) {
									stateStack.Push(497);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 508;
								} else {
									Error(la);
									goto case 497;
								}
							}
						}
					}
				}
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (set[101].Get(la.kind)) {
					stateStack.Push(509);
					SetIdentifierExpected(la);
					goto case 580;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(509);
						goto case 565;
					} else {
						if (la.kind == 101) {
							stateStack.Push(509);
							goto case 548;
						} else {
							if (la.kind == 119) {
								stateStack.Push(509);
								goto case 535;
							} else {
								if (la.kind == 98) {
									stateStack.Push(509);
									goto case 523;
								} else {
									if (la.kind == 172) {
										stateStack.Push(509);
										goto case 510;
									} else {
										Error(la);
										goto case 509;
									}
								}
							}
						}
					}
				}
			}
			case 509: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				Expect(172, la); // "Operator"
				currentState = 511;
				break;
			}
			case 511: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 512;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				currentState = 513;
				break;
			}
			case 513: {
				PopContext();
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				Expect(37, la); // "("
				currentState = 515;
				break;
			}
			case 515: {
				stateStack.Push(516);
				goto case 369;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				Expect(38, la); // ")"
				currentState = 517;
				break;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 63) {
					currentState = 521;
					break;
				} else {
					goto case 518;
				}
			}
			case 518: {
				stateStack.Push(519);
				goto case 222;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				Expect(113, la); // "End"
				currentState = 520;
				break;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				Expect(172, la); // "Operator"
				currentState = 18;
				break;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 40) {
					stateStack.Push(521);
					goto case 383;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(522);
					goto case 33;
				}
			}
			case 522: {
				PopContext();
				goto case 518;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				Expect(98, la); // "Custom"
				currentState = 524;
				break;
			}
			case 524: {
				stateStack.Push(525);
				goto case 535;
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				if (set[97].Get(la.kind)) {
					goto case 527;
				} else {
					Expect(113, la); // "End"
					currentState = 526;
					break;
				}
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				Expect(119, la); // "Event"
				currentState = 18;
				break;
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 40) {
					stateStack.Push(527);
					goto case 383;
				} else {
					if (la.kind == 56) {
						currentState = 528;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 528;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 528;
								break;
							} else {
								Error(la);
								goto case 528;
							}
						}
					}
				}
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				Expect(37, la); // "("
				currentState = 529;
				break;
			}
			case 529: {
				stateStack.Push(530);
				goto case 369;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				Expect(38, la); // ")"
				currentState = 531;
				break;
			}
			case 531: {
				stateStack.Push(532);
				goto case 222;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				Expect(113, la); // "End"
				currentState = 533;
				break;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (la.kind == 56) {
					currentState = 534;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 534;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 534;
							break;
						} else {
							Error(la);
							goto case 534;
						}
					}
				}
			}
			case 534: {
				stateStack.Push(525);
				goto case 18;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				Expect(119, la); // "Event"
				currentState = 536;
				break;
			}
			case 536: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(537);
				goto case 166;
			}
			case 537: {
				PopContext();
				goto case 538;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 545;
				} else {
					if (set[131].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 542;
							break;
						} else {
							goto case 539;
						}
					} else {
						Error(la);
						goto case 539;
					}
				}
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				if (la.kind == 136) {
					currentState = 540;
					break;
				} else {
					goto case 18;
				}
			}
			case 540: {
				stateStack.Push(541);
				goto case 33;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 22) {
					currentState = 540;
					break;
				} else {
					goto case 18;
				}
			}
			case 542: {
				SetIdentifierExpected(la);
				goto case 543;
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(544);
					goto case 369;
				} else {
					goto case 544;
				}
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				Expect(38, la); // ")"
				currentState = 539;
				break;
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(63, la); // "As"
				currentState = 546;
				break;
			}
			case 546: {
				stateStack.Push(547);
				goto case 33;
			}
			case 547: {
				PopContext();
				goto case 539;
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				Expect(101, la); // "Declare"
				currentState = 549;
				break;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 550;
					break;
				} else {
					goto case 550;
				}
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (la.kind == 210) {
					currentState = 551;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 551;
						break;
					} else {
						Error(la);
						goto case 551;
					}
				}
			}
			case 551: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(552);
				goto case 166;
			}
			case 552: {
				PopContext();
				goto case 553;
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				Expect(149, la); // "Lib"
				currentState = 554;
				break;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				Expect(3, la); // LiteralString
				currentState = 555;
				break;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				if (la.kind == 59) {
					currentState = 564;
					break;
				} else {
					goto case 556;
				}
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (la.kind == 37) {
					currentState = 561;
					break;
				} else {
					goto case 557;
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 558;
				} else {
					goto case 18;
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				Expect(63, la); // "As"
				currentState = 559;
				break;
			}
			case 559: {
				stateStack.Push(560);
				goto case 33;
			}
			case 560: {
				PopContext();
				goto case 18;
			}
			case 561: {
				SetIdentifierExpected(la);
				goto case 562;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(563);
					goto case 369;
				} else {
					goto case 563;
				}
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				Expect(38, la); // ")"
				currentState = 557;
				break;
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(3, la); // LiteralString
				currentState = 556;
				break;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				if (la.kind == 210) {
					currentState = 566;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 566;
						break;
					} else {
						Error(la);
						goto case 566;
					}
				}
			}
			case 566: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 567;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				currentState = 568;
				break;
			}
			case 568: {
				PopContext();
				goto case 569;
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				if (la.kind == 37) {
					currentState = 576;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 573;
					} else {
						goto case 570;
					}
				}
			}
			case 570: {
				stateStack.Push(571);
				goto case 222;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				Expect(113, la); // "End"
				currentState = 572;
				break;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				if (la.kind == 210) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 18;
						break;
					} else {
						goto case 459;
					}
				}
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				Expect(63, la); // "As"
				currentState = 574;
				break;
			}
			case 574: {
				stateStack.Push(575);
				goto case 33;
			}
			case 575: {
				PopContext();
				goto case 570;
			}
			case 576: {
				SetIdentifierExpected(la);
				goto case 577;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				if (set[130].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 579;
						break;
					} else {
						if (set[71].Get(la.kind)) {
							stateStack.Push(578);
							goto case 369;
						} else {
							Error(la);
							goto case 578;
						}
					}
				} else {
					goto case 578;
				}
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				Expect(38, la); // ")"
				currentState = 569;
				break;
			}
			case 579: {
				stateStack.Push(578);
				goto case 427;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 88) {
					currentState = 581;
					break;
				} else {
					goto case 581;
				}
			}
			case 581: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(582);
				goto case 589;
			}
			case 582: {
				PopContext();
				goto case 583;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 586;
				} else {
					goto case 584;
				}
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (la.kind == 20) {
					currentState = 585;
					break;
				} else {
					goto case 18;
				}
			}
			case 585: {
				stateStack.Push(18);
				goto case 49;
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				Expect(63, la); // "As"
				currentState = 587;
				break;
			}
			case 587: {
				stateStack.Push(588);
				goto case 33;
			}
			case 588: {
				PopContext();
				goto case 584;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (set[116].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 116;
					} else {
						if (la.kind == 126) {
							goto case 100;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 590: {
				isMissingModifier = false;
				goto case 505;
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				Expect(136, la); // "Implements"
				currentState = 592;
				break;
			}
			case 592: {
				stateStack.Push(593);
				goto case 33;
			}
			case 593: {
				PopContext();
				goto case 594;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				if (la.kind == 22) {
					currentState = 595;
					break;
				} else {
					stateStack.Push(497);
					goto case 18;
				}
			}
			case 595: {
				PushContext(Context.Type, la, t);
				stateStack.Push(596);
				goto case 33;
			}
			case 596: {
				PopContext();
				goto case 594;
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(140, la); // "Inherits"
				currentState = 598;
				break;
			}
			case 598: {
				stateStack.Push(599);
				goto case 33;
			}
			case 599: {
				PopContext();
				stateStack.Push(495);
				goto case 18;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				Expect(169, la); // "Of"
				currentState = 601;
				break;
			}
			case 601: {
				stateStack.Push(602);
				goto case 427;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				Expect(38, la); // ")"
				currentState = 492;
				break;
			}
			case 603: {
				isMissingModifier = false;
				goto case 23;
			}
			case 604: {
				if (la == null) { currentState = 604; break; }
				Expect(140, la); // "Inherits"
				currentState = 605;
				break;
			}
			case 605: {
				stateStack.Push(606);
				goto case 33;
			}
			case 606: {
				PopContext();
				goto case 607;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				if (la.kind == 22) {
					currentState = 608;
					break;
				} else {
					stateStack.Push(14);
					goto case 18;
				}
			}
			case 608: {
				PushContext(Context.Type, la, t);
				stateStack.Push(609);
				goto case 33;
			}
			case 609: {
				PopContext();
				goto case 607;
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				Expect(169, la); // "Of"
				currentState = 611;
				break;
			}
			case 611: {
				stateStack.Push(612);
				goto case 427;
			}
			case 612: {
				if (la == null) { currentState = 612; break; }
				Expect(38, la); // ")"
				currentState = 11;
				break;
			}
			case 613: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 614;
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (set[44].Get(la.kind)) {
					currentState = 614;
					break;
				} else {
					PopContext();
					stateStack.Push(615);
					goto case 18;
				}
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(615);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 616;
					break;
				}
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				Expect(160, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				Expect(137, la); // "Imports"
				currentState = 618;
				break;
			}
			case 618: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 619;
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				if (set[15].Get(la.kind)) {
					currentState = 625;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 621;
						break;
					} else {
						Error(la);
						goto case 620;
					}
				}
			}
			case 620: {
				PopContext();
				goto case 18;
			}
			case 621: {
				stateStack.Push(622);
				goto case 166;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(20, la); // "="
				currentState = 623;
				break;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				Expect(3, la); // LiteralString
				currentState = 624;
				break;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 620;
				break;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (la.kind == 37) {
					stateStack.Push(625);
					goto case 38;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 626;
						break;
					} else {
						goto case 620;
					}
				}
			}
			case 626: {
				stateStack.Push(620);
				goto case 33;
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				Expect(173, la); // "Option"
				currentState = 628;
				break;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 630;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 629;
						break;
					} else {
						goto case 459;
					}
				}
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (la.kind == 213) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 18;
						break;
					} else {
						goto case 459;
					}
				}
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 18;
					break;
				} else {
					goto case 18;
				}
			}
		}

		if (la != null) {
			t = la;
			nextTokenIsPotentialStartOfExpression = false;
			readXmlIdentifier = false;
			nextTokenIsStartOfImportsOrAccessExpression = false;
			wasQualifierTokenAtStart = false;
			identifierExpected = false;
		}
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	public BitArray GetExpectedSet() { return GetExpectedSet(currentState); }
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 436215809, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 436207617, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234112, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 436207616, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 134234112, 436207616, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 256, 1048576, -1601568128, 671109120, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 256, 1048576, -1601568128, 671105024, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 256, 1048576, -1601699200, 671105024, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 0, 1048576, -1601699200, 671105024, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
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
		new BitArray(new int[] {7, 1157628160, 26477055, -493343812, 680323108, 1073567107, -533262446, 1347}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {721920, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850752, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850696, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850688, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850944, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220956, 671930656, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220956, 671926560, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220956, 671926304, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {5, 1140850944, 26214479, -493220956, 671926304, 532485251, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493352028, 671926304, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850688, 26214479, -493352028, 671926304, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850688, 26214479, -1030223452, 135055392, 17110016, -2143942656, 65}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 17110016, -2144073728, 65}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 134234112, 436207617, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 128, 0}),
		new BitArray(new int[] {0, 0, 0, 536871424, 536870912, 448266370, 384, 1280}),
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
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0})

	};

} // end Parser


}