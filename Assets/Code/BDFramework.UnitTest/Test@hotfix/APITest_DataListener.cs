﻿using System;
using BDFramework.DataListener;
using BDFramework.UnitTest;
using UnityEngine;

namespace Tests
{
    [HotfixTest(Des = "数据监听测试")]
    static public class DataListener
    {
        public enum Msg
        {
            test,
        }

        [HotfixTest(Des = "添加监听测试")]
        public static void AddListener()
        {
            int count        = 0;
            int compareValue = 100;
            var service      = DataListenerServer.Create(nameof(Msg.test));
            service.AddData(Msg.test);

            service.AddListener(Msg.test, (o) =>
            {
                //每次自增
                count++;
            });
            for (int i = 0; i < compareValue; i++)
            {
                service.TriggerEvent(Msg.test);
            }

            DataListenerServer.DelService(nameof(Msg.test));
            Assert.Equals(count, 100);
        }


        public class Msg_ParamTest
        {
            public int test1 = 1;
            public int test2 = 2;
        }

        [HotfixTest(Des = "参数类型测试")]
        public static void AddListener_ParamType()
        {
            int count   = 0;
            var service = DataListenerServer.Create(nameof(Msg.test));
            service.AddData(Msg.test);
            service.AddListener<Msg_ParamTest>(Msg.test, triggerNum: 10, action: (o) =>
            {
                //每次自增
                Debug.Log("直接接受类型 p1 :" + o.test1);
                Debug.Log("直接接受类型 p2 :" + o.test2);
            });
            
            service.AddListener(Msg.test, triggerNum: 10, action: (o) =>
            {
                var _o = o as Msg_ParamTest;
                //每次自增
                Debug.Log("param1:" + _o.test1);
                Debug.Log("param2:" + _o.test2);
            });
            
            service.TriggerEvent(Msg.test, new Msg_ParamTest());
            
            DataListenerServer.DelService(nameof(Msg.test));
            Assert.IsPass(true);
        }

        [HotfixTest(Des = "触发次数测试")]
        public static void AddListener_TriggerCount()
        {
            int count   = 0;
            var service = DataListenerServer.Create(nameof(Msg.test));
            service.AddData(Msg.test);
            service.AddListener(Msg.test, triggerNum: 10, action: (o) =>
            {
                //每次自增
                count++;
            });


            //次数到了之后不会再执行
            for (int i = 0; i < 20; i++)
            {
                service.TriggerEvent(Msg.test);
            }

            DataListenerServer.DelService(nameof(Msg.test));
            Assert.Equals(count, 10);
        }

        [HotfixTest(Des = "添加顺序测试")]
        public static void AddListener_Order()
        {
            int count   = 0;
            int count2  = 0;
            int count3  = 0;
            var service = DataListenerServer.Create(nameof(Msg.test));
            service.AddData(Msg.test);
            service.AddListener(Msg.test, order: 10, action: (o) =>
            {
                //每次自增
                count2++;
            });
            service.AddListener(Msg.test, order: 1, action: (o) =>
            {
                //每次自增
                if (count2 == 0)
                {
                    count++;
                }
            });
            service.AddListener(Msg.test, order: 11, action: (o) =>
            {
                //每次自增
                if (count == 1 && count2 == 1)
                {
                    count3++;
                }
            });

            //次数到了之后不会再执行
            service.TriggerEvent(Msg.test);
            DataListenerServer.DelService(nameof(Msg.test));
            //两个必须等于1
            Assert.Equals(count, 1);
            Assert.Equals(count2, 1);
            Assert.Equals(count3, 1);
        }


        [HotfixTest(Des = "删除监听测试")]
        public static void DeleteListener()
        {
            int count   = 0;
            var service = DataListenerServer.Create(nameof(Msg.test));
            service.AddData(Msg.test);
            Action<object> callback = (o) =>
            {
                //每次自增
                count++;
            };
            //初始化数据
            service.AddListener(nameof(Msg.test), triggerNum: 10, order: 10, callback: callback);

            //测试
            for (int i = 0; i < 10; i++)
            {
                service.TriggerEvent(Msg.test);
                service.RemoveListener(Msg.test, callback);
            }

            DataListenerServer.DelService(nameof(Msg.test));
            //验证结果
            Assert.Equals(count, 1);
        }
    }
}