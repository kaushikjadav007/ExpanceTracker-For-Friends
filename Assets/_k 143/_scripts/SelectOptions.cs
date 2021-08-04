using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectOptions : MonoBehaviour
{

    public GameObject m_obj;
    [Space]
    public _Type_Expance m_type_e;

    [Space]
    public ServerManager m_sv_manager;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(_SetType);
    }

    private void _SetType()
    {

        m_sv_manager._SetExpanceType(GetComponentInChildren<TextMeshProUGUI>().text);
       // m_sv_manager.m_type = m_type_e;
        m_obj.SetActive(false);
    }
}
