using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;

public class NameSelectioon : MonoBehaviour
{

    public TMP_InputField m_name;

    private DatabaseReference m_database_reference;


    public _Names m_All_Name;

    public bool m_processing; 

    private void OnEnable()
    {

    }

    public void _NameSelection()
    {
        if (m_processing)
        {
            return;
        }

        m_processing = true;

        if (m_name.text.Length>0)
        {

            ServerManager.Instance.m_loading.SetActive(true);


            StartCoroutine(_GetAllNames());
        }
    }

    public IEnumerator _SendDataToServer()
    {
        Debug.Log("Working");

        m_database_reference = FirebaseDatabase.DefaultInstance.RootReference;

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

        m_All_Name.m_all_users.Add(m_name.text);
        PlayerPrefs.SetString(_String_Data.m_user_name, m_name.text);
        StartCoroutine(_SendDataToServer());


        Debug.Log("Names Returned");

        yield return null;

    }



}
