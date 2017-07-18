using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Ais.Net.Radius.Attributes;
using System.Diagnostics;
using SharpPcap;
using System.Xml;
using System.IO;
using DBM;

namespace Ais.Net.Radius.Example
{
    class Program
    {
        static string transid;
        private static DBConnectX dbc;
        private static int ranNum;
        private static int ranNum1;
        private static int ranNum2;
        private static int ranNum3;
        private static int ranNum4;
        private static int ranNum5;
        static void Main(string[] args)
        {
            dbc = new DBConnectX("1.1.1.1", "dhcp");
            string mode = getConfig("mode");
            string hostname = getConfig("hostname");
            string srcip = getConfig("srcip");
            string dstip = getConfig("dstip");
            string srcmac = getConfig("srcmac");
            string dstmac = getConfig("dstmac");
            string counter = getConfig("counter");
            string sum = getConfig("sum");
            ranNum = 255;
            ranNum1 = 255;
            ranNum2 = 255;
            ranNum3 = 255;
            ranNum4 = 255;
            ranNum5 = 255;
            /*
            Console.WriteLine("nWelcome to DNS FLUSH .NET demo!");
            Console.WriteLine("================================n");

            Console.WriteLine("ActMode(1.固定IP随机域名 2.固定IP固定域名 3.随机IP随机域名 4.随机IP固定域名):");
            mode = "4";

            if (mode=="2" || mode=="1")
            {
                Console.WriteLine("源IP地址：");
                srcip = Console.ReadLine();
            }

            if (mode=="2" || mode=="4")
            {
                Console.WriteLine("指定域名：");
                hostname = "www.baidu.com";//Console.ReadLine();
            }

            Console.WriteLine("目标服务器IP：");
            dstip = "218.108.248.201";//Console.ReadLine();

            Console.WriteLine("目标MAC地址(ee-ee-ee-ee-ee-ee)：");
            dstmac = "5c-dd-70-15-7a-4d";//Console.ReadLine();

            **/
            var devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i_device = 0;

            // Print out the available devices
            foreach (var dev in devices)
            {
                Console.WriteLine("{0}) {1}", i_device, dev.Description);
                i_device++;
            }
            
            Console.WriteLine();
            Console.Write("-- Please choose a device to send a packet on: ");
            i_device = int.Parse(Console.ReadLine());

            var device = devices[i_device];

            //byte[] finalpacket = BuilderPacket();

            if (mode == "1")
            {
                for (int i = 0; i < 255; i++)
                {
                    for (int k = 0; k < 255; k++) // k代表 多少个1ms  即总个数/3
                    {
                        for (int l = 0; l < 255; l++) //小于等于3000/s 速率
                        {
                            //固定IP 随机域名
                            hostname = "a" + l + "." + "b" + k + "." + "c" + i;
                            PacketSent(device, BuilderPacket(hostname, srcip, dstip, dstmac, srcmac));
                        }
                    }
                }
                
            }
            else if (mode == "2")
            {
                string[] ips;
                if (dstip.IndexOf(",")!=-1)
                {
                    string[] split = new string[] { "," };
                    ips  = dstip.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    while (true)
                    {
                        for (int i = 0; i < ips.Length; i++)
                        {
                            try
                            {
                                Console.WriteLine("开始向" + ips[i] + "发送DHCP-DISCOVER");
                                PacketSent(device, BuilderPacket(hostname, srcip, ips[i], dstmac, srcmac));
                                Console.WriteLine(ips[i] + " ID:" + transid + "数据包发送完毕");
                                
                                string discoverTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                                string sql = "INSERT INTO dhcp_server_status (ser_address,trans_id,status,discover_time) VALUES ('" + ips[i] + "','" + transid + "','1','"+discoverTime+"');";
                                if (dbc.equryCommandNone(sql) > 0)
                                {
                                    Console.WriteLine("插入数据库成功");
                                }
                                
                                Thread.Sleep(1000);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                            
                        }
                        
                    }
                }
                else
                {
                    while (true)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            for (int j  = 0; j < 200; j++)
                            {
                                byte[] paaa = BuilderPacket(hostname, srcip, dstip, dstmac, srcmac);
                                PacketSent(device, paaa);
                            }
                            Thread.Sleep(100);
                        }
                        //Console.WriteLine(transid + "数据包发送完毕");
                        
                    }
                   
                }
                
                



            }
            else if (mode == "3")
            {
                for (int i = 0; i < 255; i++)
                {
                    for (int k = 0; k < 255; k++) // k代表 多少个1ms  即总个数/3
                    {
                        for (int l = 0; l < 255; l++) //小于等于3000/s 速率
                        {
                            for (int h = 0; h < 255; h++)
                            {
                                //随机IP 随机域名
                                hostname = h+"a" + l + "." + "b" + k + "d"+i+"." + "com";
                                srcip = "218." + "108." + (l + 1) + "." + (h + 1);
                                PacketSent(device, BuilderPacket(hostname, srcip, dstip, dstmac, srcmac));
                            }
                            
                        }
                    }
                }
                
            }
            else if (mode == "4")
            {
                for (int i = 0; i < 255; i++)
                {
                    for (int k = 0; k < 255; k++) // k代表 多少个1ms  即总个数/3
                    {
                        for (int l = 0; l < 255; l++) //小于等于3000/s 速率
                        {
                            for (int h = 0; h < 255; h++)
                            {
                                //随机IP 固定域名
                                srcip = "218." + "108." + (l + 1) + "." + (h + 1);
                                PacketSent(device, BuilderPacket(hostname, srcip, dstip, dstmac, srcmac));
                            }
                            
                        }
                    }
                }
                
            }
            else if (mode == "5")
            {
                FileStream fs = new FileStream("./final.txt", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string fullstring = sr.ReadToEnd();
                string[] split = new string[] { "\n" };
                string[] sname = fullstring.Split(split, StringSplitOptions.RemoveEmptyEntries);
                int repeat = 0;

                for (int i = 0; i < 255; i++)
                {
                    for (int k = 0; k < 255; k++) // k代表 多少个1ms  即总个数/3
                    {
                        for (int l = 0; l < 255; l++) //小于等于3000/s 速率
                        {
                            for (int h = 0; h < 255; h++)
                            {
                                //随机IP 固定域名
                                srcip = "218." + "108." + (l + 1) + "." + (h + 1);
                                PacketSent(device, BuilderPacket(sname[repeat], srcip, dstip, dstmac, srcmac));
                                if (repeat<499)
                                {
                                    repeat++;
                                }
                                else
                                {
                                    repeat = 0;
                                }
                            }

                        }
                    }
                }

            }

            
        }


        private static string getConfig(string key)//获取配置信息
        {
            string result = null;
            try
            {
                XmlNode rnode;
                //创建xmldocument实例并且加载Xml数据
                XmlDocument doc = new XmlDocument();
                doc.Load("./Config.xml");
                rnode = doc.SelectSingleNode("/config/pingwatch/" + key);
                result = rnode.InnerText;
                rnode = null;
                doc = null;
            }
            catch (Exception)
            {
               Console.WriteLine("配置文件读取失败。");
            }
            return result;
        }

        private static byte[] BuilderPacket(string hostname, string srcip, string dstip, string dstmac, string srcmac)
        {

            if (ranNum5>0)
            {
                ranNum5 = ranNum5 - 1;
            }
            else
            {
                ranNum5 = 255;
                if (ranNum4>0)
                {
                    ranNum4 = ranNum4 - 1;
                }
                else
                {
                    ranNum4 = 255;
                    if (ranNum3 > 0)
                    {
                        ranNum3 = ranNum3 - 1;
                    }
                    else
                    {
                        ranNum3 = 255;
                        if (ranNum2 > 0)
                        {
                            ranNum2 = ranNum2 - 1;
                        }
                        else
                        {
                            ranNum2 = 255;
                            if (ranNum1 > 0)
                            {
                                ranNum1 = ranNum1 - 1;
                            }
                            else
                            {
                                ranNum1 = 255;
                                if (ranNum > 0)
                                {
                                    ranNum = ranNum - 1;
                                }
                                else
                                {
                                    ranNum = 255;

                                }
                            }
                        }
                    }
                }
                
            }



            byte[] clientMac = new byte[6]
                {
                    byte.Parse((255-ranNum).ToString()),byte.Parse((255-ranNum1).ToString()),byte.Parse((255-ranNum2).ToString()),byte.Parse((255-ranNum3).ToString()),byte.Parse((255-ranNum4).ToString()),byte.Parse((255-ranNum5).ToString())
                };

            Random rnd = new Random();
            int t1 = 24;
            int t2 = 24;
            int t3 = 24;
            int t4 = 24;


            byte[] transactioId = new byte[4] {

                byte.Parse(ranNum2.ToString()),byte.Parse(ranNum3.ToString()),byte.Parse(ranNum4.ToString()),byte.Parse(ranNum5.ToString())
            };


            StringBuilder sbTansactionId = new StringBuilder();
            for (int i = 0; i < transactioId.Length; i++)
            {
                sbTansactionId.Append(transactioId[i].ToString("x"));
            }

             transid = sbTansactionId.ToString();

            //---------------------------------------------// questions

            string[] split = new string[] { "." };
            string[] split_mac = new string[] { "-" };
            string[] ips = srcip.Split(split, StringSplitOptions.RemoveEmptyEntries);
            byte[] bips = GetIP(ips);
            string[] ipd = dstip.Split(split, StringSplitOptions.RemoveEmptyEntries);
            byte[] bipd = GetIP(ipd);

            string[] macs = srcmac.Split(split_mac, StringSplitOptions.RemoveEmptyEntries);
            string[] macd = dstmac.Split(split_mac, StringSplitOptions.RemoveEmptyEntries);
            byte[] bmacs = GetMAC(macs);
            byte[] bmacd = GetMAC(macd);


            int totallength = hostname.Length + 2;
            byte[] qus = new byte[totallength];

            int index = 0;

            byte[] udp_attack = new byte[1000]
                {
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,
                    0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff
                };

            byte[] dhcp = new byte[548];

            byte[] dhcp_header = new byte[4]
            {
                0x01,0x01,0x06,0x00
            };
            dhcp_header.CopyTo(dhcp,0);
            transactioId.CopyTo(dhcp, 4);
            byte[] dhcp_padding = new byte[16] {

                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            };
            dhcp_padding.CopyTo(dhcp, 8);
            
            bips.CopyTo(dhcp, 24);

            clientMac.CopyTo(dhcp, 28);
            byte[] dhcp_end = new byte[212]
                {
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x63,0x82,0x53,0x63,
                    0x35,0x01,0x01,0x3d,0x07,0x01 };
            dhcp_end.CopyTo(dhcp, 34);
            clientMac.CopyTo(dhcp, 246);
                byte [] dhcp_end2 = new byte[296]
                { 0x3c,0x0a,0x48,0x5a,
                    0x43,0x4e,0x43,0x53,0x54,0x42,0x56,0x31,0x37,0x0b,0x01,0x03,0x06,0x0c,0x0f,0x1c,
                    0x28,0x29,0x2a,0x2b,0x3c,0xff,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x00,0x00,0x00,0x00
                };
            dhcp_end2.CopyTo(dhcp,252);

            int dhcplength = dhcp.Length;

            //------------------------UDP--------------------------

            index = 0;
            int udplength = dhcplength + 8;

            byte[] udp = new byte[udplength];
            dhcp.CopyTo(udp, 8);

            byte[] udp_port_src_16 = new byte[2]
            {
                0x00,0x44
            };
            udp_port_src_16.CopyTo(udp, index);
            index = index + 2;

            byte[] udp_port_dst_16 = new byte[2]
            {
                0x00,0x43
            };
            udp_port_dst_16.CopyTo(udp, index);
            index = index + 2;

            string udpll = udplength.ToString("x").PadLeft(4, '0');

            int udpll_left = Convert.ToInt32(udpll.Substring(0, 2), 16);
            int udpll_right = Convert.ToInt32(udpll.Substring(2, 2), 16);

            byte[] udp_lenght_16 = new byte[2]
            {
                byte.Parse(udpll_left.ToString()),byte.Parse(udpll_right.ToString())
            };
            udp_lenght_16.CopyTo(udp, index);
            index = index + 2;

            byte[] ip_source_32_udp_de = bips;
            byte[] ip_dst_32_udp_de = bipd;

            byte[] protocol_8_udp_de = new byte[2]
            {
                0x00,0x11
            };

            byte[] udp_lenght_16_udp_de = new byte[2]
            {
                 byte.Parse(udpll_left.ToString()),byte.Parse(udpll_right.ToString())
            };

            byte[] udp_checksum = new byte[12];
            ip_dst_32_udp_de.CopyTo(udp_checksum, 0);
            ip_source_32_udp_de.CopyTo(udp_checksum, ip_dst_32_udp_de.Length);
            protocol_8_udp_de.CopyTo(udp_checksum, ip_dst_32_udp_de.Length + ip_source_32_udp_de.Length);
            udp_lenght_16_udp_de.CopyTo(udp_checksum, ip_dst_32_udp_de.Length + ip_source_32_udp_de.Length + protocol_8_udp_de.Length);

            string checksum = GetCheckSum(udp_checksum, udp);


            byte[] udp_checksum_16 = new byte[2]
            {
                System.Convert.ToByte(checksum.Substring(0, 2), 16),System.Convert.ToByte(checksum.Substring(2, 2), 16)
            };

            udp_checksum_16.CopyTo(udp, 6);





            //--------------------------------------IP-----------------------------

            index = 0;
            byte[] ip_pack = new byte[udp.Length + 20];
            byte[] ip_header = new byte[20];


            byte[] ip_version_4_ip_header_lenght_4 = new byte[1]
            {
                0x45
            };
            ip_version_4_ip_header_lenght_4.CopyTo(ip_header, index);
            index = index + 1;


            byte[] TOS_8 = new byte[1]
            {
                0x00
            };
            TOS_8.CopyTo(ip_header, index);
            index = index + 1;

            string ipll = (udp.Length + 20).ToString("x").PadLeft(4, '0');

            int ipll_left = Convert.ToInt32(ipll.Substring(0, 2), 16);
            int ipll_right = Convert.ToInt32(ipll.Substring(2, 2), 16);



            byte[] total_lenght_16 = new byte[2]
            {
                byte.Parse(ipll_left.ToString()),byte.Parse(ipll_right.ToString())
            };
            total_lenght_16.CopyTo(ip_header, index);
            index = index + 2;

            byte[] identification_16 = new byte[2]
            {
                0x09,0x72
            };
            identification_16.CopyTo(ip_header, index);
            index = index + 2;


            byte[] flag_3_frame_offset_13 = new byte[2]
            {
                0x00,0x00
            };
            flag_3_frame_offset_13.CopyTo(ip_header, index);
            index = index + 2;


            byte[] ttl_8 = new byte[1]
            {
                0x80
            };
            ttl_8.CopyTo(ip_header, index);
            index = index + 1;

            byte[] protocol_8 = new byte[1]
            {
                0x11
            };
            protocol_8.CopyTo(ip_header, index);
            index = index + 1;


            byte[] headerchecksum_16 = new byte[2]
            {
                0x00,0x00
            };
            headerchecksum_16.CopyTo(ip_header, index);
            index = index + 2;


            byte[] ip_source_32 = bips;
            ip_source_32.CopyTo(ip_header, index);
            index = index + 4;


            byte[] ip_dst_32 = bipd;
            ip_dst_32.CopyTo(ip_header, index);

            string ipchecksum = GetIPCheckSum(ip_header);

            byte[] ip_checksum_16 = new byte[2]
            {
                System.Convert.ToByte(ipchecksum.Substring(0, 2), 16),System.Convert.ToByte(ipchecksum.Substring(2, 2), 16)
            };

            ip_checksum_16.CopyTo(ip_header, 10);

            ip_header.CopyTo(ip_pack, 0);
            udp.CopyTo(ip_pack, 20);


            //-----------------------------------Eth-----------------------------------

            index = 0;
            byte[] finalpacket = new byte[ip_pack.Length + 22];


            byte[] mac_dst_48 = bmacd;
            mac_dst_48.CopyTo(finalpacket, index);
            index = index + 6;

            byte[] mac_src_48 = clientMac;
            mac_src_48.CopyTo(finalpacket, index);
            index = index + 6;

            byte[] vlan_type = new byte[2]
            {
                0x81,0x00
            };


            vlan_type.CopyTo(finalpacket, index);
            index = index + 2;


            byte[] vlan_outer = new byte[2] {
                0x03,0x84
            };

            vlan_outer.CopyTo(finalpacket, index);
            index = index + 2;


            vlan_type.CopyTo(finalpacket, index);
            index = index + 2;

            string vlan_inner_s = (ranNum5 + ranNum4 + ranNum3 + ranNum2 + ranNum1 + ranNum).ToString("x").PadLeft(4, '0');

            int vlan_inner_left = Convert.ToInt32(vlan_inner_s.Substring(0, 2), 16);
            int vlan_inner_right = Convert.ToInt32(vlan_inner_s.Substring(2, 2), 16);


            byte[] vlan_inner = new byte[2] {
                byte.Parse(vlan_inner_left.ToString()),byte.Parse(vlan_inner_right.ToString())
            };

            vlan_inner.CopyTo(finalpacket, index);
            index = index + 2;

            



            byte[] eth_type_16 = new byte[2]
            {
                0x08,0x00
            };
            eth_type_16.CopyTo(finalpacket, index);
            index = index + 2;
            ip_pack.CopyTo(finalpacket, index);
            return finalpacket;
        }

        private static byte[] GetMAC(string[] macd)
        {
            

            byte[] result = new byte[6]
                {
                    System.Convert.ToByte(macd[0], 16),System.Convert.ToByte(macd[1], 16),System.Convert.ToByte(macd[2], 16),System.Convert.ToByte(macd[3], 16),System.Convert.ToByte(macd[4], 16),System.Convert.ToByte(macd[5], 16)
                };

            return result;
        }

        private static byte[] GetIP(string[] ipd)
        {
            
            string[] tmp = new string[4];
            for (int i = 0; i < ipd.Length; i++)
            {
                tmp[i] = System.Convert.ToString(System.Convert.ToInt32(ipd[i], 10),16) ;
            }
            byte[] result = new byte[4]
                {
                    System.Convert.ToByte(tmp[0],16),System.Convert.ToByte(tmp[1],16),System.Convert.ToByte(tmp[2],16),System.Convert.ToByte(tmp[3],16)
                };

            return result;
        }

        private static string GetIPCheckSum(byte[] final_pack)
        {
            


            int final = 0;
            for (int i = 0; i < final_pack.Length / 2; i++)
            {
                string tmp1 = System.Convert.ToString(final_pack[i * 2], 16);
                string tmp2 = System.Convert.ToString(final_pack[i * 2 + 1], 16);

                if (tmp1.Length == 1)
                {
                    tmp1 = "0" + tmp1;
                }

                if (tmp2.Length == 1)
                {
                    tmp2 = "0" + tmp2;
                }

                string tmp = tmp1 + tmp2;




                int tt = Convert.ToInt32(tmp, 16);

                final = final + tt;



            }
            string fds = System.Convert.ToString(final, 16);
            if (fds.Length > 4)
            {
                int fds1 = System.Convert.ToInt32(fds.Substring(1, 4), 16);
                int fds2 = System.Convert.ToInt32(fds.Substring(0, 1), 16);
                final = fds1 + fds2;
            }
            int final1 = ~final;
            string fd = System.Convert.ToString(final1, 16).Substring(4, 4);

            return fd;
        }

        private static string GetCheckSum(byte[] udp_checksum,byte[] qus)
        {
            byte[] final_pack; 
            

            if (qus.Length%2!=0)
            {
                final_pack = new byte[qus.Length + 13];
                byte[] buffer = new byte[1]
                {
                    0x00
                };
                qus.CopyTo(final_pack, 0);
                buffer.CopyTo(final_pack, qus.Length);
                udp_checksum.CopyTo(final_pack, qus.Length + 1);
            }
            else
            {
                final_pack = new byte[qus.Length+12];
                qus.CopyTo(final_pack, 0);
                udp_checksum.CopyTo(final_pack, qus.Length);
            }



            int final = 0;
            for (int i = 0; i < final_pack.Length / 2; i++)
            {
                string tmp1 = System.Convert.ToString(final_pack[i * 2], 16);
                string tmp2 = System.Convert.ToString(final_pack[i * 2 + 1], 16);

                if (tmp1.Length==1)
                {
                    tmp1 = "0" + tmp1;
                }

                if (tmp2.Length==1)
                {
                    tmp2 = "0" + tmp2;
                }

                string tmp = tmp1 + tmp2;




                int tt = Convert.ToInt32(tmp, 16);


                final = final + tt;
                
                
                
            }
            string fds = System.Convert.ToString(final, 16);
            if (fds.Length>4)
            {
                int fds1 = System.Convert.ToInt32(fds.Substring(1, 4), 16);
                int fds2 = System.Convert.ToInt32(fds.Substring(0, 1), 16);
                final = fds1 + fds2;
            }
            int final1 = ~final;
            string fd = System.Convert.ToString(final1, 16).Substring(4,4);

            return fd;
        }

        private static void PacketSent(ICaptureDevice device, byte[] packetx)
        {
            device.Open();
            packetx.CopyTo(packetx, 0);
            device.SendPacket(packetx);
        }

        static byte[] GetBytes(string str)
        {
            byte[] sd = System.Text.Encoding.ASCII.GetBytes(str);

            return sd;
        }
    }
}
