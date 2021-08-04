using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MyDetails : MonoBehaviour
{
    public TextMeshProUGUI m_title;
    public TextMeshProUGUI m_total;

    public void _FillDetails(string m_category,int m_money)
    {
        m_title.text = m_category;
        m_total.text = m_money.ToString();
    }

}
