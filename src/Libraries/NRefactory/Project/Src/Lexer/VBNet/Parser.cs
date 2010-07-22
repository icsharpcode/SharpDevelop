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
			case 64:
			case 227:
			case 453:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 155:
			case 161:
			case 167:
			case 204:
			case 208:
			case 247:
			case 348:
			case 357:
			case 404:
			case 443:
			case 451:
			case 459:
			case 482:
			case 517:
			case 571:
			case 585:
			case 654:
				return set[6];
			case 10:
			case 483:
			case 484:
			case 528:
			case 538:
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
			case 31:
			case 219:
			case 222:
			case 223:
			case 233:
			case 248:
			case 252:
			case 274:
			case 289:
			case 300:
			case 303:
			case 309:
			case 314:
			case 323:
			case 324:
			case 337:
			case 345:
			case 365:
			case 461:
			case 476:
			case 485:
			case 494:
			case 511:
			case 515:
			case 523:
			case 529:
			case 532:
			case 539:
			case 542:
			case 566:
			case 569:
			case 593:
			case 603:
			case 607:
			case 632:
			case 653:
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
			case 250:
			case 304:
			case 346:
			case 390:
			case 492:
			case 512:
			case 530:
			case 534:
			case 540:
			case 567:
			case 604:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 17:
			case 457:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 20:
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 21:
			case 22:
				return set[9];
			case 23:
			case 636:
				return set[10];
			case 24:
				return set[11];
			case 25:
				return set[12];
			case 26:
			case 27:
			case 120:
			case 177:
			case 178:
			case 228:
			case 380:
			case 381:
			case 396:
			case 397:
			case 398:
			case 470:
			case 471:
			case 504:
			case 505:
			case 599:
			case 600:
			case 646:
			case 647:
				return set[13];
			case 28:
			case 29:
			case 444:
			case 452:
			case 472:
			case 473:
			case 590:
			case 601:
			case 602:
				return set[14];
			case 30:
			case 32:
			case 124:
			case 134:
			case 150:
			case 165:
			case 181:
			case 259:
			case 284:
			case 364:
			case 377:
			case 407:
			case 447:
			case 467:
			case 475:
			case 551:
			case 575:
			case 580:
			case 592:
			case 606:
			case 625:
			case 628:
			case 631:
			case 638:
			case 641:
			case 659:
				return set[15];
			case 33:
			case 36:
				return set[16];
			case 34:
				return set[17];
			case 35:
			case 70:
			case 74:
			case 129:
			case 340:
			case 410:
				return set[18];
			case 37:
			case 140:
			case 147:
			case 151:
			case 213:
			case 384:
			case 403:
			case 406:
			case 506:
			case 507:
			case 563:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 38:
			case 39:
			case 131:
			case 132:
				return set[19];
			case 40:
			case 133:
			case 216:
			case 362:
			case 387:
			case 405:
			case 421:
			case 450:
			case 456:
			case 479:
			case 509:
			case 545:
			case 548:
			case 557:
			case 565:
			case 579:
			case 596:
			case 610:
			case 635:
			case 645:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 41:
			case 42:
			case 46:
			case 47:
			case 415:
			case 416:
				return set[20];
			case 43:
			case 44:
				return set[21];
			case 45:
			case 142:
			case 149:
			case 343:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 48:
			case 135:
			case 144:
			case 361:
			case 363:
			case 367:
			case 375:
			case 414:
			case 418:
			case 428:
			case 435:
			case 442:
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
			case 72:
			case 122:
			case 141:
			case 143:
			case 145:
			case 148:
			case 157:
			case 159:
			case 199:
			case 232:
			case 236:
			case 238:
			case 239:
			case 256:
			case 273:
			case 278:
			case 287:
			case 293:
			case 295:
			case 299:
			case 302:
			case 308:
			case 319:
			case 321:
			case 327:
			case 342:
			case 344:
			case 376:
			case 400:
			case 412:
			case 413:
			case 466:
			case 550:
				return set[22];
			case 51:
			case 55:
				return set[23];
			case 56:
			case 66:
			case 67:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 58:
			case 73:
			case 438:
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
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 61:
			case 62:
				return set[24];
			case 63:
			case 75:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 65:
				return set[25];
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 71:
			case 180:
			case 182:
			case 183:
			case 286:
			case 655:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 76:
			case 305:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 80:
			case 251:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 83:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 84:
			case 391:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 88:
			case 311:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 89:
			case 516:
			case 535:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 93:
			case 268:
			case 275:
			case 290:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 96:
			case 186:
			case 191:
			case 193:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 97:
			case 188:
			case 192:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 100:
			case 221:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 101:
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 104:
			case 158:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 107:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 108:
			case 558:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 111:
			case 170:
			case 198:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 117:
			case 210:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 119:
				return set[26];
			case 121:
				return set[27];
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 125:
				return set[28];
			case 126:
				return set[29];
			case 127:
			case 128:
			case 408:
			case 409:
				return set[30];
			case 130:
				return set[31];
			case 136:
			case 137:
			case 271:
			case 280:
				return set[32];
			case 138:
				return set[33];
			case 139:
			case 326:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 146:
				return set[34];
			case 152:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 153:
			case 154:
				return set[35];
			case 156:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 160:
			case 174:
			case 190:
			case 195:
			case 201:
			case 203:
			case 207:
			case 209:
				return set[36];
			case 162:
			case 163:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 164:
			case 166:
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 168:
			case 169:
			case 171:
			case 173:
			case 175:
			case 176:
			case 184:
			case 189:
			case 194:
			case 202:
			case 206:
				return set[37];
			case 172:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 179:
				return set[38];
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
				return set[39];
			case 200:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 205:
				return set[40];
			case 212:
			case 469:
			case 584:
			case 598:
			case 605:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 214:
			case 215:
			case 385:
			case 386:
			case 448:
			case 449:
			case 454:
			case 455:
			case 477:
			case 478:
			case 543:
			case 544:
			case 546:
			case 547:
			case 555:
			case 556:
			case 577:
			case 578:
			case 594:
			case 595:
				return set[41];
			case 217:
			case 218:
				return set[42];
			case 224:
			case 225:
				return set[43];
			case 226:
				return set[44];
			case 229:
				return set[45];
			case 230:
			case 231:
			case 332:
				return set[46];
			case 235:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 237:
			case 279:
			case 294:
				return set[47];
			case 240:
			case 241:
			case 261:
			case 262:
			case 276:
			case 277:
			case 291:
			case 292:
				return set[48];
			case 242:
			case 333:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 243:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 244:
				return set[49];
			case 245:
			case 264:
				return set[50];
			case 246:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 249:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 253:
			case 254:
				return set[51];
			case 255:
			case 260:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 257:
			case 258:
				return set[52];
			case 263:
				return set[53];
			case 265:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 266:
			case 267:
				return set[54];
			case 269:
			case 270:
				return set[55];
			case 281:
			case 282:
				return set[56];
			case 283:
				return set[57];
			case 285:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 288:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 296:
				return set[58];
			case 297:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 298:
				return set[59];
			case 306:
			case 307:
				return set[60];
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 312:
			case 313:
				return set[61];
			case 315:
			case 316:
				return set[62];
			case 317:
			case 576:
			case 613:
			case 626:
			case 627:
			case 629:
			case 639:
			case 640:
			case 642:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 318:
			case 320:
				return set[63];
			case 322:
			case 328:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 325:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 329:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 330:
			case 331:
			case 335:
			case 336:
			case 388:
			case 389:
				return set[64];
			case 338:
			case 339:
				return set[65];
			case 341:
				return set[66];
			case 347:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 349:
			case 350:
			case 358:
			case 359:
				return set[67];
			case 351:
			case 360:
				return set[68];
			case 352:
				return set[69];
			case 353:
			case 356:
				return set[70];
			case 354:
			case 355:
			case 619:
			case 620:
				return set[71];
			case 366:
			case 368:
			case 369:
			case 508:
			case 564:
				return set[72];
			case 370:
			case 371:
				return set[73];
			case 372:
			case 373:
				return set[74];
			case 374:
			case 378:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 379:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 382:
			case 383:
				return set[75];
			case 392:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 393:
				return set[76];
			case 394:
				return set[77];
			case 395:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 399:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 401:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 402:
				return set[78];
			case 411:
				return set[79];
			case 417:
				return set[80];
			case 419:
			case 420:
			case 608:
			case 609:
				return set[81];
			case 422:
			case 423:
			case 424:
			case 429:
			case 430:
			case 611:
			case 634:
			case 644:
				return set[82];
			case 425:
			case 431:
			case 440:
				return set[83];
			case 426:
			case 427:
			case 432:
			case 433:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 434:
			case 436:
			case 441:
				return set[84];
			case 437:
			case 439:
				return set[85];
			case 445:
			case 460:
			case 474:
			case 510:
			case 591:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 446:
			case 514:
				return set[86];
			case 458:
			case 463:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 462:
				return set[87];
			case 464:
				return set[88];
			case 465:
			case 522:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 468:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 480:
			case 481:
			case 493:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 486:
			case 487:
				return set[89];
			case 488:
			case 489:
				return set[90];
			case 490:
			case 491:
			case 502:
				return set[91];
			case 495:
				return set[92];
			case 496:
			case 497:
				return set[93];
			case 498:
			case 499:
			case 623:
				return set[94];
			case 500:
				return set[95];
			case 501:
				return set[96];
			case 503:
			case 513:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 518:
			case 519:
				return set[97];
			case 520:
				return set[98];
			case 521:
			case 554:
				return set[99];
			case 524:
			case 525:
			case 526:
			case 549:
				return set[100];
			case 527:
			case 531:
			case 541:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 533:
				return set[101];
			case 536:
				return set[102];
			case 537:
				return set[103];
			case 552:
			case 618:
			case 621:
				return set[104];
			case 553:
				return set[105];
			case 559:
			case 561:
			case 570:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 560:
				return set[106];
			case 562:
				return set[107];
			case 568:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 572:
			case 573:
				return set[108];
			case 574:
			case 581:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 582:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 583:
				return set[109];
			case 586:
			case 587:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 588:
			case 597:
			case 656:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 589:
				return set[110];
			case 612:
			case 614:
				return set[111];
			case 615:
			case 622:
				return set[112];
			case 616:
			case 617:
				return set[113];
			case 624:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 630:
			case 637:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 633:
			case 643:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 648:
				return set[114];
			case 649:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 650:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 651:
			case 652:
				return set[115];
			case 657:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 658:
				return set[116];
			case 660:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 661:
				return set[117];
			case 662:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 663:
				return set[118];
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
					goto case 660;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 650;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 379;
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
					currentState = 646;
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
					goto case 379;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[119].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 480;
					} else {
						if (la.kind == 103) {
							currentState = 469;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 459;
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
				goto case 167;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (la.kind == 37) {
					currentState = 643;
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
					goto case 637;
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
					goto case 379;
				} else {
					isMissingModifier = true;
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (set[120].Get(la.kind)) {
					currentState = 636;
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
					goto case 480;
				} else {
					if (la.kind == 103) {
						stateStack.Push(14);
						goto case 468;
					} else {
						if (la.kind == 115) {
							stateStack.Push(14);
							goto case 458;
						} else {
							if (la.kind == 142) {
								stateStack.Push(14);
								goto case 457;
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
					currentState = 451;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 443;
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
					currentState = 419;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 30;
						break;
					} else {
						goto case 18;
					}
				}
			}
			case 30: {
				PushContext(Context.Type, la, t);
				stateStack.Push(31);
				goto case 32;
			}
			case 31: {
				PopContext();
				goto case 18;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (la.kind == 130) {
					currentState = 33;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 33;
						break;
					} else {
						if (set[121].Get(la.kind)) {
							currentState = 33;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 33;
								break;
							} else {
								Error(la);
								goto case 33;
							}
						}
					}
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 37) {
					stateStack.Push(33);
					goto case 37;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 26) {
					currentState = 35;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				stateStack.Push(36);
				goto case 74;
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (la.kind == 37) {
					stateStack.Push(36);
					goto case 37;
				} else {
					goto case 34;
				}
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				Expect(37, la); // "("
				currentState = 38;
				break;
			}
			case 38: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 169) {
					currentState = 417;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 41;
					} else {
						Error(la);
						goto case 40;
					}
				}
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 41: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 42;
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(40);
					goto case 43;
				} else {
					goto case 40;
				}
			}
			case 43: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (set[22].Get(la.kind)) {
					goto case 413;
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
				if (set[122].Get(la.kind)) {
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
				if (set[123].Get(la.kind)) {
					currentState = 53;
					break;
				} else {
					if (set[32].Get(la.kind)) {
						stateStack.Push(125);
						goto case 136;
					} else {
						if (la.kind == 220) {
							currentState = 122;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(55);
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
				PushContext(Context.ObjectCreation, la, t);
				goto case 62;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(119);
					goto case 32;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (la.kind == 233) {
					currentState = 66;
					break;
				} else {
					goto case 64;
				}
			}
			case 64: {
				Error(la);
				goto case 65;
			}
			case 65: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 66: {
				stateStack.Push(65);
				goto case 67;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				Expect(35, la); // "{"
				currentState = 68;
				break;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 147) {
					currentState = 69;
					break;
				} else {
					goto case 69;
				}
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				Expect(26, la); // "."
				currentState = 70;
				break;
			}
			case 70: {
				stateStack.Push(71);
				goto case 74;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				Expect(20, la); // "="
				currentState = 72;
				break;
			}
			case 72: {
				stateStack.Push(73);
				goto case 49;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (la.kind == 22) {
					currentState = 68;
					break;
				} else {
					goto case 59;
				}
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				if (la.kind == 2) {
					goto case 118;
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
								goto case 117;
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
												goto case 116;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 115;
													} else {
														if (la.kind == 65) {
															goto case 114;
														} else {
															if (la.kind == 66) {
																goto case 113;
															} else {
																if (la.kind == 67) {
																	goto case 112;
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
																				goto case 111;
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
																																		goto case 110;
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
																																					goto case 109;
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
																																																goto case 108;
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
																																																						goto case 107;
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
																																																									goto case 106;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 105;
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
																																																																		goto case 104;
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
																																																																							goto case 103;
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
																																																																										goto case 102;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 101;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 100;
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
																																																																																			goto case 99;
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
																																																																																									goto case 98;
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
																																																																																													goto case 97;
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
																																																																																																goto case 96;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 95;
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
																																																																																																																goto case 94;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 93;
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
																																																																																																																								goto case 92;
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
																																																																																																																														goto case 91;
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
																																																																																																																																						goto case 90;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 89;
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
																																																																																																																																																			goto case 88;
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
																																																																																																																																																									goto case 87;
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
																																																																																																																																																												goto case 86;
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
																																																																																																																																																															goto case 85;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 84;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 83;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 82;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 81;
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
																																																																																																																																																																								goto case 80;
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
																																																																																																																																																																													goto case 79;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 78;
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
																																																																																																																																																																																				goto case 77;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 76;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 75;
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
				currentState = stateStack.Pop();
				break;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 120;
						break;
					} else {
						goto case 63;
					}
				} else {
					goto case 65;
				}
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				if (la.kind == 35) {
					stateStack.Push(65);
					goto case 56;
				} else {
					if (set[27].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 64;
					}
				}
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				currentState = 65;
				break;
			}
			case 122: {
				stateStack.Push(123);
				goto case 52;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				Expect(144, la); // "Is"
				currentState = 124;
				break;
			}
			case 124: {
				stateStack.Push(55);
				goto case 32;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (set[29].Get(la.kind)) {
					stateStack.Push(125);
					goto case 126;
				} else {
					goto case 55;
				}
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 37) {
					currentState = 131;
					break;
				} else {
					if (set[124].Get(la.kind)) {
						currentState = 127;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 127: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 128;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (la.kind == 10) {
					currentState = 129;
					break;
				} else {
					goto case 129;
				}
			}
			case 129: {
				stateStack.Push(130);
				goto case 74;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 131: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 132;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 169) {
					currentState = 134;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(133);
							goto case 43;
						} else {
							goto case 133;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 133: {
				PopContext();
				goto case 40;
			}
			case 134: {
				stateStack.Push(135);
				goto case 32;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.kind == 22) {
					currentState = 134;
					break;
				} else {
					goto case 40;
				}
			}
			case 136: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 137;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (set[125].Get(la.kind)) {
					currentState = 138;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 412;
						break;
					} else {
						if (set[126].Get(la.kind)) {
							currentState = 138;
							break;
						} else {
							if (set[121].Get(la.kind)) {
								currentState = 138;
								break;
							} else {
								if (set[124].Get(la.kind)) {
									currentState = 408;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 406;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 403;
											break;
										} else {
											if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
												stateStack.Push(138);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 392;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(138);
													goto case 212;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(138);
														PushContext(Context.Query, la, t);
														goto case 152;
													} else {
														if (set[34].Get(la.kind)) {
															stateStack.Push(138);
															goto case 146;
														} else {
															if (la.kind == 135) {
																stateStack.Push(138);
																goto case 139;
															} else {
																Error(la);
																goto case 138;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 138: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				Expect(135, la); // "If"
				currentState = 140;
				break;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(37, la); // "("
				currentState = 141;
				break;
			}
			case 141: {
				stateStack.Push(142);
				goto case 49;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(22, la); // ","
				currentState = 143;
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 49;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 22) {
					currentState = 145;
					break;
				} else {
					goto case 40;
				}
			}
			case 145: {
				stateStack.Push(40);
				goto case 49;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (set[127].Get(la.kind)) {
					currentState = 151;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 147;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(37, la); // "("
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(149);
				goto case 49;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(22, la); // ","
				currentState = 150;
				break;
			}
			case 150: {
				stateStack.Push(40);
				goto case 32;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(37, la); // "("
				currentState = 145;
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.kind == 126) {
					stateStack.Push(153);
					goto case 211;
				} else {
					if (la.kind == 58) {
						stateStack.Push(153);
						goto case 210;
					} else {
						Error(la);
						goto case 153;
					}
				}
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(153);
					goto case 154;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
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
								goto case 106;
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
													currentState = 168;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 155;
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
			case 155: {
				stateStack.Push(156);
				goto case 161;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(171, la); // "On"
				currentState = 157;
				break;
			}
			case 157: {
				stateStack.Push(158);
				goto case 49;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(116, la); // "Equals"
				currentState = 159;
				break;
			}
			case 159: {
				stateStack.Push(160);
				goto case 49;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.kind == 22) {
					currentState = 157;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(162);
				goto case 167;
			}
			case 162: {
				PopContext();
				goto case 163;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 63) {
					currentState = 165;
					break;
				} else {
					goto case 164;
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(138, la); // "In"
				currentState = 49;
				break;
			}
			case 165: {
				PushContext(Context.Type, la, t);
				stateStack.Push(166);
				goto case 32;
			}
			case 166: {
				PopContext();
				goto case 164;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (set[112].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 108;
					} else {
						goto case 6;
					}
				}
			}
			case 168: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 169;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (la.kind == 146) {
					goto case 186;
				} else {
					if (set[37].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 171;
							break;
						} else {
							if (set[37].Get(la.kind)) {
								goto case 184;
							} else {
								Error(la);
								goto case 170;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				Expect(70, la); // "By"
				currentState = 171;
				break;
			}
			case 171: {
				stateStack.Push(172);
				goto case 175;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				if (la.kind == 22) {
					currentState = 171;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 173;
					break;
				}
			}
			case 173: {
				stateStack.Push(174);
				goto case 175;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 22) {
					currentState = 173;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 176;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(177);
					goto case 167;
				} else {
					goto case 49;
				}
			}
			case 177: {
				PopContext();
				goto case 178;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 63) {
					currentState = 181;
					break;
				} else {
					if (la.kind == 20) {
						goto case 180;
					} else {
						if (set[38].Get(la.kind)) {
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
			case 179: {
				if (la == null) { currentState = 179; break; }
				currentState = 49;
				break;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				currentState = 49;
				break;
			}
			case 181: {
				PushContext(Context.Type, la, t);
				stateStack.Push(182);
				goto case 32;
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
				goto case 175;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 22) {
					currentState = 184;
					break;
				} else {
					goto case 170;
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
				goto case 175;
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
				currentState = 155;
				break;
			}
			case 194: {
				stateStack.Push(195);
				goto case 175;
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
				goto case 175;
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
				goto case 161;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(205);
					goto case 154;
				} else {
					Expect(143, la); // "Into"
					currentState = 206;
					break;
				}
			}
			case 206: {
				stateStack.Push(207);
				goto case 175;
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
				goto case 161;
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
					currentState = 384;
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
				if (set[72].Get(la.kind)) {
					stateStack.Push(216);
					goto case 366;
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
							currentState = 364;
							break;
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
				if (set[128].Get(la.kind)) {
					if (set[64].Get(la.kind)) {
						if (set[46].Get(la.kind)) {
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
					if (set[45].Get(la.kind)) {
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
					currentState = 348;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 344;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 342;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 340;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 321;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 306;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 302;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 296;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 269;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 265;
																break;
															} else {
																goto case 265;
															}
														} else {
															if (la.kind == 194) {
																currentState = 263;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 261;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 248;
																break;
															} else {
																if (set[129].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 245;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 244;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 243;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 86;
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
																				if (set[130].Get(la.kind)) {
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
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(242);
					goto case 49;
				} else {
					goto case 242;
				}
			}
			case 242: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 108) {
					goto case 105;
				} else {
					if (la.kind == 124) {
						goto case 102;
					} else {
						if (la.kind == 231) {
							goto case 76;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (la.kind == 108) {
					goto case 105;
				} else {
					if (la.kind == 124) {
						goto case 102;
					} else {
						if (la.kind == 231) {
							goto case 76;
						} else {
							if (la.kind == 197) {
								goto case 88;
							} else {
								if (la.kind == 210) {
									goto case 84;
								} else {
									if (la.kind == 127) {
										goto case 100;
									} else {
										if (la.kind == 186) {
											goto case 89;
										} else {
											if (la.kind == 218) {
												goto case 80;
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
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (set[6].Get(la.kind)) {
					goto case 247;
				} else {
					if (la.kind == 5) {
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 248: {
				stateStack.Push(249);
				goto case 222;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 75) {
					currentState = 253;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 252;
						break;
					} else {
						goto case 250;
					}
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				Expect(113, la); // "End"
				currentState = 251;
				break;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 252: {
				stateStack.Push(250);
				goto case 222;
			}
			case 253: {
				SetIdentifierExpected(la);
				goto case 254;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(257);
					goto case 167;
				} else {
					goto case 255;
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 229) {
					currentState = 256;
					break;
				} else {
					goto case 248;
				}
			}
			case 256: {
				stateStack.Push(248);
				goto case 49;
			}
			case 257: {
				PopContext();
				goto case 258;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 63) {
					currentState = 259;
					break;
				} else {
					goto case 255;
				}
			}
			case 259: {
				PushContext(Context.Type, la, t);
				stateStack.Push(260);
				goto case 32;
			}
			case 260: {
				PopContext();
				goto case 255;
			}
			case 261: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 262;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.kind == 163) {
					goto case 93;
				} else {
					goto case 264;
				}
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (la.kind == 5) {
					goto case 246;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 247;
					} else {
						goto case 6;
					}
				}
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				Expect(118, la); // "Error"
				currentState = 266;
				break;
			}
			case 266: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 267;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 132) {
						currentState = 264;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 268;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 269: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(286);
					goto case 280;
				} else {
					if (la.kind == 110) {
						currentState = 271;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 271: {
				stateStack.Push(272);
				goto case 280;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				Expect(138, la); // "In"
				currentState = 273;
				break;
			}
			case 273: {
				stateStack.Push(274);
				goto case 49;
			}
			case 274: {
				stateStack.Push(275);
				goto case 222;
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
				if (set[22].Get(la.kind)) {
					goto case 278;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 278: {
				stateStack.Push(279);
				goto case 49;
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
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(281);
				goto case 136;
			}
			case 281: {
				PopContext();
				goto case 282;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (la.kind == 33) {
					currentState = 283;
					break;
				} else {
					goto case 283;
				}
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (set[29].Get(la.kind)) {
					stateStack.Push(283);
					goto case 126;
				} else {
					if (la.kind == 63) {
						currentState = 284;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 284: {
				PushContext(Context.Type, la, t);
				stateStack.Push(285);
				goto case 32;
			}
			case 285: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(20, la); // "="
				currentState = 287;
				break;
			}
			case 287: {
				stateStack.Push(288);
				goto case 49;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.kind == 205) {
					currentState = 295;
					break;
				} else {
					goto case 289;
				}
			}
			case 289: {
				stateStack.Push(290);
				goto case 222;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(163, la); // "Next"
				currentState = 291;
				break;
			}
			case 291: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 292;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (set[22].Get(la.kind)) {
					goto case 293;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 293: {
				stateStack.Push(294);
				goto case 49;
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
				stateStack.Push(289);
				goto case 49;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 299;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(297);
						goto case 222;
					} else {
						goto case 6;
					}
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(152, la); // "Loop"
				currentState = 298;
				break;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 49;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 299: {
				stateStack.Push(300);
				goto case 49;
			}
			case 300: {
				stateStack.Push(301);
				goto case 222;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				stateStack.Push(303);
				goto case 49;
			}
			case 303: {
				stateStack.Push(304);
				goto case 222;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(113, la); // "End"
				currentState = 305;
				break;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 306: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 307;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (la.kind == 74) {
					currentState = 308;
					break;
				} else {
					goto case 308;
				}
			}
			case 308: {
				stateStack.Push(309);
				goto case 49;
			}
			case 309: {
				stateStack.Push(310);
				goto case 18;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (la.kind == 74) {
					currentState = 312;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 311;
					break;
				}
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 312: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 313;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 111) {
					currentState = 314;
					break;
				} else {
					if (set[62].Get(la.kind)) {
						goto case 315;
					} else {
						Error(la);
						goto case 314;
					}
				}
			}
			case 314: {
				stateStack.Push(310);
				goto case 222;
			}
			case 315: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 316;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (set[131].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 318;
						break;
					} else {
						goto case 318;
					}
				} else {
					if (set[22].Get(la.kind)) {
						stateStack.Push(317);
						goto case 49;
					} else {
						Error(la);
						goto case 317;
					}
				}
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 22) {
					currentState = 315;
					break;
				} else {
					goto case 314;
				}
			}
			case 318: {
				stateStack.Push(319);
				goto case 320;
			}
			case 319: {
				stateStack.Push(317);
				goto case 52;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
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
			case 321: {
				stateStack.Push(322);
				goto case 49;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 214) {
					currentState = 330;
					break;
				} else {
					goto case 323;
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 324;
				} else {
					goto case 6;
				}
			}
			case 324: {
				stateStack.Push(325);
				goto case 222;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 329;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 327;
							break;
						} else {
							Error(la);
							goto case 324;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 326;
					break;
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 327: {
				stateStack.Push(328);
				goto case 49;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 214) {
					currentState = 324;
					break;
				} else {
					goto case 324;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 135) {
					currentState = 327;
					break;
				} else {
					goto case 324;
				}
			}
			case 330: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 331;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (set[46].Get(la.kind)) {
					goto case 332;
				} else {
					goto case 323;
				}
			}
			case 332: {
				stateStack.Push(333);
				goto case 230;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 21) {
					currentState = 338;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 335;
						break;
					} else {
						goto case 334;
					}
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 336;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (set[46].Get(la.kind)) {
					stateStack.Push(337);
					goto case 230;
				} else {
					goto case 337;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 21) {
					currentState = 335;
					break;
				} else {
					goto case 334;
				}
			}
			case 338: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 339;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (set[46].Get(la.kind)) {
					goto case 332;
				} else {
					goto case 333;
				}
			}
			case 340: {
				stateStack.Push(341);
				goto case 74;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 37) {
					currentState = 41;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 342: {
				stateStack.Push(343);
				goto case 49;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				Expect(22, la); // ","
				currentState = 49;
				break;
			}
			case 344: {
				stateStack.Push(345);
				goto case 49;
			}
			case 345: {
				stateStack.Push(346);
				goto case 222;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				Expect(113, la); // "End"
				currentState = 347;
				break;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 233) {
					goto case 75;
				} else {
					if (la.kind == 211) {
						goto case 83;
					} else {
						goto case 6;
					}
				}
			}
			case 348: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(349);
				goto case 167;
			}
			case 349: {
				PopContext();
				goto case 350;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 33) {
					currentState = 351;
					break;
				} else {
					goto case 351;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 37) {
					currentState = 363;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 22) {
					currentState = 357;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 354;
						break;
					} else {
						goto case 353;
					}
				}
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 20) {
					goto case 180;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 354: {
				PushContext(Context.Type, la, t);
				goto case 355;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 162) {
					stateStack.Push(356);
					goto case 60;
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(356);
						goto case 32;
					} else {
						Error(la);
						goto case 356;
					}
				}
			}
			case 356: {
				PopContext();
				goto case 353;
			}
			case 357: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(358);
				goto case 167;
			}
			case 358: {
				PopContext();
				goto case 359;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (la.kind == 33) {
					currentState = 360;
					break;
				} else {
					goto case 360;
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 37) {
					currentState = 361;
					break;
				} else {
					goto case 352;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 22) {
					currentState = 361;
					break;
				} else {
					goto case 362;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(38, la); // ")"
				currentState = 352;
				break;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 22) {
					currentState = 363;
					break;
				} else {
					goto case 362;
				}
			}
			case 364: {
				PushContext(Context.Type, la, t);
				stateStack.Push(365);
				goto case 32;
			}
			case 365: {
				PopContext();
				goto case 219;
			}
			case 366: {
				stateStack.Push(367);
				PushContext(Context.Parameter, la, t);
				goto case 368;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (la.kind == 22) {
					currentState = 366;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 368: {
				SetIdentifierExpected(la);
				goto case 369;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 40) {
					stateStack.Push(368);
					goto case 379;
				} else {
					goto case 370;
				}
			}
			case 370: {
				SetIdentifierExpected(la);
				goto case 371;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (set[132].Get(la.kind)) {
					currentState = 370;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(372);
					goto case 167;
				}
			}
			case 372: {
				PopContext();
				goto case 373;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 63) {
					currentState = 377;
					break;
				} else {
					goto case 374;
				}
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (la.kind == 20) {
					currentState = 376;
					break;
				} else {
					goto case 375;
				}
			}
			case 375: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 376: {
				stateStack.Push(375);
				goto case 49;
			}
			case 377: {
				PushContext(Context.Type, la, t);
				stateStack.Push(378);
				goto case 32;
			}
			case 378: {
				PopContext();
				goto case 374;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				Expect(40, la); // "<"
				currentState = 380;
				break;
			}
			case 380: {
				PushContext(Context.Attribute, la, t);
				goto case 381;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (set[133].Get(la.kind)) {
					currentState = 381;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 382;
					break;
				}
			}
			case 382: {
				PopContext();
				goto case 383;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(37, la); // "("
				currentState = 385;
				break;
			}
			case 385: {
				SetIdentifierExpected(la);
				goto case 386;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(387);
					goto case 366;
				} else {
					goto case 387;
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				Expect(38, la); // ")"
				currentState = 388;
				break;
			}
			case 388: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 389;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (set[46].Get(la.kind)) {
					goto case 230;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(390);
						goto case 222;
					} else {
						goto case 6;
					}
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(113, la); // "End"
				currentState = 391;
				break;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 402;
					break;
				} else {
					stateStack.Push(393);
					goto case 395;
				}
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 17) {
					currentState = 394;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 16) {
					currentState = 393;
					break;
				} else {
					goto case 393;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 396;
				break;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (set[134].Get(la.kind)) {
					if (set[135].Get(la.kind)) {
						currentState = 396;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(396);
							goto case 399;
						} else {
							Error(la);
							goto case 396;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 397;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[136].Get(la.kind)) {
					if (set[137].Get(la.kind)) {
						currentState = 397;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(397);
							goto case 399;
						} else {
							if (la.kind == 10) {
								stateStack.Push(397);
								goto case 395;
							} else {
								Error(la);
								goto case 397;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 398;
					break;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (set[138].Get(la.kind)) {
					if (set[139].Get(la.kind)) {
						currentState = 398;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(398);
							goto case 399;
						} else {
							Error(la);
							goto case 398;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 400;
				break;
			}
			case 400: {
				stateStack.Push(401);
				goto case 49;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 16) {
					currentState = 392;
					break;
				} else {
					goto case 392;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(37, la); // "("
				currentState = 404;
				break;
			}
			case 404: {
				readXmlIdentifier = true;
				stateStack.Push(405);
				goto case 167;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(38, la); // ")"
				currentState = 138;
				break;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				Expect(37, la); // "("
				currentState = 407;
				break;
			}
			case 407: {
				stateStack.Push(405);
				goto case 32;
			}
			case 408: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 409;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 10) {
					currentState = 410;
					break;
				} else {
					goto case 410;
				}
			}
			case 410: {
				stateStack.Push(411);
				goto case 74;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 11) {
					currentState = 138;
					break;
				} else {
					goto case 138;
				}
			}
			case 412: {
				stateStack.Push(405);
				goto case 49;
			}
			case 413: {
				stateStack.Push(414);
				goto case 49;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 22) {
					currentState = 415;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 415: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 416;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (set[22].Get(la.kind)) {
					goto case 413;
				} else {
					goto case 414;
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(418);
					goto case 32;
				} else {
					goto case 418;
				}
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 22) {
					currentState = 417;
					break;
				} else {
					goto case 40;
				}
			}
			case 419: {
				SetIdentifierExpected(la);
				goto case 420;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (set[140].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 422;
						break;
					} else {
						if (set[72].Get(la.kind)) {
							stateStack.Push(421);
							goto case 366;
						} else {
							Error(la);
							goto case 421;
						}
					}
				} else {
					goto case 421;
				}
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(38, la); // ")"
				currentState = 29;
				break;
			}
			case 422: {
				stateStack.Push(421);
				goto case 423;
			}
			case 423: {
				SetIdentifierExpected(la);
				goto case 424;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 425;
					break;
				} else {
					goto case 425;
				}
			}
			case 425: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(426);
				goto case 440;
			}
			case 426: {
				PopContext();
				goto case 427;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (la.kind == 63) {
					currentState = 441;
					break;
				} else {
					goto case 428;
				}
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 22) {
					currentState = 429;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 429: {
				SetIdentifierExpected(la);
				goto case 430;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 431;
					break;
				} else {
					goto case 431;
				}
			}
			case 431: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(432);
				goto case 440;
			}
			case 432: {
				PopContext();
				goto case 433;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 63) {
					currentState = 434;
					break;
				} else {
					goto case 428;
				}
			}
			case 434: {
				PushContext(Context.Type, la, t);
				stateStack.Push(435);
				goto case 436;
			}
			case 435: {
				PopContext();
				goto case 428;
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (set[85].Get(la.kind)) {
					goto case 439;
				} else {
					if (la.kind == 35) {
						currentState = 437;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 437: {
				stateStack.Push(438);
				goto case 439;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 22) {
					currentState = 437;
					break;
				} else {
					goto case 59;
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (set[15].Get(la.kind)) {
					currentState = 33;
					break;
				} else {
					if (la.kind == 162) {
						goto case 94;
					} else {
						if (la.kind == 84) {
							goto case 110;
						} else {
							if (la.kind == 209) {
								goto case 85;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 2) {
					goto case 118;
				} else {
					if (la.kind == 62) {
						goto case 116;
					} else {
						if (la.kind == 64) {
							goto case 115;
						} else {
							if (la.kind == 65) {
								goto case 114;
							} else {
								if (la.kind == 66) {
									goto case 113;
								} else {
									if (la.kind == 67) {
										goto case 112;
									} else {
										if (la.kind == 70) {
											goto case 111;
										} else {
											if (la.kind == 87) {
												goto case 109;
											} else {
												if (la.kind == 104) {
													goto case 107;
												} else {
													if (la.kind == 107) {
														goto case 106;
													} else {
														if (la.kind == 116) {
															goto case 104;
														} else {
															if (la.kind == 121) {
																goto case 103;
															} else {
																if (la.kind == 133) {
																	goto case 99;
																} else {
																	if (la.kind == 139) {
																		goto case 98;
																	} else {
																		if (la.kind == 143) {
																			goto case 97;
																		} else {
																			if (la.kind == 146) {
																				goto case 96;
																			} else {
																				if (la.kind == 147) {
																					goto case 95;
																				} else {
																					if (la.kind == 170) {
																						goto case 92;
																					} else {
																						if (la.kind == 176) {
																							goto case 91;
																						} else {
																							if (la.kind == 184) {
																								goto case 90;
																							} else {
																								if (la.kind == 203) {
																									goto case 87;
																								} else {
																									if (la.kind == 212) {
																										goto case 82;
																									} else {
																										if (la.kind == 213) {
																											goto case 81;
																										} else {
																											if (la.kind == 223) {
																												goto case 79;
																											} else {
																												if (la.kind == 224) {
																													goto case 78;
																												} else {
																													if (la.kind == 230) {
																														goto case 77;
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
			case 441: {
				PushContext(Context.Type, la, t);
				stateStack.Push(442);
				goto case 436;
			}
			case 442: {
				PopContext();
				goto case 428;
			}
			case 443: {
				stateStack.Push(444);
				goto case 167;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 37) {
					currentState = 448;
					break;
				} else {
					goto case 445;
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 63) {
					currentState = 446;
					break;
				} else {
					goto case 18;
				}
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 40) {
					stateStack.Push(446);
					goto case 379;
				} else {
					goto case 447;
				}
			}
			case 447: {
				stateStack.Push(18);
				goto case 32;
			}
			case 448: {
				SetIdentifierExpected(la);
				goto case 449;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(450);
					goto case 366;
				} else {
					goto case 450;
				}
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				Expect(38, la); // ")"
				currentState = 445;
				break;
			}
			case 451: {
				stateStack.Push(452);
				goto case 167;
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 447;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 454;
							break;
						} else {
							goto case 453;
						}
					}
				} else {
					goto case 18;
				}
			}
			case 453: {
				Error(la);
				goto case 18;
			}
			case 454: {
				SetIdentifierExpected(la);
				goto case 455;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(456);
					goto case 366;
				} else {
					goto case 456;
				}
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				Expect(38, la); // ")"
				currentState = 18;
				break;
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				Expect(142, la); // "Interface"
				currentState = 9;
				break;
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				Expect(115, la); // "Enum"
				currentState = 459;
				break;
			}
			case 459: {
				stateStack.Push(460);
				goto case 167;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (la.kind == 63) {
					currentState = 467;
					break;
				} else {
					goto case 461;
				}
			}
			case 461: {
				stateStack.Push(462);
				goto case 18;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (set[88].Get(la.kind)) {
					goto case 464;
				} else {
					Expect(113, la); // "End"
					currentState = 463;
					break;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(115, la); // "Enum"
				currentState = 18;
				break;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (la.kind == 40) {
					stateStack.Push(464);
					goto case 379;
				} else {
					stateStack.Push(465);
					goto case 167;
				}
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				if (la.kind == 20) {
					currentState = 466;
					break;
				} else {
					goto case 461;
				}
			}
			case 466: {
				stateStack.Push(461);
				goto case 49;
			}
			case 467: {
				stateStack.Push(461);
				goto case 32;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				Expect(103, la); // "Delegate"
				currentState = 469;
				break;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 210) {
					currentState = 470;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 470;
						break;
					} else {
						Error(la);
						goto case 470;
					}
				}
			}
			case 470: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 471;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				currentState = 472;
				break;
			}
			case 472: {
				PopContext();
				goto case 473;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 37) {
					currentState = 477;
					break;
				} else {
					goto case 474;
				}
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (la.kind == 63) {
					currentState = 475;
					break;
				} else {
					goto case 18;
				}
			}
			case 475: {
				PushContext(Context.Type, la, t);
				stateStack.Push(476);
				goto case 32;
			}
			case 476: {
				PopContext();
				goto case 18;
			}
			case 477: {
				SetIdentifierExpected(la);
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(479);
					goto case 366;
				} else {
					goto case 479;
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				Expect(38, la); // ")"
				currentState = 474;
				break;
			}
			case 480: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 481;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 155) {
					currentState = 482;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 482;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 482;
							break;
						} else {
							Error(la);
							goto case 482;
						}
					}
				}
			}
			case 482: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(483);
				goto case 167;
			}
			case 483: {
				PopContext();
				goto case 484;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 37) {
					currentState = 633;
					break;
				} else {
					goto case 485;
				}
			}
			case 485: {
				stateStack.Push(486);
				goto case 18;
			}
			case 486: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 487;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 630;
				} else {
					goto case 488;
				}
			}
			case 488: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 489;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 624;
				} else {
					goto case 490;
				}
			}
			case 490: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (set[93].Get(la.kind)) {
					goto case 496;
				} else {
					isMissingModifier = false;
					goto case 492;
				}
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				Expect(113, la); // "End"
				currentState = 493;
				break;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 155) {
					currentState = 494;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 494;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 494;
							break;
						} else {
							Error(la);
							goto case 494;
						}
					}
				}
			}
			case 494: {
				stateStack.Push(495);
				goto case 18;
			}
			case 495: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 496: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 497;
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				if (la.kind == 40) {
					stateStack.Push(496);
					goto case 379;
				} else {
					isMissingModifier = true;
					goto case 498;
				}
			}
			case 498: {
				SetIdentifierExpected(la);
				goto case 499;
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				if (set[120].Get(la.kind)) {
					currentState = 623;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 500;
				}
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(490);
					goto case 480;
				} else {
					if (la.kind == 103) {
						stateStack.Push(490);
						goto case 468;
					} else {
						if (la.kind == 115) {
							stateStack.Push(490);
							goto case 458;
						} else {
							if (la.kind == 142) {
								stateStack.Push(490);
								goto case 457;
							} else {
								if (set[96].Get(la.kind)) {
									stateStack.Push(490);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 501;
								} else {
									Error(la);
									goto case 490;
								}
							}
						}
					}
				}
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				if (set[111].Get(la.kind)) {
					stateStack.Push(502);
					goto case 612;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(502);
						goto case 598;
					} else {
						if (la.kind == 101) {
							stateStack.Push(502);
							goto case 582;
						} else {
							if (la.kind == 119) {
								stateStack.Push(502);
								goto case 570;
							} else {
								if (la.kind == 98) {
									stateStack.Push(502);
									goto case 558;
								} else {
									if (la.kind == 186) {
										stateStack.Push(502);
										goto case 516;
									} else {
										if (la.kind == 172) {
											stateStack.Push(502);
											goto case 503;
										} else {
											Error(la);
											goto case 502;
										}
									}
								}
							}
						}
					}
				}
			}
			case 502: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 503: {
				if (la == null) { currentState = 503; break; }
				Expect(172, la); // "Operator"
				currentState = 504;
				break;
			}
			case 504: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 505;
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				currentState = 506;
				break;
			}
			case 506: {
				PopContext();
				goto case 507;
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				Expect(37, la); // "("
				currentState = 508;
				break;
			}
			case 508: {
				stateStack.Push(509);
				goto case 366;
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				Expect(38, la); // ")"
				currentState = 510;
				break;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				if (la.kind == 63) {
					currentState = 514;
					break;
				} else {
					goto case 511;
				}
			}
			case 511: {
				stateStack.Push(512);
				goto case 222;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				Expect(113, la); // "End"
				currentState = 513;
				break;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				Expect(172, la); // "Operator"
				currentState = 18;
				break;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 40) {
					stateStack.Push(514);
					goto case 379;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(515);
					goto case 32;
				}
			}
			case 515: {
				PopContext();
				goto case 511;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				Expect(186, la); // "Property"
				currentState = 517;
				break;
			}
			case 517: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(518);
				goto case 167;
			}
			case 518: {
				PopContext();
				goto case 519;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				if (la.kind == 37) {
					currentState = 555;
					break;
				} else {
					goto case 520;
				}
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (la.kind == 63) {
					currentState = 553;
					break;
				} else {
					goto case 521;
				}
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 136) {
					currentState = 551;
					break;
				} else {
					goto case 522;
				}
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				if (la.kind == 20) {
					currentState = 550;
					break;
				} else {
					goto case 523;
				}
			}
			case 523: {
				stateStack.Push(524);
				goto case 18;
			}
			case 524: {
				PopContext();
				goto case 525;
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				if (la.kind == 40) {
					stateStack.Push(525);
					goto case 379;
				} else {
					goto case 526;
				}
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (set[141].Get(la.kind)) {
					currentState = 549;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 527;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 128) {
					currentState = 528;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 528;
						break;
					} else {
						Error(la);
						goto case 528;
					}
				}
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (la.kind == 37) {
					currentState = 546;
					break;
				} else {
					goto case 529;
				}
			}
			case 529: {
				stateStack.Push(530);
				goto case 222;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				Expect(113, la); // "End"
				currentState = 531;
				break;
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				if (la.kind == 128) {
					currentState = 532;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 532;
						break;
					} else {
						Error(la);
						goto case 532;
					}
				}
			}
			case 532: {
				stateStack.Push(533);
				goto case 18;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (set[102].Get(la.kind)) {
					goto case 536;
				} else {
					goto case 534;
				}
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				Expect(113, la); // "End"
				currentState = 535;
				break;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				Expect(186, la); // "Property"
				currentState = 18;
				break;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				if (la.kind == 40) {
					stateStack.Push(536);
					goto case 379;
				} else {
					goto case 537;
				}
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (set[141].Get(la.kind)) {
					currentState = 537;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 538;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 538;
							break;
						} else {
							Error(la);
							goto case 538;
						}
					}
				}
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 37) {
					currentState = 543;
					break;
				} else {
					goto case 539;
				}
			}
			case 539: {
				stateStack.Push(540);
				goto case 222;
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				Expect(113, la); // "End"
				currentState = 541;
				break;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 128) {
					currentState = 542;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 542;
						break;
					} else {
						Error(la);
						goto case 542;
					}
				}
			}
			case 542: {
				stateStack.Push(534);
				goto case 18;
			}
			case 543: {
				SetIdentifierExpected(la);
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(545);
					goto case 366;
				} else {
					goto case 545;
				}
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(38, la); // ")"
				currentState = 539;
				break;
			}
			case 546: {
				SetIdentifierExpected(la);
				goto case 547;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(548);
					goto case 366;
				} else {
					goto case 548;
				}
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				Expect(38, la); // ")"
				currentState = 529;
				break;
			}
			case 549: {
				SetIdentifierExpected(la);
				goto case 526;
			}
			case 550: {
				stateStack.Push(523);
				goto case 49;
			}
			case 551: {
				stateStack.Push(552);
				goto case 32;
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (la.kind == 22) {
					currentState = 551;
					break;
				} else {
					goto case 522;
				}
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				if (la.kind == 40) {
					stateStack.Push(553);
					goto case 379;
				} else {
					if (la.kind == 162) {
						stateStack.Push(521);
						goto case 60;
					} else {
						if (set[15].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(554);
							goto case 32;
						} else {
							Error(la);
							goto case 521;
						}
					}
				}
			}
			case 554: {
				PopContext();
				goto case 521;
			}
			case 555: {
				SetIdentifierExpected(la);
				goto case 556;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(557);
					goto case 366;
				} else {
					goto case 557;
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				Expect(38, la); // ")"
				currentState = 520;
				break;
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				Expect(98, la); // "Custom"
				currentState = 559;
				break;
			}
			case 559: {
				stateStack.Push(560);
				goto case 570;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				if (set[107].Get(la.kind)) {
					goto case 562;
				} else {
					Expect(113, la); // "End"
					currentState = 561;
					break;
				}
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				Expect(119, la); // "Event"
				currentState = 18;
				break;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				if (la.kind == 40) {
					stateStack.Push(562);
					goto case 379;
				} else {
					if (la.kind == 56) {
						currentState = 563;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 563;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 563;
								break;
							} else {
								Error(la);
								goto case 563;
							}
						}
					}
				}
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				Expect(37, la); // "("
				currentState = 564;
				break;
			}
			case 564: {
				stateStack.Push(565);
				goto case 366;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(38, la); // ")"
				currentState = 566;
				break;
			}
			case 566: {
				stateStack.Push(567);
				goto case 222;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				Expect(113, la); // "End"
				currentState = 568;
				break;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 56) {
					currentState = 569;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 569;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 569;
							break;
						} else {
							Error(la);
							goto case 569;
						}
					}
				}
			}
			case 569: {
				stateStack.Push(560);
				goto case 18;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(119, la); // "Event"
				currentState = 571;
				break;
			}
			case 571: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(572);
				goto case 167;
			}
			case 572: {
				PopContext();
				goto case 573;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (la.kind == 63) {
					currentState = 580;
					break;
				} else {
					if (set[142].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 577;
							break;
						} else {
							goto case 574;
						}
					} else {
						Error(la);
						goto case 574;
					}
				}
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (la.kind == 136) {
					currentState = 575;
					break;
				} else {
					goto case 18;
				}
			}
			case 575: {
				stateStack.Push(576);
				goto case 32;
			}
			case 576: {
				if (la == null) { currentState = 576; break; }
				if (la.kind == 22) {
					currentState = 575;
					break;
				} else {
					goto case 18;
				}
			}
			case 577: {
				SetIdentifierExpected(la);
				goto case 578;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(579);
					goto case 366;
				} else {
					goto case 579;
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				Expect(38, la); // ")"
				currentState = 574;
				break;
			}
			case 580: {
				PushContext(Context.Type, la, t);
				stateStack.Push(581);
				goto case 32;
			}
			case 581: {
				PopContext();
				goto case 574;
			}
			case 582: {
				if (la == null) { currentState = 582; break; }
				Expect(101, la); // "Declare"
				currentState = 583;
				break;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 584;
					break;
				} else {
					goto case 584;
				}
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (la.kind == 210) {
					currentState = 585;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 585;
						break;
					} else {
						Error(la);
						goto case 585;
					}
				}
			}
			case 585: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(586);
				goto case 167;
			}
			case 586: {
				PopContext();
				goto case 587;
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				Expect(149, la); // "Lib"
				currentState = 588;
				break;
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				Expect(3, la); // LiteralString
				currentState = 589;
				break;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (la.kind == 59) {
					currentState = 597;
					break;
				} else {
					goto case 590;
				}
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (la.kind == 37) {
					currentState = 594;
					break;
				} else {
					goto case 591;
				}
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				if (la.kind == 63) {
					currentState = 592;
					break;
				} else {
					goto case 18;
				}
			}
			case 592: {
				PushContext(Context.Type, la, t);
				stateStack.Push(593);
				goto case 32;
			}
			case 593: {
				PopContext();
				goto case 18;
			}
			case 594: {
				SetIdentifierExpected(la);
				goto case 595;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(596);
					goto case 366;
				} else {
					goto case 596;
				}
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				Expect(38, la); // ")"
				currentState = 591;
				break;
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(3, la); // LiteralString
				currentState = 590;
				break;
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				if (la.kind == 210) {
					currentState = 599;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 599;
						break;
					} else {
						Error(la);
						goto case 599;
					}
				}
			}
			case 599: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 600;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				currentState = 601;
				break;
			}
			case 601: {
				PopContext();
				goto case 602;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (la.kind == 37) {
					currentState = 608;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 606;
						break;
					} else {
						goto case 603;
					}
				}
			}
			case 603: {
				stateStack.Push(604);
				goto case 222;
			}
			case 604: {
				if (la == null) { currentState = 604; break; }
				Expect(113, la); // "End"
				currentState = 605;
				break;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				if (la.kind == 210) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 18;
						break;
					} else {
						goto case 453;
					}
				}
			}
			case 606: {
				PushContext(Context.Type, la, t);
				stateStack.Push(607);
				goto case 32;
			}
			case 607: {
				PopContext();
				goto case 603;
			}
			case 608: {
				SetIdentifierExpected(la);
				goto case 609;
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				if (set[140].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 611;
						break;
					} else {
						if (set[72].Get(la.kind)) {
							stateStack.Push(610);
							goto case 366;
						} else {
							Error(la);
							goto case 610;
						}
					}
				} else {
					goto case 610;
				}
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				Expect(38, la); // ")"
				currentState = 602;
				break;
			}
			case 611: {
				stateStack.Push(610);
				goto case 423;
			}
			case 612: {
				stateStack.Push(613);
				SetIdentifierExpected(la);
				goto case 614;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				if (la.kind == 22) {
					currentState = 612;
					break;
				} else {
					goto case 18;
				}
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (la.kind == 88) {
					currentState = 615;
					break;
				} else {
					goto case 615;
				}
			}
			case 615: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(616);
				goto case 622;
			}
			case 616: {
				PopContext();
				goto case 617;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (la.kind == 63) {
					currentState = 619;
					break;
				} else {
					goto case 618;
				}
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				if (la.kind == 20) {
					goto case 180;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 619: {
				PushContext(Context.Type, la, t);
				goto case 620;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (la.kind == 162) {
					stateStack.Push(621);
					goto case 60;
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(621);
						goto case 32;
					} else {
						Error(la);
						goto case 621;
					}
				}
			}
			case 621: {
				PopContext();
				goto case 618;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				if (set[126].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 117;
					} else {
						if (la.kind == 126) {
							goto case 101;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 623: {
				isMissingModifier = false;
				goto case 498;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				Expect(136, la); // "Implements"
				currentState = 625;
				break;
			}
			case 625: {
				stateStack.Push(626);
				goto case 32;
			}
			case 626: {
				PopContext();
				goto case 627;
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				if (la.kind == 22) {
					currentState = 628;
					break;
				} else {
					stateStack.Push(490);
					goto case 18;
				}
			}
			case 628: {
				PushContext(Context.Type, la, t);
				stateStack.Push(629);
				goto case 32;
			}
			case 629: {
				PopContext();
				goto case 627;
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				Expect(140, la); // "Inherits"
				currentState = 631;
				break;
			}
			case 631: {
				stateStack.Push(632);
				goto case 32;
			}
			case 632: {
				PopContext();
				stateStack.Push(488);
				goto case 18;
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				Expect(169, la); // "Of"
				currentState = 634;
				break;
			}
			case 634: {
				stateStack.Push(635);
				goto case 423;
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				Expect(38, la); // ")"
				currentState = 485;
				break;
			}
			case 636: {
				isMissingModifier = false;
				goto case 23;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				Expect(140, la); // "Inherits"
				currentState = 638;
				break;
			}
			case 638: {
				stateStack.Push(639);
				goto case 32;
			}
			case 639: {
				PopContext();
				goto case 640;
			}
			case 640: {
				if (la == null) { currentState = 640; break; }
				if (la.kind == 22) {
					currentState = 641;
					break;
				} else {
					stateStack.Push(14);
					goto case 18;
				}
			}
			case 641: {
				PushContext(Context.Type, la, t);
				stateStack.Push(642);
				goto case 32;
			}
			case 642: {
				PopContext();
				goto case 640;
			}
			case 643: {
				if (la == null) { currentState = 643; break; }
				Expect(169, la); // "Of"
				currentState = 644;
				break;
			}
			case 644: {
				stateStack.Push(645);
				goto case 423;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				Expect(38, la); // ")"
				currentState = 11;
				break;
			}
			case 646: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 647;
			}
			case 647: {
				if (la == null) { currentState = 647; break; }
				if (set[45].Get(la.kind)) {
					currentState = 647;
					break;
				} else {
					PopContext();
					stateStack.Push(648);
					goto case 18;
				}
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(648);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 649;
					break;
				}
			}
			case 649: {
				if (la == null) { currentState = 649; break; }
				Expect(160, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				Expect(137, la); // "Imports"
				currentState = 651;
				break;
			}
			case 651: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 652;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				if (set[143].Get(la.kind)) {
					currentState = 658;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 654;
						break;
					} else {
						Error(la);
						goto case 653;
					}
				}
			}
			case 653: {
				PopContext();
				goto case 18;
			}
			case 654: {
				stateStack.Push(655);
				goto case 167;
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				Expect(20, la); // "="
				currentState = 656;
				break;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				Expect(3, la); // LiteralString
				currentState = 657;
				break;
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 653;
				break;
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				if (la.kind == 37) {
					stateStack.Push(658);
					goto case 37;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 659;
						break;
					} else {
						goto case 653;
					}
				}
			}
			case 659: {
				stateStack.Push(653);
				goto case 32;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				Expect(173, la); // "Option"
				currentState = 661;
				break;
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 663;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 662;
						break;
					} else {
						goto case 453;
					}
				}
			}
			case 662: {
				if (la == null) { currentState = 662; break; }
				if (la.kind == 213) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 18;
						break;
					} else {
						goto case 453;
					}
				}
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
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
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 444604417, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 444596225, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 444596224, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 134234112, 444596224, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671109120, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, 889192437, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, 889192405, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-62257156, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-62257156, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-62257156, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66451460, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007290364, 1140850720, -51384097, -972018405, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66451460, 1174405160, -51384097, -972018401, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66451460, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64354306, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64354306, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66451460, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1174405160, -51384097, -971985637, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66451460, 1174405160, -51384097, -972018405, -1030969166, 17106228, -97186284, 8259}),
		new BitArray(new int[] {1007290364, 1140850720, -51384097, -972002021, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66451460, 1174405160, -51383073, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65402884, 1174409128, -51384097, -971985637, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65402884, 1174409128, -51384097, -972018405, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-64354306, 1191182376, -1051937, -680476901, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {2097154, 32, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483648, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {5242880, -2147483584, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7, 1157628162, 26477055, -493212676, 680323109, 2147308935, -533262382, 3395}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {721920, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850754, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850698, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850690, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671930656, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926560, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 671926304, 1606227075, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493351964, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -493351964, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -1030223452, 135055392, 84218880, -2143942656, 65}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 84218880, -2144073728, 65}),
		new BitArray(new int[] {3145730, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926305, 1606227074, -2143942208, 3393}),
		new BitArray(new int[] {0, 256, 0, 537001984, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 256, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {7340034, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {7340034, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 444596224, 128, 0}),
		new BitArray(new int[] {0, 0, 0, 536871488, 536870912, 1522008194, 384, 3328}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 67108864, -1040187392, 32, 33554432, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 0, -1133776896, 3, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1191182376, -1051937, -680378597, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66451460, 1174405160, -51383585, -972018405, -1030969182, 17106228, -97186288, 8259}),
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
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 0, 0}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67})

	};

} // end Parser


}