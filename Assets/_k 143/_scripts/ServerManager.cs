using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;

public class ServerManager : MonoBehaviour
{
    public Button m_select;
    public TMP_InputField m_details;
    public TMP_InputField m_moeny;

    [Space]
    public GameObject m_select_options;
    public GameObject m_loading;
    [Space]
    public TextMeshProUGUI m_type_text;
    public TextMeshProUGUI m_warning;

    [Space]
    public string m_name;

    [Space]
    public _Expance_Data m_expance_data;
    [Space]
    public _PersonalData m_p_data;


    [Space]
    public string m_json;

    [Space]
    public bool m_test;




    private int m_year;
    private int m_month;
    private _Type_Expance m_type;
    private DependencyStatus m_dependancy_status;
    private DatabaseReference m_database_reference;



    public static ServerManager Instance;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }    
    }

    private IEnumerator Start()
    {
        if (m_test)
        {
            PlayerPrefs.DeleteAll();
        }

        m_p_data = new _PersonalData();
        m_p_data.m_expance_data = new List<_Expance_Data>();

        m_month = PlayerPrefs.GetInt(_String_Data.m_month_no);

        if (m_month!=System.DateTime.Now.Month)
        {
            PlayerPrefs.SetInt(_String_Data.m_month_no, System.DateTime.Now.Month);
            m_month = PlayerPrefs.GetInt(_String_Data.m_month_no);
            PlayerPrefs.SetString(_String_Data.m_my_expance_json,"");
            m_json= PlayerPrefs.GetString(_String_Data.m_my_expance_json);
        }
        else
        {
            m_json = PlayerPrefs.GetString(_String_Data.m_my_expance_json);
        }


        if (m_json.Length>0)
        {
          m_p_data = JsonUtility.FromJson<_PersonalData>(m_json);
        }


        m_year = System.DateTime.Now.Year;

        m_name = PlayerPrefs.GetString(_String_Data.m_user_name);


        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void _SelectType()
    {
        m_select_options.SetActive(true);
    }

    public void _SetExpanceType(string m_s)
    {
        m_type_text.text = m_s;
    }

    public void _SetDataNow()
    {

        if (m_type_text.text == "SELECT")
        {
            m_warning.text = "SELECT TYPE OF EXPANCE";
            m_warning.gameObject.SetActive(true);
            return;
        }


        if (m_moeny.text == "")
        {
            m_warning.text = "ADD MONEY";
            m_warning.gameObject.SetActive(true);
            return;
        }

        m_warning.gameObject.SetActive(false);


        m_expance_data.m_type = m_type_text.text;
        m_expance_data.m_price = int.Parse(m_moeny.text);
        m_expance_data.m_type_of_purchase = m_details.text;


        m_p_data.m_expance_data.Add(m_expance_data);

        m_json = JsonUtility.ToJson(m_p_data);

        m_name = PlayerPrefs.GetString(_String_Data.m_user_name);

        m_loading.SetActive(true);
        PlayerPrefs.SetString(_String_Data.m_my_expance_json, m_json);
        PlayerPrefs.Save();
        StartCoroutine(_SendDataToServer());

    }

    public IEnumerator _SendDataToServer()
    {
        Debug.Log("Working");

        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

        var m_task = m_database_reference.Child(m_year.ToString()).Child(m_month.ToString()).Child(m_name).SetRawJsonValueAsync(m_json);

        yield return new WaitUntil(() => m_task.IsCompleted);

        yield return new WaitUntil(predicate: () => m_task.IsCompleted);

        if (m_task.IsCompleted)
        {

            Debug.Log("COmplete");
            m_type_text.text = "SELECT";
            m_moeny.text = "ENTER PRICE";
            m_details.text = "ENTER DETAILS";
            Home.Instance.m_home_panel.SetActive(true);
            Home.Instance.m_add_expance_panel.SetActive(false);
            m_loading.SetActive(false);
        }
        else
        {


        }

        yield return null;
    }

    public void _DOGetAllData()
    {
        StartCoroutine(_GetData());
    }

     IEnumerator _GetData()
    {
        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

        var m_getdata_task = m_database_reference.Child(m_year.ToString()).Child(m_month.ToString()).Child(m_name).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Data Retrived");

                m_json = snapshot.GetRawJsonValue();

                m_p_data = JsonUtility.FromJson<_PersonalData>(m_json);

            }
        });

        yield return new WaitUntil(() => m_getdata_task.IsCompleted);

    }
}
