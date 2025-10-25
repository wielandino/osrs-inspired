using System.Collections;
using TMPro;
using UnityEngine;

public class XPBar : MonoBehaviour
{
    [SerializeField] private GameObject _xpPanel;
    [SerializeField] private TextMeshProUGUI _currentXP;
    [SerializeField] private TextMeshProUGUI _neededXP;

    public void DisplayXPBar(SkillType skillType, string currentXP, string neededXP)
    {
        _xpPanel.SetActive(true);
        //Todo show skilltype icon
        _currentXP.text = currentXP;
        _neededXP.text = neededXP;
        StartCoroutine(ShowXPBar());
    }

    private IEnumerator ShowXPBar()
    {
        yield return new WaitForSeconds(5);
        _xpPanel.SetActive(false);
        yield break;
    }
}