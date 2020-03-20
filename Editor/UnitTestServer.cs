using Grpc.Core;
using TridentUnitTest;
//using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace GrpcServer
{
    class GrpcImpl : UnitTestService.UnitTestServiceBase
    {

        public override Task<TestReply> UnitTest(TestRequest request, ServerCallContext context)
        {
            bool finishFlag = false;

            MainThreadUtility.Instance.EnqueueAction(delegate
            {
                UnitTestServer.TestCall();

            });
            while (!finishFlag)
            {
                if (UnitTestServer.IsEnd())
                {
                    finishFlag = true;
                }
            }

            return Task.FromResult(new TestReply { Message = "test reply " + request.Name + "," + UnitTestServer.GetResultDes() });
        }

        
    }

    public class UnitTestServer
    {


        static StringBuilder testResultDes = new StringBuilder();
        static bool isResultEnd = false;

        public static string GetResultDes()
        {
            return testResultDes.ToString();
        }

        public static bool IsEnd()
        {
            return isResultEnd;
        }

        public static void TestCall()
        {
            testResultDes.Clear();
            isResultEnd = false;

            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            testRunnerApi.RegisterCallbacks(new MyCallbacks());
            var filter = new Filter()
            {
                testMode = TestMode.EditMode
            };
            testRunnerApi.Execute(new ExecutionSettings(filter));
        }


        private class MyCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {

            }

            public void RunFinished(ITestResultAdaptor result)
            {
                isResultEnd = true;
            }

            public void TestStarted(ITestAdaptor test)
            {

            }

            public void TestFinished(ITestResultAdaptor result)
            {

                if (!result.HasChildren && result.ResultState != "Success")
                {
                    string resultStr = string.Format("Test {0} {1}", result.Test.Name, result.ResultState);
                    testResultDes.Append(resultStr);
                }
            }


        }

        static Server server;
        const int Port = 50053;
        [MenuItem("Test/RunServer")]
        [InitializeOnLoadMethod]
        public static void Main()
        {
            server = new Server
            {
                Services = { UnitTestService.BindService(new GrpcImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Debug.Log("server listening on port " + Port);
            

        }

        [MenuItem("Test/ShutDownServer")]
        public static void ShutDownServer()
        {
            if (server == null)
            {
                return;
            }

            server.ShutdownAsync().Wait();
            server = null;
            Debug.Log("Unity service server shutdown successfully.");
        }


        static Server server1;
        const int Port1 = 50054;
        [MenuItem("Test/RunServer1")]
        [InitializeOnLoadMethod]
        public static void Other()
        {
            server1 = new Server
            {
                Services = { UnitTestService.BindService(new GrpcImpl()) },
                Ports = { new ServerPort("localhost", Port1, ServerCredentials.Insecure) }
            };

            server1.Start();
            Debug.Log("server listening on port " + Port1);


        }

        [MenuItem("Test/ShutDownServer1")]
        public static void ShutDownServer1()
        {
            if (server1 == null)
            {
                return;
            }

            server1.ShutdownAsync().Wait();
            server1 = null;
            Debug.Log("Unity service1 server shutdown successfully.");
        }
    }
}
