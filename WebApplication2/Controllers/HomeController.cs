using GatesProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        static bool flag = true;
        public ActionResult Index()
        {
            GatesCorpEntities db = new GatesCorpEntities();
            List<device> device = db.device.ToList();
            List<member> members = db.member.ToList();
            List<Arduino> Arduino = db.Arduino.ToList();
            List<object> model = new List<object>();
            model.Add(device);
            model.Add(members);
            model.Add(Arduino);
            if(flag == true)
            {
                CheckDevice();
                flag = false;
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateDeviceInfo(int deviceId, string deviceName, string deviceWifi, string devicePassword, string deviceIp, string level, string frequency)
        {
            GatesCorpEntities db = new GatesCorpEntities();
            device deviceInfo = new device();
            device device = db.device.Find(deviceId);
            device.deviceName = deviceName;
            device.deviceIp = deviceIp;
            device.wifiName = deviceWifi;
            device.wifiPassword = devicePassword;
            device.waterLevel = level;
            device.frequency = frequency;
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateMember(string listMember, int selectedBoilerID, string subject, string message, string mailAddress, string mailPassword)
        {
            GatesCorpEntities db = new GatesCorpEntities();
            List<member> members = db.member.ToList();
            foreach (var item in members)
            {
                if (item.deviceID == selectedBoilerID)
                {
                    db.member.Remove(item);
                }
            }

            string[] strArr = null;
            char[] splitchar = { '-' };
            strArr = listMember.Split(splitchar);
            int count = 0;
            for (count = 0; count <= strArr.Length - 2; count++)
            {
                member member = new member();
                member.deviceID = selectedBoilerID;
                member.member1 = strArr[count];
                db.member.Add(member);
                db.SaveChanges();
            }

            device device = db.device.Find(selectedBoilerID);
            device.subject = subject;
            device.message = message;
            device.mailPassword = mailPassword;
            device.mailAddress = mailAddress;
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddDevice(string deviceName, string wifiName, string wifiPassword)
        {
            GatesCorpEntities db = new GatesCorpEntities();
            device deviceInfo = new device();

            deviceInfo.deviceName = deviceName;
            deviceInfo.wifiName = wifiName;
            deviceInfo.wifiPassword = wifiPassword;
            deviceInfo.deviceIp = "0.0.0.0";
            deviceInfo.message = "Sample Message";
            deviceInfo.subject = "Subject";
            deviceInfo.mailAddress = "example@gmail.com";
            deviceInfo.mailPassword = "root";
            deviceInfo.status = "STOPED";
            deviceInfo.waterLevel = "100";
            deviceInfo.frequency = "3600";
            db.device.Add(deviceInfo);
            db.SaveChanges();

            List<device> device = db.device.ToList();
            foreach (var item in device)
            {
                if (item.deviceName == deviceName)
                {
                    member member = new member();
                    member.deviceID = item.deviceID;
                    member.member1 = "example@example.com";
                    db.member.Add(member);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveDevice(int deviceId)
        {
            GatesCorpEntities db = new GatesCorpEntities();
            device deviceInfo = new device();
            device device = db.device.Find(deviceId);

            db.device.Remove(device);
            db.SaveChanges();

            List<member> members = db.member.ToList();
            foreach (var item in members)
            {
                if (item.deviceID == deviceId)
                {
                    var rm = db.member.Where(u => u.deviceID == deviceId).FirstOrDefault();
                    db.member.Remove(rm);
                    db.SaveChanges();
                }
            }

            List<Arduino> arduino = db.Arduino.Where(x => x.deviceID == deviceId).ToList();
            foreach(var item in arduino)
            {
                db.Arduino.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult GetValues(int deviceID, int waterLevel)
        {
            GatesCorpEntities db = new GatesCorpEntities();
            Arduino arduino = new Arduino();
            int level = Int32.Parse(db.device.Find(deviceID).waterLevel);
            arduino.deviceID = deviceID;
            arduino.waterLevel = waterLevel;
            var time = DateTime.Now;
            string formattedDate = time.ToString("dd/MM/yyyy");
            string formattedTime = time.ToString("hh:mm:ss");
            arduino.time = formattedTime;
            arduino.date = formattedDate;
            db.Arduino.Add(arduino);
            db.SaveChanges();

            if (waterLevel > level)
            {
                SendMail(deviceID);
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetDeviceIp(int deviceID, int status, string deviceIp)
        {
            string a = Request.Params["deviceIp"];
            GatesCorpEntities db = new GatesCorpEntities();
            device device = db.device.Find(deviceID);
            deviceIp = deviceIp.Replace("-", ".");
            device.deviceIp = deviceIp;
            if(status == 1)
            {
                device.status = "LIVE";
            }
            else
            {
                device.status = "STOPED";
            }
            
            db.Entry(device).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void CheckDevice()
        {
            var waitHandle = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                // Method to execute
                (state, timeout) =>
                {

                    GatesCorpEntities db = new GatesCorpEntities();
                    List<device> device = db.device.ToList();
                    foreach (var item in device)
                    {
                        var time = DateTime.Now;
                        string formattedDate = time.ToString("dd/MM/yyyy");
                        string formattedTime = time.ToString("hh:mm:ss");
                        List<Arduino> arduino = db.Arduino.Where(x => x.deviceID == item.deviceID).ToList();
                        var lastLog = arduino.OrderByDescending(x => DateTime.ParseExact(x.date, "dd/MM/yyyy", null)).
                        ThenByDescending(x => DateTime.ParseExact(x.time, "hh:mm:ss", null)).Take(1);

                        foreach (var i in lastLog)
                        {
                            if (i.date.Equals(formattedDate))
                            {
                                item.status = "LIVE";
                            }
                            else
                            {
                                item.status = "STOPED";
                            }
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }

                },
                // optional state object to pass to the method
                null,
                // Execute the method after 5 seconds
                //TimeSpan.FromHours(1),
                TimeSpan.FromSeconds(10),
                // Set this to false to execute it repeatedly every 5 seconds
                false
            );
        }

        public void SendMail(int deviceID)
        {
            GatesCorpEntities db = new GatesCorpEntities();
            device device = db.device.Find(deviceID);
            List<member> member = db.member.ToList();

            MailMessage mail = new MailMessage();
            foreach (var mem in member)
            {
                if (mem.deviceID.Equals(deviceID))
                    mail.To.Add(mem.member1);
            }
            mail.From = new MailAddress(device.mailAddress, device.subject, System.Text.Encoding.UTF8);
            mail.Subject = device.subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = device.message;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(device.mailAddress, device.mailPassword);
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }
            }
        }
    }
}