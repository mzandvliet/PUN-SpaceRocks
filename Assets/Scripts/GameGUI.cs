using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour {
    private void Start() {
        StatusGUI.Instance.SetStatus("Round started!");
    }
}
