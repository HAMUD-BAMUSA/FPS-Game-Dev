using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAmmo : MonoBehaviour
{
    public static int handgunAmmoCount = 20;
    [SerializeField] GameObject ammoDisplay;

    void Update()
    {
        ammoDisplay.GetComponent<TMPro.TMP_Text>().text = "" + handgunAmmoCount; //nothing + val in handgun ammo 
    }

    
}
