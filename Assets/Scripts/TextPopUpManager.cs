using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopUpManager : MonoBehaviour
{
    public GameObject comboTextPopUp;
    public GameObject gridScoretextPopUp;

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
        comboTextPopUp.SetActive(true);
        comboTextPopUp.GetComponent<TextMeshProUGUI>().text = "COMBO x" + number.ToString() + "!";
        yield return new WaitForSeconds(1f);
        comboTextPopUp.SetActive(false);
    }

    public void TextGridScorePopUp(int number, Transform pos)
    {
        StartCoroutine(TextGridScorePopUpCoroutine(number, pos));
    }

    IEnumerator TextGridScorePopUpCoroutine(int number, Transform pos)
    {
        GameObject popUp = Instantiate(gridScoretextPopUp, gameObject.transform, true);
        popUp.GetComponent<TextMeshProUGUI>().text = "+" + number.ToString();
        popUp.transform.position = pos.position;
        Debug.Log("popUp: " + popUp.transform.position + " | " + "gridSquare: " + pos.position);
        yield return new WaitForSeconds(0.75f);
        Destroy(popUp);
    }

}
