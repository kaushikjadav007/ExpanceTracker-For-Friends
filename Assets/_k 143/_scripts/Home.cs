using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{

    [Header("PANELS")]
    public GameObject m_home_panel;
    public GameObject m_total_expance_palnel;
    public GameObject m_add_expance_panel;
    public GameObject m_name_panel;

    [Space]
    public Button m_add_expance;
    public Button m_total_expance;

    [Space]
    public Button m_back_from_add;
    public Button m_back_from_total;

    private DependencyStatus m_dependancy_status;

    public static Home Instance;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        m_add_expance.onClick.AddListener(_OpenAddPanel);
        m_total_expance.onClick.AddListener(_OpenTotalExpancePanel);
        m_back_from_add.onClick.AddListener(_BackFromAddPanel);
        m_back_from_total.onClick.AddListener(_BackFromTotal);
    }

    private void OnDisable()
    {
        m_add_expance.onClick.RemoveListener(_OpenAddPanel);
        m_total_expance.onClick.RemoveListener(_OpenTotalExpancePanel);
        m_back_from_add.onClick.RemoveListener(_BackFromAddPanel);
        m_back_from_total.onClick.AddListener(_BackFromTotal);
    }


    private IEnumerator Start()
    {

        ServerManager.Instance.m_loading.SetActive(true);    

        if (PlayerPrefs.GetString(_String_Data.m_user_name) != "")
        {
            m_name_panel.SetActive(false);
        }
        else
        {
            m_name_panel.SetActive(true);
        }

        var m_task = FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            m_dependancy_status = task.Result;

            if (m_dependancy_status == DependencyStatus.Available)
            {
                Debug.Log("Initilized");
            }
            else
            {
                Debug.Log("Dpendancy not available");
            }
        });

        yield return new WaitUntil(predicate: () => m_task.IsCompleted);

        ServerManager.Instance.m_loading.SetActive(false);
    }

    void _OpenAddPanel()
    {
        m_home_panel.SetActive(false);
        m_add_expance_panel.SetActive(true);
    }

    void _OpenTotalExpancePanel()
    {
        m_home_panel.SetActive(false);
        m_total_expance_palnel.SetActive(true);
    }

    void _BackFromAddPanel()
    {
        m_add_expance_panel.SetActive(false);
        m_home_panel.SetActive(true);
    }


    void _BackFromTotal()
    {
        m_total_expance_palnel.SetActive(false);
        m_home_panel.SetActive(true);
    }

}
