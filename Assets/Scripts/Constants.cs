using UnityEngine;
using System.Collections;

public static class Constants  {
	
	
	//  BLOCK  - 0100000000
	public static int MaskBlock = 256;

	//  BLOCK  - Target 1000000000
	public static int MaskTarget = 512;

	
	// FLOOR - BLOCK - DEFAULT 10100000001
	public static int MaskCollision  = 1281;
	
	// DEFAULT - BLOCK - FLOOR - 10000000000
	public static int MaskGround  = 1024;
	
}
