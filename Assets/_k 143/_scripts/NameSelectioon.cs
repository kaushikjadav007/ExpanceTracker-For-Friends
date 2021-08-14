using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase;

public class NameSelectioon : MonoBehaviour
{

    public TMP_InputField m_name;

    private DatabaseReference m_database_reference;


    public _Names m_All_Name;

    public bool m_processing;


    private string m_name_string;


    private void OnEnable()
    {
        //StartCoroutine(_SendDataToServer());
    }

    public void _NameSelection()
    {

        m_All_Name = new _Names();
        m_All_Name.m_all_users = new List<string>();


        if (m_processing)
        {
            return;
        }

        m_processing = true;

        if (m_name.text.Length>0)
        {
            m_name_string = m_name.text;
            ServerManager.Instance.m_loading.SetActive(true);
            StartCoroutine(_GetAllNames());
        }
    }


    public IEnumerator _SendDataToServer()
    {
        Debug.Log(m_All_Name.m_all_users[0].Equals(m_name_string));

        yield return new WaitForEndOfFrame();

        if (m_All_Name.m_all_users.Count>0)
        {
            for (int i = 0; i < m_All_Name.m_all_users.Count; i++)
            {
                if (m_All_Name.m_all_users[i].Equals(m_name_string))
                {
                    Debug.Log("Same Name");
                    PlayerPrefs.SetString(_String_Data.m_user_name, m_name_string);
                    gameObject.SetActive(false);
                    ServerManager.Instance.m_loading.SetActive(false);
                    yield break;
                }
            }
        }


        m_All_Name.m_all_users.Add(m_name_string);
        PlayerPrefs.SetString(_String_Data.m_user_name, m_name_string);
        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log(JsonUtility.ToJson(m_All_Name));

        var m_task = m_database_reference.Child("Name").SetRawJsonValueAsync(JsonUtility.ToJson(m_All_Name));

        yield return new WaitUntil(() => m_task.IsCompleted);

        yield return new WaitUntil(predicate: () => m_task.IsCompleted);

        if (m_task.IsCompleted)
        {
            gameObject.SetActive(false);
            ServerManager.Instance.m_loading.SetActive(false);
            Debug.Log("COmplete");
        }
        else
        {

        }

        yield return null;
    }


    IEnumerator _GetAllNames()
    {
        _Names m_data = new _Names();

        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

        var m_getdata_task = m_database_reference.Child("Name").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {

            }

            if (task.IsCompleted)
            {
               
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.GetRawJsonValue());
                m_All_Name = JsonUtility.FromJson<_Names>(snapshot.GetRawJsonValue());
            }
        });


        yield return new WaitUntil(() => m_getdata_task.IsCompleted);


        if (m_getdata_task.IsCompleted)
        {
            Debug.Log(JsonUtility.ToJson(m_All_Name));

            Debug.Log(m_name_string);
            StartCoroutine(_SendDataToServer());
            Debug.Log("Names Returned");
        }
        else
        {

        }





        yield return null;

    }



}
