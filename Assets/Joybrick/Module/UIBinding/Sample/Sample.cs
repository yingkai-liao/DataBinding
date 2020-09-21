using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joybrick;
using UniRx;
using Cysharp.Threading.Tasks;

public class PropertySource1
{
    [DataBind("IntValue")]
    IntProperty IntBindingValue = new IntProperty(0);
    [DataBind("AnotherValue")]
    StringProperty AnotherValue = new StringProperty("testValue");

    [DataBind("Int2String")]
    List<string> ListBinding = new List<string>() {
        "剪刀","石頭","布"
    };

    public void RandomIntValue()
    {
        IntBindingValue.Value++;
        if (IntBindingValue.Value > 2)
            IntBindingValue.Value = 0;
    }
}

public class PropertySource2
{
    [DataBind("IntValue")]
    IntProperty IntBindingValue = new IntProperty(0);

    [DataBind("Int2String")]
    List<string> ListBinding = new List<string>() {
        "棒","老虎","雞","蟲"
    };

    public void RandomIntValue()
    {
        IntBindingValue.Value++;
        if (IntBindingValue.Value > 3)
            IntBindingValue.Value = 0;
    }
}

public class DynamicSource : IDynamicDataSource
{
    ListProperty<StringProperty> randomString = new ListProperty<StringProperty>();
    List<string> data = new List<string>() { "1", "2", "3", "4", "5" };

    public object GetValue(string key)
    {
        if (int.TryParse(key, out int index) && index < data.Count)
            return data[index];
        return null;
    }
}

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    PropertySource1 s1 = new PropertySource1();
    PropertySource2 s2 = new PropertySource2();
    DynamicSource dynamic = new DynamicSource();

    private void Awake()
    {
        DataBindingManager dataBindingManager = DataBindingManager.Instance;
        DeepBindManager deepBindManager = new DeepBindManager(dataBindingManager);

        dataBindingManager.SetSource("DataSet", s1);
        dataBindingManager.SetSource("ListData", dynamic);

        dataBindingManager.Subscribe("UISample.ToS1", ToS1); ;
        dataBindingManager.Subscribe("UISample.ToS2", ToS2);
        dataBindingManager.Subscribe("UISample.Random", RandS);

        Dictionary<string, object> JsonData = new Dictionary<string, object>();
        JsonData["Name"] = "TestData";
        var data = new Dictionary<string, object>();
        JsonData["Data"] = data;
        data["Age"] = 16;
        data["ATK"] = 80;
        data["Love"] = "Lucy";
        DataBindingManager.Instance.SetSource("Json", JsonData);
    }
    
    void ToS1(object newValue)
    {
        DataBindingManager.Instance.SetSource("DataSet", s1);
    }

    void ToS2(object newValue)
    {
        DataBindingManager.Instance.SetSource("DataSet", s2);
    }

    void RandS(object newValue)
    {
        s1.RandomIntValue();
        s2.RandomIntValue();
    }
}
