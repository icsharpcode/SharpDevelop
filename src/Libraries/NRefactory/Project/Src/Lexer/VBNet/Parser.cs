using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 249;
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
			case 72:
			case 201:
			case 250:
			case 495:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 10:
			case 20:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 11:
			case 177:
			case 183:
			case 189:
			case 227:
			case 231:
			case 276:
			case 376:
			case 385:
			case 441:
			case 482:
			case 492:
			case 503:
			case 533:
			case 569:
			case 626:
			case 643:
			case 719:
				return set[6];
			case 12:
			case 13:
			case 534:
			case 535:
			case 580:
			case 590:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 14:
			case 21:
			case 23:
			case 24:
			case 36:
			case 242:
			case 245:
			case 246:
			case 277:
			case 281:
			case 303:
			case 318:
			case 329:
			case 332:
			case 338:
			case 343:
			case 352:
			case 353:
			case 373:
			case 396:
			case 488:
			case 500:
			case 506:
			case 510:
			case 518:
			case 526:
			case 536:
			case 545:
			case 562:
			case 567:
			case 575:
			case 581:
			case 584:
			case 591:
			case 594:
			case 621:
			case 624:
			case 651:
			case 662:
			case 666:
			case 698:
			case 718:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 15:
			case 16:
				return set[7];
			case 17:
			case 18:
				return set[8];
			case 19:
			case 243:
			case 257:
			case 279:
			case 333:
			case 374:
			case 421:
			case 543:
			case 563:
			case 582:
			case 586:
			case 592:
			case 622:
			case 663:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 511:
			case 546:
				return set[9];
			case 25:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 26:
			case 27:
				return set[10];
			case 28:
			case 702:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 136:
			case 199:
			case 200:
			case 251:
			case 260:
			case 261:
			case 411:
			case 412:
			case 429:
			case 430:
			case 431:
			case 432:
			case 521:
			case 522:
			case 555:
			case 556:
			case 657:
			case 658:
			case 711:
			case 712:
				return set[14];
			case 33:
			case 34:
			case 483:
			case 484:
			case 493:
			case 494:
			case 523:
			case 524:
			case 648:
				return set[15];
			case 35:
			case 37:
			case 141:
			case 152:
			case 155:
			case 171:
			case 187:
			case 204:
			case 288:
			case 313:
			case 395:
			case 408:
			case 444:
			case 499:
			case 517:
			case 525:
			case 603:
			case 606:
			case 630:
			case 633:
			case 638:
			case 650:
			case 665:
			case 668:
			case 691:
			case 694:
			case 697:
			case 703:
			case 706:
			case 724:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 81:
			case 85:
			case 147:
			case 368:
			case 448:
				return set[19];
			case 42:
			case 161:
			case 168:
			case 173:
			case 236:
			case 415:
			case 440:
			case 443:
			case 557:
			case 558:
			case 618:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 149:
			case 150:
				return set[20];
			case 45:
			case 151:
			case 172:
			case 239:
			case 418:
			case 442:
			case 445:
			case 460:
			case 491:
			case 498:
			case 529:
			case 560:
			case 597:
			case 600:
			case 612:
			case 620:
			case 637:
			case 654:
			case 672:
			case 701:
			case 710:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 46:
			case 47:
			case 51:
			case 52:
			case 53:
			case 55:
			case 389:
			case 390:
			case 392:
			case 393:
			case 454:
			case 455:
			case 685:
			case 686:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 163:
			case 170:
			case 371:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 54:
			case 153:
			case 154:
			case 156:
			case 165:
			case 391:
			case 394:
			case 398:
			case 406:
			case 451:
			case 453:
			case 457:
			case 467:
			case 474:
			case 481:
			case 687:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 56:
			case 57:
			case 59:
			case 60:
			case 61:
			case 67:
			case 83:
			case 139:
			case 162:
			case 164:
			case 166:
			case 169:
			case 179:
			case 181:
			case 222:
			case 265:
			case 267:
			case 268:
			case 285:
			case 302:
			case 307:
			case 316:
			case 322:
			case 324:
			case 328:
			case 331:
			case 337:
			case 348:
			case 350:
			case 356:
			case 370:
			case 372:
			case 407:
			case 434:
			case 450:
			case 452:
			case 516:
			case 602:
				return set[23];
			case 58:
			case 62:
			case 142:
				return set[24];
			case 63:
			case 75:
			case 77:
			case 132:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 64:
			case 65:
				return set[25];
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 68:
			case 84:
			case 477:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 69:
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 70:
			case 71:
				return set[26];
			case 73:
			case 76:
			case 133:
			case 134:
			case 137:
				return set[27];
			case 74:
			case 86:
			case 131:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(36, true);
					a.Set(147, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 80:
			case 669:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 82:
			case 203:
			case 205:
			case 206:
			case 264:
			case 315:
			case 720:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 87:
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 91:
			case 280:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 95:
			case 422:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 99:
			case 340:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 100:
			case 568:
			case 587:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 104:
			case 297:
			case 304:
			case 319:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 107:
			case 209:
			case 214:
			case 216:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 108:
			case 211:
			case 215:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 111:
			case 244:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 112:
			case 135:
			case 234:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 115:
			case 180:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 119:
			case 613:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 120:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 121:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 122:
			case 192:
			case 221:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 124:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 125:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 126:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 127:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 128:
			case 233:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 129:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 130:
				return set[28];
			case 138:
				return set[29];
			case 140:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 143:
				return set[30];
			case 144:
				return set[31];
			case 145:
			case 146:
			case 446:
			case 447:
				return set[32];
			case 148:
				return set[33];
			case 157:
			case 158:
			case 300:
			case 309:
				return set[34];
			case 159:
			case 424:
				return set[35];
			case 160:
			case 355:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 167:
				return set[36];
			case 174:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 175:
			case 176:
				return set[37];
			case 178:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 182:
			case 196:
			case 213:
			case 218:
			case 224:
			case 226:
			case 230:
			case 232:
				return set[38];
			case 184:
			case 185:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 186:
			case 188:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 190:
			case 191:
			case 193:
			case 195:
			case 197:
			case 198:
			case 207:
			case 212:
			case 217:
			case 225:
			case 229:
			case 255:
			case 259:
				return set[39];
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 202:
				return set[40];
			case 208:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 210:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 219:
			case 220:
				return set[41];
			case 223:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 228:
				return set[42];
			case 235:
			case 520:
			case 642:
			case 656:
			case 664:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 237:
			case 238:
			case 416:
			case 417:
			case 489:
			case 490:
			case 496:
			case 497:
			case 595:
			case 596:
			case 598:
			case 599:
			case 610:
			case 611:
			case 635:
			case 636:
			case 652:
			case 653:
				return set[43];
			case 240:
			case 241:
				return set[44];
			case 247:
			case 248:
				return set[45];
			case 249:
				return set[46];
			case 252:
				return set[47];
			case 253:
			case 254:
			case 361:
				return set[48];
			case 256:
			case 346:
			case 631:
			case 632:
			case 634:
			case 675:
			case 692:
			case 693:
			case 695:
			case 704:
			case 705:
			case 707:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 258:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 262:
			case 263:
				return set[49];
			case 266:
			case 308:
			case 323:
				return set[50];
			case 269:
			case 270:
			case 290:
			case 291:
			case 305:
			case 306:
			case 320:
			case 321:
				return set[51];
			case 271:
			case 362:
			case 365:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 273:
				return set[52];
			case 274:
			case 293:
				return set[53];
			case 275:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 278:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 282:
			case 283:
				return set[54];
			case 284:
			case 289:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 286:
			case 287:
				return set[55];
			case 292:
				return set[56];
			case 294:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 295:
			case 296:
				return set[57];
			case 298:
			case 299:
				return set[58];
			case 310:
			case 311:
				return set[59];
			case 312:
				return set[60];
			case 314:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 317:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 325:
				return set[61];
			case 326:
			case 330:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 327:
				return set[62];
			case 335:
			case 336:
				return set[63];
			case 339:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 341:
			case 342:
				return set[64];
			case 344:
			case 345:
				return set[65];
			case 347:
			case 349:
				return set[66];
			case 351:
			case 357:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 354:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 358:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 359:
			case 360:
			case 419:
			case 420:
				return set[67];
			case 363:
			case 364:
			case 366:
			case 367:
				return set[68];
			case 369:
				return set[69];
			case 375:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 377:
			case 378:
			case 386:
			case 387:
				return set[70];
			case 379:
			case 388:
				return set[71];
			case 380:
				return set[72];
			case 381:
			case 384:
				return set[73];
			case 382:
			case 383:
			case 682:
			case 683:
				return set[74];
			case 397:
			case 399:
			case 400:
			case 559:
			case 619:
				return set[75];
			case 401:
			case 402:
				return set[76];
			case 403:
			case 404:
				return set[77];
			case 405:
			case 409:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 410:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 413:
			case 414:
				return set[78];
			case 423:
				return set[79];
			case 425:
			case 438:
				return set[80];
			case 426:
			case 439:
				return set[81];
			case 427:
			case 428:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 433:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 435:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 436:
				return set[82];
			case 437:
				return set[83];
			case 449:
				return set[84];
			case 456:
				return set[85];
			case 458:
			case 459:
			case 527:
			case 528:
			case 670:
			case 671:
				return set[86];
			case 461:
			case 462:
			case 463:
			case 468:
			case 469:
			case 530:
			case 673:
			case 700:
			case 709:
				return set[87];
			case 464:
			case 470:
			case 479:
				return set[88];
			case 465:
			case 466:
			case 471:
			case 472:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 473:
			case 475:
			case 480:
				return set[89];
			case 476:
			case 478:
				return set[90];
			case 485:
			case 504:
			case 505:
			case 561:
			case 649:
			case 661:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 486:
			case 487:
			case 565:
			case 566:
				return set[91];
			case 501:
			case 502:
			case 509:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 507:
			case 508:
				return set[92];
			case 512:
			case 513:
				return set[93];
			case 514:
			case 515:
			case 574:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 519:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 531:
			case 532:
			case 544:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 537:
			case 538:
				return set[94];
			case 539:
			case 540:
				return set[95];
			case 541:
			case 542:
			case 553:
				return set[96];
			case 547:
			case 548:
				return set[97];
			case 549:
			case 550:
			case 689:
				return set[98];
			case 551:
				return set[99];
			case 552:
				return set[100];
			case 554:
			case 564:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 570:
			case 571:
				return set[101];
			case 572:
				return set[102];
			case 573:
			case 609:
				return set[103];
			case 576:
			case 577:
			case 578:
			case 601:
				return set[104];
			case 579:
			case 583:
			case 593:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 585:
				return set[105];
			case 588:
				return set[106];
			case 589:
				return set[107];
			case 604:
			case 605:
			case 607:
			case 681:
			case 684:
				return set[108];
			case 608:
				return set[109];
			case 614:
			case 616:
			case 625:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 615:
				return set[110];
			case 617:
				return set[111];
			case 623:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 627:
			case 628:
				return set[112];
			case 629:
			case 639:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 640:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 641:
				return set[113];
			case 644:
			case 645:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 646:
			case 655:
			case 721:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 647:
				return set[114];
			case 659:
			case 660:
				return set[115];
			case 667:
				return set[116];
			case 674:
			case 676:
				return set[117];
			case 677:
			case 688:
				return set[118];
			case 678:
			case 679:
				return set[119];
			case 680:
				return set[120];
			case 690:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 696:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 699:
			case 708:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 713:
				return set[121];
			case 714:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 715:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 716:
			case 717:
				return set[122];
			case 722:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 723:
				return set[123];
			case 725:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 726:
				return set[124];
			case 727:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 728:
				return set[125];
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
	int activeArgument = 0;
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
					goto case 725;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 715;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 410;
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
					currentState = 711;
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
					goto case 410;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[126].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 531;
					} else {
						if (la.kind == 103) {
							currentState = 520;
							break;
						} else {
							if (la.kind == 115) {
								goto case 501;
							} else {
								if (la.kind == 142) {
									goto case 9;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 10;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				Expect(142, la); // "Interface"
				currentState = 11;
				break;
			}
			case 11: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(12);
				goto case 189;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 708;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				stateStack.Push(15);
				goto case 23;
			}
			case 15: {
				isMissingModifier = true;
				goto case 16;
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.kind == 140) {
					currentState = 703;
					break;
				} else {
					goto case 17;
				}
			}
			case 17: {
				isMissingModifier = true;
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[10].Get(la.kind)) {
					goto case 26;
				} else {
					isMissingModifier = false;
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				Expect(113, la); // "End"
				currentState = 20;
				break;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				Expect(142, la); // "Interface"
				currentState = 21;
				break;
			}
			case 21: {
				stateStack.Push(22);
				goto case 23;
			}
			case 22: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 23: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 24;
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 26: {
				isMissingModifier = true;
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				if (la.kind == 40) {
					stateStack.Push(26);
					goto case 410;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[127].Get(la.kind)) {
					currentState = 702;
					break;
				} else {
					isMissingModifier = false;
					goto case 29;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(17);
					goto case 531;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 519;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 501;
						} else {
							if (la.kind == 142) {
								stateStack.Push(17);
								goto case 9;
							} else {
								if (set[13].Get(la.kind)) {
									stateStack.Push(17);
									goto case 30;
								} else {
									Error(la);
									goto case 17;
								}
							}
						}
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (la.kind == 119) {
					currentState = 492;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 482;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 31;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 31: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				currentState = 33;
				break;
			}
			case 33: {
				PopContext();
				goto case 34;
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 37) {
					currentState = 458;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 35;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 35: {
				PushContext(Context.Type, la, t);
				stateStack.Push(36);
				goto case 37;
			}
			case 36: {
				PopContext();
				goto case 23;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (la.kind == 130) {
					currentState = 38;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 38;
						break;
					} else {
						if (set[128].Get(la.kind)) {
							currentState = 38;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 38;
								break;
							} else {
								Error(la);
								goto case 38;
							}
						}
					}
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (la.kind == 37) {
					stateStack.Push(38);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 26) {
					currentState = 40;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 40: {
				stateStack.Push(41);
				goto case 85;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 37) {
					stateStack.Push(41);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(37, la); // "("
				currentState = 43;
				break;
			}
			case 43: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 169) {
					currentState = 456;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						goto case 46;
					} else {
						Error(la);
						goto case 45;
					}
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 47;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(45);
					goto case 48;
				} else {
					goto case 45;
				}
			}
			case 48: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 49;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (set[23].Get(la.kind)) {
					activeArgument = 0;
					goto case 452;
				} else {
					if (la.kind == 22) {
						activeArgument = 0;
						goto case 50;
					} else {
						goto case 6;
					}
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				Expect(22, la); // ","
				currentState = 51;
				break;
			}
			case 51: {
				activeArgument++;
				goto case 52;
			}
			case 52: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 53;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(54);
					goto case 56;
				} else {
					goto case 54;
				}
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (la.kind == 22) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 55: {
				activeArgument++;
				goto case 52;
			}
			case 56: {
				PushContext(Context.Expression, la, t);
				goto case 57;
			}
			case 57: {
				stateStack.Push(58);
				goto case 59;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (set[129].Get(la.kind)) {
					currentState = 57;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 59: {
				PushContext(Context.Expression, la, t);
				goto case 60;
			}
			case 60: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 61;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (set[130].Get(la.kind)) {
					currentState = 60;
					break;
				} else {
					if (set[34].Get(la.kind)) {
						stateStack.Push(143);
						goto case 157;
					} else {
						if (la.kind == 220) {
							currentState = 139;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(62);
								goto case 69;
							} else {
								if (la.kind == 35) {
									stateStack.Push(62);
									goto case 63;
								} else {
									Error(la);
									goto case 62;
								}
							}
						}
					}
				}
			}
			case 62: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				Expect(35, la); // "{"
				currentState = 64;
				break;
			}
			case 64: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 65;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (set[23].Get(la.kind)) {
					goto case 67;
				} else {
					goto case 66;
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				stateStack.Push(68);
				goto case 56;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 22) {
					currentState = 67;
					break;
				} else {
					goto case 66;
				}
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				Expect(162, la); // "New"
				currentState = 70;
				break;
			}
			case 70: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 71;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(130);
					goto case 37;
				} else {
					if (la.kind == 233) {
						PushContext(Context.ObjectInitializer, la, t);
						goto case 74;
					} else {
						goto case 72;
					}
				}
			}
			case 72: {
				Error(la);
				goto case 73;
			}
			case 73: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(233, la); // "With"
				currentState = 75;
				break;
			}
			case 75: {
				stateStack.Push(76);
				goto case 77;
			}
			case 76: {
				PopContext();
				goto case 73;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(35, la); // "{"
				currentState = 78;
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				if (la.kind == 26 || la.kind == 147) {
					goto case 79;
				} else {
					goto case 66;
				}
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				if (la.kind == 147) {
					currentState = 80;
					break;
				} else {
					goto case 80;
				}
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				Expect(26, la); // "."
				currentState = 81;
				break;
			}
			case 81: {
				stateStack.Push(82);
				goto case 85;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				Expect(20, la); // "="
				currentState = 83;
				break;
			}
			case 83: {
				stateStack.Push(84);
				goto case 56;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.kind == 22) {
					currentState = 79;
					break;
				} else {
					goto case 66;
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				if (la.kind == 2) {
					goto case 129;
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
								goto case 128;
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
												goto case 127;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 126;
													} else {
														if (la.kind == 65) {
															goto case 125;
														} else {
															if (la.kind == 66) {
																goto case 124;
															} else {
																if (la.kind == 67) {
																	goto case 123;
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
																				goto case 122;
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
																																		goto case 121;
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
																																					goto case 120;
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
																																																goto case 119;
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
																																																						goto case 118;
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
																																																									goto case 117;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 116;
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
																																																																		goto case 115;
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
																																																																							goto case 114;
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
																																																																										goto case 113;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 112;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 111;
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
																																																																																			goto case 110;
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
																																																																																									goto case 109;
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
																																																																																													goto case 108;
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
																																																																																																goto case 107;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 106;
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
																																																																																																																goto case 105;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 104;
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
																																																																																																																								goto case 103;
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
																																																																																																																														goto case 102;
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
																																																																																																																																						goto case 101;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 100;
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
																																																																																																																																																			goto case 99;
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
																																																																																																																																																									goto case 98;
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
																																																																																																																																																												goto case 97;
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
																																																																																																																																																															goto case 96;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 95;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 94;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 93;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 92;
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
																																																																																																																																																																								goto case 91;
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
																																																																																																																																																																													goto case 90;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 89;
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
																																																																																																																																																																																				goto case 88;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 87;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 86;
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
				currentState = stateStack.Pop();
				break;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 35 || la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						PushContext(Context.CollectionInitializer, la, t);
						goto case 135;
					} else {
						if (la.kind == 35) {
							PushContext(Context.CollectionInitializer, la, t);
							stateStack.Push(134);
							goto case 63;
						} else {
							if (la.kind == 233) {
								PushContext(Context.ObjectInitializer, la, t);
								goto case 131;
							} else {
								goto case 72;
							}
						}
					}
				} else {
					goto case 73;
				}
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				Expect(233, la); // "With"
				currentState = 132;
				break;
			}
			case 132: {
				stateStack.Push(133);
				goto case 77;
			}
			case 133: {
				PopContext();
				goto case 73;
			}
			case 134: {
				PopContext();
				goto case 73;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				Expect(126, la); // "From"
				currentState = 136;
				break;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 35) {
					stateStack.Push(137);
					goto case 63;
				} else {
					if (set[29].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						Error(la);
						goto case 137;
					}
				}
			}
			case 137: {
				PopContext();
				goto case 73;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				currentState = 137;
				break;
			}
			case 139: {
				stateStack.Push(140);
				goto case 59;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(144, la); // "Is"
				currentState = 141;
				break;
			}
			case 141: {
				PushContext(Context.Type, la, t);
				stateStack.Push(142);
				goto case 37;
			}
			case 142: {
				PopContext();
				goto case 62;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(143);
					goto case 144;
				} else {
					goto case 62;
				}
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 37) {
					currentState = 149;
					break;
				} else {
					if (set[131].Get(la.kind)) {
						currentState = 145;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 145: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 146;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 10) {
					currentState = 147;
					break;
				} else {
					goto case 147;
				}
			}
			case 147: {
				stateStack.Push(148);
				goto case 85;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 149: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 150;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 169) {
					currentState = 152;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						if (set[22].Get(la.kind)) {
							stateStack.Push(151);
							goto case 48;
						} else {
							goto case 151;
						}
					} else {
						Error(la);
						goto case 151;
					}
				}
			}
			case 151: {
				PopContext();
				goto case 45;
			}
			case 152: {
				PushContext(Context.Type, la, t);
				stateStack.Push(153);
				goto case 37;
			}
			case 153: {
				PopContext();
				goto case 154;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 22) {
					currentState = 155;
					break;
				} else {
					goto case 151;
				}
			}
			case 155: {
				PushContext(Context.Type, la, t);
				stateStack.Push(156);
				goto case 37;
			}
			case 156: {
				PopContext();
				goto case 154;
			}
			case 157: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 158;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (set[132].Get(la.kind)) {
					currentState = 159;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 450;
						break;
					} else {
						if (set[133].Get(la.kind)) {
							currentState = 159;
							break;
						} else {
							if (set[128].Get(la.kind)) {
								currentState = 159;
								break;
							} else {
								if (set[131].Get(la.kind)) {
									currentState = 446;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 443;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 440;
											break;
										} else {
											if (set[79].Get(la.kind)) {
												stateStack.Push(159);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 423;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(159);
													goto case 235;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(159);
														PushContext(Context.Query, la, t);
														goto case 174;
													} else {
														if (set[36].Get(la.kind)) {
															stateStack.Push(159);
															goto case 167;
														} else {
															if (la.kind == 135) {
																stateStack.Push(159);
																goto case 160;
															} else {
																Error(la);
																goto case 159;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 159: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				Expect(135, la); // "If"
				currentState = 161;
				break;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				Expect(37, la); // "("
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 56;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				Expect(22, la); // ","
				currentState = 164;
				break;
			}
			case 164: {
				stateStack.Push(165);
				goto case 56;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 22) {
					currentState = 166;
					break;
				} else {
					goto case 45;
				}
			}
			case 166: {
				stateStack.Push(45);
				goto case 56;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (set[134].Get(la.kind)) {
					currentState = 173;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 168;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				Expect(37, la); // "("
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 56;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				Expect(22, la); // ","
				currentState = 171;
				break;
			}
			case 171: {
				PushContext(Context.Type, la, t);
				stateStack.Push(172);
				goto case 37;
			}
			case 172: {
				PopContext();
				goto case 45;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				Expect(37, la); // "("
				currentState = 166;
				break;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 126) {
					stateStack.Push(175);
					goto case 234;
				} else {
					if (la.kind == 58) {
						stateStack.Push(175);
						goto case 233;
					} else {
						Error(la);
						goto case 175;
					}
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (set[37].Get(la.kind)) {
					stateStack.Push(175);
					goto case 176;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 126) {
					currentState = 231;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 227;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 225;
							break;
						} else {
							if (la.kind == 107) {
								goto case 117;
							} else {
								if (la.kind == 230) {
									currentState = 56;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 221;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 219;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 217;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 190;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 177;
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
			case 177: {
				stateStack.Push(178);
				goto case 183;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				Expect(171, la); // "On"
				currentState = 179;
				break;
			}
			case 179: {
				stateStack.Push(180);
				goto case 56;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(116, la); // "Equals"
				currentState = 181;
				break;
			}
			case 181: {
				stateStack.Push(182);
				goto case 56;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				if (la.kind == 22) {
					currentState = 179;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 183: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(184);
				goto case 189;
			}
			case 184: {
				PopContext();
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 63) {
					currentState = 187;
					break;
				} else {
					goto case 186;
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(138, la); // "In"
				currentState = 56;
				break;
			}
			case 187: {
				PushContext(Context.Type, la, t);
				stateStack.Push(188);
				goto case 37;
			}
			case 188: {
				PopContext();
				goto case 186;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (set[118].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 119;
					} else {
						goto case 6;
					}
				}
			}
			case 190: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 191;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.kind == 146) {
					goto case 209;
				} else {
					if (set[39].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 193;
							break;
						} else {
							if (set[39].Get(la.kind)) {
								goto case 207;
							} else {
								Error(la);
								goto case 192;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				Expect(70, la); // "By"
				currentState = 193;
				break;
			}
			case 193: {
				stateStack.Push(194);
				goto case 197;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.kind == 22) {
					currentState = 193;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 195;
					break;
				}
			}
			case 195: {
				stateStack.Push(196);
				goto case 197;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.kind == 22) {
					currentState = 195;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 197: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 198;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(199);
					goto case 189;
				} else {
					goto case 56;
				}
			}
			case 199: {
				PopContext();
				nextTokenIsPotentialStartOfExpression = true;
				goto case 200;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (set[135].Get(la.kind)) {
					if (la.kind == 63) {
						currentState = 204;
						break;
					} else {
						if (la.kind == 20) {
							goto case 203;
						} else {
							if (set[40].Get(la.kind)) {
								currentState = endOfStatementTerminatorAndBlock; /* leave this block */
									InformToken(t); /* process Identifier again*/
									/* for processing current token (la): go to the position after processing End */
									goto switchlbl;

							} else {
								goto case 201;
							}
						}
					}
				} else {
					goto case 56;
				}
			}
			case 201: {
				Error(la);
				goto case 56;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				currentState = 56;
				break;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				currentState = 56;
				break;
			}
			case 204: {
				PushContext(Context.Type, la, t);
				stateStack.Push(205);
				goto case 37;
			}
			case 205: {
				PopContext();
				goto case 206;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 207: {
				stateStack.Push(208);
				goto case 197;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 22) {
					currentState = 207;
					break;
				} else {
					goto case 192;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 216;
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 214;
						break;
					} else {
						if (la.kind == 146) {
							goto case 209;
						} else {
							Error(la);
							goto case 210;
						}
					}
				} else {
					goto case 211;
				}
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(143, la); // "Into"
				currentState = 212;
				break;
			}
			case 212: {
				stateStack.Push(213);
				goto case 197;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.kind == 22) {
					currentState = 212;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 214: {
				stateStack.Push(215);
				goto case 216;
			}
			case 215: {
				stateStack.Push(210);
				goto case 211;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(146, la); // "Join"
				currentState = 177;
				break;
			}
			case 217: {
				stateStack.Push(218);
				goto case 197;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 220;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(70, la); // "By"
				currentState = 222;
				break;
			}
			case 222: {
				stateStack.Push(223);
				goto case 56;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.kind == 64) {
					currentState = 224;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 224;
						break;
					} else {
						Error(la);
						goto case 224;
					}
				}
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 22) {
					currentState = 222;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 225: {
				stateStack.Push(226);
				goto case 197;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.kind == 22) {
					currentState = 225;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 227: {
				stateStack.Push(228);
				goto case 183;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (set[37].Get(la.kind)) {
					stateStack.Push(228);
					goto case 176;
				} else {
					Expect(143, la); // "Into"
					currentState = 229;
					break;
				}
			}
			case 229: {
				stateStack.Push(230);
				goto case 197;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 22) {
					currentState = 229;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 231: {
				stateStack.Push(232);
				goto case 183;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (la.kind == 22) {
					currentState = 231;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				Expect(58, la); // "Aggregate"
				currentState = 227;
				break;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(126, la); // "From"
				currentState = 231;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 210) {
					currentState = 415;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 236;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(37, la); // "("
				currentState = 237;
				break;
			}
			case 237: {
				SetIdentifierExpected(la);
				goto case 238;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(239);
					goto case 397;
				} else {
					goto case 239;
				}
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				Expect(38, la); // ")"
				currentState = 240;
				break;
			}
			case 240: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 395;
							break;
						} else {
							goto case 242;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 242: {
				stateStack.Push(243);
				goto case 245;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				Expect(113, la); // "End"
				currentState = 244;
				break;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 245: {
				PushContext(Context.Body, la, t);
				goto case 246;
			}
			case 246: {
				stateStack.Push(247);
				goto case 23;
			}
			case 247: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 248;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (set[136].Get(la.kind)) {
					if (set[67].Get(la.kind)) {
						if (set[48].Get(la.kind)) {
							stateStack.Push(246);
							goto case 253;
						} else {
							goto case 246;
						}
					} else {
						if (la.kind == 113) {
							currentState = 251;
							break;
						} else {
							goto case 250;
						}
					}
				} else {
					goto case 249;
				}
			}
			case 249: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 250: {
				Error(la);
				goto case 247;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 246;
				} else {
					if (set[47].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 250;
					}
				}
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				currentState = 247;
				break;
			}
			case 253: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 254;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 376;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 372;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 370;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 368;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 350;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 335;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 331;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 325;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 298;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 294;
																break;
															} else {
																goto case 294;
															}
														} else {
															if (la.kind == 194) {
																currentState = 292;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 290;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 277;
																break;
															} else {
																if (set[137].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 274;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 273;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 272;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 97;
																				} else {
																					if (la.kind == 195) {
																						currentState = 269;
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
																		currentState = 267;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 265;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 255;
																				break;
																			} else {
																				if (set[138].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 56;
																						break;
																					} else {
																						goto case 56;
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
			case 255: {
				stateStack.Push(256);
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 259;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (la.kind == 22) {
					currentState = 255;
					break;
				} else {
					stateStack.Push(257);
					goto case 245;
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				Expect(113, la); // "End"
				currentState = 258;
				break;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(260);
					goto case 189;
				} else {
					goto case 56;
				}
			}
			case 260: {
				PopContext();
				nextTokenIsPotentialStartOfExpression = true;
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[135].Get(la.kind)) {
					if (la.kind == 63) {
						currentState = 262;
						break;
					} else {
						if (la.kind == 20) {
							goto case 203;
						} else {
							if (set[40].Get(la.kind)) {
								currentState = endOfStatementTerminatorAndBlock; /* leave this block */
									InformToken(t); /* process Identifier again*/
									/* for processing current token (la): go to the position after processing End */
									goto switchlbl;

							} else {
								goto case 201;
							}
						}
					}
				} else {
					goto case 56;
				}
			}
			case 262: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 263;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(264);
					goto case 37;
				} else {
					goto case 56;
				}
			}
			case 264: {
				PopContext();
				goto case 206;
			}
			case 265: {
				stateStack.Push(266);
				goto case 56;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.kind == 22) {
					currentState = 265;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 269: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(271);
					goto case 56;
				} else {
					goto case 271;
				}
			}
			case 271: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (la.kind == 108) {
					goto case 116;
				} else {
					if (la.kind == 124) {
						goto case 113;
					} else {
						if (la.kind == 231) {
							goto case 87;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 108) {
					goto case 116;
				} else {
					if (la.kind == 124) {
						goto case 113;
					} else {
						if (la.kind == 231) {
							goto case 87;
						} else {
							if (la.kind == 197) {
								goto case 99;
							} else {
								if (la.kind == 210) {
									goto case 95;
								} else {
									if (la.kind == 127) {
										goto case 111;
									} else {
										if (la.kind == 186) {
											goto case 100;
										} else {
											if (la.kind == 218) {
												goto case 91;
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
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[6].Get(la.kind)) {
					goto case 276;
				} else {
					if (la.kind == 5) {
						goto case 275;
					} else {
						goto case 6;
					}
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 277: {
				stateStack.Push(278);
				goto case 245;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (la.kind == 75) {
					currentState = 282;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 281;
						break;
					} else {
						goto case 279;
					}
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(113, la); // "End"
				currentState = 280;
				break;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 281: {
				stateStack.Push(279);
				goto case 245;
			}
			case 282: {
				SetIdentifierExpected(la);
				goto case 283;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(286);
					goto case 189;
				} else {
					goto case 284;
				}
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 229) {
					currentState = 285;
					break;
				} else {
					goto case 277;
				}
			}
			case 285: {
				stateStack.Push(277);
				goto case 56;
			}
			case 286: {
				PopContext();
				goto case 287;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 63) {
					currentState = 288;
					break;
				} else {
					goto case 284;
				}
			}
			case 288: {
				PushContext(Context.Type, la, t);
				stateStack.Push(289);
				goto case 37;
			}
			case 289: {
				PopContext();
				goto case 284;
			}
			case 290: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 291;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 163) {
					goto case 104;
				} else {
					goto case 293;
				}
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 5) {
					goto case 275;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 276;
					} else {
						goto case 6;
					}
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				Expect(118, la); // "Error"
				currentState = 295;
				break;
			}
			case 295: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 296;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 293;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 297;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 299;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(315);
					goto case 309;
				} else {
					if (la.kind == 110) {
						currentState = 300;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 309;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(138, la); // "In"
				currentState = 302;
				break;
			}
			case 302: {
				stateStack.Push(303);
				goto case 56;
			}
			case 303: {
				stateStack.Push(304);
				goto case 245;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(163, la); // "Next"
				currentState = 305;
				break;
			}
			case 305: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 306;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (set[23].Get(la.kind)) {
					goto case 307;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 307: {
				stateStack.Push(308);
				goto case 56;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 22) {
					currentState = 307;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 309: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(310);
				goto case 157;
			}
			case 310: {
				PopContext();
				goto case 311;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (la.kind == 33) {
					currentState = 312;
					break;
				} else {
					goto case 312;
				}
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(312);
					goto case 144;
				} else {
					if (la.kind == 63) {
						currentState = 313;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 313: {
				PushContext(Context.Type, la, t);
				stateStack.Push(314);
				goto case 37;
			}
			case 314: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(20, la); // "="
				currentState = 316;
				break;
			}
			case 316: {
				stateStack.Push(317);
				goto case 56;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 205) {
					currentState = 324;
					break;
				} else {
					goto case 318;
				}
			}
			case 318: {
				stateStack.Push(319);
				goto case 245;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(163, la); // "Next"
				currentState = 320;
				break;
			}
			case 320: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 321;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (set[23].Get(la.kind)) {
					goto case 322;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 322: {
				stateStack.Push(323);
				goto case 56;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 22) {
					currentState = 322;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 324: {
				stateStack.Push(318);
				goto case 56;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 328;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(326);
						goto case 245;
					} else {
						goto case 6;
					}
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(152, la); // "Loop"
				currentState = 327;
				break;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 328: {
				stateStack.Push(329);
				goto case 56;
			}
			case 329: {
				stateStack.Push(330);
				goto case 245;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 331: {
				stateStack.Push(332);
				goto case 56;
			}
			case 332: {
				stateStack.Push(333);
				goto case 245;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(113, la); // "End"
				currentState = 334;
				break;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 336;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 74) {
					currentState = 337;
					break;
				} else {
					goto case 337;
				}
			}
			case 337: {
				stateStack.Push(338);
				goto case 56;
			}
			case 338: {
				stateStack.Push(339);
				goto case 23;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 74) {
					currentState = 341;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 340;
					break;
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 341: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 111) {
					currentState = 343;
					break;
				} else {
					if (set[65].Get(la.kind)) {
						goto case 344;
					} else {
						Error(la);
						goto case 343;
					}
				}
			}
			case 343: {
				stateStack.Push(339);
				goto case 245;
			}
			case 344: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 345;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (set[139].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 347;
						break;
					} else {
						goto case 347;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(346);
						goto case 56;
					} else {
						Error(la);
						goto case 346;
					}
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 22) {
					currentState = 344;
					break;
				} else {
					goto case 343;
				}
			}
			case 347: {
				stateStack.Push(348);
				goto case 349;
			}
			case 348: {
				stateStack.Push(346);
				goto case 59;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
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
			case 350: {
				stateStack.Push(351);
				goto case 56;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 214) {
					currentState = 359;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 353;
				} else {
					goto case 6;
				}
			}
			case 353: {
				stateStack.Push(354);
				goto case 245;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 358;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 356;
							break;
						} else {
							Error(la);
							goto case 353;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 355;
					break;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 356: {
				stateStack.Push(357);
				goto case 56;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 214) {
					currentState = 353;
					break;
				} else {
					goto case 353;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 135) {
					currentState = 356;
					break;
				} else {
					goto case 353;
				}
			}
			case 359: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 360;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (set[48].Get(la.kind)) {
					goto case 361;
				} else {
					goto case 352;
				}
			}
			case 361: {
				stateStack.Push(362);
				goto case 253;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 21) {
					currentState = 366;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 363;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 363: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 364;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (set[48].Get(la.kind)) {
					stateStack.Push(365);
					goto case 253;
				} else {
					goto case 365;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 21) {
					currentState = 363;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 366: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (set[48].Get(la.kind)) {
					goto case 361;
				} else {
					goto case 362;
				}
			}
			case 368: {
				stateStack.Push(369);
				goto case 85;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 370: {
				stateStack.Push(371);
				goto case 56;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 372: {
				stateStack.Push(373);
				goto case 56;
			}
			case 373: {
				stateStack.Push(374);
				goto case 245;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				Expect(113, la); // "End"
				currentState = 375;
				break;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 233) {
					goto case 86;
				} else {
					if (la.kind == 211) {
						goto case 94;
					} else {
						goto case 6;
					}
				}
			}
			case 376: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(377);
				goto case 189;
			}
			case 377: {
				PopContext();
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 33) {
					currentState = 379;
					break;
				} else {
					goto case 379;
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 37) {
					currentState = 392;
					break;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 22) {
					currentState = 385;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 382;
						break;
					} else {
						goto case 381;
					}
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (la.kind == 20) {
					goto case 203;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 382: {
				PushContext(Context.Type, la, t);
				goto case 383;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 162) {
					stateStack.Push(384);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(384);
						goto case 37;
					} else {
						Error(la);
						goto case 384;
					}
				}
			}
			case 384: {
				PopContext();
				goto case 381;
			}
			case 385: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(386);
				goto case 189;
			}
			case 386: {
				PopContext();
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 33) {
					currentState = 388;
					break;
				} else {
					goto case 388;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 37) {
					currentState = 389;
					break;
				} else {
					goto case 380;
				}
			}
			case 389: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 390;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(391);
					goto case 56;
				} else {
					goto case 391;
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 22) {
					currentState = 389;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 388;
					break;
				}
			}
			case 392: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(394);
					goto case 56;
				} else {
					goto case 394;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 22) {
					currentState = 392;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 379;
					break;
				}
			}
			case 395: {
				PushContext(Context.Type, la, t);
				stateStack.Push(396);
				goto case 37;
			}
			case 396: {
				PopContext();
				goto case 242;
			}
			case 397: {
				stateStack.Push(398);
				PushContext(Context.Parameter, la, t);
				goto case 399;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 22) {
					currentState = 397;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 399: {
				SetIdentifierExpected(la);
				goto case 400;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (la.kind == 40) {
					stateStack.Push(399);
					goto case 410;
				} else {
					goto case 401;
				}
			}
			case 401: {
				SetIdentifierExpected(la);
				goto case 402;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (set[140].Get(la.kind)) {
					currentState = 401;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(403);
					goto case 189;
				}
			}
			case 403: {
				PopContext();
				goto case 404;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (la.kind == 63) {
					currentState = 408;
					break;
				} else {
					goto case 405;
				}
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 20) {
					currentState = 407;
					break;
				} else {
					goto case 406;
				}
			}
			case 406: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 407: {
				stateStack.Push(406);
				goto case 56;
			}
			case 408: {
				PushContext(Context.Type, la, t);
				stateStack.Push(409);
				goto case 37;
			}
			case 409: {
				PopContext();
				goto case 405;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				Expect(40, la); // "<"
				currentState = 411;
				break;
			}
			case 411: {
				PushContext(Context.Attribute, la, t);
				goto case 412;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (set[141].Get(la.kind)) {
					currentState = 412;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 413;
					break;
				}
			}
			case 413: {
				PopContext();
				goto case 414;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				Expect(37, la); // "("
				currentState = 416;
				break;
			}
			case 416: {
				SetIdentifierExpected(la);
				goto case 417;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(418);
					goto case 397;
				} else {
					goto case 418;
				}
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				Expect(38, la); // ")"
				currentState = 419;
				break;
			}
			case 419: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 420;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (set[48].Get(la.kind)) {
					goto case 253;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(421);
						goto case 245;
					} else {
						goto case 6;
					}
				}
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(113, la); // "End"
				currentState = 422;
				break;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 436;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(425);
						goto case 427;
					} else {
						Error(la);
						goto case 424;
					}
				}
			}
			case 424: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 17) {
					currentState = 426;
					break;
				} else {
					goto case 424;
				}
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (la.kind == 16) {
					currentState = 425;
					break;
				} else {
					goto case 425;
				}
			}
			case 427: {
				PushContext(Context.Xml, la, t);
				goto case 428;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 429;
				break;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (set[142].Get(la.kind)) {
					if (set[143].Get(la.kind)) {
						currentState = 429;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(429);
							goto case 433;
						} else {
							Error(la);
							goto case 429;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 430;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 431;
							break;
						} else {
							Error(la);
							goto case 430;
						}
					}
				}
			}
			case 430: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (set[144].Get(la.kind)) {
					if (set[145].Get(la.kind)) {
						currentState = 431;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(431);
							goto case 433;
						} else {
							if (la.kind == 10) {
								stateStack.Push(431);
								goto case 427;
							} else {
								Error(la);
								goto case 431;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 432;
					break;
				}
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (set[146].Get(la.kind)) {
					if (set[147].Get(la.kind)) {
						currentState = 432;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(432);
							goto case 433;
						} else {
							Error(la);
							goto case 432;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 430;
					break;
				}
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 434;
				break;
			}
			case 434: {
				stateStack.Push(435);
				goto case 56;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (la.kind == 16) {
					currentState = 437;
					break;
				} else {
					goto case 437;
				}
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 436;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(438);
						goto case 427;
					} else {
						goto case 424;
					}
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 17) {
					currentState = 439;
					break;
				} else {
					goto case 424;
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (la.kind == 16) {
					currentState = 438;
					break;
				} else {
					goto case 438;
				}
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				Expect(37, la); // "("
				currentState = 441;
				break;
			}
			case 441: {
				readXmlIdentifier = true;
				stateStack.Push(442);
				goto case 189;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				Expect(38, la); // ")"
				currentState = 159;
				break;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				Expect(37, la); // "("
				currentState = 444;
				break;
			}
			case 444: {
				PushContext(Context.Type, la, t);
				stateStack.Push(445);
				goto case 37;
			}
			case 445: {
				PopContext();
				goto case 442;
			}
			case 446: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 447;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (la.kind == 10) {
					currentState = 448;
					break;
				} else {
					goto case 448;
				}
			}
			case 448: {
				stateStack.Push(449);
				goto case 85;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (la.kind == 11) {
					currentState = 159;
					break;
				} else {
					goto case 159;
				}
			}
			case 450: {
				stateStack.Push(451);
				goto case 56;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 22) {
					currentState = 450;
					break;
				} else {
					goto case 442;
				}
			}
			case 452: {
				stateStack.Push(453);
				goto case 56;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 22) {
					currentState = 454;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 454: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 455;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (set[23].Get(la.kind)) {
					goto case 452;
				} else {
					goto case 453;
				}
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(457);
					goto case 37;
				} else {
					goto case 457;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 22) {
					currentState = 456;
					break;
				} else {
					goto case 45;
				}
			}
			case 458: {
				SetIdentifierExpected(la);
				goto case 459;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (set[148].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 461;
						break;
					} else {
						if (set[75].Get(la.kind)) {
							stateStack.Push(460);
							goto case 397;
						} else {
							Error(la);
							goto case 460;
						}
					}
				} else {
					goto case 460;
				}
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 461: {
				stateStack.Push(460);
				goto case 462;
			}
			case 462: {
				SetIdentifierExpected(la);
				goto case 463;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 464;
					break;
				} else {
					goto case 464;
				}
			}
			case 464: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(465);
				goto case 479;
			}
			case 465: {
				PopContext();
				goto case 466;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 63) {
					currentState = 480;
					break;
				} else {
					goto case 467;
				}
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (la.kind == 22) {
					currentState = 468;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 468: {
				SetIdentifierExpected(la);
				goto case 469;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 470;
					break;
				} else {
					goto case 470;
				}
			}
			case 470: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(471);
				goto case 479;
			}
			case 471: {
				PopContext();
				goto case 472;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 63) {
					currentState = 473;
					break;
				} else {
					goto case 467;
				}
			}
			case 473: {
				PushContext(Context.Type, la, t);
				stateStack.Push(474);
				goto case 475;
			}
			case 474: {
				PopContext();
				goto case 467;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (set[90].Get(la.kind)) {
					goto case 478;
				} else {
					if (la.kind == 35) {
						currentState = 476;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 476: {
				stateStack.Push(477);
				goto case 478;
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				if (la.kind == 22) {
					currentState = 476;
					break;
				} else {
					goto case 66;
				}
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 162) {
						goto case 105;
					} else {
						if (la.kind == 84) {
							goto case 121;
						} else {
							if (la.kind == 209) {
								goto case 96;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				if (la.kind == 2) {
					goto case 129;
				} else {
					if (la.kind == 62) {
						goto case 127;
					} else {
						if (la.kind == 64) {
							goto case 126;
						} else {
							if (la.kind == 65) {
								goto case 125;
							} else {
								if (la.kind == 66) {
									goto case 124;
								} else {
									if (la.kind == 67) {
										goto case 123;
									} else {
										if (la.kind == 70) {
											goto case 122;
										} else {
											if (la.kind == 87) {
												goto case 120;
											} else {
												if (la.kind == 104) {
													goto case 118;
												} else {
													if (la.kind == 107) {
														goto case 117;
													} else {
														if (la.kind == 116) {
															goto case 115;
														} else {
															if (la.kind == 121) {
																goto case 114;
															} else {
																if (la.kind == 133) {
																	goto case 110;
																} else {
																	if (la.kind == 139) {
																		goto case 109;
																	} else {
																		if (la.kind == 143) {
																			goto case 108;
																		} else {
																			if (la.kind == 146) {
																				goto case 107;
																			} else {
																				if (la.kind == 147) {
																					goto case 106;
																				} else {
																					if (la.kind == 170) {
																						goto case 103;
																					} else {
																						if (la.kind == 176) {
																							goto case 102;
																						} else {
																							if (la.kind == 184) {
																								goto case 101;
																							} else {
																								if (la.kind == 203) {
																									goto case 98;
																								} else {
																									if (la.kind == 212) {
																										goto case 93;
																									} else {
																										if (la.kind == 213) {
																											goto case 92;
																										} else {
																											if (la.kind == 223) {
																												goto case 90;
																											} else {
																												if (la.kind == 224) {
																													goto case 89;
																												} else {
																													if (la.kind == 230) {
																														goto case 88;
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
			case 480: {
				PushContext(Context.Type, la, t);
				stateStack.Push(481);
				goto case 475;
			}
			case 481: {
				PopContext();
				goto case 467;
			}
			case 482: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(483);
				goto case 189;
			}
			case 483: {
				PopContext();
				goto case 484;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 37) {
					currentState = 489;
					break;
				} else {
					goto case 485;
				}
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				if (la.kind == 63) {
					currentState = 486;
					break;
				} else {
					goto case 23;
				}
			}
			case 486: {
				PushContext(Context.Type, la, t);
				goto case 487;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				if (la.kind == 40) {
					stateStack.Push(487);
					goto case 410;
				} else {
					stateStack.Push(488);
					goto case 37;
				}
			}
			case 488: {
				PopContext();
				goto case 23;
			}
			case 489: {
				SetIdentifierExpected(la);
				goto case 490;
			}
			case 490: {
				if (la == null) { currentState = 490; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(491);
					goto case 397;
				} else {
					goto case 491;
				}
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				Expect(38, la); // ")"
				currentState = 485;
				break;
			}
			case 492: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(493);
				goto case 189;
			}
			case 493: {
				PopContext();
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 499;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 496;
							break;
						} else {
							goto case 495;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 495: {
				Error(la);
				goto case 23;
			}
			case 496: {
				SetIdentifierExpected(la);
				goto case 497;
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(498);
					goto case 397;
				} else {
					goto case 498;
				}
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 499: {
				PushContext(Context.Type, la, t);
				stateStack.Push(500);
				goto case 37;
			}
			case 500: {
				PopContext();
				goto case 23;
			}
			case 501: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 502;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				Expect(115, la); // "Enum"
				currentState = 503;
				break;
			}
			case 503: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(504);
				goto case 189;
			}
			case 504: {
				PopContext();
				goto case 505;
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 63) {
					currentState = 517;
					break;
				} else {
					goto case 506;
				}
			}
			case 506: {
				stateStack.Push(507);
				goto case 23;
			}
			case 507: {
				SetIdentifierExpected(la);
				goto case 508;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (set[93].Get(la.kind)) {
					goto case 512;
				} else {
					Expect(113, la); // "End"
					currentState = 509;
					break;
				}
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				Expect(115, la); // "Enum"
				currentState = 510;
				break;
			}
			case 510: {
				stateStack.Push(511);
				goto case 23;
			}
			case 511: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 512: {
				SetIdentifierExpected(la);
				goto case 513;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				if (la.kind == 40) {
					stateStack.Push(512);
					goto case 410;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(514);
					goto case 189;
				}
			}
			case 514: {
				PopContext();
				goto case 515;
			}
			case 515: {
				if (la == null) { currentState = 515; break; }
				if (la.kind == 20) {
					currentState = 516;
					break;
				} else {
					goto case 506;
				}
			}
			case 516: {
				stateStack.Push(506);
				goto case 56;
			}
			case 517: {
				PushContext(Context.Type, la, t);
				stateStack.Push(518);
				goto case 37;
			}
			case 518: {
				PopContext();
				goto case 506;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				Expect(103, la); // "Delegate"
				currentState = 520;
				break;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (la.kind == 210) {
					currentState = 521;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 521;
						break;
					} else {
						Error(la);
						goto case 521;
					}
				}
			}
			case 521: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 522;
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				currentState = 523;
				break;
			}
			case 523: {
				PopContext();
				goto case 524;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				if (la.kind == 37) {
					currentState = 527;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 525;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 525: {
				PushContext(Context.Type, la, t);
				stateStack.Push(526);
				goto case 37;
			}
			case 526: {
				PopContext();
				goto case 23;
			}
			case 527: {
				SetIdentifierExpected(la);
				goto case 528;
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (set[148].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 530;
						break;
					} else {
						if (set[75].Get(la.kind)) {
							stateStack.Push(529);
							goto case 397;
						} else {
							Error(la);
							goto case 529;
						}
					}
				} else {
					goto case 529;
				}
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				Expect(38, la); // ")"
				currentState = 524;
				break;
			}
			case 530: {
				stateStack.Push(529);
				goto case 462;
			}
			case 531: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 532;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				if (la.kind == 155) {
					currentState = 533;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 533;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 533;
							break;
						} else {
							Error(la);
							goto case 533;
						}
					}
				}
			}
			case 533: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(534);
				goto case 189;
			}
			case 534: {
				PopContext();
				goto case 535;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				if (la.kind == 37) {
					currentState = 699;
					break;
				} else {
					goto case 536;
				}
			}
			case 536: {
				stateStack.Push(537);
				goto case 23;
			}
			case 537: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 538;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 696;
				} else {
					goto case 539;
				}
			}
			case 539: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 540;
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 690;
				} else {
					goto case 541;
				}
			}
			case 541: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 542;
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				if (set[97].Get(la.kind)) {
					goto case 547;
				} else {
					isMissingModifier = false;
					goto case 543;
				}
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				Expect(113, la); // "End"
				currentState = 544;
				break;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (la.kind == 155) {
					currentState = 545;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 545;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 545;
							break;
						} else {
							Error(la);
							goto case 545;
						}
					}
				}
			}
			case 545: {
				stateStack.Push(546);
				goto case 23;
			}
			case 546: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 547: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 548;
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				if (la.kind == 40) {
					stateStack.Push(547);
					goto case 410;
				} else {
					isMissingModifier = true;
					goto case 549;
				}
			}
			case 549: {
				SetIdentifierExpected(la);
				goto case 550;
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (set[127].Get(la.kind)) {
					currentState = 689;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 551;
				}
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(541);
					goto case 531;
				} else {
					if (la.kind == 103) {
						stateStack.Push(541);
						goto case 519;
					} else {
						if (la.kind == 115) {
							stateStack.Push(541);
							goto case 501;
						} else {
							if (la.kind == 142) {
								stateStack.Push(541);
								goto case 9;
							} else {
								if (set[100].Get(la.kind)) {
									stateStack.Push(541);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 552;
								} else {
									Error(la);
									goto case 541;
								}
							}
						}
					}
				}
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (set[117].Get(la.kind)) {
					stateStack.Push(553);
					goto case 674;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(553);
						goto case 656;
					} else {
						if (la.kind == 101) {
							stateStack.Push(553);
							goto case 640;
						} else {
							if (la.kind == 119) {
								stateStack.Push(553);
								goto case 625;
							} else {
								if (la.kind == 98) {
									stateStack.Push(553);
									goto case 613;
								} else {
									if (la.kind == 186) {
										stateStack.Push(553);
										goto case 568;
									} else {
										if (la.kind == 172) {
											stateStack.Push(553);
											goto case 554;
										} else {
											Error(la);
											goto case 553;
										}
									}
								}
							}
						}
					}
				}
			}
			case 553: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				Expect(172, la); // "Operator"
				currentState = 555;
				break;
			}
			case 555: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 556;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				currentState = 557;
				break;
			}
			case 557: {
				PopContext();
				goto case 558;
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				Expect(37, la); // "("
				currentState = 559;
				break;
			}
			case 559: {
				stateStack.Push(560);
				goto case 397;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				Expect(38, la); // ")"
				currentState = 561;
				break;
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				if (la.kind == 63) {
					currentState = 565;
					break;
				} else {
					goto case 562;
				}
			}
			case 562: {
				stateStack.Push(563);
				goto case 245;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				Expect(113, la); // "End"
				currentState = 564;
				break;
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 565: {
				PushContext(Context.Type, la, t);
				goto case 566;
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				if (la.kind == 40) {
					stateStack.Push(566);
					goto case 410;
				} else {
					stateStack.Push(567);
					goto case 37;
				}
			}
			case 567: {
				PopContext();
				goto case 562;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				Expect(186, la); // "Property"
				currentState = 569;
				break;
			}
			case 569: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(570);
				goto case 189;
			}
			case 570: {
				PopContext();
				goto case 571;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (la.kind == 37) {
					currentState = 610;
					break;
				} else {
					goto case 572;
				}
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				if (la.kind == 63) {
					currentState = 608;
					break;
				} else {
					goto case 573;
				}
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (la.kind == 136) {
					currentState = 603;
					break;
				} else {
					goto case 574;
				}
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (la.kind == 20) {
					currentState = 602;
					break;
				} else {
					goto case 575;
				}
			}
			case 575: {
				stateStack.Push(576);
				goto case 23;
			}
			case 576: {
				PopContext();
				goto case 577;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				if (la.kind == 40) {
					stateStack.Push(577);
					goto case 410;
				} else {
					goto case 578;
				}
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (set[149].Get(la.kind)) {
					currentState = 601;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 579;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				if (la.kind == 128) {
					currentState = 580;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 580;
						break;
					} else {
						Error(la);
						goto case 580;
					}
				}
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 37) {
					currentState = 598;
					break;
				} else {
					goto case 581;
				}
			}
			case 581: {
				stateStack.Push(582);
				goto case 245;
			}
			case 582: {
				if (la == null) { currentState = 582; break; }
				Expect(113, la); // "End"
				currentState = 583;
				break;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 128) {
					currentState = 584;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 584;
						break;
					} else {
						Error(la);
						goto case 584;
					}
				}
			}
			case 584: {
				stateStack.Push(585);
				goto case 23;
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				if (set[106].Get(la.kind)) {
					goto case 588;
				} else {
					goto case 586;
				}
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				Expect(113, la); // "End"
				currentState = 587;
				break;
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				if (la.kind == 40) {
					stateStack.Push(588);
					goto case 410;
				} else {
					goto case 589;
				}
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (set[149].Get(la.kind)) {
					currentState = 589;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 590;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 590;
							break;
						} else {
							Error(la);
							goto case 590;
						}
					}
				}
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (la.kind == 37) {
					currentState = 595;
					break;
				} else {
					goto case 591;
				}
			}
			case 591: {
				stateStack.Push(592);
				goto case 245;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				Expect(113, la); // "End"
				currentState = 593;
				break;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				if (la.kind == 128) {
					currentState = 594;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 594;
						break;
					} else {
						Error(la);
						goto case 594;
					}
				}
			}
			case 594: {
				stateStack.Push(586);
				goto case 23;
			}
			case 595: {
				SetIdentifierExpected(la);
				goto case 596;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(597);
					goto case 397;
				} else {
					goto case 597;
				}
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(38, la); // ")"
				currentState = 591;
				break;
			}
			case 598: {
				SetIdentifierExpected(la);
				goto case 599;
			}
			case 599: {
				if (la == null) { currentState = 599; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(600);
					goto case 397;
				} else {
					goto case 600;
				}
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				Expect(38, la); // ")"
				currentState = 581;
				break;
			}
			case 601: {
				SetIdentifierExpected(la);
				goto case 578;
			}
			case 602: {
				stateStack.Push(575);
				goto case 56;
			}
			case 603: {
				PushContext(Context.Type, la, t);
				stateStack.Push(604);
				goto case 37;
			}
			case 604: {
				PopContext();
				goto case 605;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				if (la.kind == 22) {
					currentState = 606;
					break;
				} else {
					goto case 574;
				}
			}
			case 606: {
				PushContext(Context.Type, la, t);
				stateStack.Push(607);
				goto case 37;
			}
			case 607: {
				PopContext();
				goto case 605;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (la.kind == 40) {
					stateStack.Push(608);
					goto case 410;
				} else {
					if (la.kind == 162) {
						stateStack.Push(573);
						goto case 69;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(609);
							goto case 37;
						} else {
							Error(la);
							goto case 573;
						}
					}
				}
			}
			case 609: {
				PopContext();
				goto case 573;
			}
			case 610: {
				SetIdentifierExpected(la);
				goto case 611;
			}
			case 611: {
				if (la == null) { currentState = 611; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(612);
					goto case 397;
				} else {
					goto case 612;
				}
			}
			case 612: {
				if (la == null) { currentState = 612; break; }
				Expect(38, la); // ")"
				currentState = 572;
				break;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				Expect(98, la); // "Custom"
				currentState = 614;
				break;
			}
			case 614: {
				stateStack.Push(615);
				goto case 625;
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				if (set[111].Get(la.kind)) {
					goto case 617;
				} else {
					Expect(113, la); // "End"
					currentState = 616;
					break;
				}
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (la.kind == 40) {
					stateStack.Push(617);
					goto case 410;
				} else {
					if (la.kind == 56) {
						currentState = 618;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 618;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 618;
								break;
							} else {
								Error(la);
								goto case 618;
							}
						}
					}
				}
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				Expect(37, la); // "("
				currentState = 619;
				break;
			}
			case 619: {
				stateStack.Push(620);
				goto case 397;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				Expect(38, la); // ")"
				currentState = 621;
				break;
			}
			case 621: {
				stateStack.Push(622);
				goto case 245;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(113, la); // "End"
				currentState = 623;
				break;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				if (la.kind == 56) {
					currentState = 624;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 624;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 624;
							break;
						} else {
							Error(la);
							goto case 624;
						}
					}
				}
			}
			case 624: {
				stateStack.Push(615);
				goto case 23;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				Expect(119, la); // "Event"
				currentState = 626;
				break;
			}
			case 626: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(627);
				goto case 189;
			}
			case 627: {
				PopContext();
				goto case 628;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				if (la.kind == 63) {
					currentState = 638;
					break;
				} else {
					if (set[150].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 635;
							break;
						} else {
							goto case 629;
						}
					} else {
						Error(la);
						goto case 629;
					}
				}
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (la.kind == 136) {
					currentState = 630;
					break;
				} else {
					goto case 23;
				}
			}
			case 630: {
				PushContext(Context.Type, la, t);
				stateStack.Push(631);
				goto case 37;
			}
			case 631: {
				PopContext();
				goto case 632;
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				if (la.kind == 22) {
					currentState = 633;
					break;
				} else {
					goto case 23;
				}
			}
			case 633: {
				PushContext(Context.Type, la, t);
				stateStack.Push(634);
				goto case 37;
			}
			case 634: {
				PopContext();
				goto case 632;
			}
			case 635: {
				SetIdentifierExpected(la);
				goto case 636;
			}
			case 636: {
				if (la == null) { currentState = 636; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(637);
					goto case 397;
				} else {
					goto case 637;
				}
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				Expect(38, la); // ")"
				currentState = 629;
				break;
			}
			case 638: {
				PushContext(Context.Type, la, t);
				stateStack.Push(639);
				goto case 37;
			}
			case 639: {
				PopContext();
				goto case 629;
			}
			case 640: {
				if (la == null) { currentState = 640; break; }
				Expect(101, la); // "Declare"
				currentState = 641;
				break;
			}
			case 641: {
				if (la == null) { currentState = 641; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 642;
					break;
				} else {
					goto case 642;
				}
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				if (la.kind == 210) {
					currentState = 643;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 643;
						break;
					} else {
						Error(la);
						goto case 643;
					}
				}
			}
			case 643: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(644);
				goto case 189;
			}
			case 644: {
				PopContext();
				goto case 645;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				Expect(149, la); // "Lib"
				currentState = 646;
				break;
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				Expect(3, la); // LiteralString
				currentState = 647;
				break;
			}
			case 647: {
				if (la == null) { currentState = 647; break; }
				if (la.kind == 59) {
					currentState = 655;
					break;
				} else {
					goto case 648;
				}
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				if (la.kind == 37) {
					currentState = 652;
					break;
				} else {
					goto case 649;
				}
			}
			case 649: {
				if (la == null) { currentState = 649; break; }
				if (la.kind == 63) {
					currentState = 650;
					break;
				} else {
					goto case 23;
				}
			}
			case 650: {
				PushContext(Context.Type, la, t);
				stateStack.Push(651);
				goto case 37;
			}
			case 651: {
				PopContext();
				goto case 23;
			}
			case 652: {
				SetIdentifierExpected(la);
				goto case 653;
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				if (set[75].Get(la.kind)) {
					stateStack.Push(654);
					goto case 397;
				} else {
					goto case 654;
				}
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				Expect(38, la); // ")"
				currentState = 649;
				break;
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				Expect(3, la); // LiteralString
				currentState = 648;
				break;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				if (la.kind == 210) {
					currentState = 657;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 657;
						break;
					} else {
						Error(la);
						goto case 657;
					}
				}
			}
			case 657: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 658;
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				currentState = 659;
				break;
			}
			case 659: {
				PopContext();
				goto case 660;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				if (la.kind == 37) {
					currentState = 670;
					break;
				} else {
					if (la.kind == 134 || la.kind == 136) {
						currentState = 667;
						break;
					} else {
						goto case 661;
					}
				}
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				if (la.kind == 63) {
					currentState = 665;
					break;
				} else {
					goto case 662;
				}
			}
			case 662: {
				stateStack.Push(663);
				goto case 245;
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
				Expect(113, la); // "End"
				currentState = 664;
				break;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 495;
					}
				}
			}
			case 665: {
				PushContext(Context.Type, la, t);
				stateStack.Push(666);
				goto case 37;
			}
			case 666: {
				PopContext();
				goto case 662;
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				if (la.kind == 153 || la.kind == 158 || la.kind == 159) {
					currentState = 669;
					break;
				} else {
					goto case 668;
				}
			}
			case 668: {
				stateStack.Push(661);
				goto case 37;
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				Expect(26, la); // "."
				currentState = 668;
				break;
			}
			case 670: {
				SetIdentifierExpected(la);
				goto case 671;
			}
			case 671: {
				if (la == null) { currentState = 671; break; }
				if (set[148].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 673;
						break;
					} else {
						if (set[75].Get(la.kind)) {
							stateStack.Push(672);
							goto case 397;
						} else {
							Error(la);
							goto case 672;
						}
					}
				} else {
					goto case 672;
				}
			}
			case 672: {
				if (la == null) { currentState = 672; break; }
				Expect(38, la); // ")"
				currentState = 660;
				break;
			}
			case 673: {
				stateStack.Push(672);
				goto case 462;
			}
			case 674: {
				stateStack.Push(675);
				SetIdentifierExpected(la);
				goto case 676;
			}
			case 675: {
				if (la == null) { currentState = 675; break; }
				if (la.kind == 22) {
					currentState = 674;
					break;
				} else {
					goto case 23;
				}
			}
			case 676: {
				if (la == null) { currentState = 676; break; }
				if (la.kind == 88) {
					currentState = 677;
					break;
				} else {
					goto case 677;
				}
			}
			case 677: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(678);
				goto case 688;
			}
			case 678: {
				PopContext();
				goto case 679;
			}
			case 679: {
				if (la == null) { currentState = 679; break; }
				if (la.kind == 33) {
					currentState = 680;
					break;
				} else {
					goto case 680;
				}
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				if (la.kind == 37) {
					currentState = 685;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 682;
						break;
					} else {
						goto case 681;
					}
				}
			}
			case 681: {
				if (la == null) { currentState = 681; break; }
				if (la.kind == 20) {
					goto case 203;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 682: {
				PushContext(Context.Type, la, t);
				goto case 683;
			}
			case 683: {
				if (la == null) { currentState = 683; break; }
				if (la.kind == 162) {
					stateStack.Push(684);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(684);
						goto case 37;
					} else {
						Error(la);
						goto case 684;
					}
				}
			}
			case 684: {
				PopContext();
				goto case 681;
			}
			case 685: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 686;
			}
			case 686: {
				if (la == null) { currentState = 686; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(687);
					goto case 56;
				} else {
					goto case 687;
				}
			}
			case 687: {
				if (la == null) { currentState = 687; break; }
				if (la.kind == 22) {
					currentState = 685;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 680;
					break;
				}
			}
			case 688: {
				if (la == null) { currentState = 688; break; }
				if (set[133].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 128;
					} else {
						if (la.kind == 126) {
							goto case 112;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 689: {
				isMissingModifier = false;
				goto case 549;
			}
			case 690: {
				if (la == null) { currentState = 690; break; }
				Expect(136, la); // "Implements"
				currentState = 691;
				break;
			}
			case 691: {
				PushContext(Context.Type, la, t);
				stateStack.Push(692);
				goto case 37;
			}
			case 692: {
				PopContext();
				goto case 693;
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				if (la.kind == 22) {
					currentState = 694;
					break;
				} else {
					stateStack.Push(541);
					goto case 23;
				}
			}
			case 694: {
				PushContext(Context.Type, la, t);
				stateStack.Push(695);
				goto case 37;
			}
			case 695: {
				PopContext();
				goto case 693;
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				Expect(140, la); // "Inherits"
				currentState = 697;
				break;
			}
			case 697: {
				PushContext(Context.Type, la, t);
				stateStack.Push(698);
				goto case 37;
			}
			case 698: {
				PopContext();
				stateStack.Push(539);
				goto case 23;
			}
			case 699: {
				if (la == null) { currentState = 699; break; }
				Expect(169, la); // "Of"
				currentState = 700;
				break;
			}
			case 700: {
				stateStack.Push(701);
				goto case 462;
			}
			case 701: {
				if (la == null) { currentState = 701; break; }
				Expect(38, la); // ")"
				currentState = 536;
				break;
			}
			case 702: {
				isMissingModifier = false;
				goto case 28;
			}
			case 703: {
				PushContext(Context.Type, la, t);
				stateStack.Push(704);
				goto case 37;
			}
			case 704: {
				PopContext();
				goto case 705;
			}
			case 705: {
				if (la == null) { currentState = 705; break; }
				if (la.kind == 22) {
					currentState = 706;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 706: {
				PushContext(Context.Type, la, t);
				stateStack.Push(707);
				goto case 37;
			}
			case 707: {
				PopContext();
				goto case 705;
			}
			case 708: {
				if (la == null) { currentState = 708; break; }
				Expect(169, la); // "Of"
				currentState = 709;
				break;
			}
			case 709: {
				stateStack.Push(710);
				goto case 462;
			}
			case 710: {
				if (la == null) { currentState = 710; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 711: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 712;
			}
			case 712: {
				if (la == null) { currentState = 712; break; }
				if (set[47].Get(la.kind)) {
					currentState = 712;
					break;
				} else {
					PopContext();
					stateStack.Push(713);
					goto case 23;
				}
			}
			case 713: {
				if (la == null) { currentState = 713; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(713);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 714;
					break;
				}
			}
			case 714: {
				if (la == null) { currentState = 714; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 715: {
				if (la == null) { currentState = 715; break; }
				Expect(137, la); // "Imports"
				currentState = 716;
				break;
			}
			case 716: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 717;
			}
			case 717: {
				if (la == null) { currentState = 717; break; }
				if (set[151].Get(la.kind)) {
					currentState = 723;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 719;
						break;
					} else {
						Error(la);
						goto case 718;
					}
				}
			}
			case 718: {
				PopContext();
				goto case 23;
			}
			case 719: {
				stateStack.Push(720);
				goto case 189;
			}
			case 720: {
				if (la == null) { currentState = 720; break; }
				Expect(20, la); // "="
				currentState = 721;
				break;
			}
			case 721: {
				if (la == null) { currentState = 721; break; }
				Expect(3, la); // LiteralString
				currentState = 722;
				break;
			}
			case 722: {
				if (la == null) { currentState = 722; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 718;
				break;
			}
			case 723: {
				if (la == null) { currentState = 723; break; }
				if (la.kind == 37) {
					stateStack.Push(723);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 724;
						break;
					} else {
						goto case 718;
					}
				}
			}
			case 724: {
				stateStack.Push(718);
				goto case 37;
			}
			case 725: {
				if (la == null) { currentState = 725; break; }
				Expect(173, la); // "Option"
				currentState = 726;
				break;
			}
			case 726: {
				if (la == null) { currentState = 726; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 728;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 727;
						break;
					} else {
						goto case 495;
					}
				}
			}
			case 727: {
				if (la == null) { currentState = 727; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 495;
					}
				}
			}
			case 728: {
				if (la == null) { currentState = 728; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 23;
					break;
				} else {
					goto case 23;
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
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 671926304, 1606227075, -2143942272, 3393}),
		new BitArray(new int[] {0, 256, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, -1258291203, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, -1258291235, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {-66189316, 1174405176, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192413, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972018405, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018401, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {65140738, 973078487, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64092162, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64092162, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-66189316, 1174405162, -51384097, -972018401, -1030969178, 17106228, -97186288, 8259}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1174405160, -51384097, -971985637, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969166, 17106228, -97186284, 8259}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972002021, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66189316, 1174405160, -51383073, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -971985637, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -972018405, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680476901, -1030969166, -1593504460, -21144002, 8903}),
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
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-318462, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-383998, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
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
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 320, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, -1030969308, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {7340034, -2147483614, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 0, 0, 0, 0, 0}),
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
		new BitArray(new int[] {66189314, -1174405161, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680378597, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66189316, 1174405160, -51383585, -972018405, -1030969182, 17106228, -97186288, 8259}),
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