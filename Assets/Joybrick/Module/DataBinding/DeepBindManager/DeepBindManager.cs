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

        public string GetLocalization(string request,params object[] replaceList)
        {
            if (string.IsNullOrEmpty(request))
                return "";

            string trueRequest = (request.IndexOf(".") < 0 && !request.StartsWith("{")) ? $"{{#.{request}}}" : request;

            var req = GetRequestResult(trueRequest);
            if (req == null)
                return request;

            var message = req.ToString();
            if (replaceList != null)
            {
                for (int i = 0; i < replaceList.Length; i += 2)
                    message = message.Replace(replaceList[i].ToString(), replaceList[i + 1].ToString());
            }
            return message;
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