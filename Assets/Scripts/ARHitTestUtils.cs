using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
	public static class ARHitTestUtils
	{
		public static bool GetARHitResult(ARPoint point, ARHitTestResultType resultTypes, out ARHitTestResult res)
		{
			List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
			if (hitResults.Count > 0) 
			{
				res = hitResults[0];
				return true;
			}
			return false;
		}

		public static bool GetBestARHitResult(Vector2 viewportPoint, ref ARHitTestResult res)
		{
			ARPoint point = new ARPoint {
				x = viewportPoint.x,
				y = viewportPoint.y
			};

			// prioritize reults types
			ARHitTestResultType[] resultTypes = {
				ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
				// if you want to use infinite planes use this:
				//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
				ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
				ARHitTestResultType.ARHitTestResultTypeFeaturePoint
			}; 

			foreach (ARHitTestResultType resultType in resultTypes)
			{
				if (GetARHitResult(point, resultType, out res))
				{
					return true;
				}
			}
			return false;
		}
	}

}