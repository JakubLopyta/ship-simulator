using System.Collections;
using UnityEngine;

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

	[ContextMenu("Zig-Zag test/10°-10°")]
	public void RunZigZag10() => StartZigZagTest(10);

	[ContextMenu("Zig-Zag test/20°-20°")]
	public void RunZigZag20() => StartZigZagTest(20);

	public void StartZigZagTest(int angleDeg)
	{
		StopAllCoroutines();
		StartCoroutine(ZigZagCoroutine(angleDeg));
	}

	private IEnumerator ZigZagCoroutine(int angleDeg)
	{
		Ship.ResetState(Mathf.Min(Ship.Vmax, ShipStartingSpeed));

		float L = Ship.Length;
		float V = (float)Ship.Speed;

		if (V < 0.1f)
		{
			Debug.LogError("Ship's velocity is too small");
			yield break;
		}

		float LoV = L / V;
        float maxFirstOvershoot = 0f;
		float maxSecondOvershoot = 0f;

		// 2. Wyliczenie dopuszczalnych limitów wg rezolucji IMO MSC.137(76)
		if (angleDeg == 10)
		{
			// Limity dla pierwszego kąta przeregulowania (10°/10°)
			if (LoV < 10f) maxFirstOvershoot = 10f;
            else if (LoV >= 30f) maxFirstOvershoot = 20f;
            else maxFirstOvershoot = 5f + 0.5f * LoV;

			// Limity dla drugiego kąta przeregulowania (10°/10°)
			if (LoV < 10f) maxSecondOvershoot = 25f;
			else if (LoV >= 30f) maxSecondOvershoot = 40f;
			else maxSecondOvershoot = 17.5f + 0.75f * LoV;
        }
		else if (angleDeg == 20)
		{
			// Limit dla pierwszego kąta przeregulowania (20°/20°)
			maxFirstOvershoot = 25f;
        }

		float startHdg = (float)Ship.Hdg;
		float currentDev = 0f;

		Debug.Log($"<color=cyan>--- START TESTU ZIG-ZAG {angleDeg}°/{angleDeg}° ---</color>\n" +
				  $"Współczynnik L/V = {LoV:F2} s (L={L}m, V={V:F2}m/s)");

		// --- FAZA 1: Pierwsze przełożenie steru ---
		Ship.Rudder  = angleDeg;
        
        yield return new WaitUntil(() => Mathf.DeltaAngle(startHdg, (float)Ship.Hdg) >= angleDeg);

		// --- FAZA 2: Drugie przełożenie steru i pomiar pierwszego przeregulowania ---
		Ship.Rudder = -angleDeg;
        float peakFirstDeviation = angleDeg;

		while ((currentDev = Mathf.DeltaAngle(startHdg, (float)Ship.Hdg)) > -angleDeg)
		{
			if (currentDev > peakFirstDeviation)
				peakFirstDeviation = currentDev;
			yield return null;
		}

		float firstOvershoot = peakFirstDeviation - angleDeg;

        // --- FAZA 3: Trzecie przełożenie steru i pomiar drugiego przeregulowania ---
        Ship.Rudder = angleDeg;
        float peakSecondDeviation = -angleDeg;

		while ((currentDev = Mathf.DeltaAngle(startHdg, (float)Ship.Hdg)) < angleDeg)
		{
			if (currentDev < peakSecondDeviation)
				peakSecondDeviation = currentDev;
			yield return null;
		}

		float secondOvershoot = Mathf.Abs(peakSecondDeviation) - angleDeg;

        // --- ZAKOŃCZENIE TESTU ---
        Ship.Rudder = 0;

		// Prezentacja wyników
		string resultMsg = $"<color=cyan>--- WYNIKI TESTU ZIG-ZAG {angleDeg}°/{angleDeg}° ---</color>\n";

		resultMsg += $"<b>Pierwszy kąt przeregulowania:</b> {firstOvershoot:F2}° ";
		if (maxFirstOvershoot > 0)
		{
			bool passed = firstOvershoot <= maxFirstOvershoot;
			resultMsg += passed ? $"<color=green>(Zaliczony, limit: {maxFirstOvershoot:F2}°)</color>\n" : $"<color=red>(Niezaliczony, limit: {maxFirstOvershoot:F2}°)</color>\n";
		}
		else resultMsg += "\n";

		resultMsg += $"<b>Drugi kąt przeregulowania:</b> {secondOvershoot:F2}° ";
		if (angleDeg == 10)
		{
			bool passed = secondOvershoot <= maxSecondOvershoot;
			resultMsg += passed ? $"<color=green>(Zaliczony, limit: {maxSecondOvershoot:F2}°)</color>" : $"<color=red>(Niezaliczony, limit: {maxSecondOvershoot:F2}°)</color>";
		}

		Debug.Log(resultMsg);
	}
}