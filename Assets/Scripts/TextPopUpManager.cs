using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextPopUpManager : MonoBehaviour
{
    public GameObject comboTextPopUp;
    public GameObject gridScoretextPopUp;
    public GameObject cheerUpPopUp;
    public List<CheerUpPopUpData> cheerUpPopUpData = new List<CheerUpPopUpData>();

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            TextComboPopUp(36);
            CheerUpPopUp(1);
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
        yield return new WaitForSeconds(0.75f);
        Destroy(popUp);
    }

    public void CheerUpPopUp(int comboIndex)
    {
        StartCoroutine(CheerUpPopUpCoroutine(comboIndex));
    }

    IEnumerator CheerUpPopUpCoroutine(int comboIndex)
    {
        if(comboIndex != 0)
        {
            if(comboIndex - 1 < cheerUpPopUpData.Count)
            {
                cheerUpPopUp.GetComponent<Image>().sprite = cheerUpPopUpData[comboIndex - 1].sprite;
                Debug.Log(comboIndex);
            }
            else
            {
                cheerUpPopUp.GetComponent<Image>().sprite = cheerUpPopUpData[cheerUpPopUpData.Count - 1].sprite;
            }
        }
        else
        {
            yield return null;
        }
        cheerUpPopUp.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        cheerUpPopUp.SetActive(false);
    }

}
