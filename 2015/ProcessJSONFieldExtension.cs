using System.Text.RegularExpressions;
using System;

public static class ProcessJSONFieldExtension
{
    public static string ToStrFromJson(this JSONObject jsonObject)
    {
        string[] newString = Regex.Split(jsonObject.ToString(), "\"");
        return newString[1]; 
    }

    public static int ToIntFromJson(this JSONObject jsonObject)
    {
        return (int)Convert.ToUInt64(jsonObject.ToString());        
    }
}