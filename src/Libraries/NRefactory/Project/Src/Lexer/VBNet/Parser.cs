using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 250;
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
			case 251:
			case 504:
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
			case 228:
			case 232:
			case 282:
			case 382:
			case 392:
			case 448:
			case 491:
			case 501:
			case 512:
			case 542:
			case 578:
			case 635:
			case 652:
			case 728:
				return set[6];
			case 12:
			case 13:
			case 543:
			case 544:
			case 589:
			case 599:
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
			case 243:
			case 246:
			case 247:
			case 283:
			case 287:
			case 309:
			case 324:
			case 335:
			case 338:
			case 344:
			case 349:
			case 358:
			case 359:
			case 379:
			case 403:
			case 497:
			case 509:
			case 515:
			case 519:
			case 527:
			case 535:
			case 545:
			case 554:
			case 571:
			case 576:
			case 584:
			case 590:
			case 593:
			case 600:
			case 603:
			case 630:
			case 633:
			case 660:
			case 671:
			case 675:
			case 707:
			case 727:
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
			case 244:
			case 258:
			case 285:
			case 339:
			case 380:
			case 428:
			case 552:
			case 572:
			case 591:
			case 595:
			case 601:
			case 631:
			case 672:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 520:
			case 555:
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
			case 711:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 136:
			case 201:
			case 202:
			case 252:
			case 263:
			case 264:
			case 418:
			case 419:
			case 436:
			case 437:
			case 438:
			case 439:
			case 530:
			case 531:
			case 564:
			case 565:
			case 666:
			case 667:
			case 720:
			case 721:
				return set[14];
			case 33:
			case 34:
			case 492:
			case 493:
			case 502:
			case 503:
			case 532:
			case 533:
			case 657:
				return set[15];
			case 35:
			case 37:
			case 141:
			case 152:
			case 155:
			case 171:
			case 187:
			case 205:
			case 294:
			case 319:
			case 402:
			case 415:
			case 451:
			case 508:
			case 526:
			case 534:
			case 612:
			case 615:
			case 639:
			case 642:
			case 647:
			case 659:
			case 674:
			case 677:
			case 700:
			case 703:
			case 706:
			case 712:
			case 715:
			case 733:
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
			case 374:
			case 455:
				return set[19];
			case 42:
			case 161:
			case 168:
			case 173:
			case 237:
			case 422:
			case 447:
			case 450:
			case 566:
			case 567:
			case 627:
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
			case 240:
			case 425:
			case 449:
			case 452:
			case 469:
			case 500:
			case 507:
			case 538:
			case 569:
			case 606:
			case 609:
			case 621:
			case 629:
			case 646:
			case 663:
			case 681:
			case 710:
			case 719:
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
			case 396:
			case 397:
			case 399:
			case 400:
			case 463:
			case 464:
			case 694:
			case 695:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 163:
			case 170:
			case 377:
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
			case 398:
			case 401:
			case 405:
			case 413:
			case 459:
			case 462:
			case 466:
			case 476:
			case 483:
			case 490:
			case 696:
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
			case 199:
			case 223:
			case 261:
			case 271:
			case 273:
			case 274:
			case 291:
			case 308:
			case 313:
			case 322:
			case 328:
			case 330:
			case 334:
			case 337:
			case 343:
			case 354:
			case 356:
			case 362:
			case 376:
			case 378:
			case 414:
			case 441:
			case 457:
			case 458:
			case 460:
			case 461:
			case 525:
			case 611:
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
			case 486:
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
			case 678:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 82:
			case 206:
			case 207:
			case 269:
			case 270:
			case 321:
			case 388:
			case 729:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 87:
			case 340:
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
			case 286:
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
			case 429:
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
			case 346:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 100:
			case 577:
			case 596:
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
			case 303:
			case 310:
			case 325:
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
			case 210:
			case 215:
			case 217:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 108:
			case 212:
			case 216:
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
			case 245:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 112:
			case 135:
			case 235:
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
			case 622:
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
			case 222:
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
			case 234:
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
			case 453:
			case 454:
				return set[32];
			case 148:
				return set[33];
			case 157:
			case 158:
			case 306:
			case 315:
				return set[34];
			case 159:
			case 431:
				return set[35];
			case 160:
			case 361:
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
			case 214:
			case 219:
			case 225:
			case 227:
			case 231:
			case 233:
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
			case 307:
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
			case 208:
			case 213:
			case 218:
			case 226:
			case 230:
			case 256:
			case 260:
				return set[39];
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 200:
				return set[40];
			case 203:
			case 265:
				return set[41];
			case 204:
			case 266:
				return set[42];
			case 209:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 220:
			case 221:
				return set[43];
			case 224:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 229:
				return set[44];
			case 236:
			case 529:
			case 651:
			case 665:
			case 673:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 238:
			case 239:
			case 423:
			case 424:
			case 498:
			case 499:
			case 505:
			case 506:
			case 604:
			case 605:
			case 607:
			case 608:
			case 619:
			case 620:
			case 644:
			case 645:
			case 661:
			case 662:
				return set[45];
			case 241:
			case 242:
				return set[46];
			case 248:
			case 249:
				return set[47];
			case 250:
				return set[48];
			case 253:
				return set[49];
			case 254:
			case 255:
			case 367:
				return set[50];
			case 257:
			case 262:
			case 352:
			case 640:
			case 641:
			case 643:
			case 684:
			case 701:
			case 702:
			case 704:
			case 713:
			case 714:
			case 716:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 259:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 267:
			case 268:
				return set[51];
			case 272:
			case 314:
			case 329:
				return set[52];
			case 275:
			case 276:
			case 296:
			case 297:
			case 311:
			case 312:
			case 326:
			case 327:
				return set[53];
			case 277:
			case 368:
			case 371:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 278:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 279:
				return set[54];
			case 280:
			case 299:
				return set[55];
			case 281:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 284:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 288:
			case 289:
				return set[56];
			case 290:
			case 295:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 292:
			case 293:
				return set[57];
			case 298:
				return set[58];
			case 300:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 301:
			case 302:
				return set[59];
			case 304:
			case 305:
				return set[60];
			case 316:
			case 317:
				return set[61];
			case 318:
				return set[62];
			case 320:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 323:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 331:
				return set[63];
			case 332:
			case 336:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 333:
				return set[64];
			case 341:
			case 342:
				return set[65];
			case 345:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 347:
			case 348:
				return set[66];
			case 350:
			case 351:
				return set[67];
			case 353:
			case 355:
				return set[68];
			case 357:
			case 363:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 360:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 364:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 365:
			case 366:
			case 426:
			case 427:
				return set[69];
			case 369:
			case 370:
			case 372:
			case 373:
				return set[70];
			case 375:
				return set[71];
			case 381:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 383:
			case 384:
			case 393:
			case 394:
				return set[72];
			case 385:
			case 395:
				return set[73];
			case 386:
				return set[74];
			case 387:
			case 391:
				return set[75];
			case 389:
			case 390:
			case 691:
			case 692:
				return set[76];
			case 404:
			case 406:
			case 407:
			case 568:
			case 628:
				return set[77];
			case 408:
			case 409:
				return set[78];
			case 410:
			case 411:
				return set[79];
			case 412:
			case 416:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 417:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 420:
			case 421:
				return set[80];
			case 430:
				return set[81];
			case 432:
			case 445:
				return set[82];
			case 433:
			case 446:
				return set[83];
			case 434:
			case 435:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 440:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 442:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 443:
				return set[84];
			case 444:
				return set[85];
			case 456:
				return set[86];
			case 465:
				return set[87];
			case 467:
			case 468:
			case 536:
			case 537:
			case 679:
			case 680:
				return set[88];
			case 470:
			case 471:
			case 472:
			case 477:
			case 478:
			case 539:
			case 682:
			case 709:
			case 718:
				return set[89];
			case 473:
			case 479:
			case 488:
				return set[90];
			case 474:
			case 475:
			case 480:
			case 481:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 482:
			case 484:
			case 489:
				return set[91];
			case 485:
			case 487:
				return set[92];
			case 494:
			case 513:
			case 514:
			case 570:
			case 658:
			case 670:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 495:
			case 496:
			case 574:
			case 575:
				return set[93];
			case 510:
			case 511:
			case 518:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 516:
			case 517:
				return set[94];
			case 521:
			case 522:
				return set[95];
			case 523:
			case 524:
			case 583:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 528:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 540:
			case 541:
			case 553:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 546:
			case 547:
				return set[96];
			case 548:
			case 549:
				return set[97];
			case 550:
			case 551:
			case 562:
				return set[98];
			case 556:
			case 557:
				return set[99];
			case 558:
			case 559:
			case 698:
				return set[100];
			case 560:
				return set[101];
			case 561:
				return set[102];
			case 563:
			case 573:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 579:
			case 580:
				return set[103];
			case 581:
				return set[104];
			case 582:
			case 618:
				return set[105];
			case 585:
			case 586:
			case 587:
			case 610:
				return set[106];
			case 588:
			case 592:
			case 602:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 594:
				return set[107];
			case 597:
				return set[108];
			case 598:
				return set[109];
			case 613:
			case 614:
			case 616:
			case 690:
			case 693:
				return set[110];
			case 617:
				return set[111];
			case 623:
			case 625:
			case 634:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 624:
				return set[112];
			case 626:
				return set[113];
			case 632:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 636:
			case 637:
				return set[114];
			case 638:
			case 648:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 649:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 650:
				return set[115];
			case 653:
			case 654:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 655:
			case 664:
			case 730:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 656:
				return set[116];
			case 668:
			case 669:
				return set[117];
			case 676:
				return set[118];
			case 683:
			case 685:
				return set[119];
			case 686:
			case 697:
				return set[120];
			case 687:
			case 688:
				return set[121];
			case 689:
				return set[122];
			case 699:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 705:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 708:
			case 717:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 722:
				return set[123];
			case 723:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 724:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 725:
			case 726:
				return set[124];
			case 731:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 732:
				return set[125];
			case 734:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 735:
				return set[126];
			case 736:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 737:
				return set[127];
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
	bool isAlreadyInExpr = false;
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
					goto case 734;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 724;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 417;
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
					currentState = 720;
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
					goto case 417;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[128].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 540;
					} else {
						if (la.kind == 103) {
							currentState = 529;
							break;
						} else {
							if (la.kind == 115) {
								goto case 510;
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
					currentState = 717;
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
					currentState = 712;
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
					goto case 417;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[129].Get(la.kind)) {
					currentState = 711;
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
					goto case 540;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 528;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 510;
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
					currentState = 501;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 491;
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
					currentState = 467;
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
						if (set[130].Get(la.kind)) {
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
					currentState = 465;
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
					goto case 461;
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
				if (set[131].Get(la.kind)) {
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
				if (set[132].Get(la.kind)) {
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
					if (set[133].Get(la.kind)) {
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
				if (set[134].Get(la.kind)) {
					currentState = 159;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 457;
						break;
					} else {
						if (set[135].Get(la.kind)) {
							currentState = 159;
							break;
						} else {
							if (set[130].Get(la.kind)) {
								currentState = 159;
								break;
							} else {
								if (set[133].Get(la.kind)) {
									currentState = 453;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 450;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 447;
											break;
										} else {
											if (set[81].Get(la.kind)) {
												stateStack.Push(159);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 430;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(159);
													goto case 236;
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
				if (set[136].Get(la.kind)) {
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
					goto case 235;
				} else {
					if (la.kind == 58) {
						stateStack.Push(175);
						goto case 234;
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
					currentState = 232;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 228;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 226;
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
										currentState = 222;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 220;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 218;
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
				if (set[120].Get(la.kind)) {
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
					goto case 210;
				} else {
					if (set[39].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 193;
							break;
						} else {
							if (set[39].Get(la.kind)) {
								goto case 208;
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
					stateStack.Push(201);
					goto case 189;
				} else {
					goto case 199;
				}
			}
			case 199: {
				stateStack.Push(200);
				goto case 56;
			}
			case 200: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 201: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 202;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (set[41].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 203;
				} else {
					goto case 199;
				}
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 63) {
					currentState = 205;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 199;
						break;
					} else {
						if (set[42].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 199;
						}
					}
				}
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				currentState = 199;
				break;
			}
			case 205: {
				PushContext(Context.Type, la, t);
				stateStack.Push(206);
				goto case 37;
			}
			case 206: {
				PopContext();
				goto case 207;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(20, la); // "="
				currentState = 199;
				break;
			}
			case 208: {
				stateStack.Push(209);
				goto case 197;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 22) {
					currentState = 208;
					break;
				} else {
					goto case 192;
				}
			}
			case 210: {
				stateStack.Push(211);
				goto case 217;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 215;
						break;
					} else {
						if (la.kind == 146) {
							goto case 210;
						} else {
							Error(la);
							goto case 211;
						}
					}
				} else {
					goto case 212;
				}
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(143, la); // "Into"
				currentState = 213;
				break;
			}
			case 213: {
				stateStack.Push(214);
				goto case 197;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.kind == 22) {
					currentState = 213;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 215: {
				stateStack.Push(216);
				goto case 217;
			}
			case 216: {
				stateStack.Push(211);
				goto case 212;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				Expect(146, la); // "Join"
				currentState = 177;
				break;
			}
			case 218: {
				stateStack.Push(219);
				goto case 197;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 22) {
					currentState = 218;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 220: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 221;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(70, la); // "By"
				currentState = 223;
				break;
			}
			case 223: {
				stateStack.Push(224);
				goto case 56;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 64) {
					currentState = 225;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 225;
						break;
					} else {
						Error(la);
						goto case 225;
					}
				}
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 22) {
					currentState = 223;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				stateStack.Push(227);
				goto case 197;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (la.kind == 22) {
					currentState = 226;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 228: {
				stateStack.Push(229);
				goto case 183;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (set[37].Get(la.kind)) {
					stateStack.Push(229);
					goto case 176;
				} else {
					Expect(143, la); // "Into"
					currentState = 230;
					break;
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 197;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 22) {
					currentState = 230;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 232: {
				stateStack.Push(233);
				goto case 183;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (la.kind == 22) {
					currentState = 232;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(58, la); // "Aggregate"
				currentState = 228;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(126, la); // "From"
				currentState = 232;
				break;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 210) {
					currentState = 422;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 237;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(37, la); // "("
				currentState = 238;
				break;
			}
			case 238: {
				SetIdentifierExpected(la);
				goto case 239;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(240);
					goto case 404;
				} else {
					goto case 240;
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				Expect(38, la); // ")"
				currentState = 241;
				break;
			}
			case 241: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 402;
							break;
						} else {
							goto case 243;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 243: {
				stateStack.Push(244);
				goto case 246;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				Expect(113, la); // "End"
				currentState = 245;
				break;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 246: {
				PushContext(Context.Body, la, t);
				goto case 247;
			}
			case 247: {
				stateStack.Push(248);
				goto case 23;
			}
			case 248: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 249;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (set[137].Get(la.kind)) {
					if (set[69].Get(la.kind)) {
						if (set[50].Get(la.kind)) {
							stateStack.Push(247);
							goto case 254;
						} else {
							goto case 247;
						}
					} else {
						if (la.kind == 113) {
							currentState = 252;
							break;
						} else {
							goto case 251;
						}
					}
				} else {
					goto case 250;
				}
			}
			case 250: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 251: {
				Error(la);
				goto case 248;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 247;
				} else {
					if (set[49].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 251;
					}
				}
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				currentState = 248;
				break;
			}
			case 254: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 255;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 382;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 378;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 376;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 374;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 356;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 341;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 337;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 331;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 304;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 300;
																break;
															} else {
																goto case 300;
															}
														} else {
															if (la.kind == 194) {
																currentState = 298;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 296;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 283;
																break;
															} else {
																if (set[138].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 280;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 279;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 278;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 97;
																				} else {
																					if (la.kind == 195) {
																						currentState = 275;
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
																		currentState = 273;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 271;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 256;
																				break;
																			} else {
																				if (set[139].Get(la.kind)) {
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
			case 256: {
				stateStack.Push(257);
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 260;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (la.kind == 22) {
					currentState = 256;
					break;
				} else {
					stateStack.Push(258);
					goto case 246;
				}
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(113, la); // "End"
				currentState = 259;
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(263);
					goto case 189;
				} else {
					goto case 261;
				}
			}
			case 261: {
				stateStack.Push(262);
				goto case 56;
			}
			case 262: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 263: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 264;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (set[41].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 265;
				} else {
					goto case 261;
				}
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (la.kind == 63) {
					currentState = 267;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 261;
						break;
					} else {
						if (set[42].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 261;
						}
					}
				}
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				currentState = 261;
				break;
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(269);
					goto case 37;
				} else {
					goto case 261;
				}
			}
			case 269: {
				PopContext();
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				Expect(20, la); // "="
				currentState = 261;
				break;
			}
			case 271: {
				stateStack.Push(272);
				goto case 56;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (la.kind == 22) {
					currentState = 271;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 273: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 275: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 276;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(277);
					goto case 56;
				} else {
					goto case 277;
				}
			}
			case 277: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
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
			case 279: {
				if (la == null) { currentState = 279; break; }
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
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (set[6].Get(la.kind)) {
					goto case 282;
				} else {
					if (la.kind == 5) {
						goto case 281;
					} else {
						goto case 6;
					}
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 283: {
				stateStack.Push(284);
				goto case 246;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 75) {
					currentState = 288;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 287;
						break;
					} else {
						goto case 285;
					}
				}
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				Expect(113, la); // "End"
				currentState = 286;
				break;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 287: {
				stateStack.Push(285);
				goto case 246;
			}
			case 288: {
				SetIdentifierExpected(la);
				goto case 289;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(292);
					goto case 189;
				} else {
					goto case 290;
				}
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				if (la.kind == 229) {
					currentState = 291;
					break;
				} else {
					goto case 283;
				}
			}
			case 291: {
				stateStack.Push(283);
				goto case 56;
			}
			case 292: {
				PopContext();
				goto case 293;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 63) {
					currentState = 294;
					break;
				} else {
					goto case 290;
				}
			}
			case 294: {
				PushContext(Context.Type, la, t);
				stateStack.Push(295);
				goto case 37;
			}
			case 295: {
				PopContext();
				goto case 290;
			}
			case 296: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 297;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 163) {
					goto case 104;
				} else {
					goto case 299;
				}
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 5) {
					goto case 281;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 282;
					} else {
						goto case 6;
					}
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(118, la); // "Error"
				currentState = 301;
				break;
			}
			case 301: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 302;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 299;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 303;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 304: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 305;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(321);
					goto case 315;
				} else {
					if (la.kind == 110) {
						currentState = 306;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 306: {
				stateStack.Push(307);
				goto case 315;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				Expect(138, la); // "In"
				currentState = 308;
				break;
			}
			case 308: {
				stateStack.Push(309);
				goto case 56;
			}
			case 309: {
				stateStack.Push(310);
				goto case 246;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(163, la); // "Next"
				currentState = 311;
				break;
			}
			case 311: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 312;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (set[23].Get(la.kind)) {
					goto case 313;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 313: {
				stateStack.Push(314);
				goto case 56;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 22) {
					currentState = 313;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 315: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(316);
				goto case 157;
			}
			case 316: {
				PopContext();
				goto case 317;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 33) {
					currentState = 318;
					break;
				} else {
					goto case 318;
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(318);
					goto case 144;
				} else {
					if (la.kind == 63) {
						currentState = 319;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 319: {
				PushContext(Context.Type, la, t);
				stateStack.Push(320);
				goto case 37;
			}
			case 320: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				Expect(20, la); // "="
				currentState = 322;
				break;
			}
			case 322: {
				stateStack.Push(323);
				goto case 56;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 205) {
					currentState = 330;
					break;
				} else {
					goto case 324;
				}
			}
			case 324: {
				stateStack.Push(325);
				goto case 246;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				Expect(163, la); // "Next"
				currentState = 326;
				break;
			}
			case 326: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 327;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (set[23].Get(la.kind)) {
					goto case 328;
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
				if (la == null) { currentState = 329; break; }
				if (la.kind == 22) {
					currentState = 328;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 330: {
				stateStack.Push(324);
				goto case 56;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 334;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(332);
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				Expect(152, la); // "Loop"
				currentState = 333;
				break;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 334: {
				stateStack.Push(335);
				goto case 56;
			}
			case 335: {
				stateStack.Push(336);
				goto case 246;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 337: {
				stateStack.Push(338);
				goto case 56;
			}
			case 338: {
				stateStack.Push(339);
				goto case 246;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				Expect(113, la); // "End"
				currentState = 340;
				break;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 341: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 74) {
					currentState = 343;
					break;
				} else {
					goto case 343;
				}
			}
			case 343: {
				stateStack.Push(344);
				goto case 56;
			}
			case 344: {
				stateStack.Push(345);
				goto case 23;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (la.kind == 74) {
					currentState = 347;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 346;
					break;
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 347: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 348;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 111) {
					currentState = 349;
					break;
				} else {
					if (set[67].Get(la.kind)) {
						goto case 350;
					} else {
						Error(la);
						goto case 349;
					}
				}
			}
			case 349: {
				stateStack.Push(345);
				goto case 246;
			}
			case 350: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 351;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (set[140].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 353;
						break;
					} else {
						goto case 353;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(352);
						goto case 56;
					} else {
						Error(la);
						goto case 352;
					}
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 22) {
					currentState = 350;
					break;
				} else {
					goto case 349;
				}
			}
			case 353: {
				stateStack.Push(354);
				goto case 355;
			}
			case 354: {
				stateStack.Push(352);
				goto case 59;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
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
			case 356: {
				stateStack.Push(357);
				goto case 56;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 214) {
					currentState = 365;
					break;
				} else {
					goto case 358;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 359;
				} else {
					goto case 6;
				}
			}
			case 359: {
				stateStack.Push(360);
				goto case 246;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 364;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 362;
							break;
						} else {
							Error(la);
							goto case 359;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 361;
					break;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 362: {
				stateStack.Push(363);
				goto case 56;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 214) {
					currentState = 359;
					break;
				} else {
					goto case 359;
				}
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 135) {
					currentState = 362;
					break;
				} else {
					goto case 359;
				}
			}
			case 365: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 366;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (set[50].Get(la.kind)) {
					goto case 367;
				} else {
					goto case 358;
				}
			}
			case 367: {
				stateStack.Push(368);
				goto case 254;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				if (la.kind == 21) {
					currentState = 372;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 369;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 369: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 370;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (set[50].Get(la.kind)) {
					stateStack.Push(371);
					goto case 254;
				} else {
					goto case 371;
				}
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 21) {
					currentState = 369;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 372: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 373;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (set[50].Get(la.kind)) {
					goto case 367;
				} else {
					goto case 368;
				}
			}
			case 374: {
				stateStack.Push(375);
				goto case 85;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 376: {
				stateStack.Push(377);
				goto case 56;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 378: {
				stateStack.Push(379);
				goto case 56;
			}
			case 379: {
				stateStack.Push(380);
				goto case 246;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				Expect(113, la); // "End"
				currentState = 381;
				break;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
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
			case 382: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(383);
				goto case 189;
			}
			case 383: {
				PopContext();
				goto case 384;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 33) {
					currentState = 385;
					break;
				} else {
					goto case 385;
				}
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (la.kind == 37) {
					currentState = 399;
					break;
				} else {
					goto case 386;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (la.kind == 22) {
					currentState = 392;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 389;
						break;
					} else {
						goto case 387;
					}
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 20) {
					goto case 388;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				currentState = 56;
				break;
			}
			case 389: {
				PushContext(Context.Type, la, t);
				goto case 390;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 162) {
					stateStack.Push(391);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(391);
						goto case 37;
					} else {
						Error(la);
						goto case 391;
					}
				}
			}
			case 391: {
				PopContext();
				goto case 387;
			}
			case 392: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(393);
				goto case 189;
			}
			case 393: {
				PopContext();
				goto case 394;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 33) {
					currentState = 395;
					break;
				} else {
					goto case 395;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 37) {
					currentState = 396;
					break;
				} else {
					goto case 386;
				}
			}
			case 396: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 397;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(398);
					goto case 56;
				} else {
					goto case 398;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 22) {
					currentState = 396;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 395;
					break;
				}
			}
			case 399: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 400;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(401);
					goto case 56;
				} else {
					goto case 401;
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (la.kind == 22) {
					currentState = 399;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 385;
					break;
				}
			}
			case 402: {
				PushContext(Context.Type, la, t);
				stateStack.Push(403);
				goto case 37;
			}
			case 403: {
				PopContext();
				goto case 243;
			}
			case 404: {
				stateStack.Push(405);
				PushContext(Context.Parameter, la, t);
				goto case 406;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 22) {
					currentState = 404;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 406: {
				SetIdentifierExpected(la);
				goto case 407;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (la.kind == 40) {
					stateStack.Push(406);
					goto case 417;
				} else {
					goto case 408;
				}
			}
			case 408: {
				SetIdentifierExpected(la);
				goto case 409;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (set[141].Get(la.kind)) {
					currentState = 408;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(410);
					goto case 189;
				}
			}
			case 410: {
				PopContext();
				goto case 411;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 63) {
					currentState = 415;
					break;
				} else {
					goto case 412;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 20) {
					currentState = 414;
					break;
				} else {
					goto case 413;
				}
			}
			case 413: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 414: {
				stateStack.Push(413);
				goto case 56;
			}
			case 415: {
				PushContext(Context.Type, la, t);
				stateStack.Push(416);
				goto case 37;
			}
			case 416: {
				PopContext();
				goto case 412;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				Expect(40, la); // "<"
				currentState = 418;
				break;
			}
			case 418: {
				PushContext(Context.Attribute, la, t);
				goto case 419;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (set[142].Get(la.kind)) {
					currentState = 419;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 420;
					break;
				}
			}
			case 420: {
				PopContext();
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(37, la); // "("
				currentState = 423;
				break;
			}
			case 423: {
				SetIdentifierExpected(la);
				goto case 424;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(425);
					goto case 404;
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				Expect(38, la); // ")"
				currentState = 426;
				break;
			}
			case 426: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 427;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (set[50].Get(la.kind)) {
					goto case 254;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(428);
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				Expect(113, la); // "End"
				currentState = 429;
				break;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 443;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(432);
						goto case 434;
					} else {
						Error(la);
						goto case 431;
					}
				}
			}
			case 431: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 17) {
					currentState = 433;
					break;
				} else {
					goto case 431;
				}
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 16) {
					currentState = 432;
					break;
				} else {
					goto case 432;
				}
			}
			case 434: {
				PushContext(Context.Xml, la, t);
				goto case 435;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 436;
				break;
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (set[143].Get(la.kind)) {
					if (set[144].Get(la.kind)) {
						currentState = 436;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(436);
							goto case 440;
						} else {
							Error(la);
							goto case 436;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 437;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 438;
							break;
						} else {
							Error(la);
							goto case 437;
						}
					}
				}
			}
			case 437: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (set[145].Get(la.kind)) {
					if (set[146].Get(la.kind)) {
						currentState = 438;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(438);
							goto case 440;
						} else {
							if (la.kind == 10) {
								stateStack.Push(438);
								goto case 434;
							} else {
								Error(la);
								goto case 438;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 439;
					break;
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (set[147].Get(la.kind)) {
					if (set[148].Get(la.kind)) {
						currentState = 439;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(439);
							goto case 440;
						} else {
							Error(la);
							goto case 439;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 437;
					break;
				}
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 441;
				break;
			}
			case 441: {
				stateStack.Push(442);
				goto case 56;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 16) {
					currentState = 444;
					break;
				} else {
					goto case 444;
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 443;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(445);
						goto case 434;
					} else {
						goto case 431;
					}
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 17) {
					currentState = 446;
					break;
				} else {
					goto case 431;
				}
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 16) {
					currentState = 445;
					break;
				} else {
					goto case 445;
				}
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				Expect(37, la); // "("
				currentState = 448;
				break;
			}
			case 448: {
				readXmlIdentifier = true;
				stateStack.Push(449);
				goto case 189;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				Expect(38, la); // ")"
				currentState = 159;
				break;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				Expect(37, la); // "("
				currentState = 451;
				break;
			}
			case 451: {
				PushContext(Context.Type, la, t);
				stateStack.Push(452);
				goto case 37;
			}
			case 452: {
				PopContext();
				goto case 449;
			}
			case 453: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 454;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (la.kind == 10) {
					currentState = 455;
					break;
				} else {
					goto case 455;
				}
			}
			case 455: {
				stateStack.Push(456);
				goto case 85;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (la.kind == 11) {
					currentState = 159;
					break;
				} else {
					goto case 159;
				}
			}
			case 457: {
				activeArgument = 0;
				goto case 458;
			}
			case 458: {
				stateStack.Push(459);
				goto case 56;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 22) {
					currentState = 460;
					break;
				} else {
					goto case 449;
				}
			}
			case 460: {
				activeArgument++;
				goto case 458;
			}
			case 461: {
				stateStack.Push(462);
				goto case 56;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (la.kind == 22) {
					currentState = 463;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 463: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 464;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (set[23].Get(la.kind)) {
					goto case 461;
				} else {
					goto case 462;
				}
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(466);
					goto case 37;
				} else {
					goto case 466;
				}
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 22) {
					currentState = 465;
					break;
				} else {
					goto case 45;
				}
			}
			case 467: {
				SetIdentifierExpected(la);
				goto case 468;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (set[149].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 470;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(469);
							goto case 404;
						} else {
							Error(la);
							goto case 469;
						}
					}
				} else {
					goto case 469;
				}
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 470: {
				stateStack.Push(469);
				goto case 471;
			}
			case 471: {
				SetIdentifierExpected(la);
				goto case 472;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 473;
					break;
				} else {
					goto case 473;
				}
			}
			case 473: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(474);
				goto case 488;
			}
			case 474: {
				PopContext();
				goto case 475;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (la.kind == 63) {
					currentState = 489;
					break;
				} else {
					goto case 476;
				}
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 22) {
					currentState = 477;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 477: {
				SetIdentifierExpected(la);
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 479;
					break;
				} else {
					goto case 479;
				}
			}
			case 479: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(480);
				goto case 488;
			}
			case 480: {
				PopContext();
				goto case 481;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 63) {
					currentState = 482;
					break;
				} else {
					goto case 476;
				}
			}
			case 482: {
				PushContext(Context.Type, la, t);
				stateStack.Push(483);
				goto case 484;
			}
			case 483: {
				PopContext();
				goto case 476;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (set[92].Get(la.kind)) {
					goto case 487;
				} else {
					if (la.kind == 35) {
						currentState = 485;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 485: {
				stateStack.Push(486);
				goto case 487;
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				if (la.kind == 22) {
					currentState = 485;
					break;
				} else {
					goto case 66;
				}
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
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
			case 488: {
				if (la == null) { currentState = 488; break; }
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
			case 489: {
				PushContext(Context.Type, la, t);
				stateStack.Push(490);
				goto case 484;
			}
			case 490: {
				PopContext();
				goto case 476;
			}
			case 491: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(492);
				goto case 189;
			}
			case 492: {
				PopContext();
				goto case 493;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 37) {
					currentState = 498;
					break;
				} else {
					goto case 494;
				}
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 63) {
					currentState = 495;
					break;
				} else {
					goto case 23;
				}
			}
			case 495: {
				PushContext(Context.Type, la, t);
				goto case 496;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (la.kind == 40) {
					stateStack.Push(496);
					goto case 417;
				} else {
					stateStack.Push(497);
					goto case 37;
				}
			}
			case 497: {
				PopContext();
				goto case 23;
			}
			case 498: {
				SetIdentifierExpected(la);
				goto case 499;
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(500);
					goto case 404;
				} else {
					goto case 500;
				}
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				Expect(38, la); // ")"
				currentState = 494;
				break;
			}
			case 501: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(502);
				goto case 189;
			}
			case 502: {
				PopContext();
				goto case 503;
			}
			case 503: {
				if (la == null) { currentState = 503; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 508;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 505;
							break;
						} else {
							goto case 504;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 504: {
				Error(la);
				goto case 23;
			}
			case 505: {
				SetIdentifierExpected(la);
				goto case 506;
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(507);
					goto case 404;
				} else {
					goto case 507;
				}
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 508: {
				PushContext(Context.Type, la, t);
				stateStack.Push(509);
				goto case 37;
			}
			case 509: {
				PopContext();
				goto case 23;
			}
			case 510: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 511;
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				Expect(115, la); // "Enum"
				currentState = 512;
				break;
			}
			case 512: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(513);
				goto case 189;
			}
			case 513: {
				PopContext();
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 63) {
					currentState = 526;
					break;
				} else {
					goto case 515;
				}
			}
			case 515: {
				stateStack.Push(516);
				goto case 23;
			}
			case 516: {
				SetIdentifierExpected(la);
				goto case 517;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (set[95].Get(la.kind)) {
					goto case 521;
				} else {
					Expect(113, la); // "End"
					currentState = 518;
					break;
				}
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				Expect(115, la); // "Enum"
				currentState = 519;
				break;
			}
			case 519: {
				stateStack.Push(520);
				goto case 23;
			}
			case 520: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 521: {
				SetIdentifierExpected(la);
				goto case 522;
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				if (la.kind == 40) {
					stateStack.Push(521);
					goto case 417;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(523);
					goto case 189;
				}
			}
			case 523: {
				PopContext();
				goto case 524;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				if (la.kind == 20) {
					currentState = 525;
					break;
				} else {
					goto case 515;
				}
			}
			case 525: {
				stateStack.Push(515);
				goto case 56;
			}
			case 526: {
				PushContext(Context.Type, la, t);
				stateStack.Push(527);
				goto case 37;
			}
			case 527: {
				PopContext();
				goto case 515;
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				Expect(103, la); // "Delegate"
				currentState = 529;
				break;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (la.kind == 210) {
					currentState = 530;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 530;
						break;
					} else {
						Error(la);
						goto case 530;
					}
				}
			}
			case 530: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 531;
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				currentState = 532;
				break;
			}
			case 532: {
				PopContext();
				goto case 533;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (la.kind == 37) {
					currentState = 536;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 534;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 534: {
				PushContext(Context.Type, la, t);
				stateStack.Push(535);
				goto case 37;
			}
			case 535: {
				PopContext();
				goto case 23;
			}
			case 536: {
				SetIdentifierExpected(la);
				goto case 537;
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (set[149].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 539;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(538);
							goto case 404;
						} else {
							Error(la);
							goto case 538;
						}
					}
				} else {
					goto case 538;
				}
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				Expect(38, la); // ")"
				currentState = 533;
				break;
			}
			case 539: {
				stateStack.Push(538);
				goto case 471;
			}
			case 540: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 541;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 155) {
					currentState = 542;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 542;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 542;
							break;
						} else {
							Error(la);
							goto case 542;
						}
					}
				}
			}
			case 542: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(543);
				goto case 189;
			}
			case 543: {
				PopContext();
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (la.kind == 37) {
					currentState = 708;
					break;
				} else {
					goto case 545;
				}
			}
			case 545: {
				stateStack.Push(546);
				goto case 23;
			}
			case 546: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 547;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 705;
				} else {
					goto case 548;
				}
			}
			case 548: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 549;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 699;
				} else {
					goto case 550;
				}
			}
			case 550: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 551;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (set[99].Get(la.kind)) {
					goto case 556;
				} else {
					isMissingModifier = false;
					goto case 552;
				}
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				Expect(113, la); // "End"
				currentState = 553;
				break;
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				if (la.kind == 155) {
					currentState = 554;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 554;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 554;
							break;
						} else {
							Error(la);
							goto case 554;
						}
					}
				}
			}
			case 554: {
				stateStack.Push(555);
				goto case 23;
			}
			case 555: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 556: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 557;
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 40) {
					stateStack.Push(556);
					goto case 417;
				} else {
					isMissingModifier = true;
					goto case 558;
				}
			}
			case 558: {
				SetIdentifierExpected(la);
				goto case 559;
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				if (set[129].Get(la.kind)) {
					currentState = 698;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 560;
				}
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(550);
					goto case 540;
				} else {
					if (la.kind == 103) {
						stateStack.Push(550);
						goto case 528;
					} else {
						if (la.kind == 115) {
							stateStack.Push(550);
							goto case 510;
						} else {
							if (la.kind == 142) {
								stateStack.Push(550);
								goto case 9;
							} else {
								if (set[102].Get(la.kind)) {
									stateStack.Push(550);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 561;
								} else {
									Error(la);
									goto case 550;
								}
							}
						}
					}
				}
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				if (set[119].Get(la.kind)) {
					stateStack.Push(562);
					goto case 683;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(562);
						goto case 665;
					} else {
						if (la.kind == 101) {
							stateStack.Push(562);
							goto case 649;
						} else {
							if (la.kind == 119) {
								stateStack.Push(562);
								goto case 634;
							} else {
								if (la.kind == 98) {
									stateStack.Push(562);
									goto case 622;
								} else {
									if (la.kind == 186) {
										stateStack.Push(562);
										goto case 577;
									} else {
										if (la.kind == 172) {
											stateStack.Push(562);
											goto case 563;
										} else {
											Error(la);
											goto case 562;
										}
									}
								}
							}
						}
					}
				}
			}
			case 562: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				Expect(172, la); // "Operator"
				currentState = 564;
				break;
			}
			case 564: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 565;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				currentState = 566;
				break;
			}
			case 566: {
				PopContext();
				goto case 567;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				Expect(37, la); // "("
				currentState = 568;
				break;
			}
			case 568: {
				stateStack.Push(569);
				goto case 404;
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				Expect(38, la); // ")"
				currentState = 570;
				break;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				if (la.kind == 63) {
					currentState = 574;
					break;
				} else {
					goto case 571;
				}
			}
			case 571: {
				stateStack.Push(572);
				goto case 246;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				Expect(113, la); // "End"
				currentState = 573;
				break;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 574: {
				PushContext(Context.Type, la, t);
				goto case 575;
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				if (la.kind == 40) {
					stateStack.Push(575);
					goto case 417;
				} else {
					stateStack.Push(576);
					goto case 37;
				}
			}
			case 576: {
				PopContext();
				goto case 571;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				Expect(186, la); // "Property"
				currentState = 578;
				break;
			}
			case 578: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(579);
				goto case 189;
			}
			case 579: {
				PopContext();
				goto case 580;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 37) {
					currentState = 619;
					break;
				} else {
					goto case 581;
				}
			}
			case 581: {
				if (la == null) { currentState = 581; break; }
				if (la.kind == 63) {
					currentState = 617;
					break;
				} else {
					goto case 582;
				}
			}
			case 582: {
				if (la == null) { currentState = 582; break; }
				if (la.kind == 136) {
					currentState = 612;
					break;
				} else {
					goto case 583;
				}
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 20) {
					currentState = 611;
					break;
				} else {
					goto case 584;
				}
			}
			case 584: {
				stateStack.Push(585);
				goto case 23;
			}
			case 585: {
				PopContext();
				goto case 586;
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (la.kind == 40) {
					stateStack.Push(586);
					goto case 417;
				} else {
					goto case 587;
				}
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				if (set[150].Get(la.kind)) {
					currentState = 610;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 588;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				if (la.kind == 128) {
					currentState = 589;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 589;
						break;
					} else {
						Error(la);
						goto case 589;
					}
				}
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (la.kind == 37) {
					currentState = 607;
					break;
				} else {
					goto case 590;
				}
			}
			case 590: {
				stateStack.Push(591);
				goto case 246;
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				Expect(113, la); // "End"
				currentState = 592;
				break;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				if (la.kind == 128) {
					currentState = 593;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 593;
						break;
					} else {
						Error(la);
						goto case 593;
					}
				}
			}
			case 593: {
				stateStack.Push(594);
				goto case 23;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				if (set[108].Get(la.kind)) {
					goto case 597;
				} else {
					goto case 595;
				}
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				Expect(113, la); // "End"
				currentState = 596;
				break;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				if (la.kind == 40) {
					stateStack.Push(597);
					goto case 417;
				} else {
					goto case 598;
				}
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				if (set[150].Get(la.kind)) {
					currentState = 598;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 599;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 599;
							break;
						} else {
							Error(la);
							goto case 599;
						}
					}
				}
			}
			case 599: {
				if (la == null) { currentState = 599; break; }
				if (la.kind == 37) {
					currentState = 604;
					break;
				} else {
					goto case 600;
				}
			}
			case 600: {
				stateStack.Push(601);
				goto case 246;
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				Expect(113, la); // "End"
				currentState = 602;
				break;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (la.kind == 128) {
					currentState = 603;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 603;
						break;
					} else {
						Error(la);
						goto case 603;
					}
				}
			}
			case 603: {
				stateStack.Push(595);
				goto case 23;
			}
			case 604: {
				SetIdentifierExpected(la);
				goto case 605;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(606);
					goto case 404;
				} else {
					goto case 606;
				}
			}
			case 606: {
				if (la == null) { currentState = 606; break; }
				Expect(38, la); // ")"
				currentState = 600;
				break;
			}
			case 607: {
				SetIdentifierExpected(la);
				goto case 608;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(609);
					goto case 404;
				} else {
					goto case 609;
				}
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				Expect(38, la); // ")"
				currentState = 590;
				break;
			}
			case 610: {
				SetIdentifierExpected(la);
				goto case 587;
			}
			case 611: {
				stateStack.Push(584);
				goto case 56;
			}
			case 612: {
				PushContext(Context.Type, la, t);
				stateStack.Push(613);
				goto case 37;
			}
			case 613: {
				PopContext();
				goto case 614;
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (la.kind == 22) {
					currentState = 615;
					break;
				} else {
					goto case 583;
				}
			}
			case 615: {
				PushContext(Context.Type, la, t);
				stateStack.Push(616);
				goto case 37;
			}
			case 616: {
				PopContext();
				goto case 614;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (la.kind == 40) {
					stateStack.Push(617);
					goto case 417;
				} else {
					if (la.kind == 162) {
						stateStack.Push(582);
						goto case 69;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(618);
							goto case 37;
						} else {
							Error(la);
							goto case 582;
						}
					}
				}
			}
			case 618: {
				PopContext();
				goto case 582;
			}
			case 619: {
				SetIdentifierExpected(la);
				goto case 620;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(621);
					goto case 404;
				} else {
					goto case 621;
				}
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				Expect(38, la); // ")"
				currentState = 581;
				break;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(98, la); // "Custom"
				currentState = 623;
				break;
			}
			case 623: {
				stateStack.Push(624);
				goto case 634;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				if (set[113].Get(la.kind)) {
					goto case 626;
				} else {
					Expect(113, la); // "End"
					currentState = 625;
					break;
				}
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 40) {
					stateStack.Push(626);
					goto case 417;
				} else {
					if (la.kind == 56) {
						currentState = 627;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 627;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 627;
								break;
							} else {
								Error(la);
								goto case 627;
							}
						}
					}
				}
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				Expect(37, la); // "("
				currentState = 628;
				break;
			}
			case 628: {
				stateStack.Push(629);
				goto case 404;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				Expect(38, la); // ")"
				currentState = 630;
				break;
			}
			case 630: {
				stateStack.Push(631);
				goto case 246;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				Expect(113, la); // "End"
				currentState = 632;
				break;
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				if (la.kind == 56) {
					currentState = 633;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 633;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 633;
							break;
						} else {
							Error(la);
							goto case 633;
						}
					}
				}
			}
			case 633: {
				stateStack.Push(624);
				goto case 23;
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				Expect(119, la); // "Event"
				currentState = 635;
				break;
			}
			case 635: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(636);
				goto case 189;
			}
			case 636: {
				PopContext();
				goto case 637;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				if (la.kind == 63) {
					currentState = 647;
					break;
				} else {
					if (set[151].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 644;
							break;
						} else {
							goto case 638;
						}
					} else {
						Error(la);
						goto case 638;
					}
				}
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (la.kind == 136) {
					currentState = 639;
					break;
				} else {
					goto case 23;
				}
			}
			case 639: {
				PushContext(Context.Type, la, t);
				stateStack.Push(640);
				goto case 37;
			}
			case 640: {
				PopContext();
				goto case 641;
			}
			case 641: {
				if (la == null) { currentState = 641; break; }
				if (la.kind == 22) {
					currentState = 642;
					break;
				} else {
					goto case 23;
				}
			}
			case 642: {
				PushContext(Context.Type, la, t);
				stateStack.Push(643);
				goto case 37;
			}
			case 643: {
				PopContext();
				goto case 641;
			}
			case 644: {
				SetIdentifierExpected(la);
				goto case 645;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(646);
					goto case 404;
				} else {
					goto case 646;
				}
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				Expect(38, la); // ")"
				currentState = 638;
				break;
			}
			case 647: {
				PushContext(Context.Type, la, t);
				stateStack.Push(648);
				goto case 37;
			}
			case 648: {
				PopContext();
				goto case 638;
			}
			case 649: {
				if (la == null) { currentState = 649; break; }
				Expect(101, la); // "Declare"
				currentState = 650;
				break;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 651;
					break;
				} else {
					goto case 651;
				}
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				if (la.kind == 210) {
					currentState = 652;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 652;
						break;
					} else {
						Error(la);
						goto case 652;
					}
				}
			}
			case 652: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(653);
				goto case 189;
			}
			case 653: {
				PopContext();
				goto case 654;
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				Expect(149, la); // "Lib"
				currentState = 655;
				break;
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				Expect(3, la); // LiteralString
				currentState = 656;
				break;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				if (la.kind == 59) {
					currentState = 664;
					break;
				} else {
					goto case 657;
				}
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				if (la.kind == 37) {
					currentState = 661;
					break;
				} else {
					goto case 658;
				}
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				if (la.kind == 63) {
					currentState = 659;
					break;
				} else {
					goto case 23;
				}
			}
			case 659: {
				PushContext(Context.Type, la, t);
				stateStack.Push(660);
				goto case 37;
			}
			case 660: {
				PopContext();
				goto case 23;
			}
			case 661: {
				SetIdentifierExpected(la);
				goto case 662;
			}
			case 662: {
				if (la == null) { currentState = 662; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(663);
					goto case 404;
				} else {
					goto case 663;
				}
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
				Expect(38, la); // ")"
				currentState = 658;
				break;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				Expect(3, la); // LiteralString
				currentState = 657;
				break;
			}
			case 665: {
				if (la == null) { currentState = 665; break; }
				if (la.kind == 210) {
					currentState = 666;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 666;
						break;
					} else {
						Error(la);
						goto case 666;
					}
				}
			}
			case 666: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 667;
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				currentState = 668;
				break;
			}
			case 668: {
				PopContext();
				goto case 669;
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				if (la.kind == 37) {
					currentState = 679;
					break;
				} else {
					if (la.kind == 134 || la.kind == 136) {
						currentState = 676;
						break;
					} else {
						goto case 670;
					}
				}
			}
			case 670: {
				if (la == null) { currentState = 670; break; }
				if (la.kind == 63) {
					currentState = 674;
					break;
				} else {
					goto case 671;
				}
			}
			case 671: {
				stateStack.Push(672);
				goto case 246;
			}
			case 672: {
				if (la == null) { currentState = 672; break; }
				Expect(113, la); // "End"
				currentState = 673;
				break;
			}
			case 673: {
				if (la == null) { currentState = 673; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 504;
					}
				}
			}
			case 674: {
				PushContext(Context.Type, la, t);
				stateStack.Push(675);
				goto case 37;
			}
			case 675: {
				PopContext();
				goto case 671;
			}
			case 676: {
				if (la == null) { currentState = 676; break; }
				if (la.kind == 153 || la.kind == 158 || la.kind == 159) {
					currentState = 678;
					break;
				} else {
					goto case 677;
				}
			}
			case 677: {
				stateStack.Push(670);
				goto case 37;
			}
			case 678: {
				if (la == null) { currentState = 678; break; }
				Expect(26, la); // "."
				currentState = 677;
				break;
			}
			case 679: {
				SetIdentifierExpected(la);
				goto case 680;
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				if (set[149].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 682;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(681);
							goto case 404;
						} else {
							Error(la);
							goto case 681;
						}
					}
				} else {
					goto case 681;
				}
			}
			case 681: {
				if (la == null) { currentState = 681; break; }
				Expect(38, la); // ")"
				currentState = 669;
				break;
			}
			case 682: {
				stateStack.Push(681);
				goto case 471;
			}
			case 683: {
				stateStack.Push(684);
				SetIdentifierExpected(la);
				goto case 685;
			}
			case 684: {
				if (la == null) { currentState = 684; break; }
				if (la.kind == 22) {
					currentState = 683;
					break;
				} else {
					goto case 23;
				}
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				if (la.kind == 88) {
					currentState = 686;
					break;
				} else {
					goto case 686;
				}
			}
			case 686: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(687);
				goto case 697;
			}
			case 687: {
				PopContext();
				goto case 688;
			}
			case 688: {
				if (la == null) { currentState = 688; break; }
				if (la.kind == 33) {
					currentState = 689;
					break;
				} else {
					goto case 689;
				}
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				if (la.kind == 37) {
					currentState = 694;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 691;
						break;
					} else {
						goto case 690;
					}
				}
			}
			case 690: {
				if (la == null) { currentState = 690; break; }
				if (la.kind == 20) {
					goto case 388;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 691: {
				PushContext(Context.Type, la, t);
				goto case 692;
			}
			case 692: {
				if (la == null) { currentState = 692; break; }
				if (la.kind == 162) {
					stateStack.Push(693);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(693);
						goto case 37;
					} else {
						Error(la);
						goto case 693;
					}
				}
			}
			case 693: {
				PopContext();
				goto case 690;
			}
			case 694: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 695;
			}
			case 695: {
				if (la == null) { currentState = 695; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(696);
					goto case 56;
				} else {
					goto case 696;
				}
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				if (la.kind == 22) {
					currentState = 694;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 689;
					break;
				}
			}
			case 697: {
				if (la == null) { currentState = 697; break; }
				if (set[135].Get(la.kind)) {
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
			case 698: {
				isMissingModifier = false;
				goto case 558;
			}
			case 699: {
				if (la == null) { currentState = 699; break; }
				Expect(136, la); // "Implements"
				currentState = 700;
				break;
			}
			case 700: {
				PushContext(Context.Type, la, t);
				stateStack.Push(701);
				goto case 37;
			}
			case 701: {
				PopContext();
				goto case 702;
			}
			case 702: {
				if (la == null) { currentState = 702; break; }
				if (la.kind == 22) {
					currentState = 703;
					break;
				} else {
					stateStack.Push(550);
					goto case 23;
				}
			}
			case 703: {
				PushContext(Context.Type, la, t);
				stateStack.Push(704);
				goto case 37;
			}
			case 704: {
				PopContext();
				goto case 702;
			}
			case 705: {
				if (la == null) { currentState = 705; break; }
				Expect(140, la); // "Inherits"
				currentState = 706;
				break;
			}
			case 706: {
				PushContext(Context.Type, la, t);
				stateStack.Push(707);
				goto case 37;
			}
			case 707: {
				PopContext();
				stateStack.Push(548);
				goto case 23;
			}
			case 708: {
				if (la == null) { currentState = 708; break; }
				Expect(169, la); // "Of"
				currentState = 709;
				break;
			}
			case 709: {
				stateStack.Push(710);
				goto case 471;
			}
			case 710: {
				if (la == null) { currentState = 710; break; }
				Expect(38, la); // ")"
				currentState = 545;
				break;
			}
			case 711: {
				isMissingModifier = false;
				goto case 28;
			}
			case 712: {
				PushContext(Context.Type, la, t);
				stateStack.Push(713);
				goto case 37;
			}
			case 713: {
				PopContext();
				goto case 714;
			}
			case 714: {
				if (la == null) { currentState = 714; break; }
				if (la.kind == 22) {
					currentState = 715;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 715: {
				PushContext(Context.Type, la, t);
				stateStack.Push(716);
				goto case 37;
			}
			case 716: {
				PopContext();
				goto case 714;
			}
			case 717: {
				if (la == null) { currentState = 717; break; }
				Expect(169, la); // "Of"
				currentState = 718;
				break;
			}
			case 718: {
				stateStack.Push(719);
				goto case 471;
			}
			case 719: {
				if (la == null) { currentState = 719; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 720: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 721;
			}
			case 721: {
				if (la == null) { currentState = 721; break; }
				if (set[49].Get(la.kind)) {
					currentState = 721;
					break;
				} else {
					PopContext();
					stateStack.Push(722);
					goto case 23;
				}
			}
			case 722: {
				if (la == null) { currentState = 722; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(722);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 723;
					break;
				}
			}
			case 723: {
				if (la == null) { currentState = 723; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 724: {
				if (la == null) { currentState = 724; break; }
				Expect(137, la); // "Imports"
				currentState = 725;
				break;
			}
			case 725: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 726;
			}
			case 726: {
				if (la == null) { currentState = 726; break; }
				if (set[152].Get(la.kind)) {
					currentState = 732;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 728;
						break;
					} else {
						Error(la);
						goto case 727;
					}
				}
			}
			case 727: {
				PopContext();
				goto case 23;
			}
			case 728: {
				stateStack.Push(729);
				goto case 189;
			}
			case 729: {
				if (la == null) { currentState = 729; break; }
				Expect(20, la); // "="
				currentState = 730;
				break;
			}
			case 730: {
				if (la == null) { currentState = 730; break; }
				Expect(3, la); // LiteralString
				currentState = 731;
				break;
			}
			case 731: {
				if (la == null) { currentState = 731; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 727;
				break;
			}
			case 732: {
				if (la == null) { currentState = 732; break; }
				if (la.kind == 37) {
					stateStack.Push(732);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 733;
						break;
					} else {
						goto case 727;
					}
				}
			}
			case 733: {
				stateStack.Push(727);
				goto case 37;
			}
			case 734: {
				if (la == null) { currentState = 734; break; }
				Expect(173, la); // "Option"
				currentState = 735;
				break;
			}
			case 735: {
				if (la == null) { currentState = 735; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 737;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 736;
						break;
					} else {
						goto case 504;
					}
				}
			}
			case 736: {
				if (la == null) { currentState = 736; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 504;
					}
				}
			}
			case 737: {
				if (la == null) { currentState = 737; break; }
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
		new BitArray(new int[] {4194304, 67108864, 64, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {66189314, -1174405161, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
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