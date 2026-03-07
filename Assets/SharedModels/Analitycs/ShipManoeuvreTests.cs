using System.Collections;
using UnityEngine;

/// <summary>
/// Script for testing ship's manoeuvrability according to IMO MSC.137(76) resolution.
/// <br></br>
/// Attach script to Ship game object and use context menu in editor to perform the tests.
/// <br></br>
/// Test are <b>vulnerable to origin point recentering</b>, adjust recenter threshold to avoid distorted results.
/// </summary>
[RequireComponent(typeof(Ship))]
public class ShipManoeuvreTests : MonoBehaviour
{
	private Ship Ship;
	[SerializeField]
	[Tooltip("m/s")]
	private float ShipStartingSpeed = 8;

	private void Awake()
	{
		Ship = GetComponent<Ship>();
	}
	[ContextMenu("Turning circle manoeuvre")]
	public void RunTurningCircle() => StartTurningCircleTest();

	[ContextMenu("Zig-Zag test/10°-10°")]
	public void RunZigZag10() => StartZigZagTest(10);

	[ContextMenu("Zig-Zag test/20°-20°")]
	public void RunZigZag20() => StartZigZagTest(20);

	private void StartTurningCircleTest()
	{
		StopAllCoroutines();
		StartCoroutine(TurningCircleCoroutine());
	}
	private IEnumerator TurningCircleCoroutine()
	{
		Ship.ResetState(Mathf.Min(Ship.Vmax, ShipStartingSpeed));
		Ship.EnginePower = 1;

		float L = Ship.Length;
		float V = (float)Ship.Speed;
		float rudderMax = (float)Ship.RudderMax;

		if (V < 0.1f || Ship.Rot != 0)
		{
			Debug.LogError("Ship's velocity is too small or ship's yaw rate is not 0.");
			yield break;
		}

		if (transform.position == Vector3.zero) // DeltaAngle returns wrong results with Vector3.zero position
			yield return new WaitUntil(() => transform.position != Vector3.zero);

		Vector3 startPos = transform.position;
		Vector3 startAngle = transform.forward;
		float startHdg = (float)Ship.Hdg;

		Debug.Log($"<color=cyan>--- STARTING TURNING CIRCLE MANOEUVRE ---</color>\n" +
				  $"Heading: {startHdg}°, speed: {V:F2}m/s");
		
		// Rudder order
		Ship.Rudder = (double)rudderMax;

		// wait until heading changes 90 deg
		Debug.Log("Waiting for 90 deg heading deviation...");
		yield return new WaitUntil(() => Mathf.DeltaAngle(startHdg, (float)Ship.Hdg) >= 90 );

		// Calculate advance
		float angleAdv = Vector3.Angle(startAngle, transform.position - startPos);
		float distanceAdv = Vector3.Distance(startPos, transform.position);
		float advance = Mathf.Abs(Mathf.Cos(angleAdv * Mathf.Deg2Rad) * distanceAdv);

		startHdg += 90; // used because Mathf.DeltaAngle returns -180:180 deg
		// wait until heading changes 180 deg
		Debug.Log("Waiting for 180 deg heading deviation...");
		yield return new WaitUntil(() => Mathf.Abs(Mathf.DeltaAngle(startHdg, (float)Ship.Hdg)) >= 90);

		// Calculate tactical diameter
		float angleTD = Vector3.Angle(startAngle, transform.position - startPos);
		float distanceTD = Vector3.Distance(startPos, transform.position);
		float tacticalDiameter = Mathf.Abs(Mathf.Sin(angleTD * Mathf.Deg2Rad) * distanceTD);

		string resultMsg = $"<color=cyan>--- TURNING CIRCLE MANOEUVRE RESULTS ---</color>\n";
		resultMsg += $"Angle: {angleAdv}; Distance: {distanceAdv:F2}; Advance: {advance:F2}m\n";
		resultMsg += $"Angle: {angleTD}; Distance: {distanceTD:F2}; Tactical diameter: {tacticalDiameter:F2}m\n";
		Debug.Log(resultMsg);



	}

	public void StartZigZagTest(int angleDeg)
	{
		StopAllCoroutines();
		StartCoroutine(ZigZagCoroutine(angleDeg));
	}
	private IEnumerator ZigZagCoroutine(int angleDeg)
	{
		Ship.ResetState(Mathf.Min(Ship.Vmax, ShipStartingSpeed));
		Ship.EnginePower = 1;

		float L = Ship.Length;
		float V = (float)Ship.Speed;

		if (V < 0.1f || Ship.Rot != 0)
		{
			Debug.LogError("Ship's velocity is too small or ship's yaw rate is not 0.");
			yield break;
		}

		float LoV = L / V;
        float maxFirstOvershoot = 0f;
		float maxSecondOvershoot = 0f;

		// Overshoot limits evaluation
		if (angleDeg == 10)
		{
			// First overshoot angle (10°/10°)
			if (LoV < 10f) maxFirstOvershoot = 10f;
            else if (LoV >= 30f) maxFirstOvershoot = 20f;
            else maxFirstOvershoot = 5f + 0.5f * LoV;

			// Second overshoot angle (10°/10°)
			if (LoV < 10f) maxSecondOvershoot = 25f;
			else if (LoV >= 30f) maxSecondOvershoot = 40f;
			else maxSecondOvershoot = 17.5f + 0.75f * LoV;
        }
		else if (angleDeg == 20)
		{
			// First overshoot angle (20°/20°)
			maxFirstOvershoot = 25f;
        }

		float startHdg = (float)Ship.Hdg;
		float currentDev = 0f;

		Debug.Log($"<color=cyan>--- STARTING ZIG-ZAG {angleDeg}°/{angleDeg}° TEST ---</color>\n" +
				  $"L/V = {LoV:F2} s (L={L}m, V={V:F2}m/s)");

		// --- First execute ---
		Ship.Rudder = angleDeg;
        
        yield return new WaitUntil(() => Mathf.DeltaAngle(startHdg, (float)Ship.Hdg) >= angleDeg);

		// --- Second execute and measurement of first overshot angle ---
		Ship.Rudder = -angleDeg;
        float peakFirstDeviation = angleDeg;

		while ((currentDev = Mathf.DeltaAngle(startHdg, (float)Ship.Hdg)) > -angleDeg)
		{
			if (currentDev > peakFirstDeviation)
				peakFirstDeviation = currentDev;
			yield return null;
		}

		float firstOvershoot = peakFirstDeviation - angleDeg;

		// --- Third execute and measurement of second overshot angle ---
		Ship.Rudder = angleDeg;
        float peakSecondDeviation = -angleDeg;

		while ((currentDev = Mathf.DeltaAngle(startHdg, (float)Ship.Hdg)) < angleDeg)
		{
			if (currentDev < peakSecondDeviation)
				peakSecondDeviation = currentDev;
			yield return null;
		}

		float secondOvershoot = Mathf.Abs(peakSecondDeviation) - angleDeg;

        // --- Test closure ---
        Ship.Rudder = 0;

		// Results presentation
		string resultMsg = $"<color=cyan>--- ZIG-ZAG {angleDeg}°/{angleDeg}° RESULTS ---</color>\n";

		resultMsg += $"<b>First overshoot angle:</b> {firstOvershoot:F2}° ";
		if (maxFirstOvershoot > 0)
		{
			bool passed = firstOvershoot <= maxFirstOvershoot;
			resultMsg += passed ? $"<color=green>(Passed, limit: {maxFirstOvershoot:F2}°)</color>\n" : $"<color=red>(Failed (ship's manoeuvrability unsatisfactory), limit: {maxFirstOvershoot:F2}°)</color>\n";
		}
		else resultMsg += "\n";

		resultMsg += $"<b>Second overshoot angle:</b> {secondOvershoot:F2}° ";
		if (angleDeg == 10)
		{
			bool passed = secondOvershoot <= maxSecondOvershoot;
			resultMsg += passed ? $"<color=green>(Passed, limit: {maxSecondOvershoot:F2}°)</color>\n" : $"<color=red>(Failed (ship's manoeuvrability unsatisfactory), limit: {maxSecondOvershoot:F2}°)</color>\n";
		}

		Debug.Log(resultMsg);
	}
}