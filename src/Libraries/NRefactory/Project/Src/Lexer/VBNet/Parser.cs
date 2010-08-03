using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 244;
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
			case 70:
			case 245:
			case 480:
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
			case 173:
			case 179:
			case 185:
			case 222:
			case 226:
			case 265:
			case 365:
			case 374:
			case 427:
			case 467:
			case 477:
			case 488:
			case 518:
			case 554:
			case 611:
			case 628:
			case 700:
				return set[6];
			case 12:
			case 13:
			case 519:
			case 520:
			case 565:
			case 575:
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
			case 237:
			case 240:
			case 241:
			case 251:
			case 266:
			case 270:
			case 292:
			case 307:
			case 318:
			case 321:
			case 327:
			case 332:
			case 341:
			case 342:
			case 362:
			case 382:
			case 473:
			case 485:
			case 491:
			case 495:
			case 503:
			case 511:
			case 521:
			case 530:
			case 547:
			case 552:
			case 560:
			case 566:
			case 569:
			case 576:
			case 579:
			case 606:
			case 609:
			case 636:
			case 647:
			case 651:
			case 679:
			case 699:
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
			case 238:
			case 252:
			case 268:
			case 322:
			case 363:
			case 407:
			case 528:
			case 548:
			case 567:
			case 571:
			case 577:
			case 607:
			case 648:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 496:
			case 531:
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
			case 683:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 132:
			case 195:
			case 196:
			case 246:
			case 397:
			case 398:
			case 415:
			case 416:
			case 417:
			case 418:
			case 506:
			case 507:
			case 540:
			case 541:
			case 642:
			case 643:
			case 692:
			case 693:
				return set[14];
			case 33:
			case 34:
			case 468:
			case 469:
			case 478:
			case 479:
			case 508:
			case 509:
			case 633:
				return set[15];
			case 35:
			case 37:
			case 137:
			case 148:
			case 151:
			case 167:
			case 183:
			case 199:
			case 277:
			case 302:
			case 381:
			case 394:
			case 430:
			case 484:
			case 502:
			case 510:
			case 588:
			case 591:
			case 615:
			case 618:
			case 623:
			case 635:
			case 650:
			case 653:
			case 672:
			case 675:
			case 678:
			case 684:
			case 687:
			case 705:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 78:
			case 82:
			case 143:
			case 357:
			case 434:
				return set[19];
			case 42:
			case 157:
			case 164:
			case 169:
			case 231:
			case 401:
			case 426:
			case 429:
			case 542:
			case 543:
			case 603:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 145:
			case 146:
				return set[20];
			case 45:
			case 147:
			case 168:
			case 234:
			case 379:
			case 404:
			case 428:
			case 431:
			case 445:
			case 476:
			case 483:
			case 514:
			case 545:
			case 582:
			case 585:
			case 597:
			case 605:
			case 622:
			case 639:
			case 657:
			case 682:
			case 691:
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
			case 439:
			case 440:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 159:
			case 166:
			case 360:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 54:
			case 149:
			case 150:
			case 152:
			case 161:
			case 378:
			case 380:
			case 384:
			case 392:
			case 438:
			case 442:
			case 452:
			case 459:
			case 466:
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
			case 64:
			case 80:
			case 135:
			case 158:
			case 160:
			case 162:
			case 165:
			case 175:
			case 177:
			case 217:
			case 250:
			case 254:
			case 256:
			case 257:
			case 274:
			case 291:
			case 296:
			case 305:
			case 311:
			case 313:
			case 317:
			case 320:
			case 326:
			case 337:
			case 339:
			case 345:
			case 359:
			case 361:
			case 393:
			case 420:
			case 436:
			case 437:
			case 501:
			case 587:
				return set[23];
			case 58:
			case 62:
			case 138:
				return set[24];
			case 63:
			case 73:
			case 75:
			case 129:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 65:
			case 81:
			case 462:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 67:
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 68:
			case 69:
				return set[25];
			case 71:
			case 74:
			case 130:
			case 133:
				return set[26];
			case 72:
			case 83:
			case 128:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 77:
			case 654:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 79:
			case 198:
			case 200:
			case 201:
			case 304:
			case 701:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 84:
			case 323:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 88:
			case 269:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 92:
			case 408:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 96:
			case 329:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 97:
			case 553:
			case 572:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 100:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 101:
			case 286:
			case 293:
			case 308:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 104:
			case 204:
			case 209:
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 105:
			case 206:
			case 210:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 107:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 108:
			case 239:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 109:
			case 131:
			case 229:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 111:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 112:
			case 176:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 116:
			case 598:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 119:
			case 188:
			case 216:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 120:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 121:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 122:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 124:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 125:
			case 228:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 126:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 127:
				return set[27];
			case 134:
				return set[28];
			case 136:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 139:
				return set[29];
			case 140:
				return set[30];
			case 141:
			case 142:
			case 432:
			case 433:
				return set[31];
			case 144:
				return set[32];
			case 153:
			case 154:
			case 289:
			case 298:
				return set[33];
			case 155:
			case 410:
				return set[34];
			case 156:
			case 344:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 163:
				return set[35];
			case 170:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 171:
			case 172:
				return set[36];
			case 174:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 178:
			case 192:
			case 208:
			case 213:
			case 219:
			case 221:
			case 225:
			case 227:
				return set[37];
			case 180:
			case 181:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 182:
			case 184:
			case 290:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 186:
			case 187:
			case 189:
			case 191:
			case 193:
			case 194:
			case 202:
			case 207:
			case 212:
			case 220:
			case 224:
				return set[38];
			case 190:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 197:
				return set[39];
			case 203:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 205:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 214:
			case 215:
				return set[40];
			case 218:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 223:
				return set[41];
			case 230:
			case 505:
			case 627:
			case 641:
			case 649:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 232:
			case 233:
			case 402:
			case 403:
			case 474:
			case 475:
			case 481:
			case 482:
			case 580:
			case 581:
			case 583:
			case 584:
			case 595:
			case 596:
			case 620:
			case 621:
			case 637:
			case 638:
				return set[42];
			case 235:
			case 236:
				return set[43];
			case 242:
			case 243:
				return set[44];
			case 244:
				return set[45];
			case 247:
				return set[46];
			case 248:
			case 249:
			case 350:
				return set[47];
			case 253:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 255:
			case 297:
			case 312:
				return set[48];
			case 258:
			case 259:
			case 279:
			case 280:
			case 294:
			case 295:
			case 309:
			case 310:
				return set[49];
			case 260:
			case 351:
			case 354:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 261:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 262:
				return set[50];
			case 263:
			case 282:
				return set[51];
			case 264:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 267:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 271:
			case 272:
				return set[52];
			case 273:
			case 278:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 275:
			case 276:
				return set[53];
			case 281:
				return set[54];
			case 283:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 284:
			case 285:
				return set[55];
			case 287:
			case 288:
				return set[56];
			case 299:
			case 300:
				return set[57];
			case 301:
				return set[58];
			case 303:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 306:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 314:
				return set[59];
			case 315:
			case 319:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 316:
				return set[60];
			case 324:
			case 325:
				return set[61];
			case 328:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 330:
			case 331:
				return set[62];
			case 333:
			case 334:
				return set[63];
			case 335:
			case 616:
			case 617:
			case 619:
			case 660:
			case 673:
			case 674:
			case 676:
			case 685:
			case 686:
			case 688:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 336:
			case 338:
				return set[64];
			case 340:
			case 346:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 343:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 347:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 348:
			case 349:
			case 405:
			case 406:
				return set[65];
			case 352:
			case 353:
			case 355:
			case 356:
				return set[66];
			case 358:
				return set[67];
			case 364:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 366:
			case 367:
			case 375:
			case 376:
				return set[68];
			case 368:
			case 377:
				return set[69];
			case 369:
				return set[70];
			case 370:
			case 373:
				return set[71];
			case 371:
			case 372:
			case 666:
			case 667:
				return set[72];
			case 383:
			case 385:
			case 386:
			case 544:
			case 604:
				return set[73];
			case 387:
			case 388:
				return set[74];
			case 389:
			case 390:
				return set[75];
			case 391:
			case 395:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 396:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 399:
			case 400:
				return set[76];
			case 409:
				return set[77];
			case 411:
			case 424:
				return set[78];
			case 412:
			case 425:
				return set[79];
			case 413:
			case 414:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 419:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 421:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 422:
				return set[80];
			case 423:
				return set[81];
			case 435:
				return set[82];
			case 441:
				return set[83];
			case 443:
			case 444:
			case 512:
			case 513:
			case 655:
			case 656:
				return set[84];
			case 446:
			case 447:
			case 448:
			case 453:
			case 454:
			case 515:
			case 658:
			case 681:
			case 690:
				return set[85];
			case 449:
			case 455:
			case 464:
				return set[86];
			case 450:
			case 451:
			case 456:
			case 457:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 458:
			case 460:
			case 465:
				return set[87];
			case 461:
			case 463:
				return set[88];
			case 470:
			case 489:
			case 490:
			case 546:
			case 634:
			case 646:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 471:
			case 472:
			case 550:
			case 551:
				return set[89];
			case 486:
			case 487:
			case 494:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 492:
			case 493:
				return set[90];
			case 497:
			case 498:
				return set[91];
			case 499:
			case 500:
			case 559:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 504:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 516:
			case 517:
			case 529:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 522:
			case 523:
				return set[92];
			case 524:
			case 525:
				return set[93];
			case 526:
			case 527:
			case 538:
				return set[94];
			case 532:
			case 533:
				return set[95];
			case 534:
			case 535:
			case 670:
				return set[96];
			case 536:
				return set[97];
			case 537:
				return set[98];
			case 539:
			case 549:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 555:
			case 556:
				return set[99];
			case 557:
				return set[100];
			case 558:
			case 594:
				return set[101];
			case 561:
			case 562:
			case 563:
			case 586:
				return set[102];
			case 564:
			case 568:
			case 578:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 570:
				return set[103];
			case 573:
				return set[104];
			case 574:
				return set[105];
			case 589:
			case 590:
			case 592:
			case 665:
			case 668:
				return set[106];
			case 593:
				return set[107];
			case 599:
			case 601:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 600:
				return set[108];
			case 602:
				return set[109];
			case 608:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 612:
			case 613:
				return set[110];
			case 614:
			case 624:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 625:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 626:
				return set[111];
			case 629:
			case 630:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 631:
			case 640:
			case 702:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 632:
				return set[112];
			case 644:
			case 645:
				return set[113];
			case 652:
				return set[114];
			case 659:
			case 661:
				return set[115];
			case 662:
			case 669:
				return set[116];
			case 663:
			case 664:
				return set[117];
			case 671:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 677:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 680:
			case 689:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 694:
				return set[118];
			case 695:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 696:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 697:
			case 698:
				return set[119];
			case 703:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 704:
				return set[120];
			case 706:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 707:
				return set[121];
			case 708:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 709:
				return set[122];
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
					goto case 706;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 696;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 396;
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
					currentState = 692;
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
					goto case 396;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[123].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 516;
					} else {
						if (la.kind == 103) {
							currentState = 505;
							break;
						} else {
							if (la.kind == 115) {
								goto case 486;
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
				goto case 185;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 689;
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
					currentState = 684;
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
					goto case 396;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[124].Get(la.kind)) {
					currentState = 683;
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
					goto case 516;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 504;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 486;
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
					currentState = 477;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 467;
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
					currentState = 443;
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
						if (set[125].Get(la.kind)) {
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
				goto case 82;
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
					currentState = 441;
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
					goto case 437;
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
				if (set[126].Get(la.kind)) {
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
				if (set[127].Get(la.kind)) {
					currentState = 60;
					break;
				} else {
					if (set[33].Get(la.kind)) {
						stateStack.Push(139);
						goto case 153;
					} else {
						if (la.kind == 220) {
							currentState = 135;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(62);
								goto case 67;
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
				stateStack.Push(65);
				goto case 56;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 22) {
					currentState = 64;
					break;
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
				if (la == null) { currentState = 67; break; }
				Expect(162, la); // "New"
				currentState = 68;
				break;
			}
			case 68: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 69;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(127);
					goto case 37;
				} else {
					if (la.kind == 233) {
						PushContext(Context.ObjectInitializer, la, t);
						goto case 72;
					} else {
						goto case 70;
					}
				}
			}
			case 70: {
				Error(la);
				goto case 71;
			}
			case 71: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				Expect(233, la); // "With"
				currentState = 73;
				break;
			}
			case 73: {
				stateStack.Push(74);
				goto case 75;
			}
			case 74: {
				PopContext();
				goto case 71;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				Expect(35, la); // "{"
				currentState = 76;
				break;
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				if (la.kind == 147) {
					currentState = 77;
					break;
				} else {
					goto case 77;
				}
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(26, la); // "."
				currentState = 78;
				break;
			}
			case 78: {
				stateStack.Push(79);
				goto case 82;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				Expect(20, la); // "="
				currentState = 80;
				break;
			}
			case 80: {
				stateStack.Push(81);
				goto case 56;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (la.kind == 22) {
					currentState = 76;
					break;
				} else {
					goto case 66;
				}
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				if (la.kind == 2) {
					goto case 126;
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
								goto case 125;
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
												goto case 124;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 123;
													} else {
														if (la.kind == 65) {
															goto case 122;
														} else {
															if (la.kind == 66) {
																goto case 121;
															} else {
																if (la.kind == 67) {
																	goto case 120;
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
																				goto case 119;
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
																																		goto case 118;
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
																																					goto case 117;
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
																																																goto case 116;
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
																																																						goto case 115;
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
																																																									goto case 114;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 113;
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
																																																																		goto case 112;
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
																																																																							goto case 111;
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
																																																																										goto case 110;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 109;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 108;
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
																																																																																			goto case 107;
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
																																																																																									goto case 106;
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
																																																																																													goto case 105;
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
																																																																																																goto case 104;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 103;
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
																																																																																																																goto case 102;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 101;
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
																																																																																																																								goto case 100;
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
																																																																																																																														goto case 99;
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
																																																																																																																																						goto case 98;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 97;
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
																																																																																																																																																			goto case 96;
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
																																																																																																																																																									goto case 95;
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
																																																																																																																																																												goto case 94;
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
																																																																																																																																																															goto case 93;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 92;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 91;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 90;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 89;
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
																																																																																																																																																																								goto case 88;
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
																																																																																																																																																																													goto case 87;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 86;
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
																																																																																																																																																																																				goto case 85;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 84;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 83;
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
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						PushContext(Context.CollectionInitializer, la, t);
						goto case 131;
					} else {
						if (la.kind == 233) {
							PushContext(Context.ObjectInitializer, la, t);
							goto case 128;
						} else {
							goto case 70;
						}
					}
				} else {
					goto case 71;
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(233, la); // "With"
				currentState = 129;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 75;
			}
			case 130: {
				PopContext();
				goto case 71;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				Expect(126, la); // "From"
				currentState = 132;
				break;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 35) {
					stateStack.Push(133);
					goto case 63;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						Error(la);
						goto case 133;
					}
				}
			}
			case 133: {
				PopContext();
				goto case 71;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				currentState = 133;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 59;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				Expect(144, la); // "Is"
				currentState = 137;
				break;
			}
			case 137: {
				PushContext(Context.Type, la, t);
				stateStack.Push(138);
				goto case 37;
			}
			case 138: {
				PopContext();
				goto case 62;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(139);
					goto case 140;
				} else {
					goto case 62;
				}
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 37) {
					currentState = 145;
					break;
				} else {
					if (set[128].Get(la.kind)) {
						currentState = 141;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 141: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 142;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				if (la.kind == 10) {
					currentState = 143;
					break;
				} else {
					goto case 143;
				}
			}
			case 143: {
				stateStack.Push(144);
				goto case 82;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 145: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 146;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 169) {
					currentState = 148;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						if (set[22].Get(la.kind)) {
							stateStack.Push(147);
							goto case 48;
						} else {
							goto case 147;
						}
					} else {
						Error(la);
						goto case 147;
					}
				}
			}
			case 147: {
				PopContext();
				goto case 45;
			}
			case 148: {
				PushContext(Context.Type, la, t);
				stateStack.Push(149);
				goto case 37;
			}
			case 149: {
				PopContext();
				goto case 150;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 22) {
					currentState = 151;
					break;
				} else {
					goto case 147;
				}
			}
			case 151: {
				PushContext(Context.Type, la, t);
				stateStack.Push(152);
				goto case 37;
			}
			case 152: {
				PopContext();
				goto case 150;
			}
			case 153: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 154;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (set[129].Get(la.kind)) {
					currentState = 155;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 436;
						break;
					} else {
						if (set[130].Get(la.kind)) {
							currentState = 155;
							break;
						} else {
							if (set[125].Get(la.kind)) {
								currentState = 155;
								break;
							} else {
								if (set[128].Get(la.kind)) {
									currentState = 432;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 429;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 426;
											break;
										} else {
											if (set[77].Get(la.kind)) {
												stateStack.Push(155);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 409;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(155);
													goto case 230;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(155);
														PushContext(Context.Query, la, t);
														goto case 170;
													} else {
														if (set[35].Get(la.kind)) {
															stateStack.Push(155);
															goto case 163;
														} else {
															if (la.kind == 135) {
																stateStack.Push(155);
																goto case 156;
															} else {
																Error(la);
																goto case 155;
															}
														}
													}
												}
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
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(135, la); // "If"
				currentState = 157;
				break;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(37, la); // "("
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 56;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				Expect(22, la); // ","
				currentState = 160;
				break;
			}
			case 160: {
				stateStack.Push(161);
				goto case 56;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 22) {
					currentState = 162;
					break;
				} else {
					goto case 45;
				}
			}
			case 162: {
				stateStack.Push(45);
				goto case 56;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (set[131].Get(la.kind)) {
					currentState = 169;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 164;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(37, la); // "("
				currentState = 165;
				break;
			}
			case 165: {
				stateStack.Push(166);
				goto case 56;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(22, la); // ","
				currentState = 167;
				break;
			}
			case 167: {
				PushContext(Context.Type, la, t);
				stateStack.Push(168);
				goto case 37;
			}
			case 168: {
				PopContext();
				goto case 45;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				Expect(37, la); // "("
				currentState = 162;
				break;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 126) {
					stateStack.Push(171);
					goto case 229;
				} else {
					if (la.kind == 58) {
						stateStack.Push(171);
						goto case 228;
					} else {
						Error(la);
						goto case 171;
					}
				}
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(171);
					goto case 172;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				if (la.kind == 126) {
					currentState = 226;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 222;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 220;
							break;
						} else {
							if (la.kind == 107) {
								goto case 114;
							} else {
								if (la.kind == 230) {
									currentState = 56;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 216;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 214;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 212;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 186;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 173;
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
			case 173: {
				stateStack.Push(174);
				goto case 179;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				Expect(171, la); // "On"
				currentState = 175;
				break;
			}
			case 175: {
				stateStack.Push(176);
				goto case 56;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(116, la); // "Equals"
				currentState = 177;
				break;
			}
			case 177: {
				stateStack.Push(178);
				goto case 56;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 22) {
					currentState = 175;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 179: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(180);
				goto case 185;
			}
			case 180: {
				PopContext();
				goto case 181;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				if (la.kind == 63) {
					currentState = 183;
					break;
				} else {
					goto case 182;
				}
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				Expect(138, la); // "In"
				currentState = 56;
				break;
			}
			case 183: {
				PushContext(Context.Type, la, t);
				stateStack.Push(184);
				goto case 37;
			}
			case 184: {
				PopContext();
				goto case 182;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (set[116].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 116;
					} else {
						goto case 6;
					}
				}
			}
			case 186: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 187;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 146) {
					goto case 204;
				} else {
					if (set[38].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 189;
							break;
						} else {
							if (set[38].Get(la.kind)) {
								goto case 202;
							} else {
								Error(la);
								goto case 188;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(70, la); // "By"
				currentState = 189;
				break;
			}
			case 189: {
				stateStack.Push(190);
				goto case 193;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 22) {
					currentState = 189;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 191;
					break;
				}
			}
			case 191: {
				stateStack.Push(192);
				goto case 193;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (la.kind == 22) {
					currentState = 191;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 193: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 194;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(195);
					goto case 185;
				} else {
					goto case 56;
				}
			}
			case 195: {
				PopContext();
				nextTokenIsPotentialStartOfExpression = true;
				goto case 196;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (set[132].Get(la.kind)) {
					if (la.kind == 63) {
						currentState = 199;
						break;
					} else {
						if (la.kind == 20) {
							goto case 198;
						} else {
							if (set[39].Get(la.kind)) {
								currentState = endOfStatementTerminatorAndBlock; /* leave this block */
									InformToken(t); /* process Identifier again*/
									/* for processing current token (la): go to the position after processing End */
									goto switchlbl;

							} else {
								Error(la);
								goto case 56;
							}
						}
					}
				} else {
					goto case 56;
				}
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				currentState = 56;
				break;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				currentState = 56;
				break;
			}
			case 199: {
				PushContext(Context.Type, la, t);
				stateStack.Push(200);
				goto case 37;
			}
			case 200: {
				PopContext();
				goto case 201;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 202: {
				stateStack.Push(203);
				goto case 193;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 22) {
					currentState = 202;
					break;
				} else {
					goto case 188;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 211;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 209;
						break;
					} else {
						if (la.kind == 146) {
							goto case 204;
						} else {
							Error(la);
							goto case 205;
						}
					}
				} else {
					goto case 206;
				}
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				Expect(143, la); // "Into"
				currentState = 207;
				break;
			}
			case 207: {
				stateStack.Push(208);
				goto case 193;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 22) {
					currentState = 207;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 211;
			}
			case 210: {
				stateStack.Push(205);
				goto case 206;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(146, la); // "Join"
				currentState = 173;
				break;
			}
			case 212: {
				stateStack.Push(213);
				goto case 193;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 215;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(70, la); // "By"
				currentState = 217;
				break;
			}
			case 217: {
				stateStack.Push(218);
				goto case 56;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (la.kind == 64) {
					currentState = 219;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 219;
						break;
					} else {
						Error(la);
						goto case 219;
					}
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 22) {
					currentState = 217;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 220: {
				stateStack.Push(221);
				goto case 193;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 22) {
					currentState = 220;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 222: {
				stateStack.Push(223);
				goto case 179;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(223);
					goto case 172;
				} else {
					Expect(143, la); // "Into"
					currentState = 224;
					break;
				}
			}
			case 224: {
				stateStack.Push(225);
				goto case 193;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 22) {
					currentState = 224;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				stateStack.Push(227);
				goto case 179;
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
				if (la == null) { currentState = 228; break; }
				Expect(58, la); // "Aggregate"
				currentState = 222;
				break;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				Expect(126, la); // "From"
				currentState = 226;
				break;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 210) {
					currentState = 401;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 231;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				Expect(37, la); // "("
				currentState = 232;
				break;
			}
			case 232: {
				SetIdentifierExpected(la);
				goto case 233;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(234);
					goto case 383;
				} else {
					goto case 234;
				}
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(38, la); // ")"
				currentState = 235;
				break;
			}
			case 235: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 236;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 381;
							break;
						} else {
							goto case 237;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 237: {
				stateStack.Push(238);
				goto case 240;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				Expect(113, la); // "End"
				currentState = 239;
				break;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 240: {
				PushContext(Context.Body, la, t);
				goto case 241;
			}
			case 241: {
				stateStack.Push(242);
				goto case 23;
			}
			case 242: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (set[133].Get(la.kind)) {
					if (set[65].Get(la.kind)) {
						if (set[47].Get(la.kind)) {
							stateStack.Push(241);
							goto case 248;
						} else {
							goto case 241;
						}
					} else {
						if (la.kind == 113) {
							currentState = 246;
							break;
						} else {
							goto case 245;
						}
					}
				} else {
					goto case 244;
				}
			}
			case 244: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 245: {
				Error(la);
				goto case 242;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 241;
				} else {
					if (set[46].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 245;
					}
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				currentState = 242;
				break;
			}
			case 248: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 249;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 365;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 361;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 359;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 357;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 339;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 324;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 320;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 314;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 287;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 283;
																break;
															} else {
																goto case 283;
															}
														} else {
															if (la.kind == 194) {
																currentState = 281;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 279;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 266;
																break;
															} else {
																if (set[134].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 263;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 262;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 261;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 94;
																				} else {
																					if (la.kind == 195) {
																						currentState = 258;
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
																		currentState = 256;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 254;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 250;
																				break;
																			} else {
																				if (set[135].Get(la.kind)) {
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
			case 250: {
				stateStack.Push(251);
				goto case 56;
			}
			case 251: {
				stateStack.Push(252);
				goto case 240;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				Expect(113, la); // "End"
				currentState = 253;
				break;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 254: {
				stateStack.Push(255);
				goto case 56;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 22) {
					currentState = 254;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 256: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 257;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 258: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 259;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(260);
					goto case 56;
				} else {
					goto case 260;
				}
			}
			case 260: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (la.kind == 108) {
					goto case 113;
				} else {
					if (la.kind == 124) {
						goto case 110;
					} else {
						if (la.kind == 231) {
							goto case 84;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (la.kind == 108) {
					goto case 113;
				} else {
					if (la.kind == 124) {
						goto case 110;
					} else {
						if (la.kind == 231) {
							goto case 84;
						} else {
							if (la.kind == 197) {
								goto case 96;
							} else {
								if (la.kind == 210) {
									goto case 92;
								} else {
									if (la.kind == 127) {
										goto case 108;
									} else {
										if (la.kind == 186) {
											goto case 97;
										} else {
											if (la.kind == 218) {
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
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (set[6].Get(la.kind)) {
					goto case 265;
				} else {
					if (la.kind == 5) {
						goto case 264;
					} else {
						goto case 6;
					}
				}
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 266: {
				stateStack.Push(267);
				goto case 240;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (la.kind == 75) {
					currentState = 271;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 270;
						break;
					} else {
						goto case 268;
					}
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(113, la); // "End"
				currentState = 269;
				break;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 270: {
				stateStack.Push(268);
				goto case 240;
			}
			case 271: {
				SetIdentifierExpected(la);
				goto case 272;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(275);
					goto case 185;
				} else {
					goto case 273;
				}
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 229) {
					currentState = 274;
					break;
				} else {
					goto case 266;
				}
			}
			case 274: {
				stateStack.Push(266);
				goto case 56;
			}
			case 275: {
				PopContext();
				goto case 276;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (la.kind == 63) {
					currentState = 277;
					break;
				} else {
					goto case 273;
				}
			}
			case 277: {
				PushContext(Context.Type, la, t);
				stateStack.Push(278);
				goto case 37;
			}
			case 278: {
				PopContext();
				goto case 273;
			}
			case 279: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 280;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 163) {
					goto case 101;
				} else {
					goto case 282;
				}
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (la.kind == 5) {
					goto case 264;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 265;
					} else {
						goto case 6;
					}
				}
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				Expect(118, la); // "Error"
				currentState = 284;
				break;
			}
			case 284: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 285;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 282;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 286;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 287: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 288;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (set[33].Get(la.kind)) {
					stateStack.Push(304);
					goto case 298;
				} else {
					if (la.kind == 110) {
						currentState = 289;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 289: {
				stateStack.Push(290);
				goto case 298;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(138, la); // "In"
				currentState = 291;
				break;
			}
			case 291: {
				stateStack.Push(292);
				goto case 56;
			}
			case 292: {
				stateStack.Push(293);
				goto case 240;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				Expect(163, la); // "Next"
				currentState = 294;
				break;
			}
			case 294: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 295;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (set[23].Get(la.kind)) {
					goto case 296;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 296: {
				stateStack.Push(297);
				goto case 56;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.kind == 22) {
					currentState = 296;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 298: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(299);
				goto case 153;
			}
			case 299: {
				PopContext();
				goto case 300;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 33) {
					currentState = 301;
					break;
				} else {
					goto case 301;
				}
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(301);
					goto case 140;
				} else {
					if (la.kind == 63) {
						currentState = 302;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 302: {
				PushContext(Context.Type, la, t);
				stateStack.Push(303);
				goto case 37;
			}
			case 303: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(20, la); // "="
				currentState = 305;
				break;
			}
			case 305: {
				stateStack.Push(306);
				goto case 56;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (la.kind == 205) {
					currentState = 313;
					break;
				} else {
					goto case 307;
				}
			}
			case 307: {
				stateStack.Push(308);
				goto case 240;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				Expect(163, la); // "Next"
				currentState = 309;
				break;
			}
			case 309: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 310;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (set[23].Get(la.kind)) {
					goto case 311;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 311: {
				stateStack.Push(312);
				goto case 56;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 22) {
					currentState = 311;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 313: {
				stateStack.Push(307);
				goto case 56;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 317;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(315);
						goto case 240;
					} else {
						goto case 6;
					}
				}
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(152, la); // "Loop"
				currentState = 316;
				break;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 317: {
				stateStack.Push(318);
				goto case 56;
			}
			case 318: {
				stateStack.Push(319);
				goto case 240;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 320: {
				stateStack.Push(321);
				goto case 56;
			}
			case 321: {
				stateStack.Push(322);
				goto case 240;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				Expect(113, la); // "End"
				currentState = 323;
				break;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 324: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 325;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 74) {
					currentState = 326;
					break;
				} else {
					goto case 326;
				}
			}
			case 326: {
				stateStack.Push(327);
				goto case 56;
			}
			case 327: {
				stateStack.Push(328);
				goto case 23;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 74) {
					currentState = 330;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 329;
					break;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 330: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 331;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (la.kind == 111) {
					currentState = 332;
					break;
				} else {
					if (set[63].Get(la.kind)) {
						goto case 333;
					} else {
						Error(la);
						goto case 332;
					}
				}
			}
			case 332: {
				stateStack.Push(328);
				goto case 240;
			}
			case 333: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 334;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (set[136].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 336;
						break;
					} else {
						goto case 336;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(335);
						goto case 56;
					} else {
						Error(la);
						goto case 335;
					}
				}
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 22) {
					currentState = 333;
					break;
				} else {
					goto case 332;
				}
			}
			case 336: {
				stateStack.Push(337);
				goto case 338;
			}
			case 337: {
				stateStack.Push(335);
				goto case 59;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
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
			case 339: {
				stateStack.Push(340);
				goto case 56;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 214) {
					currentState = 348;
					break;
				} else {
					goto case 341;
				}
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 342;
				} else {
					goto case 6;
				}
			}
			case 342: {
				stateStack.Push(343);
				goto case 240;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 347;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 345;
							break;
						} else {
							Error(la);
							goto case 342;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 344;
					break;
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 345: {
				stateStack.Push(346);
				goto case 56;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 214) {
					currentState = 342;
					break;
				} else {
					goto case 342;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 135) {
					currentState = 345;
					break;
				} else {
					goto case 342;
				}
			}
			case 348: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 349;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (set[47].Get(la.kind)) {
					goto case 350;
				} else {
					goto case 341;
				}
			}
			case 350: {
				stateStack.Push(351);
				goto case 248;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 21) {
					currentState = 355;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 352;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 352: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 353;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (set[47].Get(la.kind)) {
					stateStack.Push(354);
					goto case 248;
				} else {
					goto case 354;
				}
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 21) {
					currentState = 352;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 355: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 356;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (set[47].Get(la.kind)) {
					goto case 350;
				} else {
					goto case 351;
				}
			}
			case 357: {
				stateStack.Push(358);
				goto case 82;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 359: {
				stateStack.Push(360);
				goto case 56;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 361: {
				stateStack.Push(362);
				goto case 56;
			}
			case 362: {
				stateStack.Push(363);
				goto case 240;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(113, la); // "End"
				currentState = 364;
				break;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 233) {
					goto case 83;
				} else {
					if (la.kind == 211) {
						goto case 91;
					} else {
						goto case 6;
					}
				}
			}
			case 365: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(366);
				goto case 185;
			}
			case 366: {
				PopContext();
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (la.kind == 33) {
					currentState = 368;
					break;
				} else {
					goto case 368;
				}
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				if (la.kind == 37) {
					currentState = 380;
					break;
				} else {
					goto case 369;
				}
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 22) {
					currentState = 374;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 371;
						break;
					} else {
						goto case 370;
					}
				}
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 20) {
					goto case 198;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 371: {
				PushContext(Context.Type, la, t);
				goto case 372;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 162) {
					stateStack.Push(373);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(373);
						goto case 37;
					} else {
						Error(la);
						goto case 373;
					}
				}
			}
			case 373: {
				PopContext();
				goto case 370;
			}
			case 374: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(375);
				goto case 185;
			}
			case 375: {
				PopContext();
				goto case 376;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 33) {
					currentState = 377;
					break;
				} else {
					goto case 377;
				}
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 37) {
					currentState = 378;
					break;
				} else {
					goto case 369;
				}
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 22) {
					currentState = 378;
					break;
				} else {
					goto case 379;
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				Expect(38, la); // ")"
				currentState = 369;
				break;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 22) {
					currentState = 380;
					break;
				} else {
					goto case 379;
				}
			}
			case 381: {
				PushContext(Context.Type, la, t);
				stateStack.Push(382);
				goto case 37;
			}
			case 382: {
				PopContext();
				goto case 237;
			}
			case 383: {
				stateStack.Push(384);
				PushContext(Context.Parameter, la, t);
				goto case 385;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 22) {
					currentState = 383;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 385: {
				SetIdentifierExpected(la);
				goto case 386;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (la.kind == 40) {
					stateStack.Push(385);
					goto case 396;
				} else {
					goto case 387;
				}
			}
			case 387: {
				SetIdentifierExpected(la);
				goto case 388;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (set[137].Get(la.kind)) {
					currentState = 387;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(389);
					goto case 185;
				}
			}
			case 389: {
				PopContext();
				goto case 390;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 63) {
					currentState = 394;
					break;
				} else {
					goto case 391;
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 20) {
					currentState = 393;
					break;
				} else {
					goto case 392;
				}
			}
			case 392: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 393: {
				stateStack.Push(392);
				goto case 56;
			}
			case 394: {
				PushContext(Context.Type, la, t);
				stateStack.Push(395);
				goto case 37;
			}
			case 395: {
				PopContext();
				goto case 391;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(40, la); // "<"
				currentState = 397;
				break;
			}
			case 397: {
				PushContext(Context.Attribute, la, t);
				goto case 398;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (set[138].Get(la.kind)) {
					currentState = 398;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 399;
					break;
				}
			}
			case 399: {
				PopContext();
				goto case 400;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(37, la); // "("
				currentState = 402;
				break;
			}
			case 402: {
				SetIdentifierExpected(la);
				goto case 403;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(404);
					goto case 383;
				} else {
					goto case 404;
				}
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				Expect(38, la); // ")"
				currentState = 405;
				break;
			}
			case 405: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 406;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (set[47].Get(la.kind)) {
					goto case 248;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(407);
						goto case 240;
					} else {
						goto case 6;
					}
				}
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				Expect(113, la); // "End"
				currentState = 408;
				break;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 422;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(411);
						goto case 413;
					} else {
						Error(la);
						goto case 410;
					}
				}
			}
			case 410: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 17) {
					currentState = 412;
					break;
				} else {
					goto case 410;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 16) {
					currentState = 411;
					break;
				} else {
					goto case 411;
				}
			}
			case 413: {
				PushContext(Context.Xml, la, t);
				goto case 414;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 415;
				break;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (set[139].Get(la.kind)) {
					if (set[140].Get(la.kind)) {
						currentState = 415;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(415);
							goto case 419;
						} else {
							Error(la);
							goto case 415;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 416;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 417;
							break;
						} else {
							Error(la);
							goto case 416;
						}
					}
				}
			}
			case 416: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (set[141].Get(la.kind)) {
					if (set[142].Get(la.kind)) {
						currentState = 417;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(417);
							goto case 419;
						} else {
							if (la.kind == 10) {
								stateStack.Push(417);
								goto case 413;
							} else {
								Error(la);
								goto case 417;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 418;
					break;
				}
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (set[143].Get(la.kind)) {
					if (set[144].Get(la.kind)) {
						currentState = 418;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(418);
							goto case 419;
						} else {
							Error(la);
							goto case 418;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 416;
					break;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 420;
				break;
			}
			case 420: {
				stateStack.Push(421);
				goto case 56;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (la.kind == 16) {
					currentState = 423;
					break;
				} else {
					goto case 423;
				}
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 422;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(424);
						goto case 413;
					} else {
						goto case 410;
					}
				}
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (la.kind == 17) {
					currentState = 425;
					break;
				} else {
					goto case 410;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 16) {
					currentState = 424;
					break;
				} else {
					goto case 424;
				}
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				Expect(37, la); // "("
				currentState = 427;
				break;
			}
			case 427: {
				readXmlIdentifier = true;
				stateStack.Push(428);
				goto case 185;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				Expect(38, la); // ")"
				currentState = 155;
				break;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				Expect(37, la); // "("
				currentState = 430;
				break;
			}
			case 430: {
				PushContext(Context.Type, la, t);
				stateStack.Push(431);
				goto case 37;
			}
			case 431: {
				PopContext();
				goto case 428;
			}
			case 432: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 433;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 10) {
					currentState = 434;
					break;
				} else {
					goto case 434;
				}
			}
			case 434: {
				stateStack.Push(435);
				goto case 82;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 11) {
					currentState = 155;
					break;
				} else {
					goto case 155;
				}
			}
			case 436: {
				stateStack.Push(428);
				goto case 56;
			}
			case 437: {
				stateStack.Push(438);
				goto case 56;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 22) {
					currentState = 439;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 439: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 440;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (set[23].Get(la.kind)) {
					goto case 437;
				} else {
					goto case 438;
				}
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(442);
					goto case 37;
				} else {
					goto case 442;
				}
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 22) {
					currentState = 441;
					break;
				} else {
					goto case 45;
				}
			}
			case 443: {
				SetIdentifierExpected(la);
				goto case 444;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (set[145].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 446;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(445);
							goto case 383;
						} else {
							Error(la);
							goto case 445;
						}
					}
				} else {
					goto case 445;
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 446: {
				stateStack.Push(445);
				goto case 447;
			}
			case 447: {
				SetIdentifierExpected(la);
				goto case 448;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 449;
					break;
				} else {
					goto case 449;
				}
			}
			case 449: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(450);
				goto case 464;
			}
			case 450: {
				PopContext();
				goto case 451;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 63) {
					currentState = 465;
					break;
				} else {
					goto case 452;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 22) {
					currentState = 453;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 453: {
				SetIdentifierExpected(la);
				goto case 454;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 455;
					break;
				} else {
					goto case 455;
				}
			}
			case 455: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(456);
				goto case 464;
			}
			case 456: {
				PopContext();
				goto case 457;
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 63) {
					currentState = 458;
					break;
				} else {
					goto case 452;
				}
			}
			case 458: {
				PushContext(Context.Type, la, t);
				stateStack.Push(459);
				goto case 460;
			}
			case 459: {
				PopContext();
				goto case 452;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (set[88].Get(la.kind)) {
					goto case 463;
				} else {
					if (la.kind == 35) {
						currentState = 461;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 461: {
				stateStack.Push(462);
				goto case 463;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (la.kind == 22) {
					currentState = 461;
					break;
				} else {
					goto case 66;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 162) {
						goto case 102;
					} else {
						if (la.kind == 84) {
							goto case 118;
						} else {
							if (la.kind == 209) {
								goto case 93;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (la.kind == 2) {
					goto case 126;
				} else {
					if (la.kind == 62) {
						goto case 124;
					} else {
						if (la.kind == 64) {
							goto case 123;
						} else {
							if (la.kind == 65) {
								goto case 122;
							} else {
								if (la.kind == 66) {
									goto case 121;
								} else {
									if (la.kind == 67) {
										goto case 120;
									} else {
										if (la.kind == 70) {
											goto case 119;
										} else {
											if (la.kind == 87) {
												goto case 117;
											} else {
												if (la.kind == 104) {
													goto case 115;
												} else {
													if (la.kind == 107) {
														goto case 114;
													} else {
														if (la.kind == 116) {
															goto case 112;
														} else {
															if (la.kind == 121) {
																goto case 111;
															} else {
																if (la.kind == 133) {
																	goto case 107;
																} else {
																	if (la.kind == 139) {
																		goto case 106;
																	} else {
																		if (la.kind == 143) {
																			goto case 105;
																		} else {
																			if (la.kind == 146) {
																				goto case 104;
																			} else {
																				if (la.kind == 147) {
																					goto case 103;
																				} else {
																					if (la.kind == 170) {
																						goto case 100;
																					} else {
																						if (la.kind == 176) {
																							goto case 99;
																						} else {
																							if (la.kind == 184) {
																								goto case 98;
																							} else {
																								if (la.kind == 203) {
																									goto case 95;
																								} else {
																									if (la.kind == 212) {
																										goto case 90;
																									} else {
																										if (la.kind == 213) {
																											goto case 89;
																										} else {
																											if (la.kind == 223) {
																												goto case 87;
																											} else {
																												if (la.kind == 224) {
																													goto case 86;
																												} else {
																													if (la.kind == 230) {
																														goto case 85;
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
			case 465: {
				PushContext(Context.Type, la, t);
				stateStack.Push(466);
				goto case 460;
			}
			case 466: {
				PopContext();
				goto case 452;
			}
			case 467: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(468);
				goto case 185;
			}
			case 468: {
				PopContext();
				goto case 469;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 37) {
					currentState = 474;
					break;
				} else {
					goto case 470;
				}
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				if (la.kind == 63) {
					currentState = 471;
					break;
				} else {
					goto case 23;
				}
			}
			case 471: {
				PushContext(Context.Type, la, t);
				goto case 472;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 40) {
					stateStack.Push(472);
					goto case 396;
				} else {
					stateStack.Push(473);
					goto case 37;
				}
			}
			case 473: {
				PopContext();
				goto case 23;
			}
			case 474: {
				SetIdentifierExpected(la);
				goto case 475;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(476);
					goto case 383;
				} else {
					goto case 476;
				}
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				Expect(38, la); // ")"
				currentState = 470;
				break;
			}
			case 477: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(478);
				goto case 185;
			}
			case 478: {
				PopContext();
				goto case 479;
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 484;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 481;
							break;
						} else {
							goto case 480;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 480: {
				Error(la);
				goto case 23;
			}
			case 481: {
				SetIdentifierExpected(la);
				goto case 482;
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(483);
					goto case 383;
				} else {
					goto case 483;
				}
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 484: {
				PushContext(Context.Type, la, t);
				stateStack.Push(485);
				goto case 37;
			}
			case 485: {
				PopContext();
				goto case 23;
			}
			case 486: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 487;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				Expect(115, la); // "Enum"
				currentState = 488;
				break;
			}
			case 488: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(489);
				goto case 185;
			}
			case 489: {
				PopContext();
				goto case 490;
			}
			case 490: {
				if (la == null) { currentState = 490; break; }
				if (la.kind == 63) {
					currentState = 502;
					break;
				} else {
					goto case 491;
				}
			}
			case 491: {
				stateStack.Push(492);
				goto case 23;
			}
			case 492: {
				SetIdentifierExpected(la);
				goto case 493;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (set[91].Get(la.kind)) {
					goto case 497;
				} else {
					Expect(113, la); // "End"
					currentState = 494;
					break;
				}
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				Expect(115, la); // "Enum"
				currentState = 495;
				break;
			}
			case 495: {
				stateStack.Push(496);
				goto case 23;
			}
			case 496: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 497: {
				SetIdentifierExpected(la);
				goto case 498;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (la.kind == 40) {
					stateStack.Push(497);
					goto case 396;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(499);
					goto case 185;
				}
			}
			case 499: {
				PopContext();
				goto case 500;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 20) {
					currentState = 501;
					break;
				} else {
					goto case 491;
				}
			}
			case 501: {
				stateStack.Push(491);
				goto case 56;
			}
			case 502: {
				PushContext(Context.Type, la, t);
				stateStack.Push(503);
				goto case 37;
			}
			case 503: {
				PopContext();
				goto case 491;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				Expect(103, la); // "Delegate"
				currentState = 505;
				break;
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 210) {
					currentState = 506;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 506;
						break;
					} else {
						Error(la);
						goto case 506;
					}
				}
			}
			case 506: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 507;
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				currentState = 508;
				break;
			}
			case 508: {
				PopContext();
				goto case 509;
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				if (la.kind == 37) {
					currentState = 512;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 510;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 510: {
				PushContext(Context.Type, la, t);
				stateStack.Push(511);
				goto case 37;
			}
			case 511: {
				PopContext();
				goto case 23;
			}
			case 512: {
				SetIdentifierExpected(la);
				goto case 513;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				if (set[145].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 515;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(514);
							goto case 383;
						} else {
							Error(la);
							goto case 514;
						}
					}
				} else {
					goto case 514;
				}
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				Expect(38, la); // ")"
				currentState = 509;
				break;
			}
			case 515: {
				stateStack.Push(514);
				goto case 447;
			}
			case 516: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 517;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 155) {
					currentState = 518;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 518;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 518;
							break;
						} else {
							Error(la);
							goto case 518;
						}
					}
				}
			}
			case 518: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(519);
				goto case 185;
			}
			case 519: {
				PopContext();
				goto case 520;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (la.kind == 37) {
					currentState = 680;
					break;
				} else {
					goto case 521;
				}
			}
			case 521: {
				stateStack.Push(522);
				goto case 23;
			}
			case 522: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 523;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 677;
				} else {
					goto case 524;
				}
			}
			case 524: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 525;
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 671;
				} else {
					goto case 526;
				}
			}
			case 526: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 527;
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (set[95].Get(la.kind)) {
					goto case 532;
				} else {
					isMissingModifier = false;
					goto case 528;
				}
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				Expect(113, la); // "End"
				currentState = 529;
				break;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (la.kind == 155) {
					currentState = 530;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 530;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 530;
							break;
						} else {
							Error(la);
							goto case 530;
						}
					}
				}
			}
			case 530: {
				stateStack.Push(531);
				goto case 23;
			}
			case 531: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 532: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 533;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (la.kind == 40) {
					stateStack.Push(532);
					goto case 396;
				} else {
					isMissingModifier = true;
					goto case 534;
				}
			}
			case 534: {
				SetIdentifierExpected(la);
				goto case 535;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				if (set[124].Get(la.kind)) {
					currentState = 670;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 536;
				}
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(526);
					goto case 516;
				} else {
					if (la.kind == 103) {
						stateStack.Push(526);
						goto case 504;
					} else {
						if (la.kind == 115) {
							stateStack.Push(526);
							goto case 486;
						} else {
							if (la.kind == 142) {
								stateStack.Push(526);
								goto case 9;
							} else {
								if (set[98].Get(la.kind)) {
									stateStack.Push(526);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 537;
								} else {
									Error(la);
									goto case 526;
								}
							}
						}
					}
				}
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (set[115].Get(la.kind)) {
					stateStack.Push(538);
					goto case 659;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(538);
						goto case 641;
					} else {
						if (la.kind == 101) {
							stateStack.Push(538);
							goto case 625;
						} else {
							if (la.kind == 119) {
								stateStack.Push(538);
								goto case 610;
							} else {
								if (la.kind == 98) {
									stateStack.Push(538);
									goto case 598;
								} else {
									if (la.kind == 186) {
										stateStack.Push(538);
										goto case 553;
									} else {
										if (la.kind == 172) {
											stateStack.Push(538);
											goto case 539;
										} else {
											Error(la);
											goto case 538;
										}
									}
								}
							}
						}
					}
				}
			}
			case 538: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				Expect(172, la); // "Operator"
				currentState = 540;
				break;
			}
			case 540: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 541;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				currentState = 542;
				break;
			}
			case 542: {
				PopContext();
				goto case 543;
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				Expect(37, la); // "("
				currentState = 544;
				break;
			}
			case 544: {
				stateStack.Push(545);
				goto case 383;
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(38, la); // ")"
				currentState = 546;
				break;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				if (la.kind == 63) {
					currentState = 550;
					break;
				} else {
					goto case 547;
				}
			}
			case 547: {
				stateStack.Push(548);
				goto case 240;
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				Expect(113, la); // "End"
				currentState = 549;
				break;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 550: {
				PushContext(Context.Type, la, t);
				goto case 551;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (la.kind == 40) {
					stateStack.Push(551);
					goto case 396;
				} else {
					stateStack.Push(552);
					goto case 37;
				}
			}
			case 552: {
				PopContext();
				goto case 547;
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				Expect(186, la); // "Property"
				currentState = 554;
				break;
			}
			case 554: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(555);
				goto case 185;
			}
			case 555: {
				PopContext();
				goto case 556;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (la.kind == 37) {
					currentState = 595;
					break;
				} else {
					goto case 557;
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 63) {
					currentState = 593;
					break;
				} else {
					goto case 558;
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 136) {
					currentState = 588;
					break;
				} else {
					goto case 559;
				}
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				if (la.kind == 20) {
					currentState = 587;
					break;
				} else {
					goto case 560;
				}
			}
			case 560: {
				stateStack.Push(561);
				goto case 23;
			}
			case 561: {
				PopContext();
				goto case 562;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				if (la.kind == 40) {
					stateStack.Push(562);
					goto case 396;
				} else {
					goto case 563;
				}
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (set[146].Get(la.kind)) {
					currentState = 586;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 564;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				if (la.kind == 128) {
					currentState = 565;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 565;
						break;
					} else {
						Error(la);
						goto case 565;
					}
				}
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				if (la.kind == 37) {
					currentState = 583;
					break;
				} else {
					goto case 566;
				}
			}
			case 566: {
				stateStack.Push(567);
				goto case 240;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				Expect(113, la); // "End"
				currentState = 568;
				break;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 128) {
					currentState = 569;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 569;
						break;
					} else {
						Error(la);
						goto case 569;
					}
				}
			}
			case 569: {
				stateStack.Push(570);
				goto case 23;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				if (set[104].Get(la.kind)) {
					goto case 573;
				} else {
					goto case 571;
				}
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				Expect(113, la); // "End"
				currentState = 572;
				break;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (la.kind == 40) {
					stateStack.Push(573);
					goto case 396;
				} else {
					goto case 574;
				}
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (set[146].Get(la.kind)) {
					currentState = 574;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 575;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 575;
							break;
						} else {
							Error(la);
							goto case 575;
						}
					}
				}
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				if (la.kind == 37) {
					currentState = 580;
					break;
				} else {
					goto case 576;
				}
			}
			case 576: {
				stateStack.Push(577);
				goto case 240;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				Expect(113, la); // "End"
				currentState = 578;
				break;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (la.kind == 128) {
					currentState = 579;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 579;
						break;
					} else {
						Error(la);
						goto case 579;
					}
				}
			}
			case 579: {
				stateStack.Push(571);
				goto case 23;
			}
			case 580: {
				SetIdentifierExpected(la);
				goto case 581;
			}
			case 581: {
				if (la == null) { currentState = 581; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(582);
					goto case 383;
				} else {
					goto case 582;
				}
			}
			case 582: {
				if (la == null) { currentState = 582; break; }
				Expect(38, la); // ")"
				currentState = 576;
				break;
			}
			case 583: {
				SetIdentifierExpected(la);
				goto case 584;
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(585);
					goto case 383;
				} else {
					goto case 585;
				}
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				Expect(38, la); // ")"
				currentState = 566;
				break;
			}
			case 586: {
				SetIdentifierExpected(la);
				goto case 563;
			}
			case 587: {
				stateStack.Push(560);
				goto case 56;
			}
			case 588: {
				PushContext(Context.Type, la, t);
				stateStack.Push(589);
				goto case 37;
			}
			case 589: {
				PopContext();
				goto case 590;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (la.kind == 22) {
					currentState = 591;
					break;
				} else {
					goto case 559;
				}
			}
			case 591: {
				PushContext(Context.Type, la, t);
				stateStack.Push(592);
				goto case 37;
			}
			case 592: {
				PopContext();
				goto case 590;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				if (la.kind == 40) {
					stateStack.Push(593);
					goto case 396;
				} else {
					if (la.kind == 162) {
						stateStack.Push(558);
						goto case 67;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(594);
							goto case 37;
						} else {
							Error(la);
							goto case 558;
						}
					}
				}
			}
			case 594: {
				PopContext();
				goto case 558;
			}
			case 595: {
				SetIdentifierExpected(la);
				goto case 596;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(597);
					goto case 383;
				} else {
					goto case 597;
				}
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(38, la); // ")"
				currentState = 557;
				break;
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				Expect(98, la); // "Custom"
				currentState = 599;
				break;
			}
			case 599: {
				stateStack.Push(600);
				goto case 610;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				if (set[109].Get(la.kind)) {
					goto case 602;
				} else {
					Expect(113, la); // "End"
					currentState = 601;
					break;
				}
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (la.kind == 40) {
					stateStack.Push(602);
					goto case 396;
				} else {
					if (la.kind == 56) {
						currentState = 603;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 603;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 603;
								break;
							} else {
								Error(la);
								goto case 603;
							}
						}
					}
				}
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(37, la); // "("
				currentState = 604;
				break;
			}
			case 604: {
				stateStack.Push(605);
				goto case 383;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				Expect(38, la); // ")"
				currentState = 606;
				break;
			}
			case 606: {
				stateStack.Push(607);
				goto case 240;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				Expect(113, la); // "End"
				currentState = 608;
				break;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (la.kind == 56) {
					currentState = 609;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 609;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 609;
							break;
						} else {
							Error(la);
							goto case 609;
						}
					}
				}
			}
			case 609: {
				stateStack.Push(600);
				goto case 23;
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				Expect(119, la); // "Event"
				currentState = 611;
				break;
			}
			case 611: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(612);
				goto case 185;
			}
			case 612: {
				PopContext();
				goto case 613;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				if (la.kind == 63) {
					currentState = 623;
					break;
				} else {
					if (set[147].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 620;
							break;
						} else {
							goto case 614;
						}
					} else {
						Error(la);
						goto case 614;
					}
				}
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (la.kind == 136) {
					currentState = 615;
					break;
				} else {
					goto case 23;
				}
			}
			case 615: {
				PushContext(Context.Type, la, t);
				stateStack.Push(616);
				goto case 37;
			}
			case 616: {
				PopContext();
				goto case 617;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (la.kind == 22) {
					currentState = 618;
					break;
				} else {
					goto case 23;
				}
			}
			case 618: {
				PushContext(Context.Type, la, t);
				stateStack.Push(619);
				goto case 37;
			}
			case 619: {
				PopContext();
				goto case 617;
			}
			case 620: {
				SetIdentifierExpected(la);
				goto case 621;
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(622);
					goto case 383;
				} else {
					goto case 622;
				}
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(38, la); // ")"
				currentState = 614;
				break;
			}
			case 623: {
				PushContext(Context.Type, la, t);
				stateStack.Push(624);
				goto case 37;
			}
			case 624: {
				PopContext();
				goto case 614;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				Expect(101, la); // "Declare"
				currentState = 626;
				break;
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 627;
					break;
				} else {
					goto case 627;
				}
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				if (la.kind == 210) {
					currentState = 628;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 628;
						break;
					} else {
						Error(la);
						goto case 628;
					}
				}
			}
			case 628: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(629);
				goto case 185;
			}
			case 629: {
				PopContext();
				goto case 630;
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				Expect(149, la); // "Lib"
				currentState = 631;
				break;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				Expect(3, la); // LiteralString
				currentState = 632;
				break;
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				if (la.kind == 59) {
					currentState = 640;
					break;
				} else {
					goto case 633;
				}
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				if (la.kind == 37) {
					currentState = 637;
					break;
				} else {
					goto case 634;
				}
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				if (la.kind == 63) {
					currentState = 635;
					break;
				} else {
					goto case 23;
				}
			}
			case 635: {
				PushContext(Context.Type, la, t);
				stateStack.Push(636);
				goto case 37;
			}
			case 636: {
				PopContext();
				goto case 23;
			}
			case 637: {
				SetIdentifierExpected(la);
				goto case 638;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(639);
					goto case 383;
				} else {
					goto case 639;
				}
			}
			case 639: {
				if (la == null) { currentState = 639; break; }
				Expect(38, la); // ")"
				currentState = 634;
				break;
			}
			case 640: {
				if (la == null) { currentState = 640; break; }
				Expect(3, la); // LiteralString
				currentState = 633;
				break;
			}
			case 641: {
				if (la == null) { currentState = 641; break; }
				if (la.kind == 210) {
					currentState = 642;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 642;
						break;
					} else {
						Error(la);
						goto case 642;
					}
				}
			}
			case 642: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 643;
			}
			case 643: {
				if (la == null) { currentState = 643; break; }
				currentState = 644;
				break;
			}
			case 644: {
				PopContext();
				goto case 645;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				if (la.kind == 37) {
					currentState = 655;
					break;
				} else {
					if (la.kind == 134 || la.kind == 136) {
						currentState = 652;
						break;
					} else {
						goto case 646;
					}
				}
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				if (la.kind == 63) {
					currentState = 650;
					break;
				} else {
					goto case 647;
				}
			}
			case 647: {
				stateStack.Push(648);
				goto case 240;
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				Expect(113, la); // "End"
				currentState = 649;
				break;
			}
			case 649: {
				if (la == null) { currentState = 649; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 480;
					}
				}
			}
			case 650: {
				PushContext(Context.Type, la, t);
				stateStack.Push(651);
				goto case 37;
			}
			case 651: {
				PopContext();
				goto case 647;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				if (la.kind == 153 || la.kind == 158 || la.kind == 159) {
					currentState = 654;
					break;
				} else {
					goto case 653;
				}
			}
			case 653: {
				stateStack.Push(646);
				goto case 37;
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				Expect(26, la); // "."
				currentState = 653;
				break;
			}
			case 655: {
				SetIdentifierExpected(la);
				goto case 656;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				if (set[145].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 658;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(657);
							goto case 383;
						} else {
							Error(la);
							goto case 657;
						}
					}
				} else {
					goto case 657;
				}
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				Expect(38, la); // ")"
				currentState = 645;
				break;
			}
			case 658: {
				stateStack.Push(657);
				goto case 447;
			}
			case 659: {
				stateStack.Push(660);
				SetIdentifierExpected(la);
				goto case 661;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				if (la.kind == 22) {
					currentState = 659;
					break;
				} else {
					goto case 23;
				}
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				if (la.kind == 88) {
					currentState = 662;
					break;
				} else {
					goto case 662;
				}
			}
			case 662: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(663);
				goto case 669;
			}
			case 663: {
				PopContext();
				goto case 664;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				if (la.kind == 63) {
					currentState = 666;
					break;
				} else {
					goto case 665;
				}
			}
			case 665: {
				if (la == null) { currentState = 665; break; }
				if (la.kind == 20) {
					goto case 198;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 666: {
				PushContext(Context.Type, la, t);
				goto case 667;
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				if (la.kind == 162) {
					stateStack.Push(668);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(668);
						goto case 37;
					} else {
						Error(la);
						goto case 668;
					}
				}
			}
			case 668: {
				PopContext();
				goto case 665;
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				if (set[130].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 125;
					} else {
						if (la.kind == 126) {
							goto case 109;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 670: {
				isMissingModifier = false;
				goto case 534;
			}
			case 671: {
				if (la == null) { currentState = 671; break; }
				Expect(136, la); // "Implements"
				currentState = 672;
				break;
			}
			case 672: {
				PushContext(Context.Type, la, t);
				stateStack.Push(673);
				goto case 37;
			}
			case 673: {
				PopContext();
				goto case 674;
			}
			case 674: {
				if (la == null) { currentState = 674; break; }
				if (la.kind == 22) {
					currentState = 675;
					break;
				} else {
					stateStack.Push(526);
					goto case 23;
				}
			}
			case 675: {
				PushContext(Context.Type, la, t);
				stateStack.Push(676);
				goto case 37;
			}
			case 676: {
				PopContext();
				goto case 674;
			}
			case 677: {
				if (la == null) { currentState = 677; break; }
				Expect(140, la); // "Inherits"
				currentState = 678;
				break;
			}
			case 678: {
				PushContext(Context.Type, la, t);
				stateStack.Push(679);
				goto case 37;
			}
			case 679: {
				PopContext();
				stateStack.Push(524);
				goto case 23;
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				Expect(169, la); // "Of"
				currentState = 681;
				break;
			}
			case 681: {
				stateStack.Push(682);
				goto case 447;
			}
			case 682: {
				if (la == null) { currentState = 682; break; }
				Expect(38, la); // ")"
				currentState = 521;
				break;
			}
			case 683: {
				isMissingModifier = false;
				goto case 28;
			}
			case 684: {
				PushContext(Context.Type, la, t);
				stateStack.Push(685);
				goto case 37;
			}
			case 685: {
				PopContext();
				goto case 686;
			}
			case 686: {
				if (la == null) { currentState = 686; break; }
				if (la.kind == 22) {
					currentState = 687;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 687: {
				PushContext(Context.Type, la, t);
				stateStack.Push(688);
				goto case 37;
			}
			case 688: {
				PopContext();
				goto case 686;
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				Expect(169, la); // "Of"
				currentState = 690;
				break;
			}
			case 690: {
				stateStack.Push(691);
				goto case 447;
			}
			case 691: {
				if (la == null) { currentState = 691; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 692: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 693;
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				if (set[46].Get(la.kind)) {
					currentState = 693;
					break;
				} else {
					PopContext();
					stateStack.Push(694);
					goto case 23;
				}
			}
			case 694: {
				if (la == null) { currentState = 694; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(694);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 695;
					break;
				}
			}
			case 695: {
				if (la == null) { currentState = 695; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				Expect(137, la); // "Imports"
				currentState = 697;
				break;
			}
			case 697: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 698;
			}
			case 698: {
				if (la == null) { currentState = 698; break; }
				if (set[148].Get(la.kind)) {
					currentState = 704;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 700;
						break;
					} else {
						Error(la);
						goto case 699;
					}
				}
			}
			case 699: {
				PopContext();
				goto case 23;
			}
			case 700: {
				stateStack.Push(701);
				goto case 185;
			}
			case 701: {
				if (la == null) { currentState = 701; break; }
				Expect(20, la); // "="
				currentState = 702;
				break;
			}
			case 702: {
				if (la == null) { currentState = 702; break; }
				Expect(3, la); // LiteralString
				currentState = 703;
				break;
			}
			case 703: {
				if (la == null) { currentState = 703; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 699;
				break;
			}
			case 704: {
				if (la == null) { currentState = 704; break; }
				if (la.kind == 37) {
					stateStack.Push(704);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 705;
						break;
					} else {
						goto case 699;
					}
				}
			}
			case 705: {
				stateStack.Push(699);
				goto case 37;
			}
			case 706: {
				if (la == null) { currentState = 706; break; }
				Expect(173, la); // "Option"
				currentState = 707;
				break;
			}
			case 707: {
				if (la == null) { currentState = 707; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 709;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 708;
						break;
					} else {
						goto case 480;
					}
				}
			}
			case 708: {
				if (la == null) { currentState = 708; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 480;
					}
				}
			}
			case 709: {
				if (la == null) { currentState = 709; break; }
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
		new BitArray(new int[] {-940564478, -1258291211, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, -1258291243, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
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