using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExpanceDevider : MonoBehaviour
{

    public _Names m_names;
    [Space]
    public List<_UsersData> m_all_data;
    [Space]

    [Header("UI SETUP")]
    [Space]
    public TextMeshProUGUI m_title;
    public GameObject m_personal_panel;
    public Transform m_content;
    [Space]
    public GameObject m_data_prefab;

    [Space]

    public List<_Expance_Data> m_gotten_data;


    private int m_year;
    private int m_month;
    private DatabaseReference m_database_reference;
    private string m_name;
    private bool m_got_all_name;

    [Space]
    private bool m_personal_type_process;

    private int m_incrimental_count;

    private List<GameObject> m_obj;

    private void Start()
    {
       // m_names = JsonUtility.FromJson<_Names>(m_s);
    }

    private void OnEnable()
    {

        m_obj = new List<GameObject>();

        ServerManager.Instance.m_loading.SetActive(true);

        StartCoroutine(_GetAllNames(m_callbacl =>
        {
            Debug.Log(m_callbacl);
           
        }));
    }

    public void _GetMyExpance()
    {

        if (m_obj.Count>0)
        {
            for (int i = 0; i < m_obj.Count; i++)
            {
                Destroy(m_obj[i].gameObject);
            }
        }
        ServerManager.Instance.m_loading.SetActive(true);
        m_personal_type_process = true;
        m_name = PlayerPrefs.GetString(_String_Data.m_user_name);
        m_all_data = new List<_UsersData>();
        StartCoroutine(_GetData(m_name));
    }

    public void _GetAllUserExpance()
    {

        if (m_obj.Count > 0)
        {
            for (int i = 0; i < m_obj.Count; i++)
            {
                Destroy(m_obj[i].gameObject);
            }
        }
        ServerManager.Instance.m_loading.SetActive(true);
        m_personal_type_process =false;
        m_all_data = new List<_UsersData>();
        StartCoroutine(_GetAllUserDataAndWait());
    }

    IEnumerator _GetAllNames(System.Action<bool> m_got_em_all)
    {
        m_got_all_name = false;

        m_year = System.DateTime.Now.Year;
        m_month = PlayerPrefs.GetInt(_String_Data.m_month_no);

        _Names m_data = new _Names();

        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

        var m_getdata_task = m_database_reference.Child("Name").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

            }

            if (task.IsCompleted)
            {
                m_got_all_name = true;
                Debug.Log("Data Retrived");
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.GetRawJsonValue());
                m_names = JsonUtility.FromJson<_Names>(snapshot.GetRawJsonValue());
            }
        });

        yield return new WaitUntil(() => m_getdata_task.IsCompleted);

         ServerManager.Instance.m_loading.SetActive(false);

        yield return m_got_all_name;

    }

    IEnumerator _GetAllUserDataAndWait()
    {

        int m_c = m_names.m_all_users.Count;
        m_incrimental_count=0;

        for (int i = 0; i < m_names.m_all_users.Count; i++)
        {
            StartCoroutine(_GetData(m_names.m_all_users[i]));
            yield return new WaitForSecondsRealtime(1f);
        }

        while (m_incrimental_count!=m_c)
        {
            Debug.Log(m_incrimental_count);
            yield return null;
        }

        StartCoroutine(_SetUpToalData());
    }

    IEnumerator _SetUpToalData()
    {
        m_gotten_data.Clear();
        Debug.Log("OPEN PERSONAL PANEL");

        m_title.text = "TOTAL";

        if (m_all_data.Count > 0)
        {

            for (int j = 0; j < m_all_data.Count; j++)
            {

                if (m_all_data[j].m_personal_data.m_expance_data.Count>0)
                {   
                    for (int i = 0; i < m_all_data[j].m_personal_data.m_expance_data.Count; i++)
                    {
                        _Expance_Data m_e = m_all_data[j].m_personal_data.m_expance_data[i];

                        if (m_gotten_data.Exists(asd => asd.m_type == m_e.m_type))
                        {
                            int m_indexx = m_gotten_data.FindIndex(asd => asd.m_type == m_e.m_type);
                            m_gotten_data[m_indexx].m_price += m_e.m_price;
                        }
                        else
                        {
                            m_gotten_data.Add(m_e);
                        }

                        yield return null;
                    }
                }

            }
        }

        yield return null;

        m_personal_panel.SetActive(true);

        if (m_gotten_data.Count > 0)
        {
            for (int i = 0; i < m_gotten_data.Count; i++)
            {
                GameObject Go = Instantiate(m_data_prefab);
                m_obj.Add(Go);
                Go.transform.SetParent(m_content);
                Go.GetComponent<MyDetails>()._FillDetails(m_gotten_data[i].m_type, m_gotten_data[i].m_price);

            }
        }

        ServerManager.Instance.m_loading.SetActive(false);
    }

    IEnumerator _GetData(string m_nnnn)
    {
        m_year = System.DateTime.Now.Year;
        m_month = PlayerPrefs.GetInt(_String_Data.m_month_no);

        _PersonalData m_data = new _PersonalData();

        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

        var m_getdata_task = m_database_reference.Child(m_year.ToString()).Child(m_month.ToString()).Child(m_nnnn).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

            }

            if (task.IsCompleted)
            {
                
                Debug.Log("Data Retrived");
                DataSnapshot snapshot= task.Result;
                Debug.Log(snapshot.GetRawJsonValue());
                m_data = JsonUtility.FromJson<_PersonalData>(snapshot.GetRawJsonValue());
            }
        });

        yield return new WaitUntil(() => m_getdata_task.IsCompleted);

        _UsersData m_d = new _UsersData();
        m_d.m_name = m_nnnn;
        m_d.m_personal_data =m_data;
        m_all_data.Add(m_d);

        yield return null;

        m_incrimental_count++;

        if (m_personal_type_process)
        {
            m_gotten_data.Clear();
            Debug.Log("OPEN PERSONAL PANEL");

            m_title.text = m_name;

            if (m_all_data.Count > 0)
            {
                for (int i = 0; i < m_all_data[0].m_personal_data.m_expance_data.Count; i++)
                {
                    _Expance_Data m_e = m_all_data[0].m_personal_data.m_expance_data[i];


                    if (m_gotten_data.Exists(asd => asd.m_type == m_e.m_type))
                    {
                        int m_indexx = m_gotten_data.FindIndex(asd => asd.m_type == m_e.m_type);
                        m_gotten_data[m_indexx].m_price += m_e.m_price;
                    }
                    else
                    {
                        m_gotten_data.Add(m_e);
                    }

                    yield return null;
                }
            }

            yield return null;

            m_personal_panel.SetActive(true);

            if (m_gotten_data.Count > 0)
            {
                for (int i = 0; i < m_gotten_data.Count; i++)
                {
                    GameObject Go = Instantiate(m_data_prefab);
                    m_obj.Add(Go);
                    Go.transform.SetParent(m_content);
                    Go.GetComponent<MyDetails>()._FillDetails(m_gotten_data[i].m_type, m_gotten_data[i].m_price);
                }
            }

            ServerManager.Instance.m_loading.SetActive(false);
        }


    }

}
