using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopUpManager : MonoBehaviour
{
    public GameObject textPopUp;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            TextComboPopUp(36);
        }
    }

    private void OnEnable()
    {
        GameEvents.ComboActivate += TextComboPopUp;
    }
    private void OnDisable()
    {
        GameEvents.ComboActivate -= TextComboPopUp;
    }

    public void TextComboPopUp(int number)
    {
        StartCoroutine(TextComboPopUpCoroutine(number));
    }

    IEnumerator TextComboPopUpCoroutine(int number)
    {
        textPopUp.SetActive(true);
        textPopUp.GetComponent<TextMeshProUGUI>().text = "COMBO x" + number.ToString() + "!";
        yield return new WaitForSeconds(1f);
        textPopUp.SetActive(false);
    }

}
