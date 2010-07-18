using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 20;
	const int endOfStatementTerminatorAndBlock = 219;
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
			case 34:
			case 220:
			case 508:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 146:
			case 152:
			case 159:
			case 197:
			case 201:
			case 239:
			case 340:
			case 352:
			case 401:
			case 471:
			case 486:
			case 564:
				return set[6];
			case 10:
			case 416:
			case 452:
			case 492:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 11:
			case 14:
			case 15:
			case 212:
			case 215:
			case 216:
			case 226:
			case 240:
			case 244:
			case 265:
			case 281:
			case 292:
			case 295:
			case 301:
			case 306:
			case 315:
			case 316:
			case 329:
			case 337:
			case 361:
			case 419:
			case 428:
			case 437:
			case 453:
			case 457:
			case 466:
			case 469:
			case 495:
			case 505:
			case 511:
			case 563:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 12:
				return set[7];
			case 13:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 16:
			case 326:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 17:
				return set[8];
			case 18:
			case 542:
			case 546:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 19:
			case 20:
			case 21:
			case 23:
			case 24:
			case 25:
			case 28:
			case 42:
			case 108:
			case 114:
			case 132:
			case 134:
			case 136:
			case 139:
			case 148:
			case 150:
			case 192:
			case 225:
			case 229:
			case 231:
			case 232:
			case 248:
			case 264:
			case 269:
			case 279:
			case 285:
			case 287:
			case 291:
			case 294:
			case 300:
			case 311:
			case 313:
			case 319:
			case 334:
			case 336:
			case 372:
			case 397:
			case 409:
			case 543:
				return set[9];
			case 22:
			case 26:
			case 35:
				return set[10];
			case 27:
			case 36:
			case 37:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 29:
			case 43:
			case 532:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 30:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 31:
			case 64:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 32:
				return set[11];
			case 33:
			case 45:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 38:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 39:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 40:
			case 44:
			case 95:
			case 121:
			case 332:
			case 407:
				return set[12];
			case 41:
			case 172:
			case 175:
			case 176:
			case 278:
			case 565:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 46:
			case 297:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 47:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 48:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 49:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 50:
			case 243:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 51:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 52:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 53:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 54:
			case 388:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 55:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 56:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 57:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 58:
			case 303:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 59:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 60:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 61:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 62:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 63:
			case 259:
			case 266:
			case 282:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 65:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 66:
			case 179:
			case 184:
			case 186:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 67:
			case 181:
			case 185:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 70:
			case 214:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 71:
			case 204:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 73:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 74:
			case 149:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 75:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 78:
			case 458:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 80:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 81:
			case 162:
			case 191:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 83:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 87:
			case 203:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 89:
				return set[13];
			case 90:
			case 169:
			case 170:
			case 221:
			case 377:
			case 378:
			case 393:
			case 394:
			case 395:
			case 412:
			case 413:
			case 424:
			case 425:
			case 446:
			case 447:
			case 501:
			case 502:
			case 550:
			case 552:
			case 556:
			case 557:
				return set[14];
			case 91:
				return set[15];
			case 92:
			case 116:
			case 125:
			case 141:
			case 157:
			case 174:
			case 252:
			case 276:
			case 347:
			case 349:
			case 351:
			case 360:
			case 374:
			case 404:
			case 410:
			case 418:
			case 475:
			case 481:
			case 494:
			case 510:
			case 545:
			case 569:
				return set[16];
			case 93:
			case 96:
				return set[17];
			case 94:
				return set[18];
			case 97:
			case 131:
			case 138:
			case 142:
			case 206:
			case 381:
			case 400:
			case 403:
			case 448:
			case 449:
			case 463:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 98:
			case 99:
			case 123:
			case 124:
				return set[19];
			case 100:
			case 209:
			case 357:
			case 384:
			case 402:
			case 422:
			case 451:
			case 465:
			case 479:
			case 498:
			case 514:
			case 555:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 101:
			case 102:
			case 105:
			case 106:
			case 110:
			case 111:
				return set[20];
			case 103:
				return set[21];
			case 104:
			case 133:
			case 140:
			case 335:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 107:
			case 109:
			case 113:
			case 126:
			case 135:
			case 356:
			case 358:
			case 363:
			case 371:
			case 521:
			case 529:
			case 537:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 112:
				return set[22];
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 117:
				return set[23];
			case 118:
				return set[24];
			case 119:
			case 120:
			case 405:
			case 406:
				return set[25];
			case 122:
				return set[26];
			case 127:
			case 128:
			case 262:
			case 271:
				return set[27];
			case 129:
				return set[28];
			case 130:
			case 318:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 137:
				return set[29];
			case 143:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 144:
			case 145:
				return set[30];
			case 147:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 151:
			case 166:
			case 183:
			case 188:
			case 194:
			case 196:
			case 200:
			case 202:
				return set[31];
			case 153:
			case 154:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 155:
			case 158:
			case 263:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 156:
			case 173:
			case 251:
			case 275:
			case 346:
			case 359:
			case 373:
			case 417:
			case 480:
			case 493:
			case 509:
			case 527:
			case 535:
			case 544:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					return a;
				}
			case 160:
			case 161:
			case 163:
			case 165:
			case 167:
			case 168:
			case 177:
			case 182:
			case 187:
			case 195:
			case 199:
				return set[32];
			case 164:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 171:
				return set[33];
			case 178:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 180:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 189:
			case 190:
				return set[34];
			case 193:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 198:
				return set[35];
			case 205:
			case 411:
			case 485:
			case 500:
			case 507:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 207:
			case 208:
			case 382:
			case 383:
			case 420:
			case 421:
			case 477:
			case 478:
			case 496:
			case 497:
				return set[36];
			case 210:
			case 211:
				return set[37];
			case 213:
			case 227:
			case 242:
			case 296:
			case 338:
			case 387:
			case 435:
			case 454:
			case 467:
			case 506:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 217:
			case 218:
				return set[38];
			case 219:
				return set[39];
			case 222:
				return set[40];
			case 223:
			case 224:
			case 324:
				return set[41];
			case 228:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 230:
			case 270:
			case 286:
				return set[42];
			case 233:
			case 234:
			case 267:
			case 268:
			case 283:
			case 284:
				return set[43];
			case 235:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 236:
				return set[44];
			case 237:
			case 255:
				return set[45];
			case 238:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 241:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 245:
			case 246:
				return set[46];
			case 247:
			case 253:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 249:
			case 250:
				return set[47];
			case 254:
				return set[48];
			case 256:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 257:
			case 258:
				return set[49];
			case 260:
			case 261:
				return set[50];
			case 272:
			case 273:
				return set[51];
			case 274:
				return set[52];
			case 277:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 280:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 288:
				return set[53];
			case 289:
			case 293:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 290:
				return set[54];
			case 298:
			case 299:
				return set[55];
			case 302:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 304:
			case 305:
				return set[56];
			case 307:
			case 308:
				return set[57];
			case 309:
			case 476:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 310:
			case 312:
				return set[58];
			case 314:
			case 320:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 317:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 321:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 322:
			case 323:
			case 327:
			case 328:
			case 385:
			case 386:
				return set[59];
			case 325:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 330:
			case 331:
				return set[60];
			case 333:
				return set[61];
			case 339:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 341:
			case 342:
			case 353:
			case 354:
				return set[62];
			case 343:
			case 355:
				return set[63];
			case 344:
				return set[64];
			case 345:
			case 350:
				return set[65];
			case 348:
				return set[66];
			case 362:
			case 364:
			case 365:
			case 450:
			case 464:
				return set[67];
			case 366:
			case 367:
				return set[68];
			case 368:
			case 369:
				return set[69];
			case 370:
			case 375:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 376:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 379:
			case 380:
				return set[70];
			case 389:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 390:
				return set[71];
			case 391:
				return set[72];
			case 392:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 396:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 398:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 399:
				return set[73];
			case 408:
				return set[74];
			case 414:
			case 415:
			case 491:
			case 503:
			case 504:
				return set[75];
			case 423:
			case 436:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 426:
			case 427:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 429:
			case 430:
				return set[76];
			case 431:
			case 432:
				return set[77];
			case 433:
			case 434:
			case 444:
				return set[78];
			case 438:
				return set[79];
			case 439:
			case 440:
				return set[80];
			case 441:
			case 442:
			case 548:
				return set[81];
			case 443:
				return set[82];
			case 445:
			case 455:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 456:
				return set[83];
			case 459:
			case 461:
			case 470:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 460:
				return set[84];
			case 462:
				return set[85];
			case 468:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 472:
			case 473:
				return set[86];
			case 474:
			case 482:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 483:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 484:
				return set[87];
			case 487:
			case 488:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 489:
			case 499:
			case 566:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 490:
				return set[88];
			case 512:
			case 513:
				return set[89];
			case 515:
			case 516:
			case 517:
			case 522:
			case 523:
			case 554:
				return set[90];
			case 518:
			case 524:
			case 534:
				return set[91];
			case 519:
			case 520:
			case 525:
			case 526:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 528:
			case 530:
			case 536:
				return set[92];
			case 531:
			case 533:
				return set[93];
			case 538:
				return set[94];
			case 539:
			case 547:
				return set[95];
			case 540:
			case 541:
				return set[96];
			case 549:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 551:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 553:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 558:
				return set[97];
			case 559:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 560:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 561:
			case 562:
				return set[98];
			case 567:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 568:
				return set[99];
			case 570:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 571:
				return set[100];
			case 572:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 573:
				return set[101];
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
		nextTokenIsPotentialStartOfExpression = false;
		readXmlIdentifier = false;
		nextTokenIsStartOfImportsOrAccessExpression = false;
		wasQualifierTokenAtStart = false;
		
		//if (la != null)
			identifierExpected = false;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 570;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					PushContext(Context.Importable, la, t);
					goto case 560;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 376;
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
					currentState = 556;
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
					goto case 376;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[102].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						PushContext(Context.TypeDeclaration, la, t);
						goto case 423;
					} else {
						if (la.kind == 103) {
							currentState = 411;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 9;
								break;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 9: {
				stateStack.Push(10);
				goto case 159;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (la.kind == 63) {
					currentState = 410;
					break;
				} else {
					goto case 11;
				}
			}
			case 11: {
				stateStack.Push(12);
				goto case 14;
			}
			case 12: {
				if (la == null) { currentState = 12; break; }
				if (set[8].Get(la.kind)) {
					goto case 17;
				} else {
					Expect(113, la); // "End"
					currentState = 13;
					break;
				}
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				Expect(115, la); // "Enum"
				currentState = 14;
				break;
			}
			case 14: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 15;
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				if (la.kind == 40) {
					stateStack.Push(17);
					goto case 376;
				} else {
					stateStack.Push(18);
					goto case 159;
				}
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (la.kind == 20) {
					currentState = 19;
					break;
				} else {
					goto case 11;
				}
			}
			case 19: {
				stateStack.Push(11);
				goto case 20;
			}
			case 20: {
				PushContext(Context.Expression, la, t);
				goto case 21;
			}
			case 21: {
				stateStack.Push(22);
				goto case 23;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (set[103].Get(la.kind)) {
					currentState = 21;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 23: {
				PushContext(Context.Expression, la, t);
				goto case 24;
			}
			case 24: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 25;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (set[104].Get(la.kind)) {
					currentState = 24;
					break;
				} else {
					if (set[27].Get(la.kind)) {
						stateStack.Push(117);
						goto case 127;
					} else {
						if (la.kind == 220) {
							currentState = 114;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(26);
								PushContext(Context.ObjectCreation, la, t);
								goto case 31;
							} else {
								if (la.kind == 35) {
									stateStack.Push(26);
									goto case 27;
								} else {
									Error(la);
									goto case 26;
								}
							}
						}
					}
				}
			}
			case 26: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				Expect(35, la); // "{"
				currentState = 28;
				break;
			}
			case 28: {
				stateStack.Push(29);
				goto case 20;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 22) {
					currentState = 28;
					break;
				} else {
					goto case 30;
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				Expect(162, la); // "New"
				currentState = 32;
				break;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(89);
					goto case 92;
				} else {
					goto case 33;
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 233) {
					currentState = 36;
					break;
				} else {
					goto case 34;
				}
			}
			case 34: {
				Error(la);
				goto case 35;
			}
			case 35: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 36: {
				stateStack.Push(35);
				goto case 37;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				Expect(35, la); // "{"
				currentState = 38;
				break;
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (la.kind == 147) {
					currentState = 39;
					break;
				} else {
					goto case 39;
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				Expect(26, la); // "."
				currentState = 40;
				break;
			}
			case 40: {
				stateStack.Push(41);
				goto case 44;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				Expect(20, la); // "="
				currentState = 42;
				break;
			}
			case 42: {
				stateStack.Push(43);
				goto case 20;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 22) {
					currentState = 38;
					break;
				} else {
					goto case 30;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 2) {
					goto case 88;
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
								goto case 87;
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
												goto case 86;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 85;
													} else {
														if (la.kind == 65) {
															goto case 84;
														} else {
															if (la.kind == 66) {
																goto case 83;
															} else {
																if (la.kind == 67) {
																	goto case 82;
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
																				goto case 81;
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
																																		goto case 80;
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
																																					goto case 79;
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
																																																goto case 78;
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
																																																						goto case 77;
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
																																																									goto case 76;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 75;
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
																																																																		goto case 74;
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
																																																																							goto case 73;
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
																																																																										goto case 72;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 71;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 70;
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
																																																																																			goto case 69;
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
																																																																																									goto case 68;
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
																																																																																													goto case 67;
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
																																																																																																goto case 66;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 65;
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
																																																																																																																goto case 64;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 63;
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
																																																																																																																								goto case 62;
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
																																																																																																																														goto case 61;
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
																																																																																																																																						goto case 60;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 59;
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
																																																																																																																																																			goto case 58;
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
																																																																																																																																																									goto case 57;
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
																																																																																																																																																												goto case 56;
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
																																																																																																																																																															goto case 55;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 54;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 53;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 52;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 51;
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
																																																																																																																																																																								goto case 50;
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
																																																																																																																																																																													goto case 49;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 48;
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
																																																																																																																																																																																				goto case 47;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 46;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 45;
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
			case 45: {
				if (la == null) { currentState = 45; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				currentState = stateStack.Pop();
				break;
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
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 90;
						break;
					} else {
						goto case 33;
					}
				} else {
					goto case 35;
				}
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				if (la.kind == 35) {
					stateStack.Push(35);
					goto case 27;
				} else {
					if (set[15].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 34;
					}
				}
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				currentState = 35;
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				if (la.kind == 130) {
					currentState = 93;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 93;
						break;
					} else {
						if (set[105].Get(la.kind)) {
							currentState = 93;
							break;
						} else {
							Error(la);
							goto case 93;
						}
					}
				}
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				if (la.kind == 37) {
					stateStack.Push(93);
					goto case 97;
				} else {
					goto case 94;
				}
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.kind == 26) {
					currentState = 95;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 95: {
				stateStack.Push(96);
				goto case 44;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				if (la.kind == 37) {
					stateStack.Push(96);
					goto case 97;
				} else {
					goto case 94;
				}
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				Expect(37, la); // "("
				currentState = 98;
				break;
			}
			case 98: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 99;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				if (la.kind == 169) {
					currentState = 112;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 101;
					} else {
						Error(la);
						goto case 100;
					}
				}
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 101: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 102;
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(100);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 103;
				} else {
					goto case 100;
				}
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				if (set[9].Get(la.kind)) {
					goto case 108;
				} else {
					if (la.kind == 22) {
						goto case 104;
					} else {
						goto case 6;
					}
				}
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				currentState = 105;
				break;
			}
			case 105: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 106;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (set[9].Get(la.kind)) {
					stateStack.Push(107);
					goto case 20;
				} else {
					goto case 107;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (la.kind == 22) {
					goto case 104;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 108: {
				stateStack.Push(109);
				goto case 20;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (la.kind == 22) {
					currentState = 110;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 110: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 111;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				if (set[9].Get(la.kind)) {
					goto case 108;
				} else {
					goto case 109;
				}
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(113);
					goto case 92;
				} else {
					goto case 113;
				}
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (la.kind == 22) {
					currentState = 112;
					break;
				} else {
					goto case 100;
				}
			}
			case 114: {
				stateStack.Push(115);
				goto case 23;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				Expect(144, la); // "Is"
				currentState = 116;
				break;
			}
			case 116: {
				stateStack.Push(26);
				goto case 92;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				if (set[24].Get(la.kind)) {
					stateStack.Push(117);
					goto case 118;
				} else {
					goto case 26;
				}
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				if (la.kind == 37) {
					currentState = 123;
					break;
				} else {
					if (set[106].Get(la.kind)) {
						currentState = 119;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 119: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 120;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				if (la.kind == 10) {
					currentState = 121;
					break;
				} else {
					goto case 121;
				}
			}
			case 121: {
				stateStack.Push(122);
				goto case 44;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 123: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 124;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				if (la.kind == 169) {
					currentState = 125;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 101;
					} else {
						goto case 6;
					}
				}
			}
			case 125: {
				stateStack.Push(126);
				goto case 92;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 22) {
					currentState = 125;
					break;
				} else {
					goto case 100;
				}
			}
			case 127: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 128;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (set[107].Get(la.kind)) {
					currentState = 129;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 409;
						break;
					} else {
						if (set[108].Get(la.kind)) {
							currentState = 129;
							break;
						} else {
							if (set[106].Get(la.kind)) {
								currentState = 405;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 403;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 400;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(129);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 389;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(129);
												goto case 205;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(129);
													PushContext(Context.Query, la, t);
													goto case 143;
												} else {
													if (set[29].Get(la.kind)) {
														stateStack.Push(129);
														goto case 137;
													} else {
														if (la.kind == 135) {
															stateStack.Push(129);
															goto case 130;
														} else {
															Error(la);
															goto case 129;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 129: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				Expect(135, la); // "If"
				currentState = 131;
				break;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				Expect(37, la); // "("
				currentState = 132;
				break;
			}
			case 132: {
				stateStack.Push(133);
				goto case 20;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				Expect(22, la); // ","
				currentState = 134;
				break;
			}
			case 134: {
				stateStack.Push(135);
				goto case 20;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.kind == 22) {
					currentState = 136;
					break;
				} else {
					goto case 100;
				}
			}
			case 136: {
				stateStack.Push(100);
				goto case 20;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (set[109].Get(la.kind)) {
					currentState = 142;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 138;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				Expect(37, la); // "("
				currentState = 139;
				break;
			}
			case 139: {
				stateStack.Push(140);
				goto case 20;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(22, la); // ","
				currentState = 141;
				break;
			}
			case 141: {
				stateStack.Push(100);
				goto case 92;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(37, la); // "("
				currentState = 136;
				break;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (la.kind == 126) {
					stateStack.Push(144);
					goto case 204;
				} else {
					if (la.kind == 58) {
						stateStack.Push(144);
						goto case 203;
					} else {
						Error(la);
						goto case 144;
					}
				}
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(144);
					goto case 145;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				if (la.kind == 126) {
					currentState = 201;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 197;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 195;
							break;
						} else {
							if (la.kind == 107) {
								goto case 76;
							} else {
								if (la.kind == 230) {
									currentState = 20;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 191;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 189;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 187;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 160;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 146;
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
			case 146: {
				stateStack.Push(147);
				goto case 152;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(171, la); // "On"
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(149);
				goto case 20;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(116, la); // "Equals"
				currentState = 150;
				break;
			}
			case 150: {
				stateStack.Push(151);
				goto case 20;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				if (la.kind == 22) {
					currentState = 148;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 152: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(153);
				goto case 159;
			}
			case 153: {
				PopContext();
				goto case 154;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 156;
				} else {
					goto case 155;
				}
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				Expect(138, la); // "In"
				currentState = 20;
				break;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(63, la); // "As"
				currentState = 157;
				break;
			}
			case 157: {
				stateStack.Push(158);
				goto case 92;
			}
			case 158: {
				PopContext();
				goto case 155;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (set[95].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 78;
					} else {
						goto case 6;
					}
				}
			}
			case 160: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 161;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 146) {
					goto case 179;
				} else {
					if (set[32].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 163;
							break;
						} else {
							if (set[32].Get(la.kind)) {
								goto case 177;
							} else {
								Error(la);
								goto case 162;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				Expect(70, la); // "By"
				currentState = 163;
				break;
			}
			case 163: {
				stateStack.Push(164);
				goto case 167;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 22) {
					currentState = 163;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 165;
					break;
				}
			}
			case 165: {
				stateStack.Push(166);
				goto case 167;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (la.kind == 22) {
					currentState = 165;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 167: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 168;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(169);
					goto case 159;
				} else {
					goto case 20;
				}
			}
			case 169: {
				PopContext();
				goto case 170;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 173;
				} else {
					if (la.kind == 20) {
						goto case 172;
					} else {
						if (set[33].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 20;
						}
					}
				}
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				currentState = 20;
				break;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				currentState = 20;
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				Expect(63, la); // "As"
				currentState = 174;
				break;
			}
			case 174: {
				stateStack.Push(175);
				goto case 92;
			}
			case 175: {
				PopContext();
				goto case 176;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(20, la); // "="
				currentState = 20;
				break;
			}
			case 177: {
				stateStack.Push(178);
				goto case 167;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 22) {
					currentState = 177;
					break;
				} else {
					goto case 162;
				}
			}
			case 179: {
				stateStack.Push(180);
				goto case 186;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 184;
						break;
					} else {
						if (la.kind == 146) {
							goto case 179;
						} else {
							Error(la);
							goto case 180;
						}
					}
				} else {
					goto case 181;
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(143, la); // "Into"
				currentState = 182;
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 167;
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
				stateStack.Push(185);
				goto case 186;
			}
			case 185: {
				stateStack.Push(180);
				goto case 181;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(146, la); // "Join"
				currentState = 146;
				break;
			}
			case 187: {
				stateStack.Push(188);
				goto case 167;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 22) {
					currentState = 187;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 189: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 190;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 231) {
					currentState = 20;
					break;
				} else {
					goto case 20;
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				Expect(70, la); // "By"
				currentState = 192;
				break;
			}
			case 192: {
				stateStack.Push(193);
				goto case 20;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (la.kind == 64) {
					currentState = 194;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 194;
						break;
					} else {
						Error(la);
						goto case 194;
					}
				}
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.kind == 22) {
					currentState = 192;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 195: {
				stateStack.Push(196);
				goto case 167;
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
				stateStack.Push(198);
				goto case 152;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(198);
					goto case 145;
				} else {
					Expect(143, la); // "Into"
					currentState = 199;
					break;
				}
			}
			case 199: {
				stateStack.Push(200);
				goto case 167;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 22) {
					currentState = 199;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 201: {
				stateStack.Push(202);
				goto case 152;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (la.kind == 22) {
					currentState = 201;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				Expect(58, la); // "Aggregate"
				currentState = 197;
				break;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				Expect(126, la); // "From"
				currentState = 201;
				break;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (la.kind == 210) {
					currentState = 381;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 206;
						break;
					} else {
						goto case 6;
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
				SetIdentifierExpected(la);
				goto case 208;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (set[67].Get(la.kind)) {
					stateStack.Push(209);
					goto case 362;
				} else {
					goto case 209;
				}
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				Expect(38, la); // ")"
				currentState = 210;
				break;
			}
			case 210: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 211;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (set[9].Get(la.kind)) {
					goto case 20;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							PushContext(Context.Type, la, t);
							goto case 359;
						} else {
							goto case 212;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 212: {
				stateStack.Push(213);
				goto case 215;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				Expect(113, la); // "End"
				currentState = 214;
				break;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 215: {
				PushContext(Context.Body, la, t);
				goto case 216;
			}
			case 216: {
				stateStack.Push(217);
				goto case 14;
			}
			case 217: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[110].Get(la.kind)) {
					if (set[59].Get(la.kind)) {
						if (set[41].Get(la.kind)) {
							stateStack.Push(216);
							goto case 223;
						} else {
							goto case 216;
						}
					} else {
						if (la.kind == 113) {
							currentState = 221;
							break;
						} else {
							goto case 220;
						}
					}
				} else {
					goto case 219;
				}
			}
			case 219: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 220: {
				Error(la);
				goto case 217;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 216;
				} else {
					if (set[40].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 220;
					}
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				currentState = 217;
				break;
			}
			case 223: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 224;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 340;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 336;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 334;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 332;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 313;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 298;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 294;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 288;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 260;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 256;
																break;
															} else {
																goto case 256;
															}
														} else {
															if (la.kind == 194) {
																currentState = 254;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 233;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 240;
																break;
															} else {
																if (set[111].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 237;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 236;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 235;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 56;
																				} else {
																					if (la.kind == 195) {
																						currentState = 233;
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
																		currentState = 231;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 229;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 225;
																				break;
																			} else {
																				if (set[112].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 20;
																						break;
																					} else {
																						goto case 20;
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
			case 225: {
				stateStack.Push(226);
				goto case 20;
			}
			case 226: {
				stateStack.Push(227);
				goto case 215;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(113, la); // "End"
				currentState = 228;
				break;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 229: {
				stateStack.Push(230);
				goto case 20;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 232;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (la.kind == 184) {
					currentState = 20;
					break;
				} else {
					goto case 20;
				}
			}
			case 233: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 234;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (set[9].Get(la.kind)) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 108) {
					goto case 75;
				} else {
					if (la.kind == 124) {
						goto case 72;
					} else {
						if (la.kind == 231) {
							goto case 46;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 108) {
					goto case 75;
				} else {
					if (la.kind == 124) {
						goto case 72;
					} else {
						if (la.kind == 231) {
							goto case 46;
						} else {
							if (la.kind == 197) {
								goto case 58;
							} else {
								if (la.kind == 210) {
									goto case 54;
								} else {
									if (la.kind == 127) {
										goto case 70;
									} else {
										if (la.kind == 186) {
											goto case 59;
										} else {
											if (la.kind == 218) {
												goto case 50;
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
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (set[6].Get(la.kind)) {
					goto case 239;
				} else {
					if (la.kind == 5) {
						goto case 238;
					} else {
						goto case 6;
					}
				}
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 240: {
				stateStack.Push(241);
				goto case 215;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (la.kind == 75) {
					currentState = 245;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 244;
						break;
					} else {
						goto case 242;
					}
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				Expect(113, la); // "End"
				currentState = 243;
				break;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 244: {
				stateStack.Push(242);
				goto case 215;
			}
			case 245: {
				SetIdentifierExpected(la);
				goto case 246;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(249);
					goto case 159;
				} else {
					goto case 247;
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 229) {
					currentState = 248;
					break;
				} else {
					goto case 240;
				}
			}
			case 248: {
				stateStack.Push(240);
				goto case 20;
			}
			case 249: {
				PopContext();
				goto case 250;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 251;
				} else {
					goto case 247;
				}
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(63, la); // "As"
				currentState = 252;
				break;
			}
			case 252: {
				stateStack.Push(253);
				goto case 92;
			}
			case 253: {
				PopContext();
				goto case 247;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 163) {
					goto case 63;
				} else {
					goto case 255;
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 5) {
					goto case 238;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 239;
					} else {
						goto case 6;
					}
				}
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				Expect(118, la); // "Error"
				currentState = 257;
				break;
			}
			case 257: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 258;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (set[9].Get(la.kind)) {
					goto case 20;
				} else {
					if (la.kind == 132) {
						currentState = 255;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 259;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(278);
					goto case 271;
				} else {
					if (la.kind == 110) {
						currentState = 262;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 262: {
				stateStack.Push(263);
				goto case 271;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				Expect(138, la); // "In"
				currentState = 264;
				break;
			}
			case 264: {
				stateStack.Push(265);
				goto case 20;
			}
			case 265: {
				stateStack.Push(266);
				goto case 215;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				Expect(163, la); // "Next"
				currentState = 267;
				break;
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (set[9].Get(la.kind)) {
					goto case 269;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 269: {
				stateStack.Push(270);
				goto case 20;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 22) {
					currentState = 269;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 271: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(272);
				goto case 127;
			}
			case 272: {
				PopContext();
				goto case 273;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 33) {
					currentState = 274;
					break;
				} else {
					goto case 274;
				}
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[24].Get(la.kind)) {
					stateStack.Push(274);
					goto case 118;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 275;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(63, la); // "As"
				currentState = 276;
				break;
			}
			case 276: {
				stateStack.Push(277);
				goto case 92;
			}
			case 277: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				Expect(20, la); // "="
				currentState = 279;
				break;
			}
			case 279: {
				stateStack.Push(280);
				goto case 20;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 205) {
					currentState = 287;
					break;
				} else {
					goto case 281;
				}
			}
			case 281: {
				stateStack.Push(282);
				goto case 215;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(163, la); // "Next"
				currentState = 283;
				break;
			}
			case 283: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 284;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (set[9].Get(la.kind)) {
					goto case 285;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 285: {
				stateStack.Push(286);
				goto case 20;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 22) {
					currentState = 285;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 287: {
				stateStack.Push(281);
				goto case 20;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 291;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(289);
						goto case 215;
					} else {
						goto case 6;
					}
				}
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				Expect(152, la); // "Loop"
				currentState = 290;
				break;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 20;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 291: {
				stateStack.Push(292);
				goto case 20;
			}
			case 292: {
				stateStack.Push(293);
				goto case 215;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 294: {
				stateStack.Push(295);
				goto case 20;
			}
			case 295: {
				stateStack.Push(296);
				goto case 215;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(113, la); // "End"
				currentState = 297;
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 299;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 74) {
					currentState = 300;
					break;
				} else {
					goto case 300;
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 20;
			}
			case 301: {
				stateStack.Push(302);
				goto case 14;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (la.kind == 74) {
					currentState = 304;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 303;
					break;
				}
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 304: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 305;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 111) {
					currentState = 306;
					break;
				} else {
					if (set[57].Get(la.kind)) {
						goto case 307;
					} else {
						Error(la);
						goto case 306;
					}
				}
			}
			case 306: {
				stateStack.Push(302);
				goto case 215;
			}
			case 307: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 308;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (set[113].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 310;
						break;
					} else {
						goto case 310;
					}
				} else {
					if (set[9].Get(la.kind)) {
						stateStack.Push(309);
						goto case 20;
					} else {
						Error(la);
						goto case 309;
					}
				}
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.kind == 22) {
					currentState = 307;
					break;
				} else {
					goto case 306;
				}
			}
			case 310: {
				stateStack.Push(311);
				goto case 312;
			}
			case 311: {
				stateStack.Push(309);
				goto case 23;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
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
			case 313: {
				stateStack.Push(314);
				goto case 20;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 214) {
					currentState = 322;
					break;
				} else {
					goto case 315;
				}
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 316;
				} else {
					goto case 6;
				}
			}
			case 316: {
				stateStack.Push(317);
				goto case 215;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 321;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 319;
							break;
						} else {
							Error(la);
							goto case 316;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 318;
					break;
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 319: {
				stateStack.Push(320);
				goto case 20;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 214) {
					currentState = 316;
					break;
				} else {
					goto case 316;
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 135) {
					currentState = 319;
					break;
				} else {
					goto case 316;
				}
			}
			case 322: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 323;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (set[41].Get(la.kind)) {
					goto case 324;
				} else {
					goto case 315;
				}
			}
			case 324: {
				stateStack.Push(325);
				goto case 223;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 21) {
					currentState = 330;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 327;
						break;
					} else {
						goto case 326;
					}
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 327: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 328;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (set[41].Get(la.kind)) {
					stateStack.Push(329);
					goto case 223;
				} else {
					goto case 329;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 21) {
					currentState = 327;
					break;
				} else {
					goto case 326;
				}
			}
			case 330: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 331;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (set[41].Get(la.kind)) {
					goto case 324;
				} else {
					goto case 325;
				}
			}
			case 332: {
				stateStack.Push(333);
				goto case 44;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 37) {
					currentState = 101;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 334: {
				stateStack.Push(335);
				goto case 20;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				Expect(22, la); // ","
				currentState = 20;
				break;
			}
			case 336: {
				stateStack.Push(337);
				goto case 20;
			}
			case 337: {
				stateStack.Push(338);
				goto case 215;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				Expect(113, la); // "End"
				currentState = 339;
				break;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 233) {
					goto case 45;
				} else {
					if (la.kind == 211) {
						goto case 53;
					} else {
						goto case 6;
					}
				}
			}
			case 340: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(341);
				goto case 159;
			}
			case 341: {
				PopContext();
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 33) {
					currentState = 343;
					break;
				} else {
					goto case 343;
				}
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 37) {
					currentState = 358;
					break;
				} else {
					goto case 344;
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 22) {
					currentState = 352;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 346;
					} else {
						goto case 345;
					}
				}
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (la.kind == 20) {
					goto case 172;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				Expect(63, la); // "As"
				currentState = 347;
				break;
			}
			case 347: {
				stateStack.Push(348);
				goto case 92;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 162) {
					currentState = 351;
					break;
				} else {
					goto case 349;
				}
			}
			case 349: {
				stateStack.Push(350);
				goto case 92;
			}
			case 350: {
				if (CurrentBlock.context == Context.ObjectCreation)
					PopContext();
				PopContext();

				goto case 345;
			}
			case 351: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 349;
			}
			case 352: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(353);
				goto case 159;
			}
			case 353: {
				PopContext();
				goto case 354;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 33) {
					currentState = 355;
					break;
				} else {
					goto case 355;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 37) {
					currentState = 356;
					break;
				} else {
					goto case 344;
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 22) {
					currentState = 356;
					break;
				} else {
					goto case 357;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(38, la); // ")"
				currentState = 344;
				break;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 22) {
					currentState = 358;
					break;
				} else {
					goto case 357;
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				Expect(63, la); // "As"
				currentState = 360;
				break;
			}
			case 360: {
				stateStack.Push(361);
				goto case 92;
			}
			case 361: {
				PopContext();
				goto case 212;
			}
			case 362: {
				stateStack.Push(363);
				PushContext(Context.Parameter, la, t);
				goto case 364;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 22) {
					currentState = 362;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 364: {
				SetIdentifierExpected(la);
				goto case 365;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 40) {
					stateStack.Push(364);
					goto case 376;
				} else {
					goto case 366;
				}
			}
			case 366: {
				SetIdentifierExpected(la);
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (set[114].Get(la.kind)) {
					currentState = 366;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(368);
					goto case 159;
				}
			}
			case 368: {
				PopContext();
				goto case 369;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 373;
				} else {
					goto case 370;
				}
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 20) {
					currentState = 372;
					break;
				} else {
					goto case 371;
				}
			}
			case 371: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 372: {
				stateStack.Push(371);
				goto case 20;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				Expect(63, la); // "As"
				currentState = 374;
				break;
			}
			case 374: {
				stateStack.Push(375);
				goto case 92;
			}
			case 375: {
				PopContext();
				goto case 370;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				Expect(40, la); // "<"
				currentState = 377;
				break;
			}
			case 377: {
				PushContext(Context.Attribute, la, t);
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (set[115].Get(la.kind)) {
					currentState = 378;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 379;
					break;
				}
			}
			case 379: {
				PopContext();
				goto case 380;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				Expect(37, la); // "("
				currentState = 382;
				break;
			}
			case 382: {
				SetIdentifierExpected(la);
				goto case 383;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (set[67].Get(la.kind)) {
					stateStack.Push(384);
					goto case 362;
				} else {
					goto case 384;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(38, la); // ")"
				currentState = 385;
				break;
			}
			case 385: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 386;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (set[41].Get(la.kind)) {
					goto case 223;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(387);
						goto case 215;
					} else {
						goto case 6;
					}
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				Expect(113, la); // "End"
				currentState = 388;
				break;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 399;
					break;
				} else {
					stateStack.Push(390);
					goto case 392;
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 17) {
					currentState = 391;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 16) {
					currentState = 390;
					break;
				} else {
					goto case 390;
				}
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 393;
				break;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (set[116].Get(la.kind)) {
					if (set[117].Get(la.kind)) {
						currentState = 393;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(393);
							goto case 396;
						} else {
							Error(la);
							goto case 393;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 394;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (set[118].Get(la.kind)) {
					if (set[119].Get(la.kind)) {
						currentState = 394;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(394);
							goto case 396;
						} else {
							if (la.kind == 10) {
								stateStack.Push(394);
								goto case 392;
							} else {
								Error(la);
								goto case 394;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 395;
					break;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (set[120].Get(la.kind)) {
					if (set[121].Get(la.kind)) {
						currentState = 395;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(395);
							goto case 396;
						} else {
							Error(la);
							goto case 395;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 397;
				break;
			}
			case 397: {
				stateStack.Push(398);
				goto case 20;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (la.kind == 16) {
					currentState = 389;
					break;
				} else {
					goto case 389;
				}
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				Expect(37, la); // "("
				currentState = 401;
				break;
			}
			case 401: {
				readXmlIdentifier = true;
				stateStack.Push(402);
				goto case 159;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				Expect(38, la); // ")"
				currentState = 129;
				break;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(37, la); // "("
				currentState = 404;
				break;
			}
			case 404: {
				stateStack.Push(402);
				goto case 92;
			}
			case 405: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 406;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 10) {
					currentState = 407;
					break;
				} else {
					goto case 407;
				}
			}
			case 407: {
				stateStack.Push(408);
				goto case 44;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (la.kind == 11) {
					currentState = 129;
					break;
				} else {
					goto case 129;
				}
			}
			case 409: {
				stateStack.Push(402);
				goto case 20;
			}
			case 410: {
				stateStack.Push(11);
				goto case 92;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 210) {
					currentState = 412;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 412;
						break;
					} else {
						Error(la);
						goto case 412;
					}
				}
			}
			case 412: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 413;
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				currentState = 414;
				break;
			}
			case 414: {
				PopContext();
				goto case 415;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 37) {
					currentState = 420;
					break;
				} else {
					goto case 416;
				}
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 417;
				} else {
					goto case 14;
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				Expect(63, la); // "As"
				currentState = 418;
				break;
			}
			case 418: {
				stateStack.Push(419);
				goto case 92;
			}
			case 419: {
				PopContext();
				goto case 14;
			}
			case 420: {
				SetIdentifierExpected(la);
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (set[67].Get(la.kind)) {
					stateStack.Push(422);
					goto case 362;
				} else {
					goto case 422;
				}
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(38, la); // ")"
				currentState = 416;
				break;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 155) {
					currentState = 424;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 424;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 424;
							break;
						} else {
							Error(la);
							goto case 424;
						}
					}
				}
			}
			case 424: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				currentState = 426;
				break;
			}
			case 426: {
				PopContext();
				goto case 427;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (la.kind == 37) {
					currentState = 553;
					break;
				} else {
					goto case 428;
				}
			}
			case 428: {
				stateStack.Push(429);
				goto case 14;
			}
			case 429: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 430;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 551;
				} else {
					goto case 431;
				}
			}
			case 431: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 432;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 549;
				} else {
					goto case 433;
				}
			}
			case 433: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 434;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (set[80].Get(la.kind)) {
					stateStack.Push(433);
					isMissingModifier = true;
					PushContext(Context.Member, la, t);
					goto case 439;
				} else {
					isMissingModifier = false; Console.WriteLine("after {MemberDeclaration}");
					goto case 435;
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				Expect(113, la); // "End"
				currentState = 436;
				break;
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (la.kind == 155) {
					currentState = 437;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 437;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 437;
							break;
						} else {
							Error(la);
							goto case 437;
						}
					}
				}
			}
			case 437: {
				stateStack.Push(438);
				goto case 14;
			}
			case 438: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 439: {
				SetIdentifierExpected(la);
				goto case 440;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 40) {
					stateStack.Push(439);
					goto case 376;
				} else {
					goto case 441;
				}
			}
			case 441: {
				SetIdentifierExpected(la);
				goto case 442;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (set[122].Get(la.kind)) {
					currentState = 548;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 443;
				}
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (set[94].Get(la.kind)) {
					stateStack.Push(444);
					SetIdentifierExpected(la);
					goto case 538;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(444);
						goto case 500;
					} else {
						if (la.kind == 101) {
							stateStack.Push(444);
							goto case 483;
						} else {
							if (la.kind == 119) {
								stateStack.Push(444);
								goto case 470;
							} else {
								if (la.kind == 98) {
									stateStack.Push(444);
									goto case 458;
								} else {
									if (la.kind == 172) {
										stateStack.Push(444);
										goto case 445;
									} else {
										Error(la);
										goto case 444;
									}
								}
							}
						}
					}
				}
			}
			case 444: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				Expect(172, la); // "Operator"
				currentState = 446;
				break;
			}
			case 446: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 447;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				currentState = 448;
				break;
			}
			case 448: {
				PopContext();
				goto case 449;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				Expect(37, la); // "("
				currentState = 450;
				break;
			}
			case 450: {
				stateStack.Push(451);
				goto case 362;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				Expect(38, la); // ")"
				currentState = 452;
				break;
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 63) {
					currentState = 456;
					break;
				} else {
					goto case 453;
				}
			}
			case 453: {
				stateStack.Push(454);
				goto case 215;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				Expect(113, la); // "End"
				currentState = 455;
				break;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				Expect(172, la); // "Operator"
				currentState = 14;
				break;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (la.kind == 40) {
					stateStack.Push(456);
					goto case 376;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(457);
					goto case 92;
				}
			}
			case 457: {
				PopContext();
				goto case 453;
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				Expect(98, la); // "Custom"
				currentState = 459;
				break;
			}
			case 459: {
				stateStack.Push(460);
				goto case 470;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (set[85].Get(la.kind)) {
					goto case 462;
				} else {
					Expect(113, la); // "End"
					currentState = 461;
					break;
				}
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				Expect(119, la); // "Event"
				currentState = 14;
				break;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (la.kind == 40) {
					stateStack.Push(462);
					goto case 376;
				} else {
					if (la.kind == 56) {
						currentState = 463;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 463;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 463;
								break;
							} else {
								Error(la);
								goto case 463;
							}
						}
					}
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(37, la); // "("
				currentState = 464;
				break;
			}
			case 464: {
				stateStack.Push(465);
				goto case 362;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				Expect(38, la); // ")"
				currentState = 466;
				break;
			}
			case 466: {
				stateStack.Push(467);
				goto case 215;
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				Expect(113, la); // "End"
				currentState = 468;
				break;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (la.kind == 56) {
					currentState = 469;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 469;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 469;
							break;
						} else {
							Error(la);
							goto case 469;
						}
					}
				}
			}
			case 469: {
				stateStack.Push(460);
				goto case 14;
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				Expect(119, la); // "Event"
				currentState = 471;
				break;
			}
			case 471: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(472);
				goto case 159;
			}
			case 472: {
				PopContext();
				goto case 473;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 480;
				} else {
					if (set[123].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 477;
							break;
						} else {
							goto case 474;
						}
					} else {
						Error(la);
						goto case 474;
					}
				}
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (la.kind == 136) {
					currentState = 475;
					break;
				} else {
					goto case 14;
				}
			}
			case 475: {
				stateStack.Push(476);
				goto case 92;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 22) {
					currentState = 475;
					break;
				} else {
					goto case 14;
				}
			}
			case 477: {
				SetIdentifierExpected(la);
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (set[67].Get(la.kind)) {
					stateStack.Push(479);
					goto case 362;
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
				if (la == null) { currentState = 480; break; }
				Expect(63, la); // "As"
				currentState = 481;
				break;
			}
			case 481: {
				stateStack.Push(482);
				goto case 92;
			}
			case 482: {
				PopContext();
				goto case 474;
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				Expect(101, la); // "Declare"
				currentState = 484;
				break;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 485;
					break;
				} else {
					goto case 485;
				}
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				if (la.kind == 210) {
					currentState = 486;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 486;
						break;
					} else {
						Error(la);
						goto case 486;
					}
				}
			}
			case 486: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(487);
				goto case 159;
			}
			case 487: {
				PopContext();
				goto case 488;
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				Expect(149, la); // "Lib"
				currentState = 489;
				break;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				Expect(3, la); // LiteralString
				currentState = 490;
				break;
			}
			case 490: {
				if (la == null) { currentState = 490; break; }
				if (la.kind == 59) {
					currentState = 499;
					break;
				} else {
					goto case 491;
				}
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 37) {
					currentState = 496;
					break;
				} else {
					goto case 492;
				}
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 493;
				} else {
					goto case 14;
				}
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				Expect(63, la); // "As"
				currentState = 494;
				break;
			}
			case 494: {
				stateStack.Push(495);
				goto case 92;
			}
			case 495: {
				PopContext();
				goto case 14;
			}
			case 496: {
				SetIdentifierExpected(la);
				goto case 497;
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				if (set[67].Get(la.kind)) {
					stateStack.Push(498);
					goto case 362;
				} else {
					goto case 498;
				}
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				Expect(38, la); // ")"
				currentState = 492;
				break;
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				Expect(3, la); // LiteralString
				currentState = 491;
				break;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 210) {
					currentState = 501;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 501;
						break;
					} else {
						Error(la);
						goto case 501;
					}
				}
			}
			case 501: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 502;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				currentState = 503;
				break;
			}
			case 503: {
				PopContext();
				goto case 504;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (la.kind == 37) {
					currentState = 512;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 509;
					} else {
						goto case 505;
					}
				}
			}
			case 505: {
				stateStack.Push(506);
				goto case 215;
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				Expect(113, la); // "End"
				currentState = 507;
				break;
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				if (la.kind == 210) {
					currentState = 14;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 14;
						break;
					} else {
						goto case 508;
					}
				}
			}
			case 508: {
				Error(la);
				goto case 14;
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				Expect(63, la); // "As"
				currentState = 510;
				break;
			}
			case 510: {
				stateStack.Push(511);
				goto case 92;
			}
			case 511: {
				PopContext();
				goto case 505;
			}
			case 512: {
				SetIdentifierExpected(la);
				goto case 513;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				if (set[124].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 515;
						break;
					} else {
						if (set[67].Get(la.kind)) {
							stateStack.Push(514);
							goto case 362;
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
				currentState = 504;
				break;
			}
			case 515: {
				stateStack.Push(514);
				goto case 516;
			}
			case 516: {
				SetIdentifierExpected(la);
				goto case 517;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 518;
					break;
				} else {
					goto case 518;
				}
			}
			case 518: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(519);
				goto case 534;
			}
			case 519: {
				PopContext();
				goto case 520;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 535;
				} else {
					goto case 521;
				}
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 22) {
					currentState = 522;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 522: {
				SetIdentifierExpected(la);
				goto case 523;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 524;
					break;
				} else {
					goto case 524;
				}
			}
			case 524: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(525);
				goto case 534;
			}
			case 525: {
				PopContext();
				goto case 526;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 527;
				} else {
					goto case 521;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				Expect(63, la); // "As"
				currentState = 528;
				break;
			}
			case 528: {
				stateStack.Push(529);
				goto case 530;
			}
			case 529: {
				PopContext();
				goto case 521;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (set[93].Get(la.kind)) {
					goto case 533;
				} else {
					if (la.kind == 35) {
						currentState = 531;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 531: {
				stateStack.Push(532);
				goto case 533;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				if (la.kind == 22) {
					currentState = 531;
					break;
				} else {
					goto case 30;
				}
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (set[16].Get(la.kind)) {
					currentState = 93;
					break;
				} else {
					if (la.kind == 162) {
						goto case 64;
					} else {
						if (la.kind == 84) {
							goto case 80;
						} else {
							if (la.kind == 209) {
								goto case 55;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				if (la.kind == 2) {
					goto case 88;
				} else {
					if (la.kind == 62) {
						goto case 86;
					} else {
						if (la.kind == 64) {
							goto case 85;
						} else {
							if (la.kind == 65) {
								goto case 84;
							} else {
								if (la.kind == 66) {
									goto case 83;
								} else {
									if (la.kind == 67) {
										goto case 82;
									} else {
										if (la.kind == 70) {
											goto case 81;
										} else {
											if (la.kind == 87) {
												goto case 79;
											} else {
												if (la.kind == 104) {
													goto case 77;
												} else {
													if (la.kind == 107) {
														goto case 76;
													} else {
														if (la.kind == 116) {
															goto case 74;
														} else {
															if (la.kind == 121) {
																goto case 73;
															} else {
																if (la.kind == 133) {
																	goto case 69;
																} else {
																	if (la.kind == 139) {
																		goto case 68;
																	} else {
																		if (la.kind == 143) {
																			goto case 67;
																		} else {
																			if (la.kind == 146) {
																				goto case 66;
																			} else {
																				if (la.kind == 147) {
																					goto case 65;
																				} else {
																					if (la.kind == 170) {
																						goto case 62;
																					} else {
																						if (la.kind == 176) {
																							goto case 61;
																						} else {
																							if (la.kind == 184) {
																								goto case 60;
																							} else {
																								if (la.kind == 203) {
																									goto case 57;
																								} else {
																									if (la.kind == 212) {
																										goto case 52;
																									} else {
																										if (la.kind == 213) {
																											goto case 51;
																										} else {
																											if (la.kind == 223) {
																												goto case 49;
																											} else {
																												if (la.kind == 224) {
																													goto case 48;
																												} else {
																													if (la.kind == 230) {
																														goto case 47;
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
			case 535: {
				if (la == null) { currentState = 535; break; }
				Expect(63, la); // "As"
				currentState = 536;
				break;
			}
			case 536: {
				stateStack.Push(537);
				goto case 530;
			}
			case 537: {
				PopContext();
				goto case 521;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 88) {
					currentState = 539;
					break;
				} else {
					goto case 539;
				}
			}
			case 539: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(540);
				goto case 547;
			}
			case 540: {
				PopContext();
				goto case 541;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 544;
				} else {
					goto case 542;
				}
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				if (la.kind == 20) {
					currentState = 543;
					break;
				} else {
					goto case 14;
				}
			}
			case 543: {
				stateStack.Push(14);
				goto case 20;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				Expect(63, la); // "As"
				currentState = 545;
				break;
			}
			case 545: {
				stateStack.Push(546);
				goto case 92;
			}
			case 546: {
				PopContext();
				goto case 542;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				if (set[108].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 87;
					} else {
						if (la.kind == 126) {
							goto case 71;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 548: {
				isMissingModifier = false;
				goto case 441;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				Expect(136, la); // "Implements"
				currentState = 550;
				break;
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (set[40].Get(la.kind)) {
					currentState = 550;
					break;
				} else {
					stateStack.Push(433);
					goto case 14;
				}
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				Expect(140, la); // "Inherits"
				currentState = 552;
				break;
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (set[40].Get(la.kind)) {
					currentState = 552;
					break;
				} else {
					stateStack.Push(431);
					goto case 14;
				}
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				Expect(169, la); // "Of"
				currentState = 554;
				break;
			}
			case 554: {
				stateStack.Push(555);
				goto case 516;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				Expect(38, la); // ")"
				currentState = 428;
				break;
			}
			case 556: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 557;
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (set[40].Get(la.kind)) {
					currentState = 557;
					break;
				} else {
					PopContext();
					stateStack.Push(558);
					goto case 14;
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(558);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 559;
					break;
				}
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				Expect(160, la); // "Namespace"
				currentState = 14;
				break;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				Expect(137, la); // "Imports"
				currentState = 561;
				break;
			}
			case 561: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 562;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				if (set[16].Get(la.kind)) {
					currentState = 568;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 564;
						break;
					} else {
						Error(la);
						goto case 563;
					}
				}
			}
			case 563: {
				PopContext();
				goto case 14;
			}
			case 564: {
				stateStack.Push(565);
				goto case 159;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(20, la); // "="
				currentState = 566;
				break;
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				Expect(3, la); // LiteralString
				currentState = 567;
				break;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 563;
				break;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 37) {
					stateStack.Push(568);
					goto case 97;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 569;
						break;
					} else {
						goto case 563;
					}
				}
			}
			case 569: {
				stateStack.Push(563);
				goto case 92;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(173, la); // "Option"
				currentState = 571;
				break;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 573;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 572;
						break;
					} else {
						goto case 508;
					}
				}
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				if (la.kind == 213) {
					currentState = 14;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 14;
						break;
					} else {
						goto case 508;
					}
				}
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 14;
					break;
				} else {
					goto case 14;
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
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134218240, 436215809, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134218240, 436207617, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134217728, 436207616, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 134217728, 436207616, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564474, 1962934261, 8650975, 1108388124, 81767716, 17272068, -512676304, 4707}),
		new BitArray(new int[] {-940564474, 1962934229, 8650975, 1108388124, 81767716, 17272068, -512676304, 4707}),
		new BitArray(new int[] {-62257156, 1174405224, -51646385, -972026621, -1039365982, 17106484, -1707866112, 8257}),
		new BitArray(new int[] {-62257156, 1174405224, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-62257156, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {4194308, 1140850752, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
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
		new BitArray(new int[] {7, 1157628160, 26477055, -493343812, 680306724, 1006458243, -533262446, 1347}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {721920, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537696544, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692448, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {1, 256, 1048576, 537526400, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850688, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 17110016, -2144073728, 65}),
		new BitArray(new int[] {4, 1140850944, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850696, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850688, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 128, 0}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
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
		new BitArray(new int[] {0, 0, 0, 536871424, 536870912, 448266370, 384, 1280}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21317120, -2144335872, 65})

	};

} // end Parser


}