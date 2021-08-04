using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class _Names
{
    public List<string> m_all_users;
}

[System.Serializable]
public class _UsersData
{
    public string m_name;
    public _PersonalData m_personal_data;
}

[System.Serializable]
public class _PersonalData
{
    public List<_Expance_Data> m_expance_data;
}

[System.Serializable]
public class _Expance_Data
{
    public string m_type;
    public string m_type_of_purchase;
    public int m_price;
}


[System.Serializable]
public enum _Type_Expance
{
    m_vegetables,
    m_petrol,
    m_stationory,
    m_grocery,
    m_dairy,
    m_cloths,
    m_personal,
    m_other
}

public class _String_Data
{
    public static string m_user_name= "m_user_name";
    //public static string m_unique_no = "m_unique_no";
    public static string m_month_no = "m_month_no";
    public static string m_my_expance_json = "m_my_expance_json";
}
