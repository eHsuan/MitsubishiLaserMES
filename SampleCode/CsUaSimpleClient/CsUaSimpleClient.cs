using System;
using System.Collections.Generic;using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using DxpUaSimpleAPI;

namespace CsUaSimpleClient
{
    class CsUaSimpleClient
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                DxpUaSimpleClass opcUA = new DxpUaSimpleClass();        // インスタンス生成
 
                // ********** 初期化処理(Directory or Windows) ********** //

                // ※先にCsUaSimpleClient.Config.xmlの変更が必要です！！
                if (!opcUA.Initialize("CsUaSimpleClient.Config.xml"))                               // 設定ファイルを読み込む
                {
                    Console.WriteLine("Failed to load file");
                    CloseApplication(opcUA);
                }
                // ********** サーバーへ接続(return Session) ********** //

                string sUrl = "opc.tcp://localhost:52210/DeviceXPlorer5/UA";                        // OPCサーバーのEndPointを指定
                SessionClient session = null;
                session = opcUA.ConnectServer(sUrl, MessageSecurityMode.None, SecurityPolicies.None);

                if (session == null)
                {
                    Console.WriteLine("Session Open Fail");
                    CloseApplication(opcUA);
                }
                opcUA.RegisterReadEventHandler(OnAsyncReadComplete);    // 非同期読出しイベントハンドラの登録
                opcUA.RegisterWriteEventHandler(OnAsyncWriteComplete);  // 非同期書込みイベントハンドラの登録

                // ********** サーバーへ接続(return bool) ********** //

                //string sUrl = "opc.tcp://localhost:52210/DeviceXPlorer5/UA";
                //if (!opcUA.Connect(sUrl, MessageSecurityMode.None, SecurityPolicies.None))
                //{
                //    Console.WriteLine("Session Open Fail");
                //    CloseApplication();
                //}
                //opcUA.RegisterReadEventHandler(OnAsyncReadComplete);    // 非同期読出しイベントハンドラの登録
                //opcUA.RegisterWriteEventHandler(OnAsyncWriteComplete);  // 非同期書込みイベントハンドラの登録
                

                // ********** ノードのブラウズ(オブジェクトフォルダor任意のノード以降をブラウズする) ********** //

                List<DxpUaSimpleClass.BrowseNodeResults> browseNodeResults = new List<DxpUaSimpleClass.BrowseNodeResults>();
                NodeId targetNodeId = ObjectIds.Server;           // サーバーオブジェクトのnodeId
                bool bString = true;                              // nodeIdがstringである場合
                if (bString)
                {
                    string id = "Device1";
                    ushort ns = 2;
                    targetNodeId = new NodeId(id, ns);
                }
                else
                {
                    // 基底のノードId等、nodeIdがuint型の時
                    uint value = 0;
                    targetNodeId = new NodeId(value);
                }

                // 実行
                if (!opcUA.BrowseNode(targetNodeId, ReferenceTypeIds.Organizes, out browseNodeResults))
                {
                    Console.WriteLine(" BrowseNode Fail," + browseNodeResults[0].m_statusCodes);
                }
                else
                {
                    if (browseNodeResults.Count() != 0)
                    {
                        if (!StatusCode.IsBad(browseNodeResults[0].m_statusCodes))
                        {
                            // 結果を処理する
                            for (int i = 0; i < browseNodeResults.Count(); i++)
                            {
                                Console.WriteLine(" BrowseResult = " + browseNodeResults[i].m_browseName + "," + browseNodeResults[i].m_nodeIdList + "<DataType = " + browseNodeResults[i].m_sDataType + "(" + browseNodeResults[i].m_sAccessLevel + ")>");
                            }
                        }
                    }
                }

				// ********** 同期読出し処理 ********** //
				List<DxpUaSimpleClass.ReadItemResults> readResultList = new List<DxpUaSimpleClass.ReadItemResults>();
				bool bSuccess = false;
				List<NodeId> readNodeIdList = new List<NodeId>();
				readNodeIdList.Add(GetNodeId("2:Device1.Tag1"));

				// 値の読み出し実行
				bSuccess = opcUA.SyncRead(readNodeIdList, out readResultList);
                
                if (bSuccess)
                {
                    for (int i = 0; i < readNodeIdList.Count(); i++)
                    {
						if (!StatusCode.IsBad(readResultList[i].m_resultCodes))
                        {
							if (readResultList[i].m_readResultValue.GetType().IsArray)
                            {
								Array castedValue = (Array)readResultList[i].m_readResultValue;
                                string sArrayValues = "";
                                foreach (object val in castedValue)
                                {
                                    if (!string.IsNullOrEmpty(sArrayValues))
                                    {
                                        sArrayValues += ",";
                                    }
                                    sArrayValues += val.ToString();
                                }
                                Console.WriteLine(" SyncRead Success," + readNodeIdList[i] + "　" + "Value = " + sArrayValues);
                            }
                            else
                            {
								Console.WriteLine(" SyncRead Success," + readNodeIdList[i] + "　" + "Value = " + readResultList[i].m_readResultValue.ToString());
                            }
                            Console.WriteLine("");

                        }
                        else
                        {
							Console.WriteLine(" SyncRead Fail," + readResultList[i].m_resultCodes);
                        }
                    }
                }


                // ********** 同期書込み処理 ********** //

                List<NodeId> writeNodeIdList = new List<NodeId>();
                List<object> writeValueList = new List<object>();
                List<DxpUaSimpleClass.WriteItemResults> writeResults = new List<DxpUaSimpleClass.WriteItemResults>();

                writeNodeIdList.Add(GetNodeId("2:Device1.Tag1"));
                short shValue = 10;
                //short[] shValue = { 1, 2, 3 };   // 配列の場合
                writeValueList.Add(shValue);

                //writeNodeIdList.Add(GetNodeId("2:Device1.Tag2"));
                //int nValue = 20;
                //writeValueList.Add(nValue);

                //writeNodeIdList.Add(GetNodeId("2:Device1.Tag3"));
                //double dbValue = 30;
                //writeValueList.Add(dbValue);

                //writeNodeIdList.Add(GetNodeId("2:Device1.Tag4"));
                //string sValue = "ab";
                //writeValueList.Add(sValue);

                // 実行
                if (opcUA.SyncWrite(writeNodeIdList, writeValueList, out writeResults))
                {
                    for (int i = 0; i < writeNodeIdList.Count(); i++)
                    {
                        if (!StatusCode.IsBad(writeResults[i].m_resultCodes))
                        {
                            Console.WriteLine(" SyncWrite Success," + writeNodeIdList[i].ToString());
                        }
                        else
                        {
                            Console.WriteLine(" SyncWrite Fail," + writeResults[i].m_resultCodes);
                        }
                    }
                }


                // ********** 非同期読み出し処理 ********** //

                List<NodeId> asyncReadNodeIdList = new List<NodeId>();
                List<object> asyncReadResultValue = new List<object>();
                List<StatusCode> asyncResultCodes = new List<StatusCode>();

                asyncReadNodeIdList.Add(GetNodeId("2:Device1.Tag1"));
                //asyncReadNodeIdList.Add(GetNodeId("2:Device1.Tag2"));
                //asyncReadNodeIdList.Add(GetNodeId("2:Device1.Tag3"));

                // 実行
                opcUA.ASyncRead(asyncReadNodeIdList);


                // ********** 非同期書込み処理 ********** //

                List<NodeId> asyncWriteNodeIdList = new List<NodeId>();
                List<object> asyncWriteValueList = new List<object>();
                // 一つ目の書込み対象を作成
                asyncWriteNodeIdList.Add(GetNodeId("2:Device1.Tag1"));
                shValue = 50;
                //shValue[0] = 50;    // 配列の場合
                asyncWriteValueList.Add(shValue);

                // 実行
                opcUA.ASyncWrite(asyncWriteNodeIdList, asyncWriteValueList);


                // ********** サブスクリプションの生成 ********** //

                string sSubscriptionName = "subscription1";
                if (opcUA.CreateSubscription(sSubscriptionName, 1000))
                {
                    Console.WriteLine(" CreateSubscription Success");
                }
                else
                {
                    Console.WriteLine(" CreateSubscription false");
                }
                opcUA.RegisterMontioredItemEventHandler(MonitoredItemNotification);         // イベントハンドラの登録
                

                // ********** 監視アイテムの追加 ********** //
                List<NodeId> NodeIdList = new List<NodeId>();
                {
                    NodeId monitoringNodeId1 = GetNodeId("2:Device1.Tag1");
                    NodeIdList.Add(monitoringNodeId1);
                }

                //{
                //    NodeId monitoringNodeId2 = GetNodeId("2:Device1.Tag2");
                //    NodeIdList.Add(monitoringNodeId2);
                //}

                // 実行
				string sErrMsg;
                if (opcUA.AddMonitoredItem(sSubscriptionName, NodeIdList, out sErrMsg))
                {
                    Console.WriteLine("Item monitoring start...Remove Item : Press ”r” key, Close the application:Press ”q” Key ");
                }
                else
                {
                    Console.WriteLine(" AddMonitoredItem false");
                }

                string removeITemKey = "";
                removeITemKey = Console.ReadLine().ToString();

                //********** 監視アイテムの削除 **********//
                if (removeITemKey == "r")
                {
                    List<NodeId> removeNodeIdList = new List<NodeId>();
                    NodeId removeMonitoredNodeId = GetNodeId("2:Device1.Tag1");
                    removeNodeIdList.Add(removeMonitoredNodeId);

                    // 実行
                    if (opcUA.RemoveMonitoredItems("subscription1", removeNodeIdList))
                    {
                        Console.WriteLine("MonitoredItem1 remove. ");
                    }

                    //List<NodeId> removeNodeIdList2 = new List<NodeId>();
                    //NodeId removeMonitoredNodeId2 = GetNodeId("2:Device1.Tag2");
                    //removeNodeIdList.Add(removeMonitoredNodeId2);
                    //// 実行
                    //if (opcUA.RemoveAllMonitoredItems("subscription1"))
                    //{
                    //    Console.WriteLine("Monitored Item all remove. ");
                    //}
                    opcUA.UnRegisterMontioredItemEventHandler(MonitoredItemNotification);       // イベントハンドラの削除
                }

                // アプリケーション終了
                string sEndKey = "";
                sEndKey = Console.ReadLine().ToString();
                if (sEndKey == "q" || session != null)
                    CloseApplication(opcUA);

            }
            catch (Exception ex)
            {
                // 例外
                Console.WriteLine("Exception Error!" + ex.Message);
            }
        }

        /// <summary>
        /// 非同期読出しのイベントハンドラ
        /// </summary>
        public static void OnAsyncReadComplete(List<string> asyncReadNodeName, List<DxpUaSimpleClass.ReadItemResults> AsyncReadItemResults)
        {
            DxpUaSimpleClass opcUA = new DxpUaSimpleClass();
            for (int i = 0; i < asyncReadNodeName.Count; i++)
            {
                if (StatusCode.IsBad(AsyncReadItemResults[i].m_resultCodes))
                {
                    Console.WriteLine("ASyncRead Fail," + asyncReadNodeName[i] + "　" + "Value = " + AsyncReadItemResults[i].m_readResultValue.ToString() + "StatusCode = " + AsyncReadItemResults[i].m_resultCodes);
                }
                else
                {
                    if (AsyncReadItemResults[i].m_readResultValue.GetType().IsArray)
                    {
                        Array castedValue = (Array)AsyncReadItemResults[i].m_readResultValue;
                        string sArrayValues = "";
                        foreach (object val in castedValue)
                        {
                            if (!string.IsNullOrEmpty(sArrayValues))
                                sArrayValues += ",";
                            sArrayValues += val.ToString();
                        }
                        Console.WriteLine(" ASyncRead Success," + asyncReadNodeName[i] + "　" + "Value = " + sArrayValues);
                    }
                    else
                    {
                        Console.WriteLine(" ASyncRead Success," + asyncReadNodeName[i] + "　" + "Value = " + AsyncReadItemResults[i].m_readResultValue.ToString());
                    }
                }
            }
            // イベントハンドラの削除
            opcUA.UnRegisterReadEventHandler(OnAsyncReadComplete);
        }

        /// <summary>
        /// 非同期書込みのイベントハンドラ
        /// </summary>
        public static void OnAsyncWriteComplete(List<string> asyncWriteNodeName, List<DxpUaSimpleClass.WriteItemResults> AsyncWriteItemResults)
        {
            DxpUaSimpleClass opcUA = new DxpUaSimpleClass();
            for (int i = 0; i < asyncWriteNodeName.Count; i++)
            {
                if (StatusCode.IsBad(AsyncWriteItemResults[i].m_resultCodes))
                {
                    Console.WriteLine(" ASyncWrite Fail," + asyncWriteNodeName[i] + "　" + "StatusCode = " + AsyncWriteItemResults[i].m_resultCodes);
                }
                else
                {
                    Console.WriteLine(" ASyncWrite Success," + asyncWriteNodeName[i]);
                }
            }
            // イベントハンドラの削除
            opcUA.UnRegisterWriteEventHandler(OnAsyncWriteComplete);
        }

        /// <summary>
        /// データ変化通知のイベントハンドラ
        /// </summary>
        public static void MonitoredItemNotification(List<string> monitoredItems, List<object> monitoredValues)
        {
            for (int ii = 0; ii < monitoredItems.Count; ii++)
            {
                if (monitoredValues[ii].GetType().IsArray)
                {
                    Array castedValue = (Array)monitoredValues[ii];
                    string sArrayValues = "";
                    foreach (object val in castedValue)
                    {
                        if (!string.IsNullOrEmpty(sArrayValues))
                            sArrayValues += ",";
                        sArrayValues += val.ToString();
                    }
                    Console.WriteLine(" ASyncRead Success," + monitoredItems[ii] + "　" + "Value = " + sArrayValues);
                }
                else
                {
                    Console.WriteLine(" ASyncRead Success," + monitoredItems[ii] + "　" + "Value = " + monitoredValues[ii].ToString());
                }
            }
        }

        /// <summary>
        /// NodeId文字列からnsとidに分割する処理
        /// </summary>
        public static NodeId GetNodeId(string sTargetNodeId)
        {
            string id = "";
            ushort ns = 0;
            if (SplitTargetNodeIdString(sTargetNodeId, out id, out ns))
            {
                NodeId targetNodeId = new NodeId(id, ns);
                return targetNodeId;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// NodeId文字列からnsとidに分割する処理
        /// </summary>
        public static bool SplitTargetNodeIdString(string sNodeId, out string id, out ushort ns)
        {
            try
            {
                // nsとidを":"で分ける処理
                string[] parts = sNodeId.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                ns = Convert.ToUInt16(parts[0]);
                id = parts[1];
                return true;
            }
            catch (Exception ex)
            {
                //NodeId分割失敗
                Console.WriteLine("SplitNodeId Fail!!" + ex.Message);
            }
            ns = 0;
            id = "";
            return false;
        }

        /// <summary>
        /// アプリケーションのクローズ処理
        /// </summary>
        public static void CloseApplication(DxpUaSimpleClass opcUA)
        {
            // ********** セッション切断 ********** //
            opcUA.DisConnect();
            Environment.Exit(100);
        }
    }
}
