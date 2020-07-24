using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UniRx;
using System.Linq;

namespace Joybrick
{
    public class DeepBindManager
    {
        public static DeepBindManager Instance;
        DataBindingManager bindingManager;
        DeepBindVarable _tmp;

        public DeepBindManager(DataBindingManager bindingMgr)
        {
            this.bindingManager = bindingMgr;
            if (Instance == null)
                Instance = this;
        }

        public DeepBindVarable Request(string request)
        {
            DeepBindVarable result = new DeepBindVarable(request, bindingManager);
            return result;
        }

        public object GetRequestResult(string request)
        {
            if (_tmp == null)
                _tmp = new DeepBindVarable("", bindingManager);
            _tmp.SetReuest(request);
            object result =  _tmp.Value;
            _tmp.Dispose();
            return result;
        }
    }
}