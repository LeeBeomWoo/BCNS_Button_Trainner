using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using BCNS_Button_Trainner;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BCNS
{
    public class Connect
    {
        Person person = new Person();
        string OutCategory;
        public string ftpUrl = "ftp://bcns.iptime.org:8620";
        
        public Person Post(Person data)
        {
            // 타겟이 되는 웹페이지 URL
            string strUrl = "http://localhost:8080/BCNS_SERVER-09_27_complete/board/Login.jsp";
            StringBuilder postParams = new StringBuilder();
            postParams.Append("Id=" + data.Id);

            Encoding encoding = Encoding.UTF8;
            byte[] result = encoding.GetBytes(postParams.ToString());


            HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(strUrl);

            // HttpWebRequest 오브젝트 설정
            wReqFirst.Method = "POST";
            wReqFirst.ContentType = "application/x-www-form-urlencoded";
            wReqFirst.ContentLength = result.Length;

            Stream postDataStream = wReqFirst.GetRequestStream();
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();

            HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();

            // Response의 결과를 스트림을 생성합니다.
            Stream respPostStream = wRespFirst.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            // 생성한 스트림으로부터 string으로 변환합니다.
            string resultPost = readerPost.ReadToEnd();
            Console.WriteLine("strResult =" + resultPost);
            Console.WriteLine("Result =" + wRespFirst.StatusDescription);
            Person m = JsonConvert.DeserializeObject<Person>(resultPost);
            return m;
        }

        public Person LoginPost(Person data)
        {
            Person m = new Person();
            try
            {
                // 타겟이 되는 웹페이지 URL
                string strUrl = "http://localhost:8080/BCNS_SERVER-09_27_complete/board/Login.jsp";
                StringBuilder postParams = new StringBuilder();
                postParams.Append("Id=" + data.Id);
                postParams.Append("&PassWord=" + data.PassWord);

                Encoding encoding = Encoding.UTF8;
                byte[] result = encoding.GetBytes(postParams.ToString());


                HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(strUrl);

                // HttpWebRequest 오브젝트 설정
                wReqFirst.Method = "POST";
                wReqFirst.ContentType = "application/x-www-form-urlencoded";
                wReqFirst.ContentLength = result.Length;

                Stream postDataStream = wReqFirst.GetRequestStream();

                postDataStream.Write(result, 0, result.Length);
                postDataStream.Close();

                HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();
                Console.WriteLine(wRespFirst.StatusDescription);

                // Response의 결과를 스트림을 생성합니다.
                Stream respPostStream = wRespFirst.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                // 생성한 스트림으로부터 string으로 변환합니다.
                string resultPost = readerPost.ReadToEnd();
                Console.WriteLine("strResult =" + resultPost);
                Console.WriteLine("Result =" + wRespFirst.StatusDescription);
                m = JsonConvert.DeserializeObject<Person>(resultPost);
            }
            catch (WebException e)
            {
                Console.WriteLine("This program is expected to throw WebException on successful run." +
                                    "\n\nException Message :" + e.Message);
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally {
            }
            return m;
        }
        public string JoinPut(Person uploaddata)
        {

            // 타겟이 되는 웹페이지 URL
            string strUrl = "http://localhost:8080/BCNS_SERVER-09_27_complete/board/ReceiveJoin.jsp";
            StringBuilder postParams = new StringBuilder();
            postParams.Append("Id=" + uploaddata.Id);
            postParams.Append("&Name=" + uploaddata.Name);
            postParams.Append("&Age=" + uploaddata.Age);
            postParams.Append("&Sex=" + uploaddata.Sex);
            postParams.Append("&PassWord=" + uploaddata.PassWord);
            postParams.Append("&HomePage=" + uploaddata.HomePage);
            postParams.Append("&NickName=" + uploaddata.NickName);
            postParams.Append("&Email=" + uploaddata.Email);
            postParams.Append("&Email_open=" + uploaddata.Email_open);
            postParams.Append("&Call=" + uploaddata.Call);
            postParams.Append("&Call_open=" + uploaddata.Call_open);
            postParams.Append("&License=" + uploaddata.License);
            postParams.Append("&License_1=" + uploaddata.License_1);
            postParams.Append("&Address=" + uploaddata.Address);
            postParams.Append("&FaceImage=" + uploaddata.FaceImage);
            postParams.Append("&BodyImage=" + uploaddata.BodyImage);
            postParams.Append("&Award=" + uploaddata.Award);
            postParams.Append("&Award_1=" + uploaddata.Award_1);
            postParams.Append("&Permission=" + uploaddata.Permission);
            postParams.Append("&Award_AA=" + uploaddata.Award_AA);
            postParams.Append("&Award_1_AA=" + uploaddata.Award_1_AA);
            postParams.Append("&License_AA=" + uploaddata.License_AA);
            postParams.Append("&License_1_AA=" + uploaddata.License_1_AA);
            postParams.Append("&Award_Image=" + uploaddata.Award_Image);
            postParams.Append("&Award_1_Image=" + uploaddata.Award_1_Image);
            postParams.Append("&License_Image=" + uploaddata.License_Image);
            postParams.Append("&License_1_Image=" + uploaddata.License_1_Image);
            postParams.Append("&Section=" + uploaddata.Section);

            Encoding encoding = Encoding.UTF8;
            byte[] result = encoding.GetBytes(postParams.ToString());


            HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(strUrl);

            // HttpWebRequest 오브젝트 설정
            wReqFirst.Method = "POST";
            wReqFirst.ContentType = "application/x-www-form-urlencoded";
            wReqFirst.ContentLength = result.Length;

            Stream postDataStream = wReqFirst.GetRequestStream();
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();

            HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();

            // Response의 결과를 스트림을 생성합니다.
            Stream respPostStream = wRespFirst.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            // 생성한 스트림으로부터 string으로 변환합니다.
            string resultPost = readerPost.ReadToEnd();
            Console.WriteLine("strResult =" + resultPost);
            return resultPost;
        }
        public string MainPut(UpLoadD uploaddata)
        {

            switch (uploaddata.OutCategory)
            {
                case "0-0":
                    OutCategory = "상체 골격";
                    break;
                case "0-1":
                    OutCategory = "상체 근육";
                    break;
                case "0-2":
                    OutCategory = "하체 골격";
                    break;
                case "0-3":
                    OutCategory = "상체 근육";
                    break;
                case "1-0":
                    OutCategory = "운동 방법";
                    break;
                case "1-1":
                    OutCategory = "스트레칭";
                    break;
                case "1-2":
                    OutCategory = "셀프마사지";
                    break;
                case "2-0":
                    OutCategory = "상체 운동";
                    break;
                case "2-1":
                    OutCategory = "하체 운동";
                    break;
                case "2-3":
                    OutCategory = "상체 골격";
                    break;
                case "3-0":
                    OutCategory = "다이어트식";
                    break;
                case "3-1":
                    OutCategory = "체력강화식";
                    break;
                case "3-2":
                    OutCategory = "근육증가식";
                    break;
                case "3-3":
                    OutCategory = "건강한 음식";
                    break;
            }

            // 타겟이 되는 웹페이지 URL
            string strUrl = "http://localhost:8080/BCNS_SERVER-09_27_complete/board/UpLoad.jsp";
            StringBuilder postParams = new StringBuilder();
            postParams.Append("Id=" + uploaddata.Id);
            postParams.Append("&Category=" + uploaddata.Category);
            postParams.Append("&ImageUrl=" + uploaddata.ImageUrl);
            postParams.Append("&Table=" + uploaddata.Table);
            postParams.Append("&Title=" + uploaddata.Title);
            postParams.Append("&Content=" + uploaddata.Content);
            postParams.Append("&ListImage=" + uploaddata.ListImage);
            postParams.Append("&OutCategory=" + OutCategory);

             Encoding encoding = Encoding.UTF8;
            byte[] result = encoding.GetBytes(postParams.ToString());


            HttpWebRequest wReqFirst = (HttpWebRequest)WebRequest.Create(strUrl);

            // HttpWebRequest 오브젝트 설정
            wReqFirst.Method = "POST";
            wReqFirst.ContentType = "application/x-www-form-urlencoded";
            wReqFirst.ContentLength = result.Length;

            Stream postDataStream = wReqFirst.GetRequestStream();
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();

            HttpWebResponse wRespFirst = (HttpWebResponse)wReqFirst.GetResponse();

            // Response의 결과를 스트림을 생성합니다.
            Stream respPostStream = wRespFirst.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            // 생성한 스트림으로부터 string으로 변환합니다.
            string resultPost = readerPost.ReadToEnd();
            Console.WriteLine("strResult =" + resultPost);
            return resultPost;
        }

        }
    
}