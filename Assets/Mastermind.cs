using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class Mastermind : MonoBehaviour {

	public KMBombInfo Info;
	public KMBombModule Module;
	public KMAudio Audio;
	public KMSelectable[] Slot; //array for the clickable buttons(or slots)
	public KMSelectable Query; //query-button
	public KMSelectable Submit; //submit-button

	public TextMesh DisplaytextLeft; //left display text
	public TextMesh DisplaytextRight; //right display text
	private Color[] SlotColor = { Color.white, Color.magenta, Color.yellow, Color.green, Color.red, Color.blue };

	private int[] SlotNR = {0, 0, 0, 0, 0}; 
	private int[] corrSlot = {0, 0, 0, 0, 0}; 
	private string[] SlotColors = {"W", "M", "Y", "G", "R", "B"};
	private int PegBlack = 0;
	private int PegWhite = 0;
	private int PegGray = 0;
	private int DisplayLeftMod;
	private int DisplayRightMod;
	private string RColor;
	private string LColor;

	private bool isActive = false;
	private bool isSolved = false;

	private static int moduleIdCounter = 1;
	private int moduleId = 0;

	string iiSerial;
	int iiLast;
	int iiFirst;
	int iiSum;
	int iiStrikes;
	int iiBatteries;
	int iiBatteryHolders;
	int iiLit;
	int iiUnlit;
	int iiPorts;
	int iiPortTypes;
	int iiModules;
	int iiSolved;
	int iiRs;






	// Loading screen
	void Start () {
	
		moduleId = moduleIdCounter++;
		Module.OnActivate += Activate;

		for (int i = 0; i < 5; i++)
		{
			corrSlot[i] = Random.Range (0, 6);
		}
		Debug.Log ("[Mastermind Cruel #" + moduleId + "] The correct code is " + SlotColors[corrSlot[0]] + " " + SlotColors[corrSlot[1]] + " " + SlotColors[corrSlot[2]] + " " + SlotColors[corrSlot[3]] + " " + SlotColors[corrSlot[4]]);

		DisplaytextLeft.text = " ";
		DisplaytextRight.text = " ";

	}

	// Lights off
	void Awake () {

		Query.OnInteract += delegate () {
			handleQuery ();
			return false;
		};

		Submit.OnInteract += delegate () {
			handleSubmit ();
			return false;
		};



		for (int i = 0; i < 5; i++)
		{
			int j = i;
			Slot[i].OnInteract += delegate () {
				handleSlot (j);
				return false;
			};
		}
	}

	// Lights on
	void Activate () {

		isActive = true;
		iiSerial = Info.GetSerialNumber ();
		iiLast = Info.GetSerialNumberNumbers ().Last ();
		iiFirst = Info.GetSerialNumberNumbers ().First ();
		iiSum = Info.GetSerialNumberNumbers ().Sum ();
		iiBatteries = Info.GetBatteryCount ();
		iiBatteryHolders = Info.GetBatteryHolderCount ();
		iiLit = Info.GetOnIndicators ().Count ();
		iiUnlit = Info.GetOffIndicators ().Count ();
		iiPorts = Info.GetPortCount ();
		iiPortTypes = Info.GetPorts ().Distinct ().Count ();
		iiModules = Info.GetModuleNames ().Count ();
//		string indc = String.Join ("", Info.GetIndicators ().ToArray ());
//		iiRs = (indc.Count ("R".Contains));

	}

	void handleQuery() {

		Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, Query.transform);
		Query.AddInteractionPunch ();

		if (!isActive || isSolved)
			return;
		else {
		

			// substituting variables
			int[] PHSlotNR = {SlotNR[0], SlotNR[1], SlotNR[2], SlotNR[3], SlotNR[4]};
			Debug.Log ("[Mastermind Cruel #" + moduleId + "] Query: " + SlotColors[PHSlotNR [0]] + " " + SlotColors[PHSlotNR [1]] + " " + SlotColors[PHSlotNR [2]] + " " + SlotColors[PHSlotNR [3]] + " " + SlotColors[PHSlotNR [4]]);
			int[] PHcorrSlot = {corrSlot[0], corrSlot[1], corrSlot[2], corrSlot[3], corrSlot[4]};


			// assigning black pegs
			for (int i = 0; i < 5; i++) {
				int j = i;
				if (PHSlotNR [j] < 6) {
					if (PHSlotNR [j] == PHcorrSlot [j]) {
						PegBlack++;
						PHSlotNR [j] = 6;
						PHcorrSlot [j] = 7;
					}
				} 
			}


			// assigning white pegs
			for (int i = 0; i < 5; i++) {
				int j = i;
				if (PHSlotNR [j] < 6) {
					for (int k = 0; k < 5; k++) {
						int l = k;
						if (PHSlotNR [j] == PHcorrSlot [l]) {
							PegWhite++;
							PHSlotNR [j] = 6;
							PHcorrSlot [l] = 7;
						}
					}
				} 
			}


			// assigning gray pegs
			PegGray = (5 - PegBlack - PegWhite);

			Debug.Log ("[Mastermind Cruel #" + moduleId + "] Query result: A=" + PegBlack + ", B=" + PegWhite + ", C=" + PegGray);


			// updating display
			// add fun stuff here
			iiStrikes = Info.GetStrikes ();
			iiSolved = Info.GetSolvedModuleNames ().Count ();

			int LeftRnd = Random.Range (0, 6);
			int RightRnd = Random.Range (0, 6);

			// define color right, and mods

			if (RightRnd == 0) {
				DisplayLeftMod = iiBatteries;
				DisplayRightMod = iiSolved;
				DisplaytextRight.color = Color.white;
				RColor = "White";
			}
			if (RightRnd == 1) {
				DisplayLeftMod = iiLit;
				DisplayRightMod = iiLast;
				DisplaytextRight.color = Color.magenta;
				RColor = "Magenta";
			}
			if (RightRnd == 2) {
				DisplayLeftMod = iiSum;
				DisplayRightMod = iiPorts;
				DisplaytextRight.color = Color.yellow;
				RColor = "Yellow";
			}
			if (RightRnd == 3) {
				DisplayLeftMod = iiModules;
				DisplayRightMod = iiUnlit;
				DisplaytextRight.color = Color.green;
				RColor = "Green";
			}
			if (RightRnd == 4) {
				DisplayLeftMod = iiPortTypes;
				DisplayRightMod = iiStrikes;
				DisplaytextRight.color = Color.red;
				RColor = "Red";
			}
			if (RightRnd == 5) {
				DisplayLeftMod = iiFirst;
				DisplayRightMod = iiBatteryHolders;
				DisplaytextRight.color = Color.blue;
				RColor = "Blue";
			}

			// define color left, and sums

			if (LeftRnd == 0) {
				DisplaytextLeft.text = (PegBlack + DisplayLeftMod).ToString ();
				DisplaytextRight.text = (PegWhite + DisplayRightMod).ToString ();
				DisplaytextLeft.color = Color.white;
				LColor = "White";
			} 
			if (LeftRnd == 1) {
				DisplaytextLeft.text = (PegGray + DisplayLeftMod).ToString ();
				DisplaytextRight.text = (PegBlack+ DisplayRightMod).ToString ();
				DisplaytextLeft.color = Color.magenta;
				LColor = "Magenta";
			} 
			if (LeftRnd == 2) {
				DisplaytextLeft.text = (PegWhite + DisplayLeftMod).ToString ();
				DisplaytextRight.text = (PegGray + DisplayRightMod).ToString ();
				DisplaytextLeft.color = Color.yellow;
				LColor = "Yellow";
			} 
			if (LeftRnd == 3) {
				DisplaytextLeft.text = (PegWhite + DisplayLeftMod).ToString ();
				DisplaytextRight.text = (PegBlack + DisplayRightMod).ToString ();
				DisplaytextLeft.color = Color.green;
				LColor = "Green";
			} 
			if (LeftRnd == 4) {
				DisplaytextLeft.text = (PegGray + DisplayLeftMod).ToString ();
				DisplaytextRight.text = (PegWhite + DisplayRightMod).ToString ();
				DisplaytextLeft.color = Color.red;
				LColor = "Red";
			} 
			if (LeftRnd == 5) {
				DisplaytextLeft.text = (PegBlack+ DisplayLeftMod).ToString ();
				DisplaytextRight.text = (PegGray + DisplayRightMod).ToString ();
				DisplaytextLeft.color = Color.blue;
				LColor = "Blue";
			}

			Debug.Log ("[Mastermind Cruel #" + moduleId + "] Display shows: " + DisplaytextLeft.text + " " + LColor + ", " + DisplaytextRight.text + " " + RColor);

//			Debug.Log ("[Mastermind Cruel #" + moduleId + "] Query: " + SlotColors[PHSlotNR [0]] + SlotColors[PHSlotNR [1]] + SlotColors[PHSlotNR [2]] + SlotColors[PHSlotNR [3]] + SlotColors[PHSlotNR [4]] + 
//				"\nPegs are: " + PegBlack + " black (A), " + PegWhite + " white (B), " + PegGray + " gray (C)" +
//				"\nDisplay shows: " + DisplaytextLeft.text + " " + LColor + ", " + DisplaytextRight.text + " " + RColor);

			// reset
			PegBlack = 0;
			PegWhite = 0;
			PegGray = 0;
		}
	}

	void handleSubmit() {

		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Submit.transform);
		Submit.AddInteractionPunch ();

		if (!isActive || isSolved)
			return;
		else
			for (int i = 0; i < 5; i++) {
				int j = i;
				if (SlotNR [j] == corrSlot [j])
					PegBlack++;
			}
		if (PegBlack == 5) {
			Module.HandlePass ();
			isSolved = true;
			Debug.Log ("[Mastermind Cruel #" + moduleId + "] Submit: " + SlotColors[SlotNR [0]] + " " + SlotColors[SlotNR [1]] + " " + SlotColors[SlotNR [2]] + " " + SlotColors[SlotNR [3]] + " " + SlotColors[SlotNR [4]] + " - Code correct. Module Solved");
		} else {
			PegBlack = 0;
			Module.HandleStrike ();
			Debug.Log ("[Mastermind Cruel #" + moduleId + "] Submit: " + SlotColors[SlotNR [0]] + " " + SlotColors[SlotNR [1]] + " " + SlotColors[SlotNR [2]] + " " + SlotColors[SlotNR [3]] + " " + SlotColors[SlotNR [4]] + " - Code incorrect. Strike");
		}
					
	}

	void handleSlot(int num) {

		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Slot[num].transform);

		if (!isActive || isSolved) return;

		else 
			SlotNR[num] = SlotNR[num] + 1;
			
			if (SlotNR [num] > 5)
			SlotNR [num] = 0;
			
		Slot[num].GetComponent<MeshRenderer>().material.color = SlotColor[SlotNR[num]];

//		Debug.Log ("[Mastermind Cruel #" + moduleId + "] Slot " + (num) + " color is now " + SlotColors[corrSlot[num]]);
	}

    private string TwitchHelpMessage = "Query the currently set colors with !{0} query. Query specific colors with !{0} query r b g y m. Submit the currently set colors with !{0} submit.  Submit specific colors with !{0} submit r b g y m. [Valid colors are R, G, B, M, Y, W]";
    IEnumerator ProcessTwitchCommand(string inputCommand)
    {
        List<string> SlotColorsFull = new List<string>(SlotColors);
        SlotColorsFull.AddRange(new[] { "WHITE", "MAGENTA", "YELLOW", "GREEN", "RED", "BLUE" });

        string[] split = inputCommand.ToUpperInvariant().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if ((split.Length != 1 && split.Length != 6) || (split[0] != "QUERY" && split[0] != "SUBMIT")) yield break;
        yield return null;

        if (split.Length == 6)
        {
            for (int i = 0; i < 5; i++)
            {
                int index = SlotColorsFull.IndexOf(split[i + 1]) % 6;
                if (index < 0)
                {
                    yield return string.Format("sendtochaterror What the hell is color {0}? The only colors I recognize are: Red, Green, Blue, Yellow, Magenta, White", split[i + 1]);
                    yield break;
                }
                while (SlotNR[i] != index)
                {
                    handleSlot(i);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        if (split[0] == "QUERY")
        {
            handleQuery();
        }
        else
        {
            handleSubmit();
        }
        yield return new WaitForSeconds(0.1f);
    }
}
