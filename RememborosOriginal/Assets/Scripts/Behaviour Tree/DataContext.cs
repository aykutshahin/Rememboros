using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContext : MonoBehaviour
{
    //Actions and Conditions get data they need from this class
    protected Hashtable data = new Hashtable(); 

    //Add an element with a key and a value
    public void add(string key, object value)
    {     
        data.Add(key, value);
    } 

    //Get the value by key
    public object get(string key)
    {
        return data[key];      
    } 

    //Remove the value by key and return the removed value
    public object remove(string key)
    {
        object tmp = data[key];
        data.Remove(key); 
        return tmp;
    } 

    //Return true if key exists in the data otherwise return false
    public bool containsKey(string key)
    {
        return data.ContainsKey(key);
    } 

    //Return true if value exists in the data otherwise return false
    public bool containsValue(object value)
    {
        return data.ContainsValue(value);
    } 

    //Clear the data
    public void clear()
    {
        data.Clear();
    } 

    //Returns a shallow copy of the data
    public object clone()
    {
        return data.Clone();
    } 

    //Returns a deep copy of the data
    public Hashtable realClone()
    {
        Hashtable realClone = new Hashtable();
        foreach(string key in data){
            realClone.Add(key, data[key]);
        }
        return realClone;
    }
}
